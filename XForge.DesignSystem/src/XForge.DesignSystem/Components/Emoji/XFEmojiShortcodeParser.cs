using System.Text.RegularExpressions;

namespace XForge.DesignSystem.Components.Emoji;

/// <summary>
/// Parses emoji shortcodes (e.g. :rocket: -> 🚀) and reverse (🚀 -> :rocket:).
/// Case-insensitive, word-boundary safe, supports aliases and skin tone modifiers.
/// </summary>
public static class XFEmojiShortcodeParser
{
    private static readonly Regex Pattern = new(
        @":([a-z0-9+\\-]+):",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string Parse(string text)
    {
        if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
        return Pattern.Replace(text, m =>
        {
            var key = m.Groups[1].Value.ToLowerInvariant();
            return XFEmojiShortcodes.Resolve($":{key}:") ?? m.Value;
        });
    }

    public static string Reverse(string text)
    {
        if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
        foreach (var (shortcode, emoji) in XFEmojiShortcodes.All)
            text = text.Replace(emoji, shortcode);
        return text;
    }

    public static bool TryResolve(string shortcode, out string emoji)
    {
        emoji = XFEmojiShortcodes.Resolve(shortcode) ?? string.Empty;
        return !string.IsNullOrEmpty(emoji);
    }
}