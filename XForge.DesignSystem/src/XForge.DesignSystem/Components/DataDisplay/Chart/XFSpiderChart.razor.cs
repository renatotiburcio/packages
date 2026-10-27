using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Spider (radar) chart that displays multivariate data as a filled polygon on radial axes.
/// </summary>
public partial class XFSpiderChart : XForgeComponentBase
{
    protected override string BaseClass => "xf-spider-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The axis definitions for the spider chart.</summary>
    [Parameter] public IReadOnlyList<XFSeriesAxis> Axes { get; set; } = Array.Empty<XFSeriesAxis>();

    /// <summary>The data series to render.</summary>
    [Parameter] public IReadOnlyList<XFSeriesData> Series { get; set; } = Array.Empty<XFSeriesData>();

    /// <summary>Maximum value for all axes (used when axis.Max is not set).</summary>
    [Parameter] public double Max { get; set; } = 100;

    /// <summary>Number of concentric grid levels.</summary>
    [Parameter] public int Levels { get; set; } = 5;

    /// <summary>Chart radius in SVG units.</summary>
    [Parameter] public double Radius { get; set; } = 100;

    /// <summary>Whether to show the legend.</summary>
    [Parameter] public bool ShowLegend { get; set; } = true;

    private double GetAngle(int index)
    {
        if (Axes.Count == 0) return 0;
        var step = 360.0 / Axes.Count;
        return -90 + step * index; // Start from top (-90°)
    }

    private static string RenderSvgText(double x, double y, string text, string cssClass)
    {
        return $"""<text x="{x:0.###}" y="{y:0.###}" text-anchor="middle" dominant-baseline="middle" class="{cssClass}">{text}</text>""";
    }
}

/// <summary>
/// Defines a single axis in the spider chart.
/// </summary>
public class XFSeriesAxis
{
    /// <summary>Axis label displayed around the chart.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Maximum value for this axis. If 0, uses the chart-level Max.</summary>
    public double Max { get; set; }
}

/// <summary>
/// Represents a data series in the spider chart.
/// </summary>
public class XFSeriesData
{
    /// <summary>Series name for the legend.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Values for each axis (must match Axes count).</summary>
    public IReadOnlyList<double> Values { get; set; } = Array.Empty<double>();

    /// <summary>CSS class for the polygon fill color.</summary>
    public string? FillColor { get; set; }

    /// <summary>CSS class for the polygon stroke color.</summary>
    public string? StrokeColor { get; set; }

    /// <summary>Stroke width for the polygon outline.</summary>
    public double StrokeWidth { get; set; }

    /// <summary>CSS class for data point circle fill.</summary>
    public string? PointColor { get; set; }
}
