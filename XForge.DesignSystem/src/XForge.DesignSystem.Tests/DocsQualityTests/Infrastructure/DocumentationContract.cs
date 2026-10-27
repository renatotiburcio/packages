namespace XForge.DesignSystem.Docs.QualityTests.Infrastructure;

public static class DocumentationContract
{
    public static IReadOnlyList<string> AccessibilityTerms { get; } = new List<string>
    {
        "aria-label", "aria-describedby", "aria-invalid", "aria-required",
        "role", "accessibility", "keyboard", "screen reader", "focus",
        "WCAG", "ARIA", "accessible", "a11y"
    };

    public static IReadOnlyList<SectionRequirement> RequiredSections { get; } = new List<SectionRequirement>
    {
        new("Overview", new[] { "Overview", "Visao Geral", "Visão Geral" }),
        new("Examples", new[] { "Exemplos", "Examples", "Example" }),
        new("API", new[] { "API", "Parametros", "Parâmetros", "Parameters" }),
        new("Accessibility", new[] { "Acessibilidade", "Accessibility" }),
        new("Changelog", new[] { "Changelog", "Historico", "Histórico" }),
    };
}

public sealed record SectionRequirement(string Id, IReadOnlyList<string> Aliases);
