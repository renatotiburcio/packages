namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Web search service for AI-powered online research.
/// Provides search, content extraction, and summarization capabilities.
/// </summary>
public interface IWebSearchService
{
    /// <summary>Whether this search service is available and configured.</summary>
    bool IsAvailable { get; }

    /// <summary>Search provider identifier.</summary>
    string ProviderId { get; }

    /// <summary>
    /// Performs a web search and returns results.
    /// </summary>
    Task<WebSearchResponse> SearchAsync(WebSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts the main content from a URL.
    /// </summary>
    Task<WebContentResponse> ExtractContentAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches and extracts content from the top results for RAG augmentation.
    /// </summary>
    Task<IReadOnlyList<WebSearchResult>> SearchAndExtractAsync(string query, int maxResults = 5, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for web search.
/// </summary>
public sealed record WebSearchRequest
{
    /// <summary>Search query.</summary>
    public required string Query { get; init; }

    /// <summary>Maximum results to return.</summary>
    public int MaxResults { get; init; } = 10;

    /// <summary>Language code (e.g., "pt-BR", "en-US").</summary>
    public string? Language { get; init; }

    /// <summary>Country code (e.g., "BR", "US").</summary>
    public string? Country { get; init; }

    /// <summary>Safe search level: "off", "moderate", "strict".</summary>
    public string SafeSearch { get; init; } = "moderate";

    /// <summary>Date range restriction (e.g., "day", "week", "month", "year").</summary>
    public string? Freshness { get; init; }

    /// <summary>Site restriction (e.g., "site:gov.br").</summary>
    public string? SiteRestriction { get; init; }
}

/// <summary>
/// Response from web search.
/// </summary>
public sealed record WebSearchResponse
{
    /// <summary>Search results.</summary>
    public required IReadOnlyList<WebSearchResult> Results { get; init; }

    /// <summary>Total results available (may be more than returned).</summary>
    public long TotalResults { get; init; }

    /// <summary>Search query that was executed.</summary>
    public string? Query { get; init; }

    /// <summary>Search provider used.</summary>
    public string? Provider { get; init; }

    /// <summary>Time taken for the search.</summary>
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// A single web search result.
/// </summary>
public sealed record WebSearchResult
{
    /// <summary>Result title.</summary>
    public required string Title { get; init; }

    /// <summary>Result URL.</summary>
    public required string Url { get; init; }

    /// <summary>Result snippet/description.</summary>
    public string? Snippet { get; init; }

    /// <summary>Full extracted content (if available).</summary>
    public string? Content { get; init; }

    /// <summary>Source domain.</summary>
    public string? Domain { get; init; }

    /// <summary>Published date (if available).</summary>
    public DateTimeOffset? PublishedDate { get; init; }

    /// <summary>Relevance score (0.0-1.0).</summary>
    public double Score { get; init; }

    /// <summary>Favicon URL.</summary>
    public string? FaviconUrl { get; init; }
}

/// <summary>
/// Response from content extraction.
/// </summary>
public sealed record WebContentResponse
{
    /// <summary>Extracted main text content.</summary>
    public required string Content { get; init; }

    /// <summary>Page title.</summary>
    public string? Title { get; init; }

    /// <summary>Page description/meta.</summary>
    public string? Description { get; init; }

    /// <summary>Author (if available).</summary>
    public string? Author { get; init; }

    /// <summary>Published date (if available).</summary>
    public DateTimeOffset? PublishedDate { get; init; }

    /// <summary>Word count of extracted content.</summary>
    public int WordCount { get; init; }

    /// <summary>Whether extraction was successful.</summary>
    public bool Success { get; init; } = true;

    /// <summary>Error message if extraction failed.</summary>
    public string? Error { get; init; }
}
