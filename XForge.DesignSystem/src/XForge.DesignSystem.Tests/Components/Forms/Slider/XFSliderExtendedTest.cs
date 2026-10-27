using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Slider;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms.Slider;

/// <summary>
/// Extended tests for XFSlider covering SizeClass Sm/Lg, ToneClass/ToneValueClass variants,
/// OnParametersSet edge cases (Max&lt;Min, Step&lt;=0), Clamp edge cases, and HelperId rendering.
/// Complements XFSliderComponentTest — no test name overlap.
/// </summary>
public class XFSliderExtendedTest : BunitContext
{
    // ── HelperId rendering (line 44) ───────────────────────────────────

    [Fact]
    public void Extended_Render_WithHelperText_HasHelperId()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.HelperText, "Adjust volume"));

        var helper = cut.Find("[id$='-helper']");
        Assert.NotNull(helper);
        Assert.Contains("Adjust volume", helper.TextContent);
    }

    // ── HelperClass rendering (line 104) ────────────────────────────────

    [Fact]
    public void Extended_Render_WithHelperText_HasHelperClass()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.HelperText, "Help"));

        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // ── SizeClass Sm branch (line 110-112) ──────────────────────────────

    [Fact]
    public void Extended_Render_SizeSm_HasSmallTrack()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.Size, XFSize.Sm));

        var input = cut.Find("input[type=range]");
        var cls = input.GetAttribute("class") ?? "";
        Assert.Contains("h-1", cls);
        Assert.Contains("w-4", cls);
    }

    // ── SizeClass Lg branch (line 113-114) ──────────────────────────────

    [Fact]
    public void Extended_Render_SizeLg_HasLargeTrack()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.Size, XFSize.Lg));

        var input = cut.Find("input[type=range]");
        var cls = input.GetAttribute("class") ?? "";
        Assert.Contains("h-2", cls);
        Assert.Contains("w-6", cls);
    }

    // ── ToneClass Secondary/Success branches (lines 123-124) ────────────

    [Fact]
    public void Extended_Render_ToneSecondary_HasSecondaryColor()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));

        Assert.Contains("var(--color-secondary)", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneSuccess_HasSuccessColor()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.Tone, XFSemanticColor.Success));

        Assert.Contains("var(--color-success)", cut.Markup);
    }

    // ── ToneClass Danger branch (line 126) ──────────────────────────────

    [Fact]
    public void Extended_Render_ToneDanger_HasDangerColor()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.Tone, XFSemanticColor.Danger));

        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    // ── ToneValueClass Secondary/Success branches (lines 132-133) ───────

    [Fact]
    public void Extended_Render_ToneSecondaryValue_HasSecondaryValueClass()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, 50m);
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
            p.Add(x => x.ShowValue, true);
        });

        Assert.Contains("text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneSuccessValue_HasSuccessValueClass()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, 50m);
            p.Add(x => x.Tone, XFSemanticColor.Success);
            p.Add(x => x.ShowValue, true);
        });

        Assert.Contains("text-success", cut.Markup);
    }

    // ── ToneValueClass Danger branch (line 135) ─────────────────────────

    [Fact]
    public void Extended_Render_ToneDangerValue_HasDangerValueClass()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, 50m);
            p.Add(x => x.Tone, XFSemanticColor.Danger);
            p.Add(x => x.ShowValue, true);
        });

        Assert.Contains("text-danger", cut.Markup);
    }

    // ── OnParametersSet: Max < Min branch (lines 142-144) ───────────────

    [Fact]
    public void Extended_Render_MaxLessThanMin_ClampsMaxToMin()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Min, 50m);
            p.Add(x => x.Max, 10m);
        });

        var input = cut.Find("input[type=range]");
        Assert.Equal("50", input.GetAttribute("max"));
    }

    // ── OnParametersSet: Step <= 0 branch (lines 147-149) ───────────────

    [Fact]
    public void Extended_Render_StepZero_SetsStepToOne()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.Step, 0m));

        var input = cut.Find("input[type=range]");
        Assert.Equal("1", input.GetAttribute("step"));
    }

    [Fact]
    public void Extended_Render_StepNegative_SetsStepToOne()
    {
        var cut = Render<XFSlider>(p => p.Add(x => x.Step, -5m));

        var input = cut.Find("input[type=range]");
        Assert.Equal("1", input.GetAttribute("step"));
    }

    // ── Clamp: value < Min branch (lines 206-207) ───────────────────────

    [Fact]
    public void Extended_InputValueBelowMin_ClampsToMin()
    {
        decimal? value = 5m;
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal?>(this, next => value = next));
            p.Add(x => x.Min, 10m);
            p.Add(x => x.Max, 100m);
        });

        cut.Find("input[type=range]").Change("2");
        Assert.Equal(10m, value);
    }

    // ── Clamp: value > Max branch (lines 210-212) ───────────────────────

    [Fact]
    public void Extended_InputValueAboveMax_ClampsToMax()
    {
        decimal? value = 50m;
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal?>(this, next => value = next));
            p.Add(x => x.Min, 0m);
            p.Add(x => x.Max, 30m);
        });

        cut.Find("input[type=range]").Change("99");
        Assert.Equal(30m, value);
    }

    // ── Vertical + Size Sm ──────────────────────────────────────────────

    [Fact]
    public void Extended_Render_VerticalSizeSm_HasVerticalSmallTrack()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Orientation, XFOrientation.Vertical);
            p.Add(x => x.Size, XFSize.Sm);
        });

        Assert.Contains("[writing-mode:vertical-lr]", cut.Markup);
    }

    // ── ShowValue false ─────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_ShowValueFalse_HidesValue()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, 50m);
            p.Add(x => x.ShowValue, false);
        });

        Assert.DoesNotContain("50", cut.Find("div[class*='xf-slider']").TextContent);
    }

    // ── ShowBounds false ────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_ShowBoundsFalse_HidesBoundsDiv()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.ShowBounds, false);
            p.Add(x => x.Min, 0m);
            p.Add(x => x.Max, 100m);
        });

        // When ShowBounds is false, the bounds div should not render
        Assert.DoesNotContain("aria-hidden=\"true\"", cut.Markup);
    }

    // ── PrefixText + SuffixText ─────────────────────────────────────────

    [Fact]
    public void Extended_Render_WithPrefixAndSuffix_ShowsFormatted()
    {
        var cut = Render<XFSlider>(p =>
        {
            p.Add(x => x.Value, 75m);
            p.Add(x => x.PrefixText, "$");
            p.Add(x => x.SuffixText, " USD");
            p.Add(x => x.ShowValue, true);
        });

        Assert.Contains("$75 USD", cut.Markup);
    }

    // ── CanCreate ───────────────────────────────────────────────────────

    [Fact]
    public void Extended_XFSlider_CanCreate()
    {
        var slider = new XFSlider();
        Assert.NotNull(slider);
    }
}
