using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.Textarea;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFTextareaComponentTest : BunitContext
{
    [Fact]
    public void Render_WithLabelHelperAndPlaceholder()
    {
        string? value = "Initial";
        var cut = Render<XFTextarea>(parameters =>
        {
            parameters.Add(p => p.Label, "Description");
            parameters.Add(p => p.HelperText, "Use plain language.");
            parameters.Add(p => p.Placeholder, "Describe...");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueExpression, () => value);
        });

        Assert.Contains("xf-textarea", cut.Markup);
        Assert.Contains("Description", cut.Markup);
        Assert.Contains("Use plain language.", cut.Markup);
        Assert.Contains("placeholder=\"Describe...\"", cut.Markup);
        Assert.Contains("Initial", cut.Markup);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFTextarea>(parameters =>
        {
            parameters.Add(p => p.Variant, XFFormVariant.Filled);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Resize, XFTextareaResize.None);
            parameters.Add(p => p.Rows, 6);
        });

        Assert.Contains("bg-surface-100", cut.Markup);
        Assert.Contains("p-4 text-base", cut.Markup);
        Assert.Contains("resize-none", cut.Markup);
        Assert.Contains("rows=\"6\"", cut.Markup);
    }

    [Fact]
    public void Change_WhenTrimEnabled_UpdatesTrimmedValue()
    {
        string? value = null;
        string? changed = null;
        var cut = Render<XFTextarea>(parameters =>
        {
            parameters.Add(p => p.Trim, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        cut.Find("textarea").Change("  content  ");

        Assert.Equal("content", changed);
    }

    [Fact]
    public void Input_WhenImmediate_UpdatesOnInput()
    {
        string? value = null;
        string? changed = null;
        var cut = Render<XFTextarea>(parameters =>
        {
            parameters.Add(p => p.Immediate, true);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, next => changed = value = next));
            parameters.Add(p => p.ValueExpression, () => value);
        });

        cut.Find("textarea").Input("live");

        Assert.Equal("live", changed);
    }

    [Fact]
    public void Render_WithCounter_ShowsCharacterCount()
    {
        var cut = Render<XFTextarea>(parameters =>
        {
            parameters.Add(p => p.ShowCount, true);
            parameters.Add(p => p.MaxLength, 20);
            parameters.Add(p => p.Value, "hello");
        });

        Assert.Contains("5/20", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomCounterFormats_UsesLocalizedCounterText()
    {
        var cut = Render<XFTextarea>(parameters =>
        {
            parameters.Add(p => p.ShowCount, true);
            parameters.Add(p => p.MaxLength, 20);
            parameters.Add(p => p.Value, "hello");
            parameters.Add(p => p.CharacterLimitFormat, "{0} of {1}");
        });

        Assert.Contains("5 of 20", cut.Markup);
        Assert.DoesNotContain("5/20", cut.Markup);

        var countOnly = Render<XFTextarea>(parameters =>
        {
            parameters.Add(p => p.ShowCount, true);
            parameters.Add(p => p.Value, "hello");
            parameters.Add(p => p.CharacterCountFormat, "{0} chars");
        });

        Assert.Contains("5 chars", countOnly.Markup);
    }

    [Fact]
    public void Render_SoftVariantUsesPrimaryTokenForBorderAndRing()
    {
        var cut = Render<XFTextarea>(parameters =>
            parameters.Add(p => p.Variant, XFFormVariant.Soft));

        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new TextareaModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Description);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Description is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFTextarea>(3);
                childBuilder.AddAttribute(4, "Value", model.Description);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Description = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Description));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Description is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    private sealed class TextareaModel
    {
        [Required]
        public string? Description { get; set; }
    }
}
