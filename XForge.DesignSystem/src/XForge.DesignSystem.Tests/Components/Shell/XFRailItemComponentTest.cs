using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Components.Shell;

public class XFRailItemComponentTest : XForgeTestContext
{
    public XFRailItemComponentTest()
    {
        Services.AddSingleton<NavigationStateService>();
        JSInterop.SetupVoid("xfTooltip.init", _ => true);
        JSInterop.SetupVoid("xfTooltip.update", _ => true);
        JSInterop.SetupVoid("xfTooltip.dispose", _ => true);
    }

    [Fact]
    public void Render_WithItemOnly_UsesItemLabelForAccessibleName()
    {
        var cut = Render<XFRailItem>(parameters =>
            parameters.Add(p => p.Item, new RailNavItem
            {
                Brand = "fiscal",
                Label = "Fiscal",
                Icon = "house"
            }));

        Assert.Contains("aria-label=\"Fiscal\"", cut.Markup);
    }

    [Fact]
    public void Render_WithoutLabel_UsesLocalizableFallbackAccessibleName()
    {
        var cut = Render<XFRailItem>(parameters =>
        {
            parameters.Add(p => p.Icon, "house");
            parameters.Add(p => p.UnlabeledItemAriaLabel, "Aplicativo sem rotulo");
        });

        Assert.Contains("aria-label=\"Aplicativo sem rotulo\"", cut.Markup);
    }
}
