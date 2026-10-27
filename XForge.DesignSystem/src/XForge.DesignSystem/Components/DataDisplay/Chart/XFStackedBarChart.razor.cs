using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Stacked bar chart that displays multiple series stacked on top of each other.
/// Supports normal stacking and 100% (full) stacking modes.
/// </summary>
public partial class XFStackedBarChart : XForgeComponentBase
{
    protected override string BaseClass => "xf-stacked-bar-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The category labels for the X axis.</summary>
    [Parameter] public IReadOnlyList<string> Labels { get; set; } = Array.Empty<string>();

    /// <summary>The data series to stack.</summary>
    [Parameter] public IReadOnlyList<XFStackedSeries> Series { get; set; } = Array.Empty<XFStackedSeries>();

    /// <summary>Stack mode: None, Stacked, or FullStacked (100%).</summary>
    [Parameter] public XFChartStackMode StackMode { get; set; } = XFChartStackMode.Stacked;

    /// <summary>Minimum Y-axis value.</summary>
    [Parameter] public double MinY { get; set; }

    /// <summary>Maximum Y-axis value. Auto-detected if 0.</summary>
    [Parameter] public double YMax { get; set; }

    /// <summary>Whether to show grid lines.</summary>
    [Parameter] public bool ShowGrid { get; set; } = true;

    /// <summary>Whether to show the legend.</summary>
    [Parameter] public bool ShowLegend { get; set; } = true;

    /// <summary>Number of grid lines.</summary>
    [Parameter] public int GridLines { get; set; } = 5;

    /// <summary>SVG viewport width.</summary>
    [Parameter] public double SvgWidth { get; set; } = 500;

    /// <summary>SVG viewport height.</summary>
    [Parameter] public double SvgHeight { get; set; } = 250;

    private const double MarginLeft = 50;
    private const double MarginTop = 10;
    private double ChartWidth => SvgWidth - MarginLeft - 20;
    private double ChartHeight => SvgHeight - MarginTop - 30;

    private double EffectiveYMax
    {
        get
        {
            if (YMax > 0) return YMax;
            if (StackMode == XFChartStackMode.FullStacked) return 100;
            var max = 0.0;
            for (var i = 0; i < Labels.Count; i++)
            {
                var total = GetStackTotal(i);
                if (total > max) max = total;
            }
            return max > 0 ? max * 1.1 : 100;
        }
    }

    private double YRange => EffectiveYMax - MinY;

    private double GetStackTotal(int index)
    {
        var total = 0.0;
        foreach (var series in Series)
        {
            if (index < series.Values.Count)
                total += series.Values[index];
        }
        return total;
    }

    private double GetSeriesValue(int seriesIndex, int dataIndex)
    {
        if (seriesIndex >= Series.Count) return 0;
        var values = Series[seriesIndex].Values;
        return dataIndex < values.Count ? values[dataIndex] : 0;
    }

    private string GetLabel(int index) => index < Labels.Count ? Labels[index] : "";

    private static readonly string[] DefaultColors = ["#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6", "#ec4899", "#06b6d4", "#84cc16"];

    private string GetSeriesColor(int index)
    {
        if (index < Series.Count && !string.IsNullOrEmpty(Series[index].Color))
            return Series[index].Color;
        return DefaultColors[index % DefaultColors.Length];
    }

    private static string FormatValue(double value) =>
        value >= 1000 ? $"{value / 1000:0.#}k" : $"{value:0.##}";
}

/// <summary>
/// Represents a data series for the stacked bar chart.
/// </summary>
public class XFStackedSeries
{
    /// <summary>Series name for the legend.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Values for each category (must match Labels count).</summary>
    public IReadOnlyList<double> Values { get; set; } = Array.Empty<double>();

    /// <summary>Custom color (CSS color or hex). If null, uses default palette.</summary>
    public string? Color { get; set; }
}
