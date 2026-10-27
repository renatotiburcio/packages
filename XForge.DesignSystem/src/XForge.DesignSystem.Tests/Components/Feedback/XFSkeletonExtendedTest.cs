using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Feedback.Skeleton;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public class XFSkeletonExtendedTest : BunitContext
{
    public XFSkeletonExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFSkeleton> RenderSkeleton(
        Action<ComponentParameterCollectionBuilder<XFSkeleton>>? configure = null)
    {
        return Render<XFSkeleton>(p => configure?.Invoke(p));
    }

    // BaseShapeClass Text (default) (line 40)
    [Fact]
    public void Extended_ShapeText_RendersRoundedFull()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Shape, XFSkeletonShape.Text));
        Assert.Contains("rounded-full", cut.Markup);
        Assert.Contains("bg-surface-200", cut.Markup);
    }

    // BaseShapeClass Circle (line 36)
    [Fact]
    public void Extended_ShapeCircle_RendersCircle()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Shape, XFSkeletonShape.Circle));
        Assert.Contains("rounded-full", cut.Markup);
        Assert.Contains("shrink-0", cut.Markup);
    }

    // BaseShapeClass Avatar (line 37)
    [Fact]
    public void Extended_ShapeAvatar_RendersAvatar()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Shape, XFSkeletonShape.Avatar));
        Assert.Contains("rounded-full", cut.Markup);
    }

    // BaseShapeClass Rectangle (line 38)
    [Fact]
    public void Extended_ShapeRectangle_RendersRectangle()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Shape, XFSkeletonShape.Rectangle));
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.DoesNotContain("rounded-full", cut.Markup);
    }

    // BaseShapeClass Custom (line 39)
    [Fact]
    public void Extended_ShapeCustom_RendersCustom()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Shape, XFSkeletonShape.Custom));
        Assert.DoesNotContain("rounded", cut.Markup);
    }

    // SizeClass Text Xs (line 64)
    [Fact]
    public void Extended_SizeTextXs()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Text);
            p.Add(x => x.Size, XFExtendedSize.Xs);
        });
        Assert.Contains("h-2", cut.Markup);
    }

    // SizeClass Text Sm (line 65)
    [Fact]
    public void Extended_SizeTextSm()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Text);
            p.Add(x => x.Size, XFExtendedSize.Sm);
        });
        Assert.Contains("h-2.5", cut.Markup);
    }

    // SizeClass Text Lg (line 66)
    [Fact]
    public void Extended_SizeTextLg()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Text);
            p.Add(x => x.Size, XFExtendedSize.Lg);
        });
        Assert.Contains("h-4", cut.Markup);
    }

    // SizeClass Text Xl (line 67)
    [Fact]
    public void Extended_SizeTextXl()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Text);
            p.Add(x => x.Size, XFExtendedSize.Xl);
        });
        Assert.Contains("h-5", cut.Markup);
    }

    // SizeClass Text default Md (line 68)
    [Fact]
    public void Extended_SizeTextDefault()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Shape, XFSkeletonShape.Text));
        Assert.Contains("h-3", cut.Markup);
    }

    // SizeClass Circle Xs (line 47)
    [Fact]
    public void Extended_SizeCircleXs()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Circle);
            p.Add(x => x.Size, XFExtendedSize.Xs);
        });
        Assert.Contains("h-6", cut.Markup);
        Assert.Contains("w-6", cut.Markup);
    }

    // SizeClass Circle Lg (line 49)
    [Fact]
    public void Extended_SizeCircleLg()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Circle);
            p.Add(x => x.Size, XFExtendedSize.Lg);
        });
        Assert.Contains("h-14", cut.Markup);
        Assert.Contains("w-14", cut.Markup);
    }

    // SizeClass Circle Xl (line 50)
    [Fact]
    public void Extended_SizeCircleXl()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Circle);
            p.Add(x => x.Size, XFExtendedSize.Xl);
        });
        Assert.Contains("h-20", cut.Markup);
        Assert.Contains("w-20", cut.Markup);
    }

    // SizeClass Rectangle Sm (line 56)
    [Fact]
    public void Extended_SizeRectangleSm()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Rectangle);
            p.Add(x => x.Size, XFExtendedSize.Sm);
        });
        Assert.Contains("h-16", cut.Markup);
    }

    // SizeClass Rectangle Xl (line 58)
    [Fact]
    public void Extended_SizeRectangleXl()
    {
        var cut = RenderSkeleton(p =>
        {
            p.Add(x => x.Shape, XFSkeletonShape.Rectangle);
            p.Add(x => x.Size, XFExtendedSize.Xl);
        });
        Assert.Contains("h-48", cut.Markup);
    }

    // AnimationClass Pulse (default) (line 75)
    [Fact]
    public void Extended_AnimationPulse_RendersAnimatePulse()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Animation, XFSkeletonAnimation.Pulse));
        Assert.Contains("animate-pulse", cut.Markup);
    }

    // AnimationClass Shimmer (line 74)
    [Fact]
    public void Extended_AnimationShimmer_RendersBefore()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Animation, XFSkeletonAnimation.Shimmer));
        Assert.Contains("before:absolute", cut.Markup);
        Assert.Contains("before:inset-0", cut.Markup);
    }

    // AnimationClass None (line 76)
    [Fact]
    public void Extended_AnimationNone_NoAnimation()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Animation, XFSkeletonAnimation.None));
        Assert.DoesNotContain("animate-pulse", cut.Markup);
        Assert.DoesNotContain("before:absolute", cut.Markup);
    }

    // CustomSizeClass WidthClass (line 85-87)
    [Fact]
    public void Extended_CustomWidthClass_Rendered()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.WidthClass, "w-32"));
        Assert.Contains("w-32", cut.Markup);
    }

    // CustomSizeClass HeightClass (line 90-92)
    [Fact]
    public void Extended_CustomHeightClass_Rendered()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.HeightClass, "h-12"));
        Assert.Contains("h-12", cut.Markup);
    }

    // CustomSizeClass RoundedClass (line 95-97)
    [Fact]
    public void Extended_CustomRoundedClass_Rendered()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.RoundedClass, "rounded-xl"));
        Assert.Contains("rounded-xl", cut.Markup);
    }

    // Visible false renders nothing (line 14)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // Decorative true renders aria-hidden (line 25)
    [Fact]
    public void Extended_DecorativeTrue_RendersAriaHidden()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Decorative, true));
        Assert.Equal("true", cut.Find("*").GetAttribute("aria-hidden"));
    }

    // Decorative false renders role=status (line 23)
    [Fact]
    public void Extended_DecorativeFalse_RendersRoleStatus()
    {
        var cut = RenderSkeleton(p => p.Add(x => x.Decorative, false));
        Assert.Equal("status", cut.Find("*").GetAttribute("role"));
    }

    // BaseClass included (line 31)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderSkeleton();
        Assert.Contains("xf-skeleton", cut.Markup);
    }
}
