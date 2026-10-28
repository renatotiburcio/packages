using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Avatar;

namespace XForge.DesignSystem.Tests.Components;

public class XFAvatarGroupComponentTest : XForgeTestContext
{
    public XFAvatarGroupComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFAvatarGroup>();
        Assert.Contains("xf-avatar-group", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFAvatarGroup>(parameters =>
            parameters.AddChildContent("Test"));
        Assert.Contains("Test", cut.Markup);
    }

    [Fact]
    public void Render_Stacked_UsesNegativeSpacing()
    {
        var cut = Render<XFAvatarGroup>(parameters =>
            parameters.Add(p => p.Stacked, true));
        Assert.Contains("-space-x-1", cut.Markup);
    }

    [Fact]
    public void Render_NotStacked_UsesGap()
    {
        var cut = Render<XFAvatarGroup>(parameters =>
            parameters.Add(p => p.Stacked, false));
        Assert.Contains("gap-2", cut.Markup);
    }

    [Fact]
    public void Render_WithOverflowCount_ShowsOverflowLabel()
    {
        var cut = Render<XFAvatarGroup>(parameters =>
        {
            parameters.Add(p => p.OverflowCount, 3);
            parameters.AddChildContent("avatars");
        });
        Assert.Contains("3", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFAvatarGroup>(parameters =>
            parameters.Add(p => p.Visible, false));
        Assert.DoesNotContain("xf-avatar-group", cut.Markup);
    }
}
