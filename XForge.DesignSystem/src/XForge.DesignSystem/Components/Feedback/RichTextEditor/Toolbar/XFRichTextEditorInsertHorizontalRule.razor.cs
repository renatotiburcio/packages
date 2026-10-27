using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for inserting a horizontal rule (separator line) in the rich text editor.
    /// Executes the <c>insertHorizontalRule</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorInsertHorizontalRule : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"insertHorizontalRule"</c>.</summary>
        [Parameter] public string Command { get; set; } = "insertHorizontalRule";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "Insert Horizontal Rule";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "horizontal_rule";

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
