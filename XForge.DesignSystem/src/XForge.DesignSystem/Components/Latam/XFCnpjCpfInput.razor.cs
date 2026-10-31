using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFCnpjCpfInput : XForgeComponentBase
    {
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public string Type { get; set; } = "CPF";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Value, Type);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

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
