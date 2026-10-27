using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFGanttE2ETests : PageTest
{
    [TestMethod]
    public async Task Gantt_RendersSVGChart()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-gantt-advanced'>
                <svg viewBox='0 0 1000 500'>
                    <rect x='0' y='0' width='100' height='20' fill='#3b82f6' />
                </svg>
            </div>
        ");
        
        var gantt = await Page.QuerySelectorAsync(".xf-gantt-advanced");
        Assert.IsNotNull(gantt);
        
        var svg = await Page.QuerySelectorAsync("svg");
        Assert.IsNotNull(svg);
    }

    [TestMethod]
    public async Task Gantt_ShowsToolbar()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-gantt-advanced'>
                <div class='xf-gantt-toolbar'>
                    <button>Add Task</button>
                </div>
            </div>
        ");
        
        var toolbar = await Page.QuerySelectorAsync(".xf-gantt-toolbar");
        Assert.IsNotNull(toolbar);
    }

    [TestMethod]
    public async Task Gantt_TreePanel_ShowsTaskNames()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-gantt-advanced'>
                <div class='xf-gantt-tree-panel'>
                    <div class='xf-gantt-tree-row'>Task 1</div>
                    <div class='xf-gantt-tree-row'>Task 2</div>
                </div>
            </div>
        ");
        
        var treePanel = await Page.QuerySelectorAsync(".xf-gantt-tree-panel");
        Assert.IsNotNull(treePanel);
        
        var rows = await Page.QuerySelectorAllAsync(".xf-gantt-tree-row");
        Assert.AreEqual(2, rows.Count);
    }
}
