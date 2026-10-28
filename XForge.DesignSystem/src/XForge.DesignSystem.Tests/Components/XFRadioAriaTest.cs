using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using XForge.DesignSystem.Components.Forms.Radio;

namespace XForge.DesignSystem.Tests.Components;

public class XFRadioAriaTest : XForgeTestContext
{
    [Fact]
    public void AriaChecked_SelectedRadio_IsTrue()
    {
        string? value = "yearly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
                AddRadio10(builder, "yearly", "Yearly");
            });
        });

        var radios = cut.FindAll("div[role='radio']");
        Assert.Equal("true", radios[1].GetAttribute("aria-checked"));
    }

    [Fact]
    public void AriaChecked_UnselectedRadio_IsFalse()
    {
        string? value = "yearly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
                AddRadio10(builder, "yearly", "Yearly");
            });
        });

        var radios = cut.FindAll("div[role='radio']");
        Assert.Equal("false", radios[0].GetAttribute("aria-checked"));
    }

    [Fact]
    public void Role_Radio_HasRadioRole()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
            });
        });

        Assert.Contains("role=\"radio\"", cut.Markup);
    }

    [Fact]
    public void Role_RadioGroup_HasRadiogroupRole()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
            });
        });

        Assert.Contains("role=\"radiogroup\"", cut.Markup);
    }

    [Fact]
    public void RadioGroup_WithLabel_UsesAriaLabelledBy()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Label, "Billing cycle");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
            });
        });

        var group = cut.Find("[role='radiogroup']");
        var labelledBy = group.GetAttribute("aria-labelledby");

        Assert.NotNull(labelledBy);
        Assert.Equal("Billing cycle", cut.Find($"#{labelledBy}").TextContent);
    }

    [Fact]
    public void RadioGroup_WithDescription_UsesAriaDescribedBy()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Description, "Choose exactly one billing cycle.");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
            });
        });

        var group = cut.Find("[role='radiogroup']");
        var describedBy = group.GetAttribute("aria-describedby");

        Assert.NotNull(describedBy);
        Assert.Equal("Choose exactly one billing cycle.", cut.Find($"#{describedBy}").TextContent);
    }

    [Fact]
    public void RadioGroup_RequiredAndDisabled_ExposeAriaState()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Required, true);
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
            });
        });

        var group = cut.Find("[role='radiogroup']");
        Assert.Equal("true", group.GetAttribute("aria-required"));
        Assert.Equal("true", group.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Radio_AriaDisabled_WhenDisabled()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
                builder.OpenComponent<XFRadio<string>>(10);
                builder.AddAttribute(11, "Value", "yearly");
                builder.AddAttribute(12, "Label", "Yearly");
                builder.AddAttribute(13, "Disabled", true);
                builder.CloseComponent();
            });
        });

        var radios = cut.FindAll("div[role='radio']");
        Assert.Equal("true", radios[1].GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Radio_TabIndex_ZeroWhenEnabled()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
            });
        });

        var radio = cut.Find("div[role='radio']");
        Assert.Equal("0", radio.GetAttribute("tabindex"));
    }

    [Fact]
    public void Radio_TabIndex_NegativeOneWhenDisabled()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFRadio<string>>(0);
                builder.AddAttribute(1, "Value", "monthly");
                builder.AddAttribute(2, "Label", "Monthly");
                builder.AddAttribute(3, "Disabled", true);
                builder.CloseComponent();
            });
        });

        var radio = cut.Find("div[role='radio']");
        Assert.Equal("-1", radio.GetAttribute("tabindex"));
    }

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
}
