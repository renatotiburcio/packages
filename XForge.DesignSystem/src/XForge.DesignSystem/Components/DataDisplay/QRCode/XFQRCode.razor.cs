using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.QRCode;

public partial class XFQRCode : XForgeComponentBase
{
    private static readonly QRVersion[] Versions =
    {
        new(1, 19, 7, 0, Array.Empty<int>()),
        new(2, 34, 10, 7, new[] { 6, 18 }),
        new(3, 55, 15, 7, new[] { 6, 22 }),
        new(4, 80, 20, 7, new[] { 6, 26 })
    };

    [Parameter] public string? Value { get; set; }
    [Parameter] public int ModuleSize { get; set; } = 8;
    [Parameter] public int QuietZone { get; set; } = 4;
    [Parameter] public bool ShowText { get; set; }
    [Parameter] public string ForegroundColor { get; set; } = "#111827";
    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";
    [Parameter] public string EmptyText { get; set; } = "Informe um texto curto para gerar o codigo QR.";
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public RenderFragment? CenterContent { get; set; }
    [Parameter] public bool ShowDownload { get; set; }
    [Parameter] public string DownloadText { get; set; } = "Baixar SVG";
    [Parameter] public string? DownloadFileName { get; set; }
    [Parameter] public string AriaLabelFormat { get; set; } = "Codigo QR {0}";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Value, ModuleSize, QuietZone, ShowText, ForegroundColor, BackgroundColor, EmptyText, AriaLabel, CenterContent, ShowDownload, DownloadText, DownloadFileName, AriaLabelFormat);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

    private bool[,]? matrix;
    private IReadOnlyList<Module> darkModules = Array.Empty<Module>();

    private int MatrixSize => matrix?.GetLength(0) ?? 0;
    private int SafeModuleSize => Math.Clamp(ModuleSize, 1, 32);
    private int SafeQuietZone => Math.Clamp(QuietZone, 0, 16);
    private int SvgSize => (MatrixSize + SafeQuietZone * 2) * SafeModuleSize;
    private string ViewBox => string.Create(CultureInfo.InvariantCulture, $"{0} {0} {SvgSize} {SvgSize}");
    private string ResolvedAriaLabel => AriaLabel ?? string.Format(CultureInfo.CurrentCulture, AriaLabelFormat, Value);
    private string RootClass => GetClass("xf-qrcode inline-block w-full max-w-56 rounded-lg bg-white p-3 text-surface-950 ring-1 ring-border-light");
    private string CenterOverlayClass => "pointer-events-none absolute left-1/2 top-1/2 grid h-10 w-10 -translate-x-1/2 -translate-y-1/2 place-items-center rounded-lg bg-white p-1.5 text-surface-950 shadow-[0_1px_4px_rgb(15_23_42/.18)] ring-4 ring-white";
    private string DownloadClass => "mt-3 inline-flex w-full items-center justify-center rounded-md border border-border-light bg-bg-surface px-3 py-1.5 text-xs font-semibold text-primary transition-colors hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";
    private string ResolvedDownloadFileName => string.IsNullOrWhiteSpace(DownloadFileName) ? "qrcode.svg" : DownloadFileName!;
    private string DownloadHref => matrix is null ? "" : $"data:image/svg+xml;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes(BuildSvgMarkup()))}";

    private IReadOnlyList<Module> DarkModules => darkModules;

    protected override void OnParametersSet()
    {
        matrix = Encode(Value);
        darkModules = BuildDarkModules(matrix);
    }

    private IReadOnlyList<Module> BuildDarkModules(bool[,]? source)
    {
        if (source is null)
        {
            return Array.Empty<Module>();
        }

        var size = source.GetLength(0);
        var modules = new List<Module>();
        for (var row = 0; row < size; row++)
        {
            for (var column = 0; column < size; column++)
            {
                if (source[row, column])
                {
                    modules.Add(new Module((column + SafeQuietZone) * SafeModuleSize, (row + SafeQuietZone) * SafeModuleSize));
                }
            }
        }

        return modules;
    }

    private string BuildSvgMarkup()
    {
        var builder = new StringBuilder();
        builder.Append(CultureInfo.InvariantCulture, $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"{ViewBox}\" shape-rendering=\"crispEdges\">");
        builder.Append(CultureInfo.InvariantCulture, $"<rect width=\"{SvgSize}\" height=\"{SvgSize}\" fill=\"{BackgroundColor}\"/>");
        foreach (var module in DarkModules)
        {
            builder.Append(CultureInfo.InvariantCulture, $"<rect x=\"{module.X}\" y=\"{module.Y}\" width=\"{SafeModuleSize}\" height=\"{SafeModuleSize}\" fill=\"{ForegroundColor}\"/>");
        }

        builder.Append("</svg>");
        return builder.ToString();
    }

    private bool[,]? Encode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var bytes = Encoding.UTF8.GetBytes(value);
        var version = Versions.FirstOrDefault(candidate => bytes.Length <= candidate.ByteCapacity);
        if (version is null)
        {
            return null;
        }

        var data = BuildDataCodewords(bytes, version.DataCodewords);
        var ecc = ReedSolomon(data, version.EccCodewords);
        var allCodewords = data.Concat(ecc).ToArray();
        var matrix = new QRMatrix(version.Size);

        DrawFunctionPatterns(matrix, version);
        DrawData(matrix, allCodewords);
        var mask = ChooseBestMask(matrix);
        ApplyMask(matrix, mask);
        DrawFormatBits(matrix, mask);

        return matrix.Modules;
    }

    private static byte[] BuildDataCodewords(byte[] bytes, int totalCodewords)
    {
        var bits = new List<bool>(totalCodewords * 8);
        AppendBits(bits, 0b0100, 4);
        AppendBits(bits, bytes.Length, 8);
        foreach (var item in bytes)
        {
            AppendBits(bits, item, 8);
        }

        var terminator = Math.Min(4, totalCodewords * 8 - bits.Count);
        AppendBits(bits, 0, terminator);
        while (bits.Count % 8 != 0)
        {
            bits.Add(false);
        }

        var codewords = BitsToBytes(bits);
        var pads = new byte[] { 0xec, 0x11 };
        var padIndex = 0;
        while (codewords.Count < totalCodewords)
        {
            codewords.Add(pads[padIndex++ % pads.Length]);
        }

        return codewords.ToArray();
    }

    private static byte[] ReedSolomon(byte[] data, int eccLength)
    {
        var generator = BuildGenerator(eccLength);
        var message = new byte[data.Length + eccLength];
        Array.Copy(data, message, data.Length);

        for (var index = 0; index < data.Length; index++)
        {
            var factor = message[index];
            if (factor == 0)
            {
                continue;
            }

            for (var generatorIndex = 0; generatorIndex < generator.Length; generatorIndex++)
            {
                message[index + generatorIndex] ^= GaloisMultiply(factor, generator[generatorIndex]);
            }
        }

        var ecc = new byte[eccLength];
        Array.Copy(message, data.Length, ecc, 0, eccLength);
        return ecc;
    }

    private static byte[] BuildGenerator(int degree)
    {
        var generator = new List<byte> { 1 };
        for (var index = 0; index < degree; index++)
        {
            generator = PolynomialMultiply(generator, new List<byte> { 1, GaloisPow(2, index) });
        }

        return generator.ToArray();
    }

    private static List<byte> PolynomialMultiply(IReadOnlyList<byte> left, IReadOnlyList<byte> right)
    {
        var result = new byte[left.Count + right.Count - 1];
        for (var leftIndex = 0; leftIndex < left.Count; leftIndex++)
        {
            for (var rightIndex = 0; rightIndex < right.Count; rightIndex++)
            {
                result[leftIndex + rightIndex] ^= GaloisMultiply(left[leftIndex], right[rightIndex]);
            }
        }

        return new List<byte>(result);
    }

    private static byte GaloisMultiply(byte left, byte right)
    {
        var result = 0;
        while (right > 0)
        {
            if ((right & 1) != 0)
            {
                result ^= left;
            }

            right >>= 1;
            left = (byte)((left << 1) ^ ((left & 0x80) != 0 ? 0x11d : 0));
        }

        return (byte)(result & 0xff);
    }

    private static byte GaloisPow(byte value, int exponent)
    {
        byte result = 1;
        for (var index = 0; index < exponent; index++)
        {
            result = GaloisMultiply(result, value);
        }

        return result;
    }

    private static void DrawFunctionPatterns(QRMatrix matrix, QRVersion version)
    {
        DrawFinder(matrix, 0, 0);
        DrawFinder(matrix, version.Size - 7, 0);
        DrawFinder(matrix, 0, version.Size - 7);

        for (var index = 8; index < version.Size - 8; index++)
        {
            matrix.SetReserved(6, index, index % 2 == 0);
            matrix.SetReserved(index, 6, index % 2 == 0);
        }

        foreach (var row in version.AlignmentPositions)
        {
            foreach (var column in version.AlignmentPositions)
            {
                if (matrix.IsReserved(row, column))
                {
                    continue;
                }

                DrawAlignment(matrix, column - 2, row - 2);
            }
        }

        matrix.SetReserved(version.Size - 8, 8, true);
        ReserveFormatAreas(matrix);
    }

    private static void DrawFinder(QRMatrix matrix, int left, int top)
    {
        for (var row = -1; row <= 7; row++)
        {
            for (var column = -1; column <= 7; column++)
            {
                var y = top + row;
                var x = left + column;
                if (!matrix.Contains(y, x))
                {
                    continue;
                }

                var dark = row is >= 0 and <= 6
                    && column is >= 0 and <= 6
                    && (row is 0 or 6 || column is 0 or 6 || row is >= 2 and <= 4 && column is >= 2 and <= 4);
                matrix.SetReserved(y, x, dark);
            }
        }
    }

    private static void DrawAlignment(QRMatrix matrix, int left, int top)
    {
        for (var row = 0; row < 5; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                var dark = row is 0 or 4 || column is 0 or 4 || row == 2 && column == 2;
                matrix.SetReserved(top + row, left + column, dark);
            }
        }
    }

    private static void ReserveFormatAreas(QRMatrix matrix)
    {
        for (var index = 0; index < 9; index++)
        {
            if (index != 6)
            {
                matrix.SetReserved(8, index, false);
                matrix.SetReserved(index, 8, false);
            }
        }

        for (var index = 0; index < 8; index++)
        {
            matrix.SetReserved(8, matrix.Size - 1 - index, false);
            matrix.SetReserved(matrix.Size - 1 - index, 8, false);
        }
    }

    private static void DrawData(QRMatrix matrix, byte[] codewords)
    {
        var bitIndex = 0;
        var upward = true;
        for (var right = matrix.Size - 1; right >= 1; right -= 2)
        {
            if (right == 6)
            {
                right--;
            }

            for (var vertical = 0; vertical < matrix.Size; vertical++)
            {
                var row = upward ? matrix.Size - 1 - vertical : vertical;
                for (var offset = 0; offset < 2; offset++)
                {
                    var column = right - offset;
                    if (matrix.IsReserved(row, column))
                    {
                        continue;
                    }

                    if (bitIndex < codewords.Length * 8)
                    {
                        matrix.Set(row, column, ((codewords[bitIndex >> 3] >> (7 - (bitIndex & 7))) & 1) != 0);
                        bitIndex++;
                    }
                }
            }

            upward = !upward;
        }
    }

    private static int ChooseBestMask(QRMatrix matrix)
    {
        var bestMask = 0;
        var bestPenalty = int.MaxValue;
        for (var mask = 0; mask < 8; mask++)
        {
            var clone = matrix.CloneModules();
            ApplyMask(clone, matrix, mask);
            var penalty = CalculatePenalty(clone);
            if (penalty < bestPenalty)
            {
                bestPenalty = penalty;
                bestMask = mask;
            }
        }

        return bestMask;
    }

    private static void ApplyMask(QRMatrix matrix, int mask)
    {
        ApplyMask(matrix.Modules, matrix, mask);
    }

    private static void ApplyMask(bool[,] modules, QRMatrix matrix, int mask)
    {
        for (var row = 0; row < matrix.Size; row++)
        {
            for (var column = 0; column < matrix.Size; column++)
            {
                if (!matrix.IsReserved(row, column) && ShouldMask(mask, row, column))
                {
                    modules[row, column] = !modules[row, column];
                }
            }
        }
    }

    private static bool ShouldMask(int mask, int row, int column) => mask switch
    {
        0 => ((row + column) & 1) == 0,
        1 => (row & 1) == 0,
        2 => column % 3 == 0,
        3 => (row + column) % 3 == 0,
        4 => (((row / 2) + (column / 3)) & 1) == 0,
        5 => (row * column) % 2 + (row * column) % 3 == 0,
        6 => (((row * column) % 2 + (row * column) % 3) & 1) == 0,
        7 => (((row + column) % 2 + (row * column) % 3) & 1) == 0,
        _ => false
    };

    private static int CalculatePenalty(bool[,] modules)
    {
        var size = modules.GetLength(0);
        var total = 0;

        for (var row = 0; row < size; row++)
        {
            var run = 1;
            for (var column = 1; column < size; column++)
            {
                if (modules[row, column] == modules[row, column - 1])
                {
                    run++;
                }
                else
                {
                    if (run >= 5)
                    {
                        total += 3 + run - 5;
                    }

                    run = 1;
                }
            }

            if (run >= 5)
            {
                total += 3 + run - 5;
            }
        }

        for (var column = 0; column < size; column++)
        {
            var run = 1;
            for (var row = 1; row < size; row++)
            {
                if (modules[row, column] == modules[row - 1, column])
                {
                    run++;
                }
                else
                {
                    if (run >= 5)
                    {
                        total += 3 + run - 5;
                    }

                    run = 1;
                }
            }

            if (run >= 5)
            {
                total += 3 + run - 5;
            }
        }

        for (var row = 0; row < size - 1; row++)
        {
            for (var column = 0; column < size - 1; column++)
            {
                if (modules[row, column] == modules[row, column + 1]
                    && modules[row, column] == modules[row + 1, column]
                    && modules[row, column] == modules[row + 1, column + 1])
                {
                    total += 3;
                }
            }
        }

        var patterns = new[]
        {
            new[] { true, false, true, true, true, false, true, false, false, false, false },
            new[] { false, false, false, false, true, false, true, true, true, false, true }
        };

        foreach (var pattern in patterns)
        {
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column <= size - pattern.Length; column++)
                {
                    if (MatchesRow(modules, row, column, pattern))
                    {
                        total += 40;
                    }
                }
            }

            for (var column = 0; column < size; column++)
            {
                for (var row = 0; row <= size - pattern.Length; row++)
                {
                    if (MatchesColumn(modules, row, column, pattern))
                    {
                        total += 40;
                    }
                }
            }
        }

        var dark = 0;
        for (var row = 0; row < size; row++)
        {
            for (var column = 0; column < size; column++)
            {
                if (modules[row, column])
                {
                    dark++;
                }
            }
        }

        var percent = (dark * 100 + size * size / 2) / (size * size);
        return total + Math.Abs(percent - 50) / 5 * 10;
    }

    private static bool MatchesRow(bool[,] modules, int row, int column, IReadOnlyList<bool> pattern)
    {
        for (var index = 0; index < pattern.Count; index++)
        {
            if (modules[row, column + index] != pattern[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool MatchesColumn(bool[,] modules, int row, int column, IReadOnlyList<bool> pattern)
    {
        for (var index = 0; index < pattern.Count; index++)
        {
            if (modules[row + index, column] != pattern[index])
            {
                return false;
            }
        }

        return true;
    }

    private static void DrawFormatBits(QRMatrix matrix, int mask)
    {
        const int eccLow = 1;
        var bits = CalculateFormatBits((eccLow << 3) | (mask & 7));
        bool GetFormatBit(int index) => ((bits >> (14 - index)) & 1) != 0;

        for (var index = 0; index <= 5; index++)
        {
            matrix.SetReserved(8, index, GetFormatBit(index));
        }

        matrix.SetReserved(8, 7, GetFormatBit(6));
        matrix.SetReserved(8, 8, GetFormatBit(7));
        matrix.SetReserved(7, 8, GetFormatBit(8));

        for (var index = 9; index < 15; index++)
        {
            matrix.SetReserved(14 - index, 8, GetFormatBit(index));
        }

        for (var index = 0; index <= 6; index++)
        {
            matrix.SetReserved(matrix.Size - 1 - index, 8, GetFormatBit(index));
        }

        for (var index = 7; index <= 14; index++)
        {
            matrix.SetReserved(8, matrix.Size - 15 + index, GetFormatBit(index));
        }
    }

    private static int CalculateFormatBits(int value)
    {
        var data = value << 10;
        for (var bit = 14; bit >= 10; bit--)
        {
            if (((data >> bit) & 1) != 0)
            {
                data ^= 0x537 << (bit - 10);
            }
        }

        return ((value << 10) | data) ^ 0x5412;
    }

    private static void AppendBits(List<bool> bits, int value, int length)
    {
        for (var index = length - 1; index >= 0; index--)
        {
            bits.Add(((value >> index) & 1) != 0);
        }
    }

    private static List<byte> BitsToBytes(IReadOnlyList<bool> bits)
    {
        var bytes = new List<byte>(bits.Count / 8);
        for (var index = 0; index < bits.Count; index += 8)
        {
            var value = 0;
            for (var offset = 0; offset < 8; offset++)
            {
                value = (value << 1) | (bits[index + offset] ? 1 : 0);
            }

            bytes.Add((byte)value);
        }

        return bytes;
    }

    private sealed record QRVersion(int Number, int DataCodewords, int EccCodewords, int RemainderBits, IReadOnlyList<int> AlignmentPositions)
    {
        public int Size => 21 + (Number - 1) * 4;
        public int ByteCapacity => DataCodewords - 2;
    }

    private sealed class QRMatrix
    {
        private readonly bool[,] reserved;

        public QRMatrix(int size)
        {
            Size = size;
            Modules = new bool[size, size];
            reserved = new bool[size, size];
        }

        public int Size { get; }
        public bool[,] Modules { get; }

        public bool Contains(int row, int column) => row >= 0 && row < Size && column >= 0 && column < Size;
        public bool IsReserved(int row, int column) => reserved[row, column];
        public void Set(int row, int column, bool value) => Modules[row, column] = value;
        public bool[,] CloneModules() => (bool[,])Modules.Clone();

        public void SetReserved(int row, int column, bool value)
        {
            Modules[row, column] = value;
            reserved[row, column] = true;
        }
    }

    private sealed record Module(int X, int Y);
}
