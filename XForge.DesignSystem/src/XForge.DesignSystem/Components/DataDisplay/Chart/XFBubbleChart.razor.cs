using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Scatter chart with variable-size bubbles, rendered via SVG.
/// Each data point is plotted by X/Y position with a radius driven by a size selector.
/// </summary>
public partial class XFBubbleChart<T> : XForgeComponentBase
{
    protected override string BaseClass => "xf-bubble-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The data points to plot.</summary>
    [Parameter] public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();

    /// <summary>Selector that extracts the X value from a data point.</summary>
    [Parameter] public Func<T, double> XSelector { get; set; } = _ => 0;

    /// <summary>Selector that extracts the Y value from a data point.</summary>
    [Parameter] public Func<T, double> YSelector { get; set; } = _ => 0;

    /// <summary>Selector that extracts the size value from a data point.</summary>
    [Parameter] public Func<T, double> SizeSelector { get; set; } = _ => 1;

    /// <summary>Selector that extracts a label string from a data point.</summary>
    [Parameter] public Func<T, string> LabelSelector { get; set; } = _ => string.Empty;

    /// <summary>Maximum bubble radius in SVG units.</summary>
    [Parameter] public double MaxBubbleSize { get; set; } = 30;

    /// <summary>CSS class for the bubble fill color.</summary>
    [Parameter] public string? FillColor { get; set; }

    /// <summary>CSS class for the bubble stroke color.</summary>
    [Parameter] public string? StrokeColor { get; set; }

    /// <summary>Whether to show text labels above each bubble.</summary>
    [Parameter] public bool ShowLabels { get; set; }

    /// <summary>SVG viewport width.</summary>
    [Parameter] public double SvgWidth { get; set; } = 400;

    /// <summary>SVG viewport height.</summary>
    [Parameter] public double SvgHeight { get; set; } = 300;

        // assinatura de render: gráfico SVG e caro de repintar — pula render redundante
        // quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', AriaLabel, Data.Count, MaxBubbleSize, FillColor, StrokeColor, ShowLabels, SvgWidth, SvgHeight);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
}
