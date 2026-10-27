using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for DeepSeek API (https://api.deepseek.com).
/// Uses OpenAI-compatible /v1/chat/completions endpoint.
/// </summary>
public sealed class DeepSeekProvider : AiProviderBase
{
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public DeepSeekProvider(
        HttpClient httpClient,
        string apiKey,
        string baseUrl = "https://api.deepseek.com/v1")
        : base(httpClient)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public override string ProviderId => "deepseek";
    public override string DisplayName => "DeepSeek";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => true;
    public override string? DefaultModel => "deepseek-chat";

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var body = BuildRequestBody(request, stream: false);
        var json = await SendPostAsync($"{_baseUrl}/chat/completions", body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<DeepSeekChatResponse>(json);
        return MapResponse(response);
    }

    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var body = BuildRequestBody(request, stream: true);
        var stream = await SendPostStreamAsync($"{_baseUrl}/chat/completions", body, cancellationToken).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(stream, cancellationToken))
        {
            var chunk = Deserialize<DeepSeekStreamChunk>(line);
            var delta = chunk?.Choices?.FirstOrDefault()?.Delta;
            if (delta != null)
            {
                yield return new AiStreamChunk
                {
                    Delta = delta.Content,
                    FinishReason = chunk?.Choices?.FirstOrDefault()?.FinishReason,
                    ChunkIndex = chunkIndex++
                };
            }
        }
    }

    public override Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("DeepSeek does not provide an embedding API. Use OpenAI or Ollama for embeddings.");
    }

    public override Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>([
            "deepseek-chat",
            "deepseek-reasoner"
        ]);
    }

    private async Task<string> SendPostAsync(string url, object body, CancellationToken ct)
    {
        ConfigureHeaders();
        return await PostJsonAsync(url, body, ct).ConfigureAwait(false);
    }

    private async Task<Stream> SendPostStreamAsync(string url, object body, CancellationToken ct)
    {
        ConfigureHeaders();
        return await PostJsonStreamAsync(url, body, ct).ConfigureAwait(false);
    }

    private void ConfigureHeaders()
    {
        HttpClient.DefaultRequestHeaders.Remove("Authorization");
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_apiKey}");
    }

    private object BuildRequestBody(AiChatRequest request, bool stream)
    {
        var model = request.Model ?? DefaultModel ?? "deepseek-chat";
        var messages = new List<object>();
        if (request.SystemPrompt is { Length: > 0 })
        {
            messages.Add(new { role = "system", content = request.SystemPrompt });
        }
        foreach (var m in request.Messages)
        {
            messages.Add(new { role = m.Role.ToString().ToLowerInvariant(), content = m.Content });
        }
        return new
        {
            model,
            messages,
            temperature = request.Temperature,
            max_tokens = request.MaxTokens,
            top_p = request.TopP,
            stream
        };
    }

    private static AiChatResponse MapResponse(DeepSeekChatResponse? response)
    {
        var choice = response?.Choices?.FirstOrDefault();
        return new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = choice?.Message?.Content ?? string.Empty
            },
            FinishReason = choice?.FinishReason,
            ResponseId = response?.Id,
            Usage = response?.Usage != null ? new AiTokenUsage
            {
                PromptTokens = response.Usage.PromptTokens,
                CompletionTokens = response.Usage.CompletionTokens,
                Model = response?.Model
            } : null
        };
    }

    private sealed record DeepSeekChatResponse(string? Id, string? Model, List<DeepSeekChoice>? Choices, DeepSeekUsage? Usage);
    private sealed record DeepSeekChoice(DeepSeekMessageData? Message, DeepSeekDelta? Delta, string? FinishReason, int Index);
    private sealed record DeepSeekMessageData(string? Content, string? Role);
    private sealed record DeepSeekDelta(string? Content);
    private sealed record DeepSeekUsage(int PromptTokens, int CompletionTokens);
    private sealed record DeepSeekStreamChunk(List<DeepSeekChoice>? Choices);
}
