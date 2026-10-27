using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorUnlink : XForgeComponentBase
    {
        [Parameter] public string Command { get; set; } = "unlink";
        [Parameter] public string Title { get; set; } = "Remove Link";
        [Parameter] public string Icon { get; set; } = "🔗";
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
