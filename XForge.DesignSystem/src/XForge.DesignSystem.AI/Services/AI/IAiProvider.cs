using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Abstraction for AI provider integrations (Ollama, OpenAI, Azure, Anthropic, Gemini, LocalLlm).
/// Each implementation handles HTTP communication, authentication, and response parsing
/// for a specific AI provider API.
/// </summary>
public interface IAiProvider
{
    /// <summary>Unique identifier for this provider (e.g., "openai", "ollama").</summary>
    string ProviderId { get; }

    /// <summary>Display name for this provider.</summary>
    string DisplayName { get; }

    /// <summary>Whether this provider supports streaming responses.</summary>
    bool SupportsStreaming { get; }

    /// <summary>Whether this provider supports tool/function calling.</summary>
    bool SupportsToolCalling { get; }

    /// <summary>Default model identifier for this provider.</summary>
    string? DefaultModel { get; }

    /// <summary>
    /// Sends a chat completion request and returns the full response.
    /// </summary>
    Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a chat completion request and yields streaming chunks.
    /// </summary>
    IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for the given text inputs.
    /// </summary>
    Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the list of available models for this provider.
    /// </summary>
    Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for generating embeddings.
/// </summary>
public sealed record AiEmbeddingRequest
{
    /// <summary>Text inputs to generate embeddings for.</summary>
    public required IReadOnlyList<string> Inputs { get; init; }

    /// <summary>Model to use for embeddings (provider-specific).</summary>
    public string? Model { get; init; }

    /// <summary>Embedding dimensions (if supported by the provider).</summary>
    public int? Dimensions { get; init; }
}

/// <summary>
/// Response containing generated embeddings.
/// </summary>
public sealed record AiEmbeddingResponse
{
    /// <summary>List of embedding vectors (one per input).</summary>
    public required IReadOnlyList<float[]> Embeddings { get; init; }

    /// <summary>Token usage for the embedding request.</summary>
    public AiTokenUsage? Usage { get; init; }

    /// <summary>Model used for generating embeddings.</summary>
    public string? Model { get; init; }
}
