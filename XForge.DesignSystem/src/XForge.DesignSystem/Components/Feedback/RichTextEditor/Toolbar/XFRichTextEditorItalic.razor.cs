using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorItalic : XForgeComponentBase
    {
        [Parameter] public string Command { get; set; } = "italic";
        [Parameter] public string Title { get; set; } = "Italic";
        [Parameter] public string Icon { get; set; } = "I";
        [Parameter] public EventCallback Click { get; set; }

        private async Task OnClickAsync()
        {
            if (Click.HasDelegate)
            {
                await Click.InvokeAsync();
            }
        }
    }
}
