#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.TextInput;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFTextInputComponentTest : Bunit.BunitContext
{
    [Fact]
    public void Render_WithLabelHelperAndPlaceholder()
    {
        string? value = "renato@example.com";
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Email");
            parameters.Add(p => p.HelperText, "Use your work email.");
            parameters.Add(p => p.Placeholder, "name@example.com");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("xf-text-input", cut.Markup);
        Assert.Contains("Email", cut.Markup);
        Assert.Contains("Use your work email.", cut.Markup);
        Assert.Contains("placeholder=\"name@example.com\"", cut.Markup);
        Assert.Contains("value=\"renato@example.com\"", cut.Markup);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        string? value = null;
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.Add(p => p.Variant, XFFormVariant.Filled);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Secondary);
            parameters.Add(p => p.Type, XFTextInputType.Search);
            parameters.Add(p => p.StartIcon, builder => builder.AddContent(0, "?"));
            parameters.Add(p => p.SuffixText, "BRL");
        });

        Assert.Contains("bg-surface-100", cut.Markup);
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("focus-within:border-[var(--color-secondary)]", cut.Markup);
        Assert.Contains("type=\"search\"", cut.Markup);
        Assert.Contains("BRL", cut.Markup);
    }

    [Fact]
    public async Task Change_WhenTrimEnabled_UpdatesTrimmedValue()
    {
        string? changed = null;
        string? value = null;
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Trim, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.Add(p => p.Change, EventCallback.Factory.Create<string?>(this, next => changed = next));
        });

        cut.Find("input").Change("  workspace  ");

        Assert.Equal("workspace", changed);
    }

    [Fact]
    public async Task Input_WhenImmediate_UpdatesOnInput()
    {
        string? changed = null;
        string? value = null;
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Immediate, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        cut.Find("input").Input("live");

        Assert.Equal("live", changed);
    }

    [Fact]
    public async Task Input_WhenDebounced_DelaysValueChanged()
    {
        string? changed = null;
        string? value = null;
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Immediate, true);
            parameters.Add(p => p.DebounceMilliseconds, 30);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        cut.Find("input").Input("live");

        Assert.Null(changed);

        await Task.Delay(60);

        Assert.Equal("live", changed);
    }

    [Fact]
    public async Task Clear_WhenClearable_ClearsValue()
    {
        string? changed = null;
        string? value = "filled";
        var cut = Render<XFTextInput>(parameters =>
        {
            parameters.Add(p => p.Clearable, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("aria-label=\"Limpar valor\"", cut.Markup);
        Assert.DoesNotContain("Clear value", cut.Markup);

        await cut.Find("button").ClickAsync();

        Assert.Equal(string.Empty, changed);
        Assert.DoesNotContain("button", cut.Markup);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new InputModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Name);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Name is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFTextInput>(3);
                childBuilder.AddAttribute(4, "Value", model.Name);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Name = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Name));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Name is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Render_DefaultUsesSoftPrimaryFocusTreatment()
    {
        var cut = Render<XFTextInput>();

        Assert.Contains("border-surface-200", cut.Markup);
        Assert.Contains("focus-within:border-[var(--color-primary)]", cut.Markup);
        Assert.Contains("focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Render_SoftVariantUsesPrimaryTokenForBorderAndRing()
    {
        var cut = Render<XFTextInput>(parameters =>
            parameters.Add(p => p.Variant, XFFormVariant.Soft));

        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]", cut.Markup);
    }

    private sealed class InputModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
