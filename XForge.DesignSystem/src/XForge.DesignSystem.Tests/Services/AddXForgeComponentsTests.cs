using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Services;
using XForge.DesignSystem.Services.AI;
using Xunit;
namespace XForge.DesignSystem.Tests.Services;
public class AddXForgeComponentsTests
{
    [Fact] public void AddXForgeComponents_Registers_AIChatService() { var sc = new ServiceCollection(); sc.AddXForgeComponents(); var sp = sc.BuildServiceProvider(); Assert.IsType<AIChatService>(sp.GetService<IAIChatService>()); }
    [Fact] public void AddXForgeComponents_Returns_Collection_For_Chaining() { var sc = new ServiceCollection(); var result = sc.AddXForgeComponents(); Assert.Same(sc, result); }
    [Fact] public void AddXForgeComponents_With_Options_Registers_Options() { var sc = new ServiceCollection(); sc.AddXForgeComponents(o => o.MaxSessions = 50); var sp = sc.BuildServiceProvider(); var opts = sp.GetService<XForgeComponentsOptions>(); Assert.NotNull(opts); Assert.Equal(50, opts!.MaxSessions); }
    [Fact] public void Default_Options_EnableAiChat_Is_True() { var sc = new ServiceCollection(); sc.AddXForgeComponents(); var sp = sc.BuildServiceProvider(); var opts = sp.GetService<XForgeComponentsOptions>(); Assert.True(opts!.EnableAiChat); }
    [Fact] public void Default_Options_DebounceMs_Is_250() { var sc = new ServiceCollection(); sc.AddXForgeComponents(); var sp = sc.BuildServiceProvider(); var opts = sp.GetService<XForgeComponentsOptions>(); Assert.Equal(250, opts!.DebounceMs); }
}