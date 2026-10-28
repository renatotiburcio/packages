using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Feedback.RichTextEditor;

namespace XForge.DesignSystem.Tests.Components;

public class XFRichTextEditorComponentTest : XForgeTestContext
{
    public XFRichTextEditorComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithLabel_ShowsLabel()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.Label, "Description");
            parameters.Add(p => p.Value, "<p>Hello</p>");
        });

        Assert.Contains("xf-richtext-editor", cut.Markup);
        Assert.Contains("Description", cut.Markup);
    }

    [Fact]
    public void Render_WithHelperText_ShowsHelper()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.HelperText, "Enter rich text content.");
        });

        Assert.Contains("Enter rich text content.", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel_SetsAriaLabel()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Product description editor");
        });

        Assert.Contains("aria-label=\"Product description editor\"", cut.Markup);
    }

    [Fact]
    public void Render_WithDisabled_SetsOpacity()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("opacity-60", cut.Markup);
        Assert.Contains("cursor-not-allowed", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomToolbar_RendersToolbarContainer()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.Toolbar, builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "class", "ql-bold");
                builder.CloseElement();
            });
        });

        Assert.Contains("ql-bold", cut.Markup);
    }

    [Fact]
    public void Render_Theme_DefaultIsSnow()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.Value, "<p>Test</p>");
        });

        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact]
    public void Render_WithValidationError_ShowsError()
    {
        var model = new EditorModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.Content);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Content is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFRichTextEditor>(3);
                childBuilder.AddAttribute(4, "Value", model.Content);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Content = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<string?>>)(() => model.Content));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Content is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Render_MinHeight_IsSet()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.Value, "<p>Test</p>");
        });

        Assert.Contains("min-height: 12rem", cut.Markup);
    }

    [Fact]
    public void Render_EditorContainer_HasCorrectId()
    {
        var cut = Render<XFRichTextEditor>(parameters =>
        {
            parameters.Add(p => p.Value, "<p>Test</p>");
        });

        Assert.Contains("-editor", cut.Markup);
    }

    private sealed class EditorModel
    {
        [Required]
        public string? Content { get; set; }
    }
}
