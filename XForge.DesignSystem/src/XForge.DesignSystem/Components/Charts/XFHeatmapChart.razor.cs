using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Charts
{
    public partial class XFHeatmapChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<string> Rows { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<string> Columns { get; set; } = new List<string>();
        [Parameter] public double[,]? Values { get; set; }
        [Parameter] public string ColorScale { get; set; } = "viridis";
        [Parameter] public EventCallback<(int Row, int Col)> OnCellClick { get; set; }

        // assinatura de render: gráficos são caros de renderizar — pula render redundante quando o
        // pai re-renderiza sem mudar nada aqui (e evita re-invocar interop JS no AfterRender)
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Rows.Count, Columns.Count, ColorScale);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private double Hue(double v) => v switch { < 0.25 => 0, < 0.5 => 60, < 0.75 => 120, _ => 240 };
    }
}
