#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.ColorInput;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFColorInputComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithLabelHelperAndValue()
    {
        string? value = "#38bdf8";
        var cut = Render<XFColorInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Brand color");
            parameters.Add(p => p.HelperText, "Used for previews and charts.");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("xf-color-input", cut.Markup);
        Assert.Contains("Brand color", cut.Markup);
        Assert.Contains("Used for previews and charts.", cut.Markup);
        Assert.Contains("type=\"color\"", cut.Markup);
        Assert.Contains("value=\"#38BDF8\"", cut.Markup);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFColorInput>(parameters =>
        {
            parameters.Add(p => p.Variant, XFFormVariant.Filled);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
            parameters.Add(p => p.PrefixText, "HEX");
            parameters.Add(p => p.SuffixText, "brand");
        });

        Assert.Contains("bg-surface-100", cut.Markup);
        Assert.Contains("min-h-12", cut.Markup);
        Assert.Contains("focus-within:border-success", cut.Markup);
        Assert.Contains("HEX", cut.Markup);
        Assert.Contains("brand", cut.Markup);
    }

    [Fact]
    public void Change_NormalizesHexValue()
    {
        string? changed = null;
        string? value = null;
        var cut = Render<XFColorInput>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
            parameters.Add(p => p.Change, EventCallback.Factory.Create<string?>(this, next => changed = next));
        });

        cut.Find("input[type=color]").Change("#22c55e");

        Assert.Equal("#22C55E", changed);
    }

    [Fact]
    public void Input_WhenImmediate_UpdatesOnInput()
    {
        string? changed = null;
        string? value = null;
        var cut = Render<XFColorInput>(parameters =>
        {
            parameters.Add(p => p.Immediate, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        cut.Find("input[type=color]").Input("#ef4444");

        Assert.Equal("#EF4444", changed);
    }

    [Fact]
    public async Task PaletteButton_SelectsColor()
    {
        string? changed = null;
        string? value = null;
        var cut = Render<XFColorInput>(parameters =>
        {
            parameters.Add(p => p.Palette, new[] { "#0ea5e9", "#f97316" });
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        await cut.Find("button[aria-label='Select #F97316']").ClickAsync();

        Assert.Equal("#F97316", changed);
        Assert.Contains("role=\"group\"", cut.Markup);
        Assert.Contains("aria-label=\"Color samples\"", cut.Markup);
        Assert.Contains("aria-label=\"Select #F97316\"", cut.Markup);
        Assert.DoesNotContain("Amostras de cor", cut.Markup);
        Assert.DoesNotContain("Selecionar #F97316", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredSwatchLabels()
    {
        var cut = Render<XFColorInput>(parameters =>
        {
            parameters.Add(p => p.Palette, new[] { "#0ea5e9" });
            parameters.Add(p => p.PaletteLabel, "Brand swatches");
            parameters.Add(p => p.SwatchLabelFormat, "Choose {0}");
            parameters.Add(p => p.ClearLabel, "Clear brand color");
            parameters.Add(p => p.Clearable, true);
            parameters.Add(p => p.Value, "#0ea5e9");
        });

        Assert.Contains("aria-label=\"Brand swatches\"", cut.Markup);
        Assert.Contains("aria-label=\"Choose #0EA5E9\"", cut.Markup);
        Assert.Contains("aria-label=\"Clear brand color\"", cut.Markup);
    }

    [Fact]
    public async Task Clear_WhenClearable_ClearsValue()
    {
        string? changed = "#111111";
        string? value = "#111111";
        var cut = Render<XFColorInput>(parameters =>
        {
            parameters.Add(p => p.Clearable, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("aria-label=\"Clear color\"", cut.Markup);
        Assert.DoesNotContain("Limpar cor", cut.Markup);

        await cut.Find("button").ClickAsync();

        Assert.Null(changed);
        Assert.DoesNotContain("button", cut.Markup);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new InputModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Color);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Color is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFColorInput>(3);
                childBuilder.AddAttribute(4, "Value", model.Color);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Color = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Color));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Color is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-danger/45", cut.Markup);
    }

    private sealed class InputModel
    {
        [Required]
        public string? Color { get; set; }
    }
}
