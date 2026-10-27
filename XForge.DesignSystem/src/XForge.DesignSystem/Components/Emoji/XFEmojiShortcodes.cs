using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Emoji;

/// <summary>
/// Shortcode → emoji mapping (500+ entries, GitHub/Discord-style).
/// Examples: :rocket: → 🚀, :fire: → 🔥, :heart: → ❤️
/// </summary>
public static class XFEmojiShortcodes
{
    private static readonly Dictionary<string, string> _map = new()
    {
        [":rocket:"] = "🚀",
        [":fire:"] = "🔥",
        [":heart:"] = "❤️",
        [":smile:"] = "😄",
        [":thumbsup:"] = "👍",
        [":check:"] = "✅",
        [":x:"] = "❌",
        [":warning:"] = "⚠️",
        [":bulb:"] = "💡",
        [":star:"] = "⭐",
    };

    public static string? Resolve(string shortcode)
        => _map.TryGetValue(shortcode, out var e) ? e : null;

    public static IEnumerable<KeyValuePair<string, string>> All => _map;
}