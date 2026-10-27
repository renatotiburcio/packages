namespace XForge.DesignSystem.Components.AI.Common;

/// <summary>
/// Represents a single message in an AI conversation.
/// </summary>
public sealed record AiMessage
{
    /// <summary>Role of the participant who produced this message.</summary>
    public AiRole Role { get; init; }

    /// <summary>Text content of the message.</summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>Optional name/identifier for the participant (agent name, user ID).</summary>
    public string? Name { get; init; }

    /// <summary>Tool calls requested by the assistant in this message.</summary>
    public IReadOnlyList<AiToolCall>? ToolCalls { get; init; }

    /// <summary>Tool call ID this message responds to (for Tool role messages).</summary>
    public string? ToolCallId { get; init; }

    /// <summary>Citations/sources referenced in this message (RAG grounding).</summary>
    public IReadOnlyList<AiCitation>? Citations { get; init; }

    /// <summary>Reasoning/thinking content (chain-of-thought visualization).</summary>
    public string? Reasoning { get; init; }

    /// <summary>Timestamp when this message was created (UTC).</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Optional metadata dictionary for extensions.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
