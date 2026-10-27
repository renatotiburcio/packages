using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
{
    public partial class XFCnaeSelect : XForgeComponentBase
    {
        private static readonly IReadOnlyList<CnaeItem> Cnaes = new List<CnaeItem>
        {
            new("0111-3/01", "Cultivo de arroz"), new("0111-3/02", "Cultivo de milho"),
            new("0141-1/00", "Pecuaria de corte"),
            new("4711-3/01", "Comercio varejista de mercadorias em geral"),
            new("4711-3/02", "Comercio varejista de mercadorias em geral - hipermercados"),
            new("4781-4/00", "Comercio varejista de artigos do vestuario"),
            new("6201-5/00", "Desenvolvimento de programas de computador sob encomenda"),
            new("6202-3/00", "Desenvolvimento e licenciamento de programas de computador customizaveis"),
            new("6203-1/00", "Desenvolvimento e licenciamento de programas de computador nao-customizaveis"),
            new("6204-0/00", "Consultoria em tecnologia da informacao"),
            new("6209-1/00", "Suporte tecnico, manutencao e outros servicos em tecnologia da informacao"),
            new("6311-9/00", "Tratamento de dados, provedores de servicos de aplicacao e servicos de hospedagem"),
            new("6410-7/00", "Bancos multiplo, com carteira comercial"),
            new("6421-2/00", "Bancos multiplo, sem carteira comercial"),
            new("6422-1/00", "Bancos cooperativos"), new("6423-9/00", "Caixas economicas")
        };

        private string Filter { get; set; } = "";
        private string? SelectedCode { get; set; }

        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private IEnumerable<CnaeItem> Filtered => string.IsNullOrEmpty(Filter)
            ? Cnaes
            : Cnaes.Where(c => c.Code.Contains(Filter, StringComparison.OrdinalIgnoreCase) || c.Description.Contains(Filter, StringComparison.OrdinalIgnoreCase));

        private void OnChanged()
        {
            Value = SelectedCode;
            ValueChanged.InvokeAsync(SelectedCode);
        }
    }
}
