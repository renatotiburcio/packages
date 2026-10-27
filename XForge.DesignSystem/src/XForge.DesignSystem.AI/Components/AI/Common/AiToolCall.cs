namespace XForge.DesignSystem.Components.AI.Common;

/// <summary>
/// Represents a tool/function call requested by the AI model.
/// </summary>
public sealed record AiToolCall
{
    /// <summary>Unique identifier for this tool call.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Name of the function/tool to invoke.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>JSON-encoded arguments for the function call.</summary>
    public string Arguments { get; init; } = string.Empty;
}
