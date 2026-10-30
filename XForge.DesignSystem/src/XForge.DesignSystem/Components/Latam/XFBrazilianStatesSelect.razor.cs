using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
{
    public partial class XFBrazilianStatesSelect : XForgeComponentBase
    {
        private static readonly IReadOnlyList<BrazilianState> States = new List<BrazilianState>
        {
            new("AC", "Acre"), new("AL", "Alagoas"), new("AP", "Amapa"), new("AM", "Amazonas"),
            new("BA", "Bahia"), new("CE", "Ceara"), new("DF", "Distrito Federal"), new("ES", "Espirito Santo"),
            new("GO", "Goias"), new("MA", "Maranhao"), new("MT", "Mato Grosso"), new("MS", "Mato Grosso do Sul"),
            new("MG", "Minas Gerais"), new("PA", "Para"), new("PB", "Paraiba"), new("PR", "Parana"),
            new("PE", "Pernambuco"), new("PI", "Piaui"), new("RJ", "Rio de Janeiro"), new("RN", "Rio Grande do Norte"),
            new("RS", "Rio Grande do Sul"), new("RO", "Rondonia"), new("RR", "Roraima"), new("SC", "Santa Catarina"),
            new("SP", "Sao Paulo"), new("SE", "Sergipe"), new("TO", "Tocantins")
        };

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Value, ValueChanged);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
