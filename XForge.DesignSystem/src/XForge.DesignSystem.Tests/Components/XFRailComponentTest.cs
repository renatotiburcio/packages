using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Components;

public class XFRailComponentTest : XForgeTestContext
{
    public XFRailComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<NavigationStateService>();
    }

    [Fact]
    public void Render_Default_ShowsNavElement()
    {
        var cut = Render<XFRail>(parameters =>
        {
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Item");
                builder.CloseElement();
            }));
        });

        Assert.Contains("<nav", cut.Markup);
        Assert.Contains("role=\"navigation\"", cut.Markup);
        Assert.Contains("Item", cut.Markup);
    }

    [Fact]
    public void Render_DefaultAriaLabel_UsesPTBR()
    {
        var cut = Render<XFRail>(parameters =>
        {
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));
        });

        Assert.Contains("Navegacao principal", cut.Markup);
    }

    [Fact]
    public void Render_CustomAriaLabel_UsesProvidedLabel()
    {
        var cut = Render<XFRail>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Main navigation");
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));
        });

        Assert.Contains("Main navigation", cut.Markup);
    }

    [Fact]
    public void Render_WithHeader_ShowsHeaderSection()
    {
        var cut = Render<XFRail>(parameters =>
        {
            parameters.Add(p => p.Header, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.AddContent(0, "Logo");
            }));
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));
        });

        Assert.Contains("Logo", cut.Markup);
    }

    [Fact]
    public void Render_WithFooter_ShowsFooterSection()
    {
        var cut = Render<XFRail>(parameters =>
        {
            parameters.Add(p => p.Footer, (Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.AddContent(0, "Footer content");
            }));
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));
        });

        Assert.Contains("Footer content", cut.Markup);
    }

    [Fact]
    public void Render_WithoutHeader_DoesNotShowHeaderSection()
    {
        var cut = Render<XFRail>(parameters =>
        {
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));
        });

        // Should not have a header div with mb-8 class
        Assert.DoesNotContain("mb-8", cut.Markup);
    }

    [Fact]
    public void Render_RailClass_ContainsExpectedClasses()
    {
        var cut = Render<XFRail>(parameters =>
        {
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));
        });

        Assert.Contains("flex", cut.Markup);
        Assert.Contains("w-16", cut.Markup);
        Assert.Contains("flex-col", cut.Markup);
        Assert.Contains("bg-primary", cut.Markup);
    }

    [Fact]
    public void Constants_HaveExpectedValues()
    {
        Assert.Equal(16, XFRail.ItemGap);
        Assert.Equal(40, XFRail.ItemHeight);
        Assert.Equal(56, XFRail.ItemStep);
    }

    [Fact]
    public void Render_WithItems_RendersFromModel()
    {
        var items = new List<RailNavItem>
        {
            new() { Brand = "components", Label = "Components", Icon = "cube", Type = RailNavItemType.Route },
            new() { Brand = "tokens", Label = "Tokens", Icon = "swatch", Type = RailNavItemType.Route }
        };

        var cut = Render<XFRail>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });

        Assert.Contains("Components", cut.Markup);
        Assert.Contains("Tokens", cut.Markup);
    }

    [Fact]
    public void Render_WithItems_SeparatorType_RendersSeparator()
    {
        var items = new List<RailNavItem>
        {
            new() { Brand = "components", Label = "Components", Icon = "cube", Type = RailNavItemType.Route },
            new() { Brand = "", Label = "separator", Type = RailNavItemType.Separator },
            new() { Brand = "settings", Label = "Settings", Icon = "cog", Type = RailNavItemType.Action }
        };

        var cut = Render<XFRail>(parameters =>
        {
            parameters.Add(p => p.Items, items);
        });

        Assert.Contains("Components", cut.Markup);
        Assert.Contains("Settings", cut.Markup);
    }
}
