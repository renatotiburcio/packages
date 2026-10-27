using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Gauge;

/// <summary>
/// Full-circle radial gauge with pointer needle, arc track, ranges, ticks, and center value.
/// </summary>
public partial class XFRadialGauge : XForgeComponentBase
{
    protected override string BaseClass => "xf-radial-gauge relative";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>Current value to display.</summary>
    [Parameter] public double Value { get; set; }

    /// <summary>Minimum scale value.</summary>
    [Parameter] public double Min { get; set; }

    /// <summary>Maximum scale value.</summary>
    [Parameter] public double Max { get; set; } = 100;

    /// <summary>Arc start angle in degrees (0° = right, default -120°).</summary>
    [Parameter] public double StartAngle { get; set; } = -120;

    /// <summary>Sweep angle in degrees (default 240°).</summary>
    [Parameter] public double SweepAngle { get; set; } = 240;

    /// <summary>Arc stroke width.</summary>
    [Parameter] public double ArcWidth { get; set; } = 8;

    /// <summary>Value arc color (CSS class). Null = primary.</summary>
    [Parameter] public string? Color { get; set; }

    /// <summary>Track arc color (CSS class).</summary>
    [Parameter] public string? TrackColor { get; set; }

    /// <summary>Whether to show the center value text.</summary>
    [Parameter] public bool ShowValue { get; set; } = true;

    /// <summary>Custom value rendering template.</summary>
    [Parameter] public RenderFragment<XFRadialGaugeValueContext>? ValueTemplate { get; set; }

    /// <summary>Format string for the displayed value.</summary>
    [Parameter] public string? ValueFormat { get; set; }

    /// <summary>Unit suffix displayed below the value.</summary>
    [Parameter] public string? Unit { get; set; }

    /// <summary>Whether to show the pointer needle.</summary>
    [Parameter] public bool ShowPointer { get; set; } = true;

    /// <summary>Pointer needle length from center.</summary>
    [Parameter] public double PointerLength { get; set; } = 65;

    /// <summary>Pointer needle stroke width.</summary>
    [Parameter] public double PointerWidth { get; set; } = 2;

    /// <summary>Pointer center dot radius.</summary>
    [Parameter] public double PointerDotRadius { get; set; } = 4;

    /// <summary>Pointer color (CSS class).</summary>
    [Parameter] public string? PointerColor { get; set; }

    /// <summary>Whether to show tick marks.</summary>
    [Parameter] public bool ShowTicks { get; set; }

    /// <summary>Number of tick marks.</summary>
    [Parameter] public int TickCount { get; set; } = 10;

    /// <summary>Whether to show tick labels.</summary>
    [Parameter] public bool ShowTickLabels { get; set; }

    /// <summary>Number of tick labels.</summary>
    [Parameter] public int TickLabelCount { get; set; } = 5;

    /// <summary>Format string for tick labels.</summary>
    [Parameter] public string TickLabelFormat { get; set; } = "0";

    /// <summary>Color range bands rendered on the arc.</summary>
    [Parameter] public IReadOnlyList<XFGaugeRange>? Ranges { get; set; }

    private double Range => Max - Min;
    private double Percent => Math.Clamp((Value - Min) / Range, 0, 1);
    private double ValueAngle => StartAngle + SweepAngle * Percent;

    private double GetAngleForValue(double value)
    {
        var pct = Math.Clamp((value - Min) / Range, 0, 1);
        return StartAngle + SweepAngle * pct;
    }

    private string FormatValue(double value)
    {
        return string.IsNullOrEmpty(ValueFormat)
            ? value.ToString("0.#")
            : string.Format(ValueFormat, value);
    }

    private static string DescribeArc(double startAngle, double endAngle, double radius)
    {
        var startRad = startAngle * Math.PI / 180;
        var endRad = endAngle * Math.PI / 180;

        var x1 = 100 + radius * Math.Cos(startRad);
        var y1 = 100 + radius * Math.Sin(startRad);
        var x2 = 100 + radius * Math.Cos(endRad);
        var y2 = 100 + radius * Math.Sin(endRad);

        var sweep = endAngle - startAngle;
        var largeArc = Math.Abs(sweep) > 180 ? 1 : 0;
        var sweepFlag = sweep > 0 ? 1 : 0;

        return $"M {x1} {y1} A {radius} {radius} 0 {largeArc} {sweepFlag} {x2} {y2}";
    }

    private static string RenderSvgText(double x, double y, string text, string cssClass)
    {
        return $"""<text x="{x:0.###}" y="{y:0.###}" text-anchor="middle" dominant-baseline="middle" class="{cssClass}">{text}</text>""";
    }
}

/// <summary>
/// Context for the radial gauge value template.
/// </summary>
public record XFRadialGaugeValueContext(double Value, double Min, double Max, double Percent);
