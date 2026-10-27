using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Forms.Select
{
    public partial class XFSelect<TValue> : XFInputComponentBase, IAsyncDisposable
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public TValue? Value { get; set; }
        [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<TValue?> Change { get; set; }
        [Parameter] public bool Searchable { get; set; }
        [Parameter] public string? SearchPlaceholder { get; set; }
        [Parameter] public RenderFragment<TValue>? OptionTemplate { get; set; }
        [Parameter] public RenderFragment<TValue>? SelectedTemplate { get; set; }
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Clearable { get; set; }
        [Parameter] public bool PlaceholderDisabled { get; set; } = true;
        [Parameter] public bool PlaceholderHidden { get; set; }
        [Parameter] public string ClearLabel { get; set; } = "Limpar selecao";

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private TValue? LastParameterValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        internal TValue CurrentValue { get; set; } = default!;
        internal bool HasValue => !string.IsNullOrEmpty(CurrentValueAsString);
        internal string CurrentValueAsString => FormatValue(CurrentValue);
        internal string SearchText { get; set; } = string.Empty;
        internal bool IsOpen { get; private set; }
        internal string? SelectedDisplayText { get; set; }
        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";

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
            Join("xf-select flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        internal string SelectButtonClass =>
            Join("xf-select-button w-full flex items-center justify-between rounded-lg border transition-all duration-200 ease-out outline-none cursor-pointer select-none",
                SizeClass, VariantClass, ToneClass, StateClass);
        internal string DropdownClass => "xf-select-dropdown";

        private string ClearButtonClass =>
            "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";

        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "h-9 px-3 text-sm",
            XFSize.Lg => "h-12 px-4 text-base",
            _ => "h-10 px-3.5 text-sm"
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
            if (!EqualityComparer<TValue?>.Default.Equals(Value, LastParameterValue))
            {
                CurrentValue = Value!;
                LastParameterValue = Value;
            }

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        internal void ToggleDropdown()
        {
            IsOpen = !IsOpen;
            StateHasChanged();
        }

        internal void CloseDropdown()
        {
            IsOpen = false;
            StateHasChanged();
        }

        internal async Task SelectValueAsync(TValue? value)
        {
            await SetValueAsync(value);
            CloseDropdown();
        }

        internal async Task SelectValueWithDisplayAsync(TValue? value, string? displayText)
        {
            SelectedDisplayText = displayText;
            await SetValueAsync(value);
            CloseDropdown();
        }

        private async Task ClearAsync()
        {
            await SetValueAsync(default);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2091", Justification = "BindConverter.TryConvertTo is part of Microsoft.AspNetCore.Components and is AOT-compatible; TValue is a user-provided type.")]
        [UnconditionalSuppressMessage("Trimming", "IL2091", Justification = "BindConverter.TryConvertTo is part of Microsoft.AspNetCore.Components and is AOT-compatible; TValue is a user-provided type.")]
        private async Task SetValueFromStringAsync(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                await SetValueAsync(default);
                return;
            }

            if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsed))
            {
                await SetValueAsync(parsed);
            }
        }

        internal async Task SetValueAsync(TValue? value)
        {
            CurrentValue = value!;
            Value = value;

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

        internal string FormatOptionValue(TValue? value)
        {
            return FormatValue(value);
        }

        private static string FormatValue(TValue? value)
        {
            return value switch
            {
                null => string.Empty,
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }

        internal static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private DotNetObjectReference<XFSelect<TValue>>? _dotNetRef;
        private bool _jsInitDone;

        [JSInvokable]
        public void CloseFromJsAsync()
        {
            CloseDropdown();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (IsOpen && !_jsInitDone)
            {
                var panelId = $"select-{Id}-dropdown";
                _dotNetRef = DotNetObjectReference.Create(this);
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfSelectRegister", Id, panelId, _dotNetRef);
                    _jsInitDone = true;
                }
                catch
                {
                    // JS not available (SSR), ignore
                }
            }
            else if (!IsOpen && _jsInitDone)
            {
                var panelId = $"select-{Id}-dropdown";
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfSelectRelease", panelId);
                }
                catch (Microsoft.JSInterop.JSDisconnectedException) { }
                catch (InvalidOperationException) { }
                _dotNetRef?.Dispose();
                _dotNetRef = null;
                _jsInitDone = false;
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();

            if (_jsInitDone)
            {
                var panelId = $"select-{Id}-dropdown";
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfSelectRelease", panelId);
                }
                catch (Microsoft.JSInterop.JSDisconnectedException) { }
                catch (InvalidOperationException) { }
            }
            _dotNetRef?.Dispose();
            _dotNetRef = null;
            _jsInitDone = false;
        }
    }
}

