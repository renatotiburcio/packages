using System.Collections.Concurrent;
using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// In-memory implementation of IAiAuditService.
/// Suitable for development, testing, and single-instance deployments.
/// Production deployments should replace with a persistent implementation (database).
/// </summary>
public sealed class InMemoryAiAuditService : IAiAuditService
{
    private readonly ConcurrentBag<AiAuditEntry> _entries = [];

    /// <inheritdoc/>
    public Task LogInteractionAsync(AiAuditEntry entry, CancellationToken cancellationToken = default)
    {
        _entries.Add(entry with { Timestamp = DateTimeOffset.UtcNow });
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<AiAuditEntry>> QueryAsync(AiAuditQuery query, CancellationToken cancellationToken = default)
    {
        var results = _entries.AsEnumerable();

        if (query.Provider is { Length: > 0 })
            results = results.Where(e => e.Provider == query.Provider);
        if (query.UserId is { Length: > 0 })
            results = results.Where(e => e.UserId == query.UserId);
        if (query.SessionId is { Length: > 0 })
            results = results.Where(e => e.SessionId == query.SessionId);
        if (query.From.HasValue)
            results = results.Where(e => e.Timestamp >= query.From.Value);
        if (query.To.HasValue)
            results = results.Where(e => e.Timestamp <= query.To.Value);
        if (query.MinRiskScore.HasValue)
            results = results.Where(e => e.RiskScore >= query.MinRiskScore.Value);
        if (query.SuccessOnly.HasValue)
            results = results.Where(e => e.Success == query.SuccessOnly.Value);

        var list = results
            .OrderByDescending(e => e.Timestamp)
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToList();

        return Task.FromResult<IReadOnlyList<AiAuditEntry>>(list);
    }

    /// <inheritdoc/>
    public Task<AiAuditStats> GetStatsAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => e.Timestamp >= from && e.Timestamp <= to).ToList();
        var stats = new AiAuditStats
        {
            TotalInteractions = entries.Count,
            SuccessfulInteractions = entries.Count(e => e.Success),
            TotalPromptTokens = entries.Sum(e => e.Usage?.PromptTokens ?? 0),
            TotalCompletionTokens = entries.Sum(e => e.Usage?.CompletionTokens ?? 0),
            TotalCostUsd = entries.Sum(e => e.Usage?.EstimatedCostUsd ?? 0),
            AverageRiskScore = entries.Count > 0 ? entries.Average(e => e.RiskScore) : 0,
            AverageDuration = entries.Count > 0 && entries.Any(e => e.Duration.HasValue)
                ? TimeSpan.FromMilliseconds(entries.Where(e => e.Duration.HasValue).Average(e => e.Duration!.Value.TotalMilliseconds))
                : TimeSpan.Zero
        };
        return Task.FromResult(stats);
    }

    /// <inheritdoc/>
    public Task<AiTokenUsage> GetUserUsageAsync(string userId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => e.UserId == userId && e.Timestamp >= from && e.Timestamp <= to).ToList();
        var usage = new AiTokenUsage
        {
            PromptTokens = entries.Sum(e => e.Usage?.PromptTokens ?? 0),
            CompletionTokens = entries.Sum(e => e.Usage?.CompletionTokens ?? 0),
            EstimatedCostUsd = entries.Sum(e => e.Usage?.EstimatedCostUsd ?? 0)
        };
        return Task.FromResult(usage);
    }
}
