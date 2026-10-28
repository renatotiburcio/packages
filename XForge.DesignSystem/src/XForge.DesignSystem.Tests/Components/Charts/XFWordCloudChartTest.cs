using Bunit;
using Xunit;
using XForge.Components.Charts;

namespace XForge.DesignSystem.Tests.Components.Charts;

public class XFWordCloudChartTest : XForgeTestContext
{
    public XFWordCloudChartTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFWordCloudChart>();
        Assert.Contains("xf-wordcloud", cut.Markup);
    }

    [Fact]
    public void Render_WithWords_ShowsWords()
    {
        var words = new List<XFWordCloudChart.WordCloudItem>
        {
            new("Blazor", 1.0, 0.1, 0.2, "#3b82f6"),
            new("C#", 0.8, 0.3, 0.5, "#ef4444")
        };

        var cut = Render<XFWordCloudChart>(p => p.Add(x => x.Words, words));
        Assert.Contains("Blazor", cut.Markup);
        Assert.Contains("C#", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomSize()
    {
        var cut = Render<XFWordCloudChart>(p =>
        {
            p.Add(x => x.Width, 800);
            p.Add(x => x.Height, 500);
        });

        Assert.Contains("800px", cut.Markup);
        Assert.Contains("500px", cut.Markup);
    }
}
