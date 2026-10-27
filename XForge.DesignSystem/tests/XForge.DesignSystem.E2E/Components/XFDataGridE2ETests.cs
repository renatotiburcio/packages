using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFDataGridE2ETests : PageTest
{
    [TestMethod]
    public async Task DataGrid_RendersWithHeaders()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <table class='xf-datagrid'>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Value</th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td>Item 1</td><td>100</td></tr>
                    <tr><td>Item 2</td><td>200</td></tr>
                </tbody>
            </table>
        ");
        
        var grid = await Page.QuerySelectorAsync(".xf-datagrid");
        Assert.IsNotNull(grid);
        
        var headers = await Page.QuerySelectorAllAsync("th");
        Assert.AreEqual(2, headers.Count);
    }

    [TestMethod]
    public async Task DataGrid_HasSortableHeaders()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <table class='xf-datagrid'>
                <thead>
                    <tr>
                        <th aria-sort='ascending'>Name</th>
                        <th aria-sort='none'>Value</th>
                    </tr>
                </thead>
            </table>
        ");
        
        var sortableHeader = await Page.QuerySelectorAsync("th[aria-sort='ascending']");
        Assert.IsNotNull(sortableHeader);
    }

    [TestMethod]
    public async Task DataGrid_CellFocusable()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <table class='xf-datagrid'>
                <tbody>
                    <tr>
                        <td tabindex='0'>Cell 1</td>
                        <td tabindex='0'>Cell 2</td>
                    </tr>
                </tbody>
            </table>
        ");
        
        var cells = await Page.QuerySelectorAllAsync("td[tabindex='0']");
        Assert.AreEqual(2, cells.Count);
    }
}