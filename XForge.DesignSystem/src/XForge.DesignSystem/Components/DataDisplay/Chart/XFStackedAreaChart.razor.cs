using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Stacked area chart that displays multiple series as filled areas stacked on top of each other.
/// </summary>
public partial class XFStackedAreaChart : XForgeComponentBase
{
    protected override string BaseClass => "xf-stacked-area-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The data points (X-axis labels).</summary>
    [Parameter] public IReadOnlyList<string> Data { get; set; } = Array.Empty<string>();

    /// <summary>The data series to stack.</summary>
    [Parameter] public IReadOnlyList<XFStackedSeries> Series { get; set; } = Array.Empty<XFStackedSeries>();

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

        // assinatura de render: gráfico SVG e caro de repintar — pula render redundante
        // quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', AriaLabel, Data.Count, Series.Count, MinY, YMax, ShowGrid, ShowLegend, GridLines, SvgWidth, SvgHeight);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

    private const double MarginLeft = 50;
    private const double MarginTop = 10;
    private double ChartWidth => SvgWidth - MarginLeft - 20;
    private double ChartHeight => SvgHeight - MarginTop - 30;

    private double EffectiveYMax
    {
        get
        {
            if (YMax > 0) return YMax;
            var max = 0.0;
            for (var i = 0; i < Data.Count; i++)
            {
                var total = 0.0;
                foreach (var series in Series)
                {
                    if (i < series.Values.Count)
                        total += series.Values[i];
                }
                if (total > max) max = total;
            }
            return max > 0 ? max * 1.1 : 100;
        }
    }

    private double YRange => EffectiveYMax - MinY;

    private double GetSeriesValue(int seriesIndex, int dataIndex)
    {
        if (seriesIndex >= Series.Count) return 0;
        var values = Series[seriesIndex].Values;
        return dataIndex < values.Count ? values[dataIndex] : 0;
    }

    private string GetLabel(int index) => index < Data.Count ? Data[index] : "";

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
