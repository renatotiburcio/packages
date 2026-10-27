using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Divider;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFDividerExtendedTest : BunitContext
{
    public XFDividerExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFDivider> RenderDivider(
        Action<ComponentParameterCollectionBuilder<XFDivider>>? configure = null)
    {
        return Render<XFDivider>(p => configure?.Invoke(p));
    }

    // Orientation Vertical (line 28-30)
    [Fact]
    public void Extended_OrientationVertical_RendersInlineBlock()
    {
        var cut = RenderDivider(p => p.Add(x => x.Orientation, XFOrientation.Vertical));
        Assert.Contains("inline-block", cut.Markup);
        Assert.Contains("self-stretch", cut.Markup);
    }

    // Orientation Horizontal default (line 38)
    [Fact]
    public void Extended_OrientationHorizontal_RendersWFull()
    {
        var cut = RenderDivider();
        Assert.Contains("w-full", cut.Markup);
    }

    // HasLabel renders flex (line 33-35)
    [Fact]
    public void Extended_HasLabel_RendersFlex()
    {
        var cut = RenderDivider(p => p.Add(x => x.Label, "Label"));
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("items-center", cut.Markup);
    }

    // No label renders w-full (line 38)
    [Fact]
    public void Extended_NoLabel_RendersWFull()
    {
        var cut = RenderDivider();
        Assert.Contains("w-full", cut.Markup);
    }

    // HorizontalSpacing None (line 43)
    [Fact]
    public void Extended_HorizontalSpacingNone()
    {
        var cut = RenderDivider(p => p.Add(x => x.Spacing, XFDividerSpacing.None));
        Assert.Contains("my-0", cut.Markup);
    }

    // HorizontalSpacing Sm (line 44)
    [Fact]
    public void Extended_HorizontalSpacingSm()
    {
        var cut = RenderDivider(p => p.Add(x => x.Spacing, XFDividerSpacing.Sm));
        Assert.Contains("my-2", cut.Markup);
    }

    // HorizontalSpacing Lg (line 45)
    [Fact]
    public void Extended_HorizontalSpacingLg()
    {
        var cut = RenderDivider(p => p.Add(x => x.Spacing, XFDividerSpacing.Lg));
        Assert.Contains("my-6", cut.Markup);
    }

    // HorizontalSpacing Xl (line 46)
    [Fact]
    public void Extended_HorizontalSpacingXl()
    {
        var cut = RenderDivider(p => p.Add(x => x.Spacing, XFDividerSpacing.Xl));
        Assert.Contains("my-8", cut.Markup);
    }

    // HorizontalSpacing default Md (line 47)
    [Fact]
    public void Extended_HorizontalSpacingDefault()
    {
        var cut = RenderDivider();
        Assert.Contains("my-4", cut.Markup);
    }

    // VerticalSpacing Sm (line 53)
    [Fact]
    public void Extended_VerticalSpacingSm()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Spacing, XFDividerSpacing.Sm);
        });
        Assert.Contains("mx-2", cut.Markup);
    }

    // VerticalSpacing Xl (line 55)
    [Fact]
    public void Extended_VerticalSpacingXl()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Spacing, XFDividerSpacing.Xl);
        });
        Assert.Contains("mx-8", cut.Markup);
    }

    // HorizontalThickness Thin (line 67)
    [Fact]
    public void Extended_HorizontalThicknessThin()
    {
        var cut = RenderDivider(p => p.Add(x => x.Thickness, XFDividerThickness.Thin));
        Assert.Contains("h-px", cut.Markup);
    }

    // HorizontalThickness Md (line 65)
    [Fact]
    public void Extended_HorizontalThicknessMd()
    {
        var cut = RenderDivider(p => p.Add(x => x.Thickness, XFDividerThickness.Md));
        Assert.Contains("h-0.5", cut.Markup);
    }

    // HorizontalThickness Lg (line 66)
    [Fact]
    public void Extended_HorizontalThicknessLg()
    {
        var cut = RenderDivider(p => p.Add(x => x.Thickness, XFDividerThickness.Lg));
        Assert.Contains("h-1", cut.Markup);
    }

    // VerticalThickness Thin (line 74)
    [Fact]
    public void Extended_VerticalThicknessThin()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Thickness, XFDividerThickness.Thin);
        });
        Assert.Contains("w-px", cut.Markup);
    }

    // VerticalThickness Md (line 72)
    [Fact]
    public void Extended_VerticalThicknessMd()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Thickness, XFDividerThickness.Md);
        });
        Assert.Contains("w-0.5", cut.Markup);
    }

    // Dashed renders border-dashed (line 77-79)
    [Fact]
    public void Extended_Dashed_RendersBorderDashed()
    {
        var cut = RenderDivider(p => p.Add(x => x.Dashed, true));
        Assert.Contains("border-dashed", cut.Markup);
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // Dashed false no border-dashed (line 77)
    [Fact]
    public void Extended_DashedFalse_NoBorderDashed()
    {
        var cut = RenderDivider(p => p.Add(x => x.Dashed, false));
        Assert.DoesNotContain("border-dashed", cut.Markup);
    }

    // Dashed Vertical (line 78)
    [Fact]
    public void Extended_DashedVertical()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Dashed, true);
        });
        Assert.Contains("border-dashed", cut.Markup);
    }

    // BackgroundTone Primary (line 121)
    [Fact]
    public void Extended_TonePrimary()
    {
        var cut = RenderDivider(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.Contains("bg-[var(--color-primary)]", cut.Markup);
    }

    // BackgroundTone Success (line 123)
    [Fact]
    public void Extended_ToneSuccess()
    {
        var cut = RenderDivider(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("bg-[var(--color-success)]", cut.Markup);
    }

    // BackgroundTone Danger (line 125)
    [Fact]
    public void Extended_ToneDanger()
    {
        var cut = RenderDivider(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("bg-[var(--color-danger)]", cut.Markup);
    }

    // BackgroundTone Neutral (line 126)
    [Fact]
    public void Extended_ToneNeutral()
    {
        var cut = RenderDivider(p => p.Add(x => x.Tone, XFSemanticColor.Neutral));
        Assert.Contains("bg-border", cut.Markup);
    }

    // BorderTone Primary dashed (line 131)
    [Fact]
    public void Extended_BorderTonePrimary()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Dashed, true);
            p.Add(x => x.Tone, XFSemanticColor.Primary);
        });
        Assert.Contains("border-[var(--color-primary)]", cut.Markup);
    }

    // Align Start (line 107)
    [Fact]
    public void Extended_AlignStart()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Label, "Label");
            p.Add(x => x.Align, XFDividerAlign.Start);
        });
        Assert.Contains("w-8", cut.Markup);
    }

    // Align End (line 114)
    [Fact]
    public void Extended_AlignEnd()
    {
        var cut = RenderDivider(p =>
        {
            p.Add(x => x.Label, "Label");
            p.Add(x => x.Align, XFDividerAlign.End);
        });
        Assert.Contains("w-8", cut.Markup);
    }

    // Align Center default (line 109)
    [Fact]
    public void Extended_AlignCenter()
    {
        var cut = RenderDivider(p => p.Add(x => x.Label, "Label"));
        Assert.Contains("flex-1", cut.Markup);
    }

    // LabelClass (line 103)
    [Fact]
    public void Extended_LabelClass_Rendered()
    {
        var cut = RenderDivider(p => p.Add(x => x.Label, "Label"));
        Assert.Contains("text-xs", cut.Markup);
        Assert.Contains("font-semibold", cut.Markup);
        Assert.Contains("uppercase", cut.Markup);
    }

    // Visible false renders nothing (line 18)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderDivider(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // BaseClass included (line 27)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderDivider();
        Assert.Contains("xf-divider", cut.Markup);
    }
}
