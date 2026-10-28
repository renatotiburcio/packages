using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Navigation.NavigationGuard;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFNavigationGuardTest : XForgeTestContext
{
    public XFNavigationGuardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<XFNavigationGuardService>();
    }

    [Fact] public void NavGuard_Renders_Default()
    {
        var cut = Render<XFNavigationGuard>();
        Assert.NotNull(cut);
    }
}
