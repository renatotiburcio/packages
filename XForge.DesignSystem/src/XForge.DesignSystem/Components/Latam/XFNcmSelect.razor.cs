using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
{
    public partial class XFNcmSelect : XForgeComponentBase
    {
        private static readonly IReadOnlyList<NcmItem> Ncms = new List<NcmItem>
        {
            new("8471.30.19", "Outros maquinas automatica p/processamento de dados portateis", 0),
            new("8471.30.12", "Notebooks", 0),
            new("8517.12.31", "Telefones celulares (smartphones)", 0),
            new("8517.12.32", "Smartwatches", 0),
            new("8528.72.00", "Outros receptores de televisao", 0),
            new("0901.21.00", "Cafe torrado", 0),
            new("2202.10.00", "Aguas, incluindo aguas minerais e aguas gaseificadas", 0),
            new("8703.23.10", "Autoveiculos de passageiros", 25)
        };

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
    }
}
