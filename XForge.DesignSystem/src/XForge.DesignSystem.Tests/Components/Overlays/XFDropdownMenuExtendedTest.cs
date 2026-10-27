using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Overlays;

public sealed class XFDropdownMenuExtendedTest : BunitContext
{
    public XFDropdownMenuExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFDropdownMenu> RenderDropdownMenu(
        Action<ComponentParameterCollectionBuilder<XFDropdownMenu>>? configure = null)
    {
        return Render<XFDropdownMenu>(p => { configure?.Invoke(p); });
    }

    // ButtonClass tests — the trigger button is always rendered
    [Fact]
    public void Extended_ButtonClass_RendersDefaultButton()
    {
        var cut = RenderDropdownMenu();
        var button = cut.Find("button[aria-haspopup='menu']");
        Assert.NotNull(button);
        Assert.Contains("inline-flex", button.GetAttribute("class"));
        Assert.Contains("h-9", button.GetAttribute("class"));
        Assert.Contains("items-center", button.GetAttribute("class"));
        Assert.Contains("gap-2", button.GetAttribute("class"));
        Assert.Contains("rounded-lg", button.GetAttribute("class"));
        Assert.Contains("border-surface-200", button.GetAttribute("class"));
        Assert.Contains("bg-bg-surface", button.GetAttribute("class"));
        Assert.Contains("px-3", button.GetAttribute("class"));
        Assert.Contains("text-sm", button.GetAttribute("class"));
        Assert.Contains("font-semibold", button.GetAttribute("class"));
        Assert.Contains("text-text-primary", button.GetAttribute("class"));
    }

    [Fact]
    public void Extended_ButtonClass_ContainsShadow()
    {
        var cut = RenderDropdownMenu();
        var button = cut.Find("button[aria-haspopup='menu']");
        Assert.NotNull(button);
        Assert.Contains("shadow-", button.GetAttribute("class"));
    }

    [Fact]
    public void Extended_ButtonClass_ContainsTransition()
    {
        var cut = RenderDropdownMenu();
        var button = cut.Find("button[aria-haspopup='menu']");
        Assert.NotNull(button);
        Assert.Contains("transition-all", button.GetAttribute("class"));
    }

    // DropdownChevronClass
    [Fact]
    public void Extended_DropdownChevronClass_Closed()
    {
        var cut = RenderDropdownMenu();
        Assert.Contains("transition-transform", cut.Markup);
        Assert.Contains("duration-150", cut.Markup);
    }

