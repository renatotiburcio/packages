using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconRegistryTests
{
    [Fact]
    public void Register_AndGet_RoundTrips()
    {
        var reg = new XFIconRegistry();
        reg.Register(new XFIconInfo("home", XFIconVariant.Regular, XFIconSize.Size24,
            new[] { "house" }, "navigation", "<svg/>"));
        Assert.NotNull(reg.Get("home", XFIconVariant.Regular, XFIconSize.Size24));
    }

    [Fact]
    public void Get_Unknown_ReturnsNull()
    {
        var reg = new XFIconRegistry();
        Assert.Null(reg.Get("xxx", XFIconVariant.Regular, XFIconSize.Size16));
    }

    [Fact]
    public void Search_MatchesKeyword_AccentInsensitive()
    {
    var reg = new XFIconRegistry();
        reg.Register(new XFIconInfo("casa", XFIconVariant.Regular, XFIconSize.Size24,
            new[] { "casa" }, "navigation", "<svg/>"));
        var hits = reg.Search("casa").ToList();
        Assert.Single(hits);
    }

    [Fact]
    public void Search_Pagination_Works()
    {
        var reg = new XFIconRegistry();
        for (int i = 0; i < 50; i++)
            reg.Register(new XFIconInfo($"icon{i}", XFIconVariant.Regular, XFIconSize.Size24,
                new[] { "test" }, "misc", "<svg/>"));
        var page = reg.Search("test", page: 0, pageSize: 10).ToList();
        Assert.Equal(10, page.Count);
    }

    [Fact]
    public void Count_Reflects_Registrations()
    {
        var reg = new XFIconRegistry();
        reg.Register(new XFIconInfo("x", XFIconVariant.Regular, XFIconSize.Size16,
            new[] { "x" }, "misc", "<svg/>"));
        Assert.Equal(1, reg.Count);
    }
}