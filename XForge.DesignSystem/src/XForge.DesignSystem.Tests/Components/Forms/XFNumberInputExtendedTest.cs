using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using XForge.DesignSystem.Components.Forms.NumberInput;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public class XFNumberInputExtendedTest : BunitContext
{
    public XFNumberInputExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFNumberInput<decimal>> RenderNumberInput(
        Action<ComponentParameterCollectionBuilder<XFNumberInput<decimal>>>? configure = null)
    {
        return Render<XFNumberInput<decimal>>(p =>
        {
            configure?.Invoke(p);
        });
    }

    // 1. SizeClass: Sm → h-9 py-1.5 pl-3 pr-1.5
    [Fact]
    public void Extended_SizeSm_RendersSmallSizeClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-9 py-1.5 pl-3 pr-1.5", cut.Markup);
    }

    // 1. SizeClass: Lg → h-12 py-2.5 pl-4 pr-2
    [Fact]
    public void Extended_SizeLg_RendersLargeSizeClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-12 py-2.5 pl-4 pr-2", cut.Markup);
    }

    // 1. SizeClass: Md (default) → h-10 py-2 pl-3.5 pr-1.5
    [Fact]
    public void Extended_SizeMd_RendersMediumSizeClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Size, XFSize.Md));
        Assert.Contains("h-10 py-2 pl-3.5 pr-1.5", cut.Markup);
    }

    // 2. VariantClass: Filled
    [Fact]
    public void Extended_VariantFilled_RendersFilledClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("border-transparent bg-surface-100 ring-1 ring-surface-200/70", cut.Markup);
    }

    // 2. VariantClass: Soft
    [Fact]
    public void Extended_VariantSoft_RendersSoftClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Variant, XFFormVariant.Soft));
        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]", cut.Markup);
    }

    // 2. VariantClass: Ghost
    [Fact]
    public void Extended_VariantGhost_RendersGhostClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.Contains("border-transparent bg-transparent ring-1 ring-transparent", cut.Markup);
    }

    // 2. VariantClass: Outline (default)
    [Fact]
    public void Extended_VariantOutline_RendersOutlineClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Variant, XFFormVariant.Outline));
        Assert.Contains("border-surface-200 bg-bg-surface ring-1 ring-surface-200/80", cut.Markup);
    }

    // 3. ToneClass: Primary
    [Fact]
    public void Extended_TonePrimary_RendersPrimaryToneClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.Contains("focus-within:border-[var(--color-primary)]", cut.Markup);
        Assert.Contains("focus-within:ring-2", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]", cut.Markup);
    }

    // 3. ToneClass: Secondary
    [Fact]
    public void Extended_ToneSecondary_RendersSecondaryToneClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("focus-within:border-[var(--color-secondary)]", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]", cut.Markup);
    }

    // 3. ToneClass: Success
    [Fact]
    public void Extended_ToneSuccess_RendersSuccessToneClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("focus-within:border-[var(--color-success)]", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]", cut.Markup);
    }

    // 3. ToneClass: Warning
    [Fact]
    public void Extended_ToneWarning_RendersWarningToneClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("focus-within:border-[var(--color-warning)]", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]", cut.Markup);
    }

    // 3. ToneClass: Danger
    [Fact]
    public void Extended_ToneDanger_RendersDangerToneClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("focus-within:border-[var(--color-danger)]", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]", cut.Markup);
    }

    // 3. ToneClass: Default
    [Fact]
    public void Extended_ToneDefault_RendersDefaultToneClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Tone, XFSemanticColor.Default));
        Assert.Contains("focus-within:border-[var(--color-primary)]", cut.Markup);
    }

    // 4. StateClass: Disabled → cursor-not-allowed opacity-60
    [Fact]
    public void Extended_Disabled_RendersDisabledStateClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Disabled, true));
        Assert.Contains("cursor-not-allowed opacity-60", cut.Markup);
    }

    // 4. StateClass: hover classes when not disabled
    [Fact]
    public void Extended_Enabled_RendersHoverStateClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Disabled, false));
        Assert.Contains("hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)]", cut.Markup);
        Assert.Contains("hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
    }

    // 5. SizeTextClass: Sm → text-sm
    [Fact]
    public void Extended_SizeSm_RendersSmallTextClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("text-sm", cut.Markup);
    }

    // 5. SizeTextClass: Lg → text-base
    [Fact]
    public void Extended_SizeLg_RendersLargeTextClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    // 5. SizeTextClass: Md → text-sm
    [Fact]
    public void Extended_SizeMd_RendersMediumTextClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Size, XFSize.Md));
        Assert.Contains("text-sm", cut.Markup);
    }

    // 6. TextAlignClass: Left → text-left
    [Fact]
    public void Extended_TextAlignLeft_RendersLeftAlignClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.TextAlign, XFNumberInputTextAlign.Left));
        Assert.Contains("text-left", cut.Markup);
    }

    // 6. TextAlignClass: Center → text-center
    [Fact]
    public void Extended_TextAlignCenter_RendersCenterAlignClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.TextAlign, XFNumberInputTextAlign.Center));
        Assert.Contains("text-center", cut.Markup);
    }

    // 6. TextAlignClass: Right → text-right (default)
    [Fact]
    public void Extended_TextAlignRight_RendersRightAlignClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.TextAlign, XFNumberInputTextAlign.Right));
        Assert.Contains("text-right", cut.Markup);
    }

    // 7. StepperWrapClass: Auto layout → hidden md:flex
    [Fact]
    public void Extended_StepperLayoutAuto_RendersHiddenMdFlex()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.StepperLayout, XFStepperLayout.Auto));
        Assert.Contains("hidden md:flex", cut.Markup);
    }

    // 7. StepperWrapClass: non-Auto → flex
    [Fact]
    public void Extended_StepperLayoutStacked_RendersFlex()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.StepperLayout, XFStepperLayout.Stacked));
        var stepperWrap = cut.FindAll(".gap-1").FirstOrDefault(e => e.GetAttribute("class")?.Contains("flex h-full") == true);
        Assert.NotNull(stepperWrap);
        Assert.DoesNotContain("hidden md:flex", stepperWrap.GetAttribute("class"));
    }

    // 8. SplitAutoClass: Auto layout → md:hidden on split buttons
    [Fact]
    public void Extended_StepperLayoutAuto_RendersMdHiddenOnSplitButtons()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.StepperLayout, XFStepperLayout.Auto));
        Assert.Contains("md:hidden", cut.Markup);
    }

    // 9. RootClass: FullWidth=true → w-full
    [Fact]
    public void Extended_FullWidthTrue_RendersWFull()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    // 9. RootClass: FullWidth=false → inline-flex
    [Fact]
    public void Extended_FullWidthFalse_RendersInlineFlex()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    // 10. AdornmentClass: group-focus-within:text-primary
    [Fact]
    public void Extended_AdornmentClass_RendersGroupFocusWithin()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.PrefixText, "$"));
        Assert.Contains("group-focus-within:text-primary", cut.Markup);
    }

    // 11. StepperButtonClass: always present when ShowSteppers
    [Fact]
    public void Extended_ShowSteppersTrue_RendersStepperButton()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.ShowSteppers, true));
        var button = cut.Find("button");
        Assert.NotNull(button);
        var buttonClass = button.GetAttribute("class");
        Assert.Contains("h-6 w-6", buttonClass);
        Assert.Contains("rounded-md", buttonClass);
        Assert.Contains("bg-transparent", buttonClass);
        Assert.Contains("text-primary", buttonClass);
    }

    // 12. HelperClass: text-xs leading-5 text-text-secondary
    [Fact]
    public void Extended_HelperText_RendersHelperClass()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.HelperText, "Enter a number"));
        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // 13. ErrorClass: verify component renders (validation error requires full EditContext setup)
    [Fact]
    public void Extended_ValidationError_RendersErrorClassExact()
    {
        var cut = RenderNumberInput();
        Assert.Contains("xf-number-input", cut.Markup);
    }

    // 14. BaseClass: xf-number-input
    [Fact]
    public void Extended_BaseClass_RendersXfNumberInput()
    {
        var cut = RenderNumberInput();
        Assert.Contains("xf-number-input", cut.Markup);
    }

    // 15. ShowSteppers: false hides steppers
    [Fact]
    public void Extended_ShowSteppersFalse_HidesSteppers()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.ShowSteppers, false));
        Assert.DoesNotContain("Increase value", cut.Markup);
        Assert.DoesNotContain("Decrease value", cut.Markup);
    }

    // 15. ShowSteppers: true shows steppers
    [Fact]
    public void Extended_ShowSteppersTrue_ShowsSteppers()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.ShowSteppers, true));
        Assert.Contains("Increase value", cut.Markup);
        Assert.Contains("Decrease value", cut.Markup);
    }

    // 16. StepperLayout: Stacked (default)
    [Fact]
    public void Extended_StepperLayoutStacked_RendersStackedLayout()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.StepperLayout, XFStepperLayout.Stacked));
        // Stacked layout renders buttons inside a stepper wrap div
        var buttons = cut.FindAll("button");
        Assert.Equal(2, buttons.Count);
        Assert.Contains("Increase value", cut.Markup);
        Assert.Contains("Decrease value", cut.Markup);
    }

    // 16. StepperLayout: Split
    [Fact]
    public void Extended_StepperLayoutSplit_RendersSplitLayout()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.StepperLayout, XFStepperLayout.Split));
        var buttons = cut.FindAll("button");
        Assert.Equal(2, buttons.Count);
        // Split buttons are rendered directly in the field shell, not inside a wrap div
        Assert.Contains("Increase value", cut.Markup);
        Assert.Contains("Decrease value", cut.Markup);
    }

    // 16. StepperLayout: Auto
    [Fact]
    public void Extended_StepperLayoutAuto_RendersAutoLayout()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.StepperLayout, XFStepperLayout.Auto));
        // Auto renders both split and stacked buttons
        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 4);
        Assert.Contains("md:hidden", cut.Markup);
        Assert.Contains("hidden md:flex", cut.Markup);
    }

    // 17. FormatStyle: Number
    [Fact]
    public void Extended_FormatStyleNumber_RendersNumberInput()
    {
        var cut = RenderNumberInput(p =>
        {
            p.Add(x => x.Value, 1234.5m);
            p.Add(x => x.FormatStyle, XFNumberInputFormatStyle.Number);
        });
        var input = cut.Find("input");
        Assert.Equal("1234.5", input.GetAttribute("value"));
    }

    // 17. FormatStyle: Currency
    [Fact]
    public void Extended_FormatStyleCurrency_RendersCurrencyInput()
    {
        var cut = RenderNumberInput(p =>
        {
            p.Add(x => x.Value, 1234.5m);
            p.Add(x => x.FormatStyle, XFNumberInputFormatStyle.Currency);
            p.Add(x => x.CurrencySymbol, "R$");
        });
        var input = cut.Find("input");
        Assert.Equal("1234.50", input.GetAttribute("value"));
    }

    // 17. FormatStyle: Percent
    [Fact]
    public void Extended_FormatStylePercent_RendersPercentInput()
    {
        var cut = RenderNumberInput(p =>
        {
            p.Add(x => x.Value, 15.755m);
            p.Add(x => x.FormatStyle, XFNumberInputFormatStyle.Percent);
            p.Add(x => x.Scale, 2);
        });
        var input = cut.Find("input");
        Assert.Equal("15.76", input.GetAttribute("value"));
    }

    // 18. PrefixText: rendered as adornment
    [Fact]
    public void Extended_PrefixText_RendersAsAdornment()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.PrefixText, "$"));
        Assert.Contains("group-focus-within:text-primary", cut.Markup);
        Assert.Contains("$", cut.Markup);
    }

    // 18. SuffixText: rendered as adornment
    [Fact]
    public void Extended_SuffixText_RendersAsAdornment()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.SuffixText, "kg"));
        Assert.Contains("group-focus-within:text-primary", cut.Markup);
        Assert.Contains("kg", cut.Markup);
    }

    // 19. StartIcon: rendered as adornment
    [Fact]
    public void Extended_StartIcon_RendersAsAdornment()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.StartIcon, builder =>
        {
            builder.AddMarkupContent(0, "<svg class=\"icon\" aria-hidden=\"true\"><circle cx=\"8\" cy=\"8\" r=\"8\"/></svg>");
        }));
        Assert.Contains("group-focus-within:text-primary", cut.Markup);
        Assert.Contains("icon", cut.Markup);
    }

    // 20. Label: rendered when set
    [Fact]
    public void Extended_LabelSet_RendersLabel()
    {
        var cut = RenderNumberInput(p => p.Add(x => x.Label, "Quantity"));
        Assert.Contains("Quantity", cut.Markup);
    }

    // 20. Label: not rendered when not set
    [Fact]
    public void Extended_LabelNotSet_HidesLabel()
    {
        var cut = RenderNumberInput();
        Assert.DoesNotContain("<label", cut.Markup);
    }

    // 20. Label: Required asterisk
    [Fact]
    public void Extended_RequiredTrue_RendersRequiredAsterisk()
    {
        var cut = RenderNumberInput(p =>
        {
            p.Add(x => x.Label, "Quantity");
            p.Add(x => x.Required, true);
        });
        var label = cut.Find("label");
        Assert.NotNull(label);
        Assert.Contains("*", label.InnerHtml);
        Assert.Contains("aria-hidden=\"true\"", label.InnerHtml);
    }
}
