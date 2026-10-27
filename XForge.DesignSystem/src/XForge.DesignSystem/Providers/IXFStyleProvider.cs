// T044: Provider-agnostic style abstraction.
namespace XForge.DesignSystem.Providers;

/// <summary>
/// Resolves inline style strings or CSS custom property values for a given semantic key.
/// </summary>
public interface IXFStyleProvider
{
    /// <summary>Resolve a named design token (e.g. <c>spacing.md</c>) to a value.</summary>
    string Token(string key);
    /// <summary>Compose an inline style string from key/value pairs.</summary>
    string Inline(string key, string value);
    /// <summary>Provider name (e.g. "Tailwind4", "Bootstrap5").</summary>
    string ProviderName { get; }
}