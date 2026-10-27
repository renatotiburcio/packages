namespace XForge.DesignSystem.Components.AI.Common;

/// <summary>
/// Represents a complete response from an AI provider.
/// </summary>
public sealed record AiChatResponse
{
    /// <summary>Response message from the assistant.</summary>
    public AiMessage Message { get; init; } = new() { Role = AiRole.Assistant };

    /// <summary>Finish reason (e.g., "stop", "length", "tool_calls").</summary>
    public string? FinishReason { get; init; }

    /// <summary>Token usage for this interaction.</summary>
    public AiTokenUsage? Usage { get; init; }

    /// <summary>Provider-specific response ID.</summary>
    public string? ResponseId { get; init; }

    /// <summary>Time taken to generate the response.</summary>
    public TimeSpan? Duration { get; init; }
}

/// <summary>
/// Represents a streaming chunk from an AI provider.
/// </summary>
public sealed record AiStreamChunk
{
    /// <summary>Incremental text content for this chunk.</summary>
    public string? Delta { get; init; }

    /// <summary>Tool call delta (incremental tool call arguments).</summary>
    public AiToolCallDelta? ToolCallDelta { get; init; }

    /// <summary>Reasoning delta (incremental reasoning content).</summary>
    public string? ReasoningDelta { get; init; }

    /// <summary>Finish reason when stream completes (null while streaming).</summary>
    public string? FinishReason { get; init; }

    /// <summary>Chunk index in the stream.</summary>
    public int ChunkIndex { get; init; }
}

/// <summary>
/// Incremental tool call information in a streaming chunk.
/// </summary>
public sealed record AiToolCallDelta
{
    /// <summary>Tool call ID (present on first chunk of a tool call).</summary>
    public string? Id { get; init; }

    /// <summary>Tool/function name (present on first chunk of a tool call).</summary>
    public string? Name { get; init; }

    /// <summary>Incremental arguments JSON.</summary>
    public string? ArgumentsDelta { get; init; }

    /// <summary>Index of this tool call in the array of parallel tool calls.</summary>
    public int Index { get; init; }
}
