using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorOrderedList : XForgeComponentBase
    {
        [Parameter] public string Command { get; set; } = "insertOrderedList";
        [Parameter] public string Title { get; set; } = "Ordered List";
        [Parameter] public string Icon { get; set; } = "1.";
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
