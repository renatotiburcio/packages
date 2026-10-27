namespace XForge.DesignSystem.Components.AI.Common;

/// <summary>
/// Represents a source citation from an AI response (RAG grounding).
/// </summary>
public sealed record AiCitation
{
    /// <summary>Display label for the source (e.g., document title).</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>URL or identifier to locate the source.</summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>Relevance confidence score between 0.0 and 1.0.</summary>
    public double Score { get; init; }

    /// <summary>Optional snippet or excerpt from the source.</summary>
    public string? Snippet { get; init; }

    /// <summary>Source type identifier (e.g., "document", "web", "database").</summary>
    public string? SourceType { get; init; }
}
