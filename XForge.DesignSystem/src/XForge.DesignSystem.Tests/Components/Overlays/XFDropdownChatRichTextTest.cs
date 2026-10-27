using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Components.Feedback.Chat;
using XForge.DesignSystem.Components.Feedback.RichTextEditor;
using XForge.DesignSystem.Components.Overlays.ContextMenu;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Overlays;

public sealed class XFDropdownChatRichTextTest : BunitContext
{
    public XFDropdownChatRichTextTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void DropdownMenu_Renders() => Render<XFDropdownMenu>();
    [Fact] public void DropdownMenu_SizeSm() => Render<XFDropdownMenu>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void DropdownMenu_CloseOnSelect() => Render<XFDropdownMenu>(p => p.Add(x => x.CloseOnSelect, false));
    [Fact] public void DropdownMenu_CloseOnEscape() => Render<XFDropdownMenu>(p => p.Add(x => x.CloseOnEscape, false));
    [Fact] public void DropdownMenu_CloseOnOutsideClick() => Render<XFDropdownMenu>(p => p.Add(x => x.CloseOnOutsideClick, false));
    [Fact] public void Chat_Renders() => Render<XFChat>();
    [Fact] public void Chat_WithMessages() { var msgs = new List<XFChatMessage> { new() { Content = "Hello" } }; Render<XFChat>(p => p.Add(x => x.Messages, msgs)); }
    [Fact] public void Chat_CurrentUserId() => Render<XFChat>(p => p.Add(x => x.CurrentUserId, "u1"));
    [Fact] public void Chat_EmptyText() => Render<XFChat>(p => p.Add(x => x.EmptyText, "No messages"));
    [Fact] public void RichTextEditor_Renders() => Render<XFRichTextEditor>();
    [Fact] public void RichTextEditor_ReadOnly() => Render<XFRichTextEditor>(p => p.Add(x => x.ReadOnly, true));
    [Fact] public void RichTextEditor_ThemeSnow() => Render<XFRichTextEditor>(p => p.Add(x => x.Theme, XFRichTextEditorTheme.Snow));
    [Fact] public void RichTextEditor_ThemeBubble() => Render<XFRichTextEditor>(p => p.Add(x => x.Theme, XFRichTextEditorTheme.Bubble));
    [Fact] public void ContextMenu_Renders() => Render<XFContextMenu>();
    [Fact] public void ContextMenu_Open() => Render<XFContextMenu>(p => p.Add(x => x.Open, true));
}
