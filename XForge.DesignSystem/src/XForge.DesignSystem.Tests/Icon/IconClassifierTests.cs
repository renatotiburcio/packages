using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconClassifierTests
{
    [Fact]
    public void Classify_EmailIcon_ReturnsCommunication()
    {
        var c = new XFIconClassifier();
        var info = new XFIconInfo("envelope", XFIconVariant.Regular, XFIconSize.Size20, new[] { "mail", "email" }, "communication", "<svg/>");
        Assert.Equal(XFIconCategory.Communication, c.Classify(info));
    }

    [Fact]
    public void Classify_HouseIcon_ReturnsBuildings()
    {
        var c = new XFIconClassifier();
        var info = new XFIconInfo("house", XFIconVariant.Regular, XFIconSize.Size20, new[] { "home" }, "navigation", "<svg/>");
        Assert.Equal(XFIconCategory.Buildings, c.Classify(info));
    }

    [Fact]
    public void Classify_UnknownIcon_ReturnsOther()
    {
        var c = new XFIconClassifier();
        var info = new XFIconInfo("xyz", XFIconVariant.Regular, XFIconSize.Size20, new[] { "xyz" }, "misc", "<svg/>");
        Assert.Equal(XFIconCategory.Other, c.Classify(info));
    }
}