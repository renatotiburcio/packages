using System.Text.Json;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Web search service using Bing Web Search API v7.
/// Requires a Bing Search API key from Azure Cognitive Services.
/// </summary>
public sealed class BingWebSearchService : IWebSearchService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly int _maxResults;

    public BingWebSearchService(HttpClient httpClient, string apiKey, string baseUrl = "https://api.bing.microsoft.com/v7.0", int maxResults = 10)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _baseUrl = baseUrl.TrimEnd('/');
        _maxResults = maxResults;
    }

    public bool IsAvailable => !string.IsNullOrEmpty(_apiKey);
    public string ProviderId => "bing";

    public async Task<WebSearchResponse> SearchAsync(WebSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Uri.EscapeDataString(request.Query);
        var count = Math.Min(request.MaxResults, _maxResults);
        var url = $"{_baseUrl}/search?q={query}&count={count}&mkt={request.Language ?? "pt-BR"}&safeSearch={request.SafeSearch}";

        if (!string.IsNullOrEmpty(request.Freshness))
            url += $"&freshness={request.Freshness}";
        if (!string.IsNullOrEmpty(request.SiteRestriction))
            url += $"+{Uri.EscapeDataString(request.SiteRestriction)}";

        _httpClient.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", _apiKey);

        var start = DateTimeOffset.UtcNow;
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var duration = DateTimeOffset.UtcNow - start;

        var bingResponse = JsonSerializer.Deserialize<BingSearchResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var results = bingResponse?.WebPages?.Value?.Select(r => new WebSearchResult
        {
            Title = r.Name ?? "",
            Url = r.Url ?? "",
            Snippet = r.Snippet,
            Domain = r.Url is { Length: > 0 } u ? new Uri(u).Host : null,
            PublishedDate = r.DateLastCrawled,
            Score = r.IsFamilyFriendly == true ? 1.0 : 0.8
        }).ToList() ?? [];

        return new WebSearchResponse
        {
            Results = results,
            TotalResults = bingResponse?.WebPages?.TotalEstimatedMatches ?? 0,
            Query = request.Query,
            Provider = "bing",
            Duration = duration
        };
    }

    public async Task<WebContentResponse> ExtractContentAsync(string url, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", _apiKey);

        var apiUrl = $"{_baseUrl}/web/url={Uri.EscapeDataString(url)}&mkt=pt-BR";
        try
        {
            var response = await _httpClient.GetAsync(apiUrl, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<BingWebResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new WebContentResponse
            {
                Content = result?.Snippet ?? "",
                Title = result?.Name,
                Success = true,
                WordCount = (result?.Snippet ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
            };
        }
        catch (Exception ex)
        {
            return new WebContentResponse { Content = "", Success = false, Error = ex.Message };
        }
    }

    public async Task<IReadOnlyList<WebSearchResult>> SearchAndExtractAsync(string query, int maxResults = 5, CancellationToken cancellationToken = default)
    {
        var searchResponse = await SearchAsync(new WebSearchRequest { Query = query, MaxResults = maxResults }, cancellationToken).ConfigureAwait(false);
        return searchResponse.Results;
    }

    // Bing API DTOs
    private sealed record BingSearchResponse(BingWebPages? WebPages);
    private sealed record BingWebPages(long TotalEstimatedMatches, List<BingWebPage>? Value);
    private sealed record BingWebPage(string? Name, string? Url, string? Snippet, DateTimeOffset? DateLastCrawled, bool? IsFamilyFriendly);
    private sealed record BingWebResponse(string? Name, string? Url, string? Snippet);
}
