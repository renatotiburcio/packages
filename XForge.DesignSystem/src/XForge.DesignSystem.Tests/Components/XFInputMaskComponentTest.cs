using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.InputMask;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFInputMaskComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithLabelHelperAndMaskedValue()
    {
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Label, "Phone");
            parameters.Add(p => p.HelperText, "Use digits only.");
            parameters.Add(p => p.Mask, "(00) 00000-0000");
            parameters.Add(p => p.Value, "11987654321");
        });

        Assert.Contains("xf-input-mask", cut.Markup);
        Assert.Contains("Phone", cut.Markup);
        Assert.Contains("Use digits only.", cut.Markup);
        Assert.Contains("value=\"(11) 98765-4321\"", cut.Markup);
        Assert.Contains("maxlength=\"15\"", cut.Markup);

        var input = cut.Find("input");
        Assert.Equal("false", input.GetAttribute("aria-required"));
        Assert.Equal("false", input.GetAttribute("aria-readonly"));
        Assert.Contains(input.GetAttribute("aria-describedby")!, cut.Markup);
    }

    [Fact]
    public void Input_WhenUnmaskValue_UpdatesWithRawValue()
    {
        string? value = null;
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Mask, "000.000.000-00");
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        cut.Find("input").Input("12345678900");

        Assert.Equal("12345678900", value);
        Assert.Contains("value=\"123.456.789-00\"", cut.Markup);
    }

    [Fact]
    public void Input_WhenMaskedValue_UpdatesWithFormattedValue()
    {
        string? value = null;
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Mask, "AAA-0000");
            parameters.Add(p => p.UnmaskValue, false);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        cut.Find("input").Input("abc2026");

        Assert.Equal("abc-2026", value);
    }

    [Fact]
    public void Change_WhenClearIncomplete_ClearsValue()
    {
        string? value = "123";
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Mask, "000-000");
            parameters.Add(p => p.ClearIncomplete, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        cut.Find("input").Change("123");

        Assert.Equal(string.Empty, value);
    }

    [Fact]
    public void Input_WhenClearIncomplete_DoesNotClearUntilBlur()
    {
        string? value = null;
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Mask, "000-000");
            parameters.Add(p => p.ClearIncomplete, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        cut.Find("input").Input("55");

        Assert.Equal("55", value);
        Assert.Contains("value=\"55\"", cut.Markup);

        cut.Find("input").Blur();

        Assert.Equal(string.Empty, value);
        Assert.Contains("value=\"\"", cut.Markup);
    }

    [Fact]
    public void Click_WhenClearable_ClearsValue()
    {
        string? value = "123456";
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Mask, "000-000");
            parameters.Add(p => p.Clearable, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        cut.Find("button").Click();

        Assert.Equal(string.Empty, value);
    }

    [Fact]
    public void Render_WithPlaceholderMaskAndVisualOptions()
    {
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Mask, "000-AAA");
            parameters.Add(p => p.ShowMaskPlaceholder, true);
            parameters.Add(p => p.PlaceholderChar, '•');
            parameters.Add(p => p.Variant, XFFormVariant.Soft);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
        });

        Assert.Contains("placeholder=\"•••-•••\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("focus-within:border-[var(--color-warning)]", cut.Markup);
    }

    [Fact]
    public void Render_WithMaskDescriptionFormat_ExposesLocalizedMaskDescription()
    {
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.HelperText, "Use digits only.");
            parameters.Add(p => p.Mask, "000-000");
            parameters.Add(p => p.MaskDescriptionFormat, "Expected format {0}");
        });

        var input = cut.Find("input");
        var describedBy = input.GetAttribute("aria-describedby");

        Assert.NotNull(describedBy);
        Assert.Contains("-helper", describedBy);
        Assert.Contains("-mask-description", describedBy);
        Assert.Contains("Expected format 000-000", cut.Markup);
    }

    [Fact]
    public void Input_WithPlaceholderMask_DoesNotInjectPlaceholderIntoValue()
    {
        string? value = null;
        var cut = Render<XFInputMask>(parameters =>
        {
            parameters.Add(p => p.Mask, "000.000.000-00");
            parameters.Add(p => p.ShowMaskPlaceholder, true);
            parameters.Add(p => p.PlaceholderChar, '*');
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
        });

        cut.Find("input").Input("16");

        Assert.Equal("16", value);
        Assert.Contains("value=\"16\"", cut.Markup);
        Assert.DoesNotContain("16*.***.***-**", cut.Markup);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new MaskModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Document);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Document is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFInputMask>(3);
                childBuilder.AddAttribute(4, "Value", model.Document);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Document = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Document));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Document is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    private sealed class MaskModel
    {
        [Required]
        public string? Document { get; set; }
    }
}
