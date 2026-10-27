namespace XForge.DesignSystem.Services.AI;

/// <summary>
/// Persistent memory service for AI interactions.
/// Supports semantic memory (knowledge facts), long-term memory (cross-session),
/// session memory (within a conversation), and contextual memory (active context).
/// </summary>
public interface IAiMemoryService
{
    /// <summary>
    /// Stores a memory entry with the specified category and scope.
    /// </summary>
    Task StoreAsync(AiMemoryEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves memories matching the query, ranked by relevance.
    /// </summary>
    Task<IReadOnlyList<AiMemoryEntry>> RecallAsync(string query, AiMemoryScope scope, int topK = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all memories for a specific session.
    /// </summary>
    Task<IReadOnlyList<AiMemoryEntry>> GetSessionMemoriesAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific memory by ID.
    /// </summary>
    Task<bool> ForgetAsync(string memoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all memories within a specific scope (e.g., session cleanup).
    /// </summary>
    Task<int> ClearAsync(AiMemoryScope scope, string? sessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns total memory count by scope.
    /// </summary>
    Task<IReadOnlyDictionary<AiMemoryScope, int>> GetStatsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// A single memory entry stored by the AI memory service.
/// </summary>
public sealed record AiMemoryEntry
{
    /// <summary>Unique identifier (auto-generated if not provided).</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>The content/text of this memory.</summary>
    public required string Content { get; init; }

    /// <summary>Memory scope category.</summary>
    public required AiMemoryScope Scope { get; init; }

    /// <summary>Session identifier (required for Session scope).</summary>
    public string? SessionId { get; init; }

    /// <summary>User identifier for user-scoped memories.</summary>
    public string? UserId { get; init; }

    /// <summary>Tags for categorization and filtering.</summary>
    public IReadOnlyList<string>? Tags { get; init; }

    /// <summary>Relevance score (0.0–1.0) assigned during recall.</summary>
    public double Score { get; init; }

    /// <summary>When this memory was created (UTC).</summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>When this memory was last accessed (UTC).</summary>
    public DateTimeOffset? LastAccessedAt { get; init; }

    /// <summary>Access count for recency/frequency scoring.</summary>
    public int AccessCount { get; init; }
}

/// <summary>
/// Categories of AI memory with different persistence and retrieval semantics.
/// </summary>
public enum AiMemoryScope
{
    /// <summary>Knowledge facts extracted from documents (RAG). Long-lived.</summary>
    Semantic,

    /// <summary>Cross-session persistent memory (user preferences, learned patterns).</summary>
    LongTerm,

    /// <summary>Within a single conversation session. Cleared on session end.</summary>
    Session,

    /// <summary>Active context window (current task, recent interactions). Ephemeral.</summary>
    Contextual
}
