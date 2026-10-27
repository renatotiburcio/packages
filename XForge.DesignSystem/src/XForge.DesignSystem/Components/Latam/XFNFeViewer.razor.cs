using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
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
    }
}
