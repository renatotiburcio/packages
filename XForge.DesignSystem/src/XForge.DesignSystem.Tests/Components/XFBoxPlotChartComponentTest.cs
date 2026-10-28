using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFBoxPlotChartComponentTest : XForgeTestContext
{
    public XFBoxPlotChartComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_With_Data_RendersBoxPlot()
    {
        var data = new[]
        {
            new BoxPlotData { Label = "Group A", Min = 10, Q1 = 25, Median = 40, Q3 = 55, Max = 70, Mean = 42 },
            new BoxPlotData { Label = "Group B", Min = 15, Q1 = 30, Median = 45, Q3 = 60, Max = 80, Mean = 47 }
        };

        var cut = Render<XFBoxPlotChart>(parameters =>
        {
            parameters.Add(p => p.Data, data);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        var rects = cut.FindAll("rect");
        Assert.Equal(2, rects.Count);
    }

    [Fact]
    public void Render_With_Outliers_ShowsOutlierCircles()
    {
        var data = new[]
        {
            new BoxPlotData
            {
                Label = "Set 1",
                Min = 20,
                Q1 = 35,
                Median = 50,
                Q3 = 65,
                Max = 80,
                Mean = 52,
                Outliers = new[] { 5.0, 95.0 }
            }
        };

        var cut = Render<XFBoxPlotChart>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.ShowOutliers, true);
        });

        var circles = cut.FindAll("circle");
        Assert.Equal(2, circles.Count);
    }

    [Fact]
    public void Render_No_Outliers_HidesOutlierCircles()
    {
        var data = new[]
        {
            new BoxPlotData
            {
                Label = "Set",
                Min = 10,
                Q1 = 25,
                Median = 40,
                Q3 = 55,
                Max = 70,
                Mean = 42,
                Outliers = new[] { 5.0 }
            }
        };

        var cut = Render<XFBoxPlotChart>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.ShowOutliers, false);
        });

        var circles = cut.FindAll("circle");
        Assert.Empty(circles);
    }

    [Fact]
    public void Render_With_Mean_ShowsMeanMarker()
    {
        var data = new[]
        {
            new BoxPlotData { Label = "X", Min = 10, Q1 = 25, Median = 40, Q3 = 55, Max = 70, Mean = 42 }
        };

        var cut = Render<XFBoxPlotChart>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.ShowMean, true);
        });

        var lines = cut.FindAll("line");
        Assert.True(lines.Count >= 5);
        Assert.Contains("stroke-dasharray", cut.Markup);
    }

    [Fact]
    public void Render_Empty_Data_NoBoxes()
    {
        var cut = Render<XFBoxPlotChart>(parameters =>
        {
            parameters.Add(p => p.Data, Array.Empty<BoxPlotData>());
        });

        var rects = cut.FindAll("rect");
        Assert.Empty(rects);
    }

    [Fact]
    public void Render_ShowsLabels()
    {
        var data = new[]
        {
            new BoxPlotData { Label = "Dataset1", Min = 5, Q1 = 15, Median = 25, Q3 = 35, Max = 45, Mean = 27 },
            new BoxPlotData { Label = "Dataset2", Min = 10, Q1 = 20, Median = 30, Q3 = 40, Max = 50, Mean = 32 }
        };

        var cut = Render<XFBoxPlotChart>(parameters =>
        {
            parameters.Add(p => p.Data, data);
        });

        Assert.Contains("Dataset1", cut.Markup);
        Assert.Contains("Dataset2", cut.Markup);
    }
}
