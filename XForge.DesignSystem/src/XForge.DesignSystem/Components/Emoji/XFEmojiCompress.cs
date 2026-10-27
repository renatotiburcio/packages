using System.IO;
using System.IO.Compression;

namespace XForge.DesignSystem.Components.Emoji;

/// <summary>
/// GZip helper for emoji payloads. Stores compressed bytes; lazy-decodes to SVG on demand.
/// 70%+ compression vs raw SVG (FluentUI pattern).
/// </summary>
public static class XFEmojiCompress
{
    public static byte[] Zip(string svg)
    {
        if (string.IsNullOrEmpty(svg)) return System.Array.Empty<byte>();
        using var output = new MemoryStream();
        using (var gz = new GZipStream(output, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gz)) writer.Write(svg);
        return output.ToArray();
    }

    public static string Unzip(byte[] data)
    {
        if (data == null || data.Length == 0) return string.Empty;
        using var input = new MemoryStream(data);
        using var gz = new GZipStream(input, CompressionMode.Decompress);
        using var reader = new StreamReader(gz);
        return reader.ReadToEnd();
    }
}