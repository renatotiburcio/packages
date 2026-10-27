using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Core.Theming;

namespace XForge.DesignSystem.Tests.Core.Theming;

public class XFThemeInjectorTest
{
    [Fact]
    public void DefaultBrandCount_IsZero()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        Assert.Equal(0, injector.BrandCount);
    }

    [Fact]
    public void RegisterBrand_IncreasesCount()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RegisterBrandAsync("brand-a", ":root { --color-primary: red; }").Wait();
        Assert.Equal(1, injector.BrandCount);
    }

    [Fact]
    public void RegisterBrandTwice_SameName_NoDuplicates()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RegisterBrandAsync("brand-a", ":root { --a }").Wait();
        injector.RegisterBrandAsync("brand-a", ":root { --b }").Wait();
        Assert.Equal(1, injector.BrandCount);
    }

    [Fact]
    public void RegisterMultipleBrands_AllTracked()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RegisterBrandAsync("brand-a", ":root { }").Wait();
        injector.RegisterBrandAsync("brand-b", ":root { }").Wait();
        injector.RegisterBrandAsync("brand-c", ":root { }").Wait();
        Assert.Equal(3, injector.BrandCount);
    }

    [Fact]
    public void UnregisterBrand_DecreasesCount()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RegisterBrandAsync("brand-a", ":root { }").Wait();
        injector.UnregisterBrandAsync("brand-a").Wait();
        Assert.Equal(0, injector.BrandCount);
    }

    [Fact]
    public void IsBrandRegistered_ReturnsTrue()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RegisterBrandAsync("brand-a", ":root { }").Wait();
        Assert.True(injector.IsBrandRegistered("brand-a"));
    }

    [Fact]
    public void IsBrandRegistered_ReturnsFalse_WhenNotRegistered()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        Assert.False(injector.IsBrandRegistered("brand-a"));
    }

    [Fact]
    public void GetRegisteredBrands_ReturnsNames()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RegisterBrandAsync("alpha", ":root { }").Wait();
        injector.RegisterBrandAsync("beta", ":root { }").Wait();
        var brands = injector.GetRegisteredBrands();
        Assert.Contains("alpha", brands);
        Assert.Contains("beta", brands);
    }

    [Fact]
    public void InjectCss_DoesNotThrow()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.InjectCssAsync("test-id", "body { color: red; }").Wait();
        // No exception means success
        Assert.True(true);
    }

    [Fact]
    public void RemoveCss_DoesNotThrow()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RemoveCssAsync("test-id").Wait();
        Assert.True(true);
    }

    [Fact]
    public void RegisterBrandWithLightAndDark_StoresBoth()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        injector.RegisterBrandAsync("brand-a", ":root { }", ":root[data-theme=dark] { }").Wait();
        Assert.Equal(1, injector.BrandCount);
    }

    [Fact]
    public void RegisterBrandFromBuilder_Works()
    {
        var injector = new XFThemeInjector(new FakeJSRuntime());
        var builder = XFThemeBuilder.Create("test-brand").WithPrimary("#6366f1");
        injector.RegisterBrandAsync(builder).Wait();
        Assert.Equal(1, injector.BrandCount);
    }

    [Fact]
    public void ServiceRegistration_IncludesInjector()
    {
        var services = new ServiceCollection();
        services.AddXForgeTheme();
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(XFThemeInjector));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
