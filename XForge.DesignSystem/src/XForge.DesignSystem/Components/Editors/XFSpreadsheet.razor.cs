using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Editors
{
    public partial class XFSpreadsheet : XForgeComponentBase
    {
        private readonly Dictionary<string, object> _cells = new();

        [Parameter] public int Rows { get; set; } = 50;
        [Parameter] public int Cols { get; set; } = 26;
        [Parameter] public EventCallback<(int Row, int Col, object Value)> OnCellChanged { get; set; }

        private object GetCell(int r, int c)
        {
            var k = $"{r}_{c}";
            return _cells.TryGetValue(k, out var v) ? v : "";
        }
    }
}
