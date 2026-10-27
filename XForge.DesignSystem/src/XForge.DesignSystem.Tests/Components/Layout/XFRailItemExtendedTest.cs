using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Components.Shared;
using XForge.DesignSystem.Services;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Shell;

public sealed class XFRailItemExtendedTest : Bunit.BunitContext
{
    public XFRailItemExtendedTest()
    {
        Services.AddSingleton<NavigationStateService>();
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupVoid("xfTooltip.init", _ => true);
        JSInterop.SetupVoid("xfTooltip.update", _ => true);
        JSInterop.SetupVoid("xfTooltip.dispose", _ => true);
    }

    private IRenderedComponent<XFRailItem> RenderDefault(
        Action<ComponentParameterCollectionBuilder<XFRailItem>>? configure = null)
    {
        return Render<XFRailItem>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderDefault();
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Label()
    {
        var cut = RenderDefault(p => p.Add(x => x.Label, "Financeiro"));
        Assert.Contains("Financeiro", cut.Markup);
    }

    [Fact]
    public void Extended_Active_RendersActiveState()
    {
        var cut = RenderDefault(p => p.Add(x => x.Active, true));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_ActiveFalse_NoActiveState()
    {
        var cut = RenderDefault(p => p.Add(x => x.Active, false));
        Assert.DoesNotContain("bg-primary-contrast/20", cut.Markup);
    }

    [Fact]
    public void Extended_ShowTooltipFalse_HidesTooltip()
    {
        var cut = RenderDefault(p => p.Add(x => x.ShowTooltip, false));
        Assert.DoesNotContain("xf-tooltip", cut.Markup);
    }

    [Fact]
    public void Extended_ShowTooltipTrue_ShowsTooltip()
    {
        var cut = RenderDefault(p => p.Add(x => x.ShowTooltip, true));
        Assert.Contains("xf-tooltip", cut.Markup);
    }

    [Fact]
    public void Extended_TooltipPosition_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.TooltipPosition, XFTooltipPosition.Left));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_TooltipTheme_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.TooltipTheme, XFTooltipTheme.Light));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_Icon_RendersIcon()
    {
        var cut = RenderDefault(p => p.Add(x => x.Icon, "home"));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_SvgIcon_RendersSvg()
    {
        var svgIcon = "<svg viewBox='0 0 24 24'><path d='M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z'/></svg>";
        var cut = RenderDefault(p => p.Add(x => x.Icon, svgIcon));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_UnlabeledItemAriaLabel_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.UnlabeledItemAriaLabel, "Navegação item"));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_RendersDisabledStyles()
    {
        var cut = RenderDefault(p => p.Add(x => x.Disabled, true));
        Assert.Contains("opacity-45", cut.Markup);
        Assert.Contains("cursor-not-allowed", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_PointerEventsNone()
    {
        var cut = RenderDefault(p => p.Add(x => x.Disabled, true));
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    [Fact]
    public void Extended_ChildContent_Renders()
    {
        var cut = Render<XFRailItem>(p => p
            .Add(x => x.Label, "Test")
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "child"))));
        Assert.Contains("child", cut.Markup);
    }

    [Fact]
    public void Extended_OnClick_CallbackRegistered()
    {
        var clicked = false;
        var cut = RenderDefault(p => p.Add(
            x => x.OnClick,
            EventCallback.Factory.Create(this, () => clicked = true)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_Item_RouteType()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route
        };
        var cut = RenderDefault(p => p.Add(x => x.Item, item));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_Item_ActionType()
    {
        var item = new RailNavItem
        {
            Brand = "settings",
            Label = "Settings",
            Type = RailNavItemType.Action
        };
        var cut = RenderDefault(p => p.Add(x => x.Item, item));
        Assert.Contains("data-rail-item", cut.Markup);
    }

    [Fact]
    public void Extended_Item_SeparatorType()
    {
        var item = new RailNavItem
        {
            Brand = "sep",
            Label = "Separator",
            Type = RailNavItemType.Separator
        };
        var cut = RenderDefault(p => p.Add(x => x.Item, item));
        Assert.Contains("data-rail-item", cut.Markup);
        Assert.Contains("role=\"separator\"", cut.Markup);
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_Works()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route,
            RoutePrefixes = new[] { "/financeiro" }
        };
        Assert.True(item.MatchesPath("/financeiro/overview"));
        Assert.True(item.MatchesPath("/financeiro"));
        Assert.False(item.MatchesPath("/estoque"));
    }

    [Fact]
    public void Extended_RailNavItem_IsActive_RouteType()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route
        };
        item.IsActive = true;
        Assert.True(item.IsActive);
    }

    [Fact]
    public void Extended_RailNavItem_IsActive_ActionType_CannotActivate()
    {
        var item = new RailNavItem
        {
            Brand = "settings",
            Label = "Settings",
            Type = RailNavItemType.Action
        };
        item.IsActive = true;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Extended_RailNavItem_IsInteractive()
    {
        var routeItem = new RailNavItem { Brand = "b", Label = "L", Type = RailNavItemType.Route };
        var actionItem = new RailNavItem { Brand = "b", Label = "L", Type = RailNavItemType.Action };
        var sepItem = new RailNavItem { Brand = "b", Label = "L", Type = RailNavItemType.Separator };
        Assert.True(routeItem.IsInteractive);
        Assert.True(actionItem.IsInteractive);
        Assert.False(sepItem.IsInteractive);
    }

    [Fact]
    public void Extended_RailNavItem_Height_ByTypes()
    {
        var routeItem = new RailNavItem { Brand = "b", Label = "L", Type = RailNavItemType.Route };
        var actionItem = new RailNavItem { Brand = "b", Label = "L", Type = RailNavItemType.Action };
        var sepItem = new RailNavItem { Brand = "b", Label = "L", Type = RailNavItemType.Separator };
        Assert.Equal(40, routeItem.Height);
        Assert.Equal(40, actionItem.Height);
        Assert.Equal(24, sepItem.Height);
    }

    [Fact]
    public void Extended_RailNavItem_GetNavigationRoute()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route,
            OverviewRoute = "/financeiro/overview"
        };
        Assert.Equal("/financeiro/overview", item.GetNavigationRoute());
    }

    [Fact]
    public void Extended_RailNavItem_GetNavigationRoute_Default()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route
        };
        Assert.Equal("/financeiro/overview", item.GetNavigationRoute());
    }

    [Fact]
    public void Extended_RailNavItem_GetNavigationRoute_ActionReturnsNull()
    {
        var item = new RailNavItem
        {
            Brand = "settings",
            Label = "Settings",
            Type = RailNavItemType.Action
        };
        Assert.Null(item.GetNavigationRoute());
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_Root()
    {
        var item = new RailNavItem
        {
            Brand = "home",
            Label = "Home",
            Type = RailNavItemType.Route,
            IsDefault = true
        };
        Assert.True(item.MatchesPath("/"));
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_WithQuery()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route,
            RoutePrefixes = new[] { "/financeiro" }
        };
        Assert.True(item.MatchesPath("/financeiro?query=1"));
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_WithHash()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route,
            RoutePrefixes = new[] { "/financeiro" }
        };
        Assert.True(item.MatchesPath("/financeiro#hash"));
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_TrailingSlash()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route,
            RoutePrefixes = new[] { "/financeiro" }
        };
        Assert.True(item.MatchesPath("/financeiro/"));
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_NoLeadingSlash()
    {
        var item = new RailNavItem
        {
            Brand = "financeiro",
            Label = "Financeiro",
            Type = RailNavItemType.Route,
            RoutePrefixes = new[] { "/financeiro" }
        };
        Assert.True(item.MatchesPath("financeiro/overview"));
    }

    [Fact]
    public void Extended_RailNavItem_IsDefault()
    {
        var item = new RailNavItem
        {
            Brand = "home",
            Label = "Home",
            Type = RailNavItemType.Route,
            IsDefault = true
        };
        Assert.True(item.IsDefault);
        Assert.True(item.MatchesPath("/"));
    }

    [Fact]
    public void Extended_Enum_RailNavItemType_Has3Values()
    {
        Assert.Equal(3, Enum.GetValues<RailNavItemType>().Length);
        Assert.Contains(RailNavItemType.Route, Enum.GetValues<RailNavItemType>());
        Assert.Contains(RailNavItemType.Action, Enum.GetValues<RailNavItemType>());
        Assert.Contains(RailNavItemType.Separator, Enum.GetValues<RailNavItemType>());
    }
}
