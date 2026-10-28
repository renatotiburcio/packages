using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFStackedBarChartComponentTest : XForgeTestContext
{
    public XFStackedBarChartComponentTest() { JSInterop.Mode = JSRuntimeMode.Loose; Services.AddSingleton<IXFLocalizer, TestLocalizer>(); }

    [Fact] public void Render_With_Data_RendersChart() { var cut = Render<XFStackedBarChart>(p => { p.Add(p => p.Labels, new[] {"Q1", "Q2", "Q3"}); p.Add(p => p.Series, new[] { new XFStackedSeries { Name = "A", Values = new[] {10.0, 20.0, 30.0} } }); }); Assert.Contains("xf-stacked-bar-chart", cut.Markup); }
    [Fact] public void Render_Empty_RendersEmpty() { var cut = Render<XFStackedBarChart>(); Assert.DoesNotContain("rect", cut.Markup); }
    [Fact] public void Render_With_MultipleSeries_RendersAll() { var cut = Render<XFStackedBarChart>(p => { p.Add(p => p.Labels, new[] {"A", "B"}); p.Add(p => p.Series, new[] { new XFStackedSeries { Name = "S1", Values = new[] {10.0, 20.0} }, new XFStackedSeries { Name = "S2", Values = new[] {5.0, 10.0} } }); }); Assert.Contains("S1", cut.Markup); Assert.Contains("S2", cut.Markup); }
    [Fact] public void Render_With_Legend_ShowsLegend() { var cut = Render<XFStackedBarChart>(p => { p.Add(p => p.ShowLegend, true); p.Add(p => p.Labels, new[] {"A"}); p.Add(p => p.Series, new[] { new XFStackedSeries { Name = "Test", Values = new[] {10.0} } }); }); Assert.Contains("Test", cut.Markup); }
    [Fact] public void Render_Custom_Colors_AppliesColors() { var cut = Render<XFStackedBarChart>(p => { p.Add(p => p.Labels, new[] {"A"}); p.Add(p => p.Series, new[] { new XFStackedSeries { Name = "S", Values = new[] {10.0}, Color = "#ff0000" } }); }); Assert.Contains("#ff0000", cut.Markup); }
}
