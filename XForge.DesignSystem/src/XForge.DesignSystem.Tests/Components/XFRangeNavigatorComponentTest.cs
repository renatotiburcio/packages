using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFRangeNavigatorComponentTest : BunitContext
{
    [Fact]
    public void Render_Default_RendersContainer()
    {
        var cut = Render<XFRangeNavigator>();
        Assert.Contains("xf-range-nav", cut.Markup);
    }

    [Fact]
    public void Render_Default_ShowsSelectionWindow()
    {
        var cut = Render<XFRangeNavigator>();
        Assert.Contains("border-primary", cut.Markup);
    }

    [Fact]
    public void Render_WithShowHandleLabels_ShowsLabels()
    {
        var cut = Render<XFRangeNavigator>(p => p.Add(x => x.ShowHandleLabels, true));
        Assert.Contains("bg-bg-surface", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    [Fact]
    public void Render_WithShowAxis_ShowsAxis()
    {
        var cut = Render<XFRangeNavigator>(p => p.Add(x => x.ShowAxis, true));
        Assert.Contains("border-surface-200", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomStartEnd_AppliesStyles()
    {
        var cut = Render<XFRangeNavigator>(p => p
            .Add(x => x.Start, 0.2)
            .Add(x => x.End, 0.8));
        Assert.Contains("left:", cut.Markup);
        Assert.Contains("width:", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_RendersContent()
    {
        var cut = Render<XFRangeNavigator>(p => p
            .AddChildContent("<div class=\"mini-chart\">Chart</div>"));
        Assert.Contains("mini-chart", cut.Markup);
        Assert.Contains("Chart", cut.Markup);
    }

    [Fact]
    public void Render_WithTickCount_RendersCorrectNumberOfTicks()
    {
        var cut = Render<XFRangeNavigator>(p => p
            .Add(x => x.ShowAxis, true)
            .Add(x => x.TickCount, 3));
        // 4 ticks for count of 3 (0, 1/3, 2/3, 1)
        var tickElements = cut.FindAll(".text-\\[10px\\]");
        Assert.Equal(4, tickElements.Count);
    }

    [Fact]
    public void Render_WithAxisFormat_UsesFormat()
    {
        var cut = Render<XFRangeNavigator>(p => p
            .Add(x => x.ShowAxis, true)
            .Add(x => x.AxisFormat, "{0:0}")
            .Add(x => x.AxisMin, 0)
            .Add(x => x.AxisMax, 100));
        Assert.Contains("0", cut.Markup);
        Assert.Contains("50", cut.Markup);
        Assert.Contains("100", cut.Markup);
    }

    [Fact]
    public void Render_ShowsShadedAreas()
    {
        var cut = Render<XFRangeNavigator>(p => p
            .Add(x => x.Start, 0.3)
            .Add(x => x.End, 0.7));
        Assert.Contains("bg-surface-900/30", cut.Markup);
    }

    [Fact]
    public void Render_ShowsStartEndHandles()
    {
        var cut = Render<XFRangeNavigator>();
        Assert.Contains("cursor-ew-resize", cut.Markup);
        Assert.Contains("bg-primary", cut.Markup);
    }
}
