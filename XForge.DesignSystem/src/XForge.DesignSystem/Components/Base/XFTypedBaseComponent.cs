// T047: Typed BaseComponent<TClasses, TStyles> (Blazorise BaseComponent pattern).
// P1, 16h, depends T044 (provider interfaces).
namespace XForge.DesignSystem.Components.Base;

using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Providers;

/// <summary>
/// Generic base class that lets a component expose a strongly-typed <c>Classes</c> and <c>Styles</c> builder.
/// Subclasses populate <typeparamref name="TClasses"/> and <typeparamref name="TStyles"/> once, then re-use them.
/// </summary>
/// <typeparam name="TClasses">Strongly-typed CSS class catalog (must have parameterless ctor).</typeparam>
/// <typeparam name="TStyles">Strongly-typed inline-style catalog (must have parameterless ctor).</typeparam>
public abstract class XFTypedBaseComponent<TClasses, TStyles> : XFBaseComponent
    where TClasses : class, new()
    where TStyles : class, new()
{
    private TClasses? _classes;
    private TStyles? _styles;
    [Inject] protected IXFClassProvider? ClassProvider { get; set; }
    [Inject] protected IXFStyleProvider? StyleProvider { get; set; }

    /// <summary>Lazy strongly-typed class catalog.</summary>
    protected TClasses Classes => _classes ??= new TClasses();

    /// <summary>Lazy strongly-typed style catalog.</summary>
    protected TStyles Styles => _styles ??= new TStyles();

    /// <summary>Resolves a CSS class through the injected provider, falling back to the <c>Class</c> parameter.</summary>
    protected string BuildClass(string? providerClass) =>
        string.IsNullOrWhiteSpace(Class) ? providerClass ?? string.Empty :
        string.IsNullOrWhiteSpace(providerClass) ? Class : $"{providerClass} {Class}";

    /// <summary>Resolves an inline style through the injected provider, falling back to the <c>Style</c> parameter.</summary>
    protected string BuildStyle(string? providerStyle) =>
        string.IsNullOrWhiteSpace(Style) ? providerStyle ?? string.Empty :
        string.IsNullOrWhiteSpace(providerStyle) ? Style : $"{providerStyle}; {Style}";
}

/// <summary>Demo class catalog used in tests.</summary>
public sealed class DemoClasses
{
    public string Button => "bg-blue-600 text-white rounded px-3 py-1.5";
    public string Card => "bg-white rounded-lg shadow p-4";
}

/// <summary>Demo style catalog used in tests.</summary>
public sealed class DemoStyles
{
    public string Spacing => "--spacing: 1rem;";
}