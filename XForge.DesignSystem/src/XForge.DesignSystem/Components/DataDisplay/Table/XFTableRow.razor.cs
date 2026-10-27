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
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

        private string RowClass => Join(
            ParentTable?.RowClass ?? "transition-colors duration-150",
            Selected ? "bg-[color-mix(in_oklab,var(--color-primary)_9%,transparent)]" : null,
            OnClick.HasDelegate ? "cursor-pointer" : null);

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
