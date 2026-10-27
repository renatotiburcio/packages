using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for toggling the HTML source view in the rich text editor.
    /// Executes the <c>viewSource</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorViewSource : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"viewSource"</c>.</summary>
        [Parameter] public string Command { get; set; } = "viewSource";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "View Source";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "code";

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
