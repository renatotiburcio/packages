using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Net;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Barcode;

public partial class XFBarcode : XForgeComponentBase
{
    private static readonly string[] Code128Patterns =
    {
        "212222","222122","222221","121223","121322","131222","122213","122312","132212","221213",
        "221312","231212","112232","122132","122231","113222","123122","123221","223211","221132",
        "221231","213212","223112","312131","311222","321122","321221","312212","322112","322211",
        "212123","212321","232121","111323","131123","131321","112313","132113","132311","211313",
        "231113","231311","112133","112331","132131","113123","113321","133121","313121","211331",
        "231131","213113","213311","213131","311123","311321","331121","312113","312311","332111",
        "314111","221411","431111","111224","111422","121124","121421","141122","141221","112214",
        "112412","122114","122411","142112","142211","241211","221114","413111","241112","134111",
        "111242","121142","121241","114212","124112","124211","411212","421112","421211","212141",
        "214121","412121","111143","111341","131141","114113","114311","411113","411311","113141",
        "114131","311141","411131","211412","211214","211232","2331112"
    };

    private static readonly string[] EanLeftOdd =
    {
        "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011"
    };

    private static readonly string[] EanLeftEven =
    {
        "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111"
    };

    private static readonly string[] EanRight =
    {
        "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100"
    };

    private static readonly string[] Ean13Parity =
    {
        "OOOOOO", "OOEOEE", "OOEEOE", "OOEEEO", "OEOOEE", "OEEOOE", "OEEEOO", "OEOEOE", "OEOEEO", "OEEOEO"
    };

    private static readonly IReadOnlyDictionary<char, string> Code39Patterns = new Dictionary<char, string>
    {
        ['0'] = "nnnwwnwnn", ['1'] = "wnnwnnnnw", ['2'] = "nnwwnnnnw", ['3'] = "wnwwnnnnn", ['4'] = "nnnwwnnnw",
        ['5'] = "wnnwwnnnn", ['6'] = "nnwwwnnnn", ['7'] = "nnnwnnwnw", ['8'] = "wnnwnnwnn", ['9'] = "nnwwnnwnn",
        ['A'] = "wnnnnwnnw", ['B'] = "nnwnnwnnw", ['C'] = "wnwnnwnnn", ['D'] = "nnnnwwnnw", ['E'] = "wnnnwwnnn",
        ['F'] = "nnwnwwnnn", ['G'] = "nnnnnwwnw", ['H'] = "wnnnnwwnn", ['I'] = "nnwnnwwnn", ['J'] = "nnnnwwwnn",
        ['K'] = "wnnnnnnww", ['L'] = "nnwnnnnww", ['M'] = "wnwnnnnwn", ['N'] = "nnnnwnnww", ['O'] = "wnnnwnnwn",
        ['P'] = "nnwnwnnwn", ['Q'] = "nnnnnnwww", ['R'] = "wnnnnnwwn", ['S'] = "nnwnnnwwn", ['T'] = "nnnnwnwwn",
        ['U'] = "wwnnnnnnw", ['V'] = "nwwnnnnnw", ['W'] = "wwwnnnnnn", ['X'] = "nwnnwnnnw", ['Y'] = "wwnnwnnnn",
        ['Z'] = "nwwnwnnnn", ['-'] = "nwnnnnwnw", ['.'] = "wwnnnnwnn", [' '] = "nwwnnnwnn", ['$'] = "nwnwnwnnn",
        ['/'] = "nwnwnnnwn", ['+'] = "nwnnnwnwn", ['%'] = "nnnwnwnwn", ['*'] = "nwnnwnwnn"
    };

    private static readonly string[] ItfPatterns =
    {
        "nnwwn", "wnnnw", "nwnnw", "wwnnn", "nnwnw", "wnwnn", "nwwnn", "nnnww", "wnnwn", "nwnwn"
    };

    [Parameter] public string? Value { get; set; }
    [Parameter] public XFBarcodeFormat Format { get; set; } = XFBarcodeFormat.Code128;
    [Parameter] public int Height { get; set; } = 96;
    [Parameter] public int ModuleWidth { get; set; } = 2;
    [Parameter] public int QuietZone { get; set; } = 10;
    [Parameter] public bool ShowText { get; set; } = true;
    [Parameter] public string ForegroundColor { get; set; } = "#111827";
    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";
    [Parameter] public string EmptyText { get; set; } = "Informe um valor valido para gerar o codigo.";
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public string AriaLabelFormat { get; set; } = "Codigo de barras {0}";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Value, Format, Height, ModuleWidth, QuietZone, ShowText, ForegroundColor, BackgroundColor, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

