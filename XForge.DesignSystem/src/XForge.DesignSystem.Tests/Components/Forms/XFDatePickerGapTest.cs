using Bunit;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFDatePickerGapTest : BunitContext
{
    public XFDatePickerGapTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFDatePicker> RenderDatePicker(
        Action<ComponentParameterCollectionBuilder<XFDatePicker>>? configure = null)
    {
        return Render<XFDatePicker>(p =>
        {
            configure?.Invoke(p);
        });
    }

    // --- 1. ToneClass — Info and Neutral (fall through to default `_` branch) ---

    [Fact]
    public void Extended_ToneInfo_FallsToDefaultPrimaryClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Info));
        Assert.Contains("focus-within:border-[var(--color-primary)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneNeutral_FallsToDefaultPrimaryClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Neutral));
        Assert.Contains("focus-within:border-[var(--color-primary)]", cut.Markup);
    }

    // --- 2. PickerShellClass ---

    [Fact]
    public void Extended_PickerShellClass_ContainsRelative()
    {
        var cut = RenderDatePicker();
        Assert.Contains("relative", cut.Markup);
    }

    // --- 3. IconButtonClass — hover/focus/active/disabled classes ---

    [Fact]
    public void Extended_IconButtonClass_ContainsOutlineNone()
    {
        var cut = RenderDatePicker();
        Assert.Contains("outline-none", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsRing1RingTransparent()
    {
        var cut = RenderDatePicker();
        Assert.Contains("ring-1 ring-transparent", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsTransitionAllDuration150()
    {
        var cut = RenderDatePicker();
        Assert.Contains("transition-all duration-150", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsHoverBg()
    {
        var cut = RenderDatePicker();
        Assert.Contains("hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsHoverRing()
    {
        var cut = RenderDatePicker();
        Assert.Contains("hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsHoverTextPrimary()
    {
        var cut = RenderDatePicker();
        Assert.Contains("hover:text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsFocusVisibleRing2()
    {
        var cut = RenderDatePicker();
        Assert.Contains("focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsActiveScale95()
    {
        var cut = RenderDatePicker();
        Assert.Contains("active:scale-95", cut.Markup);
    }

    [Fact]
    public void Extended_IconButtonClass_ContainsDisabledPointerEventsNone()
    {
        var cut = RenderDatePicker();
        Assert.Contains("disabled:pointer-events-none disabled:text-text-muted", cut.Markup);
    }

    // --- 4. NavButtonClass — hover/focus classes ---

    [Fact]
    public void Extended_NavButtonClass_ContainsOutlineNoneTransitionAllDuration150()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("outline-none transition-all duration-150", cut.Markup);
    }

    [Fact]
    public void Extended_NavButtonClass_ContainsHoverBg()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_NavButtonClass_ContainsHoverTextPrimary()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("hover:text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_NavButtonClass_ContainsFocusVisibleRing()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]", cut.Markup);
    }

    // --- 5. FooterButtonClass — hover/focus/disabled classes ---

    [Fact]
    public void Extended_FooterButtonClass_ContainsOutlineNoneTransitionAllDuration150()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("outline-none transition-all duration-150", cut.Markup);
    }

    [Fact]
    public void Extended_FooterButtonClass_ContainsHoverBg()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_FooterButtonClass_ContainsFocusVisibleRing()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_FooterButtonClass_ContainsDisabledClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("disabled:pointer-events-none disabled:text-text-muted", cut.Markup);
    }

    // --- 6. InputClass — disabled suffix ---

    [Fact]
    public void Extended_InputClass_ContainsDisabledCursorNotAllowed()
    {
        var cut = RenderDatePicker();
        Assert.Contains("disabled:cursor-not-allowed disabled:text-text-muted", cut.Markup);
    }

    // --- 7. StateClass — enabled hover ring ---

    [Fact]
    public void Extended_StateClass_Enabled_ContainsHoverRing()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Disabled, false));
        Assert.Contains("hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
    }

    // --- 8. VariantClass — full ring/shadow strings ---

    [Fact]
    public void Extended_VariantFilled_ContainsRing1RingSurface200_70()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("ring-1 ring-surface-200/70", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_ContainsRing1ColorMix14()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Soft));
        Assert.Contains("ring-1 ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_VariantGhost_ContainsRing1()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.Contains("ring-1", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_ContainsRing1RingSurface200_80()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Outline));
        Assert.Contains("ring-1 ring-surface-200/80", cut.Markup);
    }

    // --- 9. SizeTextClass — Lg ---

    [Fact]
    public void Extended_SizeTextLg_ContainsTextBase()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }
}
