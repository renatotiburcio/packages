using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// A generic AI provider that accepts any HTTP endpoint for chat completions.
/// Useful for proxies, custom gateways, local models with non-standard APIs,
/// or any service that can be adapted to the XForge AI contract.
/// </summary>
/// <remarks>
/// Three API format modes are supported:
/// <list type="bullet">
///   <item><see cref="GenericApiFormat.OpenAICompatible"/> — delegates to the OpenAI wire format</item>
///   <item><see cref="GenericApiFormat.OllamaCompatible"/> — delegates to the Ollama wire format</item>
///   <item><see cref="GenericApiFormat.RawHttp"/> — full control via delegate functions</item>
/// </list>
/// For <c>RawHttp</c>, you must supply <see cref="_requestMapper"/>, <see cref="_responseMapper"/>,
/// and <see cref="_chunkMapper"/> delegates to handle serialization and deserialization.
/// </remarks>
public sealed class GenericAiProvider : AiProviderBase
{
    private readonly string? _apiKey;

    /// <summary>
    /// The API format that determines how requests and responses are serialized.
    /// </summary>
    public enum GenericApiFormat
    {
        /// <summary>OpenAI-compatible wire format: /v1/chat/completions with SSE streaming.</summary>
        OpenAICompatible,

        /// <summary>Ollama-compatible wire format: /api/chat with JSON streaming.</summary>
        OllamaCompatible,

        /// <summary>Fully custom HTTP via delegate functions.</summary>
        RawHttp
    }

    private readonly Func<AiChatRequest, object>? _requestMapper;
    private readonly Func<string, AiChatResponse>? _responseMapper;
    private readonly Func<string, AiStreamChunk>? _chunkMapper;
    private readonly string? _customEndpoint;

    /// <summary>
    /// Creates a new generic AI provider.
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls.</param>
    /// <param name="baseUrl">Base URL of the API endpoint (e.g., "http://localhost:8080").</param>
    /// <param name="format">Wire format to use. Default: <see cref="GenericApiFormat.OpenAICompatible"/>.</param>
    /// <param name="apiKey">Optional API key sent as Bearer token in the Authorization header.</param>
    /// <param name="requestMapper">
    /// Custom request mapper for <see cref="GenericApiFormat.RawHttp"/> mode.
    /// Must not be null when format is RawHttp.
    /// </param>
    /// <param name="responseMapper">
    /// Custom response mapper for <see cref="GenericApiFormat.RawHttp"/> mode.
    /// Must not be null when format is RawHttp.
    /// </param>
    /// <param name="chunkMapper">
    /// Custom chunk mapper for <see cref="GenericApiFormat.RawHttp"/> mode.
    /// When null in RawHttp mode, streaming is not supported.
    /// </param>
    /// <param name="customEndpoint">
    /// For RawHttp mode: the full endpoint URL to POST to. If null, baseUrl is used as-is.
    /// For OpenAICompatible/OllamaCompatible: this value is ignored.
    /// </param>
    /// <param name="providerId">Custom provider identifier. Default: "generic".</param>
    /// <param name="displayName">Display name for this provider. Default: "Generic AI".</param>
    /// <param name="supportsToolCalling">
    /// Whether this provider supports tool/function calling.
    /// Default is true for OpenAICompatible and OllamaCompatible, false for RawHttp.
    /// </param>
    /// <param name="supportsEmbeddings">
    /// Whether this provider supports embeddings.
    /// Default is true for OpenAICompatible, false for others.
    /// </param>
    public GenericAiProvider(
        HttpClient httpClient,
        string baseUrl,
        GenericApiFormat format = GenericApiFormat.OpenAICompatible,
        string? apiKey = null,
        Func<AiChatRequest, object>? requestMapper = null,
        Func<string, AiChatResponse>? responseMapper = null,
        Func<string, AiStreamChunk>? chunkMapper = null,
        string? customEndpoint = null,
        string providerId = "generic",
        string displayName = "Generic AI",
        bool? supportsToolCalling = null,
        bool? supportsEmbeddings = null)
        : base(httpClient)
    {
        BaseUrl = (baseUrl ?? throw new ArgumentNullException(nameof(baseUrl))).TrimEnd('/');
        Format = format;
        _apiKey = apiKey;
        _requestMapper = requestMapper;
        _responseMapper = responseMapper;
        _chunkMapper = chunkMapper;
        _customEndpoint = customEndpoint;
        ProviderIdValue = providerId;
        DisplayNameValue = displayName;
        SupportsToolCallingValue = supportsToolCalling ?? (format != GenericApiFormat.RawHttp);
        SupportsEmbeddingsValue = supportsEmbeddings ?? (format == GenericApiFormat.OpenAICompatible);

        if (format == GenericApiFormat.RawHttp)
        {
            if (requestMapper is null)
                throw new ArgumentNullException(nameof(requestMapper), "RawHttp format requires a request mapper.");
            if (responseMapper is null)
                throw new ArgumentNullException(nameof(responseMapper), "RawHttp format requires a response mapper.");
        }
    }

