#region Using directives
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Gauge;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Components.Layout;
#endregion

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Tests for XFGauge (radial) and XFLinearGauge components.
/// Covers rendering, ARIA, value display, ranges, ticks, labels, sizes, orientations.
/// </summary>
public class XFGaugeComponentTest : XForgeTestContext
{
    #region XFGauge — Rendering

    [Fact]
    public void Gauge_Renders_Default()
    {
        var cut = Render<XFGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("xf-gauge", cut.Markup);
        Assert.Contains("role=\"meter\"", cut.Markup);
        Assert.Contains("aria-valuenow=\"50\"", cut.Markup);
    }

    [Fact]
    public void Gauge_Renders_WithCustomMinMax()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Min, 10);
            p.Add(g => g.Max, 200);
            p.Add(g => g.Value, 100);
        });

        Assert.Contains("aria-valuemin=\"10\"", cut.Markup);
        Assert.Contains("aria-valuemax=\"200\"", cut.Markup);
        Assert.Contains("aria-valuenow=\"100\"", cut.Markup);
    }

    [Fact]
    public void Gauge_Renders_SVG_ArcPath()
    {
        var cut = Render<XFGauge>(p => p.Add(g => g.Value, 75));

        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("<path", cut.Markup);
        Assert.Contains("viewBox=\"0 0 200 200\"", cut.Markup);
    }

    [Fact]
    public void Gauge_Clamps_Value_BelowMin()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Min, 10);
            p.Add(g => g.Max, 100);
            p.Add(g => g.Value, 0);
        });

        Assert.Contains("aria-valuenow=\"10\"", cut.Markup);
    }

    [Fact]
    public void Gauge_Clamps_Value_AboveMax()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Min, 0);
            p.Add(g => g.Max, 100);
            p.Add(g => g.Value, 200);
        });

        Assert.Contains("aria-valuenow=\"100\"", cut.Markup);
    }

    #endregion

    #region XFGauge — Value Display

    [Fact]
    public void Gauge_ShowValue_True_RendersValueText()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 42);
            p.Add(g => g.ShowValue, true);
        });

        Assert.Contains("42", cut.Markup);
    }

    [Fact]
    public void Gauge_ShowValue_False_HidesValueText()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 42);
            p.Add(g => g.ShowValue, false);
        });

        // The center value div should not be present
        Assert.DoesNotContain("grid place-items-center", cut.Markup);
    }

    [Fact]
    public void Gauge_WithUnit_ShowsUnitText()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 75);
            p.Add(g => g.Unit, "°C");
        });

        Assert.Contains("°C", cut.Markup);
    }

    [Fact]
    public void Gauge_WithValueFormat_AppliesFormat()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 75.5);
            p.Add(g => g.ValueFormat, "{0:F1}%");
        });

        // Culture-sensitive: may be "75.5%" or "75,5%" depending on locale
        var expected = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:F1}%", 75.5);
        Assert.Contains(expected, cut.Markup);
    }

    [Fact]
    public void Gauge_ValueTemplate_IsRendered()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.ValueTemplate, ctx => builder =>
            {
                builder.AddContent(0, $"Custom: {ctx.Value:F0} ({ctx.Percent:F0}%)");
            });
        });

        Assert.Contains("Custom: 50 (50%)", cut.Markup);
    }

    #endregion

    #region XFGauge — Ranges

    [Fact]
    public void Gauge_WithRanges_RendersRangeArcs()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Ranges, new List<XFGaugeRange>
            {
                new() { From = 0, To = 30, Color = "stroke-success" },
                new() { From = 30, To = 70, Color = "stroke-warning" },
                new() { From = 70, To = 100, Color = "stroke-danger" }
            });
        });

        // Should have 3 range arcs + 1 track arc + 1 value arc = 5 path elements
        var pathCount = cut.Markup.Split("<path").Length - 1;
        Assert.True(pathCount >= 4, $"Expected at least 4 <path> elements, found {pathCount}");
    }

    #endregion

    #region XFGauge — Ticks and Labels

    [Fact]
    public void Gauge_WithTicks_RendersTickLines()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.ShowTicks, true);
        });

        Assert.Contains("<line", cut.Markup);
    }

    [Fact]
    public void Gauge_WithLabels_RendersTextElements()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.ShowTicks, true);
            p.Add(g => g.ShowLabels, true);
        });

        Assert.Contains("<text", cut.Markup);
    }

    [Fact]
    public void Gauge_WithMinMaxLabels_RendersMinMaxText()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.ShowMinMaxLabels, true);
        });

        Assert.Contains("<text", cut.Markup);
    }

    [Fact]
    public void Gauge_WithCustomTickStep_UsesCustomStep()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.ShowTicks, true);
            p.Add(g => g.TickStep, 25);
        });

        Assert.Contains("<line", cut.Markup);
    }

    #endregion

    #region XFGauge — ARIA

    [Fact]
    public void Gauge_Has_MeterRole()
    {
        var cut = Render<XFGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("role=\"meter\"", cut.Markup);
    }

    [Fact]
    public void Gauge_Has_AriaLabel()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.AriaLabel, "CPU Usage");
        });

        Assert.Contains("aria-label=\"CPU Usage\"", cut.Markup);
    }

    [Fact]
    public void Gauge_Has_DefaultAriaLabel()
    {
        var cut = Render<XFGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("aria-label=\"Medidor\"", cut.Markup);
    }

    [Fact]
    public void Gauge_SVG_IsHiddenFromAria()
    {
        var cut = Render<XFGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Gauge_AriaValueText_WithUnit()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 75);
            p.Add(g => g.Unit, "%");
        });

        Assert.Contains("aria-valuetext=\"75 %\"", cut.Markup);
    }

    #endregion

    #region XFGauge — Sizes

    [Theory]
    [InlineData(XFExtendedSize.Xs, "h-16 w-16")]
    [InlineData(XFExtendedSize.Sm, "h-24 w-24")]
    [InlineData(XFExtendedSize.Md, "h-40 w-40")]
    [InlineData(XFExtendedSize.Lg, "h-56 w-56")]
    [InlineData(XFExtendedSize.Xl, "h-72 w-72")]
    public void Gauge_Size_RendersCorrectClass(XFExtendedSize size, string expectedClass)
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Size, size);
        });

        Assert.Contains(expectedClass, cut.Markup);
    }

    #endregion

    #region XFGauge — Arc Angles

    [Fact]
    public void Gauge_CustomAngles_AreUsed()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.StartAngle, -90);
            p.Add(g => g.EndAngle, 90);
        });

        // Should render without errors
        Assert.Contains("xf-gauge", cut.Markup);
        Assert.Contains("<path", cut.Markup);
    }

    [Fact]
    public void Gauge_FullCircle_360Degree()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 75);
            p.Add(g => g.StartAngle, 0);
            p.Add(g => g.EndAngle, 360);
        });

        Assert.Contains("xf-gauge", cut.Markup);
    }

    #endregion

    #region XFGauge — Color

    [Fact]
    public void Gauge_CustomColor_RendersStyle()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Color, "#ff0000");
        });

        Assert.Contains("stroke:#ff0000", cut.Markup);
    }

    [Fact]
    public void Gauge_TailwindColorClass_IsApplied()
    {
        var cut = Render<XFGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Color, "stroke-success");
        });

        Assert.Contains("stroke-success", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Rendering

    [Fact]
    public void LinearGauge_Renders_Default()
    {
        var cut = Render<XFLinearGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("xf-linear-gauge", cut.Markup);
        Assert.Contains("role=\"meter\"", cut.Markup);
        Assert.Contains("aria-valuenow=\"50\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_Renders_SVG_Rects()
    {
        var cut = Render<XFLinearGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("<rect", cut.Markup);
        Assert.Contains("viewBox=\"0 0 100 100\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_Clamps_Value_BelowMin()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Min, 10);
            p.Add(g => g.Max, 100);
            p.Add(g => g.Value, 0);
        });

        Assert.Contains("aria-valuenow=\"10\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_Clamps_Value_AboveMax()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Min, 0);
            p.Add(g => g.Max, 100);
            p.Add(g => g.Value, 200);
        });

        Assert.Contains("aria-valuenow=\"100\"", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Orientation

    [Fact]
    public void LinearGauge_Horizontal_IsDefault()
    {
        var cut = Render<XFLinearGauge>(p => p.Add(g => g.Value, 50));

        // Horizontal layout: wider than tall
        Assert.Contains("h-12 w-48", cut.Markup);
    }

    [Fact]
    public void LinearGauge_Vertical_RendersTallLayout()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Orientation, XFOrientation.Vertical);
        });

        Assert.Contains("h-48 w-12", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Value Display

    [Fact]
    public void LinearGauge_ShowValue_True_RendersValueText()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 42);
            p.Add(g => g.ShowValue, true);
        });

        Assert.Contains("42", cut.Markup);
    }

    [Fact]
    public void LinearGauge_WithUnit_ShowsUnitText()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 75);
            p.Add(g => g.Unit, "km/h");
        });

        Assert.Contains("km/h", cut.Markup);
    }

    [Fact]
    public void LinearGauge_ValueTemplate_IsRendered()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.ValueTemplate, ctx => builder =>
            {
                builder.AddContent(0, $"Speed: {ctx.Value:F0}");
            });
        });

        Assert.Contains("Speed: 50", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Ranges

    [Fact]
    public void LinearGauge_WithRanges_RendersRangeRects()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Ranges, new List<XFGaugeRange>
            {
                new() { From = 0, To = 30, Color = "fill-success" },
                new() { From = 30, To = 70, Color = "fill-warning" },
                new() { From = 70, To = 100, Color = "fill-danger" }
            });
        });

        // Should have 3 range rects + 1 track rect + 1 value rect = 5 rect elements
        var rectCount = cut.Markup.Split("<rect").Length - 1;
        Assert.True(rectCount >= 4, $"Expected at least 4 <rect> elements, found {rectCount}");
    }

    #endregion

    #region XFLinearGauge — Reversed

    [Fact]
    public void LinearGauge_Reversed_Renders()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Reversed, true);
        });

        Assert.Contains("xf-linear-gauge", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — ARIA

    [Fact]
    public void LinearGauge_Has_MeterRole()
    {
        var cut = Render<XFLinearGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("role=\"meter\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_Has_AriaLabel()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.AriaLabel, "Speed");
        });

        Assert.Contains("aria-label=\"Speed\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_Has_DefaultAriaLabel()
    {
        var cut = Render<XFLinearGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("aria-label=\"Medidor linear\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_SVG_IsHiddenFromAria()
    {
        var cut = Render<XFLinearGauge>(p => p.Add(g => g.Value, 50));

        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Sizes

    [Theory]
    [InlineData(XFExtendedSize.Xs, "h-8 w-24")]
    [InlineData(XFExtendedSize.Sm, "h-10 w-32")]
    [InlineData(XFExtendedSize.Md, "h-12 w-48")]
    [InlineData(XFExtendedSize.Lg, "h-16 w-64")]
    [InlineData(XFExtendedSize.Xl, "h-20 w-80")]
    public void LinearGauge_HorizontalSize_RendersCorrectClass(XFExtendedSize size, string expectedClass)
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Size, size);
        });

        Assert.Contains(expectedClass, cut.Markup);
    }

    [Theory]
    [InlineData(XFExtendedSize.Xs, "h-24 w-8")]
    [InlineData(XFExtendedSize.Sm, "h-32 w-10")]
    [InlineData(XFExtendedSize.Md, "h-48 w-12")]
    [InlineData(XFExtendedSize.Lg, "h-64 w-16")]
    [InlineData(XFExtendedSize.Xl, "h-80 w-20")]
    public void LinearGauge_VerticalSize_RendersCorrectClass(XFExtendedSize size, string expectedClass)
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Size, size);
            p.Add(g => g.Orientation, XFOrientation.Vertical);
        });

        Assert.Contains(expectedClass, cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Color

    [Fact]
    public void LinearGauge_CustomColor_RendersStyle()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Color, "#00ff00");
        });

        Assert.Contains("fill:#00ff00", cut.Markup);
    }

    [Fact]
    public void LinearGauge_TailwindColorClass_IsApplied()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Color, "fill-success");
        });

        Assert.Contains("fill-success", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Rounded

    [Fact]
    public void LinearGauge_Rounded_True_RendersRoundedEnds()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Rounded, true);
        });

        // Track and value rects should have rx > 0
        Assert.Contains("rx=\"4\"", cut.Markup);
    }

    [Fact]
    public void LinearGauge_Rounded_False_RendersSquareEnds()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Value, 50);
            p.Add(g => g.Rounded, false);
        });

        Assert.Contains("rx=\"0\"", cut.Markup);
    }

    #endregion

    #region XFLinearGauge — Min/Max

    [Fact]
    public void LinearGauge_CustomMinMax_AreReflectedInAria()
    {
        var cut = Render<XFLinearGauge>(p =>
        {
            p.Add(g => g.Min, -50);
            p.Add(g => g.Max, 50);
            p.Add(g => g.Value, 25);
        });

        Assert.Contains("aria-valuemin=\"-50\"", cut.Markup);
        Assert.Contains("aria-valuemax=\"50\"", cut.Markup);
        Assert.Contains("aria-valuenow=\"25\"", cut.Markup);
    }

    #endregion
}
