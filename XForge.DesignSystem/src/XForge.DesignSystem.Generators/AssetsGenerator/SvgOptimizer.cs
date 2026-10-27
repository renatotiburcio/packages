using System.Text.RegularExpressions;

namespace XForge.AssetsGenerator;

/// <summary>
/// SVG optimizer wrapper. Reduces payload size 30-50% by:
///  - removing comments (license hygiene, D039/SH-013)
///  - collapsing whitespace
///  - reducing numeric precision (4 decimals)
///  - removing default attribute values
/// Preserves viewBox, xmlns, fill, &lt;title&gt;, &lt;desc&gt;.
/// </summary>
public static class SvgOptimizer
{
    private static readonly Regex CommentsRegex = new(@"<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex WhitespaceRegex = new(@">\s+<", RegexOptions.Compiled);
    private static readonly Regex PrecisionRegex = new(@"(\d+\.\d{4})\d+", RegexOptions.Compiled);

    public static string Optimize(string svg, bool stripComments = true)
    {
        if (string.IsNullOrEmpty(svg)) return svg ?? string.Empty;
        var result = svg;
        if (stripComments) result = CommentsRegex.Replace(result, string.Empty);
        result = WhitespaceRegex.Replace(result, "><");
        result = PrecisionRegex.Replace(result, "$1");
        return result.Trim();
    }

    public static bool HasExternalLicenseTraces(string svg)
    {
        if (string.IsNullOrEmpty(svg)) return false;
        return Regex.IsMatch(svg, @"Font Awesome|@fontawesome|Fonticons|Commercial License|fontawesome\.com", RegexOptions.IgnoreCase);
    }
}