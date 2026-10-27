using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Chip;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFChipListExtendedTest : BunitContext
{
    public XFChipListExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFChipList> RenderChipList(
        Action<ComponentParameterCollectionBuilder<XFChipList>>? configure = null)
    {
        return Render<XFChipList>(p =>
        {
            p.AddChildContent("content");
            configure?.Invoke(p);
        });
    }

    // RootClass includes BaseClass (line 24-27)
    [Fact]
    public void Extended_RootClass_IncludesBaseClass()
    {
        var cut = RenderChipList();
        Assert.Contains("xf-chip-list", cut.Markup);
    }

    // RootClass includes flex layout (line 25-26)
    [Fact]
    public void Extended_RootClass_RendersFlexLayout()
    {
        var cut = RenderChipList();
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("flex-col", cut.Markup);
    }

    // LayoutClass Wrap true (line 30)
    [Fact]
    public void Extended_WrapTrue_RendersFlexWrap()
    {
        var cut = RenderChipList(p => p.Add(x => x.Wrap, true));
        Assert.Contains("flex-wrap", cut.Markup);
    }

    // LayoutClass Wrap false (line 30)
    [Fact]
    public void Extended_WrapFalse_RendersOverflowAuto()
    {
        var cut = RenderChipList(p => p.Add(x => x.Wrap, false));
        Assert.Contains("overflow-x-auto", cut.Markup);
        Assert.DoesNotContain("flex-wrap", cut.Markup);
    }

    // LayoutClass default Wrap=true (line 30)
    [Fact]
    public void Extended_WrapDefault_RendersFlexWrap()
    {
        var cut = RenderChipList();
        Assert.Contains("flex-wrap", cut.Markup);
    }

    // LayoutClass includes items-center and gap (line 29-30)
    [Fact]
    public void Extended_LayoutClass_RendersItemsCenter()
    {
        var cut = RenderChipList();
        Assert.Contains("items-center", cut.Markup);
        Assert.Contains("gap-1.5", cut.Markup);
    }

    // Selectable parameter (line 18)
    [Fact]
    public void Extended_SelectableTrue_ParameterSet()
    {
        var cut = RenderChipList(p => p.Add(x => x.Selectable, true));
        Assert.True(cut.Instance.Selectable);
    }

    // ChipVariant parameter (line 20)
    [Fact]
    public void Extended_ChipVariantOutline_ParameterSet()
    {
        var cut = RenderChipList(p => p.Add(x => x.ChipVariant, XFChipVariant.Outline));
        Assert.Equal(XFChipVariant.Outline, cut.Instance.ChipVariant);
    }

    // ChipSize parameter (line 21)
    [Fact]
    public void Extended_ChipSizeSm_ParameterSet()
    {
        var cut = RenderChipList(p => p.Add(x => x.ChipSize, XFSize.Sm));
        Assert.Equal(XFSize.Sm, cut.Instance.ChipSize);
    }

    // ChipTone parameter (line 22)
    [Fact]
    public void Extended_ChipTonePrimary_ParameterSet()
    {
        var cut = RenderChipList(p => p.Add(x => x.ChipTone, XFSemanticColor.Primary));
        Assert.Equal(XFSemanticColor.Primary, cut.Instance.ChipTone);
    }
}
