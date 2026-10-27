﻿using XForge.DesignSystem.Services;
using Xunit;

namespace XForge.DesignSystem.Tests.Services;

public sealed class RailNavItemTypeTests
{
    [Theory]
    [InlineData(RailNavItemType.Route, 0)]
    [InlineData(RailNavItemType.Action, 1)]
    [InlineData(RailNavItemType.Separator, 2)]
    public void Enum_HasExpectedIntValue(RailNavItemType type, int expected) =>
        Assert.Equal(expected, (int)type);

    [Fact]
    public void Enum_HasExactlyThreeMembers() =>
        Assert.Equal(3, Enum.GetNames<RailNavItemType>().Length);

    [Fact]
    public void Route_ToString_ReturnsRoute() =>
        Assert.Equal("Route", RailNavItemType.Route.ToString());

    [Fact]
    public void Action_ToString_ReturnsAction() =>
        Assert.Equal("Action", RailNavItemType.Action.ToString());

    [Fact]
    public void Separator_ToString_ReturnsSeparator() =>
        Assert.Equal("Separator", RailNavItemType.Separator.ToString());
}

public sealed class RailNavItemIsActiveTests
{
    private static RailNavItem MakeItem(RailNavItemType type, string brand = "test") => new()
    {
        Type = type, Brand = brand, Label = "Test", IsActive = false
    };

    [Fact]
    public void Route_CanSetIsActiveTrue()
    {
        var item = MakeItem(RailNavItemType.Route);
        item.IsActive = true;
        Assert.True(item.IsActive);
    }

