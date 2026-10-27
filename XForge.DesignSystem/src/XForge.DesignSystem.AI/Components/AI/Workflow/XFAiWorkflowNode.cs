namespace XForge.DesignSystem.Components.AI.Orchestration;

/// <summary>
/// Represents a node in a workflow designer.
/// </summary>
public sealed record XFAiWorkflowNode
{
    /// <summary>Unique node identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Display label for the node.</summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>Node type determining its behavior and appearance.</summary>
    public XFAiWorkflowNodeType Type { get; init; } = XFAiWorkflowNodeType.Ai;

    /// <summary>X position on the canvas (pixels).</summary>
    public double X { get; init; }

    /// <summary>Y position on the canvas (pixels).</summary>
    public double Y { get; init; }

    /// <summary>Optional description or configuration summary.</summary>
    public string? Description { get; init; }

    /// <summary>Whether this node is currently active/running.</summary>
    public bool IsActive { get; init; }

    /// <summary>Whether this node has completed execution.</summary>
    public bool IsCompleted { get; init; }

    /// <summary>Whether this node is disabled.</summary>
    public bool IsDisabled { get; init; }

    /// <summary>Configuration parameters for the node.</summary>
    public IReadOnlyDictionary<string, string>? Config { get; init; }

    /// <summary>Icon identifier override.</summary>
    public string? Icon { get; init; }
}

/// <summary>
/// Represents a connection (edge) between two workflow nodes.
/// </summary>
public sealed record XFAiWorkflowConnection
{
    /// <summary>Unique connection identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Source node ID.</summary>
    public string SourceId { get; init; } = string.Empty;

    /// <summary>Target node ID.</summary>
    public string TargetId { get; init; } = string.Empty;

    /// <summary>Optional label for the connection.</summary>
    public string? Label { get; init; }

    /// <summary>Whether this connection is a conditional branch.</summary>
    public bool IsConditional { get; init; }

    /// <summary>Whether this connection is currently active.</summary>
    public bool IsActive { get; init; }
}

/// <summary>
/// Types of workflow nodes.
/// </summary>
public enum XFAiWorkflowNodeType
{
    /// <summary>Start trigger node.</summary>
    Start,

    /// <summary>End/completion node.</summary>
    End,

    /// <summary>AI processing node.</summary>
    Ai,

    /// <summary>Business rule evaluation node.</summary>
    Rule,

    /// <summary>Human approval node.</summary>
    Approval,

    /// <summary>External trigger node.</summary>
    Trigger,

    /// <summary>Event listener node.</summary>
    Event,

    /// <summary>Delay/scheduling node.</summary>
    Schedule
}
