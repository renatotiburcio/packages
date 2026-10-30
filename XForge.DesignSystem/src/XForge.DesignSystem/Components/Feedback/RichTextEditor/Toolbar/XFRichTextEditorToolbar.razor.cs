using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorToolbar : XForgeComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: botão de toolbar — evita re-render dos 26 itens a cada
        // tecla digitada no editor (o pai re-renderiza sem mudar nada aqui)
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override string BaseClass => "xf-toolbar flex items-center gap-1 px-2 py-1 border-b border-surface-200 bg-bg-surface";
    }
}
