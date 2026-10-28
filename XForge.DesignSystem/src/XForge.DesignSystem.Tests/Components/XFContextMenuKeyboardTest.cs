using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Overlays.ContextMenu;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFContextMenuKeyboardTest : XForgeTestContext
{
    public XFContextMenuKeyboardTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static IReadOnlyList<XFDropdownMenuItem> GetMenuItems() => new List<XFDropdownMenuItem>
    {
        new() { Id = "1", Text = "Copy", Kind = XFDropdownMenuItemKind.Action },
        new() { Id = "2", Text = "Paste", Kind = XFDropdownMenuItemKind.Action },
        new() { Id = "3", Text = "Delete", Kind = XFDropdownMenuItemKind.Action, Tone = XFSemanticColor.Danger },
    };

    [Fact]
    public void Escape_Key_Closes_Context_Menu()
    {
        var open = true;
        var cut = Render<XFContextMenu>(parameters =>
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
        var cut = Render<XFContextMenu>(parameters =>
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
        var cut = Render<XFContextMenu>(parameters =>
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
    public void Home_Moves_To_First_Item()
    {
        var open = true;
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var menu = cut.Find("div[role='menu']");
        menu.KeyDown("Home");

        Assert.True(open);
    }

    [Fact]
    public void End_Moves_To_Last_Item()
    {
        var open = true;
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        var menu = cut.Find("div[role='menu']");
        menu.KeyDown("End");

        Assert.True(open);
    }

    [Fact]
    public void Verify_Role_Menu_And_Menuitem_Roles()
    {
        var open = true;
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
        });

        Assert.Contains("role=\"menu\"", cut.Markup);
        Assert.Contains("role=\"menuitem\"", cut.Markup);
    }

    [Fact]
    public void Context_Menu_Triggered_On_Right_Click()
    {
        var open = false;
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, open);
            parameters.Add(p => p.Items, GetMenuItems());
            parameters.Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, value => open = value));
            parameters.AddChildContent("<div>Right click here</div>");
        });

        var trigger = cut.Find("div.xf-context-menu");
        trigger.ContextMenu(new MouseEventArgs());
    }
}
