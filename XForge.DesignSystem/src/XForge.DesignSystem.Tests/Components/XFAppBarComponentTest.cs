using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public class XFAppBarComponentTest : XForgeTestContext
{
    public XFAppBarComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFAppBar>();
        Assert.Contains("<header", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFAppBar>(parameters =>
            parameters.AddChildContent("AppBar content"));
        Assert.Contains("AppBar content", cut.Markup);
    }

    [Fact]
    public void Render_WithFixedPosition()
    {
        var cut = Render<XFAppBar>(parameters =>
            parameters.Add(p => p.Position, XFAppBarPosition.Fixed));
        Assert.Contains("sticky", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotBordered_NoBorderClass()
    {
        var cut = Render<XFAppBar>(parameters =>
            parameters.Add(p => p.Bordered, false));
        Assert.DoesNotContain("border-b", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel()
    {
        var cut = Render<XFAppBar>(parameters =>
            parameters.Add(p => p.AriaLabel, "Custom app bar"));
        Assert.Contains("Custom app bar", cut.Markup);
    }
}
