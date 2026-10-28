using Bunit;
using Xunit;
using XForge.Components.Charts;

namespace XForge.DesignSystem.Tests.Components.Charts;

public class XFHeatmapChartTest : XForgeTestContext
{
    public XFHeatmapChartTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFHeatmapChart>();
        Assert.Contains("xf-heatmap", cut.Markup);
    }

    [Fact]
    public void Render_WithRowsAndColumns()
    {
        var rows = new List<string> { "Row1", "Row2" };
        var cols = new List<string> { "Col1", "Col2", "Col3" };

        var cut = Render<XFHeatmapChart>(p =>
        {
            p.Add(x => x.Rows, rows);
            p.Add(x => x.Columns, cols);
        });

        Assert.Contains("data-rows=\"2\"", cut.Markup);
        Assert.Contains("data-cols=\"3\"", cut.Markup);
    }

    [Fact]
    public void Render_WithValues_ShowsCells()
    {
        var rows = new List<string> { "A", "B" };
        var cols = new List<string> { "X", "Y" };
        var values = new double[,] { { 0.2, 0.8 }, { 0.5, 0.1 } };

        var cut = Render<XFHeatmapChart>(p =>
        {
            p.Add(x => x.Rows, rows);
            p.Add(x => x.Columns, cols);
            p.Add(x => x.Values, values);
        });

        Assert.Contains("xf-heatmap-cell", cut.Markup);
    }
}
