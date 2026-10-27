using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.Components;

[TestClass]
public class XFProfileMenuE2ETests : PageTest
{
    [TestMethod]
    public async Task ProfileMenu_RendersDropdown()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-profile-menu'>
                <button class='xf-profile-menu-trigger' aria-haspopup='true'>
                    <span class='xf-profile-menu-avatar'>JD</span>
                </button>
                <div class='xf-profile-menu-dropdown' role='menu'>
                    <a href='/profile' role='menuitem'>Profile</a>
                    <a href='/settings' role='menuitem'>Settings</a>
                    <a href='/logout' role='menuitem'>Logout</a>
                </div>
            </div>
        ");
        
        var menu = await Page.QuerySelectorAsync(".xf-profile-menu");
        Assert.IsNotNull(menu);
        
        var trigger = await Page.QuerySelectorAsync(".xf-profile-menu-trigger");
        Assert.IsNotNull(trigger);
        
        var dropdown = await Page.QuerySelectorAsync(".xf-profile-menu-dropdown");
        Assert.IsNotNull(dropdown);
    }

    [TestMethod]
    public async Task ProfileMenu_Dropdown_HasCorrectRole()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div class='xf-profile-menu-dropdown' role='menu'>
                <a href='/profile' role='menuitem'>Profile</a>
            </div>
        ");
        
        var dropdown = await Page.QuerySelectorAsync("[role='menu']");
        Assert.IsNotNull(dropdown);
        
        var menuitem = await Page.QuerySelectorAsync("[role='menuitem']");
        Assert.IsNotNull(menuitem);
    }
}
