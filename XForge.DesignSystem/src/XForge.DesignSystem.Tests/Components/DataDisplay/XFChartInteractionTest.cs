using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public sealed class XFChartInteractionTest : BunitContext
{
    public XFChartInteractionTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact] public void Chart_PointSelected_HandlesEvent()
    {
        XFChartPointSelectedEventArgs<ChartDataPoint>? args = null;
        var cut = Render<XFChart<ChartDataPoint>>(p => p
            .Add(x => x.Data, new List<ChartDataPoint> { new() { Label = "A", Value = 10 } })
            .Add(x => x.PointSelected, EventCallback.Factory.Create<XFChartPointSelectedEventArgs<ChartDataPoint>>(this, v => args = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_LegendChanged_HandlesEvent()
    {
        XFChartLegendChangedEventArgs? args = null;
        var series = new List<XFChartSeries<ChartDataPoint>> { new() { Name = "S1", Data = new List<ChartDataPoint> { new() } } };
        var cut = Render<XFChart<ChartDataPoint>>(p => p
            .Add(x => x.Series, series)
            .Add(x => x.LegendChanged, EventCallback.Factory.Create<XFChartLegendChangedEventArgs>(this, v => args = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_CustomEChartsOptions()
    {
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.EChartsOptions, new { theme = "dark" }));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_MultipleSeries()
    {
        var series = new List<XFChartSeries<ChartDataPoint>>
        {
            new() { Name = "Revenue", Data = new List<ChartDataPoint> { new() { Label = "Q1", Value = 100 } } },
            new() { Name = "Cost", Data = new List<ChartDataPoint> { new() { Label = "Q1", Value = 60 } } }
        };
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.Series, series));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_WithAnnotations()
    {
        var annotations = new List<XFChartAnnotation> { new() { Label = "Target", Value = 100, Tone = XFSemanticColor.Danger } };
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.Annotations, annotations));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_WithTrendlines()
    {
        var trendlines = new List<XFChartTrendline> { new() { Name = "Growth", StartIndex = 0, StartValue = 10, EndIndex = 2, EndValue = 30 } };
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.Trendlines, trendlines));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_ExportEnabled()
    {
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.ExportEnabled, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_DataZoom()
    {
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.DataZoomMode, XFChartDataZoomMode.Slider));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_ValueFormatter()
    {
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.ValueFormatter, v => $"R${v:F2}"));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_CustomPalette()
    {
        var palette = new List<string> { "#ff0000", "#00ff00", "#0000ff" };
        var cut = Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.Palette, palette));
        Assert.NotNull(cut);
    }

    [Fact] public void Chart_ShowDataTable() => Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.ShowDataTable, true));
    [Fact] public void Chart_ShowSummary() => Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.ShowSummary, true));
    [Fact] public void Chart_ShowValueLabels() => Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.ShowValueLabels, true));
    [Fact] public void Chart_ShowLegend() => Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.ShowLegend, true));
    [Fact] public void Chart_Culture() => Render<XFChart<ChartDataPoint>>(p => p.Add(x => x.Culture, new System.Globalization.CultureInfo("pt-BR")));
}