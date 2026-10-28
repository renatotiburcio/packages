using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Layout.Splitter;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public class XFMultiSplitterComponentTest : XForgeTestContext
{
    public XFMultiSplitterComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithChildContent_RendersContent()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
            parameters.AddChildContent("Splitter content"));
        Assert.Contains("Splitter content", cut.Markup);
    }

    [Fact]
    public void Render_WithOrientation_Horizontal()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.Orientation, XFOrientation.Horizontal);
            parameters.AddChildContent("H-Split");
        });
        Assert.Contains("H-Split", cut.Markup);
    }

    [Fact]
    public void Render_WithOrientation_Vertical()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.Orientation, XFOrientation.Vertical);
            parameters.AddChildContent("V-Split");
        });
        Assert.Contains("V-Split", cut.Markup);
    }

    [Fact]
    public void Render_WithSizes()
    {
        var sizes = new List<double> { 1.0, 2.0 };
        var cut = Render<XFMultiSplitter>(parameters =>
        {
            parameters.Add(p => p.Sizes, sizes);
            parameters.AddChildContent("Sized");
        });
        Assert.Contains("Sized", cut.Markup);
    }

    [Fact]
    public void Render_WhenVisibleFalse_RendersNothing()
    {
        var cut = Render<XFMultiSplitter>(parameters =>
            parameters.Add(p => p.Visible, false));
        Assert.Equal("", cut.Markup.Trim());
    }
}
