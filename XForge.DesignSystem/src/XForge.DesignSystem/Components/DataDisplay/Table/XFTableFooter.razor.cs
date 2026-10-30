using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.DataDisplay.Table
{
    public partial class XFTableFooter
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Visible { get; set; } = true;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent, Visible);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

        private string FooterClass => "bg-surface-50/80 font-semibold";
    }
}