    [Fact]
    public void Route_CanSetIsActiveFalse()
    {
        var item = MakeItem(RailNavItemType.Route);
        item.IsActive = true;
        item.IsActive = false;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Action_SetIsActiveTrue_ReturnsFalse()
    {
        var item = MakeItem(RailNavItemType.Action);
        item.IsActive = true;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Action_SetIsActiveFalse_ReturnsFalse()
    {
        var item = MakeItem(RailNavItemType.Action);
        item.IsActive = false;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Separator_SetIsActiveTrue_ReturnsFalse()
    {
        var item = MakeItem(RailNavItemType.Separator);
        item.IsActive = true;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Separator_SetIsActiveFalse_ReturnsFalse()
    {
        var item = MakeItem(RailNavItemType.Separator);
        item.IsActive = false;
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Separator_AlwaysReturnsFalse()
    {
        var item = MakeItem(RailNavItemType.Separator);
        item.IsActive = false;
        item.IsActive = true;
        item.IsActive = false;
        Assert.False(item.IsActive);
    }
}

public sealed class RailNavItemHeightTests
{
    [Fact]
    public void Route_Height_Is40() =>
        Assert.Equal(40, new RailNavItem { Type = RailNavItemType.Route, Brand = "x", Label = "x" }.Height);

    [Fact]
    public void Action_Height_Is40() =>
        Assert.Equal(40, new RailNavItem { Type = RailNavItemType.Action, Brand = "x", Label = "x" }.Height);

    [Fact]
    public void Separator_Height_Is24() =>
        Assert.Equal(24, new RailNavItem { Type = RailNavItemType.Separator, Brand = "x", Label = "x" }.Height);
}

public sealed class RailNavItemIsInteractiveTests
{
    [Fact]
    public void Route_IsInteractive_True() =>
        Assert.True(new RailNavItem { Type = RailNavItemType.Route, Brand = "x", Label = "x" }.IsInteractive);

    [Fact]
    public void Action_IsInteractive_True() =>
        Assert.True(new RailNavItem { Type = RailNavItemType.Action, Brand = "x", Label = "x" }.IsInteractive);

    [Fact]
    public void Separator_IsInteractive_False() =>
        Assert.False(new RailNavItem { Type = RailNavItemType.Separator, Brand = "x", Label = "x" }.IsInteractive);
}

public sealed class RailNavItemGetNavigationRouteTests
{
    [Fact]
    public void Separator_ReturnsNull() =>
        Assert.Null(new RailNavItem { Type = RailNavItemType.Separator, Brand = "x", Label = "x" }
            .GetNavigationRoute());

    [Fact]
    public void Action_ReturnsNull() =>
        Assert.Null(new RailNavItem { Type = RailNavItemType.Action, Brand = "x", Label = "x" }
            .GetNavigationRoute());

    [Fact]
    public void Route_WithOverviewRoute_ReturnsOverviewRoute() =>
        Assert.Equal("/custom/route", new RailNavItem
        {
            Type = RailNavItemType.Route, Brand = "test", Label = "x", OverviewRoute = "/custom/route"
        }.GetNavigationRoute());

    [Fact]
    public void Route_WithoutOverviewRoute_ReturnsBrandOverview() =>
        Assert.Equal("/mytest/overview", new RailNavItem
        {
            Type = RailNavItemType.Route, Brand = "MyTest", Label = "x"
        }.GetNavigationRoute());

    [Fact]
    public void Route_BrandIsLowercasedInRoute() =>
        Assert.Equal("/uppercase/overview", new RailNavItem
        {
            Type = RailNavItemType.Route, Brand = "UPPERCASE", Label = "x"
        }.GetNavigationRoute());
}

public sealed class RailNavItemMatchesPathTests
{
    private static RailNavItem RouteItem(string brand, bool isDefault = false,
        string[]? prefixes = null) => new()
    {
        Type = RailNavItemType.Route,
        Brand = brand,
        Label = "x",
        IsDefault = isDefault,
        RoutePrefixes = prefixes
    };

    [Fact]
    public void Separator_ReturnsFalse() =>
        Assert.False(new RailNavItem
        {
            Type = RailNavItemType.Separator, Brand = "x", Label = "x"
        }.MatchesPath("/anything"));

    [Fact]
    public void Action_ReturnsFalse() =>
        Assert.False(new RailNavItem
        {
            Type = RailNavItemType.Action, Brand = "x", Label = "x"
        }.MatchesPath("/anything"));

    [Fact]
    public void RootPath_WithIsDefault_ReturnsTrue() =>
        Assert.True(RouteItem("test", isDefault: true).MatchesPath("/"));

    [Fact]
    public void RootPath_WithoutIsDefault_ReturnsFalse() =>
        Assert.False(RouteItem("test", isDefault: false).MatchesPath("/"));

    [Fact]
    public void NormalizedEmptyPath_WithoutIsDefault_ReturnsFalse() =>
        Assert.False(RouteItem("test", isDefault: false).MatchesPath(""));

    [Fact]
    public void RoutePrefixes_ExactMatch_ReturnsTrue() =>
        Assert.True(RouteItem("test", prefixes: ["/myroute"]).MatchesPath("/myroute"));

    [Fact]
    public void RoutePrefixes_StartsWithMatch_ReturnsTrue() =>
        Assert.True(RouteItem("test", prefixes: ["/myroute"]).MatchesPath("/myroute/deep"));

    [Fact]
    public void RoutePrefixes_NoMatch_ReturnsFalse() =>
        Assert.False(RouteItem("test", prefixes: ["/myroute"]).MatchesPath("/other"));

    [Fact]
    public void RoutePrefixes_CaseInsensitiveMatch_ReturnsTrue() =>
        Assert.True(RouteItem("test", prefixes: ["/MyRoute"]).MatchesPath("/myroute"));

    [Fact]
    public void RoutePrefixes_MultiplePrefixes_AnyMatchReturnsTrue() =>
        Assert.True(RouteItem("test", prefixes: ["/alpha", "/beta", "/gamma"])
            .MatchesPath("/beta"));

    [Fact]
    public void RoutePrefixes_RootPrefix_RequiresIsDefault() =>
        Assert.True(RouteItem("test", isDefault: true, prefixes: ["/"]).MatchesPath("/"));

    [Fact]
    public void RoutePrefixes_RootPrefix_WithoutIsDefault_ReturnsFalse() =>
        Assert.False(RouteItem("test", isDefault: false, prefixes: ["/"]).MatchesPath("/"));


    [Fact]
    public void NoPrefixes_BrandMatch_ReturnsTrue() =>
        Assert.True(RouteItem("Components").MatchesPath("/components/buttons"));

    [Fact]
    public void NoPrefixes_BrandMismatch_ReturnsFalse() =>
        Assert.False(RouteItem("Forms").MatchesPath("/components/buttons"));

    [Fact]
    public void NoPrefixes_BrandExactMatch_ReturnsTrue() =>
        Assert.True(RouteItem("Components").MatchesPath("/Components"));

    [Fact]
    public void NoPrefixes_BrandCaseInsensitive_ReturnsTrue() =>
        Assert.True(RouteItem("Components").MatchesPath("/COMPONENTS"));
}

public sealed class RailNavItemNormalizePathTests
{
    [Fact]
    public void Null_ReturnsEmpty() =>
        Assert.Equal(string.Empty, RailNavItem.NormalizePath(null!));

    [Fact]
    public void Whitespace_ReturnsEmpty() =>
        Assert.Equal(string.Empty, RailNavItem.NormalizePath("   "));

    [Fact]
    public void Empty_ReturnsEmpty() =>
        Assert.Equal(string.Empty, RailNavItem.NormalizePath(string.Empty));

    [Fact]
    public void TrimsWhitespace() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("  /path  "));

    [Fact]
    public void StripsQueryString() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("/path?query=1"));

    [Fact]
    public void StripsHashFragment() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("/path#section"));

    [Fact]
    public void StripsBothQueryAndHash() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("/path?q=1#section"));

    [Fact]
    public void QueryBeforeHash_StripsBoth() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("/path?q=1#frag"));

