using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFBubbleChartComponentTest : XForgeTestContext
{
    public XFBubbleChartComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    private record BubbleData(string Label, double X, double Y, double Size);

    [Fact]
    public void Render_With_Data_RendersBubbles()
    {
        var data = new[]
        {
            new BubbleData("Alpha", 10, 20, 15),
            new BubbleData("Beta", 30, 40, 25),
            new BubbleData("Gamma", 50, 10, 10),
            new BubbleData("Delta", 20, 35, 20)
        };

        var cut = Render<XFBubbleChart<BubbleData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.XSelector, d => d.X);
            parameters.Add(p => p.YSelector, d => d.Y);
            parameters.Add(p => p.SizeSelector, d => d.Size);
            parameters.Add(p => p.LabelSelector, d => d.Label);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        var circles = cut.FindAll("circle");
        Assert.Equal(4, circles.Count);
    }

    [Fact]
    public void Render_Empty_Data_NoBubbles()
    {
        var cut = Render<XFBubbleChart<BubbleData>>(parameters =>
        {
            parameters.Add(p => p.Data, Array.Empty<BubbleData>());
            parameters.Add(p => p.XSelector, d => d.X);
            parameters.Add(p => p.YSelector, d => d.Y);
            parameters.Add(p => p.SizeSelector, d => d.Size);
        });

        var circles = cut.FindAll("circle");
        Assert.Empty(circles);
    }

    [Fact]
    public void Render_With_Labels_ShowsLabelText()
    {
        var data = new[]
        {
            new BubbleData("Item1", 10, 20, 10),
            new BubbleData("Item2", 30, 40, 20)
        };

        var cut = Render<XFBubbleChart<BubbleData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.XSelector, d => d.X);
            parameters.Add(p => p.YSelector, d => d.Y);
            parameters.Add(p => p.SizeSelector, d => d.Size);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ShowLabels, true);
        });

        Assert.Contains("Item1", cut.Markup);
        Assert.Contains("Item2", cut.Markup);
    }

    [Fact]
    public void Render_Custom_Colors_AppliesFill()
    {
        var data = new[]
        {
            new BubbleData("A", 10, 20, 10),
            new BubbleData("B", 20, 30, 15)
        };

        var cut = Render<XFBubbleChart<BubbleData>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.XSelector, d => d.X);
            parameters.Add(p => p.YSelector, d => d.Y);
            parameters.Add(p => p.SizeSelector, d => d.Size);
            parameters.Add(p => p.FillColor, "fill-warning/40");
            parameters.Add(p => p.StrokeColor, "stroke-warning");
        });

        Assert.Contains("fill-warning/40", cut.Markup);
        Assert.Contains("stroke-warning", cut.Markup);
    }
}
