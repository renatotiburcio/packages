#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Services;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFRailItemComponentTest : XForgeTestContext
{
    public XFRailItemComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<NavigationStateService>();
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFRailItem>();
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Render_WithLabel()
    {
        var cut = Render<XFRailItem>(parameters =>
        {
            parameters.Add(p => p.Label, "Dashboard");
        });
        Assert.Contains("Dashboard", cut.Markup);
    }

    [Fact]
    public void Render_WithIcon()
    {
        var cut = Render<XFRailItem>(parameters =>
        {
            parameters.Add(p => p.Icon, "cube");
        });
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Render_WithActiveState()
    {
        var cut = Render<XFRailItem>(parameters =>
        {
            parameters.Add(p => p.Active, true);
        });
        Assert.Contains("page", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFRailItem>(parameters =>
        {
            parameters.Add(p => p.ShowTooltip, false);
            parameters.AddChildContent("Custom item");
        });
        Assert.Contains("Custom item", cut.Markup);
    }
}