using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public class XFSkeletonGroupExtendedTest : XForgeTestContext
{
    public XFSkeletonGroupExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFSkeletonGroup> RenderSkeletonGroup(
        Action<ComponentParameterCollectionBuilder<XFSkeletonGroup>>? configure = null)
    {
        return Render<XFSkeletonGroup>(p =>
        {
            p.AddChildContent("content");
            configure?.Invoke(p);
        });
    }

    // GetComponentClass Inline (line 26)
    [Fact]
    public void Extended_Inline_RendersFlex()
    {
        var cut = RenderSkeletonGroup(p => p.Add(x => x.Inline, true));
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("items-center", cut.Markup);
    }

    // GetComponentClass default block (line 26)
    [Fact]
    public void Extended_InlineFalse_RendersGrid()
    {
        var cut = RenderSkeletonGroup(p => p.Add(x => x.Inline, false));
        Assert.Contains("grid", cut.Markup);
        Assert.DoesNotContain("flex", cut.Markup);
    }

    // GapClass default (line 27)
    [Fact]
    public void Extended_GapClassDefault()
    {
        var cut = RenderSkeletonGroup();
        Assert.Contains("gap-3", cut.Markup);
    }

    // GapClass custom (line 27)
    [Fact]
    public void Extended_GapClassCustom()
    {
        var cut = RenderSkeletonGroup(p => p.Add(x => x.GapClass, "gap-5"));
        Assert.Contains("gap-5", cut.Markup);
    }

    // Visible false renders nothing (line 10)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderSkeletonGroup(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // Decorative true renders aria-hidden (line 20)
    [Fact]
    public void Extended_DecorativeTrue_RendersAriaHidden()
    {
        var cut = RenderSkeletonGroup(p => p.Add(x => x.Decorative, true));
        Assert.Equal("true", cut.Find("*").GetAttribute("aria-hidden"));
    }

    // Decorative false renders role=status (line 18)
    [Fact]
    public void Extended_DecorativeFalse_RendersRoleStatus()
    {
        var cut = RenderSkeletonGroup(p => p.Add(x => x.Decorative, false));
        Assert.Equal("status", cut.Find("*").GetAttribute("role"));
    }

    // BaseClass included (line 27)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderSkeletonGroup();
        Assert.Contains("xf-skeleton-group", cut.Markup);
    }
}
