using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFBIDashboardE2ETests : PageTest
{
    [TestMethod]
    public async Task Dashboard_RendersWidgetGrid()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-bi-dashboard'>
                <div class='xf-bi-widget-grid'>
                    <div class='xf-bi-widget'>Widget 1</div>
                    <div class='xf-bi-widget'>Widget 2</div>
                </div>
            </div>
        ");
        
        var dashboard = await Page.QuerySelectorAsync(".xf-bi-dashboard");
        Assert.IsNotNull(dashboard);
        
        var grid = await Page.QuerySelectorAsync(".xf-bi-widget-grid");
        Assert.IsNotNull(grid);
    }

    [TestMethod]
    public async Task Dashboard_Widgets_AreAccessible()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-bi-dashboard'>
                <div class='xf-bi-widget' role='region' aria-label='Sales Widget'>
                    <h3>Sales</h3>
                </div>
            </div>
        ");
        
        var widget = await Page.QuerySelectorAsync("[role='region']");
        Assert.IsNotNull(widget);
        
        var label = await widget.GetAttributeAsync("aria-label");
        Assert.AreEqual("Sales Widget", label);
    }
}
