using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.AI.Components;

/// <summary>
/// Estimates API cost for AI model calls based on input/output token counts
/// and the selected model's pricing. Displays breakdown and optional warnings.
/// </summary>
public partial class XFAiCostEstimator : XForgeComponentBase
{
    /// <summary>AI model identifier for pricing lookup.</summary>
    [Parameter] public string Model { get; set; } = "gpt-4o";

    /// <summary>Number of input (prompt) tokens.</summary>
    [Parameter] public int InputTokens { get; set; } = 1000;

    /// <summary>Number of output (completion) tokens.</summary>
    [Parameter] public int OutputTokens { get; set; } = 500;

    /// <summary>Total estimated tokens (input + output).</summary>
    public int EstimatedTokens => InputTokens + OutputTokens;

    /// <summary>Estimated input cost in USD.</summary>
    public decimal InputCost => InputTokens * 0.000005m;

    /// <summary>Estimated output cost in USD.</summary>
    public decimal OutputCost => OutputTokens * 0.000015m;

    /// <summary>Total estimated cost in USD.</summary>
    public decimal EstimatedCost => InputCost + OutputCost;

    /// <summary>Optional warning message (e.g., quota limits, cost thresholds).</summary>
    [Parameter] public string? Warning { get; set; }

    /// <summary>Fires when the cost estimate changes.</summary>
    [Parameter] public EventCallback<decimal> OnEstimateChanged { get; set; }
}
