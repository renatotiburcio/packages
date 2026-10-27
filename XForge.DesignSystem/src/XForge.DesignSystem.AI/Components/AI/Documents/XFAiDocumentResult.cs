namespace XForge.DesignSystem.Components.AI.Documents;

/// <summary>
/// Represents the result of AI-powered document processing, including
/// summary, chunks, metadata, and processing metrics.
/// </summary>
public sealed class XFAiDocumentResult
{
    /// <summary>Unique identifier for this processing result.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Original document filename.</summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>Document format type.</summary>
    public XFAiDocumentType DocumentType { get; set; }

    /// <summary>AI-generated summary of the document content.</summary>
    public string? AiSummary { get; set; }

    /// <summary>Document content split into semantic chunks.</summary>
    public IReadOnlyList<XFAiDocumentChunk> Chunks { get; set; } = Array.Empty<XFAiDocumentChunk>();

    /// <summary>Total character count of the original document.</summary>
    public int TotalCharacters { get; set; }

    /// <summary>Total number of chunks generated.</summary>
    public int TotalChunks { get; set; }

    /// <summary>Time taken to process the document.</summary>
    public TimeSpan ProcessingDuration { get; set; }

    /// <summary>Arbitrary metadata extracted from the document.</summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Represents a single chunk of document content with position and optional embedding data.
/// </summary>
public sealed class XFAiDocumentChunk
{
    /// <summary>Zero-based index of this chunk within the document.</summary>
    public int Index { get; set; }

    /// <summary>Text content of this chunk.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Starting character position in the original document.</summary>
    public int StartChar { get; set; }

    /// <summary>Ending character position in the original document.</summary>
    public int EndChar { get; set; }

    /// <summary>Optional embedding vector for semantic search.</summary>
    public float[]? Embedding { get; set; }

    /// <summary>Optional relevance score from semantic search (0.0 to 1.0).</summary>
    public double? RelevanceScore { get; set; }
}

/// <summary>
/// Supported document format types for the AI Document Reader.
/// </summary>
public enum XFAiDocumentType
{
    /// <summary>Adobe PDF document.</summary>
    Pdf,

    /// <summary>Microsoft Word document.</summary>
    Docx,

    /// <summary>Microsoft Excel spreadsheet.</summary>
    Xlsx,

    /// <summary>XML document.</summary>
    Xml,

    /// <summary>Plain text file.</summary>
    Txt,

    /// <summary>Comma-separated values file.</summary>
    Csv,

    /// <summary>JPEG image.</summary>
    Jpg,

    /// <summary>PNG image.</summary>
    Png
}
