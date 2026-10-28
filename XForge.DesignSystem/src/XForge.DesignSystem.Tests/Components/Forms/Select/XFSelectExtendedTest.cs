using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Select;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms.Select;

/// <summary>
/// Extended tests for XFSelect covering SizeClass, VariantClass, ToneClass,
/// AdornmentClass, HelperClass, and Placeholder rendering.
/// </summary>
public class XFSelectExtendedTest : XForgeTestContext
{
    public XFSelectExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // ── HelperClass rendering ──────────────────────────────────────────

    [Fact]
    public void Extended_Render_WithHelperText_HasHelperClass()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.HelperText, "Choose one");
        });

        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // ── SizeClass Sm/Lg branches ───────────────────────────────────────

    [Fact]
    public void Extended_Render_SizeSm_HasSmallHeight()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
        });

        var button = cut.Find("button");
        Assert.Contains("h-9", button.GetAttribute("class"));
    }

    [Fact]
    public void Extended_Render_SizeLg_HasLargeHeight()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
        });

        var button = cut.Find("button");
        Assert.Contains("h-12", button.GetAttribute("class"));
    }

    // ── SizeTextClass Sm/Lg branches ───────────────────────────────────

    [Fact]
    public void Extended_Render_SizeSmText_HasSmallText()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Size, XFSize.Sm);
        });

        var button = cut.Find("button");
        Assert.Contains("text-sm", button.GetAttribute("class"));
    }

    [Fact]
    public void Extended_Render_SizeLgText_HasLargeText()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Size, XFSize.Lg);
        });

        var button = cut.Find("button");
        Assert.Contains("text-base", button.GetAttribute("class"));
    }

    // ── VariantClass Filled branch ─────────────────────────────────────

    [Fact]
    public void Extended_Render_VariantFilled_HasFilledStyle()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Variant, XFFormVariant.Filled);
        });

        var button = cut.Find("button");
        Assert.Contains("bg-surface-100", button.GetAttribute("class"));
    }

    // ── ToneClass branches ─────────────────────────────────────────────

    [Fact]
    public void Extended_Render_ToneSecondary_HasSecondaryFocus()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Secondary);
        });

        var button = cut.Find("button");
        Assert.Contains("--color-secondary", button.GetAttribute("class"));
    }

    [Fact]
    public void Extended_Render_ToneSuccess_HasSuccessFocus()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Success);
        });

        var button = cut.Find("button");
        Assert.Contains("--color-success", button.GetAttribute("class"));
    }

    [Fact]
    public void Extended_Render_ToneWarning_HasWarningFocus()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Warning);
        });

        var button = cut.Find("button");
        Assert.Contains("--color-warning", button.GetAttribute("class"));
    }

    [Fact]
    public void Extended_Render_ToneDanger_HasDangerFocus()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Tone, XFSemanticColor.Danger);
        });

        var button = cut.Find("button");
        Assert.Contains("--color-danger", button.GetAttribute("class"));
    }

    // ── FullWidth false ────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_FullWidthFalse_HasInlineFlex()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.FullWidth, false);
        });

        Assert.Contains("inline-flex", cut.Markup);
    }

    // ── Placeholder ────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_Placeholder_ShowsPlaceholderText()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.Placeholder, "Pick one");
        });

        Assert.Contains("Pick one", cut.Markup);
    }

    // ── ReadOnly ───────────────────────────────────────────────────────

    [Fact]
    public void Extended_Render_ReadOnly_ButtonExists()
    {
        var cut = Render<XFSelect<string>>(p =>
        {
            p.Add(x => x.ReadOnly, true);
        });

        var button = cut.Find("button");
        Assert.NotNull(button);
    }

    // ── CanCreate ──────────────────────────────────────────────────────

    [Fact]
    public void Extended_XFSelect_CanCreate()
    {
        var select = new XFSelect<string>();
        Assert.NotNull(select);
    }
}
