using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Utilities.Collapse;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Utilities;

public class XFCollapseExtendedTest : BunitContext
{
    public XFCollapseExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFCollapse> RenderCollapse(
        Action<ComponentParameterCollectionBuilder<XFCollapse>>? configure = null)
    {
        return Render<XFCollapse>(p =>
        {
            p.Add(x => x.Title, "Collapse Title");
            p.AddChildContent("Collapse content");
            configure?.Invoke(p);
        });
    }

    // VariantOuterClass Plain (line 134)
    [Fact]
    public void Extended_VariantPlain_NoExtraClass()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Variant, XFCollapseVariant.Plain));
        Assert.Contains("xf-collapse", cut.Markup);
    }

    // VariantOuterClass Surface (line 130)
    [Fact]
    public void Extended_VariantSurface_RendersSurface()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Variant, XFCollapseVariant.Surface));
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-surface-50/30", cut.Markup);
    }

    // VariantOuterClass Soft (line 131)
    [Fact]
    public void Extended_VariantSoft_RendersSoft()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Variant, XFCollapseVariant.Soft));
        Assert.Contains("bg-primary/[0.04]", cut.Markup);
    }

    // VariantOuterClass Outline (line 132)
    [Fact]
    public void Extended_VariantOutline_RendersOutline()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Variant, XFCollapseVariant.Outline));
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // VariantOuterClass Ghost (line 133)
    [Fact]
    public void Extended_VariantGhost_RendersGhost()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Variant, XFCollapseVariant.Ghost));
        Assert.Contains("border-transparent", cut.Markup);
    }

    // HeaderClass Sm (line 146)
    [Fact]
    public void Extended_HeaderSizeSm()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("px-3", cut.Markup);
        Assert.Contains("py-2", cut.Markup);
        Assert.Contains("min-h-[40px]", cut.Markup);
        Assert.Contains("text-sm", cut.Markup);
    }

    // HeaderClass Lg (line 147)
    [Fact]
    public void Extended_HeaderSizeLg()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("px-5", cut.Markup);
        Assert.Contains("py-4", cut.Markup);
        Assert.Contains("min-h-[48px]", cut.Markup);
        Assert.Contains("text-base", cut.Markup);
    }

    // HeaderClass default Md (line 148)
    [Fact]
    public void Extended_HeaderSizeDefault()
    {
        var cut = RenderCollapse();
        Assert.Contains("px-4", cut.Markup);
        Assert.Contains("py-3", cut.Markup);
        Assert.Contains("min-h-[44px]", cut.Markup);
    }

    // HeaderClass Ghost hover (line 142)
    [Fact]
    public void Extended_HeaderVariantGhost_HoverTextPrimary()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Variant, XFCollapseVariant.Ghost));
        Assert.Contains("hover:text-primary", cut.Markup);
    }

    // HeaderClass non-Ghost hover (line 143)
    [Fact]
    public void Extended_HeaderVariantNonGhost_HoverBg()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Variant, XFCollapseVariant.Surface));
        Assert.Contains("hover:bg-primary/[0.04]", cut.Markup);
    }

    // HeaderClass disabled (line 140)
    [Fact]
    public void Extended_HeaderDisabled_RendersDisabledStyles()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Disabled, true));
        Assert.Contains("disabled:cursor-not-allowed", cut.Markup);
        Assert.Contains("disabled:text-text-muted", cut.Markup);
        Assert.Contains("disabled:opacity-70", cut.Markup);
    }

    // StartIconClass Sm (line 153)
    [Fact]
    public void Extended_StartIconSizeSm()
    {
        var cut = RenderCollapse(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("h-7", cut.Markup);
        Assert.Contains("w-7", cut.Markup);
    }

    // StartIconClass Lg (line 154)
    [Fact]
    public void Extended_StartIconSizeLg()
    {
        var cut = RenderCollapse(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("h-9", cut.Markup);
        Assert.Contains("w-9", cut.Markup);
    }

    // StartIconClass default Md (line 155)
    [Fact]
    public void Extended_StartIconSizeDefault()
    {
        var cut = RenderCollapse(p => p.Add(x => x.StartIcon, builder => builder.AddContent(0, "icon")));
        Assert.Contains("h-8", cut.Markup);
        Assert.Contains("w-8", cut.Markup);
    }

    // IconContainerClass expanded (line 161-162)
    [Fact]
    public void Extended_IconContainerExpanded()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Expanded, true));
        Assert.Contains("bg-primary/[0.09]", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    // IconContainerClass collapsed (line 163)
    [Fact]
    public void Extended_IconContainerCollapsed()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Expanded, false));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // IconContainerClass Size Lg (line 164)
    [Fact]
    public void Extended_IconContainerSizeLg()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-8", cut.Markup);
        Assert.Contains("w-8", cut.Markup);
    }

    // ChevronIconClass expanded (line 169)
    [Fact]
    public void Extended_ChevronExpanded_Rotated()
    {
        var cut = RenderCollapse(p =>
        {
            p.Add(x => x.Expanded, true);
            p.Add(x => x.IconMode, XFCollapseIconMode.Chevron);
        });
        Assert.Contains("rotate-180", cut.Markup);
    }

    // ChevronIconClass collapsed (line 169)
    [Fact]
    public void Extended_ChevronCollapsed_NotRotated()
    {
        var cut = RenderCollapse(p =>
        {
            p.Add(x => x.Expanded, false);
            p.Add(x => x.IconMode, XFCollapseIconMode.Chevron);
        });
        Assert.Contains("rotate-0", cut.Markup);
    }

    // PanelOuterClass expanded (line 181)
    [Fact]
    public void Extended_PanelExpanded_GridRows1Fr()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Expanded, true));
        Assert.Contains("grid-rows-[1fr]", cut.Markup);
    }

    // PanelOuterClass collapsed (line 181)
    [Fact]
    public void Extended_PanelCollapsed_GridRows0Fr()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Expanded, false));
        Assert.Contains("grid-rows-[0fr]", cut.Markup);
    }

    // GetAnimationOuterClass None (line 192) — no grid-template-rows transition
    [Fact]
    public void Extended_AnimationNone_NoGridTransition()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Animation, XFCollapseAnimation.None));
        // None should not have transition-[grid-template-rows]
        Assert.DoesNotContain("transition-[grid-template-rows]", cut.Markup);
    }

    // GetAnimationOuterClass Height (line 193)
    [Fact]
    public void Extended_AnimationHeight_Transition()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Animation, XFCollapseAnimation.Height));
        Assert.Contains("transition-[grid-template-rows]", cut.Markup);
    }

    // GetAnimationOuterClass SlideFade (line 194)
    [Fact]
    public void Extended_AnimationSlideFade_Transition()
    {
        var cut = RenderCollapse(p => p.Add(x => x.Animation, XFCollapseAnimation.SlideFade));
        Assert.Contains("transition-[grid-template-rows]", cut.Markup);
    }

    // GetAnimationOuterClass HeightFade (default) (line 195)
    [Fact]
    public void Extended_AnimationHeightFade_Transition()
    {
        var cut = RenderCollapse();
        Assert.Contains("transition-[grid-template-rows]", cut.Markup);
    }

    // PlusIconClass (line 171)
    [Fact]
    public void Extended_PlusIconMode_RendersPlusIcon()
    {
        var cut = RenderCollapse(p => p.Add(x => x.IconMode, XFCollapseIconMode.PlusMinus));
        Assert.Contains("h-3.5", cut.Markup);
    }

    // PlusVerticalClass expanded (line 176)
    [Fact]
    public void Extended_PlusVerticalExpanded_ScaleY0()
    {
        var cut = RenderCollapse(p =>
        {
            p.Add(x => x.Expanded, true);
            p.Add(x => x.IconMode, XFCollapseIconMode.PlusMinus);
        });
        Assert.Contains("scale-y-0", cut.Markup);
    }

    // PlusVerticalClass collapsed (line 176)
    [Fact]
    public void Extended_PlusVerticalCollapsed_ScaleY100()
    {
        var cut = RenderCollapse(p =>
        {
            p.Add(x => x.Expanded, false);
            p.Add(x => x.IconMode, XFCollapseIconMode.PlusMinus);
        });
        Assert.Contains("scale-y-100", cut.Markup);
    }

    // BaseClass included (line 125)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderCollapse();
        Assert.Contains("xf-collapse", cut.Markup);
    }
}
