using Microsoft.AspNetCore.Components.Web;

namespace XForge.DesignSystem.Components.Utilities.KeyCode;

public sealed class XFKeyCodeEventArgs
{
    public XFKeyCodeEventArgs(XFKeyCodeShortcut shortcut, KeyboardEventArgs keyboardEventArgs)
    {
        Shortcut = shortcut;
        KeyboardEventArgs = keyboardEventArgs;
    }

    public XFKeyCodeShortcut Shortcut { get; }
    public KeyboardEventArgs KeyboardEventArgs { get; }
}
