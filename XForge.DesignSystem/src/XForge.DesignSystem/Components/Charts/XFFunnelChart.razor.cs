using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Charts
{
    public partial class XFFunnelChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<FunnelStage> Stages { get; set; } = new List<FunnelStage>();
        [Parameter] public bool Horizontal { get; set; } = true;
        [Parameter] public EventCallback<FunnelStage> OnStageClick { get; set; }
    }
}
