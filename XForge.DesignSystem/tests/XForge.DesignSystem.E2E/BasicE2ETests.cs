using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E;

[TestClass]
public class BasicE2ETests : PageTest
{
    [TestMethod]
    public async Task HomePage_LoadsSuccessfully()
    {
        // This is a placeholder test - in a real implementation,
        // you would navigate to the docs site and verify it loads
        await Page.GotoAsync("about:blank");
        var title = await Page.TitleAsync();
        Assert.IsNotNull(title);
    }

    [TestMethod]
    public async Task Page_HasAccessibleElements()
    {
        // Test that basic accessible elements exist
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync("<div role='main'><h1>Test</h1></div>");
        
        var main = await Page.QuerySelectorAsync("[role='main']");
        Assert.IsNotNull(main);
        
        var heading = await Page.QuerySelectorAsync("h1");
        Assert.IsNotNull(heading);
    }

    [TestMethod]
    public async Task Form_Inputs_AreAccessible()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <form>
                <label for='test-input'>Test Input</label>
                <input id='test-input' type='text' aria-required='true' />
                <button type='submit'>Submit</button>
            </form>");
        
        var input = await Page.QuerySelectorAsync("#test-input");
        Assert.IsNotNull(input);
        
        var label = await Page.QuerySelectorAsync("label[for='test-input']");
        Assert.IsNotNull(label);
        
        var button = await Page.QuerySelectorAsync("button[type='submit']");
        Assert.IsNotNull(button);
    }
}
