using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Button;
using XForge.DesignSystem.Components.Button.SplitButton;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Button;

public sealed class XFSplitButtonTest : BunitContext
{
    public XFSplitButtonTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private static List<XFDropdownMenuItem> SampleItems => new()
    {
        new XFDropdownMenuItem { Text = "Action 1", Kind = XFDropdownMenuItemKind.Action },
        new XFDropdownMenuItem { Text = "Action 2", Kind = XFDropdownMenuItemKind.Action },
        new XFDropdownMenuItem { Text = "Separator", Kind = XFDropdownMenuItemKind.Separator },
        new XFDropdownMenuItem { Text = "Checkbox 1", Kind = XFDropdownMenuItemKind.XFCheckbox, Checked = true },
        new XFDropdownMenuItem { Text = "Radio 1", Kind = XFDropdownMenuItemKind.XFRadio, Checked = false }
    };

    [Fact] public void Renders_Default()
    {
        var cut = Render<XFSplitButton>();
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithText()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Actions"));
        Assert.Contains("Actions", cut.Markup);
    }

    [Fact] public void Renders_WithVariantOutline()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Variant, XFButtonVariant.Outline));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithVariantGhost()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Variant, XFButtonVariant.Ghost));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithVariantSoft()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Variant, XFButtonVariant.Soft));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithVariantSubtle()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Variant, XFButtonVariant.Subtle));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithColorSecondary()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Color, XFButtonColor.Secondary));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithColorSuccess()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Color, XFButtonColor.Success));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithColorWarning()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Color, XFButtonColor.Warning));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithColorDanger()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Color, XFButtonColor.Danger));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithColorNeutral()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Color, XFButtonColor.Neutral));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithColorInfo()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Color, XFButtonColor.Info));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithSizeXs()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Size, XFExtendedSize.Xs));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithSizeSm()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Size, XFExtendedSize.Sm));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithSizeLg()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Size, XFExtendedSize.Lg));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithSizeXl()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Size, XFExtendedSize.Xl));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithFullWidth()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    [Fact] public void Renders_WithOpenTrue()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, SampleItems).Add(x => x.Open, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithItems()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, SampleItems));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithMenuAriaLabel()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.MenuAriaLabel, "Options menu"));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithMenuPanelAriaLabel()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.MenuPanelAriaLabel, "Panel label"));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithCloseOnSelectFalse()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, SampleItems).Add(x => x.CloseOnSelect, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithChildContent()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Custom"))));
        Assert.Contains("Custom", cut.Markup);
    }

    [Fact] public void Renders_WithClass()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Class, "my-split"));
        Assert.Contains("my-split", cut.Markup);
    }

    [Fact] public void Renders_WithId()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Id, "split1"));
        Assert.NotNull(cut.Find("#split1"));
    }

    [Fact] public void Renders_WithDisabled()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithOnClick()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithItemSelected()
    {
        XFDropdownMenuItem? selected = null;
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, SampleItems).Add(x => x.ItemSelected, EventCallback.Factory.Create<XFDropdownMenuItem>(this, item => selected = item)));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithOpenChanged()
    {
        bool? changed = null;
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, SampleItems).Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, val => changed = val)));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_EmptyItems()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, Array.Empty<XFDropdownMenuItem>()));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_DisabledItems()
    {
        var items = new List<XFDropdownMenuItem>
        {
            new() { Text = "Disabled", Kind = XFDropdownMenuItemKind.Action, Disabled = true }
        };
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_CheckboxItems()
    {
        var items = new List<XFDropdownMenuItem>
        {
            new() { Text = "Check 1", Kind = XFDropdownMenuItemKind.XFCheckbox, Checked = true },
            new() { Text = "Check 2", Kind = XFDropdownMenuItemKind.XFCheckbox, Checked = false }
        };
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_RadioItems()
    {
        var items = new List<XFDropdownMenuItem>
        {
            new() { Text = "Radio 1", Kind = XFDropdownMenuItemKind.XFRadio, Checked = true },
            new() { Text = "Radio 2", Kind = XFDropdownMenuItemKind.XFRadio, Checked = false }
        };
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_MixedItemKinds()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, SampleItems));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_SelectedCheckboxItem()
    {
        var items = new List<XFDropdownMenuItem>
        {
            new() { Text = "Checked", Kind = XFDropdownMenuItemKind.XFCheckbox, Checked = true }
        };
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_TypeSubmit()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Type, XFButtonType.Submit));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_TypeReset()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test").Add(x => x.Type, XFButtonType.Reset));
        Assert.NotNull(cut);
    }

    [Fact] public void DisposeAsync_DoesNotThrow()
    {
        var cut = Render<XFSplitButton>(p => p.Add(x => x.Text, "Test"));
        cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
    }
}
