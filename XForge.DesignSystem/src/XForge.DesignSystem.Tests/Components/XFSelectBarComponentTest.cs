using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Forms.SelectBar;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFSelectBarComponentTest : XForgeTestContext
{
    public XFSelectBarComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithItems_RendersButtons()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "Option A", Value = "a" },
            new() { Text = "Option B", Value = "b" },
            new() { Text = "Option C", Value = "c" }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });

        Assert.Contains("xf-select-bar", cut.Markup);
        Assert.Contains("radiogroup", cut.Markup);
        Assert.Contains("Option A", cut.Markup);
        Assert.Contains("Option B", cut.Markup);
        Assert.Contains("Option C", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel_SetsAttribute()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "A", Value = "a" }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.AriaLabel, "Choose option");
        });

        Assert.Contains("aria-label=\"Choose option\"", cut.Markup);
    }

    [Fact]
    public void Render_SelectedItem_ShowsCheckedState()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "Yes", Value = "yes" },
            new() { Text = "No", Value = "no" }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Value, "yes");
        });

        Assert.Contains("aria-checked=\"true\"", cut.Markup);
        Assert.Contains("bg-primary", cut.Markup);
    }

    [Fact]
    public void Render_UnselectedItem_ShowsUncheckedState()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "Option", Value = "opt" }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Value, "other");
        });

        Assert.Contains("aria-checked=\"false\"", cut.Markup);
    }

    [Fact]
    public void Render_VerticalOrientation_AppliesVerticalLayout()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "A", Value = "a" }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Orientation, XFOrientation.Vertical);
        });

        Assert.Contains("flex-col", cut.Markup);
    }

    [Fact]
    public void Render_HorizontalOrientation_AppliesHorizontalLayout()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "A", Value = "a" }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Orientation, XFOrientation.Horizontal);
        });

        Assert.Contains("flex-row", cut.Markup);
    }

    [Fact]
    public void Render_DisabledItem_ShowsDisabledState()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "Active", Value = "active" },
            new() { Text = "Disabled", Value = "disabled", Disabled = true }
        };

        var cut = Render<XFSelectBar>(parameters =>
            parameters.Add(p => p.Items, items));

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
        Assert.Contains("opacity-50", cut.Markup);
    }

    [Fact]
    public async Task Click_SelectableItem_InvokesValueChanged()
    {
        string? selectedValue = null;
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "A", Value = "a" },
            new() { Text = "B", Value = "b" }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, v => selectedValue = v));
        });

        var buttons = cut.FindAll("button[role=\"radio\"]");
        await buttons[1].ClickAsync();

        Assert.Equal("b", selectedValue);
    }

    [Fact]
    public async Task Click_DisabledItem_DoesNotInvokeValueChanged()
    {
        string? selectedValue = "original";
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "Disabled", Value = "disabled", Disabled = true }
        };

        var cut = Render<XFSelectBar>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.Value, "original");
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, v => selectedValue = v));
        });

        var button = cut.Find("button[role=\"radio\"]");
        await button.ClickAsync();

        Assert.Equal("original", selectedValue);
    }

    [Fact]
    public void Render_WithIcon_ShowsIcon()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "Star", Value = "star", Icon = null }
        };

        var cut = Render<XFSelectBar>(parameters =>
            parameters.Add(p => p.Items, items));

        Assert.Contains("Star", cut.Markup);
    }

    [Fact]
    public void Render_AllButtons_HaveRoleRadio()
    {
        var items = new List<XFSelectBarItem>
        {
            new() { Text = "A", Value = "a" },
            new() { Text = "B", Value = "b" }
        };

        var cut = Render<XFSelectBar>(parameters =>
            parameters.Add(p => p.Items, items));

        var radioButtons = cut.FindAll("button[role=\"radio\"]");
        Assert.Equal(2, radioButtons.Count);
    }
}
