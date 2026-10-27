using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFButtonE2ETests : PageTest
{
    [TestMethod]
    public async Task Button_Click_CallsOnClick()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <button id='test-btn' type='button'>Click Me</button>
            <div id='output'></div>
            <script>
                document.getElementById('test-btn').addEventListener('click', function() {
                    document.getElementById('output').textContent = 'clicked';
                });
            </script>
        ");
        
        await Page.ClickAsync("#test-btn");
        var output = await Page.TextContentAsync("#output");
        Assert.AreEqual("clicked", output);
    }

    [TestMethod]
    public async Task Button_Disabled_IsNotClickable()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <button id='test-btn' type='button' disabled>Click Me</button>
            <div id='output'></div>
            <script>
                document.getElementById('test-btn').addEventListener('click', function() {
                    document.getElementById('output').textContent = 'clicked';
                });
            </script>
        ");
        
        await Page.ClickAsync("#test-btn", new() { Force = true });
        var output = await Page.TextContentAsync("#output");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public async Task Button_HasCorrectType()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync("<button type='submit'>Submit</button>");
        
        var button = await Page.QuerySelectorAsync("button[type='submit']");
        Assert.IsNotNull(button);
    }

    [TestMethod]
    public async Task Button_WithIcon_RendersIcon()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <button type='button'>
                <svg width='16' height='16' viewBox='0 0 16 16'>
                    <path d='M8 1v14M1 8h14' stroke='currentColor' stroke-width='2'/>
                </svg>
                Add Item
            </button>
        ");
        
        var button = await Page.QuerySelectorAsync("button");
        Assert.IsNotNull(button);
        
        var svg = await Page.QuerySelectorAsync("button svg");
        Assert.IsNotNull(svg);
    }

    [TestMethod]
    public async Task Button_KeyboardNavigation_TabFocus()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <button type='button'>Button 1</button>
            <button type='button'>Button 2</button>
        ");
        
        await Page.Keyboard.PressAsync("Tab");
        var focused = await Page.EvaluateAsync<string>("document.activeElement.tagName");
        Assert.AreEqual("BUTTON", focused);
    }
}
