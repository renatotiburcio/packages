using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// In-memory implementation of IAiAudioProvider for testing and development.
/// Returns simulated audio processing results.
/// </summary>
public class InMemoryAiAudioProvider : IAiAudioProvider
{
    /// <inheritdoc />
    public string ProviderId => "inmemory-audio";

    /// <inheritdoc />
    public string DisplayName => "In-Memory Audio (Dev)";

    /// <inheritdoc />
    public bool SupportsStreamingTranscription => false;

    /// <inheritdoc />
    public bool SupportsTextToSpeech { get; init; } = true;

    /// <inheritdoc />
    public long MaxAudioSizeBytes => 25 * 1024 * 1024; // 25MB

    /// <inheritdoc />
    public IReadOnlyList<string> SupportedFormats => new[] { "mp3", "wav", "webm", "ogg", "m4a" };

    /// <inheritdoc />
    public Task<AiTranscriptionResponse> TranscribeAsync(AiTranscriptionRequest request, CancellationToken cancellationToken = default)
    {
        var response = new AiTranscriptionResponse
        {
            Text = "Bom dia, gostaria de verificar o status do pedido número 12345. " +
                   "O cliente ligou ontem reclamando do atraso na entrega. " +
                   "Precisamos priorizar esse envio para manter o relacionamento.",
            DetectedLanguage = request.Language ?? "pt-BR",
            DurationSeconds = 12.5f,
            Segments = new List<AiTranscriptionSegment>
            {
                new()
                {
                    Text = "Bom dia, gostaria de verificar o status do pedido número 12345.",
                    Start = 0.0f,
                    End = 4.2f,
                    Words = new List<AiTranscriptionWord>
                    {
                        new() { Word = "Bom", Start = 0.0f, End = 0.3f, Confidence = 0.98f },
                        new() { Word = "dia,", Start = 0.3f, End = 0.7f, Confidence = 0.97f },
                        new() { Word = "gostaria", Start = 0.8f, End = 1.3f, Confidence = 0.95f }
                    }
                },
                new()
                {
                    Text = "O cliente ligou ontem reclamando do atraso na entrega.",
                    Start = 4.5f,
                    End = 8.1f
                },
                new()
                {
                    Text = "Precisamos priorizar esse envio para manter o relacionamento.",
                    Start = 8.3f,
                    End = 12.5f
                }
            },
            Usage = new AiTokenUsage { PromptTokens = 30, CompletionTokens = 65 },
            Model = "inmemory-whisper-v1"
        };

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<AiTranscriptionChunk> TranscribeStreamAsync(AiTranscriptionRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chunks = new[]
        {
            new AiTranscriptionChunk { Text = "Bom ", IsFinal = false, Timestamp = 0.0f, Confidence = 0.95f },
            new AiTranscriptionChunk { Text = "dia, ", IsFinal = false, Timestamp = 0.3f, Confidence = 0.97f },
            new AiTranscriptionChunk { Text = "gostaria ", IsFinal = false, Timestamp = 0.8f, Confidence = 0.94f },
            new AiTranscriptionChunk { Text = "de verificar ", IsFinal = false, Timestamp = 1.3f, Confidence = 0.92f },
            new AiTranscriptionChunk { Text = "o status do pedido.", IsFinal = true, Timestamp = 2.5f, Confidence = 0.96f }
        };

        foreach (var chunk in chunks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return chunk;
            await Task.Delay(50, cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task<AiSpeechResponse> SynthesizeSpeechAsync(AiSpeechRequest request, CancellationToken cancellationToken = default)
    {
        // Return a small placeholder base64 audio (silent WAV)
        var response = new AiSpeechResponse
        {
            Audio = Convert.ToBase64String(new byte[] { 0x52, 0x49, 0x46, 0x46 }), // "RIFF" header
            Format = request.Format,
            DurationSeconds = request.Text.Length * 0.06f, // ~60ms per char
            Model = request.Model ?? "inmemory-tts-v1"
        };

        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public Task<AiAudioAnalysisResponse> AnalyzeAudioAsync(AiAudioAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        var response = new AiAudioAnalysisResponse
        {
            Summary = "Áudio contém uma conversa telefônica sobre status de pedido. " +
                      "Tone profissional, solicitando priorização de envio.",
            Sentiment = "neutral",
            Topics = new[] { "pedido", "entrega", "cliente", "atraso", "priorização" },
            DetectedLanguage = request.Language ?? "pt-BR",
            DurationSeconds = 12.5f,
            Usage = new AiTokenUsage { PromptTokens = 20, CompletionTokens = 45 }
        };

        return Task.FromResult(response);
    }
}
