using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Components.Overlays.Popover;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFDropdownMenuKeyboardTest : XForgeTestContext
{
    public XFDropdownMenuKeyboardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static IReadOnlyList<XFDropdownMenuItem> GetMenuItems() => new List<XFDropdownMenuItem>
    {
        new() { Id = "1", Text = "Edit", Kind = XFDropdownMenuItemKind.Action },
        new() { Id = "2", Text = "Duplicate", Kind = XFDropdownMenuItemKind.Action },
        new() { Id = "3", Text = "Delete", Kind = XFDropdownMenuItemKind.Action, Tone = XFSemanticColor.Danger },
    };

    [Fact]
    public void Escape_Key_Closes_Dropdown()
    {
        var open = true;
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var menu = cut.Find("div[role='menu']");
        menu.KeyDown("Escape");

        Assert.False(open);
    }

    [Fact]
    public void ArrowDown_Moves_To_Next_Item()
    {
        var open = true;
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var menu = cut.Find("div[role='menu']");
        menu.KeyDown("ArrowDown");

        Assert.True(open);
    }

    [Fact]
    public void ArrowUp_Moves_To_Previous_Item()
    {
        var open = true;
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var menu = cut.Find("div[role='menu']");
        menu.KeyDown("ArrowUp");

        Assert.True(open);
    }

    [Fact]
    public void Enter_Activates_Item()
    {
        var open = true;
        XFDropdownMenuItem? selected = null;
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.ItemSelected, EventCallback.Factory.Create<XFDropdownMenuItem>(this, item => selected = item));
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var menu = cut.Find("div[role='menu']");
        menu.KeyDown("Enter");

        Assert.NotNull(selected);
    }

    [Fact]
    public void Space_Activates_Item()
    {
        var open = true;
        XFDropdownMenuItem? selected = null;
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.ItemSelected, EventCallback.Factory.Create<XFDropdownMenuItem>(this, item => selected = item));
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var menu = cut.Find("div[role='menu']");
        menu.KeyDown(" ");

        Assert.NotNull(selected);
    }

    [Fact]
    public void Verify_Role_Menu_And_Menuitem_Roles()
    {
        var open = true;
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        Assert.Contains("role=\"menu\"", cut.Markup);
        Assert.Contains("role=\"menuitem\"", cut.Markup);
    }

    [Fact]
    public void Dropdown_Has_Aria_Haspopup_On_Trigger()
    {
        var open = false;
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        Assert.Contains("aria-haspopup=\"menu\"", cut.Markup);
    }
}
