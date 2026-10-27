using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFTileLayoutE2ETests : PageTest
{
    [TestMethod]
    public async Task TileLayout_RendersGrid()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-tile-layout' style='display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px;'>
                <div class='xf-tile'>Tile 1</div>
                <div class='xf-tile'>Tile 2</div>
                <div class='xf-tile'>Tile 3</div>
            </div>
        ");
        
        var layout = await Page.QuerySelectorAsync(".xf-tile-layout");
        Assert.IsNotNull(layout);
        
        var tiles = await Page.QuerySelectorAllAsync(".xf-tile");
        Assert.AreEqual(3, tiles.Count);
    }

    [TestMethod]
    public async Task TileLayout_Tiles_AreAccessible()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-tile-layout'>
                <div class='xf-tile' role='region' aria-label='Sales Chart'>
                    <h3>Sales</h3>
                </div>
            </div>
        ");
        
        var tile = await Page.QuerySelectorAsync("[role='region']");
        Assert.IsNotNull(tile);
        
        var label = await tile.GetAttributeAsync("aria-label");
        Assert.AreEqual("Sales Chart", label);
    }
}
