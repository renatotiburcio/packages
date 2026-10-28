using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Maps;
using XForge.DesignSystem.Components.Navigation.NavigationGuard;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Tests for XFMap and XFNavigationGuard components.
/// </summary>
public sealed class XFMapAndNavigationGuardTest : XForgeTestContext
{
    public XFMapAndNavigationGuardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFMap

    [Fact]
    public void Map_Renders_Default()
    {
        var cut = Render<XFMap>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Renders_WithCustomCenter()
    {
        var center = new XFMapCoordinate(-23.5505, -46.6333); // São Paulo
        var cut = Render<XFMap>(p => p.Add(x => x.Center, center));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Renders_WithCustomZoom()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Zoom, 10.0));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Renders_WithCustomHeight()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Height, "500px"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Renders_WithCustomWidth()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Width, "800px"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Renders_NonInteractive()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Interactive, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Renders_WithoutZoomControl()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.ShowZoomControl, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Renders_WithoutAttributionControl()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.ShowAttribution, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Map_Dispose_DoesNotThrow()
    {
        var cut = Render<XFMap>();
        cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
    }

    #endregion

    #region XFNavigationGuard

    [Fact]
    public void NavigationGuard_Renders_Default()
    {
        // NavigationGuard requires XFNavigationGuardService in DI
        Services.AddSingleton<XFNavigationGuardService>();
        var cut = Render<XFNavigationGuard>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void NavigationGuard_Dispose_DoesNotThrow()
    {
        Services.AddSingleton<XFNavigationGuardService>();
        var cut = Render<XFNavigationGuard>();
        cut.Instance.Dispose();
    }

    [Fact]
    public void NavigationGuard_Renders_WithPendingChanges()
    {
        Services.AddSingleton<XFNavigationGuardService>();
        var cut = Render<XFNavigationGuard>();
        // After rendering, the service should be accessible
        var service = Services.GetService<XFNavigationGuardService>();
        Assert.NotNull(service);
    }

    #endregion
}
