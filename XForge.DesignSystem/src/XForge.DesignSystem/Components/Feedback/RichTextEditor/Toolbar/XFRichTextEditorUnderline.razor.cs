using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorUnderline : XForgeComponentBase
    {
        [Parameter] public string Command { get; set; } = "underline";
        [Parameter] public string Title { get; set; } = "Underline";
        [Parameter] public string Icon { get; set; } = "U";
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
