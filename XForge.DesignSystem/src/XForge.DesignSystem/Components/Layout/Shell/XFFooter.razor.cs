using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Layout.Shell
{
    public partial class XFFooter : XForgeComponentBase
    {
        [Parameter] public string StatusText { get; set; } = "Estavel";
        [Parameter] public string? UserName { get; set; }
        [Parameter] public string? Profile { get; set; }
        [Parameter] public string? AppName { get; set; }
        [Parameter] public string Environment { get; set; } = "Producao";
        [Parameter] public string Version { get; set; } = "v1.0.0";
        [Parameter] public string? CustomDate { get; set; }
        [Parameter] public RenderFragment? StartContent { get; set; }
        [Parameter] public RenderFragment? EndContent { get; set; }
        [Parameter] public bool ShowStatusDot { get; set; } = true;
        [Parameter] public string StatusClass { get; set; } = "text-success";

        private string DateText => !string.IsNullOrEmpty(CustomDate) ? CustomDate : DateTime.Now.ToString("MM/yyyy");

        private string GetFooterClass()
        {
            return GetClass("flex items-center w-full min-h-10 bg-bg-surface border-t border-border-light px-4 shrink-0 transition-theme");
        }
    }
}
