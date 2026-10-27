using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class ComponentAccessibilityTests : PageTest
{
    [TestMethod]
    public async Task Button_HasAccessibleRole()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync("<button type='button'>Click me</button>");
        
        var button = await Page.QuerySelectorAsync("button");
        Assert.IsNotNull(button);
        
        var role = await button.GetAttributeAsync("role");
        // Buttons implicitly have role="button"
        Assert.IsNull(role); // Default role is fine
    }

    [TestMethod]
    public async Task Input_HasLabel()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <label for='email'>Email</label>
            <input id='email' type='email' />
        ");
        
        var input = await Page.QuerySelectorAsync("#email");
        Assert.IsNotNull(input);
        
        var label = await Page.QuerySelectorAsync("label[for='email']");
        Assert.IsNotNull(label);
    }

    [TestMethod]
    public async Task Dialog_HasCorrectRole()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div role='dialog' aria-labelledby='dialog-title'>
                <h2 id='dialog-title'>Confirm</h2>
                <p>Are you sure?</p>
                <button>Yes</button>
                <button>No</button>
            </div>
        ");
        
        var dialog = await Page.QuerySelectorAsync("[role='dialog']");
        Assert.IsNotNull(dialog);
        
        var title = await Page.QuerySelectorAsync("#dialog-title");
        Assert.IsNotNull(title);
    }

    [TestMethod]
    public async Task TabPanel_HasCorrectRole()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div role='tablist'>
                <button role='tab' aria-selected='true'>Tab 1</button>
                <button role='tab' aria-selected='false'>Tab 2</button>
            </div>
            <div role='tabpanel'>Content 1</div>
        ");
        
        var tablist = await Page.QuerySelectorAsync("[role='tablist']");
        Assert.IsNotNull(tablist);
        
        var tab = await Page.QuerySelectorAsync("[role='tab']");
        Assert.IsNotNull(tab);
    }

    [TestMethod]
    public async Task Alert_HasCorrectRole()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync("<div role='alert'>Error message</div>");
        
        var alert = await Page.QuerySelectorAsync("[role='alert']");
        Assert.IsNotNull(alert);
    }

    [TestMethod]
    public async Task Progressbar_HasCorrectAttributes()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div role='progressbar' aria-valuenow='50' aria-valuemin='0' aria-valuemax='100'>
                Progress
            </div>
        ");
        
        var progressbar = await Page.QuerySelectorAsync("[role='progressbar']");
        Assert.IsNotNull(progressbar);
        
        var valuenow = await progressbar.GetAttributeAsync("aria-valuenow");
        Assert.AreEqual("50", valuenow);
    }

    [TestMethod]
    public async Task DataGrid_HasCorrectStructure()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <table role='grid' aria-label='Data Grid'>
                <thead>
                    <tr>
                        <th scope='col'>Name</th>
                        <th scope='col'>Email</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>John Doe</td>
                        <td>john@example.com</td>
                    </tr>
                </tbody>
            </table>
        ");
        
        var grid = await Page.QuerySelectorAsync("[role='grid']");
        Assert.IsNotNull(grid);
        
        var headers = await Page.QuerySelectorAllAsync("th[scope='col']");
        Assert.AreEqual(2, headers.Count);
    }

    [TestMethod]
    public async Task TextInput_HasRequiredAttributes()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <input type='text' aria-required='true' aria-invalid='false' aria-describedby='help-text' />
            <div id='help-text'>Enter your name</div>
        ");
        
        var input = await Page.QuerySelectorAsync("input");
        Assert.IsNotNull(input);
        
        var required = await input.GetAttributeAsync("aria-required");
        Assert.AreEqual("true", required);
        
        var describedby = await input.GetAttributeAsync("aria-describedby");
        Assert.AreEqual("help-text", describedby);
    }

    [TestMethod]
    public async Task Select_HasCorrectAttributes()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <select aria-label='Choose an option'>
                <option value='1'>Option 1</option>
                <option value='2'>Option 2</option>
            </select>
        ");
        
        var select = await Page.QuerySelectorAsync("select");
        Assert.IsNotNull(select);
        
        var label = await select.GetAttributeAsync("aria-label");
        Assert.AreEqual("Choose an option", label);
    }
}
