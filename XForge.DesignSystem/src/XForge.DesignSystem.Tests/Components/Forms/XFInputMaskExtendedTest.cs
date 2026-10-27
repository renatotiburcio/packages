using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.InputMask;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFInputMaskExtendedTest : BunitContext
{
    private IRenderedComponent<XFInputMask> RenderMask(
        Action<ComponentParameterCollectionBuilder<XFInputMask>>? configure = null)
    {
        return Render<XFInputMask>(p =>
        {
            p.Add(x => x.Mask, "(99) 99999-9999");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderMask();
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithLabel()
    {
        var cut = RenderMask(p => p.Add(x => x.Label, "Phone"));
        Assert.Contains("Phone", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithPlaceholder()
    {
        var cut = RenderMask(p => p.Add(x => x.Placeholder, "Enter phone"));
        Assert.Contains("Enter phone", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithHelperText()
    {
        var cut = RenderMask(p => p.Add(x => x.HelperText, "Format: (XX) XXXXX-XXXX"));
        Assert.Contains("Format: (XX) XXXXX-XXXX", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithPrefixText()
    {
        var cut = RenderMask(p => p.Add(x => x.PrefixText, "+55"));
        Assert.Contains("+55", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithSuffixText()
    {
        var cut = RenderMask(p => p.Add(x => x.SuffixText, "ext"));
        Assert.Contains("ext", cut.Markup);
    }

    [Fact]
    public void Extended_ShowMaskPlaceholder_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.ShowMaskPlaceholder, true));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_CustomPlaceholderChar_Renders()
    {
        var cut = RenderMask(p => p
            .Add(x => x.ShowMaskPlaceholder, true)
            .Add(x => x.PlaceholderChar, '#'));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_UnmaskValueFalse_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.UnmaskValue, false));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_ClearIncomplete_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.ClearIncomplete, true));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_Clearable_RendersClearButton()
    {
        var cut = RenderMask(p => p
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, "11987654321"));
        Assert.Contains("Limpar", cut.Markup);
    }

    [Fact]
    public void Extended_CustomClearLabel_Renders()
    {
        var cut = RenderMask(p => p
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, "11987654321")
            .Add(x => x.ClearLabel, "Clear"));
        Assert.Contains("Clear", cut.Markup);
    }

    [Fact]
    public void Extended_MaskDescriptionFormat_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.MaskDescriptionFormat, "Phone: {0}"));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_AutoComplete_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.AutoComplete, "tel"));
        Assert.Contains("autocomplete=\"tel\"", cut.Markup);
    }

    [Fact]
    public void Extended_InputMode_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.InputMode, "numeric"));
        Assert.Contains("inputmode=\"numeric\"", cut.Markup);
    }

    [Fact]
    public void Extended_VariantFilled_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Variant, XFFormVariant.Soft));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_SizeSm_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_ToneDanger_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_ReadOnly_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.ReadOnly, true));
        Assert.Contains("readonly", cut.Markup);
    }

    [Fact]
    public void Extended_Required_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Required, true));
        Assert.Contains("required", cut.Markup);
    }

    [Fact]
    public void Extended_FullWidthFalse_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.FullWidth, false));
        Assert.DoesNotContain("w-full", cut.Markup);
    }

    [Fact]
    public void Extended_ImmediateFalse_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Immediate, false));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_StartIcon_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.StartIcon, (RenderFragment)(b => b.AddContent(0, "icon"))));
        Assert.Contains("icon", cut.Markup);
    }

    [Fact]
    public void Extended_EndIcon_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.EndIcon, (RenderFragment)(b => b.AddContent(0, "end-icon"))));
        Assert.Contains("end-icon", cut.Markup);
    }

    [Fact]
    public void Extended_CustomName_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.Name, "phone"));
        Assert.Contains("name=\"phone\"", cut.Markup);
    }

    [Fact]
    public void Extended_CustomAriaLabel_Renders()
    {
        var cut = RenderMask(p => p.Add(x => x.AriaLabel, "Phone number"));
        Assert.Contains("aria-label=\"Phone number\"", cut.Markup);
    }

    [Fact]
    public void Extended_EmptyMask_Renders()
    {
        var cut = Render<XFInputMask>();
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_CpfMask_Renders()
    {
        var cut = Render<XFInputMask>(p => p.Add(x => x.Mask, "999.999.999-99"));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_CnpjMask_Renders()
    {
        var cut = Render<XFInputMask>(p => p.Add(x => x.Mask, "99.999.999/9999-99"));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public void Extended_DateMask_Renders()
    {
        var cut = Render<XFInputMask>(p => p.Add(x => x.Mask, "99/99/9999"));
        Assert.Contains("xf-input-mask", cut.Markup);
    }

    [Fact]
    public async Task Extended_ValueChanged_Fires()
    {
        string? value = null;
        var cut = RenderMask(p => p.Add(
            x => x.ValueChanged,
            EventCallback.Factory.Create<string?>(this, v => value = v)));
        await cut.Find("input").ChangeAsync(new ChangeEventArgs { Value = "11999998888" });
        Assert.NotNull(value);
    }

    [Fact]
    public async Task Extended_Change_Fires()
    {
        string? value = null;
        var cut = RenderMask(p => p.Add(
            x => x.Change,
            EventCallback.Factory.Create<string?>(this, v => value = v)));
        await cut.Find("input").ChangeAsync(new ChangeEventArgs { Value = "11999998888" });
        Assert.NotNull(value);
    }
}
