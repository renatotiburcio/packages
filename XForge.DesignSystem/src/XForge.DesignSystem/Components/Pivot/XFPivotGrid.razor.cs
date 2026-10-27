using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Pivot
{
    public partial class XFPivotGrid : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<string> Rows { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<string> Columns { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<(string Row, string Col, double Value)> Cells { get; set; } = new List<(string, string, double)>();
        [Parameter] public AggregationType Aggregation { get; set; } = AggregationType.Sum;
        [Parameter] public bool ShowFieldList { get; set; } = true;
        [Parameter] public EventCallback<(string, string, double)> OnCellClick { get; set; }
    }
}
