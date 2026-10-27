using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar
{
    /// <summary>
    /// Toolbar button for inserting an image into the rich text editor.
    /// Executes the <c>insertImage</c> editor command.
    /// </summary>
    public partial class XFRichTextEditorInsertImage : XForgeComponentBase
    {
        /// <summary>The editor command to execute on click. Defaults to <c>"insertImage"</c>.</summary>
        [Parameter] public string Command { get; set; } = "insertImage";

        /// <summary>Tooltip text displayed on hover.</summary>
        [Parameter] public string Title { get; set; } = "Insert Image";

        /// <summary>Icon markup or text rendered inside the button.</summary>
        [Parameter] public string Icon { get; set; } = "image";

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
