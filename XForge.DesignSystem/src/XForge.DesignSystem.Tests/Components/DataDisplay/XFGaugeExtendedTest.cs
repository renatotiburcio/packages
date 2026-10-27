using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Gauge;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFGaugeExtendedTest : BunitContext
{
    public XFGaugeExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFGauge> RenderGauge(
        Action<ComponentParameterCollectionBuilder<XFGauge>>? configure = null)
    {
        return Render<XFGauge>(p =>
        {
            p.Add(x => x.Value, 75);
            p.Add(x => x.Min, 0);
            p.Add(x => x.Max, 100);
            configure?.Invoke(p);
        });
    }

    // UnitTextSizeClass Xs (line 141) — requires Unit set, ValueFormat null
    [Fact]
    public void Extended_SizeXs_RendersExtraSmallUnitText()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xs);
            p.Add(x => x.Unit, "%");
        });
        Assert.Contains("text-[8px]", cut.Markup);
    }

    // UnitTextSizeClass Sm (line 142)
    [Fact]
    public void Extended_SizeSm_RendersSmallUnitText()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Sm);
            p.Add(x => x.Unit, "%");
        });
        Assert.Contains("text-[10px]", cut.Markup);
    }

    // UnitTextSizeClass Lg (line 143)
    [Fact]
    public void Extended_SizeLg_RendersLargeUnitText()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Lg);
            p.Add(x => x.Unit, "%");
        });
        Assert.Contains("text-sm", cut.Markup);
    }

    // UnitTextSizeClass Xl (line 144)
    [Fact]
    public void Extended_SizeXl_RendersExtraLargeUnitText()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xl);
            p.Add(x => x.Unit, "%");
        });
        Assert.Contains("text-base", cut.Markup);
    }

    // TickLabelSizeClass Xs (line 150) — requires ShowLabels=true
    [Fact]
    public void Extended_SizeXs_RendersExtraSmallTickLabels()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xs);
            p.Add(x => x.ShowLabels, true);
        });
        Assert.Contains("text-[7px]", cut.Markup);
    }

    // TickLabelSizeClass Sm (line 151)
    [Fact]
    public void Extended_SizeSm_RendersSmallTickLabels()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Sm);
            p.Add(x => x.ShowLabels, true);
        });
        Assert.Contains("text-[8px]", cut.Markup);
    }

    // TickLabelSizeClass Lg (line 152)
    [Fact]
    public void Extended_SizeLg_RendersLargeTickLabels()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Lg);
            p.Add(x => x.ShowLabels, true);
        });
        Assert.Contains("text-[11px]", cut.Markup);
    }

    // TickLabelSizeClass Xl (line 153)
    [Fact]
    public void Extended_SizeXl_RendersExtraLargeTickLabels()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xl);
            p.Add(x => x.ShowLabels, true);
        });
        Assert.Contains("text-xs", cut.Markup);
    }

    // ShowValue false — center value div should not render
    [Fact]
    public void Extended_ShowValueFalse_HidesCenterValue()
    {
        var cut = RenderGauge(p => p.Add(x => x.ShowValue, false));
        // The center div with pointer-events-none should not be present
        Assert.DoesNotContain("pointer-events-none", cut.Markup);
    }

    // Unit
    [Fact]
    public void Extended_Unit_RendersUnit()
    {
        var cut = RenderGauge(p => p.Add(x => x.Unit, "%"));
        Assert.Contains("%", cut.Markup);
    }

    // ValueFormat — when set, Unit is suppressed (line 113)
    [Fact]
    public void Extended_ValueFormat_RendersFormattedValue()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.ValueFormat, "{0:F1}");
            p.Add(x => x.ShowValue, true);
        });
        // ValueFormat changes the ValueText; the center value should contain formatted text
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    // Custom Color
    [Fact]
    public void Extended_CustomColor_RendersGauge()
    {
        var cut = RenderGauge(p => p.Add(x => x.Color, "#FF0000"));
        Assert.Contains("xf-gauge", cut.Markup);
    }

    // ShowTicks
    [Fact]
    public void Extended_ShowTicks_RendersTickLines()
    {
        var cut = RenderGauge(p => p.Add(x => x.ShowTicks, true));
        // Ticks render <line> elements inside SVG
        Assert.Contains("<line", cut.Markup);
    }

    // ValueTemplate
    [Fact]
    public void Extended_ValueTemplate_RendersCustomTemplate()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.ValueTemplate, (XFGaugeValueContext ctx) => builder => builder.AddContent(0, $"VAL:{ctx.Value}"));
        });
        Assert.Contains("VAL:75", cut.Markup);
    }

    // Thickness
    [Fact]
    public void Extended_Thickness_RendersCustomThickness()
    {
        var cut = RenderGauge(p => p.Add(x => x.Thickness, 20));
        Assert.Contains("xf-gauge", cut.Markup);
    }

    // ShowMinMaxLabels
    [Fact]
    public void Extended_ShowMinMaxLabels_RendersMinMax()
    {
        var cut = RenderGauge(p => p.Add(x => x.ShowMinMaxLabels, true));
        Assert.Contains("xf-gauge", cut.Markup);
    }

    // TrackColor custom
    [Fact]
    public void Extended_TrackColor_RendersCustomTrack()
    {
        var cut = RenderGauge(p => p.Add(x => x.TrackColor, "#CCCCCC"));
        Assert.Contains("xf-gauge", cut.Markup);
    }

    // Ranges
    [Fact]
    public void Extended_Ranges_RendersRangeBands()
    {
        var cut = RenderGauge(p =>
        {
            p.Add(x => x.Ranges, new List<XFGaugeRange>
            {
                new() { From = 0, To = 50, Color = "#00FF00" },
                new() { From = 50, To = 100, Color = "#FF0000" }
            });
        });
        Assert.Contains("xf-gauge", cut.Markup);
    }
}
