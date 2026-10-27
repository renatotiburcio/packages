namespace XForge.DesignSystem.Components.AI.Workflow;

// ══════════════════════════════════════════════════════════════════════
// Shared models for Phase 19: Advanced Workflow & Automation (S-V2-003)
// ══════════════════════════════════════════════════════════════════════

#region Enums

/// <summary>Node types for the visual workflow designer.</summary>
public enum XFAiWfNodeType
{
    Start, End, AiProcessor, Rule, Approval, Trigger, Event, Schedule,
    Condition, HttpCall, Transform, Delay, Parallel, Merge, SubProcess
}

/// <summary>Execution status of a workflow or node.</summary>
public enum XFAiWfExecutionStatus
{
    Pending, Running, Completed, Failed, Cancelled, WaitingApproval, Skipped
}

/// <summary>Execution log level.</summary>
public enum XFAiWfLogLevel { Info, Warning, Error, Debug, Success }

/// <summary>Condition comparison operators.</summary>
public enum XFAiConditionOperator
{
    Equals, NotEquals, Contains, GreaterThan, LessThan,
    GreaterOrEqual, LessOrEqual, IsEmpty, IsNotEmpty, StartsWith, EndsWith
}

/// <summary>Logical operators for combining conditions.</summary>
public enum XFAiLogicalOperator { And, Or }

/// <summary>Action types for automation rules.</summary>
public enum XFAiAutomationActionType
{
    SendNotification, ExecuteWorkflow, CallAiProvider,
    UpdateField, SendEmail, TriggerWebhook, LogEvent, WaitForApproval
}

/// <summary>Batch item processing status.</summary>
public enum XFAiBatchItemStatus
{
    Pending, Processing, Completed, Failed, Cancelled, Retrying
}

/// <summary>Batch processing strategy.</summary>
public enum XFAiBatchStrategy { Sequential, Parallel, Batched }

/// <summary>Schedule frequency for recurring jobs.</summary>
public enum XFAiScheduleFrequency
{
    Once, Minutely, Hourly, Daily, Weekly, Monthly, Custom
}

/// <summary>Scheduled job status.</summary>
public enum XFAiScheduledJobStatus
{
    Active, Paused, Completed, Failed, Expired
}

#endregion

#region Workflow Designer V2 Models

/// <summary>Enhanced workflow node for v2 designer with dimensions and ports.</summary>
public sealed record XFAiWfNode
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string Label { get; init; } = string.Empty;
    public XFAiWfNodeType Type { get; init; } = XFAiWfNodeType.AiProcessor;
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; } = 160;
    public double Height { get; init; } = 80;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public bool IsActive { get; init; }
    public bool IsCompleted { get; init; }
    public bool IsDisabled { get; init; }
    public int? Order { get; init; }
    public IReadOnlyDictionary<string, string>? Config { get; init; }
}

/// <summary>Port on a workflow node for connections.</summary>
public sealed record XFAiWfPort
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string NodeId { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public bool IsOutput { get; init; }
    public int Index { get; init; }
}

/// <summary>Connection (edge) between two workflow nodes.</summary>
public sealed record XFAiWfConnection
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string SourceNodeId { get; init; } = string.Empty;
    public string TargetNodeId { get; init; } = string.Empty;
    public string? SourcePortId { get; init; }
    public string? TargetPortId { get; init; }
    public string? Label { get; init; }
    public bool IsConditional { get; init; }
    public bool IsActive { get; init; }
}

#endregion

#region Workflow Executor Models

/// <summary>Complete workflow execution state.</summary>
public sealed record XFAiWfExecutionState
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string WorkflowName { get; init; } = string.Empty;
    public XFAiWfExecutionStatus Status { get; init; } = XFAiWfExecutionStatus.Pending;
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; init; }
    public string? CurrentNodeId { get; init; }
    public int TotalNodes { get; init; }
    public int CompletedNodes { get; init; }
    public int FailedNodes { get; init; }
    public IReadOnlyList<XFAiWfExecLog> Logs { get; init; } = Array.Empty<XFAiWfExecLog>();
    public string? ErrorMessage { get; init; }
    public double Progress => TotalNodes > 0 ? (double)CompletedNodes / TotalNodes * 100 : 0;
}

