using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorBold : XForgeComponentBase
    {
        [Parameter] public string Command { get; set; } = "bold";
        [Parameter] public string Title { get; set; } = "Bold";
        [Parameter] public string Icon { get; set; } = "B";
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
