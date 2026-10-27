// T050: Tests for XFExtensionRegistry.
using Xunit;
using XForge.DesignSystem.Extensions;

namespace XForge.DesignSystem.Tests.Extensions;

public class XFExtensionRegistryTests
{
    [Fact] public void XFCatalogedExtensions_CountIs38() => Assert.Equal(38, XFCatalogedExtensions.Count);

    [Fact] public void Register_IncrementsCount()
    {
        var r = new XFExtensionRegistry();
        r.Register(new XFDataGridExtension());
        Assert.Equal(1, r.RegisteredCount);
    }

    [Fact] public void Activate_TogglesActive()
    {
        var r = new XFExtensionRegistry();
        r.Register(new XFDataGridExtension());
        r.Activate("DataGrid", new ServiceProviderStub());
        Assert.True(r.IsActive("DataGrid"));
    }

    [Fact] public void Activate_UnknownId_Throws() => Assert.Throws<InvalidOperationException>(() => new XFExtensionRegistry().Activate("nope", new ServiceProviderStub()));

    [Fact] public void Activate_Idempotent()
    {
        var r = new XFExtensionRegistry();
        r.Register(new XFDataGridExtension());
        r.Activate("DataGrid", new ServiceProviderStub());
        r.Activate("DataGrid", new ServiceProviderStub());
        Assert.Single(r.ActiveIds());
    }

    [Fact] public void Deactivate_RemovesFromActive()
    {
        var r = new XFExtensionRegistry();
        r.Register(new XFDataGridExtension());
        r.Activate("DataGrid", new ServiceProviderStub());
        r.Deactivate("DataGrid");
        Assert.False(r.IsActive("DataGrid"));
    }

    private sealed class ServiceProviderStub : IServiceProvider { public object? GetService(Type serviceType) => null; }
}