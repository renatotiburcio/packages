using System.Collections.Generic;
using System.Linq;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Static catalog of icons keyed by name+variant+size. Backed by <see cref="XFIconInfo"/>.
/// Generated at build time by <c>XForge.AssetsGenerator</c> (T080) from the curated
/// 3,698 Font Awesome Pro 7.1.0 + 1 custom agro-business icon set.
/// </summary>
public static partial class XFIconCatalog
{
    private static readonly Dictionary<string, XFIconInfo> _byKey = new();

    public static int Count => _byKey.Count;

    /// <summary>Lookup icon by name, variant and size. Returns null when missing.</summary>
    public static XFIconInfo? Get(string name, XFIconVariant variant, XFIconSize size)
    {
        var key = BuildKey(name, variant, size);
        return _byKey.TryGetValue(key, out var info) ? info : null;
    }

    /// <summary>Search icons whose name or keywords contain the supplied term (case-insensitive).</summary>
    public static IEnumerable<XFIconInfo> Search(string term, int max = 50)
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

    public static void Register(XFIconInfo info)
    {
        _byKey[BuildKey(info.Name, info.Variant, info.Size)] = info;
    }

    private static string BuildKey(string name, XFIconVariant variant, XFIconSize size)
        => $"{variant}:{size}:{name}";
}