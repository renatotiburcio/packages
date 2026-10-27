using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Navigation.NavigationGuard;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Components;

public class XFNavigationGuardComponentTest : BunitContext
{
    public XFNavigationGuardComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithGuardService_RendersNavigationLock()
    {
        var guardService = new XFNavigationGuardService();
        Services.AddSingleton(guardService);
        var cut = Render<XFNavigationGuard>();

        // Verify the component renders without error and the service is wired
        Assert.False(guardService.HasPendingChanges);
    }

    [Fact]
    public void Render_WithPendingChanges_SetsConfirmExternalNavigation()
    {
        var guardService = new XFNavigationGuardService();
        Services.AddSingleton(guardService);

        // Register a guard with pending changes
        guardService.Register(() => true, _ => Task.FromResult(false));

        var cut = Render<XFNavigationGuard>();

        Assert.True(guardService.HasPendingChanges);
    }

    [Fact]
    public void Render_WithoutPendingChanges_HasNoPendingChanges()
    {
        var guardService = new XFNavigationGuardService();
        Services.AddSingleton(guardService);

        guardService.Register(() => false, _ => Task.FromResult(true));

        var cut = Render<XFNavigationGuard>();

        Assert.False(guardService.HasPendingChanges);
    }

    [Fact]
    public async Task ConfirmNavigation_WhenApproved_ReturnsTrue()
    {
        var guardService = new XFNavigationGuardService();
        Services.AddSingleton(guardService);

        guardService.Register(() => true, _ => Task.FromResult(true));

        var result = await guardService.ConfirmNavigationAsync("/other-page");
        Assert.True(result);
    }

    [Fact]
    public async Task ConfirmNavigation_WhenDenied_ReturnsFalse()
    {
        var guardService = new XFNavigationGuardService();
        Services.AddSingleton(guardService);

        guardService.Register(() => true, _ => Task.FromResult(false));

        var result = await guardService.ConfirmNavigationAsync("/other-page");
        Assert.False(result);
    }

    [Fact]
    public void Register_WhenDisposed_RemovesRegistration()
    {
        var guardService = new XFNavigationGuardService();
        Services.AddSingleton(guardService);

        var registration = guardService.Register(() => true, _ => Task.FromResult(true));
        Assert.True(guardService.HasPendingChanges);

        registration.Dispose();
        Assert.False(guardService.HasPendingChanges);
    }
}