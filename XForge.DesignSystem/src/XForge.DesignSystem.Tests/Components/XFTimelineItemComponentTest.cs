#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Timeline;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Components.Layout;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFTimelineItemComponentTest : Bunit.BunitContext
{
    public XFTimelineItemComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFTimelineItem>();
        Assert.Contains("xf-timeline-item", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle()
    {
        var cut = Render<XFTimelineItem>(parameters =>
        {
            parameters.Add(p => p.Title, "Deployment");
        });
        Assert.Contains("Deployment", cut.Markup);
    }

    [Fact]
    public void Render_WithTimestamp()
    {
        var cut = Render<XFTimelineItem>(parameters =>
        {
            parameters.Add(p => p.Timestamp, "Jan 2026");
        });
        Assert.Contains("Jan 2026", cut.Markup);
    }

    [Fact]
    public void Render_WithText()
    {
        var cut = Render<XFTimelineItem>(parameters =>
        {
            parameters.Add(p => p.Text, "Feature shipped successfully.");
        });
        Assert.Contains("Feature shipped successfully.", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFTimelineItem>(parameters =>
        {
            parameters.AddChildContent("Custom detail content");
        });
        Assert.Contains("Custom detail content", cut.Markup);
    }
}
