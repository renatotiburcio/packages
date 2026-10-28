using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Image;

namespace XForge.DesignSystem.Tests.Components;

public class XFFigureComponentTest : XForgeTestContext
{
    public XFFigureComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFFigure>();
        Assert.Contains("xf-figure", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFFigure>(parameters =>
            parameters.AddChildContent("Image here"));
        Assert.Contains("Image here", cut.Markup);
    }

    [Fact]
    public void Render_WithCaption_ShowsCaption()
    {
        var cut = Render<XFFigure>(parameters =>
            parameters.Add(p => p.Caption, "A figure caption"));
        Assert.Contains("A figure caption", cut.Markup);
    }

    [Fact]
    public void Render_Framed_AddsFrameClasses()
    {
        var cut = Render<XFFigure>(parameters =>
            parameters.Add(p => p.Framed, true));
        Assert.Contains("ring-1 ring-border-light", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFFigure>(parameters =>
            parameters.Add(p => p.Visible, false));
        Assert.DoesNotContain("xf-figure", cut.Markup);
    }
}
