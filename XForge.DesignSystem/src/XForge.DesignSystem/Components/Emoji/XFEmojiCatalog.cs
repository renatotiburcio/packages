using System.Collections.Generic;
using System.Linq;

namespace XForge.DesignSystem.Components.Emoji;

public static partial class XFEmojiCatalog
{
    private static readonly Dictionary<string, XFEmojiInfo> _byKey = new();
    public static int Count => _byKey.Count;

    public static XFEmojiInfo? Get(string name, XFEmojiStyle style, XFEmojiSize size)
    {
        var key = $"{name}:{style}:{size}";
        return _byKey.TryGetValue(key, out var info) ? info : null;
    }

    public static XFEmojiInfo? GetByShortcode(string shortcode)
    {
        var resolved = XFEmojiShortcodes.Resolve(shortcode);
        if (resolved == null) return null;
        return _byKey.Values.FirstOrDefault(e => e.Name == shortcode.Trim(':'));
    }

    public static IEnumerable<XFEmojiInfo> Search(string term, int max = 25)
    {
        if (string.IsNullOrWhiteSpace(term)) yield break;
        var hits = 0;
        foreach (var info in _byKey.Values)
        {
            if (info.Name.Contains(term, System.StringComparison.OrdinalIgnoreCase)
                || info.Keywords.Any(k => k.Contains(term, System.StringComparison.OrdinalIgnoreCase)))
            {
                yield return info;
                if (++hits >= max) yield break;
            }
        }
    }

    public static void Register(XFEmojiInfo info)
    {
        var key = $"{info.Name}:{info.Style}:{info.Size}";
        _byKey[key] = info;
    }
}