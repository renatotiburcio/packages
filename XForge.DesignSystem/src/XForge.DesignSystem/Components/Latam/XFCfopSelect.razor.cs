using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFCfopSelect : XForgeComponentBase
    {
        private static readonly IReadOnlyList<CfopItem> Cfops = new List<CfopItem>
        {
            new("1101", "Compra para industrializacao"),
            new("1102", "Compra para comercializacao"),
            new("1111", "Compra para industrializacao de mercadoria recebida anteriormente em consignacao"),
            new("1401", "Compra para o ativo imobilizado"),
            new("1501", "Entrada de mercadoria recebida com fim especifico de exportacao"),
            new("5101", "Venda de producao do estabelecimento"),
            new("5102", "Venda de mercadoria adquirida ou recebida de terceiros"),
            new("5401", "Venda de ativo imobilizado"),
            new("5501", "Remessa de mercadoria com fim especifico de exportacao"),
            new("6101", "Venda de producao do estabelecimento - ECF"),
            new("6102", "Venda de mercadoria adquirida ou recebida de terceiros - ECF")
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
