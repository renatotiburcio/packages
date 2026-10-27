using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Utilities.KeyCode;

namespace XForge.DesignSystem.Tests.Components;

public class XFKeyCodeComponentTest : BunitContext
{
    [Fact]
    public void Render_FocusableAreaAndHints()
    {
        var cut = Render<XFKeyCode>(parameters =>
        {
            parameters.Add(p => p.Shortcuts, CreateShortcuts());
            parameters.Add(p => p.ShowHints, true);
            parameters.AddChildContent("Editor");
        });

        Assert.Contains("tabindex=\"0\"", cut.Markup);
        Assert.Contains("aria-label=\"Area de atalhos de teclado\"", cut.Markup);
        Assert.Contains("Ctrl+s", cut.Markup);
        Assert.Contains("Editor", cut.Markup);
    }

    [Fact]
    public void Render_WithShortcutTextSeparator_UsesLocalizedSeparator()
    {
        var cut = Render<XFKeyCode>(parameters =>
        {
            parameters.Add(p => p.Shortcuts, CreateShortcuts());
            parameters.Add(p => p.ShowHints, true);
            parameters.Add(p => p.ShortcutTextSeparator, " + ");
        });

        Assert.Contains("Ctrl + s", cut.Markup);
        Assert.Contains("Atalhos disponiveis", cut.Markup);
    }

    [Fact]
    public async Task KeyDown_MatchingShortcutRaisesCallback()
    {
        XFKeyCodeEventArgs? invoked = null;
        var cut = Render<XFKeyCode>(parameters =>
        {
            parameters.Add(p => p.Shortcuts, CreateShortcuts());
            parameters.Add(p => p.ShortcutInvoked, args => invoked = args);
        });

        await cut.Find(".xf-key-code").KeyDownAsync(new KeyboardEventArgs { Key = "s", CtrlKey = true });

        Assert.NotNull(invoked);
        Assert.Equal("save", invoked!.Shortcut.Id);
    }

    [Fact]
    public async Task KeyDown_DifferentModifiersDoesNotRaise()
    {
        var called = false;
        var cut = Render<XFKeyCode>(parameters =>
        {
            parameters.Add(p => p.Shortcuts, CreateShortcuts());
            parameters.Add(p => p.ShortcutInvoked, _ => called = true);
        });

        await cut.Find(".xf-key-code").KeyDownAsync(new KeyboardEventArgs { Key = "s" });

        Assert.False(called);
    }

    [Fact]
    public async Task KeyDown_DisabledShortcutDoesNotRaise()
    {
        var called = false;
        var cut = Render<XFKeyCode>(parameters =>
        {
            parameters.Add(p => p.Shortcuts, new[] { new XFKeyCodeShortcut { Id = "blocked", Key = "b", Disabled = true } });
            parameters.Add(p => p.ShortcutInvoked, _ => called = true);
        });

        await cut.Find(".xf-key-code").KeyDownAsync(new KeyboardEventArgs { Key = "b" });

        Assert.False(called);
    }

    private static IReadOnlyList<XFKeyCodeShortcut> CreateShortcuts() => new[]
    {
        new XFKeyCodeShortcut { Id = "save", Key = "s", Ctrl = true, Label = "Salvar" },
        new XFKeyCodeShortcut { Id = "cancel", Key = "Escape", Label = "Cancelar" },
    };
}
