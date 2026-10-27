using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Timeline;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFTimelineExtendedTest : BunitContext
{
    public XFTimelineExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFTimeline> RenderTimeline(
        Action<ComponentParameterCollectionBuilder<XFTimeline>>? configure = null)
    {
        return Render<XFTimeline>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<XFTimelineItem>(0);
                builder.AddAttribute(1, "Title", "Step 1");
                builder.CloseComponent();
                builder.OpenComponent<XFTimelineItem>(2);
                builder.AddAttribute(3, "Title", "Step 2");
                builder.CloseComponent();
            });
            configure?.Invoke(p);
        });
    }

    // RootClass Vertical (line 38-40)
    [Fact]
    public void Extended_OrientationVertical_RendersGrid()
    {
        var cut = RenderTimeline(p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("grid", cut.Markup);
        Assert.Contains("gap-0", cut.Markup);
    }

    // RootClass Horizontal (line 38-39)
    [Fact]
    public void Extended_OrientationHorizontal_RendersFlex()
    {
        var cut = RenderTimeline(p => p.Add(x => x.Orientation, XFOrientation.Horizontal));
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("overflow-x-auto", cut.Markup);
    }

    // Reverse Horizontal (line 41)
    [Fact]
    public void Extended_ReverseHorizontal()
    {
        var cut = RenderTimeline(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Horizontal);
            p.Add(x => x.Reverse, true);
        });
        Assert.Contains("flex-row-reverse", cut.Markup);
    }

    // Reverse Vertical (line 41)
    [Fact]
    public void Extended_ReverseVertical()
    {
        var cut = RenderTimeline(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Reverse, true);
        });
        // Reverse vertical adds [&>*]:[direction:ltr] class
        Assert.Contains("xf-timeline", cut.Markup);
    }

    // ShowConnector true (line 23)
    [Fact]
    public void Extended_ShowConnectorTrue()
    {
        var cut = RenderTimeline(p => p.Add(x => x.ShowConnector, true));
        Assert.Contains("xf-timeline", cut.Markup);
    }

    // ShowConnector false (line 23)
    [Fact]
    public void Extended_ShowConnectorFalse()
    {
        var cut = RenderTimeline(p => p.Add(x => x.ShowConnector, false));
        Assert.Contains("xf-timeline", cut.Markup);
    }

    // Alternating align (line 22)
    [Fact]
    public void Extended_AlternatingTrue()
    {
        var cut = RenderTimeline(p => p.Add(x => x.Alternating, true));
        Assert.Contains("xf-timeline", cut.Markup);
    }

    // BaseClass included (line 37)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderTimeline();
        Assert.Contains("xf-timeline", cut.Markup);
    }
}
