using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Components.Layout;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Gauge
{
    /// <summary>
    /// SVG-based linear gauge component for displaying a value on a horizontal or vertical scale.
    /// Supports color ranges, custom value templates, and reversed direction.
    /// </summary>
    public partial class XFLinearGauge : XForgeComponentBase
    {
        protected override string BaseClass => "xf-linear-gauge";

        /// <summary>Current value to display.</summary>
        [Parameter] public double Value { get; set; }

        /// <summary>Minimum scale value.</summary>
        [Parameter] public double Min { get; set; }

        /// <summary>Maximum scale value.</summary>
        [Parameter] public double Max { get; set; } = 100;

        /// <summary>Bar orientation.</summary>
        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Horizontal;

        /// <summary>Whether to reverse the fill direction.</summary>
        [Parameter] public bool Reversed { get; set; }

        /// <summary>Size preset (affects height/width and text).</summary>
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;

        /// <summary>Bar thickness in SVG units.</summary>
        [Parameter] public double Thickness { get; set; } = 8;

        /// <summary>Value bar color (Tailwind fill class or CSS color). Null = primary.</summary>
        [Parameter] public string? Color { get; set; }

        /// <summary>Track (background) color. Null = surface-200.</summary>
        [Parameter] public string? TrackColor { get; set; }

        /// <summary>Whether to show the value text.</summary>
        [Parameter] public bool ShowValue { get; set; } = true;

        /// <summary>Position of value text relative to the bar.</summary>
        [Parameter] public XFGaugeValuePosition ValuePosition { get; set; } = XFGaugeValuePosition.End;

        /// <summary>Custom value rendering template.</summary>
        [Parameter] public RenderFragment<XFGaugeValueContext>? ValueTemplate { get; set; }

        /// <summary>Composite format string for value (args: {0}=value, {1}=percent).</summary>
        [Parameter] public string? ValueFormat { get; set; }

        /// <summary>Unit suffix appended to value.</summary>
        [Parameter] public string? Unit { get; set; }

        /// <summary>Color range bands rendered on the bar.</summary>
        [Parameter] public IReadOnlyList<XFGaugeRange>? Ranges { get; set; }

        /// <summary>Whether to round the bar ends.</summary>
        [Parameter] public bool Rounded { get; set; } = true;

        /// <summary>Accessibility label for the gauge.</summary>
        [Parameter] public string? AriaLabel { get; set; }

        // assinatura de render: gráfico SVG e caro de repintar — pula render redundante
        // quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Value, Min, Max, Orientation, Reversed, Size, Thickness, Color, TrackColor, ShowValue, ValueFormat, Unit, Ranges?.Count, Rounded, AriaLabel);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        // ─── Derived Properties ───────────────────────────────────────

        private double Range => Math.Max(0.0001, Max - Min);
        private double ClampedValue => Math.Clamp(Value, Min, Max);
        private double Percent => ((ClampedValue - Min) / Range) * 100;

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

        internal bool IsVertical => Orientation is XFOrientation.Vertical;

        // ─── Size Maps ────────────────────────────────────────────────

        private string ContainerSizeClass => (IsVertical, Size) switch
        {
            (true, XFExtendedSize.Xs) => "h-24 w-8",
            (true, XFExtendedSize.Sm) => "h-32 w-10",
            (true, XFExtendedSize.Lg) => "h-64 w-16",
            (true, XFExtendedSize.Xl) => "h-80 w-20",
            (true, _) => "h-48 w-12",
            (false, XFExtendedSize.Xs) => "h-8 w-24",
            (false, XFExtendedSize.Sm) => "h-10 w-32",
            (false, XFExtendedSize.Lg) => "h-16 w-64",
            (false, XFExtendedSize.Xl) => "h-20 w-80",
            (false, _) => "h-12 w-48"
        };

        private string ValueTextSizeClass => Size switch
        {
            XFExtendedSize.Xs => "text-[10px]",
            XFExtendedSize.Sm => "text-xs",
            XFExtendedSize.Lg => "text-base",
            XFExtendedSize.Xl => "text-lg",
            _ => "text-sm"
        };

        // ─── Color Resolution ─────────────────────────────────────────

        private string TrackFill => string.IsNullOrWhiteSpace(TrackColor)
            ? "fill-surface-200 dark:fill-surface-700"
            : TrackColor.StartsWith("fill-") ? TrackColor : "";

        private string TrackFillStyle => string.IsNullOrWhiteSpace(TrackColor) || TrackColor.StartsWith("fill-")
            ? ""
            : $"fill:{TrackColor}";

        private string ValueFill => string.IsNullOrWhiteSpace(Color)
            ? "fill-primary"
            : Color.StartsWith("fill-") ? Color : "";

        private string ValueFillStyle => string.IsNullOrWhiteSpace(Color) || Color.StartsWith("fill-")
            ? ""
            : $"fill:{Color}";

        private string RangeFillClass(string? rangeColor)
        {
            if (string.IsNullOrWhiteSpace(rangeColor)) return "fill-surface-400";
            return rangeColor.StartsWith("fill-") ? rangeColor : "";
        }

        private string RangeFillStyle(string? rangeColor)
        {
            if (string.IsNullOrWhiteSpace(rangeColor) || rangeColor.StartsWith("fill-")) return "";
            return $"fill:{rangeColor}";
        }

        // ─── Bar Geometry ─────────────────────────────────────────────

        internal const double BarPadding = 4;

        // ─── Value Text Position ──────────────────────────────────────

        private string ValueTextPosition => (IsVertical, ValuePosition) switch
        {
            (true, XFGaugeValuePosition.Start) => "absolute top-1 left-1/2 -translate-x-1/2",
            (true, XFGaugeValuePosition.End) => "absolute bottom-1 left-1/2 -translate-x-1/2",
            (true, _) => "absolute inset-0 grid place-items-center",
            (false, XFGaugeValuePosition.Start) => "absolute left-1 top-1/2 -translate-y-1/2",
            (false, XFGaugeValuePosition.End) => "absolute right-1 top-1/2 -translate-y-1/2",
            (false, _) => "absolute inset-0 grid place-items-center"
        };

        // ─── Component Class ──────────────────────────────────────────

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} relative inline-block {ContainerSizeClass}");
        }
    }
}
