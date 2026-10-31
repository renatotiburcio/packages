using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Editors
{
    public partial class XFDocumentEditor : XForgeComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string Header { get; set; } = "";
        [Parameter] public string Footer { get; set; } = "";
        [Parameter] public int Pages { get; set; } = 1;
        [Parameter] public string Orientation { get; set; } = "portrait";
        [Parameter] public bool TrackChanges { get; set; } = false;
        [Parameter] public IReadOnlyList<string> ToolbarCommands { get; set; } = new List<string> { "Bold", "Italic", "Underline", "Strike", "H1", "H2", "P", "Table", "Image", "List" };
        [Parameter] public EventCallback<string> OnCommand { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent, Header, Footer, Pages, Orientation, TrackChanges);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
