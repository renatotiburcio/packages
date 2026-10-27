using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Feedback.Progress
{
    public partial class XFProgress : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-progress";

        protected override void OnInitialized()
        {
            LoadingText = L("progress.loading", LoadingText);
            base.OnInitialized();
        }

        [Parameter] public double Value { get; set; }
        [Parameter] public double Min { get; set; }
        [Parameter] public double Max { get; set; } = 100;
        [Parameter] public bool Indeterminate { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool ShowValue { get; set; }
        [Parameter] public bool InlineValue { get; set; }
        [Parameter] public bool Rounded { get; set; } = true;
        [Parameter] public bool Striped { get; set; }
        [Parameter] public bool Animated { get; set; }
        [Parameter] public bool Paused { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string LoadingText { get; set; } = "Carregando";
        [Parameter] public string? Unit { get; set; } = "%";
        [Parameter] public string? ValueFormat { get; set; }
        [Parameter] public XFProgressColor Color { get; set; } = XFProgressColor.Primary;
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;
        [Parameter] public XFProgressVariant Variant { get; set; } = XFProgressVariant.Solid;
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private double Range => Math.Max(0.0001, Max - Min);

        private double ClampedValue => Math.Clamp(Value, Min, Max);

        private double Percent => Math.Clamp(((ClampedValue - Min) / Range) * 100, 0, 100);

        private string PercentString => Percent.ToString("0.###", CultureInfo.InvariantCulture);

        private string? AriaValueNow => Indeterminate ? null : ClampedValue.ToString("0.###", CultureInfo.InvariantCulture);

        private string? AriaValueText => Indeterminate ? LoadingText : ValueText;

        private bool ShowHeader => !string.IsNullOrWhiteSpace(Label);

        private bool ShowInlineValue => InlineValue && (ShowValue || ChildContent is not null) && Size is XFExtendedSize.Lg or XFExtendedSize.Xl;

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
            return GetClass($"{BaseClass} grid gap-2 text-text-primary");
        }

        private string HeaderClass => "flex items-center justify-between gap-3 text-sm font-medium";

        private string TrackClass
        {
            get
            {
                var height = Size switch
                {
                    XFExtendedSize.Xs => "h-1",
                    XFExtendedSize.Sm => "h-1.5",
                    XFExtendedSize.Lg => "h-4",
                    XFExtendedSize.Xl => "h-6",
                    _ => "h-2.5"
                };

                var radius = Rounded ? "rounded-full" : "rounded";
                var background = Variant switch
                {
                    XFProgressVariant.Soft => "bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]",
                    XFProgressVariant.Subtle => "bg-surface-100",
                    _ => "bg-surface-200"
                };

                return $"relative w-full overflow-hidden {radius} {height} {background}";
            }
        }

        private string BarClass
        {
            get
            {
                var radius = Rounded ? "rounded-full" : "rounded";
                var stripes = Striped ? "bg-[length:1rem_1rem] bg-[linear-gradient(45deg,rgb(255_255_255/.22)_25%,transparent_25%,transparent_50%,rgb(255_255_255/.22)_50%,rgb(255_255_255/.22)_75%,transparent_75%,transparent)]" : "";
                var animated = Animated && Striped
                    ? (Paused ? "motion-safe:xf-progress-stripe-animate-paused" : "motion-safe:xf-progress-stripe-animate")
                    : "";

                return $"flex h-full items-center justify-center overflow-hidden transition-[width] duration-300 ease-out {radius} {ToneClass} {stripes} {animated}";
            }
        }

        private string IndeterminateBarClass
        {
            get
            {
                var radius = Rounded ? "rounded-full" : "rounded";
                var animated = Paused
                    ? "motion-safe:xf-progress-indeterminate-animate-paused"
                    : "motion-safe:xf-progress-indeterminate-animate";
                return $"absolute inset-y-0 w-1/3 {radius} {ToneClass} {animated}";
            }
        }

        private string BarStyle => $"width: {PercentString}%;";

        private string InlineLabelClass => Size == XFExtendedSize.Xl
            ? "px-2 text-xs font-semibold text-white"
            : "px-2 text-[11px] font-semibold text-white";

        private string FooterValueClass => "text-right text-xs font-medium text-text-secondary";

        private string ToneClass => Color switch
        {
            XFProgressColor.Secondary => "bg-secondary text-secondary-contrast",
            XFProgressColor.Success => "bg-success text-white",
            XFProgressColor.Warning => "bg-warning text-surface-950",
            XFProgressColor.Danger => "bg-danger text-white",
            XFProgressColor.Neutral => "bg-surface-700 text-white",
            XFProgressColor.Info => "bg-info text-white",
            XFProgressColor.Light => "bg-surface-200 text-surface-900",
            XFProgressColor.Dark => "bg-surface-900 text-white",
            _ => "bg-primary text-primary-contrast"
        };
    }
}
