using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFNFeViewer : XForgeComponentBase
    {
        [Parameter] public string AccessKey { get; set; } = "";
        [Parameter] public string Number { get; set; } = "";
        [Parameter] public DateTime IssueDate { get; set; }
        [Parameter] public string Status { get; set; } = "Autorizada";
        [Parameter] public string IssuerName { get; set; } = "";
        [Parameter] public string IssuerCnpj { get; set; } = "";
        [Parameter] public string RecipientName { get; set; } = "";
        [Parameter] public string RecipientCnpj { get; set; } = "";
        [Parameter] public IReadOnlyList<NFeItem> Items { get; set; } = new List<NFeItem>();
        [Parameter] public decimal TotalProducts { get; set; }
        [Parameter] public decimal IcmsValue { get; set; }
        [Parameter] public decimal IpiValue { get; set; }
        [Parameter] public decimal TotalAmount { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', AccessKey, Number, IssueDate, Status, IssuerName, IssuerCnpj, RecipientName, RecipientCnpj, Items.Count, TotalProducts, IcmsValue, TotalAmount);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
