using System.Runtime.CompilerServices;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for OpenRouter (https://openrouter.ai).
/// OpenRouter aggregates multiple AI providers behind an OpenAI-compatible API.
/// Requires extra headers: HTTP-Referer and X-Title for app attribution.
/// </summary>
public sealed class OpenRouterProvider : AiProviderBase
{
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _httpReferer;
    private readonly string _appTitle;

    public OpenRouterProvider(
        HttpClient httpClient,
        string apiKey,
        string httpReferer = "https://xforge.designsystem",
        string appTitle = "XForge Design System",
        string baseUrl = "https://openrouter.ai/api/v1")
        : base(httpClient)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _httpReferer = httpReferer;
        _appTitle = appTitle;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public override string ProviderId => "openrouter";
    public override string DisplayName => "OpenRouter";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => true;
    public override string? DefaultModel => "anthropic/claude-sonnet-4-20250514";

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var body = BuildRequestBody(request, stream: false);
        var json = await SendPostAsync($"{_baseUrl}/chat/completions", body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<OpenRouterChatResponse>(json);
        return MapResponse(response);
    }

    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var body = BuildRequestBody(request, stream: true);
        var stream = await SendPostStreamAsync($"{_baseUrl}/chat/completions", body, cancellationToken).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(stream, cancellationToken))
        {
            var chunk = Deserialize<OpenRouterStreamChunk>(line);
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

    public override async Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? "openai/text-embedding-3-small";
        var body = new { model, input = request.Inputs, dimensions = request.Dimensions };
        var json = await SendPostAsync($"{_baseUrl}/embeddings", body, cancellationToken).ConfigureAwait(false);
        var result = Deserialize<OpenRouterEmbeddingResponse>(json);
        return new AiEmbeddingResponse
        {
            Embeddings = result?.Data?.Select(d => d.Embedding.ToArray()).ToList() ?? [],
            Model = model,
            Usage = result?.Usage != null ? new AiTokenUsage { PromptTokens = result.Usage.PromptTokens, Model = model } : null
        };
    }

    public override async Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        var json = await SendGetAsync($"{_baseUrl}/models", cancellationToken).ConfigureAwait(false);
        var result = Deserialize<OpenRouterModelList>(json);
        return result?.Data?.Select(m => m.Id).ToList() ?? [];
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

    private async Task<string> SendGetAsync(string url, CancellationToken ct)
    {
        ConfigureHeaders();
        return await GetAsync(url, ct).ConfigureAwait(false);
    }

    private void ConfigureHeaders()
    {
        HttpClient.DefaultRequestHeaders.Remove("Authorization");
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_apiKey}");
        HttpClient.DefaultRequestHeaders.Remove("HTTP-Referer");
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("HTTP-Referer", _httpReferer);
        HttpClient.DefaultRequestHeaders.Remove("X-Title");
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Title", _appTitle);
    }

    private object BuildRequestBody(AiChatRequest request, bool stream)
    {
        var model = request.Model ?? DefaultModel ?? "anthropic/claude-sonnet-4-20250514";
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

    private static AiChatResponse MapResponse(OpenRouterChatResponse? response)
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

    // OpenRouter API DTOs (OpenAI-compatible)
    private sealed record OpenRouterChatResponse(string? Id, string? Model, List<OpenRouterChoice>? Choices, OpenRouterUsage? Usage);
    private sealed record OpenRouterChoice(OpenRouterMessageData? Message, OpenRouterDelta? Delta, string? FinishReason, int Index);
    private sealed record OpenRouterMessageData(string? Content, string? Role);
    private sealed record OpenRouterDelta(string? Content);
    private sealed record OpenRouterUsage(int PromptTokens, int CompletionTokens);
    private sealed record OpenRouterStreamChunk(List<OpenRouterChoice>? Choices);
    private sealed record OpenRouterEmbeddingResponse(List<OpenRouterEmbeddingData>? Data, OpenRouterUsage? Usage);
    private sealed record OpenRouterEmbeddingData(float[] Embedding, int Index);
    private sealed record OpenRouterModelList(List<OpenRouterModelData>? Data);
    private sealed record OpenRouterModelData(string Id);
}
