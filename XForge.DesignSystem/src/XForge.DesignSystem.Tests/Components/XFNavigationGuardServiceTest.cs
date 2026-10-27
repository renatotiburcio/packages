using Xunit;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Tests for XFNavigationGuardService — the service behind navigation guard functionality.
/// Tests registration, pending changes detection, and confirmation flow.
/// </summary>
public class XFNavigationGuardServiceTest
{
    [Fact]
    public void HasPendingChanges_NoRegistrations_ReturnsFalse()
    {
        var service = new XFNavigationGuardService();
        Assert.False(service.HasPendingChanges);
    }

    [Fact]
    public void HasPendingChanges_WithPendingRegistration_ReturnsTrue()
    {
        var service = new XFNavigationGuardService();
        service.Register(() => true, _ => Task.FromResult(true));

        Assert.True(service.HasPendingChanges);
    }

    [Fact]
    public void HasPendingChanges_WithNoPendingRegistration_ReturnsFalse()
    {
        var service = new XFNavigationGuardService();
        service.Register(() => false, _ => Task.FromResult(true));

        Assert.False(service.HasPendingChanges);
    }

    [Fact]
    public void Register_ReturnsDisposable()
    {
        var service = new XFNavigationGuardService();
        var registration = service.Register(() => true, _ => Task.FromResult(true));

        Assert.NotNull(registration);
        Assert.IsAssignableFrom<IDisposable>(registration);
    }

    [Fact]
    public void Dispose_RemovesRegistration()
    {
        var service = new XFNavigationGuardService();
        var registration = service.Register(() => true, _ => Task.FromResult(true));

        Assert.True(service.HasPendingChanges);

        registration.Dispose();

        Assert.False(service.HasPendingChanges);
    }

    [Fact]
    public void Dispose_MultipleRegistrations_OnlyRemovesOne()
    {
        var service = new XFNavigationGuardService();
        var reg1 = service.Register(() => true, _ => Task.FromResult(true));
        var reg2 = service.Register(() => true, _ => Task.FromResult(true));

        reg1.Dispose();

        Assert.True(service.HasPendingChanges);

        reg2.Dispose();

        Assert.False(service.HasPendingChanges);
    }

    [Fact]
    public async Task ConfirmNavigationAsync_NoPendingChanges_ReturnsTrue()
    {
        var service = new XFNavigationGuardService();
        var result = await service.ConfirmNavigationAsync("/target");

        Assert.True(result);
    }

    [Fact]
    public async Task ConfirmNavigationAsync_WithApproved_ReturnsTrue()
    {
        var service = new XFNavigationGuardService();
        service.Register(() => true, _ => Task.FromResult(true));

        var result = await service.ConfirmNavigationAsync("/target");

        Assert.True(result);
    }

    [Fact]
    public async Task ConfirmNavigationAsync_WithDenied_ReturnsFalse()
    {
        var service = new XFNavigationGuardService();
        service.Register(() => true, _ => Task.FromResult(false));

        var result = await service.ConfirmNavigationAsync("/target");

        Assert.False(result);
    }

    [Fact]
    public async Task ConfirmNavigationAsync_PassesTargetUriToCallback()
    {
        var service = new XFNavigationGuardService();
        string? receivedUri = null;
        service.Register(() => true, uri =>
        {
            receivedUri = uri;
            return Task.FromResult(true);
        });

        await service.ConfirmNavigationAsync("/test-path");

        Assert.Equal("/test-path", receivedUri);
    }

    [Fact]
    public async Task ConfirmNavigationAsync_MultipleRegistrations_AllMustApprove()
    {
        var service = new XFNavigationGuardService();
        service.Register(() => true, _ => Task.FromResult(true));
        service.Register(() => true, _ => Task.FromResult(true));

        var result = await service.ConfirmNavigationAsync("/target");

        Assert.True(result);
    }

    [Fact]
    public async Task ConfirmNavigationAsync_MultipleRegistrations_OneDenies_ReturnsFalse()
    {
        var service = new XFNavigationGuardService();
        service.Register(() => true, _ => Task.FromResult(true));
        service.Register(() => true, _ => Task.FromResult(false));

        var result = await service.ConfirmNavigationAsync("/target");

        Assert.False(result);
    }

    [Fact]
    public async Task ConfirmNavigationAsync_SkipsRegistrationsWithoutPendingChanges()
    {
        var service = new XFNavigationGuardService();
        var callbackInvoked = false;
        service.Register(() => false, _ =>
        {
            callbackInvoked = true;
            return Task.FromResult(false);
        });

        var result = await service.ConfirmNavigationAsync("/target");

        Assert.True(result);
        Assert.False(callbackInvoked);
    }

    [Fact]
    public void Dispose_DoubleDispose_DoesNotThrow()
    {
        var service = new XFNavigationGuardService();
        var registration = service.Register(() => true, _ => Task.FromResult(true));

        registration.Dispose();
        var exception = Record.Exception(() => registration.Dispose());

        Assert.Null(exception);
    }
}
