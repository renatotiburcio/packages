using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFSpedViewer : XForgeComponentBase
    {
        [Parameter] public string Type { get; set; } = "EFD_ICMS_IPI";
        [Parameter] public DateTime PeriodStart { get; set; } = DateTime.Today.AddMonths(-1);
        [Parameter] public DateTime PeriodEnd { get; set; } = DateTime.Today;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback<string> OnTypeChanged { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Type, PeriodStart, PeriodEnd, ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
