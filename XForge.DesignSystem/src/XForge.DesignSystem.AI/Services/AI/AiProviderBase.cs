using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Base class for AI provider implementations.
/// Provides shared HTTP infrastructure, JSON serialization, and common patterns.
/// </summary>
public abstract class AiProviderBase : IAiProvider, IAsyncDisposable
{
    /// <summary>HTTP client for API calls. Accessible by derived provider classes.</summary>
    protected readonly HttpClient HttpClient;
    private readonly bool _ownsHttpClient;

    /// <summary>
    /// JSON serializer options used for provider request/response serialization.
    /// </summary>
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    protected AiProviderBase(HttpClient httpClient, bool ownsHttpClient = false)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = ownsHttpClient;
    }

    /// <inheritdoc/>
    public abstract string ProviderId { get; }

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <inheritdoc/>
    public abstract bool SupportsStreaming { get; }

    /// <inheritdoc/>
    public abstract bool SupportsToolCalling { get; }

    /// <inheritdoc/>
    public abstract string? DefaultModel { get; }

    /// <inheritdoc/>
    public abstract Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default);

    /// <summary>Sends an HTTP POST with JSON body and returns the response content string.</summary>
    protected async Task<string> PostJsonAsync(string url, object body, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Sends an HTTP GET and returns the response content string.</summary>
    protected async Task<string> GetAsync(string url, CancellationToken cancellationToken)
    {
        var response = await HttpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Sends an HTTP POST and returns the response stream for SSE reading.</summary>
    protected async Task<Stream> PostJsonStreamAsync(string url, object body, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Deserializes a JSON string to the specified type.</summary>
    protected static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonOptions);

    /// <summary>Yields SSE data lines from a stream, skipping event/id/comment lines.</summary>
    protected static async IAsyncEnumerable<string> ReadSseLinesAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null) break;
            if (line.StartsWith("data: ", StringComparison.Ordinal))
            {
                var data = line[6..];
                if (data == "[DONE]") yield break;
                yield return data;
            }
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        if (_ownsHttpClient)
        {
            HttpClient.Dispose();
        }
        GC.SuppressFinalize(this);
        await ValueTask.CompletedTask;
    }
}
