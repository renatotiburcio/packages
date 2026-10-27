namespace XForge.DesignSystem.Components.AI.Common;

/// <summary>
/// Represents a request to an AI provider for chat completion.
/// </summary>
public sealed record AiChatRequest
{
    /// <summary>List of conversation messages.</summary>
    public required IReadOnlyList<AiMessage> Messages { get; init; }

    /// <summary>Model identifier (e.g., "gpt-4o", "llama3", "claude-3-opus").</summary>
    public string? Model { get; init; }

    /// <summary>System prompt prepended to the conversation.</summary>
    public string? SystemPrompt { get; init; }

    /// <summary>Temperature (creativity) — 0.0 (deterministic) to 2.0 (very creative).</summary>
    public double? Temperature { get; init; }

    /// <summary>Maximum tokens in the completion.</summary>
    public int? MaxTokens { get; init; }

    /// <summary>Top-p nucleus sampling (0.0–1.0).</summary>
    public double? TopP { get; init; }

    /// <summary>Stop sequences — generation stops when any is encountered.</summary>
    public IReadOnlyList<string>? StopSequences { get; init; }

    /// <summary>Tool/function definitions available to the model.</summary>
    public IReadOnlyList<AiToolDefinition>? Tools { get; init; }

    /// <summary>User identifier for rate limiting and audit.</summary>
    public string? UserId { get; init; }

    /// <summary>Session identifier for memory/context continuity.</summary>
    public string? SessionId { get; init; }
}

/// <summary>
/// Defines a tool/function that the AI model can invoke.
/// </summary>
public sealed record AiToolDefinition
{
    /// <summary>Function name.</summary>
    public required string Name { get; init; }

    /// <summary>Description of what the function does.</summary>
    public required string Description { get; init; }

    /// <summary>JSON Schema for the function parameters.</summary>
    public string? ParametersSchema { get; init; }
}
