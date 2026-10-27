using System.Text.Json;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Web search service using Brave Search API.
/// Free tier available: https://brave.com/search/api/
/// </summary>
public sealed class BraveWebSearchService : IWebSearchService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public BraveWebSearchService(HttpClient httpClient, string apiKey, string baseUrl = "https://api.search.brave.com/res/v1")
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public bool IsAvailable => !string.IsNullOrEmpty(_apiKey);
    public string ProviderId => "brave";

    public async Task<WebSearchResponse> SearchAsync(WebSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Uri.EscapeDataString(request.Query);
        var count = Math.Min(request.MaxResults, 20);
        var url = $"{_baseUrl}/web/search?q={query}&count={count}";

        if (!string.IsNullOrEmpty(request.Language))
            url += $"&search_lang={request.Language}";
        if (!string.IsNullOrEmpty(request.Country))
            url += $"&country={request.Country}";
        if (!string.IsNullOrEmpty(request.Freshness))
            url += $"&freshness={request.Freshness}";

        _httpClient.DefaultRequestHeaders.Remove("Accept");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Remove("Accept-Encoding");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip");
        _httpClient.DefaultRequestHeaders.Remove("X-Subscription-Token");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Subscription-Token", _apiKey);

        var start = DateTimeOffset.UtcNow;
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var duration = DateTimeOffset.UtcNow - start;

        var braveResponse = JsonSerializer.Deserialize<BraveSearchResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var results = braveResponse?.Web?.Results?.Select(r => new WebSearchResult
        {
            Title = r.Title ?? "",
            Url = r.Url ?? "",
            Snippet = r.Description,
            Domain = r.Url is { Length: > 0 } u ? new Uri(u).Host : null,
            PublishedDate = r.Age,
            FaviconUrl = r.Icon
        }).ToList() ?? [];

        return new WebSearchResponse
        {
            Results = results,
            TotalResults = braveResponse?.Web?.TotalResults ?? 0,
            Query = request.Query,
            Provider = "brave",
            Duration = duration
        };
    }

    public async Task<WebContentResponse> ExtractContentAsync(string url, CancellationToken cancellationToken = default)
    {
        // Brave doesn't have a content extraction API, return basic info
        return await Task.FromResult(new WebContentResponse
        {
            Content = "",
            Success = false,
            Error = "Brave Search does not support content extraction. Use SearchAndExtractAsync for search results with snippets."
        }).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<WebSearchResult>> SearchAndExtractAsync(string query, int maxResults = 5, CancellationToken cancellationToken = default)
    {
        var searchResponse = await SearchAsync(new WebSearchRequest { Query = query, MaxResults = maxResults }, cancellationToken).ConfigureAwait(false);
        return searchResponse.Results;
    }

    // Brave API DTOs
    private sealed record BraveSearchResponse(BraveWebData? Web);
    private sealed record BraveWebData(long TotalResults, List<BraveWebResult>? Results);
    private sealed record BraveWebResult(string? Title, string? Url, string? Description, DateTimeOffset? Age, string? Icon);
}
