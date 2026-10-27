using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for center-aligning text in the rich text editor.
    /// Executes the <c>justifyCenter</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorAlignCenter : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"justifyCenter"</c>.</summary>
        [Parameter] public string Command { get; set; } = "justifyCenter";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "Align Center";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "align_center";

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
