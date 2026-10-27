namespace XForge.DesignSystem.Components.AI.Prompting;

/// <summary>
/// Represents a reusable prompt template with optional variable placeholders.
/// Variables use double-brace syntax: {{variableName}}.
/// </summary>
public sealed record XFAiPromptTemplate
{
    /// <summary>Unique identifier for the template.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Display name shown in the template selector.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Optional description shown as tooltip or subtitle.</summary>
    public string? Description { get; init; }

    /// <summary>
    /// The prompt content to insert when the template is selected.
    /// May contain {{variable}} placeholders that are highlighted in the editor.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>Optional category for grouping templates in the selector.</summary>
    public string? Category { get; init; }

    /// <summary>
    /// List of variable names expected in the content (without braces).
    /// Example: ["cliente", "periodo"] corresponds to {{cliente}} and {{periodo}}.
    /// </summary>
    public IReadOnlyList<string>? Variables { get; init; }
}