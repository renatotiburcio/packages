using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Abstraction for AI vision providers (GPT-4V, Claude Vision, Gemini Pro Vision).
/// Handles image analysis, OCR, document scanning, and chart reading.
/// </summary>
public interface IAiVisionProvider
{
    /// <summary>Unique identifier for this vision provider.</summary>
    string ProviderId { get; }

    /// <summary>Display name for this vision provider.</summary>
    string DisplayName { get; }

    /// <summary>Whether this provider supports streaming for vision responses.</summary>
    bool SupportsStreaming { get; }

    /// <summary>Maximum number of images per request.</summary>
    int MaxImagesPerRequest { get; }

    /// <summary>Maximum image size in bytes.</summary>
    long MaxImageSizeBytes { get; }

    /// <summary>
    /// Analyzes one or more images with a text prompt and returns the response.
    /// </summary>
    Task<AiVisionResponse> AnalyzeAsync(AiVisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes images with streaming response.
    /// </summary>
    IAsyncEnumerable<AiStreamChunk> AnalyzeStreamAsync(AiVisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts text from images (OCR).
    /// </summary>
    Task<AiOcrResponse> ExtractTextAsync(AiOcrRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for image analysis.
/// </summary>
public sealed record AiVisionRequest
{
    /// <summary>Text prompt describing what to analyze.</summary>
    public required string Prompt { get; init; }

    /// <summary>Image data as base64 strings (with or without data URI prefix).</summary>
    public required IReadOnlyList<string> Images { get; init; }

    /// <summary>MIME types for each image (e.g., "image/png", "image/jpeg").</summary>
    public IReadOnlyList<string>? MimeTypes { get; init; }

    /// <summary>Model to use for vision analysis.</summary>
    public string? Model { get; init; }

    /// <summary>Maximum tokens in the response.</summary>
    public int? MaxTokens { get; init; }

    /// <summary>Temperature for response generation.</summary>
    public float? Temperature { get; init; }

    /// <summary>System prompt for the vision model.</summary>
    public string? SystemPrompt { get; init; }
}

/// <summary>
/// Response from image analysis.
/// </summary>
public sealed record AiVisionResponse
{
    /// <summary>Text analysis result.</summary>
    public required string Text { get; init; }

    /// <summary>Structured data extracted from the image (JSON).</summary>
    public string? StructuredData { get; init; }

    /// <summary>Confidence score (0.0–1.0).</summary>
    public float? Confidence { get; init; }

    /// <summary>Detected objects/regions.</summary>
    public IReadOnlyList<AiVisionDetection>? Detections { get; init; }

    /// <summary>Token usage for the request.</summary>
    public AiTokenUsage? Usage { get; init; }

    /// <summary>Model used for the analysis.</summary>
    public string? Model { get; init; }
}

/// <summary>
/// A detected object/region in an image.
/// </summary>
public sealed record AiVisionDetection
{
    /// <summary>Label/class of the detected object.</summary>
    public required string Label { get; init; }

    /// <summary>Confidence score (0.0–1.0).</summary>
    public float Confidence { get; init; }

    /// <summary>Bounding box (normalized 0.0–1.0): [x, y, width, height].</summary>
    public float[]? BoundingBox { get; init; }

    /// <summary>Additional description of the detection.</summary>
    public string? Description { get; init; }
}

/// <summary>
/// Request for OCR text extraction.
/// </summary>
public sealed record AiOcrRequest
{
    /// <summary>Image data as base64 string.</summary>
    public required string Image { get; init; }

    /// <summary>MIME type (e.g., "image/png").</summary>
    public string? MimeType { get; init; }

    /// <summary>Language hint for OCR (e.g., "pt-BR", "en-US").</summary>
    public string? Language { get; init; }

    /// <summary>Whether to preserve layout/structure.</summary>
    public bool PreserveLayout { get; init; }
}

/// <summary>
/// Response from OCR text extraction.
/// </summary>
public sealed record AiOcrResponse
{
    /// <summary>Extracted text.</summary>
    public required string Text { get; init; }

    /// <summary>Confidence score (0.0–1.0).</summary>
    public float? Confidence { get; init; }

    /// <summary>Detected language.</summary>
    public string? DetectedLanguage { get; init; }

    /// <summary>Text regions with position info.</summary>
    public IReadOnlyList<AiOcrRegion>? Regions { get; init; }

    /// <summary>Token usage.</summary>
    public AiTokenUsage? Usage { get; init; }
}

/// <summary>
/// A text region detected by OCR.
/// </summary>
public sealed record AiOcrRegion
{
    /// <summary>Text content of this region.</summary>
    public required string Text { get; init; }

    /// <summary>Bounding box (normalized 0.0–1.0): [x, y, width, height].</summary>
    public float[]? BoundingBox { get; init; }

    /// <summary>Confidence for this region.</summary>
    public float Confidence { get; init; }
}
