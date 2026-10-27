namespace XForge.DesignSystem.Components.AI.Insights;

/// <summary>
/// Represents a contextual AI suggestion with type, confidence, and optional metadata.
/// </summary>
public sealed record XFAiSuggestion
{
    /// <summary>Unique identifier for the suggestion.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Short title displayed as the suggestion heading.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Optional longer description shown below the title.</summary>
    public string? Description { get; init; }

    /// <summary>Optional SVG icon name resolved via XFSvgIcons.</summary>
    public string? Icon { get; init; }

    /// <summary>Suggestion category that determines visual styling.</summary>
    public XFAiSuggestionType Type { get; init; }

    /// <summary>AI confidence score between 0.0 and 1.0.</summary>
    public double? Confidence { get; init; }

    /// <summary>Optional URL or route to navigate to when the suggestion is activated.</summary>
    public string? ActionUrl { get; init; }

    /// <summary>Optional grouping category label.</summary>
    public string? Category { get; init; }

    /// <summary>Arbitrary key-value metadata for extensibility.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Determines the visual style and semantic meaning of an XFAiSuggestion.
/// </summary>
public enum XFAiSuggestionType
{
    /// <summary>Suggested next action - blue accent.</summary>
    Action,

    /// <summary>AI insight or prediction - purple accent.</summary>
    Insight,

    /// <summary>Alert or warning - amber accent.</summary>
    Warning,

    /// <summary>Optimization or recommendation - green accent.</summary>
    Recommendation,

    /// <summary>General information - slate accent.</summary>
    Info
}
