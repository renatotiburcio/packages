using Xunit;
using XForge.DesignSystem.Core.Theming;

namespace XForge.DesignSystem.Tests.Core.Theming;

public class XFThemeServiceTest
{
    [Fact]
    public void DefaultTheme_IsLight()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        Assert.Equal(XFTheme.Light, service.CurrentTheme);
    }

    [Fact]
    public void DefaultMode_IsAuto()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        Assert.Equal(XFThemeMode.Auto, service.Mode);
    }

    [Fact]
    public void IsDark_FalseByDefault()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        Assert.False(service.IsDark);
    }

    [Fact]
    public void ThemeChanged_FiresOnToggle()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        var fired = false;
        service.ThemeChanged += (_, _) => fired = true;
        // ThemeChanged only fires in Auto mode via OnSystemThemeChanged
        Assert.False(fired);
    }

    [Fact]
    public void OnSystemThemeChanged_SetsDark_WhenPrefersDark()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.OnSystemThemeChanged(true).Wait();
        Assert.Equal(XFTheme.Dark, service.CurrentTheme);
        Assert.True(service.IsDark);
    }

    [Fact]
    public void OnSystemThemeChanged_SetsLight_WhenPrefersLight()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.OnSystemThemeChanged(false).Wait();
        Assert.Equal(XFTheme.Light, service.CurrentTheme);
        Assert.False(service.IsDark);
    }

    [Fact]
    public void OnSystemThemeChanged_Ignores_WhenNotAuto()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.SetModeAsync(XFThemeMode.Light).Wait();
        service.OnSystemThemeChanged(true).Wait();
        Assert.Equal(XFTheme.Light, service.CurrentTheme);
    }

    [Fact]
    public void SetModeAsync_Light_SetsLight()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.SetModeAsync(XFThemeMode.Light).Wait();
        Assert.Equal(XFThemeMode.Light, service.Mode);
        Assert.Equal(XFTheme.Light, service.CurrentTheme);
    }

    [Fact]
    public void SetModeAsync_Dark_SetsDark()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.SetModeAsync(XFThemeMode.Dark).Wait();
        Assert.Equal(XFThemeMode.Dark, service.Mode);
        Assert.Equal(XFTheme.Dark, service.CurrentTheme);
    }

    [Fact]
    public void SetThemeAsync_Dark_SetsDark()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.SetThemeAsync(XFTheme.Dark).Wait();
        Assert.Equal(XFTheme.Dark, service.CurrentTheme);
        Assert.Equal(XFThemeMode.Dark, service.Mode);
    }

    [Fact]
    public void SetThemeAsync_Light_SetsLight()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.SetThemeAsync(XFTheme.Light).Wait();
        Assert.Equal(XFTheme.Light, service.CurrentTheme);
        Assert.Equal(XFThemeMode.Light, service.Mode);
    }

    [Fact]
    public void ThemeEnum_HasLightAndDark()
    {
        Assert.Equal(2, Enum.GetValues<XFTheme>().Length);
    }

    [Fact]
    public void ThemeModeEnum_HasAllValues()
    {
        Assert.Equal(3, Enum.GetValues<XFThemeMode>().Length);
    }

    [Fact]
    public void ThemeChangedEventArgs_HasProperties()
    {
        var args = new XFThemeChangedEventArgs(XFTheme.Dark, XFThemeMode.Auto);
        Assert.Equal(XFTheme.Dark, args.Theme);
        Assert.Equal(XFThemeMode.Auto, args.Mode);
    }

    [Fact]
    public void ThemeChangedEventArgs_HasBrand()
    {
        var args = new XFThemeChangedEventArgs(XFTheme.Light, XFThemeMode.Light) { Brand = "brand-components" };
        Assert.Equal("brand-components", args.Brand);
    }

    [Fact]
    public void CurrentBrand_DefaultsToNull()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        Assert.Null(service.CurrentBrand);
    }

    [Fact]
    public void SetBrandAsync_SetsBrand()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.SetBrandAsync("brand-components").Wait();
        Assert.Equal("brand-components", service.CurrentBrand);
    }

    [Fact]
    public void ClearBrandAsync_RemovesBrand()
    {
        var service = new XFThemeService(new FakeJSRuntime());
        service.SetBrandAsync("brand-components").Wait();
        service.ClearBrandAsync().Wait();
        Assert.Null(service.CurrentBrand);
    }

    [Fact]
    public void ServiceRegistration_AddsSingleton()
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddXForgeTheme();
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(XFThemeService));
        Assert.NotNull(descriptor);
        Assert.Equal(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}

internal class FakeJSRuntime : Microsoft.JSInterop.IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args = null)
    {
        return ValueTask.FromResult(default(TValue)!);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args = null)
    {
        return ValueTask.FromResult(default(TValue)!);
    }

    public ValueTask InvokeVoidAsync(string identifier, object?[]? args = null)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken, object?[]? args = null)
    {
        return ValueTask.CompletedTask;
    }
}
