#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Gauge;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Components.Layout;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFLinearGaugeComponentTest : Bunit.BunitContext
{
    public XFLinearGaugeComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFLinearGauge>();
        Assert.Contains("xf-linear-gauge", cut.Markup);
    }

    [Fact]
    public void Render_WithValueAndRange()
    {
        var cut = Render<XFLinearGauge>(parameters =>
        {
            parameters.Add(p => p.Value, 75);
            parameters.Add(p => p.Min, 0);
            parameters.Add(p => p.Max, 100);
        });
        Assert.Contains("xf-linear-gauge", cut.Markup);
        Assert.Contains("aria-valuenow", cut.Markup);
    }

    [Fact]
    public void Render_VerticalOrientation()
    {
        var cut = Render<XFLinearGauge>(parameters =>
        {
            parameters.Add(p => p.Orientation, XFOrientation.Vertical);
        });
        Assert.Contains("h-48", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomColor()
    {
        var cut = Render<XFLinearGauge>(parameters =>
        {
            parameters.Add(p => p.Value, 50);
            parameters.Add(p => p.Color, "fill-success");
        });
        Assert.Contains("fill-success", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel()
    {
        var cut = Render<XFLinearGauge>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "CPU usage");
        });
        Assert.Contains("CPU usage", cut.Markup);
    }
}