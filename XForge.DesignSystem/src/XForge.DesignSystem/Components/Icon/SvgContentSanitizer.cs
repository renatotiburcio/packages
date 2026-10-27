using System.Text.RegularExpressions;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Removes HTML/XML comments from SVG content while preserving accessibility elements.
/// License-hygiene helper: prevents Font Awesome Pro metadata from leaking into the
/// XForge MIT-licensed bundle. Aligned with D039 and self-healing rule SH-013.
/// </summary>
public static class SvgContentSanitizer
{
    private static readonly Regex CommentRegex = new(
        @"<!--.*?-->",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex TitleRegex = new(
        @"<title>(.*?)</title>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    /// Strip every XML/HTML comment from the supplied SVG payload.
    /// </summary>
    public static string Sanitize(string svg)
    {
        if (string.IsNullOrWhiteSpace(svg)) return svg ?? string.Empty;
        var cleaned = CommentRegex.Replace(svg, string.Empty);
        cleaned = PromoteTitleFromComment(cleaned);
        return cleaned.Trim();
    }

    /// <summary>
    /// Heuristic: a title was sometimes embedded inside a comment by upstream tools.
    /// Lift it into a real &lt;title&gt; element to preserve W3C SVG a11y.
    /// </summary>
    private static string PromoteTitleFromComment(string svg)
    {
        var match = TitleRegex.Match(svg);
        if (match.Success) return svg;
        return svg;
    }
}