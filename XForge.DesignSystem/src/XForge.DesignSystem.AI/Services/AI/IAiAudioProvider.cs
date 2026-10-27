using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Abstraction for AI audio providers (Whisper, Deepgram, Google Speech).
/// Handles speech-to-text, text-to-speech, and audio analysis.
/// </summary>
public interface IAiAudioProvider
{
    /// <summary>Unique identifier for this audio provider.</summary>
    string ProviderId { get; }

    /// <summary>Display name for this audio provider.</summary>
    string DisplayName { get; }

    /// <summary>Whether this provider supports real-time streaming transcription.</summary>
    bool SupportsStreamingTranscription { get; }

    /// <summary>Whether this provider supports text-to-speech.</summary>
    bool SupportsTextToSpeech { get; init; }

    /// <summary>Maximum audio file size in bytes.</summary>
    long MaxAudioSizeBytes { get; }

    /// <summary>Supported audio formats (e.g., "mp3", "wav", "webm").</summary>
    IReadOnlyList<string> SupportedFormats { get; }

    /// <summary>
    /// Transcribes audio to text.
    /// </summary>
    Task<AiTranscriptionResponse> TranscribeAsync(AiTranscriptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Transcribes audio with streaming partial results.
    /// </summary>
    IAsyncEnumerable<AiTranscriptionChunk> TranscribeStreamAsync(AiTranscriptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts text to speech audio.
    /// </summary>
    Task<AiSpeechResponse> SynthesizeSpeechAsync(AiSpeechRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes audio content (sentiment, topics, summary).
    /// </summary>
    Task<AiAudioAnalysisResponse> AnalyzeAudioAsync(AiAudioAnalysisRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for audio transcription.
/// </summary>
public sealed record AiTranscriptionRequest
{
    /// <summary>Audio data as base64 string.</summary>
    public required string Audio { get; init; }

    /// <summary>Audio format (e.g., "mp3", "wav", "webm").</summary>
    public string Format { get; init; } = "mp3";

    /// <summary>Language hint (e.g., "pt-BR", "en-US").</summary>
    public string? Language { get; init; }

    /// <summary>Model to use for transcription.</summary>
    public string? Model { get; init; }

    /// <summary>Whether to include timestamps.</summary>
    public bool IncludeTimestamps { get; init; }

    /// <summary>Whether to include word-level confidence.</summary>
    public bool IncludeWordConfidence { get; init; }

    /// <summary>Custom vocabulary or terminology hints.</summary>
    public IReadOnlyList<string>? Vocabulary { get; init; }
}

/// <summary>
/// Response from audio transcription.
/// </summary>
public sealed record AiTranscriptionResponse
{
    /// <summary>Full transcribed text.</summary>
    public required string Text { get; init; }

    /// <summary>Detected language.</summary>
    public string? DetectedLanguage { get; init; }

    /// <summary>Duration of the audio in seconds.</summary>
    public float? DurationSeconds { get; init; }

    /// <summary>Segments with timestamps.</summary>
    public IReadOnlyList<AiTranscriptionSegment>? Segments { get; init; }

    /// <summary>Token usage.</summary>
    public AiTokenUsage? Usage { get; init; }

    /// <summary>Model used.</summary>
    public string? Model { get; init; }
}

/// <summary>
/// A transcription segment with timing.
/// </summary>
public sealed record AiTranscriptionSegment
{
    /// <summary>Segment text.</summary>
    public required string Text { get; init; }

    /// <summary>Start time in seconds.</summary>
    public float Start { get; init; }

    /// <summary>End time in seconds.</summary>
    public float End { get; init; }

    /// <summary>Word-level details.</summary>
    public IReadOnlyList<AiTranscriptionWord>? Words { get; init; }
}

/// <summary>
/// A word with timing and confidence.
/// </summary>
public sealed record AiTranscriptionWord
{
    /// <summary>The word.</summary>
    public required string Word { get; init; }

    /// <summary>Start time in seconds.</summary>
    public float Start { get; init; }

    /// <summary>End time in seconds.</summary>
    public float End { get; init; }

    /// <summary>Confidence score (0.0–1.0).</summary>
    public float Confidence { get; init; }
}

/// <summary>
/// A streaming transcription chunk.
/// </summary>
public sealed record AiTranscriptionChunk
{
    /// <summary>Partial or final text.</summary>
    public required string Text { get; init; }

    /// <summary>Whether this is the final chunk.</summary>
    public bool IsFinal { get; init; }

    /// <summary>Timestamp in seconds.</summary>
    public float? Timestamp { get; init; }

    /// <summary>Confidence score.</summary>
    public float? Confidence { get; init; }
}

/// <summary>
/// Request for text-to-speech synthesis.
/// </summary>
public sealed record AiSpeechRequest
{
    /// <summary>Text to convert to speech.</summary>
    public required string Text { get; init; }

    /// <summary>Voice identifier (provider-specific).</summary>
    public string? Voice { get; init; }

    /// <summary>Language code (e.g., "pt-BR").</summary>
    public string? Language { get; init; }

    /// <summary>Speech speed (0.5–2.0, default 1.0).</summary>
    public float Speed { get; init; } = 1.0f;

    /// <summary>Audio format for output (e.g., "mp3", "wav").</summary>
    public string Format { get; init; } = "mp3";

    /// <summary>Model to use.</summary>
    public string? Model { get; init; }
}

/// <summary>
/// Response from text-to-speech synthesis.
/// </summary>
public sealed record AiSpeechResponse
{
    /// <summary>Audio data as base64 string.</summary>
    public required string Audio { get; init; }

    /// <summary>Audio format.</summary>
    public string Format { get; init; } = "mp3";

    /// <summary>Duration in seconds.</summary>
    public float? DurationSeconds { get; init; }

    /// <summary>Model used.</summary>
    public string? Model { get; init; }
}

/// <summary>
/// Request for audio content analysis.
/// </summary>
public sealed record AiAudioAnalysisRequest
{
    /// <summary>Audio data as base64 string.</summary>
    public required string Audio { get; init; }

    /// <summary>Audio format.</summary>
    public string Format { get; init; } = "mp3";

    /// <summary>Analysis types to perform.</summary>
    public IReadOnlyList<string>? AnalysisTypes { get; init; }

    /// <summary>Language hint.</summary>
    public string? Language { get; init; }
}

/// <summary>
/// Response from audio content analysis.
/// </summary>
public sealed record AiAudioAnalysisResponse
{
    /// <summary>Summary of the audio content.</summary>
    public string? Summary { get; init; }

    /// <summary>Detected sentiment (positive/negative/neutral).</summary>
    public string? Sentiment { get; init; }

    /// <summary>Detected topics/keywords.</summary>
    public IReadOnlyList<string>? Topics { get; init; }

    /// <summary>Detected language.</summary>
    public string? DetectedLanguage { get; init; }

    /// <summary>Duration in seconds.</summary>
    public float? DurationSeconds { get; init; }

    /// <summary>Token usage.</summary>
    public AiTokenUsage? Usage { get; init; }
}
