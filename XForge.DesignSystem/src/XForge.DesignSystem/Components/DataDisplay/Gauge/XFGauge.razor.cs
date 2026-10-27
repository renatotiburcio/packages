using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Gauge
{
    /// <summary>
    /// SVG-based radial/arc gauge component for displaying a value on a scale.
    /// Supports tick marks, labels, color ranges, and custom value templates.
    /// </summary>
    public partial class XFGauge : XForgeComponentBase
    {
        protected override string BaseClass => "xf-gauge";

        /// <summary>Current value to display.</summary>
        [Parameter] public double Value { get; set; }

        /// <summary>Minimum scale value.</summary>
        [Parameter] public double Min { get; set; }

        /// <summary>Maximum scale value.</summary>
        [Parameter] public double Max { get; set; } = 100;

        /// <summary>Arc start angle in degrees (0° = top, 90° = right, default -135°).</summary>
        [Parameter] public double StartAngle { get; set; } = -135;

        /// <summary>Arc end angle in degrees (0° = top, 90° = right, default 135°).</summary>
        [Parameter] public double EndAngle { get; set; } = 135;

        /// <summary>Size preset.</summary>
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;

        /// <summary>Arc stroke thickness in SVG units.</summary>
        [Parameter] public double Thickness { get; set; } = 10;

        /// <summary>Value arc color (Tailwind stroke class or CSS color). Null = primary.</summary>
        [Parameter] public string? Color { get; set; }

        /// <summary>Track (background) arc color. Null = surface-200.</summary>
        [Parameter] public string? TrackColor { get; set; }

        /// <summary>Whether to show the center value text.</summary>
        [Parameter] public bool ShowValue { get; set; } = true;

        /// <summary>Custom value rendering template.</summary>
        [Parameter] public RenderFragment<XFGaugeValueContext>? ValueTemplate { get; set; }

        /// <summary>Composite format string for value (args: {0}=value, {1}=percent).</summary>
        [Parameter] public string? ValueFormat { get; set; }

        /// <summary>Unit suffix appended to value.</summary>
        [Parameter] public string? Unit { get; set; }

        /// <summary>Color range bands rendered on the arc.</summary>
        [Parameter] public IReadOnlyList<XFGaugeRange>? Ranges { get; set; }

        /// <summary>Whether to show tick marks along the arc.</summary>
        [Parameter] public bool ShowTicks { get; set; }

        /// <summary>Major tick interval. 0 = auto (10 ticks).</summary>
        [Parameter] public double TickStep { get; set; }

        /// <summary>Whether to show tick labels.</summary>
        [Parameter] public bool ShowLabels { get; set; }

        /// <summary>Format string for tick labels (args: {0}=tick value).</summary>
        [Parameter] public string? LabelFormat { get; set; }

        /// <summary>Whether to show the min/max labels at arc endpoints.</summary>
        [Parameter] public bool ShowMinMaxLabels { get; set; }

        /// <summary>Accessibility label for the gauge.</summary>
        [Parameter] public string? AriaLabel { get; set; }

        // ─── Derived Properties ───────────────────────────────────────

        private const double ViewBoxSize = 200;
        private const double Center = 100;
        private double Radius => Center - Thickness / 2 - (ShowTicks ? 12 : 4);

        private double Range => Math.Max(0.0001, Max - Min);
        private double ClampedValue => Math.Clamp(Value, Min, Max);
        private double Percent => ((ClampedValue - Min) / Range) * 100;

        private double SweepAngle
        {
            get
            {
                var sweep = EndAngle - StartAngle;
                return sweep < 0 ? sweep + 360 : sweep;
            }
        }

        private double ValueAngle => StartAngle + SweepAngle * Percent / 100;

        private string AriaValueNow => ClampedValue.ToString("0.###", CultureInfo.InvariantCulture);

        private string AriaValueText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ValueFormat))
                    return string.Format(CultureInfo.CurrentCulture, ValueFormat, ClampedValue, Percent);
                return Unit is not null ? $"{ClampedValue:0.##} {Unit}" : $"{ClampedValue:0.##}";
            }
        }

        private string ValueText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ValueFormat))
                    return string.Format(CultureInfo.CurrentCulture, ValueFormat, ClampedValue, Percent);
                return Unit is not null ? $"{ClampedValue:0.##} {Unit}" : $"{ClampedValue:0.##}";
            }
        }

        // ─── Size Maps ────────────────────────────────────────────────

        private string SizeClass => Size switch
        {
            XFExtendedSize.Xs => "h-16 w-16",
            XFExtendedSize.Sm => "h-24 w-24",
            XFExtendedSize.Lg => "h-56 w-56",
            XFExtendedSize.Xl => "h-72 w-72",
            _ => "h-40 w-40"
        };

        private string ValueTextSizeClass => Size switch
        {
            XFExtendedSize.Xs => "text-xs",
            XFExtendedSize.Sm => "text-sm",
            XFExtendedSize.Lg => "text-2xl",
            XFExtendedSize.Xl => "text-3xl",
            _ => "text-lg"
        };

        private string UnitTextSizeClass => Size switch
        {
            XFExtendedSize.Xs => "text-[8px]",
            XFExtendedSize.Sm => "text-[10px]",
            XFExtendedSize.Lg => "text-sm",
            XFExtendedSize.Xl => "text-base",
            _ => "text-xs"
        };

        private string TickLabelSizeClass => Size switch
        {
            XFExtendedSize.Xs => "text-[7px]",
            XFExtendedSize.Sm => "text-[8px]",
            XFExtendedSize.Lg => "text-[11px]",
            XFExtendedSize.Xl => "text-xs",
            _ => "text-[9px]"
        };

        // ─── Color Resolution ─────────────────────────────────────────

        private string TrackStroke => string.IsNullOrWhiteSpace(TrackColor)
            ? "stroke-surface-200 dark:stroke-surface-700"
            : TrackColor.StartsWith("stroke-") ? TrackColor : "";

        private string TrackStrokeStyle => string.IsNullOrWhiteSpace(TrackColor) || TrackColor.StartsWith("stroke-")
            ? ""
            : $"stroke:{TrackColor}";

        private string ValueStroke => string.IsNullOrWhiteSpace(Color)
            ? "stroke-primary"
            : Color.StartsWith("stroke-") ? Color : "";

        private string ValueStrokeStyle => string.IsNullOrWhiteSpace(Color) || Color.StartsWith("stroke-")
            ? ""
            : $"stroke:{Color}";

        private string RangeStrokeClass(string? rangeColor)
        {
            if (string.IsNullOrWhiteSpace(rangeColor)) return "stroke-surface-400";
            return rangeColor.StartsWith("stroke-") ? rangeColor : "";
        }

        private string RangeStrokeStyle(string? rangeColor)
        {
            if (string.IsNullOrWhiteSpace(rangeColor) || rangeColor.StartsWith("stroke-")) return "";
            return $"stroke:{rangeColor}";
        }

        // ─── SVG Arc Helpers ──────────────────────────────────────────

        private (double x, double y) PolarToCartesian(double angleDeg)
        {
            var rad = angleDeg * Math.PI / 180.0;
            return (Center + Radius * Math.Sin(rad), Center - Radius * Math.Cos(rad));
        }

        private string DescribeArc(double startAngle, double endAngle, double? radius = null)
        {
            var r = radius ?? Radius;
            var startRad = startAngle * Math.PI / 180.0;
            var endRad = endAngle * Math.PI / 180.0;
            var sx = Center + r * Math.Sin(startRad);
            var sy = Center - r * Math.Cos(startRad);
            var ex = Center + r * Math.Sin(endRad);
            var ey = Center - r * Math.Cos(endRad);

            var sweep = endAngle - startAngle;
            if (sweep < 0) sweep += 360;
            var largeArc = sweep > 180 ? 1 : 0;

            return FormattableString.Invariant(
                $"M {sx:0.###} {sy:0.###} A {r:0.###} {r:0.###} 0 {largeArc} 1 {ex:0.###} {ey:0.###}");
        }

        // ─── Tick Generation ──────────────────────────────────────────

        private double EffectiveTickStep => TickStep > 0 ? TickStep : Range / 10;

        private IEnumerable<(double value, double angle)> GetTicks()
        {
            var step = EffectiveTickStep;
            var v = Min;
            while (v <= Max + step * 0.001)
            {
                var clamped = Math.Clamp(v, Min, Max);
                var pct = (clamped - Min) / Range;
                var angle = StartAngle + SweepAngle * pct;
                yield return (clamped, angle);
                v += step;
            }
        }

        private string FormatTickLabel(double value)
        {
            if (!string.IsNullOrWhiteSpace(LabelFormat))
                return string.Format(CultureInfo.CurrentCulture, LabelFormat, value);
            return value.ToString("0.##", CultureInfo.CurrentCulture);
        }

        // ─── Tick Geometry ────────────────────────────────────────────

        private const double TickOuterOffset = 4;
        private const double MajorTickLength = 8;
        private const double MinorTickLength = 4;
        private const double LabelOffset = 16;

        private (double x1, double y1, double x2, double y2) GetTickLine(double angle, bool major)
        {
            var len = major ? MajorTickLength : MinorTickLength;
            var rad = angle * Math.PI / 180.0;
            var outerR = Radius + Thickness / 2 + TickOuterOffset;
            var innerR = outerR + len;
            return (
                Center + outerR * Math.Sin(rad),
                Center - outerR * Math.Cos(rad),
                Center + innerR * Math.Sin(rad),
                Center - innerR * Math.Cos(rad)
            );
        }

        private (double x, double y) GetLabelPosition(double angle)
        {
            var rad = angle * Math.PI / 180.0;
            var r = Radius + Thickness / 2 + TickOuterOffset + MajorTickLength + LabelOffset;
            return (Center + r * Math.Sin(rad), Center - r * Math.Cos(rad));
        }

        // ─── SVG Text Helper ──────────────────────────────────────────
        // Razor treats <text> as a special element; render SVG <text> as MarkupString.

        private static string RenderSvgText(double x, double y, string content, string cssClass)
        {
            return FormattableString.Invariant(
                $"<text x=\"{x:0.###}\" y=\"{y:0.###}\" text-anchor=\"middle\" dominant-baseline=\"central\" class=\"{cssClass}\">{System.Net.WebUtility.HtmlEncode(content)}</text>");
        }

        // ─── Component Class ──────────────────────────────────────────

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} relative inline-grid shrink-0 place-items-center {SizeClass}");
        }
    }
}
