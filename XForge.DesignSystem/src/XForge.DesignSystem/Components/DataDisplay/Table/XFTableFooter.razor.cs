using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.DataDisplay.Table
{
    public partial class XFTableFooter
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

        private string FooterClass => "bg-surface-50/80 font-semibold";
    }
}
