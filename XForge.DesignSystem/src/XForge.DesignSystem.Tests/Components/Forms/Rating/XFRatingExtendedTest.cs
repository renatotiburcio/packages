using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Forms.Rating;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms.Rating;

/// <summary>
/// Extended tests for XFRating covering SizeClass variants, ToneTextClass variants,
/// keyboard edge cases, NormalizeValue edge cases, and ClearAsync path.
/// Complements XFRatingComponentTest — no test name overlap.
/// </summary>
public class XFRatingExtendedTest : XForgeTestContext
{
    // ── SizeClass branches (Sm, Lg, Xl) ────────────────────────────────

    [Fact]
    public void Extended_Render_SizeSm_HasSmallDimensions()
    {
        var cut = Render<XFRating>(p => p.Add(x => x.Size, XFExtendedSize.Sm));

        Assert.Contains("h-7", cut.Markup);
        Assert.Contains("w-7", cut.Markup);
    }

    [Fact]
    public void Extended_Render_SizeLg_HasLargeDimensions()
    {
        var cut = Render<XFRating>(p => p.Add(x => x.Size, XFExtendedSize.Lg));

        Assert.Contains("h-10", cut.Markup);
        Assert.Contains("w-10", cut.Markup);
    }

    [Fact]
    public void Extended_Render_SizeXl_HasExtraLargeDimensions()
    {
        var cut = Render<XFRating>(p => p.Add(x => x.Size, XFExtendedSize.Xl));

        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("w-12", cut.Markup);
    }

    // ── ToneTextClass branches (Secondary, Success, Danger, Neutral) ────

    [Fact]
    public void Extended_Render_ToneSecondary_HasSecondaryText()
    {
        var cut = Render<XFRating>(p => p
            .Add(x => x.Value, 3d)
            .Add(x => x.Tone, XFSemanticColor.Secondary));

        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneSuccess_HasSuccessText()
    {
        var cut = Render<XFRating>(p => p
            .Add(x => x.Value, 3d)
            .Add(x => x.Tone, XFSemanticColor.Success));

        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneDanger_HasDangerText()
    {
        var cut = Render<XFRating>(p => p
            .Add(x => x.Value, 3d)
            .Add(x => x.Tone, XFSemanticColor.Danger));

        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Extended_Render_ToneNeutral_HasPrimaryText()
    {
        var cut = Render<XFRating>(p => p
            .Add(x => x.Value, 3d)
            .Add(x => x.Tone, XFSemanticColor.Neutral));

        Assert.Contains("text-text-primary", cut.Markup);
    }

    // ── Keyboard: ArrowLeft/Down, Home, End, Delete/Backspace ──────────

    [Fact]
    public async Task Extended_Keyboard_ArrowLeft_DecrementsValue()
    {
        var value = 3d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("[role=radiogroup]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        Assert.Equal(2d, value);
    }

    [Fact]
    public async Task Extended_Keyboard_ArrowDown_DecrementsValue()
    {
        var value = 3d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("[role=radiogroup]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal(2d, value);
    }

    [Fact]
    public async Task Extended_Keyboard_Home_SetsMinValue()
    {
        var value = 3d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("[role=radiogroup]").KeyDownAsync(new KeyboardEventArgs { Key = "Home" });
        Assert.Equal(1d, value);
    }

    [Fact]
    public async Task Extended_Keyboard_HomeWithAllowReset_SetsZero()
    {
        var value = 3d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.AllowReset, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("[role=radiogroup]").KeyDownAsync(new KeyboardEventArgs { Key = "Home" });
        Assert.Equal(0d, value);
    }

    [Fact]
    public async Task Extended_Keyboard_DeleteWithAllowReset_ClearsValue()
    {
        var value = 3d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.AllowReset, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("[role=radiogroup]").KeyDownAsync(new KeyboardEventArgs { Key = "Delete" });
        Assert.Equal(0d, value);
    }

    [Fact]
    public async Task Extended_Keyboard_BackspaceWithAllowReset_ClearsValue()
    {
        var value = 2d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.AllowReset, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("[role=radiogroup]").KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });
        Assert.Equal(0d, value);
    }

    // ── Keyboard: non-interactive does nothing ─────────────────────────

    [Fact]
    public async Task Extended_Keyboard_Disabled_DoesNotChangeValue()
    {
        var value = 2d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("[role=radiogroup]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal(2d, value);
    }

    // ── ClearAsync via clear button click ───────────────────────────────

    [Fact]
    public async Task Extended_ClickClearButton_ClearsValue()
    {
        var value = 3d;
        var cut = Render<XFRating>(p =>
        {
            p.Add(x => x.AllowReset, true);
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, next => value = next));
            p.Add(x => x.ValueExpression, () => value);
        });

        await cut.Find("button[aria-label='Limpar avaliacao']").ClickAsync();
        Assert.Equal(0d, value);
    }

    // ── NormalizeValue edge cases: NaN / Infinity clamp to 0 ────────────

    [Fact]
    public void Extended_Render_ValueNaN_ClampsToZero()
    {
        var cut = Render<XFRating>(p => p.Add(x => x.Value, double.NaN));

        // NaN normalizes to 0 — no stars filled
        var checkedRadios = cut.FindAll("button[aria-checked='true']");
        Assert.Empty(checkedRadios);
    }

    [Fact]
    public void Extended_Render_ValueInfinity_ClampsToZero()
    {
        var cut = Render<XFRating>(p => p.Add(x => x.Value, double.PositiveInfinity));

        // Infinity normalizes to 0 — no stars filled
        var checkedRadios = cut.FindAll("button[aria-checked='true']");
        Assert.Empty(checkedRadios);
    }

    // ── Max value ───────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_CustomMax_RendersCorrectCount()
    {
        var cut = Render<XFRating>(p => p.Add(x => x.Max, 3));

        var radios = cut.FindAll("button[role=radio]");
        Assert.Equal(3, radios.Count);
    }

    // ── Half value rendering ────────────────────────────────────────────

    [Fact]
    public void Extended_Render_HalfValue_HasSomeFilledStars()
    {
        var cut = Render<XFRating>(p => p.Add(x => x.Value, 2.5));

        // 2.5 means at least 1 filled star
        var checkedRadios = cut.FindAll("button[aria-checked='true']");
        Assert.True(checkedRadios.Count >= 1);
    }

    // ── CanCreate ───────────────────────────────────────────────────────

    [Fact]
    public void Extended_XFRating_CanCreate()
    {
        var rating = new XFRating();
        Assert.NotNull(rating);
    }
}
