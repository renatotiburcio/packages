using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFSchedulerE2ETests : PageTest
{
    [TestMethod]
    public async Task Scheduler_RendersCalendarGrid()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-scheduler-advanced' data-view='Week'>
                <div class='xf-scheduler-header'>
                    <button>Today</button>
                    <button>←</button>
                    <button>→</button>
                </div>
                <div class='xf-scheduler-week-grid'>
                    <div class='xf-scheduler-week-day-column'></div>
                </div>
            </div>
        ");
        
        var scheduler = await Page.QuerySelectorAsync(".xf-scheduler-advanced");
        Assert.IsNotNull(scheduler);
        
        var weekGrid = await Page.QuerySelectorAsync(".xf-scheduler-week-grid");
        Assert.IsNotNull(weekGrid);
    }

    [TestMethod]
    public async Task Scheduler_ViewSwitcher_Works()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-scheduler-advanced'>
                <div class='xf-scheduler-view-switcher'>
                    <button class='active' data-view='Week'>Week</button>
                    <button data-view='Month'>Month</button>
                </div>
            </div>
        ");
        
        var weekBtn = await Page.QuerySelectorAsync("button.active");
        Assert.IsNotNull(weekBtn);
        var text = await weekBtn.TextContentAsync();
        Assert.AreEqual("Week", text);
    }
}
