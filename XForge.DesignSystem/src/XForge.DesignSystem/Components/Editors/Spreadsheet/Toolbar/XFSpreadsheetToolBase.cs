using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Abstract base class for all spreadsheet toolbar tool components.
    /// Provides common parameters (Command, Title, Icon, Disabled, Click) and command execution logic.
    /// </summary>
    public abstract class XFSpreadsheetToolBase : XForgeComponentBase
    {
        /// <summary>
        /// The command identifier for this toolbar tool.
        /// </summary>
        [Parameter]
        public string Command { get; set; } = string.Empty;

        /// <summary>
        /// The tooltip title displayed when hovering over the tool button.
        /// </summary>
        [Parameter]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The Material icon name or icon fragment to display inside the button.
        /// </summary>
        [Parameter]
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Callback invoked when the tool button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback Click { get; set; }

        /// <summary>
        /// Executes the command asynchronously by invoking the Click callback.
        /// </summary>
        protected async Task ExecuteCommandAsync()
        {
            if (Click.HasDelegate)
            {
                await Click.InvokeAsync();
            }
        }
    }
}
