using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Divider;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Layout;

/// <summary>
/// Extended tests for XFDivider covering variants, orientations, tones, sizes,
/// spacing, thickness, inset, child content, visibility, and CanCreate.
/// Complements XFDividerComponentTest — no test name overlap.
/// </summary>
public class XFDividerExtendedTest : BunitContext
{
    // ── Base class / base CSS ──────────────────────────────────────────

    [Fact]
    public void Extended_Render_Default_HasBaseClass()
    {
        var cut = Render<XFDivider>();

        Assert.Contains("xf-divider", cut.Markup);
        Assert.Contains("separator", cut.Markup);
    }

    [Fact]
    public void Extended_Render_Default_IsHorizontal()
    {
        var cut = Render<XFDivider>();

        var divider = cut.Find(".xf-divider");
        Assert.Equal("horizontal", divider.GetAttribute("aria-orientation"));
    }

    // ── Orientations ───────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_VerticalOrientation_HasVerticalAria()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Orientation, XFOrientation.Vertical));

        var divider = cut.Find(".xf-divider");
        Assert.Equal("vertical", divider.GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Extended_Render_VerticalOrientation_HasWidthPx()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Orientation, XFOrientation.Vertical));

        var divider = cut.Find(".xf-divider");
        Assert.Contains("w-px", divider.GetAttribute("class"));
    }

    [Fact]
    public void Extended_Render_HorizontalOrientation_HasHeightPx()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Orientation, XFOrientation.Horizontal));

        var divider = cut.Find(".xf-divider");
        Assert.Contains("h-px", divider.GetAttribute("class"));
    }

    // ── Dashed style ───────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_DashedHorizontal_HasDashedBorder()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Orientation, XFOrientation.Horizontal));

        Assert.Contains("border-dashed", cut.Markup);
        Assert.Contains("bg-transparent", cut.Markup);
    }

    [Fact]
    public void Extended_Render_DashedVertical_HasDashedBorder()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Orientation, XFOrientation.Vertical));

        Assert.Contains("border-dashed", cut.Markup);
    }

    // ── Tones ──────────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_TonePrimary_HasPrimaryColor()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));

        Assert.Contains("var(--color-primary)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneSecondary_HasSecondaryColor()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));

        Assert.Contains("var(--color-secondary)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneSuccess_HasSuccessColor()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Tone, XFSemanticColor.Success));

        Assert.Contains("var(--color-success)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneWarning_HasWarningColor()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Tone, XFSemanticColor.Warning));

        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneDanger_HasDangerColor()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Tone, XFSemanticColor.Danger));

        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneNeutral_HasBorderColor()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Tone, XFSemanticColor.Neutral));

        Assert.Contains("bg-border", cut.Markup);
    }

    // ── Spacing ────────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_SpacingNone_HasMy0()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Spacing, XFDividerSpacing.None));

        Assert.Contains("my-0", cut.Markup);
    }

    [Fact]
    public void Extended_Render_SpacingSm_HasMy2()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Spacing, XFDividerSpacing.Sm));

        Assert.Contains("my-2", cut.Markup);
    }

    [Fact]
    public void Extended_Render_SpacingLg_HasMy6()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Spacing, XFDividerSpacing.Lg));

        Assert.Contains("my-6", cut.Markup);
    }

    [Fact]
    public void Extended_Render_SpacingXl_HasMy8()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Spacing, XFDividerSpacing.Xl));

        Assert.Contains("my-8", cut.Markup);
    }

    // ── Thickness ──────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_ThicknessMd_HasHalfPxHeight()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Thickness, XFDividerThickness.Md));

        Assert.Contains("h-0.5", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ThicknessLg_HasHeight1()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Thickness, XFDividerThickness.Lg));

        Assert.Contains("h-1", cut.Markup);
    }

    // ── Label / Child content ──────────────────────────────────────────

    [Fact]
    public void Extended_Render_WithLabel_RendersLabelText()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Label, "Section"));

        Assert.Contains("Section", cut.Markup);
    }

    [Fact]
    public void Extended_Render_WithLabel_HasFlexLayout()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Label, "Divider"));

        Assert.Contains("flex w-full items-center", cut.Markup);
    }

    [Fact]
    public void Extended_Render_WithChildContent_RendersChildContent()
    {
        var cut = Render<XFDivider>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Custom content"))));

        Assert.Contains("Custom content", cut.Markup);
    }

    [Fact]
    public void Extended_Render_LabelAlignStart_HasShortBeforeLine()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Label, "Start")
            .Add(x => x.Align, XFDividerAlign.Start));

        Assert.Contains("w-8", cut.Markup);
    }

    [Fact]
    public void Extended_Render_LabelAlignEnd_HasShortAfterLine()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Label, "End")
            .Add(x => x.Align, XFDividerAlign.End));

        Assert.Contains("w-8", cut.Markup);
    }

    // ── Visibility ─────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_VisibleFalse_DoesNotRender()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Visible, false));

        Assert.DoesNotContain("xf-divider", cut.Markup);
    }

    [Fact]
    public void Extended_Render_VisibleTrue_Renders()
    {
        var cut = Render<XFDivider>(p => p.Add(x => x.Visible, true));

        Assert.Contains("xf-divider", cut.Markup);
    }

    // ── Dashed + Tone combination ──────────────────────────────────────

    [Fact]
    public void Extended_Render_DashedWithTonePrimary_HasPrimaryBorder()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Tone, XFSemanticColor.Primary));

        Assert.Contains("border-[var(--color-primary)]", cut.Markup);
    }

    // ── Vertical spacing (VerticalSpacingClass branches) ────────────────

    [Fact]
    public void Extended_Render_VerticalSpacingNone_HasMx0()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Spacing, XFDividerSpacing.None));

        Assert.Contains("mx-0", cut.Markup);
    }

    [Fact]
    public void Extended_Render_VerticalSpacingSm_HasMx2()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Spacing, XFDividerSpacing.Sm));

        Assert.Contains("mx-2", cut.Markup);
    }

    [Fact]
    public void Extended_Render_VerticalSpacingLg_HasMx6()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Spacing, XFDividerSpacing.Lg));

        Assert.Contains("mx-6", cut.Markup);
    }

    [Fact]
    public void Extended_Render_VerticalSpacingXl_HasMx8()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Spacing, XFDividerSpacing.Xl));

        Assert.Contains("mx-8", cut.Markup);
    }

    // ── Vertical thickness (VerticalThicknessClass branches) ────────────

    [Fact]
    public void Extended_Render_VerticalThicknessMd_HasW05()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Thickness, XFDividerThickness.Md));

        Assert.Contains("w-0.5", cut.Markup);
    }

    [Fact]
    public void Extended_Render_VerticalThicknessLg_HasW1()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Thickness, XFDividerThickness.Lg));

        Assert.Contains("w-1", cut.Markup);
    }

    // ── Dashed + thickness (HorizontalBorderThicknessClass branches) ────

    [Fact]
    public void Extended_Render_DashedThicknessMd_HasBorderT2()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Thickness, XFDividerThickness.Md));

        Assert.Contains("border-t-2", cut.Markup);
    }

    [Fact]
    public void Extended_Render_DashedThicknessLg_HasBorderT4()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Thickness, XFDividerThickness.Lg));

        Assert.Contains("border-t-4", cut.Markup);
    }

    // ── Dashed + vertical + thickness (VerticalBorderThicknessClass) ────

    [Fact]
    public void Extended_Render_DashedVerticalThicknessMd_HasBorderL2()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Thickness, XFDividerThickness.Md));

        Assert.Contains("border-l-2", cut.Markup);
    }

    [Fact]
    public void Extended_Render_DashedVerticalThicknessLg_HasBorderL4()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Orientation, XFOrientation.Vertical)
            .Add(x => x.Thickness, XFDividerThickness.Lg));

        Assert.Contains("border-l-4", cut.Markup);
    }

    // ── Dashed + tone (BorderToneClass branches) ────────────────────────

    [Fact]
    public void Extended_Render_DashedToneSecondary_HasSecondaryBorder()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Tone, XFSemanticColor.Secondary));

        Assert.Contains("border-[var(--color-secondary)]", cut.Markup);
    }

    [Fact]
    public void Extended_Render_DashedToneSuccess_HasSuccessBorder()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Tone, XFSemanticColor.Success));

        Assert.Contains("border-[var(--color-success)]", cut.Markup);
    }

    [Fact]
    public void Extended_Render_DashedToneWarning_HasWarningBorder()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Tone, XFSemanticColor.Warning));

        Assert.Contains("border-[var(--color-warning)]", cut.Markup);
    }

    [Fact]
    public void Extended_Render_DashedToneDanger_HasDangerBorder()
    {
        var cut = Render<XFDivider>(p => p
            .Add(x => x.Dashed, true)
            .Add(x => x.Tone, XFSemanticColor.Danger));

        Assert.Contains("border-[var(--color-danger)]", cut.Markup);
    }

    // ── CanCreate ──────────────────────────────────────────────────────

    [Fact]
    public void Extended_XFDivider_CanCreate()
    {
        var divider = new XFDivider();
        Assert.NotNull(divider);
    }

    // ── Enum coverage ──────────────────────────────────────────────────

    [Fact]
    public void Extended_XFDividerAlign_EnumHasAllValues()
    {
        var values = Enum.GetValues<XFDividerAlign>();
        Assert.Equal(3, values.Length);
        Assert.Contains(XFDividerAlign.Start, values);
        Assert.Contains(XFDividerAlign.Center, values);
        Assert.Contains(XFDividerAlign.End, values);
    }

    [Fact]
    public void Extended_XFDividerSpacing_EnumHasAllValues()
    {
        var values = Enum.GetValues<XFDividerSpacing>();
        Assert.Equal(5, values.Length);
        Assert.Contains(XFDividerSpacing.None, values);
        Assert.Contains(XFDividerSpacing.Sm, values);
        Assert.Contains(XFDividerSpacing.Md, values);
        Assert.Contains(XFDividerSpacing.Lg, values);
        Assert.Contains(XFDividerSpacing.Xl, values);
    }

    [Fact]
    public void Extended_XFDividerThickness_EnumHasAllValues()
    {
        var values = Enum.GetValues<XFDividerThickness>();
        Assert.Equal(3, values.Length);
        Assert.Contains(XFDividerThickness.Thin, values);
        Assert.Contains(XFDividerThickness.Md, values);
        Assert.Contains(XFDividerThickness.Lg, values);
    }

    [Fact]
    public void Extended_XFOrientation_EnumHasAllValues()
    {
        var values = Enum.GetValues<XFOrientation>();
        Assert.Equal(2, values.Length);
        Assert.Contains(XFOrientation.Horizontal, values);
        Assert.Contains(XFOrientation.Vertical, values);
    }
}
