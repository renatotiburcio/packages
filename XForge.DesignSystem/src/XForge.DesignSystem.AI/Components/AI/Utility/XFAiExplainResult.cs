namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Result of an AI explanation request.
/// </summary>
public sealed record XFAiExplainResult
{
    /// <summary>The original element/field that was explained.</summary>
    public string ElementName { get; init; } = string.Empty;

    /// <summary>The AI-generated explanation text.</summary>
    public string Explanation { get; init; } = string.Empty;

    /// <summary>Optional detailed breakdown of the explanation.</summary>
    public IReadOnlyList<XFAiExplainDetail> Details { get; init; } = Array.Empty<XFAiExplainDetail>();

    /// <summary>AI confidence score between 0.0 and 1.0.</summary>
    public double Confidence { get; init; }

    /// <summary>Optional source references used for the explanation.</summary>
    public IReadOnlyList<string> Sources { get; init; } = Array.Empty<string>();
}

/// <summary>
/// A detailed breakdown item within an explanation.
/// </summary>
public sealed record XFAiExplainDetail
{
    /// <summary>Label for this detail item.</summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>Value or content of this detail.</summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>Optional icon name.</summary>
    public string? Icon { get; init; }
}
