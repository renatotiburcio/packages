using System;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Services;
using Xunit;

namespace XForge.DesignSystem.Tests.Services;

// AddHxServices e um shim legado obsoleto: virou no-op na migracao para
// AddXForgeComponents. Os testes abaixo asseveram o contrato atual do shim
// (devolve a mesma colecao, nao lanca, e emite aviso de obsolencia) em vez do
// comportamento de registro que foi removido de proposito.
public class AddHxServicesTests
{
    [Fact]
    public void AddHxServices_ReturnsSameCollection_ForChaining()
    {
        var sc = new ServiceCollection();

#pragma warning disable CS0618 // AddHxServices e obsoleto por design
        var result = sc.AddHxServices();
#pragma warning restore CS0618

        Assert.Same(sc, result);
    }

    [Fact]
    public void AddHxServices_DoesNotThrow_WhenCalledWithConfigure()
    {
        var sc = new ServiceCollection();

#pragma warning disable CS0618
        var exception = Record.Exception(() => sc.AddHxServices(o => o.UseUtcClock = false));
#pragma warning restore CS0618

        Assert.Null(exception);
    }

    [Fact]
    public void AddHxServices_IsMarkedObsolete()
    {
        var method = typeof(HxServiceCollectionExtensions).GetMethod("AddHxServices");

        Assert.NotNull(method);
        var attribute = method!.GetCustomAttributes(typeof(ObsoleteAttribute), false);

        Assert.NotEmpty(attribute);
    }

    [Fact]
    public void AddXForgeComponents_RegistersWhatLegacyHxServicesUsedTo()
    {
        // O caminho suportado registra TimeProvider e opcoes.
        var sc = new ServiceCollection();
        sc.AddXForgeComponents();
        var sp = sc.BuildServiceProvider();

        Assert.NotNull(sp.GetService<TimeProvider>());
        Assert.Same(TimeProvider.System, sp.GetService<TimeProvider>());
        Assert.NotNull(sp.GetService<XForgeComponentsOptions>());
    }

    [Fact]
    public void AddXForgeComponents_Configures_Options_ThroughOverload()
    {
        var sc = new ServiceCollection();
        sc.AddXForgeComponents(o => o.DebounceMs = 50);
        var sp = sc.BuildServiceProvider();

        Assert.Equal(50, sp.GetService<XForgeComponentsOptions>()!.DebounceMs);
    }
}
