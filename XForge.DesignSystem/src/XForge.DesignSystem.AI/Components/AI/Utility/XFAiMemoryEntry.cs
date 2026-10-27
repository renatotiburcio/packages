namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Represents a single AI memory entry for visualization.
/// </summary>
public sealed record XFAiMemoryEntry
{
    /// <summary>Unique identifier for the memory entry.</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>The memory content or summary.</summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>The type of memory.</summary>
    public XFAiMemoryType Type { get; init; }

    /// <summary>Relevance score between 0.0 and 1.0.</summary>
    public double Relevance { get; init; }

    /// <summary>When this memory was created.</summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>When this memory was last accessed.</summary>
    public DateTime? LastAccessedAt { get; init; }

    /// <summary>How many times this memory has been accessed.</summary>
    public int AccessCount { get; init; }

    /// <summary>Optional context or source of this memory.</summary>
    public string? Context { get; init; }

    /// <summary>Optional tags for categorization.</summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Optional embedding vector dimension count (for display purposes).</summary>
    public int? EmbeddingDimension { get; init; }
}

/// <summary>
/// Types of AI memory.
/// </summary>
public enum XFAiMemoryType
{
    /// <summary>Short-term conversation context.</summary>
    Session,

    /// <summary>Long-term persistent knowledge.</summary>
    LongTerm,

    /// <summary>Semantic/vector-based memory.</summary>
    Semantic,

    /// <summary>User preferences and settings.</summary>
    Preferences,

    /// <summary>Factual knowledge base entries.</summary>
    Factual
}
