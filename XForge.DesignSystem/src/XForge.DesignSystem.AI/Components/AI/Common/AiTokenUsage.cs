namespace XForge.DesignSystem.Components.AI.Common;

/// <summary>
/// Token usage and cost metrics for an AI interaction.
/// </summary>
public sealed record AiTokenUsage
{
    /// <summary>Number of tokens in the prompt/input.</summary>
    public int PromptTokens { get; init; }

    /// <summary>Number of tokens in the completion/output.</summary>
    public int CompletionTokens { get; init; }

    /// <summary>Total tokens consumed (prompt + completion).</summary>
    public int TotalTokens => PromptTokens + CompletionTokens;

    /// <summary>Estimated cost in USD for this interaction.</summary>
    public decimal? EstimatedCostUsd { get; init; }

    /// <summary>Model identifier used for this interaction.</summary>
    public string? Model { get; init; }
}
