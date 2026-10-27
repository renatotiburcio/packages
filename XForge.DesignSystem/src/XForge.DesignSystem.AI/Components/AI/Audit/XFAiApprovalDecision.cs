namespace XForge.DesignSystem.Components.AI.Audit;

/// <summary>
/// Decision made by a human reviewer on an approval card.
/// </summary>
public enum XFAiApprovalDecision
{
    /// <summary>No decision made yet.</summary>
    Pending,

    /// <summary>Action approved by the reviewer.</summary>
    Approved,

    /// <summary>Action rejected by the reviewer.</summary>
    Rejected,

    /// <summary>Action approved with modifications.</summary>
    Edited
}

/// <summary>
/// Represents the payload for a human approval request.
/// </summary>
public sealed record XFAiApprovalRequest
{
    /// <summary>Unique identifier for the approval request.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Title of the action requiring approval.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Detailed description of the action.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Who requested the action (AI or user).</summary>
    public string RequestedBy { get; init; } = string.Empty;

    /// <summary>When the request was created.</summary>
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Risk score from 0.0 (safe) to 1.0 (critical).</summary>
    public double RiskScore { get; init; }

    /// <summary>AI-generated impact analysis.</summary>
    public string? ImpactAnalysis { get; init; }

    /// <summary>Optional category (e.g., "Pagamento", "Exclusao", "Fiscal").</summary>
    public string? Category { get; init; }

    /// <summary>Proposed data/action payload to be applied if approved.</summary>
    public string? ProposedPayload { get; init; }

    /// <summary>Optional list of affected entities or records.</summary>
    public IReadOnlyList<string> AffectedEntities { get; init; } = Array.Empty<string>();
}
