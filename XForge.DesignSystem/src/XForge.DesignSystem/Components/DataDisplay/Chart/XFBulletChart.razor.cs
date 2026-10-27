using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Chart;

/// <summary>
/// Bullet chart comparing actual value against a target, with optional qualitative range bands.
/// Supports horizontal and vertical orientations.
/// </summary>
public partial class XFBulletChart : XForgeComponentBase
{
    protected override string BaseClass => "xf-bullet-chart";

    /// <summary>ARIA label for accessibility.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>The actual measured value.</summary>
    [Parameter] public double Value { get; set; }

    /// <summary>The target / comparative measure.</summary>
    [Parameter] public double Target { get; set; }

    /// <summary>Qualitative range bands displayed behind the value bar (e.g. poor/average/good).</summary>
    [Parameter] public IReadOnlyList<BulletRange> Ranges { get; set; } = Array.Empty<BulletRange>();

    /// <summary>Orientation of the chart.</summary>
    [Parameter] public BulletOrientation Orientation { get; set; } = BulletOrientation.Horizontal;

    /// <summary>Whether to render a marker line at the target position.</summary>
    [Parameter] public bool ShowMarkers { get; set; } = true;

    private RenderFragment RenderHorizontal() => builder =>
    {
        var maxVal = ComputeMax();
        var barY = 25.0;
        var barH = 30.0;

        // Range bands
        foreach (var range in Ranges)
        {
            var x1 = ToHPosX(range.From, maxVal);
            var w = ToHPosX(range.To, maxVal) - x1;
            builder.OpenElement(0, "rect");
            builder.AddAttribute(1, "x", x1);
            builder.AddAttribute(2, "y", barY);
            builder.AddAttribute(3, "width", w);
            builder.AddAttribute(4, "height", barH);
            builder.AddAttribute(5, "fill", range.Color);
            builder.AddAttribute(6, "rx", "2");
            builder.CloseElement();
        }

        // Value bar
        var valueW = ToHPosX(Value, maxVal);
        builder.OpenElement(10, "rect");
        builder.AddAttribute(11, "x", 0);
        builder.AddAttribute(12, "y", barY + 8);
        builder.AddAttribute(13, "width", valueW);
        builder.AddAttribute(14, "height", barH - 16);
        builder.AddAttribute(15, "fill", "var(--color-primary, #3b82f6)");
        builder.AddAttribute(16, "rx", "2");
        builder.CloseElement();

        // Target marker
        if (ShowMarkers)
        {
            var targetX = ToHPosX(Target, maxVal);
            builder.OpenElement(20, "line");
            builder.AddAttribute(21, "x1", targetX);
            builder.AddAttribute(22, "y1", barY - 4);
            builder.AddAttribute(23, "x2", targetX);
            builder.AddAttribute(24, "y2", barY + barH + 4);
            builder.AddAttribute(25, "stroke", "var(--color-text-primary, #0f172a)");
            builder.AddAttribute(26, "stroke-width", "2");
            builder.CloseElement();
        }

        // Labels
        builder.OpenElement(30, "text");
        builder.AddAttribute(31, "x", 0);
        builder.AddAttribute(32, "y", 18);
        builder.AddAttribute(33, "class", "fill-text-secondary text-[10px]");
        builder.AddContent(34, $"Value: {Value:0.##}");
        builder.CloseElement();

        builder.OpenElement(40, "text");
        builder.AddAttribute(41, "x", 0);
        builder.AddAttribute(42, "y", 75);
        builder.AddAttribute(43, "class", "fill-text-secondary text-[10px]");
        builder.AddContent(44, $"Target: {Target:0.##}");
        builder.CloseElement();
    };

    private RenderFragment RenderVertical() => builder =>
    {
        var maxVal = ComputeMax();
        var barX = 25.0;
        var barW = 30.0;
        var svgH = 300.0;

        // Range bands
        foreach (var range in Ranges)
        {
            var y2 = svgH - ToVPosY(range.From, maxVal);
            var h = ToVPosY(range.To, maxVal) - ToVPosY(range.From, maxVal);
            builder.OpenElement(0, "rect");
            builder.AddAttribute(1, "x", barX);
            builder.AddAttribute(2, "y", y2);
            builder.AddAttribute(3, "width", barW);
            builder.AddAttribute(4, "height", h);
            builder.AddAttribute(5, "fill", range.Color);
            builder.AddAttribute(6, "rx", "2");
            builder.CloseElement();
        }

        // Value bar
        var valueH = ToVPosY(Value, maxVal);
        builder.OpenElement(10, "rect");
        builder.AddAttribute(11, "x", barX + 8);
        builder.AddAttribute(12, "y", svgH - valueH);
        builder.AddAttribute(13, "width", barW - 16);
        builder.AddAttribute(14, "height", valueH);
        builder.AddAttribute(15, "fill", "var(--color-primary, #3b82f6)");
        builder.AddAttribute(16, "rx", "2");
        builder.CloseElement();

        // Target marker
        if (ShowMarkers)
        {
            var targetY = svgH - ToVPosY(Target, maxVal);
            builder.OpenElement(20, "line");
            builder.AddAttribute(21, "x1", barX - 4);
            builder.AddAttribute(22, "y1", targetY);
            builder.AddAttribute(23, "x2", barX + barW + 4);
            builder.AddAttribute(24, "y2", targetY);
            builder.AddAttribute(25, "stroke", "var(--color-text-primary, #0f172a)");
            builder.AddAttribute(26, "stroke-width", "2");
            builder.CloseElement();
        }

        // Labels
        builder.OpenElement(30, "text");
        builder.AddAttribute(31, "x", 0);
        builder.AddAttribute(32, "y", svgH - 5);
        builder.AddAttribute(33, "class", "fill-text-secondary text-[10px]");
        builder.AddContent(34, $"{Value:0.##}");
        builder.CloseElement();

        builder.OpenElement(40, "text");
        builder.AddAttribute(41, "x", 0);
        builder.AddAttribute(42, "y", 10);
        builder.AddAttribute(43, "class", "fill-text-secondary text-[10px]");
        builder.AddContent(44, $"{Target:0.##}");
        builder.CloseElement();
    };

    private double ComputeMax()
    {
        var vals = new List<double> { Value, Target };
        vals.AddRange(Ranges.Select(r => Math.Max(r.From, r.To)));
        return vals.Count > 0 ? vals.Max() * 1.1 : 100;
    }

    private double ToHPosX(double val, double max) => val / max * 280;
    private double ToVPosY(double val, double max) => val / max * 260;
}

/// <summary>
/// A qualitative range band on the bullet chart.
/// </summary>
public class BulletRange
{
    /// <summary>Start value of the range.</summary>
    public double From { get; set; }

    /// <summary>End value of the range.</summary>
    public double To { get; set; }

    /// <summary>CSS color for the range background.</summary>
    public string Color { get; set; } = "#e2e8f0";
}

/// <summary>
/// Orientation of the bullet chart.
/// </summary>
public enum BulletOrientation
{
    /// <summary>Chart is laid out left-to-right.</summary>
    Horizontal,

    /// <summary>Chart is laid out bottom-to-top.</summary>
    Vertical
}
