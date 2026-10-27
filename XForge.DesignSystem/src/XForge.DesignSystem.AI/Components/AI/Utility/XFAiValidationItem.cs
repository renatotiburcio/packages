namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Represents a single validation item with AI diagnostics.
/// </summary>
public sealed record XFAiValidationItem
{
    /// <summary>Unique identifier for the validation item.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Field or element name that failed validation.</summary>
    public string FieldName { get; init; } = string.Empty;

    /// <summary>The validation error message.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Severity of the validation issue.</summary>
    public XFAiValidationSeverity Severity { get; init; }

    /// <summary>AI-generated explanation of why this validation failed.</summary>
    public string? AiExplanation { get; init; }

    /// <summary>AI-suggested fix for the validation issue.</summary>
    public string? AiFixSuggestion { get; init; }

    /// <summary>Optional code or value that triggered the validation.</summary>
    public string? TriggerValue { get; init; }

    /// <summary>Whether this item has been resolved.</summary>
    public bool IsResolved { get; init; }
}

/// <summary>
/// Severity level for validation items.
/// </summary>
public enum XFAiValidationSeverity
{
    /// <summary>Informational message.</summary>
    Info,

    /// <summary>Warning that should be reviewed.</summary>
    Warning,

    /// <summary>Error that must be fixed.</summary>
    Error,

    /// <summary>Critical error blocking submission.</summary>
    Critical
}
