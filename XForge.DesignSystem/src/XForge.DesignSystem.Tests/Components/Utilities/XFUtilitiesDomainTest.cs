using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Utilities.KeyCode;
using XForge.DesignSystem.Components.Utilities.DragDrop;

namespace XForge.DesignSystem.Tests.Components.Utilities;

public sealed class XFUtilitiesDomainTest : XForgeTestContext
{
    public XFUtilitiesDomainTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void KeyCode_Renders() => Render<XFKeyCode>();
    [Fact] public void KeyCode_WithShortcuts() { var shortcuts = new List<XFKeyCodeShortcut> { new() { Key = "s" } }; Render<XFKeyCode>(p => p.Add(x => x.Shortcuts, shortcuts)); }
    [Fact] public void KeyCode_FocusableFalse() => Render<XFKeyCode>(p => p.Add(x => x.Focusable, false));
    [Fact] public void KeyCode_ShowHints() => Render<XFKeyCode>(p => p.Add(x => x.ShowHints, true));
    [Fact] public void DragDropZone_Renders() => Render<XFDragDropZone<string>>();
    [Fact] public void DragDropZone_WithchildContent() => Render<XFDragDropZone<string>>(p => p.Add(x => x.ChildContent, builder => builder.AddMarkupContent(0, "<div>Drop here</div>")));
    [Fact] public void DragDropZone_AcceptMultiple() => Render<XFDragDropZone<string>>(p => p.Add(x => x.RejectUnknownTypes, true));
    [Fact] public void DragDropZone_FalseValue() => Render<XFDragDropZone<bool>>();
    [Fact] public void DragDropZone_EnumValues() => Assert.Equal(3, Enum.GetValues<XFDragDropState>().Length);
    [Fact] public void KeyCodeShortcut_Created() => Assert.Equal("s", new XFKeyCodeShortcut { Key = "s" }.Key);
    [Fact] public void KeyCodeShortcut_DefaultDisabled() => Assert.False(new XFKeyCodeShortcut().Disabled);
    [Fact] public void KeyCodeShortcut_SetDisabled() => Assert.True(new XFKeyCodeShortcut { Disabled = true }.Disabled);
    [Fact] public void DragDropState_Idle() => Assert.Equal(XFDragDropState.Idle, XFDragDropState.Idle);
    [Fact] public void DragDropState_DragOver() => Assert.Equal(XFDragDropState.DragOver, XFDragDropState.DragOver);
    [Fact] public void DragDropState_Rejected() => Assert.Equal(XFDragDropState.Rejected, XFDragDropState.Rejected);
}
