using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Charts
{
    public partial class XFHeatmapChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<string> Rows { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<string> Columns { get; set; } = new List<string>();
        [Parameter] public double[,]? Values { get; set; }
        [Parameter] public string ColorScale { get; set; } = "viridis";
        [Parameter] public EventCallback<(int Row, int Col)> OnCellClick { get; set; }

        private double Hue(double v) => v switch { < 0.25 => 0, < 0.5 => 60, < 0.75 => 120, _ => 240 };
    }
}
