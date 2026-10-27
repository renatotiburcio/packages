namespace XForge.DesignSystem.Components.AI.Orchestration;

/// <summary>
/// Represents a single log entry from an agent in the orchestration console.
/// </summary>
public sealed record XFAiAgentLogEntry
{
    /// <summary>Unique log entry identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Timestamp of the log entry (UTC).</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>ID of the agent that generated this log.</summary>
    public string AgentId { get; init; } = string.Empty;

    /// <summary>Display name of the agent.</summary>
    public string AgentName { get; init; } = string.Empty;

    /// <summary>Log level.</summary>
    public XFAiAgentLogLevel Level { get; init; } = XFAiAgentLogLevel.Info;

    /// <summary>Log message.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Optional tool name if this is a tool call log.</summary>
    public string? ToolName { get; init; }

    /// <summary>Optional token usage for this entry.</summary>
    public int? TokensUsed { get; init; }

    /// <summary>Optional cost in USD for this entry.</summary>
    public decimal? CostUsd { get; init; }

    /// <summary>Duration in milliseconds.</summary>
    public int? DurationMs { get; init; }

    /// <summary>Arbitrary metadata.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Log level for agent entries.
/// </summary>
public enum XFAiAgentLogLevel
{
    /// <summary>Debug information.</summary>
    Debug,

    /// <summary>Informational message.</summary>
    Info,

    /// <summary>Warning message.</summary>
    Warning,

    /// <summary>Error message.</summary>
    Error,

    /// <summary>Tool execution log.</summary>
    Tool,

    /// <summary>Memory operation log.</summary>
    Memory
}
