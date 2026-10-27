using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace XForge.Localization.Json;

/// <summary>
/// JSON-based locale loader. Inspired by antd-blazor (70+ locale JSONs).
/// </summary>
public sealed class XFJsonLocaleProvider
{
    private readonly Dictionary<string, Dictionary<string, string>> _locales = new();
    private string _currentCulture = "en";

    public string CurrentCulture
    {
        get => _currentCulture;
        set => _currentCulture = value ?? "en";
    }

    public int LoadedLocaleCount => _locales.Count;

    public void RegisterLocale(string culture, IDictionary<string, string> strings)
    {
        if (string.IsNullOrEmpty(culture)) return;
        _locales[culture] = new Dictionary<string, string>(strings);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Dictionary<string,string> is a primitive type")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Dictionary<string,string> is a primitive type")]
    public void RegisterLocaleFromJson(string culture, string json)
    {
        if (string.IsNullOrEmpty(culture) || string.IsNullOrEmpty(json)) return;
        var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (parsed is not null) RegisterLocale(culture, parsed);
    }

    public string? Get(string key)
    {
        if (_locales.TryGetValue(_currentCulture, out var locale) && locale.TryGetValue(key, out var value))
        {
            return value;
        }
        if (_locales.TryGetValue("en", out var fallback) && fallback.TryGetValue(key, out var fb))
        {
            return fb;
        }
        return null;
    }

    public IReadOnlyCollection<string> AvailableCultures => _locales.Keys;
}