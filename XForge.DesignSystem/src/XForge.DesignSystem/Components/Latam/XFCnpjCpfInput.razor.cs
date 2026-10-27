using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Latam
{
    public partial class XFCnpjCpfInput : XForgeComponentBase
    {
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public string Type { get; set; } = "CPF";

        private void OnTypeChanged()
        {
            Value = "";
            ValueChanged.InvokeAsync("");
        }

        private void OnValue(string v)
        {
            Value = v;
            ValueChanged.InvokeAsync(v);
        }
    }
}
