namespace XForge.DesignSystem.Components.AI.Vision;

/// <summary>
/// Type of vision analysis to perform.
/// </summary>
public enum XFAiVisionAnalysisType
{
    /// <summary>General description of the image.</summary>
    Describe,
    /// <summary>Extract text (OCR).</summary>
    Ocr,
    /// <summary>Detect and classify objects.</summary>
    DetectObjects,
    /// <summary>Analyze charts/graphs.</summary>
    AnalyzeChart,
    /// <summary>Read documents (invoices, receipts).</summary>
    ReadDocument,
    /// <summary>Compare multiple images.</summary>
    Compare
}

/// <summary>
/// Result from a vision analysis operation.
/// </summary>
public sealed record XFAiVisionResult
{
    /// <summary>Text description/analysis.</summary>
    public required string Text { get; init; }

    /// <summary>Structured data extracted (JSON string).</summary>
    public string? StructuredData { get; init; }

    /// <summary>Confidence score (0.0–1.0).</summary>
    public float? Confidence { get; init; }

    /// <summary>Analysis type used.</summary>
    public XFAiVisionAnalysisType AnalysisType { get; init; }

    /// <summary>Number of images analyzed.</summary>
    public int ImageCount { get; init; }

    /// <summary>Timestamp of the analysis.</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
