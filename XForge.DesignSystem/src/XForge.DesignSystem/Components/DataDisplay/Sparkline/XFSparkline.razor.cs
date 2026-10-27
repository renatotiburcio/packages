using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Sparkline
{
    public partial class XFSparkline<TItem> : XForgeComponentBase
    {
        private bool _showTooltip;
        private int _hoverIndex = -1;
        private double _tooltipX;
        private double _tooltipY;

        /// <summary>Whether the sparkline is visible.</summary>
        [Parameter] public bool Visible { get; set; } = true;

        /// <summary>Data items to render.</summary>
        [Parameter, EditorRequired] public IEnumerable<TItem>? Data { get; set; }

        /// <summary>Selector for the numeric value.</summary>
        [Parameter, EditorRequired] public Func<TItem, double>? ValueSelector { get; set; }

        /// <summary>Optional label selector for tooltip text.</summary>
        [Parameter] public Func<TItem, string>? LabelSelector { get; set; }

        /// <summary>Type of sparkline (Line, Area, Bar).</summary>
        [Parameter] public XFSparklineType Type { get; set; } = XFSparklineType.Line;

        /// <summary>Color of the sparkline. Uses CSS color value.</summary>
        [Parameter] public string? Color { get; set; }

        /// <summary>Stroke width for line/area types.</summary>
        [Parameter] public double StrokeWidth { get; set; } = 1.5;

        /// <summary>Whether to show dots at data points.</summary>
        [Parameter] public bool ShowDots { get; set; }

        /// <summary>Radius of dots.</summary>
        [Parameter] public double DotRadius { get; set; } = 2;

        /// <summary>Height of the sparkline container.</summary>
        [Parameter] public string Height { get; set; } = "2rem";

        /// <summary>Format string for tooltip values.</summary>
        [Parameter] public string? ValueFormat { get; set; }

        /// <summary>Culture for value formatting.</summary>
        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>Accessible label for the sparkline.</summary>
        [Parameter] public string? AriaLabel { get; set; } = "Gráfico sparkline";

        /// <summary>Tooltip text template. {0}=value, {1}=label.</summary>
        [Parameter] public string? TooltipFormat { get; set; }

        private const int ViewBoxWidth = 200;
        private const int ViewBoxHeight = 40;

        private string GradientId => $"xf-sparkline-gradient-{Id}";
        private string EffectiveColor => Color ?? "var(--color-primary)";
        private bool HasData => DataPoints.Count > 0;

        private List<TItem> RawItems => Data?.Where(d => d is not null).ToList() ?? new();

        private List<(TItem Item, double Value)> ValidData => RawItems
            .Select(d => (Item: d, Value: ValueSelector!(d)))
            .Where(d => !double.IsNaN(d.Value) && !double.IsInfinity(d.Value))
            .ToList();

        private List<double> DataPoints => ValidData.Select(d => d.Value).ToList();

        private List<(double X, double Y)> ComputedPoints
        {
            get
            {
                var points = DataPoints;
                if (points.Count == 0) return new();

                var min = points.Min();
                var max = points.Max();
                var range = max - min;
                if (range == 0) range = 1; // flat line

                var step = points.Count > 1 ? ViewBoxWidth / (double)(points.Count - 1) : ViewBoxWidth;
                return points.Select((v, i) => (
                    X: i * step,
                    Y: ViewBoxHeight - ((v - min) / range * (ViewBoxHeight - 4) + 2)
                )).ToList();
            }
        }

        private string LinePoints => string.Join(" ", ComputedPoints.Select(p => $"{p.X:F1},{p.Y:F1}"));

        private string AreaPath
        {
            get
            {
                var pts = ComputedPoints;
                if (pts.Count == 0) return string.Empty;
                var line = string.Join(" L", pts.Select(p => $"{p.X:F1},{p.Y:F1}"));
                return $"M{line} L{pts[^1].X:F1},{ViewBoxHeight} L{pts[0].X:F1},{ViewBoxHeight} Z";
            }
        }

        private IEnumerable<(double X, double Y, double Width, double Height)> BarRects
        {
            get
            {
                var pts = ComputedPoints;
                if (pts.Count == 0) yield break;
                var barWidth = Math.Max(1, ViewBoxWidth / (double)pts.Count - 1);
                foreach (var pt in pts)
                {
                    yield return (pt.X - barWidth / 2, pt.Y, barWidth, ViewBoxHeight - pt.Y);
                }
            }
        }

        private IEnumerable<(double X, double Y)> DotPositions => ComputedPoints;

        private string RootClass => Join("xf-sparkline relative inline-block", Class);

        private string TooltipClass => "absolute z-10 pointer-events-none -translate-x-1/2 -translate-y-full -top-1 rounded bg-surface-800 px-2 py-0.5 text-xs text-white shadow-lg";

        private string StrokeWidthFmt => StrokeWidth.ToString(CultureInfo.InvariantCulture);
        private string DotRadiusFmt => DotRadius.ToString(CultureInfo.InvariantCulture);
        private string DotRadiusPlusOneFmt => (DotRadius + 1).ToString(CultureInfo.InvariantCulture);

        private string FormatTooltipValue(TItem item)
        {
            var value = ValueSelector!(item);
            var formatted = string.IsNullOrWhiteSpace(ValueFormat)
                ? value.ToString("G", Culture)
                : value.ToString(ValueFormat, Culture);

            if (LabelSelector is not null)
            {
                var label = LabelSelector(item);
                return string.IsNullOrWhiteSpace(TooltipFormat)
                    ? $"{label}: {formatted}"
                    : string.Format(Culture, TooltipFormat, formatted, label);
            }

            return formatted;
        }

        private TItem? GetHoveredItem()
        {
            if (_hoverIndex < 0 || _hoverIndex >= ValidData.Count) return default;
            return ValidData[_hoverIndex].Item;
        }

        private async Task HandleMouseMoveAsync(MouseEventArgs args)
        {
            var pts = ComputedPoints;
            if (pts.Count == 0) return;

            // Find closest point by X distance
            var relativeX = args.OffsetX;
            var containerWidth = 200.0; // approximate, refined by viewBox ratio
            var viewBoxX = relativeX / containerWidth * ViewBoxWidth;

            var closest = pts.Select((p, i) => (Point: p, Index: i))
                .OrderBy(p => Math.Abs(p.Point.X - viewBoxX))
                .First();

            _hoverIndex = closest.Index;
            _tooltipX = args.OffsetX;
            _tooltipY = args.OffsetY - 10;
            StateHasChanged();
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
        }
    }
}
