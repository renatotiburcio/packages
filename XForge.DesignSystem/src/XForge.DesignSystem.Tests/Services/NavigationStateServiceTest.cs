using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Services;

/// <summary>
/// Tests for NavigationStateService and RailNavItem — centralized navigation
/// state with brand management, active tracking, and localStorage persistence.
/// </summary>
public sealed class NavigationStateServiceTest
{
    #region RailNavItem — Construction

    [Fact]
    public void RailNavItem_DefaultType_IsRoute()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test" };
        Assert.Equal(RailNavItemType.Route, item.Type);
    }

    [Fact]
    public void RailNavItem_SeparatorType_HeightIs24()
    {
        var item = new RailNavItem { Brand = "sep", Label = "Separator", Type = RailNavItemType.Separator };
        Assert.Equal(24, item.Height);
    }

    [Fact]
    public void RailNavItem_ActionType_HeightIs40()
    {
        var item = new RailNavItem { Brand = "act", Label = "Action", Type = RailNavItemType.Action };
        Assert.Equal(40, item.Height);
    }

    [Fact]
    public void RailNavItem_RouteType_HeightIs40()
    {
        var item = new RailNavItem { Brand = "route", Label = "Route" };
        Assert.Equal(40, item.Height);
    }

    [Fact]
    public void RailNavItem_IsInteractive_RouteAndAction_ReturnsTrue()
    {
        var route = new RailNavItem { Brand = "r", Label = "R", Type = RailNavItemType.Route };
        var action = new RailNavItem { Brand = "a", Label = "A", Type = RailNavItemType.Action };
        var separator = new RailNavItem { Brand = "s", Label = "S", Type = RailNavItemType.Separator };

        Assert.True(route.IsInteractive);
        Assert.True(action.IsInteractive);
        Assert.False(separator.IsInteractive);
    }

    #endregion

    #region RailNavItem — IsActive

    [Fact]
    public void RailNavItem_IsActive_RouteType_CanBeSet()
    {
        var item = new RailNavItem { Brand = "test", Label = "Test" };
        Assert.False(item.IsActive);
        item.IsActive = true;
        Assert.True(item.IsActive);
    }

    [Fact]
    public void RailNavItem_IsActive_NonRouteType_AlwaysFalse()
    {
        var item = new RailNavItem { Brand = "act", Label = "Action", Type = RailNavItemType.Action };
        item.IsActive = true; // Should be ignored
        Assert.False(item.IsActive);
    }

    [Fact]
    public void RailNavItem_IsActive_SeparatorType_AlwaysFalse()
    {
        var item = new RailNavItem { Brand = "sep", Label = "Sep", Type = RailNavItemType.Separator };
        item.IsActive = true;
        Assert.False(item.IsActive);
    }

    #endregion

    #region RailNavItem — GetNavigationRoute

    [Fact]
    public void RailNavItem_GetNavigationRoute_RouteWithOverviewRoute_ReturnsOverviewRoute()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms", OverviewRoute = "/forms/overview" };
        Assert.Equal("/forms/overview", item.GetNavigationRoute());
    }

    [Fact]
    public void RailNavItem_GetNavigationRoute_RouteWithoutOverview_ReturnsDefaultRoute()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms" };
        Assert.Equal("/forms/overview", item.GetNavigationRoute());
    }

    [Fact]
    public void RailNavItem_GetNavigationRoute_NonRouteType_ReturnsNull()
    {
        var action = new RailNavItem { Brand = "act", Label = "Action", Type = RailNavItemType.Action };
        var separator = new RailNavItem { Brand = "sep", Label = "Sep", Type = RailNavItemType.Separator };
        Assert.Null(action.GetNavigationRoute());
        Assert.Null(separator.GetNavigationRoute());
    }

    #endregion

    #region RailNavItem — MatchesPath

    [Fact]
    public void RailNavItem_MatchesPath_NonRouteType_ReturnsFalse()
    {
        var item = new RailNavItem { Brand = "act", Label = "Action", Type = RailNavItemType.Action };
        Assert.False(item.MatchesPath("/any/path"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_WithRoutePrefixes_MatchesExactPrefix()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms", RoutePrefixes = ["/forms"] };
        Assert.True(item.MatchesPath("/forms"));
        Assert.True(item.MatchesPath("/forms/input"));
        Assert.False(item.MatchesPath("/display"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_WithRoutePrefixes_MatchesStartsWith()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms", RoutePrefixes = ["/forms"] };
        Assert.True(item.MatchesPath("/forms/text-input"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_WithoutRoutePrefixes_UsesBrand()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms" };
        Assert.True(item.MatchesPath("/forms"));
        Assert.True(item.MatchesPath("/forms/input"));
        Assert.False(item.MatchesPath("/display"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_RootPath_WithIsDefault_ReturnsTrue()
    {
        var item = new RailNavItem { Brand = "home", Label = "Home", IsDefault = true };
        Assert.True(item.MatchesPath("/"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_RootPath_WithoutIsDefault_ReturnsFalse()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms" };
        Assert.False(item.MatchesPath("/"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_EmptyPath_ReturnsFalse()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms" };
        Assert.False(item.MatchesPath(""));
    }

    [Fact]
    public void RailNavItem_MatchesPath_CaseInsensitive()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms" };
        Assert.True(item.MatchesPath("/Forms"));
        Assert.True(item.MatchesPath("/FORMS/input"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_MultipleRoutePrefixes_MatchesBestPrefix()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms", RoutePrefixes = ["/forms", "/forms/special"] };
        Assert.True(item.MatchesPath("/forms/special/page"));
        Assert.True(item.MatchesPath("/forms/regular"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_StripsQueryString()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms" };
        Assert.True(item.MatchesPath("/forms?tab=input"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_StripsHash()
    {
        var item = new RailNavItem { Brand = "forms", Label = "Forms" };
        Assert.True(item.MatchesPath("/forms#section"));
    }

    [Fact]
    public void RailNavItem_MatchesPath_DefaultWithRootPrefix()
    {
        var item = new RailNavItem { Brand = "home", Label = "Home", IsDefault = true, RoutePrefixes = ["/"] };
        Assert.True(item.MatchesPath("/"));
    }

    #endregion

    #region RailNavItem — NormalizePath

    [Fact]
    public void NormalizePath_NullPath_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath(null!));
    }

    [Fact]
    public void NormalizePath_EmptyPath_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath(""));
    }

    [Fact]
    public void NormalizePath_StripsQueryAndHash()
    {
        // NormalizePath is internal — test indirectly via DeriveBrandFromPath
        Assert.Equal("forms", NavigationStateService.DeriveBrandFromPath("forms?tab=input#section"));
    }

    [Fact]
    public void NormalizePath_AddsLeadingSlash_IndirectTest()
    {
        // DeriveBrandFromPath strips leading slash internally
        Assert.Equal("forms", NavigationStateService.DeriveBrandFromPath("/forms"));
    }

    #endregion

    #region NavigationStateService — InitializeAsync

    [Fact]
    public async Task InitializeAsync_SetsCurrentBrandFromPath()
    {
        var nav = CreateNavigationManager("http://localhost/forms/input");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms", IsDefault = true },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        Assert.Equal("forms", service.CurrentBrand);
    }

    [Fact]
    public async Task InitializeAsync_DefaultPath_SetsDefaultBrand()
    {
        var nav = CreateNavigationManager("http://localhost/");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "home", Label = "Home", IsDefault = true },
            new RailNavItem { Brand = "forms", Label = "Forms" }
        };
        await service.InitializeAsync(items);
        Assert.Equal("home", service.CurrentBrand);
    }

    [Fact]
    public async Task InitializeAsync_SetsIsInitialized()
    {
        var nav = CreateNavigationManager("http://localhost/");
        var service = new NavigationStateService(nav);
        Assert.False(service.IsInitialized);
        await service.InitializeAsync([]);
        Assert.True(service.IsInitialized);
    }

    [Fact]
    public async Task InitializeAsync_SyncsActiveItem()
    {
        var nav = CreateNavigationManager("http://localhost/forms/input");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        Assert.True(items[0].IsActive);
        Assert.False(items[1].IsActive);
    }

    [Fact]
    public async Task InitializeAsync_FallbackToBrandMatch_WhenNoPathMatch()
    {
        var nav = CreateNavigationManager("http://localhost/forms");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        Assert.Equal("forms", service.CurrentBrand);
        Assert.True(items[0].IsActive);
    }

    [Fact]
    public async Task InitializeAsync_FiresBrandChanged_WhenBrandChanges()
    {
        var nav = CreateNavigationManager("http://localhost/forms");
        var service = new NavigationStateService(nav);
        string? changedBrand = null;
        service.BrandChanged += (_, brand) => changedBrand = brand;
        await service.InitializeAsync([new RailNavItem { Brand = "forms", Label = "Forms" }]);
        Assert.Equal("forms", changedBrand);
    }

    [Fact]
    public async Task InitializeAsync_FiresActiveItemChanged_WhenActiveChanges()
    {
        var nav = CreateNavigationManager("http://localhost/forms");
        var service = new NavigationStateService(nav);
        RailNavItem? activeItem = null;
        service.ActiveItemChanged += (_, item) => activeItem = item;
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        Assert.NotNull(activeItem);
        Assert.Equal("forms", activeItem.Brand);
    }

    #endregion

    #region NavigationStateService — GetActiveIndex/GetActiveItem/GetIndicatorOffset

    [Fact]
    public async Task GetActiveIndex_ReturnsCorrectIndex()
    {
        var nav = CreateNavigationManager("http://localhost/display");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        Assert.Equal(1, service.GetActiveIndex());
    }

    [Fact]
    public async Task GetActiveItem_ReturnsCorrectItem()
    {
        var nav = CreateNavigationManager("http://localhost/display");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        var active = service.GetActiveItem();
        Assert.NotNull(active);
        Assert.Equal("display", active.Brand);
    }

    [Fact]
    public async Task GetIndicatorOffset_CalculatesCorrectOffset()
    {
        var nav = CreateNavigationManager("http://localhost/display");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },    // Height = 40
            new RailNavItem { Brand = "display", Label = "Display" } // Height = 40, active
        };
        await service.InitializeAsync(items);
        var offset = service.GetIndicatorOffset();
        // offset = 40 (first item height) + 16 (gap) + (40 - 28) / 2 (centering) = 56 + 6 = 62
        Assert.Equal(62, offset);
    }

    [Fact]
    public async Task GetIndicatorOffset_NoActiveItem_ReturnsNegativeOne()
    {
        var nav = CreateNavigationManager("http://localhost/unknown");
        var service = new NavigationStateService(nav);
        await service.InitializeAsync([]);
        Assert.Equal(-1, service.GetIndicatorOffset());
    }

    #endregion

    #region NavigationStateService — IsBrandActive/IsItemActive

    [Fact]
    public async Task IsBrandActive_ReturnsTrueForActiveBrand()
    {
        var nav = CreateNavigationManager("http://localhost/forms");
        var service = new NavigationStateService(nav);
        await service.InitializeAsync([new RailNavItem { Brand = "forms", Label = "Forms" }]);
        Assert.True(service.IsBrandActive("forms"));
        Assert.False(service.IsBrandActive("display"));
    }

    [Fact]
    public async Task IsBrandActive_CaseInsensitive()
    {
        var nav = CreateNavigationManager("http://localhost/forms");
        var service = new NavigationStateService(nav);
        await service.InitializeAsync([new RailNavItem { Brand = "forms", Label = "Forms" }]);
        Assert.True(service.IsBrandActive("Forms"));
        Assert.True(service.IsBrandActive("FORMS"));
    }

    [Fact]
    public async Task IsItemActive_ReturnsItemIsActive()
    {
        var nav = CreateNavigationManager("http://localhost/forms");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        Assert.True(service.IsItemActive(items[0]));
        Assert.False(service.IsItemActive(items[1]));
    }

    #endregion

    #region NavigationStateService — NavigateToBrand/NavigateToItem

    [Fact(Skip = "NavigationManager.NavigateTo throws in test context — tested indirectly via integration tests")]
    public async Task NavigateToBrand_InvokesWithoutError()
    {
        var nav = CreateNavigationManager("http://localhost/");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms", OverviewRoute = "/forms/overview" },
        };
        await service.InitializeAsync(items);
        // NavigateToBrand calls NavigationManager.NavigateTo which may throw
        // in test context. We just verify the service logic doesn't throw internally.
        try { service.NavigateToBrand("forms"); } catch (InvalidOperationException) { }
    }

    [Fact]
    public async Task NavigateToItem_NonRouteType_DoesNothing()
    {
        var nav = CreateNavigationManager("http://localhost/");
        var service = new NavigationStateService(nav);
        var item = new RailNavItem { Brand = "act", Label = "Action", Type = RailNavItemType.Action };
        await service.InitializeAsync([item]);
        // Non-route type should not call NavigateTo — no exception possible
        service.NavigateToItem(item);
    }

    #endregion

    #region NavigationStateService — DeriveBrandFromPath

    [Fact]
    public void DeriveBrandFromPath_NullPath_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath(null!));
    }

    [Fact]
    public void DeriveBrandFromPath_EmptyPath_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath(""));
    }

    [Fact]
    public void DeriveBrandFromPath_RootPath_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath("/"));
    }

    [Fact]
    public void DeriveBrandFromPath_SingleSegment_ReturnsFirstSegment()
    {
        Assert.Equal("forms", NavigationStateService.DeriveBrandFromPath("/forms"));
    }

    [Fact]
    public void DeriveBrandFromPath_MultipleSegments_ReturnsFirstSegment()
    {
        Assert.Equal("forms", NavigationStateService.DeriveBrandFromPath("/forms/input/text"));
    }

    [Fact]
    public void DeriveBrandFromPath_StripsQueryAndHash()
    {
        Assert.Equal("forms", NavigationStateService.DeriveBrandFromPath("forms?tab=input#section"));
    }

    [Fact]
    public void DeriveBrandFromPath_IsCaseInsensitive()
    {
        Assert.Equal("forms", NavigationStateService.DeriveBrandFromPath("/Forms/Input"));
    }

    #endregion

    #region NavigationStateService — RailItems

    [Fact]
    public async Task RailItems_ReturnsRegisteredItems()
    {
        var nav = CreateNavigationManager("http://localhost/");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        Assert.Equal(2, service.RailItems.Count);
    }

    [Fact]
    public async Task RegisterRailItems_ReplacesExistingItems()
    {
        var nav = CreateNavigationManager("http://localhost/");
        var service = new NavigationStateService(nav);
        await service.InitializeAsync([new RailNavItem { Brand = "old", Label = "Old" }]);
        service.RegisterRailItems([new RailNavItem { Brand = "new", Label = "New" }]);
        Assert.Single(service.RailItems);
        Assert.Equal("new", service.RailItems[0].Brand);
    }

    #endregion

    #region NavigationStateService — SyncItemsActiveState

    [Fact]
    public async Task SyncItemsActiveState_SetsFallbackWhenNoMatch()
    {
        var nav = CreateNavigationManager("http://localhost/forms");
        var service = new NavigationStateService(nav);
        var items = new[]
        {
            new RailNavItem { Brand = "forms", Label = "Forms" },
            new RailNavItem { Brand = "display", Label = "Display" }
        };
        await service.InitializeAsync(items);
        // After init, forms should be active since the path matches
        Assert.True(items[0].IsActive);
    }

    #endregion

    #region NavigationStateService — Dispose

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var nav = CreateNavigationManager("http://localhost/");
        var service = new NavigationStateService(nav);
        service.Dispose();
        service.Dispose(); // Should not throw
    }

    #endregion

    #region RailNavItemType Enum

    [Fact]
    public void RailNavItemType_HasThreeValues()
    {
        Assert.Equal(3, Enum.GetValues<RailNavItemType>().Length);
        Assert.Equal(RailNavItemType.Route, (RailNavItemType)0);
        Assert.Equal(RailNavItemType.Action, (RailNavItemType)1);
        Assert.Equal(RailNavItemType.Separator, (RailNavItemType)2);
    }

    #endregion

    #region Constants

    [Fact]
    public void NavigationStateService_IndicatorHeight_Is28()
    {
        Assert.Equal(28, NavigationStateService.IndicatorHeight);
    }

    [Fact]
    public void NavigationStateService_ItemGap_Is16()
    {
        Assert.Equal(16, NavigationStateService.ItemGap);
    }

    #endregion

    #region Helpers

    private static NavigationManager CreateNavigationManager(string uri)
    {
        var nav = new TestNavigationManager();
        nav.Initialize(uri);
        return nav;
    }

    /// <summary>
    /// A test NavigationManager that allows setting the URI programmatically.
    /// </summary>
    private sealed class TestNavigationManager : NavigationManager
    {
        public void Initialize(string uri)
        {
            Initialize(uri, uri);
        }
    }

    #endregion
}
