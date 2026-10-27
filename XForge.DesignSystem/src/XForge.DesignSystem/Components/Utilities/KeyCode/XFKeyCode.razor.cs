using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Utilities.KeyCode;

public partial class XFKeyCode : XForgeComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public IReadOnlyList<XFKeyCodeShortcut> Shortcuts { get; set; } = Array.Empty<XFKeyCodeShortcut>();
    [Parameter] public EventCallback<XFKeyCodeEventArgs> ShortcutInvoked { get; set; }
    [Parameter] public bool Focusable { get; set; } = true;
    [Parameter] public bool ShowHints { get; set; }
    [Parameter] public bool PreventDefault { get; set; }
    [Parameter] public bool StopPropagation { get; set; }
    [Parameter] public string? AriaLabel { get; set; } = "Area de atalhos de teclado";
    [Parameter] public string HintsLabel { get; set; } = "Atalhos disponiveis";
    [Parameter] public string ShortcutTextSeparator { get; set; } = "+";

    private IReadOnlyList<XFKeyCodeShortcut> EnabledShortcuts => Shortcuts.Where(shortcut => !shortcut.Disabled).ToArray();
    private string? TabIndex => Focusable ? "0" : null;
    private string? Role => Focusable ? "group" : null;

    private string RootClass => Join(
        "xf-key-code outline-none focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2",
        Class);

    private string HintsClass =>
        "mt-3 flex flex-wrap gap-2";

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (Disabled)
        {
            return;
        }

        var shortcut = EnabledShortcuts.FirstOrDefault(candidate => Matches(candidate, args));
        if (shortcut is null)
        {
            return;
        }

        if (ShortcutInvoked.HasDelegate)
        {
            await ShortcutInvoked.InvokeAsync(new XFKeyCodeEventArgs(shortcut, args));
        }
    }

    private static bool Matches(XFKeyCodeShortcut shortcut, KeyboardEventArgs args)
    {
        return string.Equals(NormalizeKey(shortcut.Key), NormalizeKey(args.Key), StringComparison.OrdinalIgnoreCase)
            && shortcut.Ctrl == args.CtrlKey
            && shortcut.Shift == args.ShiftKey
            && shortcut.Alt == args.AltKey
            && shortcut.Meta == args.MetaKey;
    }

    private static string NormalizeKey(string? key)
    {
        return key switch
        {
            null => string.Empty,
            " " => "Space",
            _ => key.Trim()
        };
    }

    private string GetShortcutText(XFKeyCodeShortcut shortcut)
    {
        var parts = new List<string>();
        if (shortcut.Ctrl) parts.Add("Ctrl");
        if (shortcut.Alt) parts.Add("Alt");
        if (shortcut.Shift) parts.Add("Shift");
        if (shortcut.Meta) parts.Add("Meta");
        parts.Add(NormalizeKey(shortcut.Key));
        return string.Join(ShortcutTextSeparator, parts);
    }

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
