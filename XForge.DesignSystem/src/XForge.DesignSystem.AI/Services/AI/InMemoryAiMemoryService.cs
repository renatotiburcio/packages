using System.Collections.Concurrent;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// In-memory implementation of IAiMemoryService.
/// Suitable for development, testing, and single-instance deployments.
/// Production deployments should replace with a persistent implementation (database/vector store).
/// </summary>
public sealed class InMemoryAiMemoryService : IAiMemoryService
{
    private readonly ConcurrentDictionary<string, AiMemoryEntry> _memories = new();

    /// <inheritdoc/>
    public Task StoreAsync(AiMemoryEntry entry, CancellationToken cancellationToken = default)
    {
        _memories[entry.Id] = entry with { CreatedAt = DateTimeOffset.UtcNow };
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<AiMemoryEntry>> RecallAsync(string query, AiMemoryScope scope, int topK = 10, CancellationToken cancellationToken = default)
    {
        var queryLower = query.ToLowerInvariant();
        var results = _memories.Values
            .Where(m => m.Scope == scope)
            .Select(m =>
            {
                // Simple keyword scoring — production would use vector similarity
                var contentLower = m.Content.ToLowerInvariant();
                var score = contentLower.Contains(queryLower) ? 1.0 : 0.0;
                // Bonus for exact word match
                if (queryLower.Split(' ').All(w => contentLower.Contains(w))) score += 0.5;
                return m with { Score = score };
            })
            .Where(m => m.Score > 0)
            .OrderByDescending(m => m.Score)
            .ThenByDescending(m => m.CreatedAt)
            .Take(topK)
            .ToList();

        // Update access stats
        foreach (var r in results)
        {
            _memories[r.Id] = r with { LastAccessedAt = DateTimeOffset.UtcNow, AccessCount = r.AccessCount + 1 };
        }

        return Task.FromResult<IReadOnlyList<AiMemoryEntry>>(results);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<AiMemoryEntry>> GetSessionMemoriesAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var results = _memories.Values
            .Where(m => m.Scope == AiMemoryScope.Session && m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<AiMemoryEntry>>(results);
    }

    /// <inheritdoc/>
    public Task<bool> ForgetAsync(string memoryId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_memories.TryRemove(memoryId, out _));
    }

    /// <inheritdoc/>
    public Task<int> ClearAsync(AiMemoryScope scope, string? sessionId = null, CancellationToken cancellationToken = default)
    {
        var toRemove = _memories.Values
            .Where(m => m.Scope == scope && (sessionId == null || m.SessionId == sessionId))
            .Select(m => m.Id)
            .ToList();

        var removed = 0;
        foreach (var id in toRemove)
        {
            if (_memories.TryRemove(id, out _)) removed++;
        }
        return Task.FromResult(removed);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyDictionary<AiMemoryScope, int>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = _memories.Values
            .GroupBy(m => m.Scope)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all scopes are present
        foreach (var scope in Enum.GetValues<AiMemoryScope>())
        {
            stats.TryAdd(scope, 0);
        }

        return Task.FromResult<IReadOnlyDictionary<AiMemoryScope, int>>(stats);
    }
}
