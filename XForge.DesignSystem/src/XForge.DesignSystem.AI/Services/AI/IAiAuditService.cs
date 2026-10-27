using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Audit trail service for AI interactions.
/// Logs prompts, completions, token usage, cost, risk scores,
/// and provides query capabilities for compliance and observability.
/// </summary>
public interface IAiAuditService
{
    /// <summary>
    /// Logs an AI interaction (request + response pair).
    /// </summary>
    Task LogInteractionAsync(AiAuditEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries audit entries with optional filters.
    /// </summary>
    Task<IReadOnlyList<AiAuditEntry>> QueryAsync(AiAuditQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns aggregated statistics for a date range.
    /// </summary>
    Task<AiAuditStats> GetStatsAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns total token usage and cost for a user.
    /// </summary>
    Task<AiTokenUsage> GetUserUsageAsync(string userId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}

/// <summary>
/// A single audit trail entry for an AI interaction.
/// </summary>
public sealed record AiAuditEntry
{
    /// <summary>Unique identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>Provider used (e.g., "openai", "ollama").</summary>
    public required string Provider { get; init; }

    /// <summary>Model used (e.g., "gpt-4o").</summary>
    public string? Model { get; init; }

    /// <summary>User who initiated the interaction.</summary>
    public string? UserId { get; init; }

    /// <summary>Session identifier.</summary>
    public string? SessionId { get; init; }

    /// <summary>The prompt/input text (may be redacted for PII).</summary>
    public string? Prompt { get; init; }

    /// <summary>The completion/output text.</summary>
    public string? Completion { get; init; }

    /// <summary>Token usage metrics.</summary>
    public AiTokenUsage? Usage { get; init; }

    /// <summary>Risk score (0.0–1.0) for security/compliance review.</summary>
    public double RiskScore { get; init; }

    /// <summary>Tool calls made during this interaction.</summary>
    public IReadOnlyList<AiToolCall>? ToolCalls { get; init; }

    /// <summary>Interaction duration.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Whether the interaction completed successfully.</summary>
    public bool Success { get; init; } = true;

    /// <summary>Error message if the interaction failed.</summary>
    public string? Error { get; init; }

    /// <summary>When the interaction occurred (UTC).</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Additional metadata.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Query parameters for filtering audit entries.
/// </summary>
public sealed record AiAuditQuery
{
    /// <summary>Filter by provider.</summary>
    public string? Provider { get; init; }

    /// <summary>Filter by user ID.</summary>
    public string? UserId { get; init; }

    /// <summary>Filter by session ID.</summary>
    public string? SessionId { get; init; }

    /// <summary>Filter entries from this date (inclusive).</summary>
    public DateTimeOffset? From { get; init; }

    /// <summary>Filter entries to this date (inclusive).</summary>
    public DateTimeOffset? To { get; init; }

    /// <summary>Minimum risk score filter.</summary>
    public double? MinRiskScore { get; init; }

    /// <summary>Filter by success/failure.</summary>
    public bool? SuccessOnly { get; init; }

    /// <summary>Maximum number of entries to return.</summary>
    public int Limit { get; init; } = 100;

    /// <summary>Offset for pagination.</summary>
    public int Offset { get; init; }
}

/// <summary>
/// Aggregated audit statistics.
/// </summary>
public sealed record AiAuditStats
{
    /// <summary>Total number of interactions.</summary>
    public int TotalInteractions { get; init; }

    /// <summary>Number of successful interactions.</summary>
    public int SuccessfulInteractions { get; init; }

    /// <summary>Number of failed interactions.</summary>
    public int FailedInteractions => TotalInteractions - SuccessfulInteractions;

    /// <summary>Total prompt tokens consumed.</summary>
    public long TotalPromptTokens { get; init; }

    /// <summary>Total completion tokens consumed.</summary>
    public long TotalCompletionTokens { get; init; }

    /// <summary>Total estimated cost in USD.</summary>
    public decimal TotalCostUsd { get; init; }

    /// <summary>Average risk score.</summary>
    public double AverageRiskScore { get; init; }

    /// <summary>Average response duration.</summary>
    public TimeSpan AverageDuration { get; init; }
}
