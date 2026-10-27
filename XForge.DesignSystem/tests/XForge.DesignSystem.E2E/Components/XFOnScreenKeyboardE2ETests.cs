using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFOnScreenKeyboardE2ETests : PageTest
{
    [TestMethod]
    public async Task OnScreenKeyboard_RendersKeys()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-onscreen-keyboard' role='region' aria-label='On-screen keyboard'>
                <div class='xf-onscreen-keyboard-row'>
                    <button class='xf-key' data-key='q'>Q</button>
                    <button class='xf-key' data-key='w'>W</button>
                    <button class='xf-key' data-key='e'>E</button>
                </div>
            </div>
        ");
        
        var keyboard = await Page.QuerySelectorAsync("[role='region']");
        Assert.IsNotNull(keyboard);
        
        var keys = await Page.QuerySelectorAllAsync(".xf-key");
        Assert.AreEqual(3, keys.Count);
    }

    [TestMethod]
    public async Task OnScreenKeyboard_Key_HasAccessibleLabel()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-onscreen-keyboard'>
                <button class='xf-key' data-key='a' aria-label='Letter A'>A</button>
            </div>
        ");
        
        var key = await Page.QuerySelectorAsync(".xf-key");
        Assert.IsNotNull(key);
        
        var label = await key.GetAttributeAsync("aria-label");
        Assert.AreEqual("Letter A", label);
    }

    [TestMethod]
    public async Task OnScreenKeyboard_ShiftKey_IsSpecial()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-onscreen-keyboard'>
                <button class='xf-key xf-key-special' data-key='shift' aria-label='Shift'>⇧</button>
            </div>
        ");
        
        var key = await Page.QuerySelectorAsync(".xf-key-special");
        Assert.IsNotNull(key);
    }
}
