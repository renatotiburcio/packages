using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace XForge.DesignSystem.Components.DataDisplay.Table
{
    public partial class XFTableRow
    {
        [CascadingParameter] public XFTable? ParentTable { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Selected { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent, Visible, Selected, OnClick);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

        private string RowClass => Join(
            ParentTable?.RowClass ?? "transition-colors duration-150",
            Selected ? "bg-[color-mix(in_oklab,var(--color-primary)_9%,transparent)]" : null,
            OnClick.HasDelegate ? "cursor-pointer" : null);

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
