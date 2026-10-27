using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.TimeInput
{
    public partial class XFTimeInput : XFInputComponentBase
    {
        private const string MinuteFormat = "HH:mm";
        private const string SecondFormat = "HH:mm:ss";

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public TimeOnly? Value { get; set; }
        [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<TimeOnly?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<TimeOnly?> Change { get; set; }
        [Parameter] public TimeOnly? Min { get; set; }
        [Parameter] public TimeOnly? Max { get; set; }
        [Parameter] public int? Step { get; set; }
        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
        [Parameter] public string? CultureName { get; set; }
        [Parameter] public XFTimeInputPrecision Precision { get; set; } = XFTimeInputPrecision.Minute;
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public bool Clamp { get; set; } = true;
        [Parameter] public string? AriaValueTextFormat { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private TimeOnly? CurrentValue { get; set; }
        private TimeOnly? LastParameterValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string AriaInvalid => HasValidationError ? "true" : "false";
        private string CurrentValueAsString => FormatNativeTime(CurrentValue);
        private string? AriaValueText => FormatAriaValueText(CurrentValue);
        private string? MinString => Min is null ? null : FormatNativeTime(Min);
        private string? MaxString => Max is null ? null : FormatNativeTime(Max);
        private string StepString => (Step ?? (Precision == XFTimeInputPrecision.Second ? 1 : 60)).ToString(CultureInfo.InvariantCulture);
        private string NativeFormat => Precision == XFTimeInputPrecision.Second ? SecondFormat : MinuteFormat;

        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(FieldIdentifierValue!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && ValueExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string? DescribedBy => HasValidationError
            ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string RootClass =>
            Join("xf-time-input flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string FieldShellClass =>
            Join(
                "group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out",
                SizeClass,
                VariantClass,
                ToneClass,
                StateClass);

        private string InputClass =>
            Join(
                "min-w-0 flex-1 border-0 bg-transparent p-0 text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted",
                "scheme-light dark:scheme-dark [&::-webkit-calendar-picker-indicator]:cursor-pointer [&::-webkit-calendar-picker-indicator]:opacity-70 [&::-webkit-calendar-picker-indicator]:transition-opacity hover:[&::-webkit-calendar-picker-indicator]:opacity-100",
                SizeTextClass);

        private string AdornmentClass =>
            "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string HelperClass => "text-xs leading-5 text-text-secondary";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "h-9 py-1.5 pl-3 pr-3",
            XFSize.Lg => "h-12 py-2.5 pl-4 pr-4",
            _ => "h-10 py-2 pl-3.5 pr-3.5"
        };

        private string SizeTextClass => Size switch
        {
            XFSize.Sm => "text-sm",
            XFSize.Lg => "text-base",
            _ => "text-sm"
        };

        private string VariantClass => Variant switch
        {
            XFFormVariant.Filled => "border-transparent bg-surface-100 ring-1 ring-surface-200/70 shadow-[inset_0_1px_0_rgb(255_255_255/.55)]",
            XFFormVariant.Soft => "border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)] bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] ring-1 ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]",
            XFFormVariant.Ghost => "border-transparent bg-transparent ring-1 ring-transparent hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]",
            _ => "border-surface-200 bg-bg-surface ring-1 ring-surface-200/80 shadow-[0_1px_2px_rgb(15_23_42/.035)]"
        };

        private string ToneClass
        {
            get
            {
                if (HasValidationError)
                {
                    return "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] ring-2 ring-[color-mix(in_oklab,var(--color-danger)_15%,transparent)] focus-within:border-[var(--color-danger)] focus-within:ring-[color-mix(in_oklab,var(--color-danger)_25%,transparent)]";
                }

                return Tone switch
                {
                    XFSemanticColor.Primary => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
                    XFSemanticColor.Secondary => "focus-within:border-[var(--color-secondary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]",
                    XFSemanticColor.Success => "focus-within:border-[var(--color-success)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]",
                    XFSemanticColor.Warning => "focus-within:border-[var(--color-warning)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]",
                    XFSemanticColor.Danger => "focus-within:border-[var(--color-danger)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]",
                    _ => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
                };
            }
        }

        private string StateClass => Disabled
            ? "cursor-not-allowed opacity-60"
            : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]";

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrWhiteSpace(CultureName))
            {
                Culture = CultureInfo.GetCultureInfo(CultureName);
            }

            if (Value != LastParameterValue)
            {
                CurrentValue = Value;
                LastParameterValue = Value;
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

        private Task HandleChangeAsync(ChangeEventArgs args)
        {
            return SetValueFromStringAsync(args.Value?.ToString(), notifyChange: true);
        }

        private async Task SetValueFromStringAsync(string? value, bool notifyChange)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await SetValueAsync(null, notifyChange);
                return;
            }

            if (TimeOnly.TryParseExact(value, new[] { MinuteFormat, SecondFormat }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var nativeTime)
                || TimeOnly.TryParse(value, Culture, DateTimeStyles.None, out nativeTime))
            {
                await SetValueAsync(NormalizeTime(nativeTime), notifyChange);
            }
        }

        private async Task SetValueAsync(TimeOnly? value, bool notifyChange)
        {
            CurrentValue = value;
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

        private TimeOnly NormalizeTime(TimeOnly value)
        {
            if (!Clamp)
            {
                return value;
            }

            if (Min is not null && value < Min.Value)
            {
                return Min.Value;
            }

            if (Max is not null && value > Max.Value)
            {
                return Max.Value;
            }

            return value;
        }

        private string FormatNativeTime(TimeOnly? value)
        {
            return value is null
                ? string.Empty
                : value.Value.ToString(NativeFormat, CultureInfo.InvariantCulture);
        }

        private string? FormatAriaValueText(TimeOnly? value)
        {
            if (value is null)
            {
                return null;
            }

            var format = Precision == XFTimeInputPrecision.Second
                ? Culture.DateTimeFormat.LongTimePattern
                : Culture.DateTimeFormat.ShortTimePattern;
            var displayValue = value.Value.ToString(format, Culture);

            return string.IsNullOrWhiteSpace(AriaValueTextFormat)
                ? displayValue
                : string.Format(Culture, AriaValueTextFormat, displayValue);
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
