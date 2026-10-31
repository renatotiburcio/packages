using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFPixKeyInput : XForgeComponentBase
    {
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public string PixType { get; set; } = "cpf";
        [Parameter] public EventCallback<string> PixTypeChanged { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Value, PixType);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string Placeholder => PixType switch
        {
            "cpf" => "000.000.000-00",
            "cnpj" => "00.000.000/0000-00",
            "email" => "email@example.com",
            "phone" => "+55 11 9XXXX-XXXX",
            "random" => "UUID v4 (auto)",
            _ => ""
        };

        private void OnTypeChanged() => PixTypeChanged.InvokeAsync(PixType);
    }
}