    private string BaseUrl { get; }
    private GenericApiFormat Format { get; }
    private string ProviderIdValue { get; }
    private string DisplayNameValue { get; }
    private bool SupportsToolCallingValue { get; }
    private bool SupportsEmbeddingsValue { get; }

    /// <inheritdoc/>
    public override string ProviderId => ProviderIdValue;

    /// <inheritdoc/>
    public override string DisplayName => DisplayNameValue;

    /// <inheritdoc/>
    public override bool SupportsStreaming => Format switch
    {
        GenericApiFormat.OpenAICompatible or GenericApiFormat.OllamaCompatible => true,
        GenericApiFormat.RawHttp => _chunkMapper is not null,
        _ => false
    };

    /// <inheritdoc/>
    public override bool SupportsToolCalling => SupportsToolCallingValue;

    /// <inheritdoc/>
    public override string? DefaultModel => null;

    /// <inheritdoc/>
    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        return Format switch
        {
            GenericApiFormat.OpenAICompatible => await CompleteOpenAIAsync(request, cancellationToken).ConfigureAwait(false),
            GenericApiFormat.OllamaCompatible => await CompleteOllamaAsync(request, cancellationToken).ConfigureAwait(false),
            GenericApiFormat.RawHttp => await CompleteRawAsync(request, cancellationToken).ConfigureAwait(false),
            _ => throw new NotSupportedException($"Format {Format} is not supported.")
        };
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        switch (Format)
        {
            case GenericApiFormat.OpenAICompatible:
                await foreach (var chunk in StreamOpenAIAsync(request, cancellationToken))
                    yield return chunk;
                yield break;

            case GenericApiFormat.OllamaCompatible:
                await foreach (var chunk in StreamOllamaAsync(request, cancellationToken))
                    yield return chunk;
                yield break;

            case GenericApiFormat.RawHttp:
                if (_chunkMapper is null)
                    throw new NotSupportedException("RawHttp streaming requires a chunk mapper.");
                await foreach (var chunk in StreamRawAsync(request, cancellationToken))
                    yield return chunk;
                yield break;

            default:
                throw new NotSupportedException($"Format {Format} is not supported.");
        }
    }

    /// <inheritdoc/>
    public override async Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default)
    {
        if (!SupportsEmbeddingsValue)
            throw new NotSupportedException($"Embeddings are not supported for provider '{ProviderIdValue}' with format {Format}.");

        if (Format == GenericApiFormat.OpenAICompatible)
        {
            var model = request.Model ?? "text-embedding-3-small";
            var body = new { model, input = request.Inputs, dimensions = request.Dimensions };
            var json = await SendPostAsync($"{BaseUrl}/v1/embeddings", body, cancellationToken).ConfigureAwait(false);
            var result = Deserialize<GenericEmbeddingResponse>(json);
            return new AiEmbeddingResponse
            {
                Embeddings = result?.Data?.Select(d => d.Embedding.ToArray()).ToList() ?? [],
                Model = model,
                Usage = result?.Usage != null ? new AiTokenUsage { PromptTokens = result.Usage.PromptTokens, Model = model } : null
            };
        }

        throw new NotSupportedException($"Embeddings are not supported in {Format} format.");
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        if (Format == GenericApiFormat.OllamaCompatible)
        {
            var json = await SendGetAsync($"{BaseUrl}/api/tags", cancellationToken).ConfigureAwait(false);
            var result = Deserialize<GenericOllamaModelList>(json);
            return result?.Models?.Select(m => m.Name).ToList() ?? [];
        }

        try
        {
            var json = await SendGetAsync($"{BaseUrl}/v1/models", cancellationToken).ConfigureAwait(false);
            var result = Deserialize<GenericModelList>(json);
            return result?.Data?.Select(m => m.Id).ToList() ?? [];
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Performs a health check against the provider API.
    /// </summary>
    public async Task<AiHealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await ListModelsAsync(cancellationToken).ConfigureAwait(false);
            sw.Stop();
            return new AiHealthStatus(true, "Provider is reachable", sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            return new AiHealthStatus(false, ex.Message);
        }
    }

    private async Task<AiChatResponse> CompleteOpenAIAsync(AiChatRequest request, CancellationToken ct)
    {
        var model = request.Model ?? "default";
        var body = BuildOpenAIBody(request, model, stream: false);
        var json = await SendPostAsync($"{BaseUrl}/v1/chat/completions", body, ct).ConfigureAwait(false);
        var response = Deserialize<GenericChatResponse>(json);
        return MapGenericResponse(response, model);
    }

    private async IAsyncEnumerable<AiStreamChunk> StreamOpenAIAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var model = request.Model ?? "default";
        var body = BuildOpenAIBody(request, model, stream: true);
        var responseStream = await SendPostStreamAsync($"{BaseUrl}/v1/chat/completions", body, ct).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(responseStream, ct))
        {
            var chunk = Deserialize<GenericStreamChunk>(line);
            var delta = chunk?.Choices?.FirstOrDefault()?.Delta;
            if (delta is not null)
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

    private static object BuildOpenAIBody(AiChatRequest request, string model, bool stream)
    {
        var messages = new List<object>();
        if (request.SystemPrompt is { Length: > 0 })
            messages.Add(new { role = "system", content = request.SystemPrompt });
        foreach (var m in request.Messages)
            messages.Add(new { role = m.Role.ToString().ToLowerInvariant(), content = m.Content });

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

    private async Task<AiChatResponse> CompleteOllamaAsync(AiChatRequest request, CancellationToken ct)
    {
        var model = request.Model ?? "default";
        var body = BuildOllamaBody(request, model, stream: false);
        var json = await SendPostAsync($"{BaseUrl}/api/chat", body, ct).ConfigureAwait(false);
        var response = Deserialize<GenericOllamaChatResponse>(json);
        return MapOllamaResponse(response, model);
    }

    private async IAsyncEnumerable<AiStreamChunk> StreamOllamaAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var model = request.Model ?? "default";
        var body = BuildOllamaBody(request, model, stream: true);
        var responseStream = await SendPostStreamAsync($"{BaseUrl}/api/chat", body, ct).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(responseStream, ct))
        {
            var chunk = Deserialize<GenericOllamaStreamChunk>(line);
            if (chunk?.Message?.Content is { Length: > 0 } content)
                yield return new AiStreamChunk { Delta = content, ChunkIndex = chunkIndex++ };
            if (chunk?.Done == true)
            {
                yield return new AiStreamChunk { FinishReason = "stop", ChunkIndex = chunkIndex };
                yield break;
            }
        }
    }

    private static object BuildOllamaBody(AiChatRequest request, string model, bool stream)
    {
        var messages = request.Messages.Select(m => new
        {
            role = m.Role.ToString().ToLowerInvariant(),
            content = m.Content
        });
        return new { model, messages, stream };
    }

    private static AiChatResponse MapOllamaResponse(GenericOllamaChatResponse? response, string model)
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

    private async Task<AiChatResponse> CompleteRawAsync(AiChatRequest request, CancellationToken ct)
    {
        var endpoint = _customEndpoint ?? BaseUrl;
        var rawBody = _requestMapper!(request);
        ConfigureAuth();
        var json = await PostJsonAsync(endpoint, rawBody, ct).ConfigureAwait(false);
        return _responseMapper!(json);
    }

    private async IAsyncEnumerable<AiStreamChunk> StreamRawAsync(
        AiChatRequest request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var endpoint = _customEndpoint ?? BaseUrl;
        var rawBody = _requestMapper!(request);
        ConfigureAuth();
        var responseStream = await PostJsonStreamAsync(endpoint, rawBody, ct).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(responseStream, ct))
        {
            var chunk = _chunkMapper!(line);
            yield return chunk with { ChunkIndex = chunkIndex++ };
        }
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

    private async Task<string> SendGetAsync(string url, CancellationToken ct)
    {
        ConfigureAuth();
        return await GetAsync(url, ct).ConfigureAwait(false);
    }

    private void ConfigureAuth()
    {
        if (_apiKey is { Length: > 0 })
            HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", _apiKey);
    }

    private static AiChatResponse MapGenericResponse(GenericChatResponse? response, string model)
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
                Model = model
            } : null
        };
    }

    private sealed record GenericChatResponse(string? Id, List<GenericChoice>? Choices, GenericUsage? Usage);
    private sealed record GenericChoice(GenericMessageData? Message, GenericDelta? Delta, string? FinishReason);
    private sealed record GenericMessageData(string? Content);
    private sealed record GenericDelta(string? Content);
    private sealed record GenericUsage(int PromptTokens, int CompletionTokens);
    private sealed record GenericStreamChunk(List<GenericChoice>? Choices);
    private sealed record GenericEmbeddingResponse(List<GenericEmbeddingData>? Data, GenericUsage? Usage);
    private sealed record GenericEmbeddingData(float[] Embedding, int Index);
    private sealed record GenericModelList(List<GenericModelData>? Data);
    private sealed record GenericModelData(string Id);
    private sealed record GenericOllamaChatResponse(GenericOllamaMessage? Message, bool Done, int PromptEvalCount, int EvalCount);
    private sealed record GenericOllamaMessage(string? Content);
    private sealed record GenericOllamaStreamChunk(GenericOllamaMessage? Message, bool Done);
    private sealed record GenericOllamaModelList(List<GenericOllamaModel>? Models);
    private sealed record GenericOllamaModel(string Name);

}