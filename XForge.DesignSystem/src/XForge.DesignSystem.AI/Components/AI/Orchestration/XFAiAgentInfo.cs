namespace XForge.DesignSystem.Components.AI.Orchestration;

/// <summary>
/// Represents an AI agent in the orchestration hierarchy.
/// </summary>
public sealed record XFAiAgentInfo
{
    /// <summary>Unique agent identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Display name of the agent.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Agent role in the hierarchy.</summary>
    public XFAiAgentRole Role { get; init; } = XFAiAgentRole.Worker;

    /// <summary>Current operational status.</summary>
    public XFAiAgentStatus Status { get; init; } = XFAiAgentStatus.Idle;

    /// <summary>Agent description or specialization.</summary>
    public string? Description { get; init; }

    /// <summary>Parent agent ID for hierarchy display.</summary>
    public string? ParentId { get; init; }

    /// <summary>Number of completed tasks.</summary>
    public int TasksCompleted { get; init; }

    /// <summary>Number of failed tasks.</summary>
    public int TasksFailed { get; init; }

    /// <summary>Total tokens consumed.</summary>
    public long TotalTokens { get; init; }

    /// <summary>Estimated cost in USD.</summary>
    public decimal CostUsd { get; init; }

    /// <summary>Number of tool calls made.</summary>
    public int ToolCalls { get; init; }

    /// <summary>Number of memory entries stored.</summary>
    public int MemoryEntries { get; init; }

    /// <summary>Agent uptime in seconds.</summary>
    public int UptimeSeconds { get; init; }

    /// <summary>Last activity timestamp.</summary>
    public DateTime LastActivity { get; init; } = DateTime.UtcNow;

    /// <summary>Current task description.</summary>
    public string? CurrentTask { get; init; }

    /// <summary>Agent model (e.g., "gpt-4o", "claude-3.5-sonnet").</summary>
    public string? Model { get; init; }

    /// <summary>Queue depth (pending tasks).</summary>
    public int QueueDepth { get; init; }

    /// <summary>Success rate (0.0–1.0).</summary>
    public double SuccessRate => TasksCompleted + TasksFailed > 0
        ? (double)TasksCompleted / (TasksCompleted + TasksFailed)
        : 1.0;
}

/// <summary>
/// Agent role in the orchestration hierarchy.
/// </summary>
public enum XFAiAgentRole
{
    /// <summary>Top-level orchestrator.</summary>
    CEO,

    /// <summary>Domain director managing specialists.</summary>
    Director,

    /// <summary>Specialized agent for specific tasks.</summary>
    Specialist,

    /// <summary>Worker agent executing individual tasks.</summary>
    Worker
}

/// <summary>
/// Agent operational status.
/// </summary>
public enum XFAiAgentStatus
{
    /// <summary>Agent is idle, waiting for tasks.</summary>
    Idle,

    /// <summary>Agent is actively processing.</summary>
    Running,

    /// <summary>Agent encountered an error.</summary>
    Error,

    /// <summary>Agent is stopped/paused.</summary>
    Stopped
}
