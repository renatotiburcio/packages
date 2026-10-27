using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorStrikeThrough : XForgeComponentBase
    {
        [Parameter] public string Command { get; set; } = "strikethrough";
        [Parameter] public string Title { get; set; } = "Strikethrough";
        [Parameter] public string Icon { get; set; } = "S";
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
