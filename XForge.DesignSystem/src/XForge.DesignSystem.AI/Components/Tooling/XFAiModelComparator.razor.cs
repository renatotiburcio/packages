using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.AI.Components;

/// <summary>
/// Side-by-side comparison of AI model runs with latency, token usage,
/// cost, and quality scoring. Highlights the winning run based on the
/// selected metric.
/// </summary>
public partial class XFAiModelComparator : XForgeComponentBase
{
    /// <summary>Model run results to compare.</summary>
    [Parameter] public IReadOnlyList<ModelRun> Runs { get; set; } = new List<ModelRun>();

    /// <summary>Metric used to determine the winner (score, latency, cost, tokens).</summary>
    [Parameter] public string Metric { get; set; } = "score";

    /// <summary>Fires when a specific run is selected for detail view.</summary>
    [Parameter] public EventCallback<ModelRun> OnRunSelected { get; set; }
}
