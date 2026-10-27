using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.Textarea
{
    public partial class XFTextarea : XFInputComponentBase
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<string?> Change { get; set; }
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFTextareaResize Resize { get; set; } = XFTextareaResize.Vertical;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public bool Trim { get; set; }
        [Parameter] public bool ShowCount { get; set; }
        [Parameter] public bool? SpellCheck { get; set; }
        [Parameter] public int Rows { get; set; } = 4;
        [Parameter] public int? Cols { get; set; }
        [Parameter] public int? MaxLength { get; set; }
        [Parameter] public int? MinLength { get; set; }
        [Parameter] public string CharacterCountFormat { get; set; } = "{0}";
        [Parameter] public string CharacterLimitFormat { get; set; } = "{0}/{1}";

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private string? CurrentValue { get; set; }
        private string? LastParameterValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private int CharacterCount => CurrentValue?.Length ?? 0;
        private string CharacterCountText => MaxLength is null
            ? string.Format(CultureInfo.CurrentCulture, CharacterCountFormat, CharacterCount)
            : string.Format(CultureInfo.CurrentCulture, CharacterLimitFormat, CharacterCount, MaxLength);
        private bool ShowMetaRow => !string.IsNullOrWhiteSpace(HelperText) || ShowCount;
        private string AriaInvalid => HasValidationError ? "true" : "false";

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
            : ShowMetaRow && !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string RootClass =>
            Join("xf-textarea flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string FieldShellClass =>
            Join(
                "group relative flex rounded-lg border transition-all duration-200 ease-out",
                VariantClass,
                ToneClass,
                StateClass);

        private string TextareaClass =>
            Join(
                "min-w-0 flex-1 border-0 bg-transparent text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted",
                SizeClass,
                ResizeClass);

        private string SizeClass => Size switch
        {
            XFSize.Sm => "p-3 text-sm leading-5",
            XFSize.Lg => "p-4 text-base leading-6",
            _ => "p-3.5 text-sm leading-6"
        };

        private string ResizeClass => Resize switch
        {
            XFTextareaResize.None => "resize-none",
            XFTextareaResize.Horizontal => "resize-x",
            XFTextareaResize.Both => "resize",
            _ => "resize-y"
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

        private string MetaRowClass => "flex items-center justify-between gap-3 text-xs leading-5 text-text-secondary";

        private string CounterClass =>
            MaxLength is not null && CharacterCount > MaxLength
                ? "font-medium text-danger"
                : "tabular-nums text-text-muted";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        protected override void OnParametersSet()
        {
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
            if (!Immediate && !ShowCount)
            {
                return;
            }

            await SetValueAsync(args.Value?.ToString(), notifyChange: Immediate);
        }

        private async Task HandleChangeAsync(ChangeEventArgs args)
        {
            await SetValueAsync(args.Value?.ToString(), notifyChange: true);
        }

        private async Task SetValueAsync(string? value, bool notifyChange)
        {
            var parsed = Trim ? value?.Trim() : value;
            CurrentValue = parsed;
            Value = parsed;

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

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
