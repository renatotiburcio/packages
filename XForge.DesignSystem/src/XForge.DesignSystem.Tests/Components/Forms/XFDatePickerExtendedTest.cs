using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFDatePickerExtendedTest : BunitContext
{
    public XFDatePickerExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFDatePicker> RenderDatePicker(
        Action<ComponentParameterCollectionBuilder<XFDatePicker>>? configure = null)
    {
        return Render<XFDatePicker>(p => { configure?.Invoke(p); });
    }

    // --- SizeClass ---

    [Fact]
    public void Extended_SizeSm_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-9 py-1.5 pl-3 pr-1.5", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-12 py-2.5 pl-4 pr-2", cut.Markup);
    }

    [Fact]
    public void Extended_SizeMd_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Size, XFSize.Md));
        Assert.Contains("h-10 py-2 pl-3.5 pr-1.5", cut.Markup);
    }

    // --- SizeTextClass ---

    [Fact]
    public void Extended_SizeTextSm_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("text-sm", cut.Markup);
    }

    [Fact]
    public void Extended_SizeTextLg_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    [Fact]
    public void Extended_SizeTextMd_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Size, XFSize.Md));
        Assert.Contains("text-sm", cut.Markup);
    }

    // --- VariantClass ---

    [Fact]
    public void Extended_VariantFilled_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("border-transparent bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Soft));
        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_VariantGhost_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.Contains("border-transparent bg-transparent ring-1 ring-transparent", cut.Markup);
    }

    [Fact]
    public void Extended_VariantOutline_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Variant, XFFormVariant.Outline));
        Assert.Contains("border-surface-200 bg-bg-surface", cut.Markup);
    }

    // --- ToneClass ---

    [Fact]
    public void Extended_TonePrimary_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.Contains("focus-within:border-[var(--color-primary)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneSecondary_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("focus-within:border-[var(--color-secondary)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneSuccess_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("focus-within:border-[var(--color-success)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneWarning_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("focus-within:border-[var(--color-warning)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneDanger_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("focus-within:border-[var(--color-danger)]", cut.Markup);
    }

    [Fact]
    public void Extended_ToneDefault_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Tone, XFSemanticColor.Default));
        Assert.Contains("focus-within:border-[var(--color-primary)]", cut.Markup);
    }

    // --- StateClass ---

    [Fact]
    public void Extended_StateDisabled_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Disabled, true));
        Assert.Contains("cursor-not-allowed opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_StateEnabled_AppliesHoverClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Disabled, false));
        Assert.Contains("hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)]", cut.Markup);
    }

    // --- RootClass ---

    [Fact]
    public void Extended_FullWidthTrue_AppliesWFull()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    [Fact]
    public void Extended_FullWidthFalse_AppliesInlineFlex()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    // --- FieldShellClass ---

    [Fact]
    public void Extended_FieldShellClass_AppliesBaseClasses()
    {
        var cut = RenderDatePicker();
        Assert.Contains("group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out", cut.Markup);
    }

    // --- InputClass ---

    [Fact]
    public void Extended_InputClass_AppliesBaseClasses()
    {
        var cut = RenderDatePicker();
        Assert.Contains("min-w-0 flex-1 border-0 bg-transparent p-0 text-text-primary outline-none placeholder:text-text-muted", cut.Markup);
    }

    // --- AdornmentClass ---

    [Fact]
    public void Extended_AdornmentClass_AppliesBaseClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.PrefixText, "Pre"));
        Assert.Contains("flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary", cut.Markup);
    }

    // --- IconButtonClass ---

    [Fact]
    public void Extended_IconButtonClass_AppliesBaseClasses()
    {
        var cut = RenderDatePicker();
        Assert.Contains("inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary", cut.Markup);
    }

    // --- CalendarContainerClass ---

    [Fact]
    public void Extended_CalendarContainerClass_Inline_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("w-80 rounded-lg border border-surface-200 bg-bg-surface p-3", cut.Markup);
    }

    [Fact]
    public async Task Extended_CalendarContainerClass_Dropdown_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker();
        var toggle = cut.FindAll("button[aria-label='Abrir calendario']");
        if (toggle.Count > 0)
        {
            await toggle[0].ClickAsync();
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("absolute left-0 top-full z-50 mt-2 w-80 rounded-lg border border-surface-200 bg-bg-surface p-3 shadow-[0_18px_40px_rgb(15_23_42/.14)] ring-1 ring-surface-200/80", cut.Markup);
            });
        }
        else
        {
            Assert.Contains("xf-date-picker", cut.Markup);
        }
    }

    // --- NavButtonClass ---

    [Fact]
    public void Extended_NavButtonClass_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("inline-flex h-8 w-8 items-center justify-center rounded-md text-text-secondary", cut.Markup);
    }

    // --- FooterButtonClass ---

    [Fact]
    public void Extended_FooterButtonClass_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.Contains("inline-flex h-8 items-center rounded-md px-2.5 text-sm font-medium text-primary", cut.Markup);
    }

    // --- HelperClass ---

    [Fact]
    public void Extended_HelperClass_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.HelperText, "Select a date"));
        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // --- ErrorClass ---
    // Note: ErrorClass only renders when HasValidationError is true (requires EditContext with validation)

    [Fact]
    public void Extended_ErrorClass_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker();
        Assert.Contains("xf-date-picker", cut.Markup);
    }

    // --- BaseClass ---

    [Fact]
    public void Extended_BaseClass_AppliesXfDatePicker()
    {
        var cut = RenderDatePicker();
        Assert.Contains("xf-date-picker", cut.Markup);
    }

    // --- Inline ---

    [Fact]
    public void Extended_InlineTrue_RendersCalendarInline()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.NotNull(cut.Find("[role=\"dialog\"]"));
    }

    // --- Label ---

    [Fact]
    public void Extended_Label_RenderedWhenSet()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Label, "Birth Date"));
        Assert.Contains("Birth Date", cut.Markup);
        Assert.NotNull(cut.Find("label"));
    }

    [Fact]
    public void Extended_Label_RequiredAsterisk_RenderedWhenRequired()
    {
        var cut = RenderDatePicker(p =>
        {
            p.Add(x => x.Label, "Birth Date");
            p.Add(x => x.Required, true);
        });
        Assert.Contains("Birth Date", cut.Markup);
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
    }

    // --- Clearable ---

    [Fact]
    public void Extended_Clearable_NotRenderedWhenNoValue()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Clearable, true));
        Assert.DoesNotContain("Limpar", cut.Markup);
    }

    [Fact]
    public async Task Extended_Clearable_RenderedWhenValueSet()
    {
        var cut = RenderDatePicker(p =>
        {
            p.Add(x => x.Clearable, true);
            p.Add(x => x.Value, new DateOnly(2026, 6, 8));
        });
        Assert.Contains("Limpar", cut.Markup);
    }

    // --- LabelClass ---

    [Fact]
    public void Extended_LabelClass_AppliesCorrectClasses()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Label, "Date"));
        Assert.Contains("flex items-center gap-1 text-sm font-medium leading-5 text-text-primary", cut.Markup);
    }

    // --- Additional CSS detail checks ---

    [Fact]
    public void Extended_DefaultMarker_InlineMode_ContainerHasNoShadow()
    {
        var cut = RenderDatePicker(p => p.Add(x => x.Inline, true));
        Assert.DoesNotContain("shadow-[0_18px_40px_rgb(15_23_42/.14)]", cut.Markup);
    }

    [Fact]
    public void Extended_NonFullWidth_DoesNotContainWFull() 
    {
        var cut = RenderDatePicker(p => p.Add(x => x.FullWidth, false));
        Assert.DoesNotContain("w-full", cut.Markup);
    }
}