    [Fact]
    public void HashBeforeQuery_StripsBoth() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("/path#frag?q=1"));

    [Fact]
    public void AddsLeadingSlash() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("path"));

    [Fact]
    public void NormalizesTrailingSlash() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("/path/"));

    [Fact]
    public void MultipleTrailingSlashes_Normalized() =>
        Assert.Equal("/path", RailNavItem.NormalizePath("/path///"));

    [Fact]
    public void RootSlash_StaysRoot() =>
        Assert.Equal("/", RailNavItem.NormalizePath("/"));

    [Fact]
    public void RootSlashWithSpaces_Normalized() =>
        Assert.Equal("/", RailNavItem.NormalizePath("  /  "));

    [Fact]
    public void QueryOnly_StripsQuery() =>
        Assert.Equal("/", RailNavItem.NormalizePath("/?query=1"));

    [Fact]
    public void HashOnly_WithLeadingSlash() =>
        Assert.Equal("/test", RailNavItem.NormalizePath("test#frag"));
}

public sealed class DeriveBrandFromPathTests
{
    [Fact]
    public void Null_ReturnsEmpty() =>
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath(null!));

    [Fact]
    public void Empty_ReturnsEmpty() =>
        Assert.Equal(string.Empty, NavigationStateService.DeriveBrandFromPath(string.Empty));

    [Fact]
    public void SingleSegment_ReturnsBrand() =>
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/components"));

    [Fact]
    public void NestedPath_ReturnsFirstSegment() =>
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/components/buttons"));

    [Fact]
    public void StripsQueryBeforeExtracting() =>
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/components?view=full"));

    [Fact]
    public void StripsHashBeforeExtracting() =>
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/components#intro"));

    [Fact]
    public void NormalizesToLowercase() =>
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("/Components"));

    [Fact]
    public void MultipleLeadingSlashes_ReturnsBrand() =>
        Assert.Equal("components", NavigationStateService.DeriveBrandFromPath("//components"));

    [Fact]
    public void PathWithQueryAndHash_ReturnsCorrectBrand() =>
        Assert.Equal("tokens",
            NavigationStateService.DeriveBrandFromPath("/tokens/colors?page=1#primary"));
}
