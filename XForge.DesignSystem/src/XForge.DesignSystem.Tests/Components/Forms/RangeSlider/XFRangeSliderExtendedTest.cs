using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.RangeSlider;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms.RangeSlider;

/// <summary>
/// Extended tests for XFRangeSlider covering SizeClass Sm/Lg, TrackHeightClass,
/// ToneThumbClass/ToneValueClass/TrackToneClass variants, OnParametersSet edge cases,
/// HelperClass, and AllowOverlap rendering.
/// Complements XFRangeSliderComponentTest — no test name overlap.
/// </summary>
public class XFRangeSliderExtendedTest : XForgeTestContext
{
    // ── HelperClass rendering (line 116) ────────────────────────────────

    [Fact]
    public void Extended_Render_WithHelperText_HasHelperClass()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.HelperText, "Select range"));

        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // ── TrackHeightClass Sm/Lg (lines 121-122) ──────────────────────────

    [Fact]
    public void Extended_Render_SizeSm_HasSmallTrack()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Size, XFSize.Sm));

        Assert.Contains("h-1", cut.Markup);
    }

    [Fact]
    public void Extended_Render_SizeLg_HasLargeTrack()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Size, XFSize.Lg));

        Assert.Contains("h-2", cut.Markup);
    }

    // ── SizeClass Sm/Lg (lines 128-129) ─────────────────────────────────

    [Fact]
    public void Extended_Render_InputSizeSm_HasSmallThumbs()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Size, XFSize.Sm));

        var inputs = cut.FindAll("input[type=range]");
        Assert.True(inputs.Count >= 2);
        var cls = inputs[0].GetAttribute("class") ?? "";
        Assert.Contains("h-4", cls);
        Assert.Contains("w-4", cls);
    }

    [Fact]
    public void Extended_Render_InputSizeLg_HasLargeThumbs()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Size, XFSize.Lg));

        var inputs = cut.FindAll("input[type=range]");
        var cls = inputs[0].GetAttribute("class") ?? "";
        Assert.Contains("h-6", cls);
        Assert.Contains("w-6", cls);
    }

    // ── ToneThumbClass Secondary/Success (lines 135-136) ─────────────────

    [Fact]
    public void Extended_Render_ToneSecondary_HasSecondaryThumb()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));

        Assert.Contains("var(--color-secondary)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneSuccess_HasSuccessThumb()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Tone, XFSemanticColor.Success));

        Assert.Contains("var(--color-success)", cut.Markup);
    }

    // ── ToneThumbClass Warning/Danger (lines 137-138) ───────────────────

    [Fact]
    public void Extended_Render_ToneWarning_HasWarningThumb()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Tone, XFSemanticColor.Warning));

        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneDanger_HasDangerThumb()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Tone, XFSemanticColor.Danger));

        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    // ── TrackToneClass Secondary/Success/Warning/Danger (lines 144-147) ─

    [Fact]
    public void Extended_Render_TrackToneSecondary_HasSecondaryTrack()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.Add(x => x.StartValue, 20m);
            p.Add(x => x.EndValue, 80m);
        });

        Assert.Contains("--xf-range-color:var(--color-secondary)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_TrackToneDanger_HasDangerTrack()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.Add(x => x.StartValue, 20m);
            p.Add(x => x.EndValue, 80m);
        });

        Assert.Contains("--xf-range-color:var(--color-danger)", cut.Markup);
    }

    // ── ToneValueClass Secondary/Success/Warning/Danger (lines 159-162) ─

    [Fact]
    public void Extended_Render_ToneSecondaryValue_HasSecondaryValue()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.Add(x => x.StartValue, 20m);
            p.Add(x => x.EndValue, 80m);
            p.Add(x => x.ShowValue, true);
        });

        Assert.Contains("text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneDangerValue_HasDangerValue()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.Add(x => x.StartValue, 20m);
            p.Add(x => x.EndValue, 80m);
            p.Add(x => x.ShowValue, true);
        });

        Assert.Contains("text-danger", cut.Markup);
    }

    // ── OnParametersSet: Max < Min (lines 169-171) ───────────────────────

    [Fact]
    public void Extended_Render_MaxLessThanMin_ClampsMaxToMin()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.Min, 50m);
            p.Add(x => x.Max, 10m);
        });

        var inputs = cut.FindAll("input[type=range]");
        Assert.All(inputs, input => Assert.Equal("50", input.GetAttribute("max")));
    }

    // ── OnParametersSet: Step <= 0 (lines 174-176) ───────────────────────

    [Fact]
    public void Extended_Render_StepZero_SetsStepToOne()
    {
        var cut = Render<XFRangeSlider>(p => p.Add(x => x.Step, 0m));

        var inputs = cut.FindAll("input[type=range]");
        Assert.All(inputs, input => Assert.Equal("1", input.GetAttribute("step")));
    }

    // ── AllowOverlap ─────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_AllowOverlap_HasFullWidthInputs()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.AllowOverlap, true);
            p.Add(x => x.StartValue, 30m);
            p.Add(x => x.EndValue, 70m);
        });

        var inputs = cut.FindAll("input[type=range]");
        Assert.All(inputs, input => Assert.Contains("inset-x-0", input.GetAttribute("class") ?? ""));
    }

    // ── ShowValue false ──────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_ShowValueFalse_HidesValue()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.ShowValue, false);
            p.Add(x => x.StartValue, 25m);
            p.Add(x => x.EndValue, 75m);
        });

        Assert.DoesNotContain("25", cut.Find(".xf-range-slider").TextContent);
    }

    // ── ShowBounds false ─────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_ShowBoundsFalse_HidesBounds()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.ShowBounds, false);
            p.Add(x => x.Min, 0m);
            p.Add(x => x.Max, 100m);
        });

        Assert.DoesNotContain("justify-between gap-2 text-xs", cut.Markup);
    }

    // ── Disabled state ───────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_Disabled_HasMutedTrack()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.StartValue, 30m);
            p.Add(x => x.EndValue, 70m);
        });

        Assert.Contains("text-text-muted", cut.Markup);
    }

    // ── PrefixText + SuffixText ──────────────────────────────────────────

    [Fact]
    public void Extended_Render_WithPrefixSuffix_ShowsFormatted()
    {
        var cut = Render<XFRangeSlider>(p =>
        {
            p.Add(x => x.StartValue, 10m);
            p.Add(x => x.EndValue, 90m);
            p.Add(x => x.PrefixText, "$");
            p.Add(x => x.SuffixText, "k");
            p.Add(x => x.ShowValue, true);
        });

        Assert.Contains("$10k", cut.Markup);
        Assert.Contains("$90k", cut.Markup);
    }

    // ── CanCreate ────────────────────────────────────────────────────────

    [Fact]
    public void Extended_XFRangeSlider_CanCreate()
    {
        var slider = new XFRangeSlider();
        Assert.NotNull(slider);
    }
}
