using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Threading;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.NumberInput
{
    public partial class XFNumberInput<TValue> : XFInputComponentBase
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public TValue? Value { get; set; }
        [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<TValue?> Change { get; set; }
        [Parameter] public decimal? Min { get; set; }
        [Parameter] public decimal? Max { get; set; }
        [Parameter] public decimal? Step { get; set; }
        [Parameter] public string? Format { get; set; }
        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
        [Parameter] public string? CultureName { get; set; }
        [Parameter] public string? CurrencySymbol { get; set; }
        [Parameter] public int? Scale { get; set; }
        [Parameter] public int? Precision { get; set; }
        [Parameter] public bool AutoDecimalSeparator { get; set; }
        [Parameter] public XFNumberInputFormatStyle FormatStyle { get; set; } = XFNumberInputFormatStyle.Number;
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFNumberInputTextAlign TextAlign { get; set; } = XFNumberInputTextAlign.Right;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public bool Clamp { get; set; } = true;
        [Parameter] public bool ShowSteppers { get; set; } = true;
        [Parameter] public XFStepperLayout StepperLayout { get; set; } = XFStepperLayout.Stacked;
        [Parameter] public string IncrementLabel { get; set; } = "Increase value";
        [Parameter] public string DecrementLabel { get; set; } = "Decrease value";
        [Parameter] public string? AriaValueTextFormat { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private TValue? CurrentValue { get; set; }
        private TValue? LastParameterValue { get; set; }
        private string? RawValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        // --- Press-and-hold stepper ---
        private Timer? _repeatTimer;
        private int _repeatDirection;
        private int _repeatElapsedMs;
        private bool _pointerDownFired;
        private const int InitialRepeatDelay = 500;
        private const int FastRepeatThreshold = 2000;
        private const int SlowRepeatInterval = 200;
        private const int FastRepeatInterval = 60;

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string AriaInvalid => HasValidationError ? "true" : "false";
        private string CurrentValueAsString => RawValue ?? FormatValue(CurrentValue);
        private string? AriaValueText => FormatAriaValueText(CurrentValue);
        private string? MinString => Min?.ToString(CultureInfo.InvariantCulture);
        private string? MaxString => Max?.ToString(CultureInfo.InvariantCulture);
        private string StepString => EffectiveStep.ToString(CultureInfo.InvariantCulture);
        private bool StepperDisabled => Disabled || ReadOnly;
        private string InputMode => IsIntegerType || AutoDecimalSeparator ? "numeric" : "decimal";
        private decimal EffectiveStep => Step ?? GetStepFromScale();
        private string? ResolvedPrefixText => PrefixText ?? (FormatStyle == XFNumberInputFormatStyle.Currency ? CurrencySymbol ?? GetDisplayCulture().NumberFormat.CurrencySymbol : null);
        private string? ResolvedSuffixText => SuffixText ?? (FormatStyle == XFNumberInputFormatStyle.Percent ? "%" : null);

        // --- Stepper layout ---
        private bool IsSplitLayout => StepperLayout == XFStepperLayout.Split;
        private bool IsAutoLayout => StepperLayout == XFStepperLayout.Auto;
        private bool IsStackedLayout => StepperLayout == XFStepperLayout.Stacked;

        /// <summary>True when split-style buttons should be rendered (Split always, Auto for responsive toggle).</summary>
        private bool ShowSplitButtons => IsSplitLayout || IsAutoLayout;

        /// <summary>True when stacked-style buttons should be rendered (Stacked always, Auto for responsive toggle).</summary>
        private bool ShowStackedButtons => IsStackedLayout || IsAutoLayout;

        private bool IsIntegerType
        {
            get
            {
                var type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
                return type == typeof(byte)
                    || type == typeof(sbyte)
                    || type == typeof(short)
                    || type == typeof(ushort)
                    || type == typeof(int)
                    || type == typeof(uint)
                    || type == typeof(long)
                    || type == typeof(ulong);
            }
        }

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
            Join("xf-number-input flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string FieldShellClass =>
            Join(
                "group relative flex items-center rounded-lg border transition-all duration-200 ease-out",
                FieldShellGapClass,
                SizeClass,
                VariantClass,
                ToneClass,
                StateClass);

        /// <summary>Gap between flex children in the field shell. All layouts use the same premium gap.</summary>
        private string FieldShellGapClass => "gap-2";

        private string InputClass =>
            Join(
                "min-w-0 flex-1 border-0 bg-transparent p-0 text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none",
                TextAlignClass,
                SizeTextClass);

        private string AdornmentClass =>
            "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string StepperWrapClass => IsAutoLayout
            ? "hidden md:flex h-full shrink-0 items-center gap-1"
            : "flex h-full shrink-0 items-center gap-1";

        private string StepperButtonClass =>
            "inline-flex h-6 w-6 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95 disabled:pointer-events-none disabled:text-text-muted";

        /// <summary>Hidden on desktop when Auto layout is active (stacked buttons shown instead).</summary>
        private string? SplitAutoClass => IsAutoLayout ? "md:hidden" : null;

        private string HelperClass => "text-xs leading-5 text-text-secondary";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "h-9 py-1.5 pl-3 pr-1.5",
            XFSize.Lg => "h-12 py-2.5 pl-4 pr-2",
            _ => "h-10 py-2 pl-3.5 pr-1.5"
        };

        private string SizeTextClass => Size switch
        {
            XFSize.Sm => "text-sm",
            XFSize.Lg => "text-base",
            _ => "text-sm"
        };

        private string TextAlignClass => TextAlign switch
        {
            XFNumberInputTextAlign.Left => "text-left",
            XFNumberInputTextAlign.Center => "text-center",
            _ => "text-right"
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

            if (!EqualityComparer<TValue?>.Default.Equals(Value, LastParameterValue))
            {
                CurrentValue = Value;
                LastParameterValue = Value;
                RawValue = null;
            }

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            RawValue = args.Value?.ToString();
            if (Immediate)
            {
                await SetValueFromStringAsync(RawValue, notifyChange: true);
            }
        }

        private async Task HandleChangeAsync(ChangeEventArgs args)
        {
            await SetValueFromStringAsync(args.Value?.ToString(), notifyChange: true);
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "ArrowUp")
            {
                await StepAsync(1);
            }
            else if (args.Key == "ArrowDown")
            {
                await StepAsync(-1);
            }
        }

        private Task IncrementAsync() => StepAsync(1);

        private Task DecrementAsync() => StepAsync(-1);

        private async Task HandleStepperPointerDown(int direction)
        {
            if (Disabled || ReadOnly) return;
            if (_pointerDownFired) return;
            _pointerDownFired = true;

            StopRepeatTimer();

            _repeatDirection = direction;
            _repeatElapsedMs = 0;

            // First step immediately on press
            await StepAsync(direction);

            // Start single-shot timer for repeat after initial delay
            _repeatTimer = new Timer(async _ =>
            {
                _repeatElapsedMs += InitialRepeatDelay;
                var interval = _repeatElapsedMs >= FastRepeatThreshold
                    ? FastRepeatInterval
                    : SlowRepeatInterval;
                _repeatElapsedMs += interval;

                await InvokeAsync(async () =>
                {
                    await StepAsync(_repeatDirection);
                    StateHasChanged();
                });

                // Reschedule with potentially faster interval
                _repeatTimer?.Change(interval, Timeout.Infinite);
            }, null, InitialRepeatDelay, Timeout.Infinite);
        }

        private void HandleStepperPointerUp()
        {
            _pointerDownFired = false;
            StopRepeatTimer();
        }

        private void HandleStepperPointerLeave()
        {
            _pointerDownFired = false;
            StopRepeatTimer();
        }

        private void StopRepeatTimer()
        {
            _repeatTimer?.Dispose();
            _repeatTimer = null;
        }

        private async Task StepAsync(int direction)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }

            var current = CurrentValue is null ? 0 : Convert.ToDecimal(CurrentValue, CultureInfo.InvariantCulture);
            await SetValueFromDecimalAsync(current + (EffectiveStep * direction));
        }

        private async Task SetValueFromStringAsync(string? value, bool notifyChange)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await SetValueAsync(default, notifyChange);
                return;
            }

            if (AutoDecimalSeparator && TryParseMinorUnitValue(value, out var minorUnitNumber))
            {
                await SetValueFromDecimalAsync(minorUnitNumber, notifyChange);
            }
            else if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number)
                || decimal.TryParse(value, NumberStyles.Number, Culture, out number))
            {
                await SetValueFromDecimalAsync(number, notifyChange);
            }
        }

        private Task SetValueFromDecimalAsync(decimal value)
        {
            return SetValueFromDecimalAsync(value, notifyChange: true);
        }

        private async Task SetValueFromDecimalAsync(decimal value, bool notifyChange)
        {
            var normalized = NormalizeValue(value);
            var converted = ConvertFromDecimal(normalized);
            await SetValueAsync(converted, notifyChange);
        }

        private async Task SetValueAsync(TValue? value, bool notifyChange)
        {
            CurrentValue = value;
            Value = value;
            RawValue = null;

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

        private decimal NormalizeValue(decimal value)
        {
            value = ApplyScale(value);
            value = ApplyPrecisionLimit(value);

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

        private decimal ApplyScale(decimal value)
        {
            if (Scale is null)
            {
                return value;
            }

            return Math.Round(value, Scale.Value, MidpointRounding.AwayFromZero);
        }

        private bool TryParseMinorUnitValue(string value, out decimal result)
        {
            result = 0;
            var isNegative = value.TrimStart().StartsWith("-", StringComparison.Ordinal);
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (digits.Length == 0 || !decimal.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var integerValue))
            {
                return false;
            }

            var scale = Scale ?? 2;
            var divisor = PowerOfTen(Math.Max(scale, 0));
            result = divisor == 0 ? integerValue : integerValue / divisor;

            if (isNegative)
            {
                result *= -1;
            }

            return true;
        }

        private decimal ApplyPrecisionLimit(decimal value)
        {
            if (Precision is null)
            {
                return value;
            }

            var scale = Scale ?? 0;
            var integerDigits = Precision.Value - scale;

            if (Precision.Value <= 0 || integerDigits <= 0)
            {
                return value;
            }

            var maxAbs = PowerOfTen(integerDigits) - (scale > 0 ? 1 / PowerOfTen(scale) : 1);
            if (maxAbs <= 0)
            {
                return value;
            }

            if (value > maxAbs)
            {
                return maxAbs;
            }

            if (value < -maxAbs)
            {
                return -maxAbs;
            }

            return value;
        }

        private TValue? ConvertFromDecimal(decimal value)
        {
            var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            return (TValue?)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        private string FormatValue(TValue? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            if (value is IFormattable formattable)
            {
                var format = Format ?? GetDefaultFormat();
                return formattable.ToString(format, CultureInfo.InvariantCulture);
            }

            return value.ToString() ?? string.Empty;
        }

        private string? FormatAriaValueText(TValue? value)
        {
            if (value is null || value is not IFormattable formattable)
            {
                return null;
            }

            var culture = GetDisplayCulture();
            var format = Format ?? GetDefaultFormat();
            var displayValue = formattable.ToString(format, culture);

            return string.IsNullOrWhiteSpace(AriaValueTextFormat)
                ? displayValue
                : string.Format(culture, AriaValueTextFormat, displayValue);
        }

        private CultureInfo GetDisplayCulture()
        {
            if (string.IsNullOrWhiteSpace(CurrencySymbol))
            {
                return Culture;
            }

            var clone = (CultureInfo)Culture.Clone();
            clone.NumberFormat.CurrencySymbol = CurrencySymbol;
            return clone;
        }

        private string GetDefaultFormat()
        {
            var decimals = Scale ?? (FormatStyle == XFNumberInputFormatStyle.Number ? (int?)null : 2);

            return decimals is null
                ? "0.#############################"
                : $"0.{new string('0', decimals.Value)}";
        }

        private decimal GetStepFromScale()
        {
            if (Scale is null || Scale.Value <= 0)
            {
                return 1;
            }

            return 1 / PowerOfTen(Scale.Value);
        }

        private static decimal PowerOfTen(int exponent)
        {
            var value = 1m;
            for (var index = 0; index < exponent; index++)
            {
                value *= 10;
            }

            return value;
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        public override async ValueTask DisposeAsync()
        {
            StopRepeatTimer();
            await base.DisposeAsync();
        }
    }
}
