using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.TextInput
{
    public partial class XFTextInput : XFInputComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private CancellationTokenSource? debounceCts;

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? InputStyle { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndIcon { get; set; }
        [Parameter] public XFTextInputType Type { get; set; } = XFTextInputType.Text;
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public int DebounceMilliseconds { get; set; }
        [Parameter] public bool Trim { get; set; }
        [Parameter] public bool Clearable { get; set; }
        [Parameter] public bool FloatingLabel { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
        [Parameter] public string? AutoComplete { get; set; }
        [Parameter] public string? InputMode { get; set; }
        [Parameter] public string? Pattern { get; set; }
        [Parameter] public int? MaxLength { get; set; }
        [Parameter] public int? MinLength { get; set; }
        [Parameter] public int? VisibleCharacters { get; set; }
        [Parameter] public string ClearLabel { get; set; } = default!;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(ClearLabel)) ClearLabel = L("input.clear", ClearLabel);
            base.OnInitialized();
        }
        [Parameter] public EventCallback<string?> Change { get; set; }
        [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }
        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private string? CurrentValue { get; set; }
        private string? LastParameterValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string? FirstValidationMessage => FormValidationHelper.GetFirstMessage(CascadedEditContext, FieldIdentifierValue);

        private bool HasValidationError => FormValidationHelper.HasErrors(CascadedEditContext, FieldIdentifierValue);

        private string? DescribedBy => HasValidationError
            ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private string AriaInvalid => HasValidationError ? "true" : "false";

        private string InputType => Type switch
        {
            XFTextInputType.Email => "email",
            XFTextInputType.Password => "password",
            XFTextInputType.Search => "search",
            XFTextInputType.Tel => "tel",
            XFTextInputType.Url => "url",
            _ => "text"
        };

        private string? ResolvedPlaceholder => FloatingLabel && !string.IsNullOrWhiteSpace(Label)
            ? Placeholder ?? " "
            : Placeholder;

        private string RootClass
        {
            get
            {
                var width = FullWidth ? "w-full" : "inline-flex";
                return Join("xf-text-input flex flex-col gap-1.5", width, Class);
            }
        }

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private bool HasEndAdornment =>
            EndIcon is not null
            || !string.IsNullOrWhiteSpace(SuffixText)
            || (Clearable && !string.IsNullOrEmpty(CurrentValue) && !ReadOnly && !Disabled);

        private string FieldShellClass =>
            Join(
                "group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out",
                SizeClass,
                VariantClass,
                ToneClass,
                StateClass);

        private string InputClass =>
            Join(
                "peer min-w-0 flex-1 border-0 bg-transparent p-0 text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted",
                SizeTextClass,
                Type == XFTextInputType.Search && Clearable ? "[&::-webkit-search-cancel-button]:appearance-none [&::-webkit-search-decoration]:appearance-none" : "",
                FloatingLabel ? "pt-2" : "");

        private string FloatingLabelClass =>
            "pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 bg-transparent px-1 text-sm text-text-muted transition-all duration-150 peer-focus:top-2 peer-focus:text-xs peer-focus:text-primary peer-[:not(:placeholder-shown)]:top-2 peer-[:not(:placeholder-shown)]:text-xs";

        private string AdornmentClass =>
            "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string EndAdornmentClass =>
            "flex h-full shrink-0 items-center justify-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string ClearButtonClass =>
            "inline-flex h-6 w-6 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";

        private string HelperClass =>
            "text-xs leading-5 text-text-secondary";

        private string ErrorClass =>
            "text-xs font-medium leading-5 text-danger";

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
                CurrentValue = Value;
                LastParameterValue = Value;
            }

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private Task HandleInputAsync(ChangeEventArgs args)
        {
            if (!Immediate)
            {
                return Task.CompletedTask;
            }

            return SetValueWithDebounceAsync(args.Value?.ToString());
        }

        private async Task HandleChangeAsync(ChangeEventArgs args)
        {
            await SetValueAsync(args.Value?.ToString());
        }

        private async Task SetValueAsync(string? value)
        {
            var parsed = Trim ? value?.Trim() : value;
            CurrentValue = parsed;
            Value = parsed;

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

        private async Task ClearAsync()
        {
            await SetValueAsync(string.Empty);
        }

        private async Task SetValueWithDebounceAsync(string? value)
        {
            if (DebounceMilliseconds <= 0)
            {
                await SetValueAsync(value);
                return;
            }

            debounceCts?.Cancel();
            debounceCts?.Dispose();
            debounceCts = new CancellationTokenSource();
            var token = debounceCts.Token;

            CurrentValue = value;

            try
            {
                await Task.Delay(DebounceMilliseconds, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (!token.IsCancellationRequested)
            {
                await SetValueAsync(value);
            }
        }

        public override async ValueTask DisposeAsync()
        {
            if (debounceCts is not null)
            {
                debounceCts.Cancel();
                debounceCts.Dispose();
                debounceCts = null;
            }

            await base.DisposeAsync();
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
