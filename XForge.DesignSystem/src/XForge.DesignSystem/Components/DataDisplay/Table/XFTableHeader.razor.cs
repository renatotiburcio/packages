using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.DataDisplay.Table
{
    public partial class XFTableHeader
    {
        [CascadingParameter] public XFTable? ParentTable { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

        private string HeaderClass => ParentTable?.HeaderClass ?? "bg-surface-50";
    }
}
