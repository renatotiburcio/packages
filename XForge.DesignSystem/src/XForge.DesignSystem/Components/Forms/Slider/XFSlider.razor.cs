using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Components.Layout;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.Slider
{
    public partial class XFSlider : XFInputComponentBase
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public decimal? Value { get; set; }
        [Parameter] public EventCallback<decimal?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<decimal?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<decimal?> Change { get; set; }
        [Parameter] public decimal Min { get; set; }
        [Parameter] public decimal Max { get; set; } = 100;
        [Parameter] public decimal Step { get; set; } = 1;
        [Parameter] public string? ValueFormat { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public string? MinLabel { get; set; }
        [Parameter] public string? MaxLabel { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Horizontal;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; } = true;
        [Parameter] public bool ShowValue { get; set; } = true;
        [Parameter] public bool ShowBounds { get; set; } = true;
        [Parameter] public CultureInfo? Culture { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private FieldIdentifier? FieldIdentifierValue { get; set; }
        private decimal CurrentValue => Clamp(Value ?? Min);
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string AriaInvalid => HasValidationError ? "true" : "false";
        private string? DescribedBy => HasValidationError
            ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private bool IsVertical => Orientation == XFOrientation.Vertical;
        private bool SliderDisabled => Disabled || ReadOnly;
        private string ValueString => CurrentValue.ToString(CultureInfo.InvariantCulture);
        private string MinString => Min.ToString(CultureInfo.InvariantCulture);
        private string MaxString => Max.ToString(CultureInfo.InvariantCulture);
        private string StepString => Step.ToString(CultureInfo.InvariantCulture);
        private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;
        private string DisplayValue => FormatDisplayValue(CurrentValue);
        private string DisplayMin => MinLabel ?? FormatDisplayValue(Min);
        private string DisplayMax => MaxLabel ?? FormatDisplayValue(Max);

        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(FieldIdentifierValue!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && ValueExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string RootClass =>
            Join("xf-slider flex flex-col gap-2", FullWidth ? "w-full" : "inline-flex", IsVertical ? "items-start" : null, Class);

        private string HeaderClass =>
            "flex items-center justify-between gap-4";

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string ValueClass =>
            Join(
                "rounded-md px-2 py-0.5 text-xs font-semibold tabular-nums ring-1",
                ToneValueClass);

        private string TrackWrapClass =>
            Join(
                "group flex flex-1",
                IsVertical ? "h-48 w-10 items-center justify-center py-2" : "w-full items-center py-1");

        private string InputClass =>
            Join(
                "xf-slider-input w-full cursor-pointer appearance-none bg-transparent outline-none disabled:cursor-not-allowed [&::-moz-range-thumb]:appearance-none [&::-moz-range-thumb]:rounded-full [&::-moz-range-thumb]:border-0 [&::-moz-range-thumb]:shadow-[0_1px_2px_rgb(15_23_42/.12)] [&::-moz-range-track]:rounded-full [&::-moz-range-track]:bg-surface-200 [&::-webkit-slider-runnable-track]:rounded-full [&::-webkit-slider-runnable-track]:bg-surface-200 [&::-webkit-slider-thumb]:appearance-none [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:shadow-[0_1px_2px_rgb(15_23_42/.12)] focus-visible:[&::-moz-range-thumb]:ring-4 focus-visible:[&::-webkit-slider-thumb]:ring-4",
                IsVertical ? "[writing-mode:vertical-lr] [direction:rtl] h-44 w-3" : null,
                SizeClass,
                ToneClass,
                SliderDisabled ? "opacity-55" : null);

        private string BoundsClass =>
            Join(
                "shrink-0 text-xs leading-5 text-text-muted tabular-nums",
                IsVertical ? "flex h-48 flex-col-reverse justify-between pl-2" : "flex min-w-16 justify-between gap-2");

        private string HelperClass => "text-xs leading-5 text-text-secondary";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => IsVertical
                ? "[&::-webkit-slider-runnable-track]:w-1 [&::-moz-range-track]:w-1 [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:translate-x-[-6px] [&::-moz-range-thumb]:h-4 [&::-moz-range-thumb]:w-4"
                : "[&::-webkit-slider-runnable-track]:h-1 [&::-moz-range-track]:h-1 [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:mt-[-6px] [&::-moz-range-thumb]:h-4 [&::-moz-range-thumb]:w-4",
            XFSize.Lg => IsVertical
                ? "[&::-webkit-slider-runnable-track]:w-2 [&::-moz-range-track]:w-2 [&::-webkit-slider-thumb]:h-6 [&::-webkit-slider-thumb]:w-6 [&::-webkit-slider-thumb]:translate-x-[-8px] [&::-moz-range-thumb]:h-6 [&::-moz-range-thumb]:w-6"
                : "[&::-webkit-slider-runnable-track]:h-2 [&::-moz-range-track]:h-2 [&::-webkit-slider-thumb]:h-6 [&::-webkit-slider-thumb]:w-6 [&::-webkit-slider-thumb]:mt-[-8px] [&::-moz-range-thumb]:h-6 [&::-moz-range-thumb]:w-6",
            _ => IsVertical
                ? "[&::-webkit-slider-runnable-track]:w-1.5 [&::-moz-range-track]:w-1.5 [&::-webkit-slider-thumb]:h-5 [&::-webkit-slider-thumb]:w-5 [&::-webkit-slider-thumb]:translate-x-[-7px] [&::-moz-range-thumb]:h-5 [&::-moz-range-thumb]:w-5"
                : "[&::-webkit-slider-runnable-track]:h-1.5 [&::-moz-range-track]:h-1.5 [&::-webkit-slider-thumb]:h-5 [&::-webkit-slider-thumb]:w-5 [&::-webkit-slider-thumb]:mt-[-7px] [&::-moz-range-thumb]:h-5 [&::-moz-range-thumb]:w-5"
        };

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Secondary => "[&::-moz-range-thumb]:bg-[var(--color-secondary)] [&::-webkit-slider-thumb]:bg-[var(--color-secondary)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-secondary)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-secondary)_22%,transparent)]",
            XFSemanticColor.Success => "[&::-moz-range-thumb]:bg-[var(--color-success)] [&::-webkit-slider-thumb]:bg-[var(--color-success)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-success)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-success)_22%,transparent)]",
            XFSemanticColor.Warning => "[&::-moz-range-thumb]:bg-[var(--color-warning)] [&::-webkit-slider-thumb]:bg-[var(--color-warning)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-warning)_26%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-warning)_26%,transparent)]",
            XFSemanticColor.Danger => "[&::-moz-range-thumb]:bg-[var(--color-danger)] [&::-webkit-slider-thumb]:bg-[var(--color-danger)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-danger)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-danger)_22%,transparent)]",
            _ => "[&::-moz-range-thumb]:bg-[var(--color-primary)] [&::-webkit-slider-thumb]:bg-[var(--color-primary)] focus-visible:[&::-moz-range-thumb]:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)] focus-visible:[&::-webkit-slider-thumb]:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]"
        };

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

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            if (Immediate)
            {
                await SetValueFromStringAsync(args.Value?.ToString(), notifyChange: true);
            }
        }

        private async Task HandleChangeAsync(ChangeEventArgs args)
        {
            await SetValueFromStringAsync(args.Value?.ToString(), notifyChange: true);
        }

        private async Task SetValueFromStringAsync(string? value, bool notifyChange)
        {
            if (SliderDisabled)
            {
                return;
            }

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
            {
                await SetValueAsync(Clamp(number), notifyChange);
            }
        }

        private async Task SetValueAsync(decimal value, bool notifyChange)
        {
            Value = value;

            if (notifyChange && ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (notifyChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(Value);
            }

            if (notifyChange && CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
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

        private string FormatDisplayValue(decimal value)
        {
            return $"{PrefixText}{value.ToString(ValueFormat ?? "0.##", EffectiveCulture)}{SuffixText}";
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
