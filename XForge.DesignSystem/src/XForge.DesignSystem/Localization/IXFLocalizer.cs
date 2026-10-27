using System;
using XForge.Localization.Json;

namespace XForge.Localization;

/// <summary>
/// JSON-based localizer for XForge components.
/// Resolves keys from XFJsonLocaleProvider with format string support.
/// Falls back to the key itself when not found.
/// </summary>
public interface IXFLocalizer
{
    /// <summary>
    /// Gets the localized string for the given key.
    /// Returns the key itself if not found.
    /// </summary>
    string this[string key] { get; }

    /// <summary>
    /// Gets the localized string for the given key, formatted with the supplied arguments.
    /// </summary>
    string Format(string key, params object[] args);
}

/// <summary>
/// Default implementation backed by <see cref="XFJsonLocaleProvider"/>.
/// </summary>
public sealed class XFLocalizer : IXFLocalizer
{
    private readonly XFJsonLocaleProvider _provider;

    public XFLocalizer(XFJsonLocaleProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public string this[string key] => _provider.Get(key) ?? key;

    public string Format(string key, params object[] args)
    {
        var value = this[key];
        return args.Length == 0 ? value : string.Format(value, args);
    }
}
