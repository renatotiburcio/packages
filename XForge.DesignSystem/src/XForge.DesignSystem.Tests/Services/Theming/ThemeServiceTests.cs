using XForge.DesignSystem.Services.Theming;
using Xunit;

namespace XForge.DesignSystem.Tests.Services.Theming;

public class ThemeServiceTests
{
    [Fact]
    public void Enum_HasFourMembers()
    {
        Assert.Equal(0, (int)XTheme.Light);
        Assert.Equal(1, (int)XTheme.Dark);
        Assert.Equal(2, (int)XTheme.Auto);
        Assert.Equal(3, (int)XTheme.HighContrast);
    }

    [Fact]
    public void Static_Constructor_DefaultsToLight()
    {
        var svc = new StaticThemeService();
        Assert.Equal(XTheme.Light, svc.Current);
    }

    [Fact]
    public void Static_SetThemeAsync_ChangesTheme()
    {
        var svc = new StaticThemeService();
        svc.SetThemeAsync(XTheme.Dark);
        Assert.Equal(XTheme.Dark, svc.Current);
    }

    [Fact]
    public void Static_SetThemeAsync_SameTheme_DoesNotRaiseEvent()
    {
        var svc = new StaticThemeService();
        XTheme? raised = null;
        svc.OnThemeChanged += t => raised = t;
        svc.SetThemeAsync(XTheme.Light);
        Assert.Null(raised);
    }

    [Fact]
    public void Static_SetThemeAsync_DifferentTheme_RaisesEvent()
    {
        var svc = new StaticThemeService();
        XTheme? raised = null;
        svc.OnThemeChanged += t => raised = t;
        svc.SetThemeAsync(XTheme.Dark);
        Assert.Equal(XTheme.Dark, raised);
    }

    [Fact]
    public void Static_GetColors_Light_ReturnsLightColors()
    {
        var svc = new StaticThemeService();
        var c = svc.GetColors(XTheme.Light);
        Assert.Equal("#0066cc", c.Primary);
        Assert.Equal("#6c757d", c.Secondary);
        Assert.Equal("#ffffff", c.Background);
        Assert.Equal("#212529", c.Foreground);
        Assert.Equal("#0d6efd", c.Accent);
    }

    [Fact]
    public void Static_GetColors_Dark_ReturnsDarkColors()
    {
        var svc = new StaticThemeService();
        var c = svc.GetColors(XTheme.Dark);
        Assert.Equal("#3ea6ff", c.Primary);
        Assert.Equal("#adb5bd", c.Secondary);
        Assert.Equal("#121212", c.Background);
        Assert.Equal("#f8f9fa", c.Foreground);
        Assert.Equal("#0a84ff", c.Accent);
    }

    [Fact]
    public void Static_GetColors_HighContrast_ReturnsHighContrastColors()
    {
        var svc = new StaticThemeService();
        var c = svc.GetColors(XTheme.HighContrast);
        Assert.Equal("#ffff00", c.Primary);
        Assert.Equal("#ffffff", c.Secondary);
        Assert.Equal("#000000", c.Background);
        Assert.Equal("#ffffff", c.Foreground);
        Assert.Equal("#ffff00", c.Accent);
    }

    [Fact]
    public void Static_GetColors_Auto_FallsBackToLight()
    {
        var svc = new StaticThemeService();
        var c = svc.GetColors(XTheme.Auto);
        Assert.Equal("#0066cc", c.Primary);
    }

    [Fact]
    public void Static_AvailableThemes_ReturnsAllFour()
    {
        var svc = new StaticThemeService();
        Assert.Equal(
            new[] { XTheme.Light, XTheme.Dark, XTheme.Auto, XTheme.HighContrast },
            svc.AvailableThemes);
    }

    [Fact]
    public void Cascading_Constructor_DefaultsToLight()
    {
        var svc = new CascadingThemeService();
        Assert.Equal(XTheme.Light, svc.Current);
    }

    [Fact]
    public void Cascading_SetThemeAsync_ChangesThemeAndPropagatesToChildren()
    {
        var parent = new CascadingThemeService();
        var child = new StaticThemeService();
        parent.RegisterChild(child);
        parent.SetThemeAsync(XTheme.Dark);
        Assert.Equal(XTheme.Dark, parent.Current);
        Assert.Equal(XTheme.Dark, child.Current);
    }

    [Fact]
    public void Cascading_SetThemeAsync_RaisesEvent()
    {
        var svc = new CascadingThemeService();
        XTheme? raised = null;
        svc.OnThemeChanged += t => raised = t;
        svc.SetThemeAsync(XTheme.HighContrast);
        Assert.Equal(XTheme.HighContrast, raised);
    }

    [Fact]
    public void Cascading_RegisterChild_AddsChild()
    {
        var parent = new CascadingThemeService();
        var child = new StaticThemeService();
        parent.RegisterChild(child);
        parent.SetThemeAsync(XTheme.Dark);
        Assert.Equal(XTheme.Dark, child.Current);
    }

    [Fact]
    public async Task Cascading_SetThemeAsync_NoChildren_DoesNotThrow()
    {
        var svc = new CascadingThemeService();
        var ex = await Record.ExceptionAsync(() => svc.SetThemeAsync(XTheme.Dark));
        Assert.Null(ex);
        Assert.Equal(XTheme.Dark, svc.Current);
    }

    [Fact]
    public void Base_GetColors_AllThemes_ReturnDistinctColors()
    {
        var svc = new StaticThemeService();
        var light = svc.GetColors(XTheme.Light).Primary;
        var dark = svc.GetColors(XTheme.Dark).Primary;
        var hc = svc.GetColors(XTheme.HighContrast).Primary;
        Assert.NotEqual(light, dark);
        Assert.NotEqual(light, hc);
        Assert.NotEqual(dark, hc);
    }
}
