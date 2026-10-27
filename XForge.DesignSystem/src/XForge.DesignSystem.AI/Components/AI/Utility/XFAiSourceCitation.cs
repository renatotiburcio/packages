namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Represents a single source citation used by the AI in its response.
/// </summary>
public sealed record XFAiSourceCitation
{
    /// <summary>Unique identifier for the citation.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Title or name of the source.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>URL or path to the source document.</summary>
    public string? Url { get; init; }

    /// <summary>Relevant excerpt or snippet from the source.</summary>
    public string? Snippet { get; init; }

    /// <summary>AI confidence score for this citation (0.0–1.0).</summary>
    public double Confidence { get; init; }

    /// <summary>Rank or order of relevance (1 = most relevant).</summary>
    public int Rank { get; init; }

    /// <summary>Source type (e.g., "Documento", "Lei", "Artigo", "Manual").</summary>
    public string? SourceType { get; init; }

    /// <summary>Publication date or last updated date.</summary>
    public DateTime? PublishedDate { get; init; }

    /// <summary>Optional page number or section reference.</summary>
    public string? PageReference { get; init; }
}
