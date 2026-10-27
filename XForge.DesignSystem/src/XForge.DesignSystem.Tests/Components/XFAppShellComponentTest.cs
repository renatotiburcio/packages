using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;

namespace XForge.DesignSystem.Tests.Components;

public class XFAppShellComponentTest : BunitContext
{
    public XFAppShellComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.AddChildContent("Main content"));
        Assert.Contains("Main content", cut.Markup);
    }

    [Fact]
    public void Render_ByDefault_NoFullViewportClasses()
    {
        var cut = Render<XFAppShell>();
        Assert.Contains("h-full", cut.Markup);
        Assert.DoesNotContain("h-dvh", cut.Markup);
    }

    [Fact]
    public void Render_WithFullViewport_AppliesViewportClasses()
    {
        var cut = Render<XFAppShell>(parameters =>
            parameters.Add(p => p.FullViewport, true));
        Assert.Contains("h-dvh", cut.Markup);
        Assert.Contains("w-screen", cut.Markup);
    }

    [Fact]
    public void Render_WithThemeAndBrand()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.Theme, "dark");
            parameters.Add(p => p.Brand, "xforge");
        });
        Assert.Contains("data-theme=\"dark\"", cut.Markup);
        Assert.Contains("data-brand=\"xforge\"", cut.Markup);
    }

    [Fact]
    public void Render_RendersMainContentById()
    {
        var cut = Render<XFAppShell>(parameters =>
        {
            parameters.Add(p => p.MainId, "my-main");
            parameters.AddChildContent("content");
        });
        Assert.Contains("id=\"my-main\"", cut.Markup);
    }
}