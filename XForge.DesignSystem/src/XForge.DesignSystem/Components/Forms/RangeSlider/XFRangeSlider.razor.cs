using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.RangeSlider
{
    public partial class XFRangeSlider : XFInputComponentBase
    {
        [Parameter] public string? StartName { get; set; }
        [Parameter] public string? EndName { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? StartAriaLabel { get; set; } = "Valor inicial";
        [Parameter] public string? EndAriaLabel { get; set; } = "Valor final";
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public decimal? StartValue { get; set; }
        [Parameter] public decimal? EndValue { get; set; }
        [Parameter] public EventCallback<decimal?> StartValueChanged { get; set; }
        [Parameter] public EventCallback<decimal?> EndValueChanged { get; set; }
        [Parameter] public Expression<Func<decimal?>>? StartValueExpression { get; set; }
        [Parameter] public Expression<Func<decimal?>>? EndValueExpression { get; set; }
        [Parameter] public EventCallback<(decimal? Start, decimal? End)> Change { get; set; }
        [Parameter] public decimal Min { get; set; }
        [Parameter] public decimal Max { get; set; } = 100;
        [Parameter] public decimal Step { get; set; } = 1;
        [Parameter] public string? ValueFormat { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public string RangeSeparatorText { get; set; } = " - ";
        [Parameter] public string? MinLabel { get; set; }
        [Parameter] public string? MaxLabel { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; } = true;
        [Parameter] public bool ShowValue { get; set; } = true;
        [Parameter] public bool ShowBounds { get; set; } = true;
        [Parameter] public bool AllowOverlap { get; set; }
        [Parameter] public CultureInfo? Culture { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', StartName, EndName, Label, StartAriaLabel, EndAriaLabel, HelperText, StartValue, EndValue, Min, Max, Step, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private FieldIdentifier? StartFieldIdentifier { get; set; }
        private FieldIdentifier? EndFieldIdentifier { get; set; }
        private string StartId => $"{Id}-start";
        private string EndId => $"{Id}-end";
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private bool SliderDisabled => Disabled || ReadOnly;
        private decimal CurrentStart => Clamp(StartValue ?? Min);
        private decimal CurrentEnd => Clamp(EndValue ?? Max);
        private decimal NormalizedStart => Math.Min(CurrentStart, CurrentEnd);
        private decimal NormalizedEnd => Math.Max(CurrentStart, CurrentEnd);
        private string StartString => NormalizedStart.ToString(CultureInfo.InvariantCulture);
        private string EndString => NormalizedEnd.ToString(CultureInfo.InvariantCulture);
        private string MinString => Min.ToString(CultureInfo.InvariantCulture);
        private string MaxString => Max.ToString(CultureInfo.InvariantCulture);
        private string StartMaxString => (AllowOverlap ? Max : NormalizedEnd).ToString(CultureInfo.InvariantCulture);
        private string EndMinString => (AllowOverlap ? Min : NormalizedStart).ToString(CultureInfo.InvariantCulture);
        private string StepString => Step.ToString(CultureInfo.InvariantCulture);
        private string DisplayStart => FormatValue(NormalizedStart);
        private string DisplayEnd => FormatValue(NormalizedEnd);
        private string DisplayRange => $"{DisplayStart}{RangeSeparatorText}{DisplayEnd}";
        private string DisplayMin => MinLabel ?? FormatValue(Min);
        private string DisplayMax => MaxLabel ?? FormatValue(Max);
        private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;
        private string AriaInvalid => HasValidationError ? "true" : "false";
        private string? DescribedBy => HasValidationError
            ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private decimal RangeSize => Max <= Min ? 1 : Max - Min;
        private decimal StartPercent => ((NormalizedStart - Min) / RangeSize) * 100;
        private decimal EndPercent => ((NormalizedEnd - Min) / RangeSize) * 100;
        private string TrackStyle => string.Create(CultureInfo.InvariantCulture, $"--xf-range-start:{StartPercent:0.###}%; --xf-range-end:{EndPercent:0.###}%;");

        private string? FirstValidationMessage =>
            StartValidationMessage ?? EndValidationMessage;

        private string? StartValidationMessage => HasStartValidationError
            ? CascadedEditContext!.GetValidationMessages(StartFieldIdentifier!.Value).FirstOrDefault()
            : null;

        private string? EndValidationMessage => HasEndValidationError
            ? CascadedEditContext!.GetValidationMessages(EndFieldIdentifier!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError => HasStartValidationError || HasEndValidationError;

        private bool HasStartValidationError =>
            CascadedEditContext is not null
            && StartValueExpression is not null
            && StartFieldIdentifier.HasValue
            && CascadedEditContext.GetValidationMessages(StartFieldIdentifier.Value).Any();

        private bool HasEndValidationError =>
            CascadedEditContext is not null
            && EndValueExpression is not null
            && EndFieldIdentifier.HasValue
            && CascadedEditContext.GetValidationMessages(EndFieldIdentifier.Value).Any();

        private string RootClass => Join("xf-range-slider flex flex-col gap-2", FullWidth ? "w-full" : "inline-flex", Class);
        private string HeaderClass => "flex items-center justify-between gap-4";
        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";
        private string ValueClass => Join("rounded-md px-2 py-0.5 text-xs font-semibold tabular-nums ring-1", SliderDisabled ? MutedValueClass : ToneValueClass);
        private string TrackWrapClass => "relative flex min-h-8 flex-1 items-center py-1";
        private string TrackClass => Join("pointer-events-none absolute left-0 right-0 rounded-full bg-surface-200 before:absolute before:inset-y-0 before:left-[var(--xf-range-start)] before:right-[calc(100%-var(--xf-range-end))] before:rounded-full before:bg-[var(--xf-range-color)]", TrackHeightClass, SliderDisabled ? MutedTrackToneClass : TrackToneClass);
        private string InputBaseClass => Join("pointer-events-none absolute top-1/2 z-10 -translate-y-1/2 cursor-pointer appearance-none bg-transparent outline-none disabled:cursor-not-allowed [&::-moz-range-thumb]:pointer-events-auto [&::-moz-range-thumb]:appearance-none [&::-moz-range-thumb]:rounded-full [&::-moz-range-thumb]:border-0 [&::-moz-range-thumb]:shadow-[0_1px_2px_rgb(15_23_42/.12)] [&::-moz-range-track]:bg-transparent [&::-webkit-slider-runnable-track]:bg-transparent [&::-webkit-slider-thumb]:pointer-events-auto [&::-webkit-slider-thumb]:appearance-none [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:shadow-[0_1px_2px_rgb(15_23_42/.12)] focus-visible:[&::-moz-range-thumb]:ring-4 focus-visible:[&::-webkit-slider-thumb]:ring-4", SizeClass, SliderDisabled ? MutedThumbClass : ToneThumbClass);
        private string StartInputClass => Join(InputBaseClass, AllowOverlap ? "inset-x-0 w-full" : "left-0 w-[var(--xf-range-end)]");
        private string EndInputClass => Join(InputBaseClass, AllowOverlap ? "inset-x-0 w-full" : "left-[var(--xf-range-start)] w-[calc(100%-var(--xf-range-start))]");
        private string BoundsClass => "flex min-w-16 shrink-0 justify-between gap-2 text-xs leading-5 text-text-muted tabular-nums";
        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string TrackHeightClass => Size switch
        {
            XFSize.Sm => "h-1",
            XFSize.Lg => "h-2",
            _ => "h-1.5"
        };

        private string SizeClass => Size switch
        {
            XFSize.Sm => "[&::-webkit-slider-runnable-track]:h-1 [&::-moz-range-track]:h-1 [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:mt-[-6px] [&::-moz-range-thumb]:h-4 [&::-moz-range-thumb]:w-4",
            XFSize.Lg => "[&::-webkit-slider-runnable-track]:h-2 [&::-moz-range-track]:h-2 [&::-webkit-slider-thumb]:h-6 [&::-webkit-slider-thumb]:w-6 [&::-webkit-slider-thumb]:mt-[-8px] [&::-moz-range-thumb]:h-6 [&::-moz-range-thumb]:w-6",
            _ => "[&::-webkit-slider-runnable-track]:h-1.5 [&::-moz-range-track]:h-1.5 [&::-webkit-slider-thumb]:h-5 [&::-webkit-slider-thumb]:w-5 [&::-webkit-slider-thumb]:mt-[-7px] [&::-moz-range-thumb]:h-5 [&::-moz-range-thumb]:w-5"
        };

        private string ToneThumbClass => Tone switch
        {
            XFSemanticColor.Secondary => "[--xf-range-color:var(--color-secondary)] [&::-moz-range-thumb]:bg-[var(--color-secondary)] [&::-webkit-slider-thumb]:bg-[var(--color-secondary)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-secondary)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-secondary)_22%,transparent)]",
            XFSemanticColor.Success => "[--xf-range-color:var(--color-success)] [&::-moz-range-thumb]:bg-[var(--color-success)] [&::-webkit-slider-thumb]:bg-[var(--color-success)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-success)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-success)_22%,transparent)]",
            XFSemanticColor.Warning => "[--xf-range-color:var(--color-warning)] [&::-moz-range-thumb]:bg-[var(--color-warning)] [&::-webkit-slider-thumb]:bg-[var(--color-warning)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-warning)_26%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-warning)_26%,transparent)]",
            XFSemanticColor.Danger => "[--xf-range-color:var(--color-danger)] [&::-moz-range-thumb]:bg-[var(--color-danger)] [&::-webkit-slider-thumb]:bg-[var(--color-danger)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-danger)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-danger)_22%,transparent)]",
            _ => "[--xf-range-color:var(--color-primary)] [&::-moz-range-thumb]:bg-[var(--color-primary)] [&::-webkit-slider-thumb]:bg-[var(--color-primary)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]"
        };

        private string TrackToneClass => Tone switch
        {
            XFSemanticColor.Secondary => "[--xf-range-color:var(--color-secondary)]",
            XFSemanticColor.Success => "[--xf-range-color:var(--color-success)]",
            XFSemanticColor.Warning => "[--xf-range-color:var(--color-warning)]",
            XFSemanticColor.Danger => "[--xf-range-color:var(--color-danger)]",
            _ => "[--xf-range-color:var(--color-primary)]"
        };

        private string MutedThumbClass => "[--xf-range-color:color-mix(in_oklab,var(--color-text-muted)_42%,var(--color-bg-surface))] [&::-moz-range-thumb]:bg-[color-mix(in_oklab,var(--color-text-muted)_42%,var(--color-bg-surface))] [&::-webkit-slider-thumb]:bg-[color-mix(in_oklab,var(--color-text-muted)_42%,var(--color-bg-surface))]";

        private string MutedTrackToneClass => "[--xf-range-color:color-mix(in_oklab,var(--color-text-muted)_38%,var(--color-bg-surface))]";

        private string MutedValueClass => "bg-surface-100 text-text-muted ring-surface-200";

        private string ToneValueClass => Tone switch
        {
            XFSemanticColor.Secondary => "bg-[color-mix(in_oklab,var(--color-secondary)_9%,transparent)] text-secondary ring-[color-mix(in_oklab,var(--color-secondary)_18%,transparent)]",
            XFSemanticColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_10%,transparent)] text-success ring-[color-mix(in_oklab,var(--color-success)_18%,transparent)]",
            XFSemanticColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_12%,transparent)] text-warning ring-[color-mix(in_oklab,var(--color-warning)_22%,transparent)]",
            XFSemanticColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger ring-[color-mix(in_oklab,var(--color-danger)_18%,transparent)]",
            _ => "bg-[color-mix(in_oklab,var(--color-primary)_9%,transparent)] text-primary ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
        };

        protected override void OnParametersSet()
        {
            if (Max < Min)
            {
                Max = Min;
            }

            if (Step <= 0)
            {
                Step = 1;
            }

            if (StartValueExpression is not null)
            {
                StartFieldIdentifier = FieldIdentifier.Create(StartValueExpression);
            }

            if (EndValueExpression is not null)
            {
                EndFieldIdentifier = FieldIdentifier.Create(EndValueExpression);
            }
        }

        private async Task HandleStartInputAsync(ChangeEventArgs args)
        {
            if (Immediate)
            {
                await SetStartFromStringAsync(args.Value?.ToString(), notifyChange: true);
            }
        }

        private async Task HandleEndInputAsync(ChangeEventArgs args)
        {
            if (Immediate)
            {
                await SetEndFromStringAsync(args.Value?.ToString(), notifyChange: true);
            }
        }

        private async Task HandleStartChangeAsync(ChangeEventArgs args) => await SetStartFromStringAsync(args.Value?.ToString(), notifyChange: true);

        private async Task HandleEndChangeAsync(ChangeEventArgs args) => await SetEndFromStringAsync(args.Value?.ToString(), notifyChange: true);

        private async Task SetStartFromStringAsync(string? value, bool notifyChange)
        {
            if (SliderDisabled)
            {
                return;
            }

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
            {
                var nextStart = Clamp(number);
                var nextEnd = NormalizedEnd;
                if (!AllowOverlap && nextStart > nextEnd)
                {
                    nextStart = nextEnd;
                }

                await SetRangeAsync(nextStart, nextEnd, notifyChange, notifyStart: true, notifyEnd: false);
            }
        }

        private async Task SetEndFromStringAsync(string? value, bool notifyChange)
        {
            if (SliderDisabled)
            {
                return;
            }

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
            {
                var nextStart = NormalizedStart;
                var nextEnd = Clamp(number);
                if (!AllowOverlap && nextEnd < nextStart)
                {
                    nextEnd = nextStart;
                }

                await SetRangeAsync(nextStart, nextEnd, notifyChange, notifyStart: false, notifyEnd: true);
            }
        }

        private async Task SetRangeAsync(decimal start, decimal end, bool notifyChange, bool notifyStart, bool notifyEnd)
        {
            StartValue = start;
            EndValue = end;

            if (notifyChange && notifyStart && StartValueChanged.HasDelegate)
            {
                await StartValueChanged.InvokeAsync(StartValue);
            }

            if (notifyChange && notifyEnd && EndValueChanged.HasDelegate)
            {
                await EndValueChanged.InvokeAsync(EndValue);
            }

            if (notifyChange && Change.HasDelegate)
            {
                await Change.InvokeAsync((StartValue, EndValue));
            }

            if (notifyChange && CascadedEditContext is not null)
            {
                if (notifyStart && StartFieldIdentifier.HasValue)
                {
                    CascadedEditContext.NotifyFieldChanged(StartFieldIdentifier.Value);
                }

                if (notifyEnd && EndFieldIdentifier.HasValue)
                {
                    CascadedEditContext.NotifyFieldChanged(EndFieldIdentifier.Value);
                }
            }
        }

        private decimal Clamp(decimal value)
        {
            if (value < Min)
            {
                return Min;
            }

            if (value > Max)
            {
                return Max;
            }

            return value;
        }

        private string FormatValue(decimal value)
        {
            return $"{PrefixText}{value.ToString(ValueFormat ?? "0.##", EffectiveCulture)}{SuffixText}";
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
