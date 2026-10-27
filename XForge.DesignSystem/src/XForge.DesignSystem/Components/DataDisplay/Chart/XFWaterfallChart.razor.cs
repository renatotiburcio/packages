using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Waterfall chart showing floating bars that accumulate to a total.
/// Each bar starts where the previous one ended; positive values go up, negative go down.
/// </summary>
public partial class XFWaterfallChart<T> : XForgeComponentBase
{
    protected override string BaseClass => "xf-waterfall-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The data points representing each step in the waterfall.</summary>
    [Parameter] public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();

    /// <summary>Selector that extracts the label string from a data point.</summary>
    [Parameter] public Func<T, string> LabelSelector { get; set; } = _ => string.Empty;

    /// <summary>Selector that extracts the numeric value from a data point.</summary>
    [Parameter] public Func<T, double> ValueSelector { get; set; } = _ => 0;

    /// <summary>Selector that returns true if the data point represents a positive (upward) change.</summary>
    [Parameter] public Func<T, bool> IsPositiveSelector { get; set; } = _ => true;

    /// <summary>CSS color for positive (upward) bars.</summary>
    [Parameter] public string PositiveColor { get; set; } = "var(--color-success, #22c55e)";

    /// <summary>CSS color for negative (downward) bars.</summary>
    [Parameter] public string NegativeColor { get; set; } = "var(--color-error, #ef4444)";

    /// <summary>Whether to display value labels above/below each bar.</summary>
    [Parameter] public bool ShowValues { get; set; } = true;

    /// <summary>Whether to display a cumulative total bar at the end.</summary>
    [Parameter] public bool ShowTotal { get; set; } = true;

    /// <summary>SVG viewport width.</summary>
    [Parameter] public double SvgWidth { get; set; } = 500;

    /// <summary>SVG viewport height.</summary>
    [Parameter] public double SvgHeight { get; set; } = 250;

    private sealed class WaterfallEntry
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public bool IsPositive { get; set; }
        public double Start { get; set; }
    }
}
