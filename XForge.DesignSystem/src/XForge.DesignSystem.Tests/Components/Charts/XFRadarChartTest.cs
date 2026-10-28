using Bunit;
using Xunit;
using XForge.Components.Charts;

namespace XForge.DesignSystem.Tests.Components.Charts;

public class XFRadarChartTest : XForgeTestContext
{
    public XFRadarChartTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFRadarChart>();
        Assert.Contains("xf-radar", cut.Markup);
    }

    [Fact]
    public void Render_WithAxesAndSeries()
    {
        var axes = new List<string> { "Speed", "Power", "Defense" };
        var series = new List<XFRadarChart.RadarSeries>
        {
            new("Player A", new List<double> { 0.8, 0.6, 0.9 }, "#3b82f6")
        };

        var cut = Render<XFRadarChart>(p =>
        {
            p.Add(x => x.Axes, axes);
            p.Add(x => x.Series, series);
        });

        Assert.Contains("data-axes=\"3\"", cut.Markup);
        Assert.Contains("data-series=\"1\"", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }
}
