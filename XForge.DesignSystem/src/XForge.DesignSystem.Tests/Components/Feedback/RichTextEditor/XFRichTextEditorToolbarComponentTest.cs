using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Feedback.RichTextEditor;

/// <summary>
/// Tests all 11 RichTextEditor toolbar components: the container and 10 button types.
/// </summary>
public class XFRichTextEditorToolbarComponentTest : XForgeTestContext
{
    public XFRichTextEditorToolbarComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    #region Toolbar Container

    [Fact]
    public void Toolbar_RendersContainer()
    {
        var cut = Render<XFRichTextEditorToolbar>();
        Assert.Contains("xf-toolbar", cut.Markup);
    }

    [Fact]
    public void Toolbar_RendersRoleToolbar()
    {
        var cut = Render<XFRichTextEditorToolbar>();
        Assert.Contains("role=\"toolbar\"", cut.Markup);
    }

    [Fact]
    public void Toolbar_RendersAriaLabel()
    {
        var cut = Render<XFRichTextEditorToolbar>();
        Assert.Contains("aria-label=\"Editor toolbar\"", cut.Markup);
    }

    [Fact]
    public void Toolbar_RendersChildContent()
    {
        var cut = Render<XFRichTextEditorToolbar>(p => p.AddChildContent("<button>Test</button>"));
        Assert.Contains("Test", cut.Markup);
    }

    [Fact]
    public void Toolbar_RendersMultipleChildren()
    {
        var cut = Render<XFRichTextEditorToolbar>(p => p.AddChildContent(b =>
        {
            b.OpenElement(0, "span");
            b.AddContent(1, "First");
            b.CloseElement();
            b.OpenElement(2, "span");
            b.AddContent(3, "Second");
            b.CloseElement();
        }));
        Assert.Contains("First", cut.Markup);
        Assert.Contains("Second", cut.Markup);
    }

    #endregion

    #region Bold

    [Fact]
    public void Bold_RendersButton()
    {
        var cut = Render<XFRichTextEditorBold>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
        Assert.Contains("<button", cut.Markup);
    }

