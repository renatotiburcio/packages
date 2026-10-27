using System.Globalization;
using Microsoft.JSInterop;

namespace XForge.DesignSystem.Docs.Shared.Contracts;

public sealed class XForgeDocsLanguageService
{
    public const string DefaultCultureName = "pt-BR";
    public const string EnglishCultureName = "en-US";

    private const string StorageKey = "xforge.docs.language";
    private static readonly HashSet<string> SupportedCultureNames = new(StringComparer.OrdinalIgnoreCase)
    {
        DefaultCultureName,
        EnglishCultureName
    };

    private readonly IJSRuntime _jsRuntime;
    private bool _initialized;

    public XForgeDocsLanguageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        SetCulture(DefaultCultureName);
    }

    public event EventHandler? LanguageChanged;

    public string CultureName { get; private set; } = DefaultCultureName;

    public bool IsPortuguese => string.Equals(CultureName, DefaultCultureName, StringComparison.OrdinalIgnoreCase);

    public IReadOnlyList<XForgeDocsLanguageOption> Options { get; } =
    [
        new(DefaultCultureName, "PT-BR", "Portugues (Brasil)"),
        new(EnglishCultureName, "EN-US", "English (United States)")
    ];

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        try
        {
            var storedCulture = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrWhiteSpace(storedCulture))
            {
                SetCulture(storedCulture, notify: true);
            }
        }
        catch
        {
            SetCulture(DefaultCultureName, notify: false);
        }
    }

    public async Task SetLanguageAsync(string? cultureName)
    {
        var nextCultureName = Normalize(cultureName);
        if (string.Equals(CultureName, nextCultureName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        SetCulture(nextCultureName);

        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, nextCultureName);
        }
        catch
        {
        }
    }

    public string Text(string ptBr, string enUs) => IsPortuguese ? ptBr : enUs;

    private void SetCulture(string? cultureName, bool notify = true)
    {
        CultureName = Normalize(cultureName);
        var culture = CultureInfo.GetCultureInfo(CultureName);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        if (notify)
        {
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private static string Normalize(string? cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return DefaultCultureName;
        }

        return SupportedCultureNames.Contains(cultureName)
            ? cultureName
            : DefaultCultureName;
    }
}

public sealed record XForgeDocsLanguageOption(string CultureName, string ShortLabel, string Label);

