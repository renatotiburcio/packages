using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    public partial class XFValidationStatus : XForgeComponentBase, IDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private EditContext? previousEditContext;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        [Parameter] public string ValidText { get; set; } = default!;
        [Parameter] public string InvalidText { get; set; } = default!;
        [Parameter] public string NeutralText { get; set; } = default!;
        [Parameter] public string? AriaLabel { get; set; } = "Status da validacao";
        [Parameter] public bool ShowIcon { get; set; } = true;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(ValidText)) ValidText = L("validation.valid", ValidText);
            if (string.IsNullOrWhiteSpace(InvalidText)) InvalidText = L("validation.invalid", InvalidText);
            if (string.IsNullOrWhiteSpace(NeutralText)) NeutralText = L("validation.neutral", NeutralText);
            if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("validation.status", AriaLabel);
            base.OnInitialized();
        }

        private bool HasContext => CascadedEditContext is not null;
        private bool HasMessages => CascadedEditContext?.GetValidationMessages().Any() == true;
        private bool HasModifiedFields => CascadedEditContext?.IsModified() == true;
        private bool IsValid => HasContext && !HasMessages;
        private string Text => !HasContext || !HasModifiedFields ? NeutralText : IsValid ? ValidText : InvalidText;
        private string IconName => !HasContext || !HasModifiedFields ? "circle-info" : IsValid ? "circle-check" : "circle-xmark";

        private string RootClass => Join(
            "xf-validation-status inline-flex items-center gap-2 rounded-full px-3 py-1 text-xs font-semibold",
            !HasContext || !HasModifiedFields
                ? "bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary"
                : IsValid
                    ? "bg-[color-mix(in_oklab,var(--color-success)_10%,transparent)] text-success"
                    : "bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger",
            Class);

        private string IconClass => "inline-flex h-4 w-4 shrink-0";

        protected override void OnParametersSet()
        {
            if (!ReferenceEquals(previousEditContext, CascadedEditContext))
            {
                if (previousEditContext is not null)
                {
                    previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
                    previousEditContext.OnFieldChanged -= OnFieldChanged;
                }

                previousEditContext = CascadedEditContext;

                if (previousEditContext is not null)
                {
                    previousEditContext.OnValidationStateChanged += OnValidationStateChanged;
                    previousEditContext.OnFieldChanged += OnFieldChanged;
                }
            }
        }

        private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs args)
        {
            _ = InvokeAsync(StateHasChanged);
        }

        private void OnFieldChanged(object? sender, FieldChangedEventArgs args)
        {
            _ = InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (previousEditContext is not null)
            {
                previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
                previousEditContext.OnFieldChanged -= OnFieldChanged;
            }
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
