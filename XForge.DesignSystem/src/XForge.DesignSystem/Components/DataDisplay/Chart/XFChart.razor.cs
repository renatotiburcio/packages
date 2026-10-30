using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    public partial class XFChart<TItem> : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-chart";

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public IEnumerable<TItem>? Data { get; set; }
        [Parameter] public IReadOnlyList<XFChartSeries<TItem>> Series { get; set; } = Array.Empty<XFChartSeries<TItem>>();
        [Parameter] public Func<TItem, string>? LabelSelector { get; set; }
        [Parameter] public Func<TItem, double>? ValueSelector { get; set; }
        [Parameter] public string LabelProperty { get; set; } = "Label";
        [Parameter] public string ValueProperty { get; set; } = "Value";
        [Parameter] public XFChartType Type { get; set; } = XFChartType.Line;
        [Parameter] public XFChartStackMode StackMode { get; set; }
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Primary;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool ShowLegend { get; set; } = true;
        [Parameter] public bool ShowSummary { get; set; } = true;
        [Parameter] public bool ShowValueLabels { get; set; }
        [Parameter] public bool ShowDataTable { get; set; } = true;
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string AriaLabel { get; set; } = default!;
        [Parameter] public string EmptyText { get; set; } = default!;
        [Parameter] public string AccessibleDescriptionFormat { get; set; } = default!;
        [Parameter] public string SeriesHeaderText { get; set; } = default!;
        [Parameter] public string LabelHeaderText { get; set; } = default!;
        [Parameter] public string ValueHeaderText { get; set; } = default!;
        [Parameter] public string PercentHeaderText { get; set; } = default!;
        [Parameter] public string SeriesNameFormat { get; set; } = default!;
        [Parameter] public string ItemLabelFormat { get; set; } = default!;
        [Parameter] public CultureInfo? Culture { get; set; }
        [Parameter] public Func<double, string>? ValueFormatter { get; set; }
        [Parameter] public IReadOnlyList<string>? Palette { get; set; }
        [Parameter] public IReadOnlyList<XFChartAnnotation> Annotations { get; set; } = Array.Empty<XFChartAnnotation>();
        [Parameter] public IReadOnlyList<XFChartTrendline> Trendlines { get; set; } = Array.Empty<XFChartTrendline>();
        [Parameter] public XFChartDataZoomMode DataZoomMode { get; set; }
        [Parameter] public bool ExportEnabled { get; set; }
        [Parameter] public string ExportFileName { get; set; } = default!;
        [Parameter] public string ExportTitle { get; set; } = default!;
        [Parameter] public object? EChartsOptions { get; set; }
        [Parameter] public RenderFragment? EmptyContent { get; set; }
        [Parameter] public EventCallback<XFChartPointSelectedEventArgs<TItem>> PointSelected { get; set; }
        [Parameter] public EventCallback<XFChartLegendChangedEventArgs> LegendChanged { get; set; }

        /// <summary>Enable live streaming mode for real-time data updates.</summary>
        [Parameter] public bool LiveStreaming { get; set; }

        /// <summary>Update interval in milliseconds for live streaming.</summary>
        [Parameter] public int LiveStreamingInterval { get; set; } = 1000;

        /// <summary>Maximum number of data points to keep in live streaming mode.</summary>
        [Parameter] public int LiveStreamingMaxPoints { get; set; } = 100;

        /// <summary>Callback when live streaming data updates.</summary>
        [Parameter] public EventCallback<XFChartLiveDataUpdateEventArgs<TItem>> OnDataUpdate { get; set; }

        private const double PlotLeft = 52;
        private const double PlotTop = 24;
        private const double PlotWidth = 560;
        private const double PlotHeight = 204;
        private static readonly IReadOnlyList<string> DefaultPalette =
        [
            "#DD6C98", "#DB9F66", "#E49597", "#E7D7A7", "#DC6967", "#A9DDEA",
            "#CE6DDA", "#A2E69D", "#DD94D9", "#689FDC", "#6DDB6E", "#97DD6C",
            "#67DCAA", "#96A3E5", "#996CDD", "#6C6CDB", "#66DADB", "#CDD96B"
        ];

        private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;
        private ElementReference ChartElement { get; set; }
        private IJSObjectReference? chartModule;
        private DotNetObjectReference<XFChart<TItem>>? dotNetReference;
        private IReadOnlyList<PointInfo> jsPointIndex = Array.Empty<PointInfo>();
        private string TitleId => $"{Id}-title";
        private string DescriptionId => $"{Id}-description";
        private string AccessibleTitle => string.IsNullOrWhiteSpace(Title) ? AriaLabel : Title!;
        private string AccessibleDescription => Description ?? string.Format(EffectiveCulture, AccessibleDescriptionFormat, VisiblePointCount);
        private string CustomOptionsState => EChartsOptions is null ? "base" : "custom";
        private bool HasMultipleSeries => Type != XFChartType.Pie && Series.Count > 0;
        private int VisiblePointCount => HasMultipleSeries ? SeriesPoints.Count : Points.Count;
        private double Total => ActivePoints.Sum(point => Math.Max(0, point.Value));
        private double MaxValue => Math.Max(1, ActivePoints.Max(point => point.Value));
        private IReadOnlyList<PointInfo> ActivePoints => HasMultipleSeries ? SeriesPoints : Points;
        private IReadOnlyList<string> EffectivePalette => Palette is { Count: > 0 } ? Palette : DefaultPalette;

        private IReadOnlyList<PointInfo> Points
        {
            get
            {
                var values = (Data ?? Array.Empty<TItem>())
                    .Select((item, index) => ToPoint(item, index))
                    .Where(point => point is not null)
                    .Cast<PointInfo>()
                    .ToArray();

                var total = values.Sum(point => Math.Max(0, point.Value));
                return values.Select(point => point with { Percent = total <= 0 ? 0 : Math.Max(0, point.Value) / total }).ToArray();
            }
        }

        private IReadOnlyList<SeriesInfo> ChartSeries
        {
            get
            {
                if (!HasMultipleSeries)
                {
                    return Array.Empty<SeriesInfo>();
                }

                var rawSeries = Series.Select((series, seriesIndex) =>
                {
                    var values = (series.Data ?? Array.Empty<TItem>())
                        .Select((item, pointIndex) => ToPoint(item, pointIndex, series, seriesIndex))
                        .Where(point => point is not null)
                        .Cast<PointInfo>()
                        .ToArray();

                    return new SeriesInfo(
                        string.IsNullOrWhiteSpace(series.Name) ? string.Format(EffectiveCulture, SeriesNameFormat, seriesIndex + 1) : series.Name,
                        seriesIndex,
                        values,
                        series.Tone);
                }).Where(series => series.Points.Count > 0).ToArray();

                var total = rawSeries.SelectMany(series => series.Points).Sum(point => Math.Max(0, point.Value));
                return rawSeries.Select(series =>
                {
                    var points = series.Points
                        .Select(point => point with { Percent = total <= 0 ? 0 : Math.Max(0, point.Value) / total })
                        .ToArray();

                    return series with { Points = points };
                }).ToArray();
            }
        }

        private IReadOnlyList<PointInfo> SeriesPoints => ChartSeries.SelectMany(series => series.Points).ToArray();

        private IReadOnlyList<PlotPoint> PlotPoints
        {
            get
            {
                if (Points.Count == 0)
                {
                    return Array.Empty<PlotPoint>();
                }

                var step = Points.Count == 1 ? 0 : PlotWidth / (Points.Count - 1);
                return Points.Select((point, index) =>
                {
                    var x = Points.Count == 1 ? PlotLeft + PlotWidth / 2 : PlotLeft + index * step;
                    var y = PlotTop + PlotHeight - (Math.Max(0, point.Value) / MaxValue * PlotHeight);
                    return new PlotPoint(point, x, y);
                }).ToArray();
            }
        }

        private IReadOnlyList<SeriesPlotInfo> SeriesPlotPoints
        {
            get
            {
                return ChartSeries.Select(series =>
                {
                    var count = series.Points.Count;
                    var step = count == 1 ? 0 : PlotWidth / (count - 1);
                    var points = series.Points.Select((point, index) =>
                    {
                        var x = count == 1 ? PlotLeft + PlotWidth / 2 : PlotLeft + index * step;
                        var y = PlotTop + PlotHeight - (Math.Max(0, point.Value) / MaxValue * PlotHeight);
                        return new PlotPoint(point, x, y);
                    }).ToArray();

                    return new SeriesPlotInfo(series.Name, series.Index, points, SeriesColorClass(series));
                }).ToArray();
            }
        }

        private IReadOnlyList<BarInfo> Bars
        {
            get
            {
                if (Points.Count == 0)
                {
                    return Array.Empty<BarInfo>();
                }

                var slot = PlotWidth / Points.Count;
                var width = Math.Max(18, Math.Min(52, slot * 0.58));
                return Points.Select((point, index) =>
                {
                    var height = Math.Max(3, Math.Max(0, point.Value) / MaxValue * PlotHeight);
                    var x = PlotLeft + index * slot + (slot - width) / 2;
                    var y = PlotTop + PlotHeight - height;
                    return new BarInfo(point, x, y, width, height, x + width / 2, ColorClass(point.Index));
                }).ToArray();
            }
        }

        private IReadOnlyList<BarInfo> SeriesBars
        {
            get
            {
                if (ChartSeries.Count == 0)
                {
                    return Array.Empty<BarInfo>();
                }

                var categoryCount = ChartSeries.Max(series => series.Points.Count);
                if (categoryCount == 0)
                {
                    return Array.Empty<BarInfo>();
                }

                var slot = PlotWidth / categoryCount;
                var groupWidth = slot * 0.72;
                var barWidth = Math.Max(8, Math.Min(36, groupWidth / ChartSeries.Count));
                var bars = new List<BarInfo>();

                foreach (var series in ChartSeries)
                {
                    foreach (var point in series.Points)
                    {
                        var height = Math.Max(3, Math.Max(0, point.Value) / MaxValue * PlotHeight);
                        var groupStart = PlotLeft + point.Index * slot + (slot - groupWidth) / 2;
                        var x = groupStart + series.Index * barWidth;
                        var y = PlotTop + PlotHeight - height;
                        bars.Add(new BarInfo(point, x, y, barWidth * 0.82, height, x + barWidth * 0.41, SeriesColorClass(series)));
                    }
                }

                return bars;
            }
        }

        private IReadOnlyList<ValueTick> ValueTicks
        {
            get
            {
                return Enumerable.Range(0, 4).Select(index =>
                {
                    var value = MaxValue / 3 * index;
                    var y = PlotTop + PlotHeight - (value / MaxValue * PlotHeight);
                    return new ValueTick(value, y);
                }).Reverse().ToArray();
            }
        }

        private IReadOnlyList<PieSlice> PieSlices
        {
            get
            {
                var cursor = -90d;
                var slices = new List<PieSlice>();

                foreach (var point in Points)
                {
                    var sweep = point.Percent * 360;
                    var path = BuildSlicePath(cursor, cursor + sweep, 104);
                    slices.Add(new PieSlice(point, path, ColorClass(point.Index)));
                    cursor += sweep;
                }

                return slices;
            }
        }

        private string LinePath => PlotPoints.Count == 0
            ? string.Empty
            : BuildLinePath(PlotPoints);

        private string AreaPath => PlotPoints.Count == 0
            ? string.Empty
            : $"{LinePath} L {PlotPoints.Last().X:0.###} {PlotTop + PlotHeight:0.###} L {PlotPoints.First().X:0.###} {PlotTop + PlotHeight:0.###} Z";

        private IReadOnlyList<PointInfo> CategoryPoints => HasMultipleSeries
            ? ChartSeries.OrderByDescending(series => series.Points.Count).FirstOrDefault()?.Points ?? Array.Empty<PointInfo>()
            : Points;

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {SurfaceClass} {SizeClass}");
        }

        protected override void OnInitialized()
        {
            // honra o valor do usuário quando informado; default localizado caso contrário
            if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("chart.aria_label", "Gráfico");
            if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("chart.empty", "Nenhum dado para exibir.");
            if (string.IsNullOrWhiteSpace(AccessibleDescriptionFormat)) AccessibleDescriptionFormat = L("chart.description_format", "{0} pontos de dados.");
            if (string.IsNullOrWhiteSpace(SeriesHeaderText)) SeriesHeaderText = L("chart.series_header", "Série");
            if (string.IsNullOrWhiteSpace(LabelHeaderText)) LabelHeaderText = L("chart.label_header", "Rótulo");
            if (string.IsNullOrWhiteSpace(ValueHeaderText)) ValueHeaderText = L("chart.value_header", "Valor");
            if (string.IsNullOrWhiteSpace(PercentHeaderText)) PercentHeaderText = L("chart.percent_header", "Percentual");
            if (string.IsNullOrWhiteSpace(SeriesNameFormat)) SeriesNameFormat = L("chart.series_name", "Série {0}");
            if (string.IsNullOrWhiteSpace(ItemLabelFormat)) ItemLabelFormat = L("chart.item_label", "Item {0}");
            if (string.IsNullOrWhiteSpace(ExportFileName)) ExportFileName = L("chart.export_file", ExportFileName);
            if (string.IsNullOrWhiteSpace(ExportTitle)) ExportTitle = L("chart.export_title", ExportTitle);
            base.OnInitialized();
        }

        // assinatura de render + guarda do interop JS: o gráfico só é redesenhado quando os
        // parâmetros visuais mudam (Blazor re-renderiza o componente a cada StateHasChanged do pai)
        private string? _renderSignature;
        private string? _drawnSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Data is null ? -1 : Data.Count(), Series is null ? -1 : Series.Count, LabelProperty, ValueProperty, Type, StackMode, Tone, Size, Visible, ShowLegend, ShowSummary, ShowValueLabels, ShowDataTable, Title, Description, AriaLabel, EmptyText, AccessibleDescriptionFormat, SeriesHeaderText, LabelHeaderText, ValueHeaderText, PercentHeaderText, SeriesNameFormat, ItemLabelFormat, Culture, Palette is null ? -1 : Palette.Count, Annotations is null ? -1 : Annotations.Count, Trendlines is null ? -1 : Trendlines.Count, DataZoomMode, ExportEnabled, ExportFileName, ExportTitle, EChartsOptions, EmptyContent, PointSelected, LegendChanged, LiveStreaming, LiveStreamingInterval, LiveStreamingMaxPoints, OnDataUpdate);
            _renderSignature = signature;

            if (_drawnSignature == signature)
            {
                return false; // pai re-renderizou sem mudar nada do gráfico — não repinta o DOM
            }

            return true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_renderSignature is not null && _renderSignature == _drawnSignature)
            {
                return; // nada mudou — não re-invoca o módulo JS (interop caro)
            }

            _drawnSignature = _renderSignature;

            if (!Visible || VisiblePointCount == 0)
            {
                return;
            }

            try
            {
                chartModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/XForge.DesignSystem/js/xf-chart.js");
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
                return;
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
                return;
            }

            dotNetReference ??= DotNetObjectReference.Create(this);
            jsPointIndex = ActivePoints;

            try
            {
                await chartModule.InvokeVoidAsync("render", ChartElement, BuildEChartsOptions(), dotNetReference);
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
            }
        }

        private async Task SelectPointAsync(PointInfo point)
        {
            await PointSelected.InvokeAsync(new XFChartPointSelectedEventArgs<TItem>(point.Item, point.Label, point.Value, point.Percent, point.Index));
        }

        [JSInvokable]
        public Task SelectPointFromChart(int index)
        {
            return index >= 0 && index < jsPointIndex.Count
                ? SelectPointAsync(jsPointIndex[index])
                : Task.CompletedTask;
        }

        [JSInvokable]
        public Task LegendChangedFromChart(string? name, bool selected)
        {
            return !string.IsNullOrWhiteSpace(name) && LegendChanged.HasDelegate
                ? LegendChanged.InvokeAsync(new XFChartLegendChangedEventArgs(name, selected))
                : Task.CompletedTask;
        }

        private object BuildEChartsOptions()
        {
            var baseOptions = Type switch
            {
                XFChartType.Pie => BuildPieOptions(),
                XFChartType.Bar => BuildCartesianOptions("bar"),
                XFChartType.Scatter => BuildScatterOptions(),
                XFChartType.Candlestick => BuildCandlestickOptions(),
                XFChartType.Radar => BuildRadarOptions(),
                XFChartType.Heatmap => BuildHeatmapOptions(),
                XFChartType.Graph => BuildGraphOptions(false),
                XFChartType.Chord => BuildGraphOptions(true),
                XFChartType.Tree => BuildTreeOptions(),
                XFChartType.Treemap => BuildTreemapOptions(),
                XFChartType.Sunburst => BuildSunburstOptions(),
                XFChartType.Sankey => BuildSankeyOptions(),
                XFChartType.Funnel => BuildFunnelOptions(),
                XFChartType.Gauge => BuildGaugeOptions(),
                XFChartType.ThemeRiver => BuildThemeRiverOptions(),
                XFChartType.Calendar => BuildCalendarOptions(),
                _ => BuildCartesianOptions("line")
            };

            return EChartsOptions is null
                ? baseOptions
                : new
                {
                    xfBaseOptions = baseOptions,
                    xfAdditionalOptions = EChartsOptions
                };
        }

        private object BuildCartesianOptions(string chartType)
        {
            var categories = CategoryPoints.Select(point => point.Label).ToArray();
            var series = HasMultipleSeries
                ? ChartSeries.Select((series, seriesIndex) => new
                {
                    name = series.Name,
                    type = chartType,
                    smooth = chartType == "line",
                    itemStyle = new { color = ColorHex(series.Index, series.Tone) },
                    label = new { show = ShowValueLabels, position = "top" },
                    lineStyle = new { width = 3 },
                    markLine = seriesIndex == 0 ? BuildMarkLine() : null,
                    markPoint = seriesIndex == 0 ? BuildMarkPoint() : null,
                    data = series.Points.Select(point => new { value = point.Value, xfIndex = ActivePoints.ToList().IndexOf(point) }).ToArray()
                }).ToArray<object>()
                : new object[]
                {
                    new
                    {
                        name = AccessibleTitle,
                        type = chartType,
                        smooth = chartType == "line",
                        areaStyle = chartType == "line" ? new { opacity = 0.08 } : null,
                        itemStyle = new { color = ColorHex(0, Tone) },
                        label = new { show = ShowValueLabels, position = "top" },
                        lineStyle = new { width = 3 },
                        markLine = BuildMarkLine(),
                        markPoint = BuildMarkPoint(),
                        data = Points.Select((point, index) => new { value = point.Value, xfIndex = index }).ToArray()
                    }
                };

            return new
            {
                animation = !LiveStreaming,
                animationDuration = LiveStreaming ? 300 : 500,
                animationEasing = LiveStreaming ? "linear" : "cubicInOut",
                color = EffectivePalette,
                tooltip = new { trigger = "axis" },
                legend = new { show = ShowLegend && HasMultipleSeries, top = 0 },
                toolbox = BuildToolbox(),
                dataZoom = BuildDataZoom(),
                grid = new { left = 44, right = 20, top = ShowLegend && HasMultipleSeries ? 42 : 18, bottom = 32, containLabel = true },
                xAxis = new { type = "category", data = categories, axisLabel = new { color = "#64748b" }, axisLine = new { lineStyle = new { color = "#cbd5e1" } } },
                yAxis = new { type = "value", axisLabel = new { color = "#64748b" }, splitLine = new { lineStyle = new { color = "#e2e8f0" } } },
                series
            };
        }

        private object? BuildToolbox()
        {
            return ExportEnabled
                ? new
                {
                    show = true,
                    feature = new
                    {
                        saveAsImage = new
                        {
                            name = ExportFileName,
                            title = ExportTitle
                        }
                    }
                }
                : null;
        }

        private object[]? BuildDataZoom()
        {
            return DataZoomMode switch
            {
                XFChartDataZoomMode.Slider => new object[] { new { type = "slider", bottom = 0 } },
                XFChartDataZoomMode.Inside => new object[] { new { type = "inside" } },
                XFChartDataZoomMode.SliderAndInside => new object[] { new { type = "slider", bottom = 0 }, new { type = "inside" } },
                _ => null
            };
        }

        private object? BuildMarkPoint()
        {
            if (Annotations.Count == 0)
            {
                return null;
            }

            return new
            {
                symbol = "pin",
                data = Annotations.Select(annotation => new
                {
                    name = annotation.Label,
                    yAxis = annotation.Axis == XFChartAnnotationAxis.Y ? annotation.Value : (double?)null,
                    xAxis = annotation.Axis == XFChartAnnotationAxis.X ? annotation.Value : (double?)null,
                    itemStyle = new { color = ColorHex(0, annotation.Tone) }
                }).ToArray()
            };
        }

        private object? BuildMarkLine()
        {
            var data = new List<object>();

            foreach (var annotation in Annotations)
            {
                data.Add(new
                {
                    name = annotation.Label,
                    yAxis = annotation.Axis == XFChartAnnotationAxis.Y ? annotation.Value : (double?)null,
                    xAxis = annotation.Axis == XFChartAnnotationAxis.X ? annotation.Value : (double?)null,
                    lineStyle = new { color = ColorHex(0, annotation.Tone), type = "dashed" }
                });
            }

            foreach (var trendline in Trendlines)
            {
                data.Add(new object[]
                {
                    new
                    {
                        name = trendline.Name,
                        coord = new object[] { trendline.StartIndex, trendline.StartValue },
                        lineStyle = new { color = ColorHex(0, trendline.Tone), type = "solid" }
                    },
                    new
                    {
                        coord = new object[] { trendline.EndIndex, trendline.EndValue }
                    }
                });
            }

            return data.Count == 0
                ? null
                : new
                {
                    symbol = new[] { "none", "none" },
                    data = data.ToArray()
                };
        }

        private object BuildPieOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                legend = new { show = ShowLegend, orient = "vertical", right = 0, top = "middle" },
                series = new object[]
                {
                    new
                    {
                        name = AccessibleTitle,
                        type = "pie",
                        radius = new[] { "42%", "70%" },
                        center = ShowLegend ? new[] { "38%", "50%" } : new[] { "50%", "50%" },
                        avoidLabelOverlap = true,
                        data = Points.Select((point, index) => new
                        {
                            name = point.Label,
                            value = point.Value,
                            xfIndex = index,
                            label = new { show = ShowValueLabels },
                            itemStyle = new { color = ColorHex(index, null) }
                        }).ToArray()
                    }
                }
            };
        }

        private object BuildScatterOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                grid = new { left = 44, right = 24, top = 24, bottom = 36, containLabel = true },
                xAxis = new { type = "value", axisLabel = new { color = "#64748b" } },
                yAxis = new { type = "value", axisLabel = new { color = "#64748b" } },
                series = new object[]
                {
                    new
                    {
                        name = AccessibleTitle,
                        type = "scatter",
                        symbolSize = 12,
                        data = Points.Select((point, index) => new { value = new[] { index + 1d, point.Value }, xfIndex = index, name = point.Label }).ToArray()
                    }
                }
            };
        }

        private object BuildCandlestickOptions()
        {
            var categories = Points.Select(point => point.Label).ToArray();
            var data = Points.Select(point =>
            {
                var open = Math.Max(0, point.Value * 0.92);
                var close = point.Value;
                var low = Math.Min(open, close) * 0.94;
                var high = Math.Max(open, close) * 1.08;
                return new[] { open, close, low, high };
            }).ToArray();

            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "axis" },
                grid = new { left = 52, right = 24, top = 24, bottom = 36, containLabel = true },
                xAxis = new { type = "category", data = categories },
                yAxis = new { type = "value", scale = true },
                series = new object[] { new { name = AccessibleTitle, type = "candlestick", data } }
            };
        }

        private object BuildRadarOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                radar = new { indicator = Points.Select(point => new { name = point.Label, max = MaxValue * 1.2 }).ToArray() },
                series = new object[] { new { name = AccessibleTitle, type = "radar", data = new object[] { new { value = Points.Select(point => point.Value).ToArray(), name = AccessibleTitle } } } }
            };
        }

        private object BuildHeatmapOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { position = "top" },
                grid = new { left = 72, right = 24, top = 24, bottom = 36 },
                xAxis = new { type = "category", data = Points.Select(point => point.Label).ToArray(), splitArea = new { show = true } },
                yAxis = new { type = "category", data = new[] { AccessibleTitle }, splitArea = new { show = true } },
                visualMap = new { min = 0, max = MaxValue, calculable = true, orient = "horizontal", left = "center", bottom = 0 },
                series = new object[] { new { name = AccessibleTitle, type = "heatmap", data = Points.Select((point, index) => new object[] { index, 0, point.Value }).ToArray(), label = new { show = ShowValueLabels } } }
            };
        }

        private object BuildGraphOptions(bool chordStyle)
        {
            var nodes = Points.Select((point, index) => new { id = point.Label, name = point.Label, value = point.Value, symbolSize = Math.Max(20, Math.Min(72, 20 + point.Value / MaxValue * 48)), itemStyle = new { color = ColorHex(index, null) }, xfIndex = index }).ToArray();
            var links = Points.Select((point, index) => new { source = point.Label, target = Points[(index + 1) % Points.Count].Label, value = point.Value }).ToArray();
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                series = new object[]
                {
                    new
                    {
                        name = AccessibleTitle,
                        type = "graph",
                        layout = chordStyle ? "circular" : "force",
                        roam = true,
                        label = new { show = true },
                        edgeSymbol = chordStyle ? new[] { "none", "none" } : new[] { "none", "arrow" },
                        data = nodes,
                        links
                    }
                }
            };
        }

        private object BuildTreeOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                series = new object[] { new { type = "tree", data = new object[] { BuildHierarchyRoot() }, top = "6%", left = "12%", bottom = "6%", right = "18%", symbolSize = 9, label = new { position = "left", verticalAlign = "middle", align = "right" }, leaves = new { label = new { position = "right", verticalAlign = "middle", align = "left" } }, expandAndCollapse = true } }
            };
        }

        private object BuildTreemapOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                series = new object[] { new { name = AccessibleTitle, type = "treemap", roam = false, breadcrumb = new { show = false }, label = new { show = true }, data = Points.Select((point, index) => new { name = point.Label, value = point.Value, itemStyle = new { color = ColorHex(index, null) }, xfIndex = index }).ToArray() } }
            };
        }

        private object BuildSunburstOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                series = new object[] { new { type = "sunburst", radius = new[] { "12%", "88%" }, data = Points.Select((point, index) => new { name = point.Label, value = point.Value, itemStyle = new { color = ColorHex(index, null) }, xfIndex = index }).ToArray(), label = new { rotate = "radial" } } }
            };
        }

        private object BuildSankeyOptions()
        {
            var nodes = Points.Select(point => new { name = point.Label }).ToArray();
            var links = Points.Skip(1).Select((point, index) => new { source = Points[index].Label, target = point.Label, value = Math.Max(1, point.Value) }).ToArray();
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                series = new object[] { new { type = "sankey", emphasis = new { focus = "adjacency" }, data = nodes, links, lineStyle = new { color = "gradient", curveness = 0.5 } } }
            };
        }

        private object BuildFunnelOptions()
        {
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                legend = new { show = ShowLegend, bottom = 0 },
                series = new object[] { new { name = AccessibleTitle, type = "funnel", left = "8%", top = 24, bottom = 42, width = "84%", sort = "descending", label = new { show = true, position = "inside" }, data = Points.Select((point, index) => new { name = point.Label, value = point.Value, xfIndex = index }).ToArray() } }
            };
        }

        private object BuildGaugeOptions()
        {
            var point = Points.FirstOrDefault();
            var value = point?.Value ?? 0;
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "item" },
                series = new object[] { new { name = AccessibleTitle, type = "gauge", min = 0, max = Math.Max(100, MaxValue), progress = new { show = true, width = 16 }, axisLine = new { lineStyle = new { width = 16 } }, detail = new { valueAnimation = true, formatter = "{value}" }, data = new object[] { new { value, name = point?.Label ?? AccessibleTitle, xfIndex = 0 } } } }
            };
        }

        private object BuildThemeRiverOptions()
        {
            var start = new DateTime(DateTime.Today.Year, 1, 1);
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { trigger = "axis" },
                legend = new { show = ShowLegend, bottom = 0 },
                singleAxis = new { type = "time", top = 40, bottom = 48 },
                series = new object[] { new { type = "themeRiver", emphasis = new { itemStyle = new { shadowBlur = 20, shadowColor = "rgba(0,0,0,0.25)" } }, data = Points.Select((point, index) => new object[] { start.AddMonths(index).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture), point.Value, point.Label }).ToArray() } }
            };
        }

        private object BuildCalendarOptions()
        {
            var start = new DateTime(DateTime.Today.Year, 1, 1);
            return new
            {
                animation = true,
                color = EffectivePalette,
                tooltip = new { position = "top" },
                visualMap = new { min = 0, max = MaxValue, calculable = true, orient = "horizontal", left = "center", top = 0 },
                calendar = new { top = 56, left = 32, right = 32, cellSize = new object[] { "auto", 18 }, range = DateTime.Today.Year.ToString(CultureInfo.InvariantCulture), itemStyle = new { borderWidth = 0.5 } },
                series = new object[] { new { type = "heatmap", coordinateSystem = "calendar", data = Points.Select((point, index) => new object[] { start.AddDays(index).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), point.Value }).ToArray() } }
            };
        }

        private object BuildHierarchyRoot()
        {
            return new
            {
                name = AccessibleTitle,
                children = Points.Select((point, index) => new { name = point.Label, value = point.Value, itemStyle = new { color = ColorHex(index, null) }, xfIndex = index }).ToArray()
            };
        }

        private PointInfo? ToPoint(TItem item, int index)
        {
            return ToPoint(item, index, LabelSelector, ValueSelector, LabelProperty, ValueProperty, null, 0);
        }

        private PointInfo? ToPoint(TItem item, int index, XFChartSeries<TItem> series, int seriesIndex)
        {
            return ToPoint(
                item,
                index,
                series.LabelSelector ?? LabelSelector,
                series.ValueSelector ?? ValueSelector,
                series.LabelProperty ?? LabelProperty,
                series.ValueProperty ?? ValueProperty,
                string.IsNullOrWhiteSpace(series.Name) ? string.Format(EffectiveCulture, SeriesNameFormat, seriesIndex + 1) : series.Name,
                seriesIndex);
        }

        private PointInfo? ToPoint(
            TItem item,
            int index,
            Func<TItem, string>? labelSelector,
            Func<TItem, double>? valueSelector,
            string labelProperty,
            string valueProperty,
            string? seriesName,
            int seriesIndex)
        {
            if (item is null)
            {
                return null;
            }

            var label = labelSelector?.Invoke(item) ?? GetPropertyValue<string>(item, labelProperty) ?? string.Format(EffectiveCulture, ItemLabelFormat, index + 1);
            var value = valueSelector?.Invoke(item) ?? GetNumericValue(item, valueProperty);
            return value is null ? null : new PointInfo(item, label, value.Value, 0, index, seriesName, seriesIndex);
        }

        private string FormatValue(double value)
        {
            return ValueFormatter?.Invoke(value) ?? value.ToString("N0", EffectiveCulture);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        private static T? GetPropertyValue<T>(object item, string propertyName)
        {
            var property = item.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            var value = property?.GetValue(item);
            return value is T typed ? typed : default;
        }

        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        private static double? GetNumericValue(object item, string propertyName)
        {
            var property = item.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            var value = property?.GetValue(item);
            return value switch
            {
                double doubleValue => doubleValue,
                float floatValue => floatValue,
                decimal decimalValue => (double)decimalValue,
                int intValue => intValue,
                long longValue => longValue,
                short shortValue => shortValue,
                string text when double.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed) => parsed,
                _ => null
            };
        }

        private static string BuildSlicePath(double startAngle, double endAngle, double radius)
        {
            if (endAngle - startAngle >= 359.99)
            {
                return $"M 0 {-radius:0.###} A {radius:0.###} {radius:0.###} 0 1 1 0 {radius:0.###} A {radius:0.###} {radius:0.###} 0 1 1 0 {-radius:0.###} Z";
            }

            var start = PolarToCartesian(radius, endAngle);
            var end = PolarToCartesian(radius, startAngle);
            var largeArc = endAngle - startAngle <= 180 ? 0 : 1;
            return $"M 0 0 L {start.X:0.###} {start.Y:0.###} A {radius:0.###} {radius:0.###} 0 {largeArc} 0 {end.X:0.###} {end.Y:0.###} Z";
        }

        private static (double X, double Y) PolarToCartesian(double radius, double angle)
        {
            var radians = angle * Math.PI / 180;
            return (Math.Cos(radians) * radius, Math.Sin(radians) * radius);
        }

        private static string BuildLinePath(IReadOnlyList<PlotPoint> points)
        {
            return string.Join(" ", points.Select((point, index) => $"{(index == 0 ? "M" : "L")} {point.X:0.###} {point.Y:0.###}"));
        }

        private string SurfaceClass => "rounded-xl border border-border-light bg-bg-surface p-4 text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)]";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "gap-3",
            XFSize.Lg => "gap-5",
            _ => "gap-4"
        };

        private string SvgClass => Size switch
        {
            XFSize.Sm => "h-48 w-full overflow-visible",
            XFSize.Lg => "h-80 w-full overflow-visible",
            _ => "h-64 w-full overflow-visible"
        };

        private string EmptyClass => "flex min-h-48 items-center justify-center rounded-lg border border-dashed border-border-light bg-surface-100 text-sm font-semibold text-text-muted";

        private string ChartHostClass => Size switch
        {
            XFSize.Sm => "h-48 w-full",
            XFSize.Lg => "h-80 w-full",
            _ => "h-64 w-full"
        };

        private string ToneStrokeClass => Tone switch
        {
            XFSemanticColor.Secondary => "text-secondary",
            XFSemanticColor.Success => "text-success",
            XFSemanticColor.Warning => "text-warning",
            XFSemanticColor.Danger => "text-danger",
            XFSemanticColor.Neutral => "text-neutral",
            _ => "text-primary"
        };

        private string ToneFillClass => ToneStrokeClass;

        private string ColorClass(int index)
        {
            if (Type != XFChartType.Pie && Tone != XFSemanticColor.Primary)
            {
                return ToneFillClass;
            }

            return (index % 6) switch
            {
                1 => "text-secondary",
                2 => "text-success",
                3 => "text-warning",
                4 => "text-danger",
                5 => "text-neutral",
                _ => "text-primary"
            };
        }

        private string SeriesColorClass(SeriesInfo series)
        {
            return series.Tone is { } tone ? ToneClass(tone) : ColorClass(series.Index);
        }

        private static string ToneClass(XFSemanticColor tone)
        {
            return tone switch
            {
                XFSemanticColor.Secondary => "text-secondary",
                XFSemanticColor.Success => "text-success",
                XFSemanticColor.Warning => "text-warning",
                XFSemanticColor.Danger => "text-danger",
                XFSemanticColor.Neutral => "text-neutral",
                _ => "text-primary"
            };
        }

        private string LegendDotClass(int index)
        {
            return $"h-2.5 w-2.5 shrink-0 rounded-full {ColorClass(index)} bg-current";
        }

        private string ColorHex(int index, XFSemanticColor? tone)
        {
            return tone switch
            {
                XFSemanticColor.Secondary => "#0ea5e9",
                XFSemanticColor.Success => "#16a34a",
                XFSemanticColor.Warning => "#d97706",
                XFSemanticColor.Danger => "#dc2626",
                XFSemanticColor.Neutral => "#64748b",
                _ => EffectivePalette[index % EffectivePalette.Count]
            };
        }

        public async ValueTask DisposeAsync()
        {
            if (chartModule is not null)
            {
                try
                {
                    await chartModule.InvokeVoidAsync("dispose", ChartElement);
                    await chartModule.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                }
            }

            dotNetReference?.Dispose();
        }

        private sealed record PointInfo(TItem Item, string Label, double Value, double Percent, int Index, string? SeriesName = null, int SeriesIndex = 0);
        private sealed record PlotPoint(PointInfo Point, double X, double Y);
        private sealed record SeriesInfo(string Name, int Index, IReadOnlyList<PointInfo> Points, XFSemanticColor? Tone);
        private sealed record SeriesPlotInfo(string Name, int Index, IReadOnlyList<PlotPoint> Points, string ColorClass);
        private sealed record BarInfo(PointInfo Point, double X, double Y, double Width, double Height, double CenterX, string ColorClass);
        private sealed record ValueTick(double Value, double Y);
        private sealed record PieSlice(PointInfo Point, string Path, string ColorClass);
    }
}
