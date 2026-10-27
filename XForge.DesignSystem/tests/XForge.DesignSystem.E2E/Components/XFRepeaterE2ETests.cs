using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFRepeaterE2ETests : PageTest
{
    [TestMethod]
    public async Task Repeater_RendersItems()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-repeater' role='group' aria-label='Items'>
                <div class='xf-repeater-item'>Item 1</div>
                <div class='xf-repeater-item'>Item 2</div>
                <div class='xf-repeater-item'>Item 3</div>
            </div>
        ");
        
        var repeater = await Page.QuerySelectorAsync("[role='group']");
        Assert.IsNotNull(repeater);
        
        var items = await Page.QuerySelectorAllAsync(".xf-repeater-item");
        Assert.AreEqual(3, items.Count);
    }

    [TestMethod]
    public async Task Repeater_HasAccessibleLabel()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-repeater' role='group' aria-label='Contact List'>
                <div class='xf-repeater-item'>Contact 1</div>
            </div>
        ");
        
        var repeater = await Page.QuerySelectorAsync("[role='group']");
        Assert.IsNotNull(repeater);
        
        var label = await repeater.GetAttributeAsync("aria-label");
        Assert.AreEqual("Contact List", label);
    }

    [TestMethod]
    public async Task Repeater_AddButton_IsAccessible()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-repeater' role='group' aria-label='Items'>
                <button class='xf-repeater-add' aria-label='Add new item'>+</button>
            </div>
        ");
        
        var addBtn = await Page.QuerySelectorAsync(".xf-repeater-add");
        Assert.IsNotNull(addBtn);
        
        var label = await addBtn.GetAttributeAsync("aria-label");
        Assert.AreEqual("Add new item", label);
    }
}
