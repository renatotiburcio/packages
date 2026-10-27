using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for inserting a table into the rich text editor.
    /// Executes the <c>insertTable</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorInsertTable : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"insertTable"</c>.</summary>
        [Parameter] public string Command { get; set; } = "insertTable";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "Insert Table";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "table_chart";

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
