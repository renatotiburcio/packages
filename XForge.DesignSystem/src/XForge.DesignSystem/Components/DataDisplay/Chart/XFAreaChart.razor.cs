using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Line chart with a filled area below the line, rendered via SVG.
/// Supports customizable colors, grid lines, dot markers, and Y-axis bounds.
/// </summary>
public partial class XFAreaChart<T> : XForgeComponentBase
{
    protected override string BaseClass => "xf-area-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The data points to plot.</summary>
    [Parameter] public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();

    /// <summary>Selector that extracts the X value from a data point.</summary>
    [Parameter] public Func<T, double> XSelector { get; set; } = _ => 0;

    /// <summary>Selector that extracts the Y value from a data point.</summary>
    [Parameter] public Func<T, double> YSelector { get; set; } = _ => 0;

    /// <summary>CSS class for the area fill color.</summary>
    [Parameter] public string? FillColor { get; set; }

    /// <summary>CSS class for the line stroke color.</summary>
    [Parameter] public string? StrokeColor { get; set; }

    /// <summary>Stroke width for the line (default 2).</summary>
    [Parameter] public double StrokeWidth { get; set; }

    /// <summary>Minimum Y-axis value. Auto-detected from data if null.</summary>
    [Parameter] public double? MinY { get; set; }

    /// <summary>Maximum Y-axis value. Auto-detected from data if null.</summary>
    [Parameter] public double? MaxY { get; set; }

    /// <summary>Stack mode for multiple series.</summary>
    [Parameter] public XFChartStackMode StackMode { get; set; }

    /// <summary>Whether to render horizontal grid lines and Y-axis labels.</summary>
    [Parameter] public bool ShowGrid { get; set; } = true;

    /// <summary>Whether to render circular dots at each data point.</summary>
    [Parameter] public bool ShowDots { get; set; } = true;

    /// <summary>Number of horizontal grid lines.</summary>
    [Parameter] public int GridLines { get; set; } = 5;

    /// <summary>SVG viewport width.</summary>
    [Parameter] public double SvgWidth { get; set; } = 400;

    /// <summary>SVG viewport height.</summary>
    [Parameter] public double SvgHeight { get; set; } = 200;

    private static string FormatValue(double value) =>
        value >= 1000 ? $"{value / 1000:0.#}k" : $"{value:0.##}";
}
