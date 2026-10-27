using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.List;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

/// <summary>
/// Extended tests for XFListItem to cover parameters, selection modes,
/// cascading parent, click interaction, and accessibility attributes.
/// </summary>
public sealed class XFListItemExtendedTest : BunitContext
{
    public XFListItemExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_BaseClass()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Base item"));

        Assert.Contains("xf-list-item", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomClass()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Custom class item")
            .Add(x => x.Class, "my-custom-class"));

        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomStyle()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Styled item")
            .Add(x => x.Style, "color: red; margin-top: 10px"));

        Assert.Contains("xf-list-item", cut.Markup);
    }

    [Fact]
    public void Renders_WithTitle()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "My Item"));

        Assert.Contains("My Item", cut.Markup);
    }

    [Fact]
    public void Renders_WithDescription()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Titled item")
            .Add(x => x.Description, "Item description"));

        Assert.Contains("Item description", cut.Markup);
    }

    [Fact]
    public void Renders_WithMetadataText()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Meta item")
            .Add(x => x.MetadataText, "2024"));

        Assert.Contains("2024", cut.Markup);
    }

    [Fact]
    public void Renders_WithChildContent()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Custom child"))));

        Assert.Contains("Custom child", cut.Markup);
    }

    [Fact]
    public void Renders_WithStartIcon()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Icon start")
            .Add(x => x.StartIcon, (RenderFragment)(builder => builder.AddContent(0, "▶"))));

        Assert.Contains("▶", cut.Markup);
    }

    [Fact]
    public void Renders_WithEndIcon()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Icon end")
            .Add(x => x.EndIcon, (RenderFragment)(builder => builder.AddContent(0, "✕"))));

        Assert.Contains("✕", cut.Markup);
    }

    [Fact]
    public void Renders_WithMetadata()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Meta render")
            .Add(x => x.Metadata, (RenderFragment)(builder => builder.AddContent(0, "<span>badge</span>"))));

        Assert.Contains("badge", cut.Markup);
    }

    [Fact]
    public void Renders_WithSelected()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Selected item")
            .Add(x => x.Selected, true));

        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    [Fact]
    public void Renders_WithDisabled()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Disabled item")
            .Add(x => x.Disabled, true));

        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-55", cut.Markup);
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Renders_WithVisibleFalse()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Hidden item")
            .Add(x => x.Visible, false));

        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void Renders_WithOnClick()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Clickable item")
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));

        Assert.Contains("cursor-pointer", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomValue()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Valued item")
            .Add(x => x.Value, "item-1"));

        Assert.Contains("xf-list-item", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomId()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Id item")
            .Add(x => x.Id, "my-item-id"));

        Assert.Contains("my-item-id", cut.Markup);
    }

    [Fact]
    public void Renders_IsSelectable()
    {
#pragma warning disable BL0005
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Selectable item")
            .Add(x => x.Value, "sel-1")
            .AddCascadingValue("ParentList", new XFList
            {
                SelectionMode = XFListSelectionMode.Single
            }));
#pragma warning restore BL0005

        Assert.Contains("option", cut.Markup);
        Assert.Contains("aria-selected", cut.Markup);
    }

    [Fact]
    public void Renders_IsInteractive()
    {
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Interactive item")
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));

        Assert.Contains("cursor-pointer", cut.Markup);
        Assert.Contains("tabindex=\"0\"", cut.Markup);
    }

    [Fact]
    public async Task Click_TriggersOnClick()
    {
        var clicked = false;
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Click me")
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true)));

        var element = cut.Find(".xf-list-item");
        await cut.InvokeAsync(() => element.ClickAsync(new MouseEventArgs()));

        Assert.True(clicked);
    }

    [Fact]
    public async Task Click_WhenDisabled_DoesNotTrigger()
    {
        var clicked = false;
        var cut = Render<XFListItem>(p => p
            .Add(x => x.Title, "Disabled click")
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true)));

        var element = cut.Find(".xf-list-item");
        await cut.InvokeAsync(() => element.ClickAsync(new MouseEventArgs()));

        Assert.False(clicked);
    }

    [Fact]
    public void Renders_WithAllSelectionModes()
    {
        foreach (XFListSelectionMode mode in Enum.GetValues<XFListSelectionMode>())
        {
#pragma warning disable BL0005
            var cut = Render<XFListItem>(p => p
                .Add(x => x.Title, $"Mode {mode}")
                .Add(x => x.Value, "val-1")
                .AddCascadingValue("ParentList", new XFList
                {
                    SelectionMode = mode
                }));
#pragma warning restore BL0005

            Assert.Contains("xf-list-item", cut.Markup);
        }
    }
}
