using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using XForge.DesignSystem.Services;
using Xunit;

namespace XForge.DesignSystem.Tests.Services;

public sealed class NavigationStateServiceExtendedTest
{
    private static NavigationStateService CreateService(string baseUri = "http://localhost/", string currentUri = "http://localhost/")
    {
        var nav = new TestNavigationManager(baseUri, currentUri);
        return new NavigationStateService(nav);
    }

    private static List<RailNavItem> CreateItems() => new()
    {
        new RailNavItem { Brand = "components", Label = "Components", IsDefault = true, Type = RailNavItemType.Route },
        new RailNavItem { Brand = "tokens", Label = "Tokens", Type = RailNavItemType.Route },
        new RailNavItem { Brand = "forms", Label = "Forms", Type = RailNavItemType.Route },
        new RailNavItem { Brand = "sep1", Label = "", Type = RailNavItemType.Separator },
        new RailNavItem { Brand = "settings", Label = "Settings", Type = RailNavItemType.Action }
    };

    [Fact]
    public void Extended_RailNavItem_CanCreate()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Route };
        Assert.Equal("test", item.Brand);
        Assert.Equal("Test", item.Label);
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Extended_RailNavItem_IsActive_RouteType()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Route };
        item.IsActive = true;
        Assert.True(item.IsActive);
    }

    [Fact]
    public void Extended_RailNavItem_IsActive_ActionType_AlwaysFalse()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Action };
        item.IsActive = true;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Extended_RailNavItem_IsActive_SeparatorType_AlwaysFalse()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Separator };
        item.IsActive = true;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Extended_RailNavItem_Height_Route()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Route };
        Assert.Equal(40, item.Height);
    }

    [Fact]
    public void Extended_RailNavItem_Height_Separator()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Separator };
        Assert.Equal(24, item.Height);
    }

    [Fact]
    public void Extended_RailNavItem_IsInteractive_Route()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Route };
        Assert.True(item.IsInteractive);
    }

    [Fact]
    public void Extended_RailNavItem_IsInteractive_Separator()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Separator };
        Assert.False(item.IsInteractive);
    }

    [Fact]
    public void Extended_RailNavItem_GetNavigationRoute_WithOverview()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", OverviewRoute = "/custom/overview" };
        Assert.Equal("/custom/overview", item.GetNavigationRoute());
    }

    [Fact]
    public void Extended_RailNavItem_GetNavigationRoute_Default()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test" };
        Assert.Equal("/test/overview", item.GetNavigationRoute());
    }

    [Fact]
    public void Extended_RailNavItem_GetNavigationRoute_ActionType_ReturnsNull()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Action };
        Assert.Null(item.GetNavigationRoute());
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_ByBrand()
    {
        var item = new RailNavItem { Brand = "components", Label = "Components" };
        Assert.True(item.MatchesPath("/components/button"));
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_ByPrefix()
    {
        var item = new RailNavItem { Brand = "comp", Label = "Comp", RoutePrefixes = new[] { "/components" } };
        Assert.True(item.MatchesPath("/components/button"));
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_DefaultItem_Root()
    {
        var item = new RailNavItem { Brand = "home", Label = "Home", IsDefault = true };
        Assert.True(item.MatchesPath("/"));
    }

    [Fact]
    public void Extended_RailNavItem_MatchesPath_ActionType_ReturnsFalse()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test", Type = RailNavItemType.Action };
        Assert.False(item.MatchesPath("/test"));
    }

    [Fact]
    public void Extended_RailNavItemType_EnumHasAllValues()
    {
        Assert.Contains(RailNavItemType.Route, Enum.GetValues<RailNavItemType>());
        Assert.Contains(RailNavItemType.Action, Enum.GetValues<RailNavItemType>());
        Assert.Contains(RailNavItemType.Separator, Enum.GetValues<RailNavItemType>());
    }

    [Fact]
    public void Extended_DeriveBrandFromPath_Simple()
    {
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/components/button"));
    }

    [Fact]
    public void Extended_DeriveBrandFromPath_Empty()
    {
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath("/"));
    }

    [Fact]
    public void Extended_DeriveBrandFromPath_Null()
    {
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath(null!));
    }

    [Fact]
    public void Extended_DeriveBrandFromPath_WithQuery()
    {
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/components/button?tab=api"));
    }

    [Fact]
    public void Extended_DeriveBrandFromPath_WithHash()
    {
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/components/button#section"));
    }

    [Fact]
    public void Extended_NavigationStateService_RegisterRailItems()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        Assert.Equal(5, service.RailItems.Count);
    }

    [Fact]
    public void Extended_NavigationStateService_NavigateToBrand_DoesNotThrow()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        service.NavigateToBrand("components");
        // Should not throw — NavigateToBrand calls NavigateTo which is a no-op in our mock
    }

    [Fact]
    public void Extended_NavigationStateService_NavigateToItem_DoesNotThrow()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        var item = service.RailItems.First();
        service.NavigateToItem(item);
        // Should not throw
    }

    [Fact]
    public void Extended_NavigationStateService_SyncItemsActiveState_DoesNotThrow()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        service.SyncItemsActiveState();
        // Should not throw — items will have IsActive = false since _currentBrand is empty
    }

    [Fact]
    public void Extended_NavigationStateService_GetActiveIndex_NoActive_ReturnsMinusOne()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        // Without initialization, no item is active
        Assert.Equal(-1, service.GetActiveIndex());
    }

    [Fact]
    public void Extended_NavigationStateService_GetActiveItem_NoActive_ReturnsNull()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        Assert.Null(service.GetActiveItem());
    }

    [Fact]
    public void Extended_NavigationStateService_GetIndicatorOffset_NoActive_ReturnsMinusOne()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        Assert.Equal(-1, service.GetIndicatorOffset());
    }

    [Fact]
    public void Extended_NavigationStateService_IsBrandActive_NoBrand_ReturnsFalse()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        Assert.False(service.IsBrandActive("components"));
    }

    [Fact]
    public void Extended_NavigationStateService_IsItemActive_NoActive_ReturnsFalse()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        var item = service.RailItems.First();
        Assert.False(service.IsItemActive(item));
    }

    [Fact]
    public void Extended_NavigationStateService_NavigateToItem_ActionType_DoesNothing()
    {
        var nav = new TestNavigationManager("http://localhost/", "http://localhost/");
        var service = new NavigationStateService(nav);
        var actionItem = new RailNavItem { Brand = "settings", Label = "Settings", Type = RailNavItemType.Action };
        service.NavigateToItem(actionItem);
        // Should not throw
    }

    [Fact]
    public void Extended_NavigationStateService_Dispose()
    {
        var service = CreateService();
        service.RegisterRailItems(CreateItems());
        service.Dispose();
        // Should not throw
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager(string baseUri = "http://localhost/", string currentUri = "http://localhost/")
        {
            Initialize(baseUri, currentUri);
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            // no-op for tests — we test brand logic separately
        }
    }
}
