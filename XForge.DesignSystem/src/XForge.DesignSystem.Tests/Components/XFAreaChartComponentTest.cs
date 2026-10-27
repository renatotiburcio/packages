using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFAreaChartComponentTest : BunitContext
{
    public XFAreaChartComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    private record ChartData(string Label, double Value);

    [Fact]
    public void Render_With_Data_RendersChart()
    {
        var data = new[]
        {
            new ChartData("Jan", 10),
            new ChartData("Feb", 25),
            new ChartData("Mar", 15),
            new ChartData("Apr", 30)
        };

        var cut = Render<XFAreaChart<ChartData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.XSelector, d => data.IndexOf(d));
            parameters.Add(p => p.YSelector, d => d.Value);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        Assert.Contains("fill-primary/20", cut.Markup);
        Assert.Contains("stroke-primary", cut.Markup);
    }

    [Fact]
    public void Render_Empty_Data_RendersEmpty()
    {
        var cut = Render<XFAreaChart<ChartData>>(parameters =>
        {
            parameters.Add(p => p.Data, Array.Empty<ChartData>());
            parameters.Add(p => p.XSelector, d => 0);
            parameters.Add(p => p.YSelector, d => d.Value);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        Assert.DoesNotContain("<polygon", cut.Markup);
        Assert.DoesNotContain("<path", cut.Markup);
    }

    [Fact]
    public void Render_With_Grid_ShowsGridLines()
    {
        var data = new[]
        {
            new ChartData("A", 5),
            new ChartData("B", 15),
            new ChartData("C", 10)
        };

        var cut = Render<XFAreaChart<ChartData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.XSelector, d => data.IndexOf(d));
            parameters.Add(p => p.YSelector, d => d.Value);
            parameters.Add(p => p.ShowGrid, true);
            parameters.Add(p => p.GridLines, 5);
        });

        Assert.Contains("stroke-surface-200", cut.Markup);
    }

    [Fact]
    public void Render_Custom_Colors_AppliesColors()
    {
        var data = new[]
        {
            new ChartData("X", 10),
            new ChartData("Y", 20),
            new ChartData("Z", 15)
        };

        var cut = Render<XFAreaChart<ChartData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.XSelector, d => data.IndexOf(d));
            parameters.Add(p => p.YSelector, d => d.Value);
            parameters.Add(p => p.FillColor, "fill-success/30");
            parameters.Add(p => p.StrokeColor, "stroke-success");
        });

        Assert.Contains("fill-success/30", cut.Markup);
        Assert.Contains("stroke-success", cut.Markup);
    }

    [Fact]
    public void Render_No_Dots_HidesDotMarkers()
    {
        var data = new[]
        {
            new ChartData("A", 10),
            new ChartData("B", 20),
            new ChartData("C", 15)
        };

        var cut = Render<XFAreaChart<ChartData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.XSelector, d => data.IndexOf(d));
            parameters.Add(p => p.YSelector, d => d.Value);
            parameters.Add(p => p.ShowDots, false);
        });

        Assert.DoesNotContain("<circle", cut.Markup);
    }
}
