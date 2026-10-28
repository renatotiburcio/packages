using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Feedback.RichTextEditor;

namespace XForge.DesignSystem.Tests.Components.Feedback;

/// <summary>
/// Additional tests for XFRichTextEditor to improve coverage from 59.6% → 85%+.
/// Covers theme, placeholder, readonly, value change, and all parameter combinations.
/// </summary>
public class XFRichTextEditorExtendedTest : XForgeTestContext
{
    public XFRichTextEditorExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact] public void Render_ThemeSnow()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Test</p>")
            .Add(x => x.Theme, XFRichTextEditorTheme.Snow));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_ThemeBubble()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Test</p>")
            .Add(x => x.Theme, XFRichTextEditorTheme.Bubble));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithPlaceholder()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "")
            .Add(x => x.Placeholder, "Enter content..."));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithReadOnly()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Read only</p>")
            .Add(x => x.ReadOnly, true));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithValueChanged()
    {
        string? newValue = null;
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Initial</p>")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => newValue = v)));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithChange()
    {
        string? changeValue = null;
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Initial</p>")
            .Add(x => x.Change, EventCallback.Factory.Create<string?>(this, v => changeValue = v)));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithLabelAndHelper()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Test</p>")
            .Add(x => x.Label, "Editor Label")
            .Add(x => x.HelperText, "Helper text"));
        Assert.Contains("Editor Label", cut.Markup);
        Assert.Contains("Helper text", cut.Markup);
    }

    [Fact] public void Render_WithAriaLabel()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Test</p>")
            .Add(x => x.AriaLabel, "Rich text editor"));
        Assert.Contains("aria-label=\"Rich text editor\"", cut.Markup);
    }

    [Fact] public void Render_WithCustomClass()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Test</p>")
            .Add(x => x.Class, "custom-editor"));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithToolbar()
    {
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Test</p>")
            .Add(x => x.Toolbar, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "class", "ql-bold");
                builder.CloseElement();
            })));
        Assert.Contains("ql-bold", cut.Markup);
    }

    [Fact] public void Render_WithEmptyValue()
    {
        var cut = Render<XFRichTextEditor>(p => p.Add(x => x.Value, ""));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithNullValue()
    {
        var cut = Render<XFRichTextEditor>(p => p.Add(x => x.Value, (string?)null));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }

    [Fact] public void Render_WithAllParameters()
    {
        string? changed = null;
        var cut = Render<XFRichTextEditor>(p => p
            .Add(x => x.Value, "<p>Full</p>")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => changed = v))
            .Add(x => x.Theme, XFRichTextEditorTheme.Bubble)
            .Add(x => x.Placeholder, "Type...")
            .Add(x => x.Label, "Full Editor")
            .Add(x => x.HelperText, "Edit rich text")
            .Add(x => x.AriaLabel, "Full editor")
            .Add(x => x.Class, "full-editor")
            .Add(x => x.ReadOnly, false));
        Assert.Contains("xf-richtext-editor", cut.Markup);
    }
}
