using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Overlays.ContextMenu;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;

namespace XForge.DesignSystem.Tests.Components;

public class XFContextMenuAriaTest : XForgeTestContext
{
    public XFContextMenuAriaTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static readonly XFDropdownMenuItem[] Items =
    {
        new() { Text = "Open", Value = "open" },
        new() { Kind = XFDropdownMenuItemKind.Separator },
        new() { Text = "Disabled", Disabled = true },
        new() { Text = "Pin", Kind = XFDropdownMenuItemKind.XFCheckbox, Checked = true }
    };

    [Fact]
    public void Role_Menu_IsMenu()
    {
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, Items);
        });

        Assert.Contains("role=\"menu\"", cut.Markup);
    }

    [Fact]
    public void AriaLabel_HasDefaultValue()
    {
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, Items);
        });

        var menu = cut.Find("[role='menu']");
        Assert.Equal("Menu de contexto", menu.GetAttribute("aria-label"));
    }

    [Fact]
    public void Role_ActionItem_IsMenuitem()
    {
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, Items);
        });

        Assert.Contains("role=\"menuitem\"", cut.Markup);
    }

    [Fact]
    public void Role_CheckboxItem_IsMenuitemcheckbox()
    {
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, Items);
        });

        Assert.Contains("role=\"menuitemcheckbox\"", cut.Markup);
    }

    [Fact]
    public void Role_RadioItem_IsMenuitemradio()
    {
        var radioItems = new[]
        {
            new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFRadio, Text = "Sort by date", Checked = true, Group = "sort" },
            new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFRadio, Text = "Sort by name", Group = "sort" }
        };

        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, radioItems);
        });

        Assert.Contains("role=\"menuitemradio\"", cut.Markup);
    }

    [Fact]
    public void AriaChecked_OnCheckboxItem_IsPresent()
    {
        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, Items);
        });

        Assert.Contains("aria-checked=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaChecked_OnRadioItem_IsPresent()
    {
        var radioItems = new[]
        {
            new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFRadio, Text = "Sort by date", Checked = true, Group = "sort" },
            new XFDropdownMenuItem { Kind = XFDropdownMenuItemKind.XFRadio, Text = "Sort by name", Group = "sort" }
        };

        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, radioItems);
        });

        Assert.Contains("aria-checked=\"true\"", cut.Markup);
        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    [Fact]
    public void AriaHaspopup_OnSubmenuItem_IsMenu()
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

        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, items);
        });

        Assert.Contains("aria-haspopup=\"menu\"", cut.Markup);
    }

    [Fact]
    public void AriaExpanded_OnSubmenuItem_IsPresent()
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

        var cut = Render<XFContextMenu>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Items, items);
        });

        Assert.Contains("aria-expanded", cut.Markup);
    }
}
