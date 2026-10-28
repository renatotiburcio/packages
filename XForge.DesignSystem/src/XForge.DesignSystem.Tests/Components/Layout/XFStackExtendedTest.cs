using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Layout;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Layout;

public class XFStackExtendedTest : XForgeTestContext
{
    public XFStackExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFStack> RenderStack(
        Action<ComponentParameterCollectionBuilder<XFStack>>? configure = null)
    {
        return Render<XFStack>(p =>
        {
            p.AddChildContent("content");
            configure?.Invoke(p);
        });
    }

    // GetDisplayClass Inline (line 49)
    [Fact]
    public void Extended_Inline_RendersInlineFlex()
    {
        var cut = RenderStack(p => p.Add(x => x.Inline, true));
        Assert.Contains("inline-flex", cut.Markup);
    }

    // GetDisplayClass default block (line 49)
    [Fact]
    public void Extended_DisplayDefault_RendersFlex()
    {
        var cut = RenderStack();
        Assert.Contains("flex", cut.Markup);
        Assert.DoesNotContain("inline-flex", cut.Markup);
    }

    // GetDirectionClass Vertical (line 57)
    [Fact]
    public void Extended_OrientationVertical_RendersFlexCol()
    {
        var cut = RenderStack(p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("flex-col", cut.Markup);
    }

    // GetDirectionClass Vertical+Reverse (line 56)
    [Fact]
    public void Extended_OrientationVerticalReverse_RendersFlexColReverse()
    {
        var cut = RenderStack(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Reverse, true);
        });
        Assert.Contains("flex-col-reverse", cut.Markup);
    }

    // GetDirectionClass Horizontal+Reverse (line 58)
    [Fact]
    public void Extended_OrientationHorizontalReverse_RendersFlexRowReverse()
    {
        var cut = RenderStack(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Horizontal);
            p.Add(x => x.Reverse, true);
        });
        Assert.Contains("flex-row-reverse", cut.Markup);
    }

    // GetDirectionClass default Horizontal (line 59)
    [Fact]
    public void Extended_OrientationDefault_RendersFlexRow()
    {
        var cut = RenderStack();
        Assert.Contains("flex-row", cut.Markup);
    }

    // GetWrapClass Wrap (line 67)
    [Fact]
    public void Extended_Wrap_RendersFlexWrap()
    {
        var cut = RenderStack(p => p.Add(x => x.Wrap, XFWrap.Wrap));
        Assert.Contains("flex-wrap", cut.Markup);
    }

    // GetWrapClass WrapReverse (line 68)
    [Fact]
    public void Extended_WrapReverse_RendersFlexWrapReverse()
    {
        var cut = RenderStack(p => p.Add(x => x.Wrap, XFWrap.WrapReverse));
        Assert.Contains("flex-wrap-reverse", cut.Markup);
    }

    // GetWrapClass NoWrap (line 69)
    [Fact]
    public void Extended_NoWrap_RendersFlexNowrap()
    {
        var cut = RenderStack(p => p.Add(x => x.Wrap, XFWrap.NoWrap));
        Assert.Contains("flex-nowrap", cut.Markup);
    }

    // GetAlignClass Start (line 77)
    [Fact]
    public void Extended_AlignItemsStart_RendersItemsStart()
    {
        var cut = RenderStack(p => p.Add(x => x.AlignItems, XFAlignItems.Start));
        Assert.Contains("items-start", cut.Markup);
    }

    // GetAlignClass Center (line 78)
    [Fact]
    public void Extended_AlignItemsCenter_RendersItemsCenter()
    {
        var cut = RenderStack(p => p.Add(x => x.AlignItems, XFAlignItems.Center));
        Assert.Contains("items-center", cut.Markup);
    }

    // GetAlignClass End (line 79)
    [Fact]
    public void Extended_AlignItemsEnd_RendersItemsEnd()
    {
        var cut = RenderStack(p => p.Add(x => x.AlignItems, XFAlignItems.End));
        Assert.Contains("items-end", cut.Markup);
    }

    // GetAlignClass Stretch (line 80)
    [Fact]
    public void Extended_AlignItemsStretch_RendersItemsStretch()
    {
        var cut = RenderStack(p => p.Add(x => x.AlignItems, XFAlignItems.Stretch));
        Assert.Contains("items-stretch", cut.Markup);
    }

    // GetAlignClass Baseline (line 81)
    [Fact]
    public void Extended_AlignItemsBaseline_RendersItemsBaseline()
    {
        var cut = RenderStack(p => p.Add(x => x.AlignItems, XFAlignItems.Baseline));
        Assert.Contains("items-baseline", cut.Markup);
    }

    // GetAlignClass Normal (line 82)
    [Fact]
    public void Extended_AlignItemsNormal_NoAlignClass()
    {
        var cut = RenderStack(p => p.Add(x => x.AlignItems, XFAlignItems.Normal));
        Assert.DoesNotContain("items-", cut.Markup);
    }

    // GetJustifyClass Start (line 90)
    [Fact]
    public void Extended_JustifyStart_RendersJustifyStart()
    {
        var cut = RenderStack(p => p.Add(x => x.JustifyContent, XFJustifyContent.Start));
        Assert.Contains("justify-start", cut.Markup);
    }

    // GetJustifyClass End (line 91)
    [Fact]
    public void Extended_JustifyEnd_RendersJustifyEnd()
    {
        var cut = RenderStack(p => p.Add(x => x.JustifyContent, XFJustifyContent.End));
        Assert.Contains("justify-end", cut.Markup);
    }

    // GetJustifyClass Center (line 92)
    [Fact]
    public void Extended_JustifyCenter_RendersJustifyCenter()
    {
        var cut = RenderStack(p => p.Add(x => x.JustifyContent, XFJustifyContent.Center));
        Assert.Contains("justify-center", cut.Markup);
    }

    // GetJustifyClass SpaceBetween (line 93)
    [Fact]
    public void Extended_JustifySpaceBetween_RendersJustifyBetween()
    {
        var cut = RenderStack(p => p.Add(x => x.JustifyContent, XFJustifyContent.SpaceBetween));
        Assert.Contains("justify-between", cut.Markup);
    }

    // GetJustifyClass SpaceAround (line 94)
    [Fact]
    public void Extended_JustifySpaceAround_RendersJustifyAround()
    {
        var cut = RenderStack(p => p.Add(x => x.JustifyContent, XFJustifyContent.SpaceAround));
        Assert.Contains("justify-around", cut.Markup);
    }

    // GetJustifyClass SpaceEvenly (line 95)
    [Fact]
    public void Extended_JustifySpaceEvenly_RendersJustifyEvenly()
    {
        var cut = RenderStack(p => p.Add(x => x.JustifyContent, XFJustifyContent.SpaceEvenly));
        Assert.Contains("justify-evenly", cut.Markup);
    }

    // GetJustifyClass Normal (line 96)
    [Fact]
    public void Extended_JustifyNormal_NoJustifyClass()
    {
        var cut = RenderStack(p => p.Add(x => x.JustifyContent, XFJustifyContent.Normal));
        Assert.DoesNotContain("justify-", cut.Markup);
    }

    // GetWidthClass FullWidth (line 101)
    [Fact]
    public void Extended_FullWidth_RendersWFull()
    {
        var cut = RenderStack(p => p.Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    // GetWidthClass default (line 102)
    [Fact]
    public void Extended_FullWidthFalse_NoWFull()
    {
        var cut = RenderStack(p => p.Add(x => x.FullWidth, false));
        Assert.DoesNotContain("w-full", cut.Markup);
    }

    // GetGapClass with token value "4" (line 116)
    [Fact]
    public void Extended_GapToken4_RendersGap4()
    {
        var cut = RenderStack(p => p.Add(x => x.Gap, (XFSizeToken)"4"));
        Assert.Contains("gap-4", cut.Markup);
    }

    // GetGapClass with token value "8" (line 119)
    [Fact]
    public void Extended_GapToken8_RendersGap8()
    {
        var cut = RenderStack(p => p.Add(x => x.Gap, (XFSizeToken)"8"));
        Assert.Contains("gap-8", cut.Markup);
    }

    // GetGapClass with token value "12" (line 121)
    [Fact]
    public void Extended_GapToken12_RendersGap12()
    {
        var cut = RenderStack(p => p.Add(x => x.Gap, (XFSizeToken)"12"));
        Assert.Contains("gap-12", cut.Markup);
    }

    // GetGapClass with no token (line 107)
    [Fact]
    public void Extended_GapNull_NoGapClass()
    {
        var cut = RenderStack(p => p.Add(x => x.Gap, (XFSizeToken?)null));
        Assert.DoesNotContain("gap-", cut.Markup);
    }

    // GetGapClass with custom value renders inline style (line 35-36)
    [Fact]
    public void Extended_GapCustomValue_RendersInlineStyle()
    {
        var cut = RenderStack(p => p.Add(x => x.Gap, (XFSizeToken)"24"));
        Assert.Contains("gap: 24px", cut.Markup);
    }

    // RowGap inline style (line 38-39)
    [Fact]
    public void Extended_RowGap_RendersInlineStyle()
    {
        var cut = RenderStack(p => p.Add(x => x.RowGap, (XFSizeToken)"16"));
        Assert.Contains("row-gap: 16px", cut.Markup);
    }

    // ColumnGap inline style (line 41-42)
    [Fact]
    public void Extended_ColumnGap_RendersInlineStyle()
    {
        var cut = RenderStack(p => p.Add(x => x.ColumnGap, (XFSizeToken)"10"));
        Assert.Contains("column-gap: 10px", cut.Markup);
    }

    // BaseClass included (line 26)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderStack();
        Assert.Contains("xf-stack", cut.Markup);
    }
}
