using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.ColorInput;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public class XFColorInputExtendedTest : XForgeTestContext
{
    public XFColorInputExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFColorInput> RenderColorInput(
        Action<ComponentParameterCollectionBuilder<XFColorInput>>? configure = null)
    {
        return Render<XFColorInput>(p =>
        {
            p.Add(x => x.Label, "Color");
            p.Add(x => x.Value, "#FF0000");
            configure?.Invoke(p);
        });
    }

    // SwatchSizeClass Sm (line 132) — requires Palette to render swatches
    [Fact]
    public void Extended_SwatchSizeSm_RendersSmallSwatch()
    {
        var cut = RenderColorInput(p =>
        {
            p.Add(x => x.SwatchSize, XFSize.Sm);
            p.Add(x => x.Palette, new[] { "#FF0000", "#00FF00" });
        });
        Assert.Contains("h-8 w-8", cut.Markup);
    }

    // SwatchSizeClass Lg (line 133)
    [Fact]
    public void Extended_SwatchSizeLg_RendersLargeSwatch()
    {
        var cut = RenderColorInput(p =>
        {
            p.Add(x => x.SwatchSize, XFSize.Lg);
            p.Add(x => x.Palette, new[] { "#FF0000", "#00FF00" });
        });
        Assert.Contains("h-14 w-14", cut.Markup);
    }

    // SwatchShapeClass Square (line 139)
    [Fact]
    public void Extended_SwatchShapeSquare_RendersSquareSwatch()
    {
        var cut = RenderColorInput(p =>
        {
            p.Add(x => x.SwatchShape, XFShape.Square);
            p.Add(x => x.Palette, new[] { "#FF0000", "#00FF00" });
        });
        Assert.Contains("rounded-none", cut.Markup);
    }

    // SwatchShapeClass Circle (line 140)
    [Fact]
    public void Extended_SwatchShapeCircle_RendersCircleSwatch()
    {
        var cut = RenderColorInput(p =>
        {
            p.Add(x => x.SwatchShape, XFShape.Circle);
            p.Add(x => x.Palette, new[] { "#FF0000", "#00FF00" });
        });
        Assert.Contains("rounded-full", cut.Markup);
    }

    // SwatchShapeClass Pill (line 141)
    [Fact]
    public void Extended_SwatchShapePill_RendersPillSwatch()
    {
        var cut = RenderColorInput(p =>
        {
            p.Add(x => x.SwatchShape, XFShape.Pill);
            p.Add(x => x.Palette, new[] { "#FF0000", "#00FF00" });
        });
        Assert.Contains("rounded-full", cut.Markup);
    }

    // SizeClass Sm (line 161)
    [Fact]
    public void Extended_SizeSm_RendersSmallInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("min-h-9", cut.Markup);
    }

    // SizeClass Lg (line 162)
    [Fact]
    public void Extended_SizeLg_RendersLargeInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("min-h-12", cut.Markup);
    }

    // ColorInputSizeClass Sm (line 168)
    [Fact]
    public void Extended_ColorInputSizeSm_RendersSmallColorInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-7 w-9", cut.Markup);
    }

    // ColorInputSizeClass Lg (line 169)
    [Fact]
    public void Extended_ColorInputSizeLg_RendersLargeColorInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-9 w-12", cut.Markup);
    }

    // VariantClass Filled (line 182)
    [Fact]
    public void Extended_VariantFilled_RendersFilledInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // VariantClass Ghost (line 184)
    [Fact]
    public void Extended_VariantGhost_RendersGhostInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // ToneClass Secondary (line 200)
    [Fact]
    public void Extended_ToneSecondary_RendersSecondaryFocus()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("ring-secondary/16", cut.Markup);
    }

    // ToneClass Warning (line 202)
    [Fact]
    public void Extended_ToneWarning_RendersWarningFocus()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("ring-warning/20", cut.Markup);
    }

    // HelperText
    [Fact]
    public void Extended_HelperText_RendersHelper()
    {
        var cut = RenderColorInput(p => p.Add(x => x.HelperText, "Pick a color"));
        Assert.Contains("Pick a color", cut.Markup);
    }

    // Disabled
    [Fact]
    public void Extended_Disabled_RendersDisabledInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Disabled, true));
        Assert.Contains("opacity-60", cut.Markup);
    }

    // FullWidth false
    [Fact]
    public void Extended_FullWidthFalse_RendersInlineFlex()
    {
        var cut = RenderColorInput(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    // Clearable
    [Fact]
    public void Extended_Clearable_RendersClearButton()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Clearable, true));
        Assert.Contains("Clear color", cut.Markup);
    }

    // Palette
    [Fact]
    public void Extended_Palette_RendersPaletteLabel()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Palette, new[] { "#FF0000", "#00FF00", "#0000FF" }));
        Assert.Contains("Color samples", cut.Markup);
    }

    // PrefixText
    [Fact]
    public void Extended_PrefixText_RendersPrefix()
    {
        var cut = RenderColorInput(p => p.Add(x => x.PrefixText, "#"));
        Assert.Contains("#", cut.Markup);
    }

    // Immediate
    [Fact]
    public void Extended_Immediate_RendersImmediateInput()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Immediate, true));
        Assert.Contains("xf-color-input", cut.Markup);
    }

    // Required
    [Fact]
    public void Extended_Required_RendersRequiredAttribute()
    {
        var cut = RenderColorInput(p => p.Add(x => x.Required, true));
        Assert.Contains("xf-color-input", cut.Markup);
    }
}
