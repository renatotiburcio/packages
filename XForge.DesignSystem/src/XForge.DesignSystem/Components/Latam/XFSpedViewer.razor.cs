using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
{
    public partial class XFSpedViewer : XForgeComponentBase
    {
        [Parameter] public string Type { get; set; } = "EFD_ICMS_IPI";
        [Parameter] public DateTime PeriodStart { get; set; } = DateTime.Today.AddMonths(-1);
        [Parameter] public DateTime PeriodEnd { get; set; } = DateTime.Today;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback<string> OnTypeChanged { get; set; }
    }
}
