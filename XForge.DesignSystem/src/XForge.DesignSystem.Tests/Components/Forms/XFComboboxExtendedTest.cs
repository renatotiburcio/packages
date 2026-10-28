using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.Combobox;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

internal sealed record ComboboxOption(int Id, string Name, bool Locked);

public sealed class XFComboboxExtendedTest : XForgeTestContext
{
    private readonly IReadOnlyList<ComboboxOption> _options = new[]
    {
        new ComboboxOption(1, "Finance", false),
        new ComboboxOption(2, "Operations", false),
        new ComboboxOption(3, "People", true)
    };

    private IRenderedComponent<XFCombobox<ComboboxOption, int?>> RenderCombobox(
        Action<ComponentParameterCollectionBuilder<XFCombobox<ComboboxOption, int?>>>? configure = null)
    {
        return Render<XFCombobox<ComboboxOption, int?>>(parameters =>
        {
            parameters.Add(p => p.Label, "Department");
            parameters.Add(p => p.Items, _options);
            parameters.Add(p => p.TextSelector, option => option.Name);
            parameters.Add(p => p.ValueSelector, option => option.Id);
            configure?.Invoke(parameters);
        });
    }

    // HelperId (line 62) + HelperClass (line 116) — rendered when HelperText is set
    [Fact]
    public void Extended_HelperText_RendersHelperDiv()
    {
        var cut = RenderCombobox(p => p.Add(x => x.HelperText, "Pick a department"));
        Assert.Contains("Pick a department", cut.Markup);
        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // AdornmentClass (line 110) — rendered when StartIcon is set
    [Fact]
    public void Extended_StartIcon_RendersAdornmentClass()
    {
        var cut = RenderCombobox(p => p.Add(x => x.StartIcon, (RenderFragment)(builder => { builder.AddMarkupContent(0, "<span>icon</span>"); })));
        Assert.Contains("group-focus-within:text-primary", cut.Markup);
    }

    // SizeClass Sm (line 121) + SizeTextClass Sm (line 128)
    [Fact]
    public void Extended_SizeSm_RendersSmallClasses()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Size, XFSize.Sm));
        var input = cut.Find("input");
        Assert.NotNull(input);
        Assert.Contains("h-9 py-1.5 pl-3 pr-1.5", cut.Markup);
    }

    // SizeClass Lg (line 122)
    [Fact]
    public void Extended_SizeLg_RendersLargeClasses()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-12 py-2.5 pl-4 pr-2", cut.Markup);
    }

    // SizeTextClass Lg (line 129)
    [Fact]
    public void Extended_SizeLg_RendersLargeText()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    // VariantClass Filled (line 135)
    [Fact]
    public void Extended_VariantFilled_RendersFilledClasses()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("bg-surface-100", cut.Markup);
        Assert.Contains("ring-surface-200/70", cut.Markup);
    }

    // VariantClass Soft (line 136)
    [Fact]
    public void Extended_VariantSoft_RendersSoftClasses()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Variant, XFFormVariant.Soft));
        Assert.Contains("var(--color-primary)", cut.Markup);
    }

    // VariantClass Ghost (line 137)
    [Fact]
    public void Extended_VariantGhost_RendersGhostClasses()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
        Assert.Contains("border-transparent bg-transparent", cut.Markup);
    }

    // ToneClass Secondary (line 152)
    [Fact]
    public void Extended_ToneSecondary_RendersSecondaryFocus()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("var(--color-secondary)", cut.Markup);
    }

    // ToneClass Success (line 153)
    [Fact]
    public void Extended_ToneSuccess_RendersSuccessFocus()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("var(--color-success)", cut.Markup);
    }

    // ToneClass Warning (line 154)
    [Fact]
    public void Extended_ToneWarning_RendersWarningFocus()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    // ToneClass Danger (line 155)
    [Fact]
    public void Extended_ToneDanger_RendersDangerFocus()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    // StateClass Disabled (line 161)
    [Fact]
    public void Extended_Disabled_RendersDisabledState()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Disabled, true));
        Assert.Contains("cursor-not-allowed opacity-60", cut.Markup);
    }

    // ErrorClass (line 117) — rendered when ErrorText is set
    [Fact]
    public void Extended_ErrorText_RendersErrorDiv()
    {
        var cut = RenderCombobox(p => p.Add(x => x.ErrorText, "Invalid selection"));
        Assert.Contains("Invalid selection", cut.Markup);
        Assert.Contains("text-xs font-medium leading-5 text-danger", cut.Markup);
    }

    // LabelClass (line 106) — rendered when Label is set
    [Fact]
    public void Extended_Label_RendersLabelClass()
    {
        var cut = RenderCombobox();
        Assert.Contains("text-sm font-medium leading-5 text-text-primary", cut.Markup);
    }

    // InputClass (line 109)
    [Fact]
    public void Extended_Input_RendersInputClass()
    {
        var cut = RenderCombobox();
        var input = cut.Find("input");
        Assert.Contains("min-w-0 flex-1 border-0 bg-transparent", input.GetAttribute("class"));
    }

    // ToggleButtonClass (line 112)
    [Fact]
    public void Extended_ToggleButton_RendersToggleClass()
    {
        var cut = RenderCombobox();
        var toggleBtn = cut.FindAll("button").FirstOrDefault(b => b.GetAttribute("aria-label") == "Toggle dropdown");
        if (toggleBtn is null)
            toggleBtn = cut.FindAll("button").LastOrDefault();
        Assert.NotNull(toggleBtn);
        Assert.Contains("h-6 w-6", toggleBtn.GetAttribute("class"));
    }

    // ChevronClass (line 113)
    [Fact]
    public void Extended_Chevron_RendersChevronClass()
    {
        var cut = RenderCombobox();
        Assert.Contains("h-4 w-4 transition-transform duration-150", cut.Markup);
    }

    // IconButtonClass (line 111) — rendered when Clearable + HasSelection
    [Fact]
    public void Extended_ClearableWithSelection_RendersIconButton()
    {
        var cut = RenderCombobox(p =>
        {
            p.Add(x => x.Clearable, true);
            p.Add(x => x.Value, 1);
        });
        Assert.Contains("h-6 w-6", cut.Markup);
        Assert.Contains("active:scale-95", cut.Markup);
    }

    // FullWidth false (line 105)
    [Fact]
    public void Extended_FullWidthFalse_RendersInlineFlex()
    {
        var cut = RenderCombobox(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("inline-flex", cut.Markup);
    }

    // ReadOnly
    [Fact]
    public void Extended_ReadOnly_RendersReadonlyAttribute()
    {
        var cut = RenderCombobox(p => p.Add(x => x.ReadOnly, true));
        var input = cut.Find("input");
        Assert.NotNull(input.GetAttribute("readonly"));
    }

    // Placeholder
    [Fact]
    public void Extended_Placeholder_RendersOnInput()
    {
        var cut = RenderCombobox(p => p.Add(x => x.Placeholder, "Search..."));
        var input = cut.Find("input");
        Assert.Equal("Search...", input.GetAttribute("placeholder"));
    }
}
