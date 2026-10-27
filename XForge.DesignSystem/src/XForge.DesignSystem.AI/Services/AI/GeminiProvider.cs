using System.Runtime.CompilerServices;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for Google Gemini API.
/// Uses the /v1beta/models/{model}:generateContent endpoint.
/// SECURITY: Uses x-goog-api-key header instead of URL query parameter.
/// </summary>
public sealed class GeminiProvider : AiProviderBase
{
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public GeminiProvider(HttpClient httpClient, string apiKey, string baseUrl = "https://generativelanguage.googleapis.com/v1beta")
        : base(httpClient)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public override string ProviderId => "gemini";
    public override string DisplayName => "Google Gemini";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => true;
    public override string? DefaultModel => "gemini-2.0-flash";

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? DefaultModel ?? "gemini-2.0-flash";
        var body = BuildRequestBody(request);
        var json = await SendPostAsync($"{_baseUrl}/models/{model}:generateContent", body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<GeminiResponse>(json);
        return MapResponse(response, model);
    }

    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? DefaultModel ?? "gemini-2.0-flash";
        var body = BuildRequestBody(request);
        var stream = await SendPostStreamAsync($"{_baseUrl}/models/{model}:streamGenerateContent?alt=sse", body, cancellationToken).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(stream, cancellationToken))
        {
            var chunk = Deserialize<GeminiResponse>(line);
            var text = chunk?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            if (text is { Length: > 0 })
            {
                yield return new AiStreamChunk { Delta = text, ChunkIndex = chunkIndex++ };
            }
        }
    }

    public override async Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        var model = request.Model ?? "text-embedding-004";
        var embeddings = new List<float[]>();
        foreach (var input in request.Inputs)
        {
            var body = new { content = new { parts = new[] { new { text = input } } } };
            var json = await SendPostAsync($"{_baseUrl}/models/{model}:embedContent", body, cancellationToken).ConfigureAwait(false);
            var result = Deserialize<GeminiEmbedResponse>(json);
            if (result?.Embedding?.Values is { Length: > 0 } values)
            {
                embeddings.Add(values);
            }
        }
        return new AiEmbeddingResponse { Embeddings = embeddings, Model = model };
    }

    public override Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>([
            "gemini-2.0-flash",
            "gemini-2.0-flash-lite",
            "gemini-2.5-pro-preview-05-06",
            "gemini-2.5-flash-preview-05-20",
            "gemini-1.5-pro",
            "gemini-1.5-flash"
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

    /// <summary>
    /// SECURITY FIX: Uses x-goog-api-key header instead of URL query parameter.
    /// URL query parameters are logged by proxies, browsers, and server access logs.
    /// </summary>
    private void ConfigureHeaders()
    {
        HttpClient.DefaultRequestHeaders.Remove("x-goog-api-key");
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", _apiKey);
    }

    private object BuildRequestBody(AiChatRequest request)
    {
        var contents = new List<object>();
        var systemInstruction = request.SystemPrompt is { Length: > 0 }
            ? new { parts = new[] { new { text = request.SystemPrompt } } }
            : null;

        foreach (var m in request.Messages)
        {
            var role = m.Role switch
            {
                AiRole.User => "user",
                AiRole.Assistant => "model",
                _ => "user"
            };
            contents.Add(new { role, parts = new[] { new { text = m.Content } } });
        }
        return new
        {
            contents,
            systemInstruction,
            generationConfig = new
            {
                temperature = request.Temperature,
                maxOutputTokens = request.MaxTokens,
                topP = request.TopP
            }
        };
    }

    private static AiChatResponse MapResponse(GeminiResponse? response, string model)
    {
        var text = response?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        return new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = text ?? string.Empty },
            FinishReason = response?.Candidates?.FirstOrDefault()?.FinishReason,
            Usage = response?.UsageMetadata != null ? new AiTokenUsage
            {
                PromptTokens = response.UsageMetadata.PromptTokenCount,
                CompletionTokens = response.UsageMetadata.CandidatesTokenCount,
                Model = model
            } : null
        };
    }

    private sealed record GeminiResponse(List<GeminiCandidate>? Candidates, GeminiUsageMetadata? UsageMetadata);
    private sealed record GeminiCandidate(GeminiContent? Content, string? FinishReason);
    private sealed record GeminiContent(List<GeminiPart>? Parts);
    private sealed record GeminiPart(string? Text);
    private sealed record GeminiUsageMetadata(int PromptTokenCount, int CandidatesTokenCount);
    private sealed record GeminiEmbedResponse(GeminiEmbeddingData? Embedding);
    private sealed record GeminiEmbeddingData(float[] Values);
}
