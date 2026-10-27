using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFDonutChartComponentTest : BunitContext
{
    public XFDonutChartComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    private record SliceData(string Label, double Value);

    [Fact]
    public void Render_With_Data_RendersDonut()
    {
        var data = new[]
        {
            new SliceData("Product A", 40),
            new SliceData("Product B", 30),
            new SliceData("Product C", 20),
            new SliceData("Product D", 10)
        };

        var cut = Render<XFDonutChart<SliceData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        Assert.Contains("<path", cut.Markup);
    }

    [Fact]
    public void Render_ShowsCenterLabel()
    {
        var data = new[]
        {
            new SliceData("A", 50),
            new SliceData("B", 50)
        };

        var cut = Render<XFDonutChart<SliceData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
        });

        Assert.Contains("100", cut.Markup);
    }

    [Fact]
    public void Render_With_Legend_ShowsLegendItems()
    {
        var data = new[]
        {
            new SliceData("Red", 30),
            new SliceData("Blue", 70)
        };

        var cut = Render<XFDonutChart<SliceData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.ShowLegend, true);
        });

        Assert.Contains("Red", cut.Markup);
        Assert.Contains("Blue", cut.Markup);
    }

    [Fact]
    public void Render_No_Legend_HidesLegend()
    {
        var data = new[]
        {
            new SliceData("X", 10),
            new SliceData("Y", 20)
        };

        var cut = Render<XFDonutChart<SliceData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.ShowLegend, false);
        });

        Assert.DoesNotContain("rounded-full", cut.Markup);
    }

    [Fact]
    public void Render_Empty_Data_NoArcs()
    {
        var cut = Render<XFDonutChart<SliceData>>(parameters =>
        {
            parameters.Add(p => p.Data, Array.Empty<SliceData>());
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
        });

        Assert.DoesNotContain("<path", cut.Markup);
    }

    [Fact]
    public void Render_Custom_Colors_AppliesColors()
    {
        var data = new[]
        {
            new SliceData("A", 60),
            new SliceData("B", 40)
        };

        var cut = Render<XFDonutChart<SliceData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.Colors, new[] { "#ff0000", "#00ff00" });
        });

        Assert.Contains("#ff0000", cut.Markup);
        Assert.Contains("#00ff00", cut.Markup);
    }
}
