using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Xunit;
using XForge.DesignSystem.Components.Forms.Radio;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFRadioComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithLabelDescriptionAndSelectedOption()
    {
        string? value = "yearly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Label, "Billing cycle");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
                AddRadio10(builder, "yearly", "Yearly");
            });
        });

        Assert.Contains("role=\"radiogroup\"", cut.Markup);
        Assert.Contains("Billing cycle", cut.Markup);
        Assert.Contains("Yearly", cut.Markup);
        Assert.Contains("aria-checked=\"true\"", cut.Markup);
        Assert.Contains("border-[var(--color-primary)]", cut.Markup);
    }

    [Fact]
    public void Change_WhenOptionSelected_UpdatesValue()
    {
        string? value = "monthly";
        string? changed = null;
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "monthly", "Monthly");
                AddRadio10(builder, "yearly", "Yearly");
            });
        });

        // Click the second radio div
        cut.FindAll("div[role='radio']")[1].Click();

        Assert.Equal("yearly", changed);
    }

    [Fact]
    public void Change_WhenSelectingLastOption_UpdatesValueAndRendersDot()
    {
        string? value = "medium";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "low", "Low");
                AddRadio10(builder, "medium", "Medium");
                AddRadio20(builder, "high", "High");
            });
        });

        Assert.Equal("medium", value);
        Assert.Single(cut.FindAll("span.h-2.w-2.rounded-full.bg-current"));

        cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, "high");
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "low", "Low");
                AddRadio10(builder, "medium", "Medium");
                AddRadio20(builder, "high", "High");
            });
        });

        Assert.Single(cut.FindAll("span.h-2.w-2.rounded-full.bg-current"));
    }

    [Fact]
    public void Render_CardVariantUsesSelectedSurfaceTreatment()
    {
        string? value = "complete";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.Add(p => p.Variant, XFRadioVariant.Card);
            parameters.Add(p => p.Tone, XFSemanticColor.Secondary);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "summary", "Summary");
                AddRadio10(builder, "complete", "Complete");
            });
        });

        Assert.Contains("rounded-lg border p-3", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-secondary)_45%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Render_WithValueFormatCulture_FormatsNativeRadioValues()
    {
        var cut = Render<XFRadioGroup<decimal>>(parameters =>
        {
            parameters.Add(p => p.ValueFormatCulture, CultureInfo.GetCultureInfo("pt-BR"));
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFRadio<decimal>>(0);
                builder.AddAttribute(1, "Value", 1.5m);
                builder.AddAttribute(2, "Label", "One and half");
                builder.CloseComponent();
            });
        });

        // No input element anymore, but the value should still render in the label
        Assert.Contains("One and half", cut.Markup);
    }

    [Fact]
    public void Change_WhenDisabled_DoesNotUpdateValue()
    {
        string? value = "monthly";
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
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

        cut.FindAll("div[role='radio']")[1].Click();

        Assert.Equal("monthly", value);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new RadioModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Plan);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Plan is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRadioGroup<string>>(3);
                childBuilder.AddAttribute(4, "Value", model.Plan);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Plan = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Plan));
                childBuilder.AddAttribute(7, "ChildContent", (RenderFragment)(radioBuilder =>
                {
                    AddRadio8(radioBuilder, "starter", "Starter");
                }));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Plan is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void SingleSelect_ClickingNewOption_DeselectsPrevious()
    {
        string? value = "opt1";
        string? changed = null;
        var cut = Render<XFRadioGroup<string>>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.AddChildContent(builder =>
            {
                AddRadio0(builder, "opt1", "Option 1");
                AddRadio10(builder, "opt2", "Option 2");
                AddRadio20(builder, "opt3", "Option 3");
            });
        });

        // Initially opt1 is selected
        Assert.Equal("opt1", value);

        // Verify initial render: opt1 checked, others unchecked
        var radios = cut.FindAll("div[role='radio']");
        Assert.Equal("true", radios[0].GetAttribute("aria-checked"));
        Assert.Equal("false", radios[1].GetAttribute("aria-checked"));
        Assert.Equal("false", radios[2].GetAttribute("aria-checked"));

        // Click opt2
        cut.FindAll("div[role='radio']")[1].Click();
        Assert.Equal("opt2", changed);
        Assert.Equal("opt2", value);

        // After clicking opt2: opt2 checked, opt1 and opt3 unchecked
        radios = cut.FindAll("div[role='radio']");
        Assert.False(radios[0].GetAttribute("aria-checked") == "true", "opt1 should be unchecked after clicking opt2");
        Assert.True(radios[1].GetAttribute("aria-checked") == "true", "opt2 should be checked after clicking it");
        Assert.False(radios[2].GetAttribute("aria-checked") == "true", "opt3 should remain unchecked");
    }

    private static void AddRadio0(RenderTreeBuilder builder, string value, string label)
    {
        builder.OpenComponent<XFRadio<string>>(0);
        builder.AddAttribute(1, "Value", value);
        builder.AddAttribute(2, "Label", label);
        builder.CloseComponent();
    }

    private static void AddRadio8(RenderTreeBuilder builder, string value, string label)
    {
        builder.OpenComponent<XFRadio<string>>(8);
        builder.AddAttribute(9, "Value", value);
        builder.AddAttribute(10, "Label", label);
        builder.CloseComponent();
    }

    private static void AddRadio10(RenderTreeBuilder builder, string value, string label)
    {
        builder.OpenComponent<XFRadio<string>>(10);
        builder.AddAttribute(11, "Value", value);
        builder.AddAttribute(12, "Label", label);
        builder.CloseComponent();
    }

    private static void AddRadio20(RenderTreeBuilder builder, string value, string label)
    {
        builder.OpenComponent<XFRadio<string>>(20);
        builder.AddAttribute(21, "Value", value);
        builder.AddAttribute(22, "Label", label);
        builder.CloseComponent();
    }

    private sealed class RadioModel
    {
        [Required]
        public string? Plan { get; set; }
    }
}