    private EncodedBarcode? Encoded => Encode(Value, Format);
    private bool CanRender => Encoded is not null;
    private IReadOnlyList<Bar> Bars => Encoded?.Bars ?? Array.Empty<Bar>();
    private int PatternWidth => Encoded?.Width ?? 0;
    private int SvgWidth => PatternWidth + QuietZone * 2;
    private int SvgHeight => Height + (ShowText ? 18 : 0) + QuietZone * 2;
    private int BarHeight => Height;
    private string ViewBox => string.Create(CultureInfo.InvariantCulture, $"{0} {0} {SvgWidth} {SvgHeight}");
    private string DisplayValue => Encoded?.Text ?? Value ?? string.Empty;
    private string ResolvedAriaLabel => AriaLabel ?? string.Format(CultureInfo.CurrentCulture, AriaLabelFormat, DisplayValue);
    private string TextElement => string.Create(
        CultureInfo.InvariantCulture,
        $"<text x=\"{SvgWidth / 2}\" y=\"{SvgHeight - 4}\" text-anchor=\"middle\" font-size=\"12\" font-family=\"ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace\" fill=\"{WebUtility.HtmlEncode(ForegroundColor)}\">{WebUtility.HtmlEncode(DisplayValue)}</text>");

    private string RootClass => GetClass("xf-barcode inline-block w-full max-w-sm rounded-lg bg-white p-2 text-surface-950 ring-1 ring-border-light");

    private EncodedBarcode? Encode(string? value, XFBarcodeFormat format)
    {
        return format switch
        {
            XFBarcodeFormat.Code128 => EncodeCode128(value, gs1: false),
            XFBarcodeFormat.Gs1_128 => EncodeCode128(value, gs1: true),
            XFBarcodeFormat.Code39 => EncodeCode39(value),
            XFBarcodeFormat.Ean13 => EncodeEan13(value),
            XFBarcodeFormat.Ean8 => EncodeEan8(value),
            XFBarcodeFormat.UpcA => EncodeUpcA(value),
            XFBarcodeFormat.Itf14 => EncodeItf14(value),
            XFBarcodeFormat.Interleaved2Of5 => EncodeInterleaved2Of5(value),
            _ => null
        };
    }

    private EncodedBarcode? EncodeCode128(string? value, bool gs1)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var encoded = new List<int> { 104 };
        if (gs1)
        {
            encoded.Add(102);
        }

        foreach (var character in value)
        {
            if (character < 32 || character > 126)
            {
                return null;
            }

            encoded.Add(character - 32);
        }

        var checksum = encoded[0];
        for (var index = 1; index < encoded.Count; index++)
        {
            checksum += encoded[index] * index;
        }

