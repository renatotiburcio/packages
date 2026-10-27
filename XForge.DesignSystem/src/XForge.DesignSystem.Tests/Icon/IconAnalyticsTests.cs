using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconAnalyticsTests
{
    [Fact]
    public void Track_Increases_Count()
    {
        var a = new XFIconAnalytics();
        a.Track("home"); a.Track("home"); a.Track("add");
        Assert.Equal(3, a.Count);
    }

    [Fact]
    public void RingBuffer_Bounds_Memory()
    {
        var a = new XFIconAnalytics(maxEvents: 5);
        for (int i = 0; i < 20; i++) a.Track($"icon{i}");
        Assert.Equal(5, a.Count);
    }

    [Fact]
    public void GetTop_Returns_MostFrequent()
    {
        var a = new XFIconAnalytics();
        for (int i = 0; i < 3; i++) a.Track("home");
        for (int i = 0; i < 2; i++) a.Track("add");
        a.Track("save");
        var top = a.GetTop(3);
        Assert.Equal("home", top.First().Key);
        Assert.Equal(3, top["home"]);
    }

    [Fact]
    public void ExportJson_Returns_NonEmpty()
    {
        var a = new XFIconAnalytics();
        a.Track("home");
        Assert.Contains("home", a.ExportJson());
    }
}