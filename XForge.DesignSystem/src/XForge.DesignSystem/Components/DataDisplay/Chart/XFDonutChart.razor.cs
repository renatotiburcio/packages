using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Donut (ring) chart that renders data as arc segments with a hollow center.
/// Supports customizable colors, labels, and legend positioning.
/// </summary>
public partial class XFDonutChart<T> : XForgeComponentBase
{
    protected override string BaseClass => "xf-donut-chart";

    private static readonly string[] DefaultColors =
    [
        "var(--color-primary, #3b82f6)",
        "var(--color-secondary, #8b5cf6)",
        "var(--color-success, #22c55e)",
        "var(--color-warning, #f59e0b)",
        "var(--color-error, #ef4444)",
        "#06b6d4", "#ec4899", "#14b8a6", "#f97316", "#6366f1"
    ];

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The data points representing each slice.</summary>
    [Parameter] public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();

    /// <summary>Selector that extracts the label string from a data point.</summary>
    [Parameter] public Func<T, string> LabelSelector { get; set; } = _ => string.Empty;

    /// <summary>Selector that extracts the numeric value from a data point.</summary>
    [Parameter] public Func<T, double> ValueSelector { get; set; } = _ => 0;

    /// <summary>Inner radius of the donut ring (0–80 for default viewBox).</summary>
    [Parameter] public double InnerRadius { get; set; } = 60;

    /// <summary>Array of CSS color values for each slice. Cycles if fewer than data count.</summary>
    [Parameter] public string[]? Colors { get; set; }

    /// <summary>Whether to show percentage labels on each slice.</summary>
    [Parameter] public bool ShowLabels { get; set; } = true;

    /// <summary>Whether to show the legend beside or below the chart.</summary>
    [Parameter] public bool ShowLegend { get; set; } = true;

    /// <summary>Legend layout position: Right places legend to the right, Bottom below.</summary>
    [Parameter] public LegendPosition LegendPosition { get; set; } = LegendPosition.Right;

    /// <summary>Overall SVG size in pixels.</summary>
    [Parameter] public double Size { get; set; } = 200;

        // assinatura de render: gráfico SVG e caro de repintar — pula render redundante
        // quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', AriaLabel, Data.Count, InnerRadius, Colors is null ? 0 : Colors.Length, ShowLabels, ShowLegend, LegendPosition, Size);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

    private string GetLayoutClass() => LegendPosition == LegendPosition.Right
        ? "items-center"
        : "flex-col items-center";

    private string GetLegendFlexDirection() => LegendPosition == LegendPosition.Right
        ? "flex-col"
        : "flex-row flex-wrap justify-center";

    private static string DescribeArc(double cx, double cy, double outerR, double innerR,
        double startAngle, double endAngle)
    {
        var startRad = startAngle * Math.PI / 180;
        var endRad = endAngle * Math.PI / 180;

        var x1 = cx + outerR * Math.Cos(startRad);
        var y1 = cy + outerR * Math.Sin(startRad);
        var x2 = cx + outerR * Math.Cos(endRad);
        var y2 = cy + outerR * Math.Sin(endRad);
        var x3 = cx + innerR * Math.Cos(endRad);
        var y3 = cy + innerR * Math.Sin(endRad);
        var x4 = cx + innerR * Math.Cos(startRad);
        var y4 = cy + innerR * Math.Sin(startRad);

        var largeArc = endAngle - startAngle > 180 ? 1 : 0;

        return $"M{x1:0.###},{y1:0.###} A{outerR},{outerR} 0 {largeArc} 1 {x2:0.###},{y2:0.###} " +
               $"L{x3:0.###},{y3:0.###} A{innerR},{innerR} 0 {largeArc} 0 {x4:0.###},{y4:0.###} Z";
    }
}

/// <summary>
/// Legend layout position for the donut chart.
/// </summary>
public enum LegendPosition
{
    /// <summary>Legend is placed to the right of the chart.</summary>
    Right,

    /// <summary>Legend is placed below the chart.</summary>
    Bottom
}
