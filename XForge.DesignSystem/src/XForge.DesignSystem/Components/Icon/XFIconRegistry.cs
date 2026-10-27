using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Thread-safe singleton icon registry. AOT-friendly (no reflection).
/// </summary>
public sealed class XFIconRegistry : IXFIconRegistry
{
    private readonly ConcurrentDictionary<string, XFIconInfo> _byKey = new();
    public int Count => _byKey.Count;

    public XFIconInfo? Get(string name, XFIconVariant variant, XFIconSize size)
    {
        _byKey.TryGetValue(Key(name, variant, size), out var info);
        return info;
    }

    public IEnumerable<XFIconInfo> Search(string keyword, int page = 0, int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(keyword)) yield break;
        var normalized = RemoveDiacritics(keyword).ToLowerInvariant();
        var hits = 0; var skipped = 0;
        foreach (var info in _byKey.Values)
        {
            if (Matches(info, normalized))
            {
                if (skipped < page * pageSize) { skipped++; continue; }
                yield return info;
                if (++hits >= pageSize) yield break;
            }
        }
    }

    public void Register(XFIconInfo info) => _byKey[Key(info.Name, info.Variant, info.Size)] = info;

    private static string Key(string n, XFIconVariant v, XFIconSize s) => $"{v}:{s}:{n}";

    private static bool Matches(XFIconInfo info, string normalized)
    {
        if (RemoveDiacritics(info.Name).Contains(normalized, System.StringComparison.OrdinalIgnoreCase)) return true;
        return info.Keywords.Any(k => RemoveDiacritics(k).Contains(normalized, System.StringComparison.OrdinalIgnoreCase));
    }

    private static string RemoveDiacritics(string s) => new string(s.Normalize(NormalizationForm.FormD)
        .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
        .ToArray()).Normalize(NormalizationForm.FormC);
}