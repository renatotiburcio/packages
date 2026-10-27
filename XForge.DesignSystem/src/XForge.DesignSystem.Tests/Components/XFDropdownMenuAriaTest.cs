using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;

namespace XForge.DesignSystem.Tests.Components;

public class XFDropdownMenuAriaTest : BunitContext
{
    public XFDropdownMenuAriaTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Role_Menu_IsMenu()
    {
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, CreateItems());
        });

        Assert.Contains("role=\"menu\"", cut.Markup);
    }

    [Fact]
    public void Role_ActionItem_IsMenuitem()
    {
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, CreateItems());
        });

        Assert.Contains("role=\"menuitem\"", cut.Markup);
    }

    [Fact]
    public void Role_CheckboxItem_IsMenuitemcheckbox()
    {
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, CreateChoiceItems());
        });

        Assert.Contains("role=\"menuitemcheckbox\"", cut.Markup);
    }

    [Fact]
    public void AriaChecked_OnCheckboxItem_IsPresent()
    {
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, CreateChoiceItems());
        });

        Assert.Contains("aria-checked=\"true\"", cut.Markup);
        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    [Fact]
    public void AriaHaspopup_OnItemsWithSubmenus_IsPresent()
    {
        var items = new[]
        {
            new XFDropdownMenuItem
            {
                Text = "Compartilhar",
                Children =
                [
                    new() { Text = "Copiar link", Value = "copy-link" }
                ]
            }
        };

        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, items);
        });

        Assert.Contains("aria-haspopup", cut.Markup);
    }

    [Fact]
    public void DefaultTrigger_ExposesExpandedAndControlsMenu()
    {
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.ButtonText, "Actions");
            parameters.Add(p => p.Items, CreateItems());
        });

        var trigger = cut.Find("button[aria-haspopup='menu']");
        var menuId = trigger.GetAttribute("aria-controls");

        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));
        Assert.NotNull(menuId);

        trigger.Click();

        Assert.Equal("true", cut.Find("button[aria-haspopup='menu']").GetAttribute("aria-expanded"));
        Assert.NotNull(cut.Find($"#{menuId}"));
    }

    [Fact]
    public void Keyboard_Navigation_ExposesActiveDescendant()
    {
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, CreateItems());
        });

        var menu = cut.Find("[role='menu']");
        var activeId = menu.GetAttribute("aria-activedescendant");

        Assert.NotNull(activeId);
        Assert.Contains("Open details", cut.Find($"#{activeId}").TextContent);

        menu.KeyDown("ArrowDown");

        var nextActiveId = cut.Find("[role='menu']").GetAttribute("aria-activedescendant");
        Assert.NotEqual(activeId, nextActiveId);
        Assert.Contains("Duplicate", cut.Find($"#{nextActiveId}").TextContent);
    }

    [Fact]
    public void DisabledItem_ExposesAriaDisabled()
    {
        var cut = Render<XFDropdownMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, CreateItems());
        });

        var disabledItem = cut.FindAll("[role='menuitem']").First(item => item.TextContent.Contains("Archive"));

        Assert.Equal("true", disabledItem.GetAttribute("aria-disabled"));
    }

    private static IReadOnlyList<XFDropdownMenuItem> CreateItems() => new[]
    {
        new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.Header, Text = "Workspace" },
        new XFDropdownMenuItem { Text = "Open details", Value = "open" },
        new XFDropdownMenuItem { Text = "Duplicate", Value = "duplicate" },
        new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.Separator },
        new XFDropdownMenuItem { Text = "Archive", Disabled = true, Value = "archive" }
    };

    private static IReadOnlyList<XFDropdownMenuItem> CreateChoiceItems() => new[]
    {
        new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFCheckbox, Text = "Show archived", Checked = true },
        new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFCheckbox, Text = "Compact rows" },
        new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFRadio, Text = "Sort by date", Checked = true, Group = "sort" },
        new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFRadio, Text = "Sort by name", Group = "sort" }
    };
}
