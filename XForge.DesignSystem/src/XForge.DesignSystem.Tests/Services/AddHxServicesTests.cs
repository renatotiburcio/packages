using System;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Services;
using Xunit;
namespace XForge.DesignSystem.Tests.Services;
public class AddHxServicesTests
{
    [Fact] public void AddHxServices_Registers_TimeProvider() { var sc = new ServiceCollection(); sc.AddHxServices(); var sp = sc.BuildServiceProvider(); Assert.NotNull(sp.GetService<TimeProvider>()); }
    [Fact] public void AddHxServices_Registers_Options() { var sc = new ServiceCollection(); sc.AddHxServices(); var sp = sc.BuildServiceProvider(); var opts = sp.GetService<HxServicesOptions>(); Assert.NotNull(opts); Assert.True(opts!.UseUtcClock); }
    [Fact] public void AddHxServices_Registers_MessageBoxService() { var sc = new ServiceCollection(); sc.AddHxServices(); var sp = sc.BuildServiceProvider(); Assert.IsType<HxMessageBoxService>(sp.GetService<IHxMessageBoxService>()); }
    [Fact] public void AddHxServices_Configures_Options() { var sc = new ServiceCollection(); sc.AddHxServices(o => o.UseUtcClock = false); var sp = sc.BuildServiceProvider(); var opts = sp.GetService<HxServicesOptions>(); Assert.False(opts!.UseUtcClock); }
    [Fact] public void AddHxServices_TimeProvider_Is_System_By_Default() { var sc = new ServiceCollection(); sc.AddHxServices(); var sp = sc.BuildServiceProvider(); Assert.Same(TimeProvider.System, sp.GetService<TimeProvider>()); }
}