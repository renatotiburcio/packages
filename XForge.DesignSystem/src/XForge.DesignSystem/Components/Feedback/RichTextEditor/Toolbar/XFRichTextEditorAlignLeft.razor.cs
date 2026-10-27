using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for left-aligning text in the rich text editor.
    /// Executes the <c>justifyLeft</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorAlignLeft : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"justifyLeft"</c>.</summary>
        [Parameter] public string Command { get; set; } = "justifyLeft";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "Align Left";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "align_left";

        /// <summary>Callback invoked when the button is clicked.</summary>
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
