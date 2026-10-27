using Bunit;
using Xunit;
using XForge.Components.Charts;

namespace XForge.DesignSystem.Tests.Components.Charts;

public class XFCandlestickChartTest : Bunit.BunitContext
{
    public XFCandlestickChartTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFCandlestickChart>();
        Assert.Contains("xf-candlestick", cut.Markup);
    }

    [Fact]
    public void Render_WithCandles_ShowsSvg()
    {
        var candles = new List<XFCandlestickChart.Candle>
        {
            new(DateTime.Now, 100, 110, 95, 105),
            new(DateTime.Now.AddDays(1), 105, 115, 100, 110)
        };

        var cut = Render<XFCandlestickChart>(p => p.Add(x => x.Candles, candles));
        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("data-count=\"2\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomSize()
    {
        var cut = Render<XFCandlestickChart>(p =>
        {
            p.Add(x => x.Width, 600);
            p.Add(x => x.Height, 300);
        });

        Assert.Contains("600", cut.Markup);
        Assert.Contains("300", cut.Markup);
    }

    [Fact]
    public void Render_EmptyCandles_NoSvgContent()
    {
        var cut = Render<XFCandlestickChart>(p => p.Add(x => x.Candles, new List<XFCandlestickChart.Candle>()));
        Assert.Contains("data-count=\"0\"", cut.Markup);
    }
}
