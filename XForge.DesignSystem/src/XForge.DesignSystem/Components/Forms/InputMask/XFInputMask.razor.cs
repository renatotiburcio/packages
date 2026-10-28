using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.InputMask
{
    public partial class XFInputMask : XFInputComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndIcon { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<string?> Change { get; set; }
        [Parameter] public string Mask { get; set; } = default!;
        [Parameter] public char PlaceholderChar { get; set; } = '_';
        [Parameter] public bool ShowMaskPlaceholder { get; set; }
        [Parameter] public bool UnmaskValue { get; set; } = true;
        [Parameter] public bool ClearIncomplete { get; set; }
        [Parameter] public bool Clearable { get; set; }
        [Parameter] public string ClearLabel { get; set; } = default!;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(ClearLabel)) ClearLabel = L("inputmask.clear", ClearLabel);
            base.OnInitialized();
        }
        [Parameter] public string? MaskDescriptionFormat { get; set; }
        [Parameter] public string? AutoComplete { get; set; }
        [Parameter] public string? InputMode { get; set; }
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; } = true;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private string? CurrentText { get; set; }
        private string? LastParameterValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string MaskDescriptionId => $"{Id}-mask-description";
        private string? MaskDescription => string.IsNullOrWhiteSpace(MaskDescriptionFormat) || string.IsNullOrWhiteSpace(Mask)
            ? null
            : string.Format(CultureInfo.CurrentCulture, MaskDescriptionFormat, Mask);
        private string? ResolvedPlaceholder => Placeholder ?? (ShowMaskPlaceholder ? BuildPlaceholder() : null);
        private int? MaxLength => string.IsNullOrEmpty(Mask) ? null : Mask.Length;
        private bool HasEndAdornment => EndIcon is not null || !string.IsNullOrWhiteSpace(SuffixText) || (Clearable && !string.IsNullOrEmpty(CurrentText) && !ReadOnly && !Disabled);
        private string AriaInvalid => HasValidationError ? "true" : "false";

        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(FieldIdentifierValue!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && ValueExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string? DescribedBy => Join(
            HasValidationError ? ErrorId : null,
            !HasValidationError && !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null,
            MaskDescription is not null ? MaskDescriptionId : null);

        private string RootClass =>
            Join("xf-input-mask flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

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
                SizeTextClass);

        private string AdornmentClass =>
            "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string ClearButtonClass =>
            "inline-flex h-6 w-6 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";

        private string HelperClass => "text-xs leading-5 text-text-secondary";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => $"h-9 py-1.5 pl-3 {(HasEndAdornment ? "pr-1.5" : "pr-3")}",
            XFSize.Lg => $"h-12 py-2.5 pl-4 {(HasEndAdornment ? "pr-2" : "pr-4")}",
            _ => $"h-10 py-2 pl-3.5 {(HasEndAdornment ? "pr-1.5" : "pr-3.5")}"
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
            if (Value != LastParameterValue)
            {
                CurrentText = UnmaskValue ? ApplyMask(Value ?? string.Empty) : Value;
                LastParameterValue = Value;
            }

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            CurrentText = ApplyMask(args.Value?.ToString() ?? string.Empty);
            if (Immediate)
            {
                await CommitAsync(CurrentText, clearIncomplete: false);
            }
        }

        private Task HandleChangeAsync(ChangeEventArgs args)
        {
            CurrentText = ApplyMask(args.Value?.ToString() ?? string.Empty);
            return CommitAsync(CurrentText, clearIncomplete: true);
        }

        private Task HandleBlurAsync()
        {
            return CommitAsync(CurrentText, clearIncomplete: true);
        }

        private Task ClearAsync()
        {
            CurrentText = string.Empty;
            return CommitAsync(CurrentText, clearIncomplete: true);
        }

        private async Task CommitAsync(string? maskedText, bool clearIncomplete)
        {
            if (clearIncomplete && ClearIncomplete && !string.IsNullOrWhiteSpace(maskedText) && !IsComplete(maskedText))
            {
                maskedText = string.Empty;
                CurrentText = string.Empty;
            }

            var nextValue = UnmaskValue ? StripLiterals(maskedText ?? string.Empty) : maskedText;
            Value = nextValue;
            LastParameterValue = nextValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(Value);
            }

            if (CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private string ApplyMask(string value)
        {
            if (string.IsNullOrEmpty(Mask))
            {
                return value;
            }

            var raw = StripLiterals(value);
            var rawIndex = 0;
            var result = new List<char>();

            foreach (var token in Mask)
            {
                if (IsMaskToken(token))
                {
                    var matched = false;
                    while (rawIndex < raw.Length)
                    {
                        var candidate = raw[rawIndex++];
                        if (MatchesToken(token, candidate))
                        {
                            result.Add(candidate);
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        break;
                    }
                }
                else if (rawIndex < raw.Length)
                {
                    result.Add(token);
                }
            }

            return new string(result.ToArray());
        }

        private string StripLiterals(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return new string(value.Where(character => char.IsLetterOrDigit(character)).ToArray());
        }

        private bool IsComplete(string value)
        {
            var unmasked = StripLiterals(value);
            var requiredCount = Mask.Count(token => token is '0' or 'A' or '*');
            return unmasked.Length >= requiredCount;
        }

        private string BuildPlaceholder()
        {
            return new string(Mask.Select(token => IsMaskToken(token) ? PlaceholderChar : token).ToArray());
        }

        private static bool IsMaskToken(char token)
        {
            return token is '0' or '9' or 'A' or '*';
        }

        private static bool MatchesToken(char token, char value)
        {
            return token switch
            {
                '0' or '9' => char.IsDigit(value),
                'A' => char.IsLetter(value),
                '*' => char.IsLetterOrDigit(value),
                _ => false
            };
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
