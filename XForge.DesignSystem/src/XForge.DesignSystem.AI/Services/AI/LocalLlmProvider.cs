using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for local/custom LLM endpoints (LM Studio, vLLM, text-generation-inference, etc.).
/// Uses OpenAI-compatible API format against any local HTTP endpoint.
/// </summary>
public sealed class LocalLlmProvider : AiProviderBase
{
    private readonly string _baseUrl;
    private readonly string? _apiKey;

    public LocalLlmProvider(HttpClient httpClient, string baseUrl, string? apiKey = null)
        : base(httpClient)
    {
        _baseUrl = (baseUrl ?? throw new ArgumentNullException(nameof(baseUrl))).TrimEnd('/');
        _apiKey = apiKey;
    }

    public override string ProviderId => "local-llm";
    public override string DisplayName => "Local LLM";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => false;
    public override string? DefaultModel => null;

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? "default";
        var body = BuildOpenAiCompatibleBody(request, model, stream: false);
        var json = await SendPostAsync($"{_baseUrl}/v1/chat/completions", body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<LocalChatResponse>(json);
        return MapResponse(response, model);
    }

    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? "default";
        var body = BuildOpenAiCompatibleBody(request, model, stream: true);
        var stream = await SendPostStreamAsync($"{_baseUrl}/v1/chat/completions", body, cancellationToken).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(stream, cancellationToken))
        {
            var chunk = Deserialize<LocalStreamChunk>(line);
            var delta = chunk?.Choices?.FirstOrDefault()?.Delta;
            if (delta?.Content is { Length: > 0 })
            {
                yield return new AiStreamChunk { Delta = delta.Content, ChunkIndex = chunkIndex++ };
            }
            if (chunk?.Choices?.FirstOrDefault()?.FinishReason is { Length: > 0 } reason)
            {
                yield return new AiStreamChunk { FinishReason = reason, ChunkIndex = chunkIndex };
                yield break;
            }
        }
    }

    public override Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Local LLM provider does not support embedding. Use Ollama or OpenAI for embeddings.");
    }

    public override Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>([]);
    }

    private async Task<string> SendPostAsync(string url, object body, CancellationToken ct)
    {
        ConfigureAuth();
        return await PostJsonAsync(url, body, ct).ConfigureAwait(false);
    }

    private async Task<Stream> SendPostStreamAsync(string url, object body, CancellationToken ct)
    {
        ConfigureAuth();
        return await PostJsonStreamAsync(url, body, ct).ConfigureAwait(false);
    }

    private void ConfigureAuth()
    {
        if (_apiKey is { Length: > 0 })
        {
            HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", _apiKey);
        }
    }

    private static object BuildOpenAiCompatibleBody(AiChatRequest request, string model, bool stream)
    {
        var messages = new List<object>();
        if (request.SystemPrompt is { Length: > 0 })
        {
            messages.Add(new { role = "system", content = request.SystemPrompt });
        }
        foreach (var m in request.Messages)
        {
            messages.Add(new { role = m.Role.ToString().ToLowerInvariant(), content = m.Content });
        }
        return new { model, messages, temperature = request.Temperature, max_tokens = request.MaxTokens, stream };
    }

    private static AiChatResponse MapResponse(LocalChatResponse? response, string model)
    {
        var choice = response?.Choices?.FirstOrDefault();
        return new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = choice?.Message?.Content ?? string.Empty },
            FinishReason = choice?.FinishReason,
            Usage = response?.Usage != null ? new AiTokenUsage
            {
                PromptTokens = response.Usage.PromptTokens,
                CompletionTokens = response.Usage.CompletionTokens,
                Model = model
            } : null
        };
    }

    private sealed record LocalChatResponse(List<LocalChoice>? Choices, LocalUsage? Usage);
    private sealed record LocalChoice(LocalMessageData? Message, LocalDelta? Delta, string? FinishReason);
    private sealed record LocalMessageData(string? Content);
    private sealed record LocalDelta(string? Content);
    private sealed record LocalUsage(int PromptTokens, int CompletionTokens);
    private sealed record LocalStreamChunk(List<LocalChoice>? Choices);
}
