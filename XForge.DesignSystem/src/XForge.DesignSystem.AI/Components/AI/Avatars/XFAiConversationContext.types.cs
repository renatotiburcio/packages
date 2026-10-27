namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// Type of context entry in the conversation.
/// </summary>
public enum XFAiContextEntryType
{
    /// <summary>User preference learned from interaction.</summary>
    Preference,
    /// <summary>Fact or knowledge extracted from conversation.</summary>
    Fact,
    /// <summary>Action performed during conversation.</summary>
    Action,
    /// <summary>Decision made during conversation.</summary>
    Decision,
    /// <summary>User feedback on AI response.</summary>
    Feedback,
    /// <summary>System-generated context (session metadata).</summary>
    System
}

/// <summary>
/// A single context entry in the conversation memory.
/// </summary>
public sealed record XFAiContextEntry
{
    /// <summary>Unique identifier.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>Type of context entry.</summary>
    public required XFAiContextEntryType Type { get; init; }

    /// <summary>Content/text of the context.</summary>
    public required string Content { get; init; }

    /// <summary>Source of the context (user, ai, system).</summary>
    public string Source { get; init; } = "system";

    /// <summary>Relevance score (0.0–1.0).</summary>
    public double Relevance { get; init; } = 1.0;

    /// <summary>Tags for categorization.</summary>
    public IReadOnlyList<string>? Tags { get; init; }

    /// <summary>When this context was captured.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Whether this context is pinned (always included in prompt).</summary>
    public bool IsPinned { get; init; }
}

/// <summary>
/// User preference stored in conversation context.
/// </summary>
public sealed record XFAiUserPreference
{
    /// <summary>Preference key (e.g., "language", "currency", "theme").</summary>
    public required string Key { get; init; }

    /// <summary>Preference value.</summary>
    public required string Value { get; init; }

    /// <summary>Confidence in this preference (0.0–1.0).</summary>
    public double Confidence { get; init; } = 1.0;

    /// <summary>When this preference was learned.</summary>
    public DateTimeOffset LearnedAt { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Summary of the conversation context state.
/// </summary>
public sealed record XFAiConversationSummary
{
    /// <summary>Total messages in conversation.</summary>
    public int MessageCount { get; init; }

    /// <summary>Total context entries.</summary>
    public int ContextEntryCount { get; init; }

    /// <summary>Pinned context entries.</summary>
    public int PinnedCount { get; init; }

    /// <summary>Learned user preferences.</summary>
    public IReadOnlyList<XFAiUserPreference> Preferences { get; init; } = Array.Empty<XFAiUserPreference>();

    /// <summary>Session start time.</summary>
    public DateTimeOffset SessionStarted { get; init; }

    /// <summary>Last activity time.</summary>
    public DateTimeOffset LastActivity { get; init; }

    /// <summary>Session duration.</summary>
    public TimeSpan Duration => LastActivity - SessionStarted;
}
