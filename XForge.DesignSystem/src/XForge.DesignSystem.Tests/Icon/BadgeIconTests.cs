using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class BadgeIconTests
{
    [Fact]
    public void Catalog_Has8Brands()
    {
        Assert.Equal(8, BrandIcons.Catalog.Count);
        Assert.Contains("Microsoft", BrandIcons.Names);
        Assert.Contains("Google", BrandIcons.Names);
        Assert.Contains("GitHub", BrandIcons.Names);
    }

    [Fact]
    public void Microsoft_Logo_HasFourColoredQuads()
    {
        var svg = BrandIcons.Catalog["Microsoft"];
        Assert.Contains("f25022", svg); Assert.Contains("7fba00", svg);
        Assert.Contains("00a4ef", svg); Assert.Contains("ffb900", svg);
    }

    [Fact]
    public void Parameters_DocumentationPresent()
    {
        Assert.False(string.IsNullOrEmpty(XFBadgeIconParameters.BrandDoc));
    }
}