/// <summary>Execution log entry.</summary>
public sealed record XFAiWfExecLog
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string NodeId { get; init; } = string.Empty;
    public string NodeLabel { get; init; } = string.Empty;
    public XFAiWfLogLevel Level { get; init; } = XFAiWfLogLevel.Info;
    public string Message { get; init; } = string.Empty;
    public int? DurationMs { get; init; }
    public string? Details { get; init; }
}

#endregion

#region Automation Rule Models

/// <summary>A single automation rule with conditions and actions.</summary>
public sealed record XFAiAutomationRuleItem
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool Enabled { get; init; } = true;
    public int Priority { get; init; }
    public IReadOnlyList<XFAiCondition> Conditions { get; init; } = Array.Empty<XFAiCondition>();
    public XFAiLogicalOperator LogicalOperator { get; init; } = XFAiLogicalOperator.And;
    public IReadOnlyList<XFAiAutomationAction> Actions { get; init; } = Array.Empty<XFAiAutomationAction>();
    public int ExecutionCount { get; init; }
    public DateTime? LastExecutedAt { get; init; }
    public string? AiEnhancementPrompt { get; init; }
}

/// <summary>A single condition in an automation rule.</summary>
public sealed record XFAiCondition
{
    public string Field { get; init; } = string.Empty;
    public XFAiConditionOperator Operator { get; init; } = XFAiConditionOperator.Equals;
    public string Value { get; init; } = string.Empty;
}

/// <summary>An action to execute when conditions are met.</summary>
public sealed record XFAiAutomationAction
{
    public XFAiAutomationActionType Type { get; init; }
    public string? Target { get; init; }
    public string? Payload { get; init; }
    public string? Description { get; init; }
}

#endregion

#region Batch Processor Models

/// <summary>A single item in a batch operation.</summary>
public sealed record XFAiBatchItem
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string Label { get; init; } = string.Empty;
    public XFAiBatchItemStatus Status { get; init; } = XFAiBatchItemStatus.Pending;
    public int Progress { get; init; }
    public string? Result { get; init; }
    public string? ErrorMessage { get; init; }
    public int RetryCount { get; init; }
    public int? DurationMs { get; init; }
    public IReadOnlyDictionary<string, string>? Data { get; init; }
}

/// <summary>Overall batch processing state.</summary>
public sealed record XFAiBatchState
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string OperationName { get; init; } = string.Empty;
    public XFAiWfExecutionStatus Status { get; init; } = XFAiWfExecutionStatus.Pending;
    public int TotalItems { get; init; }
    public int ProcessedItems { get; init; }
    public int SuccessfulItems { get; init; }
    public int FailedItems { get; init; }
    public int CancelledItems { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public double Progress => TotalItems > 0 ? (double)ProcessedItems / TotalItems * 100 : 0;
    public double SuccessRate => ProcessedItems > 0 ? (double)SuccessfulItems / ProcessedItems * 100 : 0;
}

#endregion

#region Scheduler Models

/// <summary>A scheduled job definition.</summary>
public sealed record XFAiScheduledJob
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public XFAiScheduleFrequency Frequency { get; init; } = XFAiScheduleFrequency.Daily;
    public string? CronExpression { get; init; }
    public DateTime? NextRunAt { get; init; }
    public DateTime? LastRunAt { get; init; }
    public XFAiScheduledJobStatus Status { get; init; } = XFAiScheduledJobStatus.Active;
    public string? WorkflowId { get; init; }
    public string? ActionTarget { get; init; }
    public int TotalRuns { get; init; }
    public int SuccessfulRuns { get; init; }
    public int FailedRuns { get; init; }
    public bool AiEnhanced { get; init; }
    public string? AiSuggestedSchedule { get; init; }
    public IReadOnlyDictionary<string, string>? Parameters { get; init; }
}

/// <summary>A log entry from a scheduled job execution.</summary>
public sealed record XFAiScheduleLog
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string JobId { get; init; } = string.Empty;
    public string JobName { get; init; } = string.Empty;
    public DateTime ExecutedAt { get; init; } = DateTime.UtcNow;
    public XFAiWfExecutionStatus Status { get; init; }
    public int? DurationMs { get; init; }
    public string? Message { get; init; }
    public string? Error { get; init; }
}

#endregion
