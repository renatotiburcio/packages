using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;

namespace XForge.DesignSystem.Tests.Components;

public class XFSidebarComponentTest : BunitContext
{
    public XFSidebarComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithDefaultExpanded_NoCollapsedClass()
    {
        var cut = Render<XFSidebar>();
        Assert.DoesNotContain("!w-0", cut.Markup);
    }

    [Fact]
    public void Render_WhenExpandedFalse_HasCollapsedClass()
    {
        var cut = Render<XFSidebar>(parameters =>
            parameters.Add(p => p.Expanded, false));
        Assert.Contains("!w-0", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFSidebar>(parameters =>
            parameters.AddChildContent("Sidebar item"));
        Assert.Contains("Sidebar item", cut.Markup);
    }

    [Fact]
    public void Render_WhenDrawerVariant_HasDrawerClass()
    {
        var cut = Render<XFSidebar>(parameters =>
            parameters.Add(p => p.Variant, XFSidebarVariant.Drawer));
        Assert.Contains("fixed", cut.Markup);
    }

    [Fact]
    public void Render_WhenMiniVariant_HasMiniWidthClass()
    {
        var cut = Render<XFSidebar>(parameters =>
            parameters.Add(p => p.Variant, XFSidebarVariant.Mini));
        Assert.Contains("w-20", cut.Markup);
    }
}