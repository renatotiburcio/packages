using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chart;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFChartComponentTest : BunitContext
{
    public XFChartComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_LineChart_RendersEChartsHostAndPoints()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.Title, "Revenue");
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
        });

        Assert.NotNull(cut.Find("div[data-xf-chart='line']"));
        Assert.Equal(3, cut.FindAll("[data-xf-chart-point]").Count);
        Assert.Contains("Revenue", cut.Markup);
    }

    [Fact]
    public void Render_BarChart_RendersBars()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.Type, XFChartType.Bar);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
        });

        Assert.NotNull(cut.Find("div[data-xf-chart='bar']"));
        Assert.Equal(3, cut.FindAll("[data-xf-chart-point]").Count);
    }

    [Fact]
    public void Render_PieChart_RendersSlicesAndLegend()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.Type, XFChartType.Pie);
            parameters.Add(p => p.LabelProperty, nameof(Metric.Label));
            parameters.Add(p => p.ValueProperty, nameof(Metric.Value));
        });

        Assert.NotNull(cut.Find("div[data-xf-chart='pie']"));
        Assert.Equal(3, cut.FindAll("[data-xf-chart-point]").Count);
        Assert.Contains("A", cut.Markup);
    }

    [Fact]
    public void Render_MultipleSeriesLine_RendersSeriesAndPoints()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Series, Series);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
        });

        Assert.NotNull(cut.Find("div[data-xf-chart='line']"));
        Assert.Equal(4, cut.FindAll("[data-xf-chart-point]").Count);
        Assert.Contains("Atual", cut.Markup);
        Assert.Contains("Meta", cut.Markup);
    }

    [Fact]
    public void Render_MultipleSeriesBar_RendersGroupedBars()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Type, XFChartType.Bar);
            parameters.Add(p => p.Series, Series);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
        });

        Assert.NotNull(cut.Find("div[data-xf-chart='bar']"));
        Assert.Equal(4, cut.FindAll("[data-xf-chart-point]").Count);
    }

    [Fact]
    public void Render_UnnamedSeries_UsesPortugueseGeneratedSeriesNames()
    {
        var series = new[]
        {
            new XFChartSeries<Metric>
            {
                Data = [new("Jan", 10)]
            }
        };

        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Series, series);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
        });

        Assert.Contains("chart.series_name", cut.Markup);
        Assert.DoesNotContain(">Series<", cut.Markup);
    }

    [Fact]
    public void Render_WithEChartsOptions_MarksHostAsCustom()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
            parameters.Add(p => p.EChartsOptions, new
            {
                toolbox = new
                {
                    show = true
                }
            });
        });

        Assert.NotNull(cut.Find("div[data-xf-chart-options='custom']"));
    }

    [Fact]
    public void Render_WithEnterpriseApis_RendersAccessibleAnnotationsAndTrendlines()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
            parameters.Add(p => p.Annotations, new[]
            {
                new XFChartAnnotation
                {
                    Label = "Meta minima",
                    Value = 18,
                    Tone = XFSemanticColor.Warning
                }
            });
            parameters.Add(p => p.Trendlines, new[]
            {
                new XFChartTrendline
                {
                    Name = "Tendencia",
                    StartIndex = 0,
                    StartValue = 10,
                    EndIndex = 2,
                    EndValue = 25,
                    Tone = XFSemanticColor.Success
                }
            });
            parameters.Add(p => p.DataZoomMode, XFChartDataZoomMode.SliderAndInside);
            parameters.Add(p => p.ExportEnabled, true);
            parameters.Add(p => p.ExportFileName, "receita");
            parameters.Add(p => p.ExportTitle, "Exportar grafico");
        });

        Assert.Contains("data-xf-chart-annotation=\"Meta minima\"", cut.Markup);
        Assert.Contains("Meta minima: 18", cut.Markup);
        Assert.Contains("data-xf-chart-trendline=\"Tendencia\"", cut.Markup);
        Assert.Contains("Tendencia: 10 - 25", cut.Markup);
    }

    [Fact]
    public async Task LegendChangedFromChart_InvokesCallback()
    {
        XFChartLegendChangedEventArgs? changed = null;
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Series, Series);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
            parameters.Add(p => p.LegendChanged, args => changed = args);
        });

        await cut.Instance.LegendChangedFromChart("Meta", false);

        Assert.NotNull(changed);
        Assert.Equal("Meta", changed!.Name);
        Assert.False(changed.Selected);
    }

    [Fact]
    public void Click_Point_RaisesPointSelected()
    {
        XFChartPointSelectedEventArgs<Metric>? selected = null;
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
            parameters.Add(p => p.PointSelected, args => selected = args);
        });

        cut.Find("[data-xf-chart-point='B']").Click();

        Assert.NotNull(selected);
        Assert.Equal("B", selected!.Label);
        Assert.Equal(20, selected.Value);
    }

    [Fact]
    public void Render_EmptyData_RendersEmptyState()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Array.Empty<Metric>());
        });

        Assert.Contains("chart.empty", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredCopy()
    {
        var empty = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Array.Empty<Metric>());
            parameters.Add(p => p.EmptyText, "No data available.");
        });

        Assert.Contains("chart.empty", empty.Markup);

        var chart = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
            parameters.Add(p => p.AccessibleDescriptionFormat, "{0} data points.");
            parameters.Add(p => p.LabelHeaderText, "Category");
            parameters.Add(p => p.ValueHeaderText, "Amount");
            parameters.Add(p => p.PercentHeaderText, "Share");
        });

        Assert.Contains("chart.description_format", chart.Markup);
        Assert.Contains("chart.label_header", chart.Markup);
        Assert.Contains("chart.value_header", chart.Markup);
        Assert.Contains("chart.percent_header", chart.Markup);
    }

    [Fact]
    public void Render_DefaultLocalization_UsesPortugueseAccessibleCopy()
    {
        var cut = Render<XFChart<Metric>>(parameters =>
        {
            parameters.Add(p => p.Data, Metrics);
            parameters.Add(p => p.LabelSelector, item => item.Label);
            parameters.Add(p => p.ValueSelector, item => item.Value);
        });

        Assert.Contains("chart.aria_label", cut.Markup);
        Assert.Contains("chart.description_format", cut.Markup);
        Assert.Contains("chart.label_header", cut.Markup);
        Assert.Contains("chart.value_header", cut.Markup);
        Assert.Contains("chart.percent_header", cut.Markup);
    }

    private static readonly IReadOnlyList<Metric> Metrics =
    [
        new("A", 10),
        new("B", 20),
        new("C", 15)
    ];

    private static readonly IReadOnlyList<XFChartSeries<Metric>> Series =
    [
        new()
        {
            Name = "Atual",
            Data =
            [
                new("Jan", 10),
                new("Fev", 20)
            ]
        },
        new()
        {
            Name = "Meta",
            Data =
            [
                new("Jan", 12),
                new("Fev", 22)
            ],
            Tone = XFSemanticColor.Success
        }
    ];

    private sealed record Metric(string Label, double Value);
}
