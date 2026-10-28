using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFWaterfallChartComponentTest : XForgeTestContext
{
    public XFWaterfallChartComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    private record WaterfallStep(string Label, double Value, bool IsPositive);

    [Fact]
    public void Render_With_Data_RendersBars()
    {
        var data = new[]
        {
            new WaterfallStep("Start", 100, true),
            new WaterfallStep("Cost", 30, false),
            new WaterfallStep("Revenue", 50, true),
            new WaterfallStep("Tax", 20, false)
        };

        var cut = Render<XFWaterfallChart<WaterfallStep>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.IsPositiveSelector, d => d.IsPositive);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        var rects = cut.FindAll("rect");
        Assert.True(rects.Count >= 4);
    }

    [Fact]
    public void Render_With_Total_ShowsTotalBar()
    {
        var data = new[]
        {
            new WaterfallStep("A", 50, true),
            new WaterfallStep("B", 20, false)
        };

        var cut = Render<XFWaterfallChart<WaterfallStep>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.IsPositiveSelector, d => d.IsPositive);
            parameters.Add(p => p.ShowTotal, true);
        });

        Assert.Contains("Total", cut.Markup);
    }

    [Fact]
    public void Render_No_Total_HidesTotalBar()
    {
        var data = new[]
        {
            new WaterfallStep("X", 10, true),
            new WaterfallStep("Y", 5, false)
        };

        var cut = Render<XFWaterfallChart<WaterfallStep>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.IsPositiveSelector, d => d.IsPositive);
            parameters.Add(p => p.ShowTotal, false);
        });

        Assert.DoesNotContain("Total", cut.Markup);
    }

    [Fact]
    public void Render_Empty_Data_NoBars()
    {
        var cut = Render<XFWaterfallChart<WaterfallStep>>(parameters =>
        {
            parameters.Add(p => p.Data, Array.Empty<WaterfallStep>());
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.IsPositiveSelector, d => d.IsPositive);
        });

        var rects = cut.FindAll("rect");
        Assert.Empty(rects);
    }

    [Fact]
    public void Render_ShowValues_DisplaysValueLabels()
    {
        var data = new[]
        {
            new WaterfallStep("Up", 25, true),
            new WaterfallStep("Down", 10, false)
        };

        var cut = Render<XFWaterfallChart<WaterfallStep>>(parameters =>
        {
            parameters.Add(p => p.Data, data);
            parameters.Add(p => p.LabelSelector, d => d.Label);
            parameters.Add(p => p.ValueSelector, d => d.Value);
            parameters.Add(p => p.IsPositiveSelector, d => d.IsPositive);
            parameters.Add(p => p.ShowValues, true);
        });

        Assert.Contains("+25", cut.Markup);
        Assert.Contains("-10", cut.Markup);
    }
}
