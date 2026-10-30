using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.Field
{
    public partial class XFField : XForgeComponentBase
    {
        private FieldIdentifier? fieldIdentifier;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? StartAdornment { get; set; }
        [Parameter] public RenderFragment? EndAdornment { get; set; }
        [Parameter] public Expression<Func<object?>>? ValueExpression { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? For { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? ErrorText { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public XFFieldOrientation Orientation { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent, StartAdornment, EndAdornment, ValueExpression, Label, For, HelperText, ErrorText, Required, Orientation);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string? CurrentErrorText => !string.IsNullOrWhiteSpace(ErrorText)
            ? ErrorText
            : fieldIdentifier is not null && CascadedEditContext is not null
                ? CascadedEditContext.GetValidationMessages(fieldIdentifier.Value).FirstOrDefault()
                : null;

        private string RootClass => Join(
            "xf-field",
            Orientation == XFFieldOrientation.Horizontal ? "grid gap-3 sm:grid-cols-[12rem_1fr] sm:items-start" : "grid gap-1.5",
            Disabled ? "opacity-60" : "",
            Class);

        private string BodyClass => "grid min-w-0 gap-1.5";
        private string LabelClass => Orientation == XFFieldOrientation.Horizontal
            ? "flex min-h-10 items-center gap-1 text-sm font-medium leading-5 text-text-primary"
            : "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string ControlShellClass => "flex min-h-10 items-center gap-2 rounded-lg border border-surface-200 bg-bg-surface px-3.5 shadow-[0_1px_2px_rgb(15_23_42/.035)] ring-1 ring-surface-200/80 focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]";
        private string AdornmentClass => "inline-flex h-5 w-5 shrink-0 items-center justify-center text-text-muted";
        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        protected override void OnParametersSet()
        {
            fieldIdentifier = ValueExpression is null ? null : FieldIdentifier.Create(ValueExpression);
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