    [Fact]
    public void Bold_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorBold>();
        Assert.Contains("title=\"Bold\"", cut.Markup);
    }

    [Fact]
    public void Bold_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorBold>();
        Assert.Contains(">B<", cut.Markup);
    }

    [Fact]
    public void Bold_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorBold>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Bold_WithCustomTitle_OverridesDefault()
    {
        var cut = Render<XFRichTextEditorBold>(p => p.Add(x => x.Title, "Custom Bold"));
        Assert.Contains("title=\"Custom Bold\"", cut.Markup);
    }

    [Fact]
    public void Bold_WithCustomIcon_OverridesDefault()
    {
        var cut = Render<XFRichTextEditorBold>(p => p.Add(x => x.Icon, "BOLD"));
        Assert.Contains("BOLD", cut.Markup);
    }

    [Fact]
    public void Bold_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorBold>(p => p.Add(x => x.Disabled, true));
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    #endregion

    #region Italic

    [Fact]
    public void Italic_RendersButton()
    {
        var cut = Render<XFRichTextEditorItalic>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void Italic_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorItalic>();
        Assert.Contains("title=\"Italic\"", cut.Markup);
    }

    [Fact]
    public void Italic_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorItalic>();
        Assert.Contains(">I<", cut.Markup);
    }

    [Fact]
    public void Italic_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorItalic>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Italic_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorItalic>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region Underline

    [Fact]
    public void Underline_RendersButton()
    {
        var cut = Render<XFRichTextEditorUnderline>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void Underline_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorUnderline>();
        Assert.Contains("title=\"Underline\"", cut.Markup);
    }

    [Fact]
    public void Underline_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorUnderline>();
        Assert.Contains(">U<", cut.Markup);
    }

    [Fact]
    public void Underline_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorUnderline>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Underline_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorUnderline>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region StrikeThrough

    [Fact]
    public void StrikeThrough_RendersButton()
    {
        var cut = Render<XFRichTextEditorStrikeThrough>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void StrikeThrough_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorStrikeThrough>();
        Assert.Contains("title=\"Strikethrough\"", cut.Markup);
    }

    [Fact]
    public void StrikeThrough_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorStrikeThrough>();
        Assert.Contains(">S<", cut.Markup);
    }

    [Fact]
    public void StrikeThrough_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorStrikeThrough>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void StrikeThrough_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorStrikeThrough>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region Link

    [Fact]
    public void Link_RendersButton()
    {
        var cut = Render<XFRichTextEditorLink>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void Link_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorLink>();
        Assert.Contains("title=\"Insert Link\"", cut.Markup);
    }

    [Fact]
    public void Link_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorLink>();
        Assert.Contains("\U0001F517", cut.Markup);
    }

    [Fact]
    public void Link_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorLink>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Link_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorLink>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region Unlink

    [Fact]
    public void Unlink_RendersButton()
    {
        var cut = Render<XFRichTextEditorUnlink>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void Unlink_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorUnlink>();
        Assert.Contains("title=\"Remove Link\"", cut.Markup);
    }

    [Fact]
    public void Unlink_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorUnlink>();
        Assert.Contains("\U0001F517", cut.Markup);
    }

    [Fact]
    public void Unlink_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorUnlink>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Unlink_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorUnlink>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region OrderedList

    [Fact]
    public void OrderedList_RendersButton()
    {
        var cut = Render<XFRichTextEditorOrderedList>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void OrderedList_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorOrderedList>();
        Assert.Contains("title=\"Ordered List\"", cut.Markup);
    }

    [Fact]
    public void OrderedList_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorOrderedList>();
        Assert.Contains(">1.<", cut.Markup);
    }

    [Fact]
    public void OrderedList_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorOrderedList>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void OrderedList_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorOrderedList>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region UnorderedList

    [Fact]
    public void UnorderedList_RendersButton()
    {
        var cut = Render<XFRichTextEditorUnorderedList>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void UnorderedList_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorUnorderedList>();
        Assert.Contains("title=\"Unordered List\"", cut.Markup);
    }

    [Fact]
    public void UnorderedList_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorUnorderedList>();
        Assert.Contains("•", cut.Markup);
    }

    [Fact]
    public void UnorderedList_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorUnorderedList>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void UnorderedList_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorUnorderedList>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region Undo

    [Fact]
    public void Undo_RendersButton()
    {
        var cut = Render<XFRichTextEditorUndo>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void Undo_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorUndo>();
        Assert.Contains("title=\"Undo\"", cut.Markup);
    }

    [Fact]
    public void Undo_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorUndo>();
        Assert.Contains("↩", cut.Markup);
    }

    [Fact]
    public void Undo_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorUndo>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Undo_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorUndo>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region Redo

    [Fact]
    public void Redo_RendersButton()
    {
        var cut = Render<XFRichTextEditorRedo>();
        Assert.Contains("xf-toolbar-btn", cut.Markup);
    }

    [Fact]
    public void Redo_HasCorrectTitle()
    {
        var cut = Render<XFRichTextEditorRedo>();
        Assert.Contains("title=\"Redo\"", cut.Markup);
    }

    [Fact]
    public void Redo_HasCorrectIcon()
    {
        var cut = Render<XFRichTextEditorRedo>();
        Assert.Contains("↪", cut.Markup);
    }

    [Fact]
    public void Redo_Click_FiresEvent()
    {
        var clicked = false;
        var cut = Render<XFRichTextEditorRedo>(p => p.Add(x => x.Click, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Redo_Disabled_SetsDisabledAttribute()
    {
        var cut = Render<XFRichTextEditorRedo>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    #endregion

    #region Toolbar with Buttons Integration

    [Fact]
    public void Toolbar_WithBoldButton_RendersBoth()
    {
        var cut = Render<XFRichTextEditorToolbar>(p => p.AddChildContent<XFRichTextEditorBold>());
        Assert.Contains("xf-toolbar", cut.Markup);
        Assert.Contains("xf-toolbar-btn", cut.Markup);
        Assert.Contains("title=\"Bold\"", cut.Markup);
    }

    [Fact]
    public void Toolbar_WithAllButtons_RendersAll()
    {
        var cut = Render<XFRichTextEditorToolbar>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<XFRichTextEditorBold>(0); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorItalic>(1); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorUnderline>(2); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorStrikeThrough>(3); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorLink>(4); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorUnlink>(5); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorOrderedList>(6); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorUnorderedList>(7); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorUndo>(8); b.CloseComponent();
            b.OpenComponent<XFRichTextEditorRedo>(9); b.CloseComponent();
        }));
        Assert.Contains("xf-toolbar", cut.Markup);
        Assert.Contains("title=\"Bold\"", cut.Markup);
        Assert.Contains("title=\"Italic\"", cut.Markup);
        Assert.Contains("title=\"Underline\"", cut.Markup);
        Assert.Contains("title=\"Strikethrough\"", cut.Markup);
        Assert.Contains("title=\"Insert Link\"", cut.Markup);
        Assert.Contains("title=\"Remove Link\"", cut.Markup);
        Assert.Contains("title=\"Ordered List\"", cut.Markup);
        Assert.Contains("title=\"Unordered List\"", cut.Markup);
        Assert.Contains("title=\"Undo\"", cut.Markup);
        Assert.Contains("title=\"Redo\"", cut.Markup);
    }

    #endregion

    #region Click without delegate (no-op)

    [Fact]
    public void Bold_ClickWithoutDelegate_DoesNotThrow()
    {
        var cut = Render<XFRichTextEditorBold>();
        cut.Find("button").Click();
        // Should not throw
    }

    [Fact]
    public void Undo_ClickWithoutDelegate_DoesNotThrow()
    {
        var cut = Render<XFRichTextEditorUndo>();
        cut.Find("button").Click();
    }

    [Fact]
    public void Redo_ClickWithoutDelegate_DoesNotThrow()
    {
        var cut = Render<XFRichTextEditorRedo>();
        cut.Find("button").Click();
    }

    #endregion
}
