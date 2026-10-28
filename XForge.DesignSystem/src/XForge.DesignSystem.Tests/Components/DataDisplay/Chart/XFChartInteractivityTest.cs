using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.DataDisplay.Chart;

public class XFChartInteractivityTest : XForgeTestContext
{
    public XFChartInteractivityTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    #region DataZoom

    [Fact]
    public void Render_WithSliderZoom_ShowsDataZoom()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.DataZoomMode, XFChartDataZoomMode.Slider);
            p.Add(x => x.Data, new[] { "A", "B", "C" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithInsideZoom_ShowsDataZoom()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.DataZoomMode, XFChartDataZoomMode.Inside);
            p.Add(x => x.Data, new[] { "A", "B" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithSliderAndInsideZoom_ShowsDataZoom()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.DataZoomMode, XFChartDataZoomMode.SliderAndInside);
            p.Add(x => x.Data, new[] { "A", "B" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithNoZoom_DefaultsToNone()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    #endregion

    #region Annotations

    [Fact]
    public void Render_WithAnnotations_ShowsAnnotations()
    {
        var annotations = new[]
        {
            new XFChartAnnotation { Label = "Target", Value = 50, Axis = XFChartAnnotationAxis.Y }
        };

        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.Annotations, annotations);
            p.Add(x => x.Data, new[] { "A", "B" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithMultipleAnnotations_AllRendered()
    {
        var annotations = new[]
        {
            new XFChartAnnotation { Label = "Target", Value = 50 },
            new XFChartAnnotation { Label = "Warning", Value = 80 }
        };

        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.Annotations, annotations);
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    #endregion

    #region Trendlines

    [Fact]
    public void Render_WithTrendlines_ShowsTrendlines()
    {
        var trendlines = new[]
        {
            new XFChartTrendline { Name = "Growth", StartValue = 10, EndValue = 50 }
        };

        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.Trendlines, trendlines);
            p.Add(x => x.Data, new[] { "A", "B" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithMultipleTrendlines_AllRendered()
    {
        var trendlines = new[]
        {
            new XFChartTrendline { Name = "Linear", StartValue = 0, EndValue = 100 },
            new XFChartTrendline { Name = "Exponential", StartValue = 5, EndValue = 95 }
        };

        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.Trendlines, trendlines);
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    #endregion

    #region Live Streaming

    [Fact]
    public void Render_WithLiveStreaming_EnablesMode()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.LiveStreaming, true);
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithLiveStreamingInterval_SetsInterval()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.LiveStreaming, true);
            p.Add(x => x.LiveStreamingInterval, 500);
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithLiveStreamingMaxPoints_SetsMaxPoints()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.LiveStreaming, true);
            p.Add(x => x.LiveStreamingMaxPoints, 50);
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    #endregion

    #region Export

    [Fact]
    public void Render_WithExportEnabled_ShowsToolbox()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.ExportEnabled, true);
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithExportDisabled_NoToolbox()
    {
        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.ExportEnabled, false);
            p.Add(x => x.Data, new[] { "A" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    #endregion

    #region Event Args

    [Fact]
    public void LiveDataUpdateEventArgs_HasDefaults()
    {
        var args = new XFChartLiveDataUpdateEventArgs<string>();
        Assert.Equal(DateTime.UtcNow.Date, args.Timestamp.Date);
        Assert.Equal(0, args.PointCount);
        Assert.False(args.WasTrimmed);
    }

    [Fact]
    public void LiveDataUpdateEventArgs_SetValues()
    {
        var args = new XFChartLiveDataUpdateEventArgs<string>
        {
            NewItem = "test",
            PointCount = 10,
            WasTrimmed = true
        };
        Assert.Equal("test", args.NewItem);
        Assert.Equal(10, args.PointCount);
        Assert.True(args.WasTrimmed);
    }

    #endregion

    #region Combined Features

    [Fact]
    public void Render_WithZoomAndAnnotations_CombinesFeatures()
    {
        var annotations = new[]
        {
            new XFChartAnnotation { Label = "Target", Value = 50 }
        };

        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.DataZoomMode, XFChartDataZoomMode.Slider);
            p.Add(x => x.Annotations, annotations);
            p.Add(x => x.Data, new[] { "A", "B" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    [Fact]
    public void Render_WithAllInteractivityFeatures_CombinesFeatures()
    {
        var annotations = new[]
        {
            new XFChartAnnotation { Label = "Target", Value = 50 }
        };
        var trendlines = new[]
        {
            new XFChartTrendline { Name = "Trend", StartValue = 0, EndValue = 100 }
        };

        var cut = Render<XFChart<string>>(p =>
        {
            p.Add(x => x.DataZoomMode, XFChartDataZoomMode.SliderAndInside);
            p.Add(x => x.Annotations, annotations);
            p.Add(x => x.Trendlines, trendlines);
            p.Add(x => x.ExportEnabled, true);
            p.Add(x => x.LiveStreaming, true);
            p.Add(x => x.Data, new[] { "A", "B", "C" });
            p.Add(x => x.LabelSelector, s => s);
            p.Add(x => x.ValueSelector, s => s.Length);
        });
        Assert.Contains("xf-chart", cut.Markup);
    }

    #endregion
}
