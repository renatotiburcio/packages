using System.Runtime.CompilerServices;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for Anthropic Claude API.
/// Uses the /v1/messages endpoint.
/// </summary>
public sealed class AnthropicProvider : AiProviderBase
{
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public AnthropicProvider(HttpClient httpClient, string apiKey, string baseUrl = "https://api.anthropic.com/v1")
        : base(httpClient)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public override string ProviderId => "anthropic";
    public override string DisplayName => "Anthropic (Claude)";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => true;
    public override string? DefaultModel => "claude-sonnet-4-20250514";

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var body = BuildRequestBody(request, stream: false);
        var json = await SendPostAsync($"{_baseUrl}/messages", body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<AnthropicResponse>(json);
        return MapResponse(response);
    }

    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var body = BuildRequestBody(request, stream: true);
        var stream = await SendPostStreamAsync($"{_baseUrl}/messages", body, cancellationToken).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(stream, cancellationToken))
        {
            var chunk = Deserialize<AnthropicStreamEvent>(line);
            if (chunk?.Type == "content_block_delta" && chunk.Delta?.Text is { Length: > 0 } text)
            {
                yield return new AiStreamChunk { Delta = text, ChunkIndex = chunkIndex++ };
            }
            if (chunk?.Type == "message_stop")
            {
                yield return new AiStreamChunk { FinishReason = "stop", ChunkIndex = chunkIndex };
                yield break;
            }
        }
    }

    public override Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Anthropic does not provide an embedding API. Use OpenAI or Ollama for embeddings.");
    }

    public override Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>([
            "claude-opus-4-20250514",
            "claude-sonnet-4-20250514",
            "claude-3-5-haiku-20241022"
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
        HttpClient.DefaultRequestHeaders.Remove("x-api-key");
        HttpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        HttpClient.DefaultRequestHeaders.Remove("anthropic-version");
        HttpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
    }

    private object BuildRequestBody(AiChatRequest request, bool stream)
    {
        var model = request.Model ?? DefaultModel ?? "claude-sonnet-4-20250514";
        var messages = request.Messages.Select(m => new
        {
            role = m.Role.ToString().ToLowerInvariant(),
            content = m.Content
        });
        return new
        {
            model,
            messages,
            max_tokens = request.MaxTokens ?? 4096,
            system = request.SystemPrompt,
            temperature = request.Temperature,
            stream
        };
    }

    private static AiChatResponse MapResponse(AnthropicResponse? response)
    {
        var textContent = response?.Content?.FirstOrDefault(c => c.Type == "text");
        return new AiChatResponse
        {
            Message = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = textContent?.Text ?? string.Empty
            },
            FinishReason = response?.StopReason,
            ResponseId = response?.Id,
            Usage = response?.Usage != null ? new AiTokenUsage
            {
                PromptTokens = response.Usage.InputTokens,
                CompletionTokens = response.Usage.OutputTokens,
                Model = response?.Model
            } : null
        };
    }

    private sealed record AnthropicResponse(string? Id, string? Model, List<AnthropicContent>? Content, string? StopReason, AnthropicUsage? Usage);
    private sealed record AnthropicContent(string? Type, string? Text);
    private sealed record AnthropicUsage(int InputTokens, int OutputTokens);
    private sealed record AnthropicStreamEvent(string? Type, AnthropicStreamDelta? Delta);
    private sealed record AnthropicStreamDelta(string? Text);
}
