using System.Runtime.CompilerServices;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for OpenAI-compatible APIs (OpenAI, LM Studio, vLLM, etc.).
/// Uses the /v1/chat/completions endpoint.
/// THREAD SAFETY: Uses per-request headers via HttpRequestMessage instead of DefaultRequestHeaders.
/// </summary>
public sealed class OpenAiProvider : AiProviderBase
{
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public OpenAiProvider(HttpClient httpClient, string apiKey, string baseUrl = "https://api.openai.com/v1")
        : base(httpClient)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public override string ProviderId => "openai";
    public override string DisplayName => "OpenAI";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => true;
    public override string? DefaultModel => "gpt-4o";

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var body = BuildRequestBody(request, stream: false);
        var json = await SendPostAsync($"{_baseUrl}/chat/completions", body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<OpenAiChatResponse>(json);
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
            var chunk = Deserialize<OpenAiStreamChunk>(line);
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
        var model = request.Model ?? "text-embedding-3-small";
        var body = new { model, input = request.Inputs, dimensions = request.Dimensions };
        var json = await SendPostAsync($"{_baseUrl}/embeddings", body, cancellationToken).ConfigureAwait(false);
        var result = Deserialize<OpenAiEmbeddingResponse>(json);
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
        var result = Deserialize<OpenAiModelList>(json);
        return result?.Data?.Select(m => m.Id).ToList() ?? [];
    }

    /// <summary>
    /// THREAD SAFETY FIX: Uses per-request HttpRequestMessage with Authorization header
    /// instead of mutating HttpClient.DefaultRequestHeaders (which is not thread-safe).
    /// </summary>
    private async Task<string> SendPostAsync(string url, object body, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        var response = await HttpClient.SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
    }

    private async Task<Stream> SendPostStreamAsync(string url, object body, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
    }

    private async Task<string> SendGetAsync(string url, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        var response = await HttpClient.SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
    }

    private object BuildRequestBody(AiChatRequest request, bool stream)
    {
        var model = request.Model ?? DefaultModel ?? "gpt-4o";
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

    private static AiChatResponse MapResponse(OpenAiChatResponse? response)
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

    // OpenAI API DTOs
    private sealed record OpenAiChatResponse(string? Id, string? Model, List<OpenAiChoice>? Choices, OpenAiUsage? Usage);
    private sealed record OpenAiChoice(OpenAiMessageData? Message, OpenAiDelta? Delta, string? FinishReason, int Index);
    private sealed record OpenAiMessageData(string? Content, string? Role);
    private sealed record OpenAiDelta(string? Content);
    private sealed record OpenAiUsage(int PromptTokens, int CompletionTokens);
    private sealed record OpenAiStreamChunk(List<OpenAiChoice>? Choices);
    private sealed record OpenAiEmbeddingResponse(List<OpenAiEmbeddingData>? Data, OpenAiUsage? Usage);
    private sealed record OpenAiEmbeddingData(float[] Embedding, int Index);
    private sealed record OpenAiModelList(List<OpenAiModelData>? Data);
    private sealed record OpenAiModelData(string Id);
}
