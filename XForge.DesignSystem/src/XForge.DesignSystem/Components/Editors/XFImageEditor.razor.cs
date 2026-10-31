using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Editors
{
    public partial class XFImageEditor : XForgeComponentBase
    {
        [Parameter] public string Source { get; set; } = "";
        [Parameter] public int Width { get; set; } = 600;
        [Parameter] public int Height { get; set; } = 400;
        [Parameter] public bool AllowAnnotations { get; set; } = true;
        [Parameter] public EventCallback<string> OnSave { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Source, Width, Height, AllowAnnotations);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private void Rotate(double deg) { }
        private void Crop() { }
    }
}
