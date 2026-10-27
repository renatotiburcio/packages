using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFBulletChartComponentTest : BunitContext
{
    public XFBulletChartComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Horizontal_RendersChart()
    {
        var cut = Render<XFBulletChart>(parameters =>
        {
            parameters.Add(p => p.Value, 75);
            parameters.Add(p => p.Target, 100);
            parameters.Add(p => p.Orientation, BulletOrientation.Horizontal);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        Assert.Contains("Value: 75", cut.Markup);
        Assert.Contains("Target: 100", cut.Markup);
    }

    [Fact]
    public void Render_Vertical_RendersChart()
    {
        var cut = Render<XFBulletChart>(parameters =>
        {
            parameters.Add(p => p.Value, 60);
            parameters.Add(p => p.Target, 80);
            parameters.Add(p => p.Orientation, BulletOrientation.Vertical);
        });

        var svg = cut.Find("svg");
        Assert.NotNull(svg);
        Assert.Contains("60", cut.Markup);
        Assert.Contains("80", cut.Markup);
    }

    [Fact]
    public void Render_With_Ranges_ShowsRangeBands()
    {
        var ranges = new[]
        {
            new BulletRange { From = 0, To = 40, Color = "#fecaca" },
            new BulletRange { From = 40, To = 70, Color = "#fef08a" },
            new BulletRange { From = 70, To = 100, Color = "#bbf7d0" }
        };

        var cut = Render<XFBulletChart>(parameters =>
        {
            parameters.Add(p => p.Value, 55);
            parameters.Add(p => p.Target, 80);
            parameters.Add(p => p.Ranges, ranges);
        });

        var rects = cut.FindAll("rect");
        Assert.True(rects.Count >= 3);
        Assert.Contains("#fecaca", cut.Markup);
        Assert.Contains("#fef08a", cut.Markup);
        Assert.Contains("#bbf7d0", cut.Markup);
    }

    [Fact]
    public void Render_No_Markers_HidesTargetLine()
    {
        var cut = Render<XFBulletChart>(parameters =>
        {
            parameters.Add(p => p.Value, 50);
            parameters.Add(p => p.Target, 75);
            parameters.Add(p => p.ShowMarkers, false);
        });

        var lines = cut.FindAll("line");
        Assert.Empty(lines);
    }

    [Fact]
    public void Render_With_Markers_ShowsTargetLine()
    {
        var cut = Render<XFBulletChart>(parameters =>
        {
            parameters.Add(p => p.Value, 50);
            parameters.Add(p => p.Target, 75);
            parameters.Add(p => p.ShowMarkers, true);
        });

        var lines = cut.FindAll("line");
        Assert.NotEmpty(lines);
    }
}
