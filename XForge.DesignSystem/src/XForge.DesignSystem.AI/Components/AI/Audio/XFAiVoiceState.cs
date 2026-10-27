namespace XForge.DesignSystem.Components.AI.Audio;

/// <summary>
/// State of the voice assistant.
/// </summary>
public enum XFAiVoiceState
{
    /// <summary>Idle, waiting for input.</summary>
    Idle,
    /// <summary>Listening/recording audio.</summary>
    Listening,
    /// <summary>Processing/transcribing audio.</summary>
    Processing,
    /// <summary>Speaking/playing audio response.</summary>
    Speaking,
    /// <summary>Error state.</summary>
    Error
}

/// <summary>
/// Result from a transcription operation.
/// </summary>
public sealed record XFAiTranscriptionResult
{
    /// <summary>Transcribed text.</summary>
    public required string Text { get; init; }

    /// <summary>Detected language.</summary>
    public string? Language { get; init; }

    /// <summary>Duration of the audio in seconds.</summary>
    public float? DurationSeconds { get; init; }

    /// <summary>Confidence score (0.0–1.0).</summary>
    public float? Confidence { get; init; }

    /// <summary>Whether this is a partial (streaming) result.</summary>
    public bool IsPartial { get; init; }

    /// <summary>Timestamp of the transcription.</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
