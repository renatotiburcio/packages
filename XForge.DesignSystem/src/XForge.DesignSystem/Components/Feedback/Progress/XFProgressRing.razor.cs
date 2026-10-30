using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Feedback.Progress
{
    public partial class XFProgressRing : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-progress-ring";

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(LoadingText)) LoadingText = L("progress.loading", LoadingText);
            base.OnInitialized();
        }

        [Parameter] public double Value { get; set; }
        [Parameter] public double Min { get; set; }
        [Parameter] public double Max { get; set; } = 100;
        [Parameter] public bool Indeterminate { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool ShowValue { get; set; }
        [Parameter] public bool Rounded { get; set; } = true;
        [Parameter] public bool Paused { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string LoadingText { get; set; } = default!;
        [Parameter] public string? Unit { get; set; } = "%";
        [Parameter] public string? ValueFormat { get; set; }
        [Parameter] public XFProgressColor Color { get; set; } = XFProgressColor.Primary;
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Value, Min, Max, Indeterminate, Visible, ShowValue, Rounded, Paused, AriaLabel, LoadingText, Unit, ValueFormat, Color, Size, ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private double Radius => Size is XFExtendedSize.Xs or XFExtendedSize.Sm ? 48 : 46;

        private double StrokeWidth => Size switch
        {
            XFExtendedSize.Xs => 8,
            XFExtendedSize.Sm => 9,
            XFExtendedSize.Lg => 11,
            XFExtendedSize.Xl => 12,
            _ => 10
        };

        private double Range => Math.Max(0.0001, Max - Min);

        private double ClampedValue => Math.Clamp(Value, Min, Max);

        private double Percent => Math.Clamp(((ClampedValue - Min) / Range) * 100, 0, 100);

        private double Circumference => 2 * Math.PI * Radius;

        private double DashOffset => Indeterminate ? Circumference * 0.32 : Circumference * (1 - Percent / 100);

        private string CircumferenceString => Circumference.ToString("0.###", CultureInfo.InvariantCulture);

        private string DashOffsetString => DashOffset.ToString("0.###", CultureInfo.InvariantCulture);

        private string? AriaValueNow => Indeterminate ? null : ClampedValue.ToString("0.###", CultureInfo.InvariantCulture);

        private string? AriaValueText => Indeterminate ? LoadingText : ValueText;

        private string StrokeLineCap => Rounded ? "round" : "butt";

        private string ValueText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ValueFormat))
                {
                    return string.Format(CultureInfo.CurrentCulture, ValueFormat, ClampedValue, Percent);
                }

                if (Unit == "%")
                {
                    return $"{Percent:0}%";
                }

                return $"{ClampedValue:0.##}{Unit}";
            }
        }

        protected override string GetComponentClass()
        {
            var sizeClass = Size switch
            {
                XFExtendedSize.Xs => "h-8 w-8",
                XFExtendedSize.Sm => "h-12 w-12",
                XFExtendedSize.Lg => "h-24 w-24",
                XFExtendedSize.Xl => "h-32 w-32",
                _ => "h-16 w-16"
            };

            return GetClass($"{BaseClass} relative inline-grid shrink-0 place-items-center {sizeClass}");
        }

        private string SvgClass => Indeterminate
            ? Paused
                ? "h-full w-full -rotate-90 motion-safe:xf-progress-ring-animate-paused"
                : "h-full w-full -rotate-90 motion-safe:xf-progress-ring-animate"
            : "h-full w-full -rotate-90";

        private string TrackClass => "stroke-surface-200";

        private string IndicatorClass
        {
            get
            {
                var transition = Indeterminate ? "" : "transition-[stroke-dashoffset] duration-300 ease-out";
                return $"{ToneStrokeClass} {transition}";
            }
        }

        private string CenterClass => Size switch
        {
            XFExtendedSize.Xs => "absolute inset-0 grid place-items-center text-[9px] font-semibold text-text-secondary",
            XFExtendedSize.Sm => "absolute inset-0 grid place-items-center text-[10px] font-semibold text-text-secondary",
            XFExtendedSize.Lg => "absolute inset-0 grid place-items-center text-sm font-semibold text-text-primary",
            XFExtendedSize.Xl => "absolute inset-0 grid place-items-center text-base font-semibold text-text-primary",
            _ => "absolute inset-0 grid place-items-center text-xs font-semibold text-text-primary"
        };

        private string ToneStrokeClass => Color switch
        {
            XFProgressColor.Secondary => "stroke-secondary",
            XFProgressColor.Success => "stroke-success",
            XFProgressColor.Warning => "stroke-warning",
            XFProgressColor.Danger => "stroke-danger",
            XFProgressColor.Neutral => "stroke-surface-700",
            XFProgressColor.Info => "stroke-info",
            XFProgressColor.Light => "stroke-surface-300",
            XFProgressColor.Dark => "stroke-surface-900",
            _ => "stroke-primary"
        };
    }
}
