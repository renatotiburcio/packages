using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconCatalogTests
{
    [Fact]
    public void Register_AndGet_RoundTrips()
    {
        var info = new XFIconInfo("house", XFIconVariant.Regular, XFIconSize.Size24,
            new[] { "home", "main" }, "navigation", "<svg/>");
        XFIconCatalog.Register(info);
        var fetched = XFIconCatalog.Get("house", XFIconVariant.Regular, XFIconSize.Size24);
        Assert.NotNull(fetched);
        Assert.Equal("navigation", fetched.Category);
    }

    [Fact]
    public void Get_UnknownName_ReturnsNull()
    {
        Assert.Null(XFIconCatalog.Get("nonexistent-xyz", XFIconVariant.Regular, XFIconSize.Size16));
    }

    [Fact]
    public void Search_MatchesByKeyword()
    {
        var info = new XFIconInfo("envelope", XFIconVariant.Regular, XFIconSize.Size20,
            new[] { "mail", "email" }, "communication", "<svg/>");
        XFIconCatalog.Register(info);
        var results = XFIconCatalog.Search("mail").ToList();
        Assert.Contains(results, r => r.Name == "envelope");
    }

    [Fact]
    public void Search_EmptyTerm_ReturnsNothing()
    {
        Assert.Empty(XFIconCatalog.Search(""));
    }

    [Fact]
    public void Count_IsNonNegative()
    {
        Assert.True(XFIconCatalog.Count >= 0);
    }
}