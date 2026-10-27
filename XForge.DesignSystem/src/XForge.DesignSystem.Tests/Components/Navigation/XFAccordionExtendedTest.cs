using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Navigation.Accordion;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public class XFAccordionExtendedTest : BunitContext
{
    public XFAccordionExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFAccordion> RenderAccordion(
        Action<ComponentParameterCollectionBuilder<XFAccordion>>? configure = null)
    {
        return Render<XFAccordion>(p =>
        {
            p.AddChildContent("content");
            configure?.Invoke(p);
        });
    }

    // VariantClass Surface (default) (line 135)
    [Fact]
    public void Extended_VariantSurface_RendersSpaceY2()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Variant, XFAccordionVariant.Surface));
        Assert.Contains("space-y-2", cut.Markup);
    }

    // VariantClass Outline (line 132)
    [Fact]
    public void Extended_VariantOutline_RendersSpaceY2()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Variant, XFAccordionVariant.Outline));
        Assert.Contains("space-y-2", cut.Markup);
    }

    // VariantClass Ghost (line 133)
    [Fact]
    public void Extended_VariantGhost_RendersSpaceY1()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Variant, XFAccordionVariant.Ghost));
        Assert.Contains("space-y-1", cut.Markup);
    }

    // VariantClass Soft (line 134)
    [Fact]
    public void Extended_VariantSoft_RendersSpaceY2()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Variant, XFAccordionVariant.Soft));
        Assert.Contains("space-y-2", cut.Markup);
    }

    // SizeClass Sm (line 140)
    [Fact]
    public void Extended_SizeSm_RendersTextSm()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("text-sm", cut.Markup);
    }

    // SizeClass Lg (line 141)
    [Fact]
    public void Extended_SizeLg_RendersTextBase()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    // SizeClass default Md (line 142)
    [Fact]
    public void Extended_SizeDefault_RendersTextSm()
    {
        var cut = RenderAccordion();
        Assert.Contains("text-sm", cut.Markup);
    }

    // GetComponentClass includes BaseClass (line 31)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderAccordion();
        Assert.Contains("xf-accordion", cut.Markup);
    }

    // Multiple parameter (line 14)
    [Fact]
    public void Extended_MultipleTrue_ParameterSet()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Multiple, true));
        Assert.True(cut.Instance.Multiple);
    }

    // Collapsible parameter (line 15)
    [Fact]
    public void Extended_CollapsibleFalse_ParameterSet()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Collapsible, false));
        Assert.False(cut.Instance.Collapsible);
    }

    // Animation parameter (line 18)
    [Fact]
    public void Extended_AnimationNone_ParameterSet()
    {
        var cut = RenderAccordion(p => p.Add(x => x.Animation, XFAccordionAnimation.None));
        Assert.Equal(XFAccordionAnimation.None, cut.Instance.Animation);
    }

    // IconMode parameter (line 19)
    [Fact]
    public void Extended_IconModePlusMinus_ParameterSet()
    {
        var cut = RenderAccordion(p => p.Add(x => x.IconMode, XFAccordionIconMode.PlusMinus));
        Assert.Equal(XFAccordionIconMode.PlusMinus, cut.Instance.IconMode);
    }

    // StaggerContent parameter (line 20)
    [Fact]
    public void Extended_StaggerContentTrue_ParameterSet()
    {
        var cut = RenderAccordion(p => p.Add(x => x.StaggerContent, true));
        Assert.True(cut.Instance.StaggerContent);
    }
}
