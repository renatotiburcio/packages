using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Skeleton;

namespace XForge.DesignSystem.Tests.Components;

public class XFSkeletonGroupComponentTest : BunitContext
{
    public XFSkeletonGroupComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFSkeletonGroup>();
        Assert.Contains("xf-skeleton-group", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFSkeletonGroup>(parameters =>
            parameters.AddChildContent("Skeleton child"));
        Assert.Contains("Skeleton child", cut.Markup);
    }

    [Fact]
    public void Render_Decorative_HidesFromAccessibility()
    {
        var cut = Render<XFSkeletonGroup>(parameters =>
        {
            parameters.Add(p => p.Decorative, true);
            parameters.AddChildContent("Decorative");
        });
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_NotDecorative_HasStatusRole()
    {
        var cut = Render<XFSkeletonGroup>(parameters =>
        {
            parameters.Add(p => p.Decorative, false);
            parameters.AddChildContent("Status");
        });
        Assert.Contains("role=\"status\"", cut.Markup);
    }

    [Fact]
    public void Render_Inline_UsesFlexLayout()
    {
        var cut = Render<XFSkeletonGroup>(parameters =>
        {
            parameters.Add(p => p.Inline, true);
            parameters.AddChildContent("Inline");
        });
        Assert.Contains("flex items-center", cut.Markup);
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFSkeletonGroup>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Hidden");
        });
        Assert.DoesNotContain("xf-skeleton-group", cut.Markup);
    }
}