using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Box-and-whisker (box plot) chart displaying statistical distributions.
/// Each data point shows min, Q1, median, Q3, max, and optional outliers / mean.
/// </summary>
public partial class XFBoxPlotChart : XForgeComponentBase
{
    protected override string BaseClass => "xf-boxplot-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The statistical data points to render as box plots.</summary>
    [Parameter] public IReadOnlyList<BoxPlotData> Data { get; set; } = Array.Empty<BoxPlotData>();

    /// <summary>Whether to render outlier points beyond the whiskers.</summary>
    [Parameter] public bool ShowOutliers { get; set; } = true;

    /// <summary>Whether to render a mean marker (dashed line) inside each box.</summary>
    [Parameter] public bool ShowMean { get; set; }

    /// <summary>SVG viewport width.</summary>
    [Parameter] public double SvgWidth { get; set; } = 500;

    /// <summary>SVG viewport height.</summary>
    [Parameter] public double SvgHeight { get; set; } = 280;

        // assinatura de render: gráfico SVG e caro de repintar — pula render redundante
        // quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', AriaLabel, Data.Count, ShowOutliers, ShowMean, SvgWidth, SvgHeight);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
}

/// <summary>
/// Statistical summary for a single box plot entry.
/// </summary>
public class BoxPlotData
{
    /// <summary>Label displayed below the box plot.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Minimum value (lower whisker end).</summary>
    public double Min { get; set; }

    /// <summary>First quartile (25th percentile, bottom of box).</summary>
    public double Q1 { get; set; }

    /// <summary>Median (50th percentile, line inside box).</summary>
    public double Median { get; set; }

    /// <summary>Third quartile (75th percentile, top of box).</summary>
    public double Q3 { get; set; }

    /// <summary>Maximum value (upper whisker end).</summary>
    public double Max { get; set; }

    /// <summary>Mean value, shown as a dashed marker if <see cref="XFBoxPlotChart.ShowMean"/> is true.</summary>
    public double Mean { get; set; }

    /// <summary>Outlier values plotted as individual circles beyond the whiskers.</summary>
    public IReadOnlyList<double>? Outliers { get; set; }
}
