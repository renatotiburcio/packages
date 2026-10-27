using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// AI provider for Azure OpenAI Service.
/// Extends OpenAiProvider with Azure-specific URL construction and API versioning.
/// </summary>
public sealed class AzureOpenAiProvider : AiProviderBase
{
    private readonly string _resourceName;
    private readonly string _deploymentName;
    private readonly string _apiKey;
    private readonly string _apiVersion;

    public AzureOpenAiProvider(
        HttpClient httpClient,
        string resourceName,
        string deploymentName,
        string apiKey,
        string apiVersion = "2024-10-21")
        : base(httpClient)
    {
        _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        _deploymentName = deploymentName ?? throw new ArgumentNullException(nameof(deploymentName));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _apiVersion = apiVersion;
    }

    public override string ProviderId => "azure-openai";
    public override string DisplayName => "Azure OpenAI";
    public override bool SupportsStreaming => true;
    public override bool SupportsToolCalling => true;
    public override string? DefaultModel => _deploymentName;

    private string BaseUrl => $"https://{_resourceName}.openai.azure.com/openai/deployments/{_deploymentName}";

    public override async Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/chat/completions?api-version={_apiVersion}";
        var body = BuildRequestBody(request, stream: false);
        HttpClient.DefaultRequestHeaders.Remove("api-key");
        HttpClient.DefaultRequestHeaders.Add("api-key", _apiKey);
        var json = await PostJsonAsync(url, body, cancellationToken).ConfigureAwait(false);
        var response = Deserialize<AzureChatResponse>(json);
        return MapResponse(response);
    }

    public override async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiChatRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/chat/completions?api-version={_apiVersion}";
        var body = BuildRequestBody(request, stream: true);
        HttpClient.DefaultRequestHeaders.Remove("api-key");
        HttpClient.DefaultRequestHeaders.Add("api-key", _apiKey);
        var stream = await PostJsonStreamAsync(url, body, cancellationToken).ConfigureAwait(false);
        var chunkIndex = 0;
        await foreach (var line in ReadSseLinesAsync(stream, cancellationToken))
        {
            var chunk = Deserialize<AzureStreamChunk>(line);
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
        // Azure embedding uses a separate deployment — not supported in this base provider.
        throw new NotSupportedException("Azure OpenAI embedding requires a separate embedding deployment. Use OpenAiProvider with Azure endpoint instead.");
    }

    public override Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>([_deploymentName]);
    }

    private object BuildRequestBody(AiChatRequest request, bool stream)
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
        return new { messages, temperature = request.Temperature, max_tokens = request.MaxTokens, stream };
    }

    private static AiChatResponse MapResponse(AzureChatResponse? response)
    {
        var choice = response?.Choices?.FirstOrDefault();
        return new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = choice?.Message?.Content ?? string.Empty },
            FinishReason = choice?.FinishReason,
            Usage = response?.Usage != null ? new AiTokenUsage
            {
                PromptTokens = response.Usage.PromptTokens,
                CompletionTokens = response.Usage.CompletionTokens
            } : null
        };
    }

    private sealed record AzureChatResponse(List<AzureChoice>? Choices, AzureUsage? Usage);
    private sealed record AzureChoice(AzureMessageData? Message, AzureDelta? Delta, string? FinishReason);
    private sealed record AzureMessageData(string? Content);
    private sealed record AzureDelta(string? Content);
    private sealed record AzureUsage(int PromptTokens, int CompletionTokens);
    private sealed record AzureStreamChunk(List<AzureChoice>? Choices);
}
