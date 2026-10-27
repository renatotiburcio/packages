using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Pivot
{
    public partial class XFPivotFieldList : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<string> AvailableFields { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<string> SelectedFields { get; set; } = new List<string>();
        [Parameter] public EventCallback<IReadOnlyList<string>> SelectedFieldsChanged { get; set; }

        private void Toggle(string f)
        {
            var l = SelectedFields.ToList();
            if (l.Contains(f)) l.Remove(f); else l.Add(f);
            SelectedFields = l;
            SelectedFieldsChanged.InvokeAsync(l);
        }
    }
}
