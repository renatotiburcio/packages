using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.TimeSpan
{
    public partial class XFTimeSpan : XFInputComponentBase
    {
        private ElementReference _daysRef;
        private ElementReference _hoursRef;
        private ElementReference _minutesRef;
        private ElementReference _secondsRef;

        private global::System.TimeSpan? _currentValue;
        private global::System.TimeSpan? _lastParameterValue;
        private FieldIdentifier? _fieldIdentifier;

        /// <summary>Name attribute for form submission.</summary>
        [Parameter] public string? Name { get; set; }

        /// <summary>Label displayed above the input.</summary>
        [Parameter] public string? Label { get; set; }

        /// <summary>Accessible label for the input group.</summary>
        [Parameter] public string? AriaLabel { get; set; } = "Duração";

        /// <summary>Helper text displayed below the input.</summary>
        [Parameter] public string? HelperText { get; set; }

        /// <summary>Icon displayed before the input segments.</summary>
        [Parameter] public RenderFragment? StartIcon { get; set; }

        /// <summary>The TimeSpan value (two-way bindable).</summary>
        [Parameter] public global::System.TimeSpan? Value { get; set; }

        /// <summary>Event callback when Value changes.</summary>
        [Parameter] public EventCallback<global::System.TimeSpan?> ValueChanged { get; set; }

        /// <summary>Expression for EditForm validation.</summary>
        [Parameter] public Expression<Func<global::System.TimeSpan?>>? ValueExpression { get; set; }

        /// <summary>Event callback when value changes (fires after ValueChanged).</summary>
        [Parameter] public EventCallback<global::System.TimeSpan?> Change { get; set; }

        /// <summary>Minimum allowed duration.</summary>
        [Parameter] public global::System.TimeSpan? Min { get; set; }

        /// <summary>Maximum allowed duration.</summary>
        [Parameter] public global::System.TimeSpan? Max { get; set; }

        /// <summary>Step increment for ArrowUp/Down in seconds. Default: 1.</summary>
        [Parameter] public int Step { get; set; } = 1;

        /// <summary>Precision of the input (Minute or Second).</summary>
        [Parameter] public XFTimeSpanPrecision Precision { get; set; } = XFTimeSpanPrecision.Second;

        /// <summary>Visual variant of the input shell.</summary>
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;

        /// <summary>Size of the input.</summary>
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;

        /// <summary>Color tone for focus state.</summary>
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;

        /// <summary>Whether to show day segment.</summary>
        [Parameter] public bool ShowDays { get; set; }

        /// <summary>Whether to show increment/decrement stepper buttons.</summary>
        [Parameter] public bool ShowSteppers { get; set; } = true;

        /// <summary>Whether the input is read-only.</summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>Whether the input is required.</summary>
        [Parameter] public bool Required { get; set; }

        /// <summary>Whether the input takes full width.</summary>
        [Parameter] public bool FullWidth { get; set; } = true;

        /// <summary>Whether to clamp value within Min/Max range.</summary>
        [Parameter] public bool Clamp { get; set; } = true;

        /// <summary>Accessible label for days segment.</summary>
        [Parameter] public string DaysLabel { get; set; } = "Dias";

        /// <summary>Accessible label for hours segment.</summary>
        [Parameter] public string HoursLabel { get; set; } = "Horas";

        /// <summary>Accessible label for minutes segment.</summary>
        [Parameter] public string MinutesLabel { get; set; } = "Minutos";

        /// <summary>Accessible label for seconds segment.</summary>
        [Parameter] public string SecondsLabel { get; set; } = "Segundos";

        /// <summary>Accessible label for increment button.</summary>
        [Parameter] public string IncrementLabel { get; set; } = "Aumentar";

        /// <summary>Accessible label for decrement button.</summary>
        [Parameter] public string DecrementLabel { get; set; } = "Diminuir";

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        // Computed segment values
        private string DaysValue => (_currentValue?.Days ?? 0).ToString();
        private string HoursValue => (_currentValue?.Hours ?? 0).ToString();
        private string MinutesValue => (_currentValue?.Minutes ?? 0).ToString();
        private string SecondsValue => (_currentValue?.Seconds ?? 0).ToString();
        private int MaxDays => Max?.Days ?? 999;
        private bool StepperDisabled => Disabled || ReadOnly;

        // Validation
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(_fieldIdentifier!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && ValueExpression is not null
            && _fieldIdentifier.HasValue
            && CascadedEditContext.GetValidationMessages(_fieldIdentifier.Value).Any();

        // CSS Classes
        private string RootClass => Join("xf-time-span flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string FieldShellClass => Join(
            "group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out",
            SizeClass, VariantClass, ToneClass, StateClass);

        private string SegmentClass => Join(
            "w-10 border-0 bg-transparent text-center text-text-primary outline-none placeholder:text-text-muted",
            "disabled:cursor-not-allowed disabled:text-text-muted",
            "[appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none",
            SizeTextClass);

        private string SeparatorClass => "flex items-center text-text-muted font-medium select-none";

        private string AdornmentClass => "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string StepperWrapClass => "flex h-full shrink-0 items-center gap-1";

        private string StepperButtonClass => "inline-flex h-6 w-6 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95 disabled:pointer-events-none disabled:text-text-muted";

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
            if (Value != _lastParameterValue)
            {
                _currentValue = Value;
                _lastParameterValue = Value;
            }

            if (ValueExpression is not null)
            {
                _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
            }
        }

        private async Task HandleSegmentInputAsync(ChangeEventArgs args, TimeSpanSegment segment)
        {
            if (int.TryParse(args.Value?.ToString(), out var intValue))
            {
                var newValue = ApplySegment(_currentValue ?? global::System.TimeSpan.Zero, segment, intValue);
                await SetValueAsync(newValue, notifyChange: false);
            }
        }

        private async Task HandleSegmentChangeAsync(ChangeEventArgs args, TimeSpanSegment segment)
        {
            if (int.TryParse(args.Value?.ToString(), out var intValue))
            {
                var clampedValue = ClampSegmentValue(segment, intValue);
                var newValue = ApplySegment(_currentValue ?? global::System.TimeSpan.Zero, segment, clampedValue);
                await SetValueAsync(newValue, notifyChange: true);
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args, TimeSpanSegment segment)
        {
            if (ReadOnly || Disabled) return;

            if (args.Key == "ArrowUp")
            {
                var current = _currentValue ?? global::System.TimeSpan.Zero;
                var delta = GetSegmentDelta(segment);
                var newValue = current.Add(delta);
                await SetValueAsync(newValue, notifyChange: true);
            }
            else if (args.Key == "ArrowDown")
            {
                var current = _currentValue ?? global::System.TimeSpan.Zero;
                var delta = GetSegmentDelta(segment);
                var newValue = current.Subtract(delta);
                if (newValue < global::System.TimeSpan.Zero) newValue = global::System.TimeSpan.Zero;
                await SetValueAsync(newValue, notifyChange: true);
            }
        }

        private async Task IncrementAsync()
        {
            if (ReadOnly || Disabled) return;
            var current = _currentValue ?? global::System.TimeSpan.Zero;
            await SetValueAsync(current.Add(global::System.TimeSpan.FromSeconds(Step)), notifyChange: true);
        }

        private async Task DecrementAsync()
        {
            if (ReadOnly || Disabled) return;
            var current = _currentValue ?? global::System.TimeSpan.Zero;
            var newValue = current.Subtract(global::System.TimeSpan.FromSeconds(Step));
            if (newValue < global::System.TimeSpan.Zero) newValue = global::System.TimeSpan.Zero;
            await SetValueAsync(newValue, notifyChange: true);
        }

        private async Task SetValueAsync(global::System.TimeSpan? value, bool notifyChange)
        {
            if (Clamp && value.HasValue)
            {
                if (Min.HasValue && value < Min.Value) value = Min.Value;
                if (Max.HasValue && value > Max.Value) value = Max.Value;
            }

            // Prevent negative
            if (value.HasValue && value.Value < global::System.TimeSpan.Zero)
            {
                value = global::System.TimeSpan.Zero;
            }

            _currentValue = value;
            Value = value;

            if (notifyChange && ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (notifyChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(Value);
            }

            if (notifyChange && CascadedEditContext is not null && _fieldIdentifier.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(_fieldIdentifier.Value);
            }
        }

        private global::System.TimeSpan ApplySegment(global::System.TimeSpan current, TimeSpanSegment segment, int value)
        {
            return segment switch
            {
                TimeSpanSegment.Days => new global::System.TimeSpan(value, current.Hours, current.Minutes, current.Seconds),
                TimeSpanSegment.Hours => new global::System.TimeSpan(current.Days, value, current.Minutes, current.Seconds),
                TimeSpanSegment.Minutes => new global::System.TimeSpan(current.Days, current.Hours, value, current.Seconds),
                TimeSpanSegment.Seconds => new global::System.TimeSpan(current.Days, current.Hours, current.Minutes, value),
                _ => current
            };
        }

        private int ClampSegmentValue(TimeSpanSegment segment, int value)
        {
            return segment switch
            {
                TimeSpanSegment.Days => Math.Max(0, Math.Min(value, MaxDays)),
                TimeSpanSegment.Hours => Math.Max(0, Math.Min(value, 23)),
                TimeSpanSegment.Minutes => Math.Max(0, Math.Min(value, 59)),
                TimeSpanSegment.Seconds => Math.Max(0, Math.Min(value, 59)),
                _ => value
            };
        }

        private global::System.TimeSpan GetSegmentDelta(TimeSpanSegment segment)
        {
            return segment switch
            {
                TimeSpanSegment.Days => global::System.TimeSpan.FromDays(1),
                TimeSpanSegment.Hours => global::System.TimeSpan.FromHours(1),
                TimeSpanSegment.Minutes => global::System.TimeSpan.FromMinutes(1),
                TimeSpanSegment.Seconds => global::System.TimeSpan.FromSeconds(Step),
                _ => global::System.TimeSpan.FromSeconds(Step)
            };
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
        }

        private enum TimeSpanSegment { Days, Hours, Minutes, Seconds }
    }
}
