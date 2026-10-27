using System.Runtime.CompilerServices;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for Ollama local models.
/// Communicates with the Ollama HTTP API (default: http://localhost:11434).
/// </summary>
public sealed class OllamaProvider : AiProviderBase
{
    private readonly string _baseUrl;

    public OllamaProvider(HttpClient httpClient, string baseUrl = "http://localhost:11434")
        : base(httpClient)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public override string ProviderId => "ollama";
    public override string DisplayName => "Ollama";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => true;
    public override string? DefaultModel => "llama3";

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? DefaultModel ?? "llama3";
        var body = BuildRequestBody(request, model, stream: false);
        var json = await PostJsonAsync($"{_baseUrl}/api/chat", body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<OllamaChatResponse>(json);
        return MapResponse(response, model);
    }

    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? DefaultModel ?? "llama3";
        var body = BuildRequestBody(request, model, stream: true);
        await foreach (var line in ReadSseLinesAsync(await PostJsonStreamAsync($"{_baseUrl}/api/chat", body, cancellationToken), cancellationToken))
        {
            var chunk = Deserialize<OllamaStreamChunk>(line);
            if (chunk?.Message?.Content is { Length: > 0 } content)
            {
                yield return new AiStreamChunk { Delta = content, ChunkIndex = 0 };
            }
            if (chunk?.Done == true)
            {
                yield return new AiStreamChunk { FinishReason = "stop", ChunkIndex = 1 };
                yield break;
            }
        }
    }

    public override async Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? "nomic-embed-text";
        var embeddings = new List<float[]>();
        foreach (var input in request.Inputs)
        {
            var body = new { model, input };
            var json = await PostJsonAsync($"{_baseUrl}/api/embed", body, cancellationToken).ConfigureAwait(false);
            var result = Deserialize<OllamaEmbedResponse>(json);
            if (result?.Embeddings is { Count: > 0 } emb)
            {
                embeddings.AddRange(emb);
            }
        }
        return new AiEmbeddingResponse { Embeddings = embeddings, Model = model };
    }

    public override async Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        var json = await GetAsync($"{_baseUrl}/api/tags", cancellationToken).ConfigureAwait(false);
        var result = Deserialize<OllamaModelList>(json);
        return result?.Models?.Select(m => m.Name).ToList() ?? [];
    }

    private static object BuildRequestBody(AiChatRequest request, string model, bool stream)
    {
        var messages = request.Messages.Select(m => new
        {
            role = m.Role.ToString().ToLowerInvariant(),
            content = m.Content
        });
        return new { model, messages, stream };
    }

    private static AiChatResponse MapResponse(OllamaChatResponse? response, string model)
    {
        return new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = response?.Message?.Content ?? string.Empty
            },
            FinishReason = response?.Done == true ? "stop" : null,
            Usage = response != null ? new AiTokenUsage
            {
                PromptTokens = response.PromptEvalCount,
                CompletionTokens = response.EvalCount,
                Model = model
            } : null
        };
    }

    // Ollama API DTOs
    private sealed record OllamaChatResponse(OllamaMessage? Message, bool Done, int PromptEvalCount, int EvalCount);
    private sealed record OllamaMessage(string? Content);
    private sealed record OllamaStreamChunk(OllamaMessage? Message, bool Done);
    private sealed record OllamaEmbedResponse(List<float[]>? Embeddings);
    private sealed record OllamaModelList(List<OllamaModel>? Models);
    private sealed record OllamaModel(string Name);
}
