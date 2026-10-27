using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using XForge.DesignSystem.Components.Forms.Radio;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFRadioExtendedTest : BunitContext
{
    private static void AddRadio0(RenderTreeBuilder builder, string value, string label)
    {
        builder.OpenComponent<XFRadio<string>>(0);
        builder.AddAttribute(1, "Value", value);
        builder.AddAttribute(2, "Label", label);
        builder.CloseComponent();
    }

    private static void AddRadio10(RenderTreeBuilder builder, string value, string label)
    {
        builder.OpenComponent<XFRadio<string>>(10);
        builder.AddAttribute(11, "Value", value);
        builder.AddAttribute(12, "Label", label);
        builder.CloseComponent();
    }

    private IRenderedComponent<XFRadioGroup<string>> RenderRadioGroup(
        Action<ComponentParameterCollectionBuilder<XFRadioGroup<string>>>? configure = null)
    {
        string? value = "opt1";
        return Render<XFRadioGroup<string>>(p =>
        {
            p.Add(x => x.Label, "Options");
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueExpression, () => value);
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                AddRadio0(builder, "opt1", "Option 1");
                AddRadio10(builder, "opt2", "Option 2");
            }));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderRadioGroup();
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Label()
    {
        var cut = RenderRadioGroup();
        Assert.Contains("Options", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_RadiogroupRole()
    {
        var cut = RenderRadioGroup();
        Assert.Contains("role=\"radiogroup\"", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_RadioRoles()
    {
        var cut = RenderRadioGroup();
        Assert.Contains("role=\"radio\"", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_RadioLabels()
    {
        var cut = RenderRadioGroup();
        Assert.Contains("Option 1", cut.Markup);
        Assert.Contains("Option 2", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Selected_AriaChecked()
    {
        var cut = RenderRadioGroup();
        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Unselected_AriaCheckedFalse()
    {
        var cut = RenderRadioGroup();
        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    [Fact]
    public void Extended_Description()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Description, "Choose one"));
        Assert.Contains("Choose one", cut.Markup);
    }

    [Fact]
    public void Extended_Required()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Required, true));
        Assert.Contains("required", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Disabled, true));
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Extended_FullWidthFalse()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.FullWidth, false));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_SizeSm()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_ToneSecondary()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_ToneDanger()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_ToneSuccess()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_ToneWarning()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public void Extended_CustomName()
    {
        var cut = RenderRadioGroup(p => p.Add(x => x.Name, "my-radio"));
        Assert.Contains("xf-radio-group", cut.Markup);
    }

    [Fact]
    public async Task Extended_ValueChanged_Fires()
    {
        string? value = "opt1";
        string? changed = null;
        var cut = Render<XFRadioGroup<string>>(p =>
        {
            p.Add(x => x.Label, "Options");
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueExpression, () => value);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => changed = value = v));
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                AddRadio0(builder, "opt1", "Option 1");
                AddRadio10(builder, "opt2", "Option 2");
            }));
        });
        var radios = cut.FindAll("div[role=radio]");
        if (radios.Count > 1)
        {
            await radios[1].ClickAsync();
            await cut.InvokeAsync(() => { });
        }
        Assert.Equal("opt2", changed);
    }

    [Fact]
    public void Extended_XFRadioGroup_CanCreate()
    {
        var group = new XFRadioGroup<string>();
        Assert.NotNull(group);
    }

    [Fact]
    public void Extended_XFRadio_CanCreate()
    {
#pragma warning disable BL0005
        var radio = new XFRadio<string> { Value = "test", Label = "Test" };
#pragma warning restore BL0005
        Assert.Equal("test", radio.Value);
    }

    [Fact]
    public void Extended_XFRadioVariant_EnumHasAllValues()
    {
        Assert.Contains(XFRadioVariant.Default, Enum.GetValues<XFRadioVariant>());
        Assert.Contains(XFRadioVariant.Card, Enum.GetValues<XFRadioVariant>());
        Assert.Contains(XFRadioVariant.Button, Enum.GetValues<XFRadioVariant>());
    }

    [Fact]
    public void Extended_XFRadioOrientation_EnumHasAllValues()
    {
        Assert.Contains(XFRadioOrientation.Vertical, Enum.GetValues<XFRadioOrientation>());
        Assert.Contains(XFRadioOrientation.Horizontal, Enum.GetValues<XFRadioOrientation>());
    }

    // XFRadio individual component CSS class tests
    private IRenderedComponent<XFRadioGroup<string>> RenderRadioGroupWith(
        Action<ComponentParameterCollectionBuilder<XFRadioGroup<string>>>? groupConfig = null)
    {
        string? value = "opt1";
        return Render<XFRadioGroup<string>>(p =>
        {
            p.Add(x => x.Label, "Options");
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueExpression, () => value);
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFRadio<string>>(0);
                builder.AddAttribute(1, "Value", "opt1");
                builder.AddAttribute(2, "Label", "Option 1");
                builder.CloseComponent();

                builder.OpenComponent<XFRadio<string>>(10);
                builder.AddAttribute(11, "Value", "opt2");
                builder.AddAttribute(12, "Label", "Option 2");
                builder.CloseComponent();
            }));
            groupConfig?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_XFRadio_VariantButton_RendersButtonClass()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g => g.Add(x => x.Variant, XFRadioVariant.Button));
        var radios = cut.FindAll("div[role=radio]");
        Assert.NotEmpty(radios);
        var cls = radios[0].GetAttribute("class");
        Assert.Contains("items-center", cls);
        Assert.Contains("rounded-lg", cls);
        Assert.Contains("px-3", cls);
    }

    [Fact]
    public void Extended_XFRadio_VariantCard_SelectedSuccessTone()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g =>
            {
                g.Add(x => x.Variant, XFRadioVariant.Card);
                g.Add(x => x.Tone, XFSemanticColor.Success);
            });
        Assert.Contains("var(--color-success)", cut.Markup);
        Assert.Contains("ring-4", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_VariantCard_SelectedWarningTone()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g =>
            {
                g.Add(x => x.Variant, XFRadioVariant.Card);
                g.Add(x => x.Tone, XFSemanticColor.Warning);
            });
        Assert.Contains("var(--color-warning)", cut.Markup);
        Assert.Contains("ring-4", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_VariantCard_SelectedDangerTone()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g =>
            {
                g.Add(x => x.Variant, XFRadioVariant.Card);
                g.Add(x => x.Tone, XFSemanticColor.Danger);
            });
        Assert.Contains("var(--color-danger)", cut.Markup);
        Assert.Contains("ring-4", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_FormattedValue_NullValue()
    {
        string? groupValue = null;
        var cut = Render<XFRadioGroup<string?>>(parameters =>
        {
            parameters.Add(x => x.Label, "Options");
            parameters.Add(x => x.Value, groupValue);
            parameters.Add(x => x.ValueExpression, () => groupValue);
            parameters.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFRadio<string?>>(0);
                builder.AddAttribute(1, "Value", (string?)null);
                builder.AddAttribute(2, "Label", "Null option");
                builder.CloseComponent();
            }));
        });
        Assert.Contains("Null option", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_FormattedValue_IFormattable()
    {
        var groupValue = new DateTime(2025, 1, 15);
        var cut = Render<XFRadioGroup<DateTime>>(parameters =>
        {
            parameters.Add(x => x.Label, "Options");
            parameters.Add(x => x.Value, groupValue);
            parameters.Add(x => x.ValueExpression, () => groupValue);
            parameters.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFRadio<DateTime>>(0);
                builder.AddAttribute(1, "Value", new DateTime(2025, 1, 15));
                builder.AddAttribute(2, "Label", "Date option");
                builder.CloseComponent();
            }));
        });
        Assert.Contains("Date option", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_FormattedValue_NonFormattable()
    {
        int groupValue = 42;
        var cut = Render<XFRadioGroup<int>>(parameters =>
        {
            parameters.Add(x => x.Label, "Options");
            parameters.Add(x => x.Value, groupValue);
            parameters.Add(x => x.ValueExpression, () => groupValue);
            parameters.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFRadio<int>>(0);
                builder.AddAttribute(1, "Value", 42);
                builder.AddAttribute(2, "Label", "Number option");
                builder.CloseComponent();
            }));
        });
        Assert.Contains("Number option", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_ButtonClass_Unselected()
    {
        string? groupValue = "opt2";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(x => x.Label, "Options");
            parameters.Add(x => x.Value, groupValue);
            parameters.Add(x => x.ValueExpression, () => groupValue);
            parameters.Add(x => x.Variant, XFRadioVariant.Button);
            parameters.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFRadio<string>>(0);
                builder.AddAttribute(1, "Value", "opt1");
                builder.AddAttribute(2, "Label", "Option 1");
                builder.CloseComponent();
                builder.OpenComponent<XFRadio<string>>(10);
                builder.AddAttribute(11, "Value", "opt2");
                builder.AddAttribute(12, "Label", "Option 2");
                builder.CloseComponent();
            }));
        });
        var radios = cut.FindAll("div[role=radio]");
        // First radio (opt1) is unselected since group value is opt2
        var cls = radios[0].GetAttribute("class");
        Assert.Contains("border-surface-200", cls);
        Assert.Contains("bg-bg-surface", cls);
    }

    [Fact]
    public void Extended_XFRadio_ReadOnly_RendersDisabled()
    {
        string? groupValue = "opt1";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(x => x.Label, "Options");
            parameters.Add(x => x.Value, groupValue);
            parameters.Add(x => x.ValueExpression, () => groupValue);
            parameters.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFRadio<string>>(0);
                builder.AddAttribute(1, "Value", "opt1");
                builder.AddAttribute(2, "Label", "Option 1");
                builder.AddAttribute(3, "ReadOnly", true);
                builder.CloseComponent();
            }));
        });
        var div = cut.Find("div[role=radio]");
        Assert.Contains("cursor-not-allowed", div.GetAttribute("class"));
        Assert.Contains("opacity-60", div.GetAttribute("class"));
    }

    [Fact]
    public void Extended_XFRadio_Description_RendersText()
    {
        string? groupValue = "opt1";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(x => x.Label, "Options");
            parameters.Add(x => x.Value, groupValue);
            parameters.Add(x => x.ValueExpression, () => groupValue);
            parameters.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFRadio<string>>(0);
                builder.AddAttribute(1, "Value", "opt1");
                builder.AddAttribute(2, "Label", "Option 1");
                builder.AddAttribute(3, "Description", "Pick this one");
                builder.CloseComponent();
            }));
        });
        Assert.Contains("Pick this one", cut.Markup);
        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_SizeSm_RendersSmallRadio()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g => g.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_SizeLg_RendersLargeRadio()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g => g.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-[1.375rem] w-[1.375rem]", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_DotClass_SizeSm()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g => g.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-1.5 w-1.5", cut.Markup);
    }

    [Fact]
    public void Extended_XFRadio_DotClass_SizeLg()
    {
        var cut = RenderRadioGroupWith(
            groupConfig: g => g.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-2.5 w-2.5", cut.Markup);
    }
}
