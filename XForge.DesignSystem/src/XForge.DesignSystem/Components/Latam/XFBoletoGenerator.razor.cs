using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Latam
{
    public partial class XFBoletoGenerator : XForgeComponentBase
    {
        private string Amount { get; set; } = "";
        private string BeneficiaryCnpj { get; set; } = "";
        private string DocumentNumber { get; set; } = "";

        [Parameter] public string? Barcode { get; set; }
        [Parameter] public EventCallback<string> BarcodeChanged { get; set; }
        [Parameter] public DateTime? DueDate { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Barcode, DueDate);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private void Generate()
        {
            Barcode = "00190500954014481606906809350314337370000000100";
            BarcodeChanged.InvokeAsync(Barcode);
        }

        public static bool IsValidLinhaDigitavel(string s)
        {
            s = new string(s.Where(char.IsDigit).ToArray());
            return s.Length == 47;
        }
    }
}
