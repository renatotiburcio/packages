// T044: Default Tailwind style provider.
namespace XForge.DesignSystem.Providers;

/// <summary>Default Tailwind v4 inline-style provider.</summary>
public sealed class XFTailwindStyleProvider : IXFStyleProvider
{
    public string ProviderName => "Tailwind4";
    public string Token(string key) => key switch
    {
        "spacing.sm" => "0.5rem",
        "spacing.md" => "1rem",
        "spacing.lg" => "1.5rem",
        "color.primary" => "#2563eb",
        "color.danger" => "#dc2626",
        _ => string.Empty,
    };
    public string Inline(string key, string value) => $"--{key}: {value};";
}