        encoded.Add(checksum % 103);
        encoded.Add(106);
        return FromWidths(encoded.Select(valueIndex => Code128Patterns[valueIndex]), value);
    }

    private EncodedBarcode? EncodeCode39(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.ToUpperInvariant();
        if (normalized.Any(character => !Code39Patterns.ContainsKey(character) || character == '*'))
        {
            return null;
        }

        var patterns = $"*{normalized}*".Select(character => Code39Patterns[character]);
        return FromNarrowWide(patterns, normalized, interCharacterGap: true);
    }

    private EncodedBarcode? EncodeEan13(string? value)
    {
        var digits = NormalizeDigits(value);
        if (digits.Length is not (12 or 13))
        {
            return null;
        }

        digits = EnsureChecksum(digits, 13);
        if (digits is null)
        {
            return null;
        }

        var parity = Ean13Parity[digits[0] - '0'];
        var bits = "101";
        for (var index = 1; index <= 6; index++)
        {
            var digit = digits[index] - '0';
            bits += parity[index - 1] == 'O' ? EanLeftOdd[digit] : EanLeftEven[digit];
        }

        bits += "01010";
        for (var index = 7; index < 13; index++)
        {
            bits += EanRight[digits[index] - '0'];
        }

        bits += "101";
        return FromBits(bits, digits);
    }

    private EncodedBarcode? EncodeEan8(string? value)
    {
        var digits = NormalizeDigits(value);
        if (digits.Length is not (7 or 8))
        {
            return null;
        }

        digits = EnsureChecksum(digits, 8);
        if (digits is null)
        {
            return null;
        }

        var bits = "101";
        for (var index = 0; index < 4; index++)
        {
            bits += EanLeftOdd[digits[index] - '0'];
        }

        bits += "01010";
        for (var index = 4; index < 8; index++)
        {
            bits += EanRight[digits[index] - '0'];
        }

        bits += "101";
        return FromBits(bits, digits);
    }

    private EncodedBarcode? EncodeUpcA(string? value)
    {
        var digits = NormalizeDigits(value);
        if (digits.Length is not (11 or 12))
        {
            return null;
        }

        digits = EnsureChecksum(digits, 12);
        if (digits is null)
        {
            return null;
        }

        var bits = "101";
        for (var index = 0; index < 6; index++)
        {
            bits += EanLeftOdd[digits[index] - '0'];
        }

        bits += "01010";
        for (var index = 6; index < 12; index++)
        {
            bits += EanRight[digits[index] - '0'];
        }

        bits += "101";
        return FromBits(bits, digits);
    }

    private EncodedBarcode? EncodeItf14(string? value)
    {
        var digits = NormalizeDigits(value);
        if (digits.Length is not (13 or 14))
        {
            return null;
        }

        digits = EnsureChecksum(digits, 14);
        return digits is null ? null : EncodeInterleavedDigits(digits);
    }

    private EncodedBarcode? EncodeInterleaved2Of5(string? value)
    {
        var digits = NormalizeDigits(value);
        if (digits.Length == 0)
        {
            return null;
        }

        if (digits.Length % 2 != 0)
        {
            digits = $"0{digits}";
        }

        return EncodeInterleavedDigits(digits);
    }

    private EncodedBarcode EncodeInterleavedDigits(string digits)
    {
        var bars = new List<Bar>();
        var x = QuietZone;
        AppendNarrowWideSequence(bars, ref x, "nnnn", startsWithBar: true);

        for (var index = 0; index < digits.Length; index += 2)
        {
            var barPattern = ItfPatterns[digits[index] - '0'];
            var spacePattern = ItfPatterns[digits[index + 1] - '0'];
            for (var element = 0; element < 5; element++)
            {
                AppendElement(bars, ref x, barPattern[element], draw: true);
                AppendElement(bars, ref x, spacePattern[element], draw: false);
            }
        }

        AppendNarrowWideSequence(bars, ref x, "wnn", startsWithBar: true);
        return new EncodedBarcode(bars, x - QuietZone, digits);
    }

    private EncodedBarcode FromBits(string bits, string text)
    {
        var bars = new List<Bar>();
        var x = QuietZone;
        var start = -1;

        for (var index = 0; index < bits.Length; index++)
        {
            if (bits[index] == '1' && start < 0)
            {
                start = index;
            }
            else if (bits[index] == '0' && start >= 0)
            {
                bars.Add(new Bar(QuietZone + start * ModuleWidth, (index - start) * ModuleWidth));
                start = -1;
            }
        }

        if (start >= 0)
        {
            bars.Add(new Bar(QuietZone + start * ModuleWidth, (bits.Length - start) * ModuleWidth));
        }

        return new EncodedBarcode(bars, bits.Length * ModuleWidth, text);
    }

    private EncodedBarcode FromWidths(IEnumerable<string> patterns, string text)
    {
        var bars = new List<Bar>();
        var x = QuietZone;
        foreach (var pattern in patterns)
        {
            var draw = true;
            foreach (var character in pattern)
            {
                var width = (character - '0') * ModuleWidth;
                if (draw)
                {
                    bars.Add(new Bar(x, width));
                }

                x += width;
                draw = !draw;
            }
        }

        return new EncodedBarcode(bars, x - QuietZone, text);
    }

    private EncodedBarcode FromNarrowWide(IEnumerable<string> patterns, string text, bool interCharacterGap)
    {
        var bars = new List<Bar>();
        var x = QuietZone;
        foreach (var pattern in patterns)
        {
            AppendNarrowWideSequence(bars, ref x, pattern, startsWithBar: true);
            if (interCharacterGap)
            {
                x += ModuleWidth;
            }
        }

        return new EncodedBarcode(bars, x - QuietZone, text);
    }

    private void AppendNarrowWideSequence(List<Bar> bars, ref int x, string pattern, bool startsWithBar)
    {
        var draw = startsWithBar;
        foreach (var element in pattern)
        {
            AppendElement(bars, ref x, element, draw);
            draw = !draw;
        }
    }

    private void AppendElement(List<Bar> bars, ref int x, char element, bool draw)
    {
        var width = (element == 'w' ? 3 : 1) * ModuleWidth;
        if (draw)
        {
            bars.Add(new Bar(x, width));
        }

        x += width;
    }

    private static string NormalizeDigits(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : new string(value.Where(char.IsDigit).ToArray());

    private static string? EnsureChecksum(string digits, int targetLength)
    {
        if (digits.Length == targetLength - 1)
        {
            return $"{digits}{CalculateModulo10Checksum(digits)}";
        }

        if (digits.Length == targetLength)
        {
            var expected = CalculateModulo10Checksum(digits[..^1]);
            return digits[^1] - '0' == expected ? digits : null;
        }

        return null;
    }

    private static int CalculateModulo10Checksum(string digits)
    {
        var sum = 0;
        var weight = 3;
        for (var index = digits.Length - 1; index >= 0; index--)
        {
            sum += (digits[index] - '0') * weight;
            weight = weight == 3 ? 1 : 3;
        }

        return (10 - sum % 10) % 10;
    }

    private sealed record EncodedBarcode(IReadOnlyList<Bar> Bars, int Width, string Text);
    private sealed record Bar(int X, int Width);
}
