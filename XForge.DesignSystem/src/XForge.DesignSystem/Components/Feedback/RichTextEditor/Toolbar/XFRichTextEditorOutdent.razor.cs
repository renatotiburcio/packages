using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for decreasing indentation in the rich text editor.
    /// Executes the <c>outdent</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorOutdent : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"outdent"</c>.</summary>
        [Parameter] public string Command { get; set; } = "outdent";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "Decrease Indent";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "format_indent_decrease";

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
