using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Audit;

/// <summary>
/// Represents a single audit trail entry with before/after state, user/AI tracking, and immutability.
/// </summary>
[Obsolete("Use AiAuditEntry from XForge.DesignSystem.Services.AI instead. This record will be removed in v2.0.")]
public sealed record XFAiAuditEntry
{
    /// <summary>Unique identifier for the audit entry.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Timestamp of the action (UTC).</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>Display label for the action performed.</summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>Who performed the action (user email, "AI", "System").</summary>
    public string Actor { get; init; } = string.Empty;

    /// <summary>Whether the actor is an AI agent.</summary>
    public bool IsAiAction { get; init; }

    /// <summary>Optional AI prompt that triggered this action.</summary>
    public string? Prompt { get; init; }

    /// <summary>Optional AI reasoning or explanation.</summary>
    public string? Reasoning { get; init; }

    /// <summary>State before the change (JSON or text).</summary>
    public string? BeforeState { get; init; }

    /// <summary>State after the change (JSON or text).</summary>
    public string? AfterState { get; init; }

    /// <summary>SHA-256 hash for immutability verification.</summary>
    public string? Hash { get; init; }

    /// <summary>Digital signature for tamper detection.</summary>
    public string? Signature { get; init; }

    /// <summary>Version number for version history tracking.</summary>
    public int Version { get; init; }

    /// <summary>Optional category or module (e.g., "Fiscal", "Financeiro").</summary>
    public string? Category { get; init; }

    /// <summary>Arbitrary metadata.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Visual style for audit timeline entries.
/// </summary>
public enum XFAiAuditEntryType
{
    /// <summary>Standard action — neutral color.</summary>
    Default,

    /// <summary>User action — blue accent.</summary>
    User,

    /// <summary>AI action — purple accent.</summary>
    Ai,

    /// <summary>System action — slate accent.</summary>
    System,

    /// <summary>Warning action — amber accent.</summary>
    Warning,

    /// <summary>Error action — red accent.</summary>
    Error
}
