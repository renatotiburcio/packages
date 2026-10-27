using System;
using System.Collections.Generic;
using System.Linq;

namespace XForge.DesignSystem.Utilities;

/// <summary>
/// Reusable text search ranking utility.
/// Scores text entries against a query using exact, prefix, contains, word-prefix, and fuzzy matching.
/// </summary>
public static class XFSearchRanker
{
    /// <summary>
    /// Calculate a relevance score for a label+slug pair against a query.
    /// Returns 0 if no match.
    /// </summary>
    /// <param name="normalizedLabel">Lowercase label text.</param>
    /// <param name="normalizedSlug">Lowercase slug/identifier text.</param>
    /// <param name="query">Lowercase search query.</param>
    public static int Score(string normalizedLabel, string normalizedSlug, string query)
    {
        if (string.IsNullOrEmpty(query)) return 0;

        // Exact match on label
        if (normalizedLabel == query) return 100;

        // Exact match on slug
        if (normalizedSlug == query) return 95;

        // Label starts with query
        if (normalizedLabel.StartsWith(query, StringComparison.Ordinal)) return 80;

        // Slug starts with query
        if (normalizedSlug.StartsWith(query, StringComparison.Ordinal)) return 75;

        // Label contains query as a whole word
        if (normalizedLabel.Contains($" {query}", StringComparison.Ordinal) ||
            normalizedLabel.Contains($"-{query}", StringComparison.Ordinal))
            return 60;

        // Label contains query
        if (normalizedLabel.Contains(query, StringComparison.Ordinal)) return 50;

        // Slug contains query
        if (normalizedSlug.Contains(query, StringComparison.Ordinal)) return 45;

        // Word-prefix match: each query word matches the start of a label word
        if (IsWordPrefixMatch(normalizedLabel, query)) return 40;

        // Fuzzy match: all query characters appear in order in the label
        if (IsFuzzyMatch(normalizedLabel, query)) return 30;

        return 0;
    }

    /// <summary>
    /// Rank a list of entries by query relevance.
    /// </summary>
    /// <typeparam name="T">Entry type.</typeparam>
    /// <param name="entries">Entries to search.</param>
    /// <param name="query">Search query.</param>
    /// <param name="labelSelector">Extracts the display label from an entry.</param>
    /// <param name="slugSelector">Extracts the slug/identifier from an entry.</param>
    /// <param name="maxResults">Maximum results to return.</param>
    public static IReadOnlyList<(T Entry, int Score)> Rank<T>(
        IEnumerable<T> entries,
        string? query,
        Func<T, string> labelSelector,
        Func<T, string> slugSelector,
        int maxResults = 15)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<(T, int)>();
        }

        var normalizedQuery = query.Trim().ToLowerInvariant();

        return entries
            .Select(entry => (
                Entry: entry,
                Score: Score(
                    labelSelector(entry).ToLowerInvariant(),
                    slugSelector(entry).ToLowerInvariant(),
                    normalizedQuery)
            ))
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => labelSelector(x.Entry))
            .Take(maxResults)
            .ToList();
    }

    /// <summary>
    /// Checks if all query characters appear in order within the text (fuzzy subsequence match).
    /// </summary>
    public static bool IsFuzzyMatch(string text, string query)
    {
        var queryIndex = 0;
        for (var i = 0; i < text.Length && queryIndex < query.Length; i++)
        {
            if (text[i] == query[queryIndex])
            {
                queryIndex++;
            }
        }

        return queryIndex == query.Length;
    }

    /// <summary>
    /// Checks if each word in the query matches the start of a word in the label.
    /// </summary>
    public static bool IsWordPrefixMatch(string label, string query)
    {
        var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (queryWords.Length <= 1) return false;

        var labelWords = label.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        return queryWords.All(qw => labelWords.Any(lw => lw.StartsWith(qw, StringComparison.Ordinal)));
    }
}