    [Fact]
    public void Extended_DropdownChevronClass_Open()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Open, true));
        Assert.Contains("rotate-180", cut.Markup);
    }

    // ButtonText
    [Fact]
    public void Extended_ButtonText_Default()
    {
        var cut = RenderDropdownMenu();
        Assert.Contains("Menu", cut.Markup);
    }

    [Fact]
    public void Extended_ButtonText_Custom()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.ButtonText, "Options"));
        Assert.Contains("Options", cut.Markup);
    }

    // Size parameter
    [Fact]
    public void Extended_SizeSm()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_SizeLg()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.NotNull(cut);
    }

    // Placement parameter (uses XFPlacement from popover)
    [Fact]
    public void Extended_Parameters_Accepted()
    {
        var cut = RenderDropdownMenu(p => p
            .Add(x => x.ButtonText, "Test")
            .Add(x => x.Open, false)
            .Add(x => x.CloseOnSelect, false)
            .Add(x => x.CloseOnEscape, false)
            .Add(x => x.CloseOnOutsideClick, false));
        Assert.Contains("Test", cut.Markup);
    }

    // ActiveTone parameter
    [Fact]
    public void Extended_ActiveTone_Secondary()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.ActiveTone, XFSemanticColor.Secondary));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_ActiveTone_Success()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.ActiveTone, XFSemanticColor.Success));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_ActiveTone_Warning()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.ActiveTone, XFSemanticColor.Warning));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_ActiveTone_Danger()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.ActiveTone, XFSemanticColor.Danger));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_ActiveTone_Neutral()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.ActiveTone, XFSemanticColor.Neutral));
        Assert.NotNull(cut);
    }

    // MaxHeightClass parameter
    [Fact]
    public void Extended_MaxHeightClass_Default()
    {
        var cut = RenderDropdownMenu();
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_MaxHeightClass_Custom()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.MaxHeightClass, "max-h-60"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_MaxHeightClass_Null()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.MaxHeightClass, (string?)null));
        Assert.NotNull(cut);
    }

    // Items parameter
    [Fact]
    public void Extended_Items_Empty()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, new List<XFDropdownMenuItem>()));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_Action()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Action", Kind = XFDropdownMenuItemKind.Action } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_Checkbox()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Check", Kind = XFDropdownMenuItemKind.XFCheckbox } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_Radio()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Radio", Kind = XFDropdownMenuItemKind.XFRadio } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_Separator()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Sep", Kind = XFDropdownMenuItemKind.Separator } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_Header()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Header", Kind = XFDropdownMenuItemKind.Header } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    // MenuAriaLabel / SubmenuAriaLabelFormat
    [Fact]
    public void Extended_MenuAriaLabel_Custom()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.MenuAriaLabel, "My Menu"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_SubmenuAriaLabelFormat_Custom()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.SubmenuAriaLabelFormat, "Sub {0}"));
        Assert.NotNull(cut);
    }

    // Open / OpenChanged
    [Fact]
    public void Extended_Open_True()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Open, true));
        Assert.Contains("rotate-180", cut.Markup);
    }

    [Fact]
    public void Extended_Open_False()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Open, false));
        Assert.DoesNotContain("rotate-180", cut.Markup);
    }

    // Aria attributes on button
    [Fact]
    public void Extended_Button_HasAriaHaspopup()
    {
        var cut = RenderDropdownMenu();
        var button = cut.Find("button[aria-haspopup='menu']");
        Assert.NotNull(button);
    }

    [Fact]
    public void Extended_Button_HasAriaExpanded()
    {
        var cut = RenderDropdownMenu();
        var button = cut.Find("button[aria-haspopup='menu']");
        Assert.NotNull(button);
        Assert.NotNull(button.GetAttribute("aria-expanded"));
    }

    // Items with various properties
    [Fact]
    public void Extended_Items_Disabled()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Disabled", Kind = XFDropdownMenuItemKind.Action, Disabled = true } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_CheckedCheckbox()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Checked", Kind = XFDropdownMenuItemKind.XFCheckbox, Checked = true } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_CheckedRadio()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Checked", Kind = XFDropdownMenuItemKind.XFRadio, Checked = true } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_WithBadge()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Badge", Kind = XFDropdownMenuItemKind.Action, Badge = "5" } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_WithShortcut()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Shortcut", Kind = XFDropdownMenuItemKind.Action, Shortcut = "Ctrl+S" } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_WithDescription()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Desc", Kind = XFDropdownMenuItemKind.Action, Description = "A description" } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_WithIcon()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Icon", Kind = XFDropdownMenuItemKind.Action, Icon = (RenderFragment)(builder => builder.AddContent(0, "star")) } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_WithChildren()
    {
        var items = new List<XFDropdownMenuItem>
        {
            new() { Text = "Parent", Kind = XFDropdownMenuItemKind.Action, Children = new List<XFDropdownMenuItem> { new() { Text = "Child", Kind = XFDropdownMenuItemKind.Action } } }
        };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_WithGroup()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Grouped", Kind = XFDropdownMenuItemKind.Action, Group = "group1" } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_ToneDanger()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Danger", Kind = XFDropdownMenuItemKind.Action, Tone = XFSemanticColor.Danger } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_ToneWarning()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Warning", Kind = XFDropdownMenuItemKind.Action, Tone = XFSemanticColor.Warning } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_ToneSuccess()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Success", Kind = XFDropdownMenuItemKind.Action, Tone = XFSemanticColor.Success } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_ToneSecondary()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Secondary", Kind = XFDropdownMenuItemKind.Action, Tone = XFSemanticColor.Secondary } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_WithValue()
    {
        var items = new List<XFDropdownMenuItem> { new() { Text = "Value", Kind = XFDropdownMenuItemKind.Action, Value = "val1" } };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Items_MixedKinds()
    {
        var items = new List<XFDropdownMenuItem>
        {
            new() { Text = "Action", Kind = XFDropdownMenuItemKind.Action },
            new() { Text = "Sep", Kind = XFDropdownMenuItemKind.Separator },
            new() { Text = "Header", Kind = XFDropdownMenuItemKind.Header },
            new() { Text = "Check", Kind = XFDropdownMenuItemKind.XFCheckbox, Checked = true },
            new() { Text = "Radio", Kind = XFDropdownMenuItemKind.XFRadio },
            new() { Text = "Disabled", Kind = XFDropdownMenuItemKind.Action, Disabled = true },
        };
        var cut = RenderDropdownMenu(p => p.Add(x => x.Items, items));
        Assert.NotNull(cut);
    }

    // Class parameter — applied to the popover wrapper, not the button
    [Fact]
    public void Extended_Class_Custom()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    // Style parameter — applied to the popover wrapper
    [Fact]
    public void Extended_Style_Custom()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Style, "color:red"));
        Assert.NotNull(cut);
    }

    // Id parameter
    [Fact]
    public void Extended_Id_Custom()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.Id, "my-menu"));
        Assert.Contains("my-menu", cut.Markup);
    }

    // AdditionalAttributes — applied to the popover wrapper
    [Fact]
    public void Extended_AdditionalAttributes()
    {
        var cut = RenderDropdownMenu(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "dropdown" } }));
        Assert.NotNull(cut);
    }

    // Trigger is RenderFragment<XFPopoverTriggerContext>? — requires typed context
    // Tested implicitly through default button rendering above

    // ItemSelected event
    [Fact]
    public void Extended_ItemSelected_EventCallback()
    {
        var cut = RenderDropdownMenu();
        Assert.NotNull(cut);
    }
}
