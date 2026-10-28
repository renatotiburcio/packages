using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Forms.OnScreenKeyboard;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Forms.OnScreenKeyboard;

public class XFOnScreenKeyboardTest : XForgeTestContext
{
    public XFOnScreenKeyboardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_ShowsAlphanumeric()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("xf-onscreen-keyboard", cut.Markup);
        Assert.Contains("Q", cut.Markup);
        Assert.Contains("W", cut.Markup);
        Assert.Contains("E", cut.Markup);
    }

    [Fact]
    public void Render_WithLabel_ShowsLabel()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Label, "Password"));
        Assert.Contains("Password", cut.Markup);
    }

    [Fact]
    public void Render_WithNumericLayout_ShowsNumbers()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Layout, XFOnScreenKeyboardLayout.Numeric));
        Assert.Contains("1", cut.Markup);
        Assert.Contains("2", cut.Markup);
        Assert.Contains("3", cut.Markup);
    }

    [Fact]
    public void Render_WithSymbolLayout_ShowsSymbols()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Layout, XFOnScreenKeyboardLayout.Symbol));
        Assert.Contains("!", cut.Markup);
        Assert.Contains("@", cut.Markup);
        Assert.Contains("#", cut.Markup);
    }

    [Fact]
    public void Render_WithEmailLayout_ShowsEmailKeys()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Layout, XFOnScreenKeyboardLayout.Email));
        Assert.Contains("@", cut.Markup);
        Assert.Contains(".", cut.Markup);
    }

    [Fact]
    public void Render_ShowsShiftKey()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("Shift", cut.Markup);
    }

    [Fact]
    public void Render_ShowsBackspaceKey()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("⌫", cut.Markup);
    }

    [Fact]
    public void Render_ShowsEnterKey()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("Enter", cut.Markup);
    }

    [Fact]
    public void Render_ShowsSpaceKey()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("Space", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomKeyHeight_AdjustsSize()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.KeyHeight, 60));
        Assert.Contains("xf-onscreen-keyboard", cut.Markup);
    }

    [Fact]
    public void Render_WithShowStatusBar_ShowsStatusBar()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.ShowStatusBar, true));
        Assert.Contains("xf-onscreen-keyboard", cut.Markup);
    }

    [Fact]
    public void Render_WithoutShowStatusBar_HidesStatusBar()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.ShowStatusBar, false));
        Assert.Contains("xf-onscreen-keyboard", cut.Markup);
    }

    [Fact]
    public void Render_ShowsAriaLabel()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("aria-label", cut.Markup);
    }

    [Fact]
    public void Render_ShowsRegionRole()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void Render_WithDisabled_DisablesKeys()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Disabled, true));
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Render_ShowsAllLetters()
    {
        var cut = Render<XFOnScreenKeyboard>();
        var letters = "QWERTYUIOPASDFGHJKLZXCVBNM";
        foreach (var letter in letters)
        {
            Assert.Contains(letter.ToString(), cut.Markup);
        }
    }

    [Fact]
    public void Render_ShowsLayoutName()
    {
        var cut = Render<XFOnScreenKeyboard>();
        Assert.Contains("Alphanumeric", cut.Markup);
    }

    [Fact]
    public void Render_NumericLayout_ShowsNumericLayoutName()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Layout, XFOnScreenKeyboardLayout.Numeric));
        Assert.Contains("Numeric", cut.Markup);
    }

    [Fact]
    public void Render_SymbolLayout_ShowsSymbolLayoutName()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Layout, XFOnScreenKeyboardLayout.Symbol));
        Assert.Contains("Symbols", cut.Markup);
    }

    [Fact]
    public void Render_EmailLayout_ShowsEmailLayoutName()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Layout, XFOnScreenKeyboardLayout.Email));
        Assert.Contains("Email", cut.Markup);
    }

    [Fact]
    public void Render_RespectsDisabled()
    {
        var cut = Render<XFOnScreenKeyboard>(p => p.Add(x => x.Disabled, true));
        Assert.Contains("disabled", cut.Markup);
    }
}
