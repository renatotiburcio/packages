using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.Rating
{
    public partial class XFRating : XFInputComponentBase
    {
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Name { get; set; }
        [Parameter] public double Value { get; set; }
        [Parameter] public EventCallback<double> ValueChanged { get; set; }
        [Parameter] public Expression<Func<double>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<double> Change { get; set; }
        [Parameter] public EventCallback<int?> HoverValueChanged { get; set; }
        [Parameter] public int Max { get; set; } = 5;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool AllowReset { get; set; }
        [Parameter] public bool ShowValue { get; set; }
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Warning;
        [Parameter] public RenderFragment<XFRatingItemContext>? Icon { get; set; }
        [Parameter] public Func<int, string>? GetItemLabel { get; set; }
        [Parameter] public string ClearLabel { get; set; } = "Limpar avaliacao";
        [Parameter] public string ItemLabelFormat { get; set; } = "Avaliar {0} de {1}";
        [Parameter] public string ValueTextFormat { get; set; } = "{0}/{1}";
        [Parameter] public CultureInfo? Culture { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private double CurrentValue { get; set; }
        private double LastParameterValue { get; set; } = double.NaN;
        private int? HoverValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private int SafeMax => Math.Clamp(Max, 1, 20);
        private bool IsInteractive => !Disabled && !ReadOnly;
        private double DisplayValue => HoverValue ?? CurrentValue;
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string AriaInvalid => HasValidationError ? "true" : "false";
        private string AriaReadonly => ReadOnly ? "true" : "false";
        private string AriaRequired => Required ? "true" : "false";
        private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;

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
            Join("xf-rating flex flex-col gap-1.5", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string RatingShellClass =>
            Join(
                "inline-flex w-fit items-center gap-1 rounded-lg outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2",
                Disabled ? "cursor-not-allowed opacity-60" : "",
                ReadOnly ? "cursor-default" : "");

        private string ItemClass =>
            Join(
                "inline-flex shrink-0 items-center justify-center rounded-md border border-transparent bg-transparent outline-none transition-all duration-150",
                SizeClass,
                IsInteractive ? "cursor-pointer hover:-translate-y-0.5 hover:bg-current/7 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 active:translate-y-px" : "cursor-default");

        private string ClearButtonClass =>
            Join(
                "inline-flex shrink-0 items-center justify-center rounded-md border border-transparent bg-transparent text-text-muted outline-none transition-all duration-150",
                SizeClass,
                IsInteractive ? "cursor-pointer hover:bg-danger/10 hover:text-danger focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2" : "cursor-default");

        private string FilledClass => ToneTextClass;
        private string EmptyClass => "text-text-muted/45";
        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";
        private string ValueClass => "ml-2 text-xs font-medium text-text-secondary";

        private string SizeClass => Size switch
        {
            XFExtendedSize.Sm => "h-7 w-7 text-base",
            XFExtendedSize.Lg => "h-10 w-10 text-2xl",
            XFExtendedSize.Xl => "h-12 w-12 text-3xl",
            _ => "h-8 w-8 text-xl"
        };

        private string ToneTextClass => Tone switch
        {
            XFSemanticColor.Primary => "text-primary",
            XFSemanticColor.Secondary => "text-text-secondary",
            XFSemanticColor.Success => "text-success",
            XFSemanticColor.Danger => "text-danger",
            XFSemanticColor.Neutral => "text-text-primary",
            _ => "text-warning"
        };

        protected override void OnParametersSet()
        {
            var normalized = NormalizeValue(Value);

            if (!normalized.Equals(LastParameterValue))
            {
                CurrentValue = normalized;
                LastParameterValue = normalized;
            }

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private async Task SetValueAsync(double value)
        {
            if (!IsInteractive)
            {
                return;
            }

            var next = NormalizeValue(value);

            if (AllowReset && next.Equals(CurrentValue))
            {
                next = 0;
            }

            CurrentValue = next;
            Value = next;
            LastParameterValue = next;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(next);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(next);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private Task ClearAsync()
        {
            return SetValueAsync(0);
        }

        private async Task SetHoverAsync(int? value)
        {
            if (!IsInteractive || HoverValue == value)
            {
                return;
            }

            HoverValue = value;

            if (HoverValueChanged.HasDelegate)
            {
                await HoverValueChanged.InvokeAsync(value);
            }
        }

        private Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (!IsInteractive)
            {
                return Task.CompletedTask;
            }

            return args.Key switch
            {
                "ArrowRight" or "ArrowUp" => SetValueAsync(CurrentValue >= SafeMax ? SafeMax : Math.Floor(CurrentValue) + 1),
                "ArrowLeft" or "ArrowDown" => SetValueAsync(CurrentValue <= 0 ? 0 : Math.Ceiling(CurrentValue) - 1),
                "Home" => SetValueAsync(AllowReset ? 0 : 1),
                "End" => SetValueAsync(SafeMax),
                "Delete" or "Backspace" when AllowReset => ClearAsync(),
                _ => Task.CompletedTask
            };
        }

        private double NormalizeValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return 0;
            }

            return Math.Clamp(value, 0, SafeMax);
        }

        private bool IsFilled(int index) => DisplayValue >= index;

        private bool IsHalf(int index) => !IsFilled(index) && DisplayValue >= index - 0.5d;

        private string GetItemClass(int index)
        {
            return Join(ItemClass, IsFilled(index) || IsHalf(index) ? FilledClass : EmptyClass);
        }

        private string GetAriaChecked(int index)
        {
            return Math.Round(CurrentValue).Equals((double)index) ? "true" : "false";
        }

        private string GetDefaultItemLabel(int index)
        {
            return GetItemLabel?.Invoke(index) ?? string.Format(CultureInfo.CurrentCulture, ItemLabelFormat, index, SafeMax);
        }

        private string FormatValue()
        {
            var valueText = CurrentValue.ToString(CurrentValue % 1 == 0 ? "0" : "0.0", EffectiveCulture);
            return string.Format(EffectiveCulture, ValueTextFormat, valueText, SafeMax);
        }

        private XFRatingItemContext CreateContext(int index)
        {
            return new(index, IsFilled(index), IsHalf(index), HoverValue == index, CurrentValue);
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
