using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Charts
{
    public partial class XFFunnelChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<FunnelStage> Stages { get; set; } = new List<FunnelStage>();
        [Parameter] public bool Horizontal { get; set; } = true;
        [Parameter] public EventCallback<FunnelStage> OnStageClick { get; set; }

        // assinatura de render: gráficos são caros de renderizar — pula render redundante quando o
        // pai re-renderiza sem mudar nada aqui (e evita re-invocar interop JS no AfterRender)
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Stages is null ? -1 : Stages.Count, Horizontal, OnStageClick);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
