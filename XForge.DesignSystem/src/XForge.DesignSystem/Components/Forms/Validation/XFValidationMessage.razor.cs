using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.Validation
{
    public partial class XFValidationMessage : XForgeComponentBase, IDisposable
    {
        private FieldIdentifier? fieldIdentifier;
        private EditContext? previousEditContext;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        [Parameter] public Expression<Func<object?>>? For { get; set; }
        [Parameter] public IEnumerable<string>? Items { get; set; }
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Danger;
        [Parameter] public bool ShowIcon { get; set; } = true;

        private IReadOnlyList<string> Messages => Items?.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray()
            ?? (fieldIdentifier is not null && CascadedEditContext is not null
                ? CascadedEditContext.GetValidationMessages(fieldIdentifier.Value).ToArray()
                : Array.Empty<string>());

        private string RootClass => Join("xf-validation-message flex items-start gap-2 text-xs font-medium leading-5", ToneClass, Class);
        private string IconClass => "mt-0.5 inline-flex h-4 w-4 shrink-0";
        private string IconName => Tone switch
        {
            XFSemanticColor.Success => "circle-check",
            XFSemanticColor.Warning => "triangle-exclamation",
            XFSemanticColor.Info => "circle-info",
            _ => "circle-xmark"
        };

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Success => "text-success",
            XFSemanticColor.Warning => "text-warning",
            XFSemanticColor.Info => "text-primary",
            _ => "text-danger"
        };

        protected override void OnParametersSet()
        {
            fieldIdentifier = For is null ? null : FieldIdentifier.Create(For);

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
