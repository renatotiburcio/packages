using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for selecting the font family in the rich text editor.
    /// Executes the <c>fontName</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorFontFamily : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"fontName"</c>.</summary>
        [Parameter] public string Command { get; set; } = "fontName";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "Font Family";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "font_download";

        /// <summary>The selected font family value (e.g. "Arial", "Times New Roman").</summary>
        [Parameter] public string Value { get; set; } = "Arial";

        /// <summary>Callback invoked when the button is clicked.</summary>
        [Parameter] public EventCallback Click { get; set; }

        // assinatura de render: botão de toolbar — evita re-render dos 26 itens a cada
        // tecla digitada no editor (o pai re-renderiza sem mudar nada aqui)
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Command, Title, Icon, Value, Click);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private async Task OnClickAsync()
        {
            if (Click.HasDelegate)
            {
                await Click.InvokeAsync();
            }
        }
    }
}
