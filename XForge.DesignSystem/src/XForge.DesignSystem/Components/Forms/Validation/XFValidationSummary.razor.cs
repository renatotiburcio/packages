using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.Validation
{
    public partial class XFValidationSummary : XForgeComponentBase, IDisposable
    {
        private EditContext? previousEditContext;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        [Parameter] public IEnumerable<string>? Items { get; set; }
        [Parameter] public string? Title { get; set; } = "Revise os campos destacados";
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Danger;
        [Parameter] public bool ShowIcon { get; set; } = true;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Items is null ? 0 : Items.Count(), Title, Tone, ShowIcon);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private IReadOnlyList<string> Messages => Items?.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray()
            ?? CascadedEditContext?.GetValidationMessages().ToArray()
            ?? Array.Empty<string>();

        private string RootClass => Join("xf-validation-summary rounded-lg border p-4", ToneClass, Class);
        private string IconClass => "mt-0.5 inline-flex h-5 w-5 shrink-0";
        private string ListClass => string.IsNullOrWhiteSpace(Title)
            ? "grid gap-1 text-sm"
            : "mt-2 grid gap-1 text-sm";

        private string IconName => Tone switch
        {
            XFSemanticColor.Success => "circle-check",
            XFSemanticColor.Warning => "triangle-exclamation",
            XFSemanticColor.Info => "circle-info",
            _ => "circle-xmark"
        };

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Success => "border-success bg-[color-mix(in_oklab,var(--color-success)_8%,transparent)] text-success",
            XFSemanticColor.Warning => "border-warning bg-[color-mix(in_oklab,var(--color-warning)_10%,transparent)] text-warning",
            XFSemanticColor.Info => "border-primary bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary",
            _ => "border-danger bg-[color-mix(in_oklab,var(--color-danger)_8%,transparent)] text-danger"
        };

        protected override void OnParametersSet()
        {
            if (!ReferenceEquals(previousEditContext, CascadedEditContext))
            {
                if (previousEditContext is not null)
                {
                    previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
                }

                previousEditContext = CascadedEditContext;

                if (previousEditContext is not null)
                {
                    previousEditContext.OnValidationStateChanged += OnValidationStateChanged;
                }
            }
        }

        private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs args)
        {
            _ = InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (previousEditContext is not null)
            {
                previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
            }
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
