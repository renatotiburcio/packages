using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    public partial class XFRichTextEditorUnorderedList : XForgeComponentBase
    {
        [Parameter] public string Command { get; set; } = "insertUnorderedList";
        [Parameter] public string Title { get; set; } = "Unordered List";
        [Parameter] public string Icon { get; set; } = "•";
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
