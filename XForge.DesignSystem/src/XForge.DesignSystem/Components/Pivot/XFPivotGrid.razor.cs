using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Pivot
{
    public partial class XFPivotGrid : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<string> Rows { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<string> Columns { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<(string Row, string Col, double Value)> Cells { get; set; } = new List<(string, string, double)>();
        [Parameter] public AggregationType Aggregation { get; set; } = AggregationType.Sum;
        [Parameter] public bool ShowFieldList { get; set; } = true;
        [Parameter] public EventCallback<(string, string, double)> OnCellClick { get; set; }

        // assinatura de render: wrapper fino — o estado interativo vive no componente
        // interno; aqui o render e funcao dos parametros
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Rows.Count, Columns.Count, Cells.Count, Aggregation, ShowFieldList);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
