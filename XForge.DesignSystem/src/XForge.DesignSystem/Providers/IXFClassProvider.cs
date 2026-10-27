// T044: Provider-agnostic class abstraction (Blazorise IClassProvider pattern).
// P0, 32h, no deps.
namespace XForge.DesignSystem.Providers;

/// <summary>
/// Resolves CSS class names for XForge components in a provider-agnostic way.
/// Default implementation: Tailwind. Bootstrap/Material/Bulma are alternative providers.
/// </summary>
public interface IXFClassProvider
{
    /// <summary>Class for a button.</summary>
    string Button(bool outline, string? size = null, string? color = null);
    /// <summary>Class for an input control.</summary>
    string Input(string? size = null, string? color = null, bool disabled = false);
    /// <summary>Class for a card container.</summary>
    string Card();
    /// <summary>Class for a modal/dialog.</summary>
    string Modal();
    /// <summary>Class for a table.</summary>
    string Table();
    /// <summary>Class for an alert/notification.</summary>
    string Alert(string? color = null);
    /// <summary>Class for a badge.</summary>
    string Badge(string? color = null);
    /// <summary>Provider name (e.g. "Tailwind", "Bootstrap5").</summary>
    string ProviderName { get; }
}