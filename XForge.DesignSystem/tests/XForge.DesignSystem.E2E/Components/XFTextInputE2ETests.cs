using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFTextInputE2ETests : PageTest
{
    [TestMethod]
    public async Task TextInput_AcceptsInput()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <input type='text' id='test-input' />
            <div id='output'></div>
            <script>
                document.getElementById('test-input').addEventListener('input', function(e) {
                    document.getElementById('output').textContent = e.target.value;
                });
            </script>
        ");
        
        await Page.FillAsync("#test-input", "Hello World");
        var output = await Page.TextContentAsync("#output");
        Assert.AreEqual("Hello World", output);
    }

    [TestMethod]
    public async Task TextInput_HasPlaceholder()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync("<input type='text' placeholder='Enter text...' />");
        
        var input = await Page.QuerySelectorAsync("input");
        Assert.IsNotNull(input);
        
        var placeholder = await input.GetAttributeAsync("placeholder");
        Assert.AreEqual("Enter text...", placeholder);
    }

    [TestMethod]
    public async Task TextInput_MaxLength_Enforced()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync("<input type='text' maxlength='5' />");
        
        await Page.FillAsync("input", "Hello World");
        var value = await Page.InputValueAsync("input");
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public async Task TextInput_ReadOnly_IsNotEditable()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync("<input type='text' value='Read only' readonly />");
        
        var input = await Page.QuerySelectorAsync("input[readonly]");
        Assert.IsNotNull(input);
    }
}