using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Editors.Spreadsheet.Toolbar
{
    /// <summary>
    /// Spreadsheet toolbar container with flex layout.
    /// Holds child toolbar tool components and arranges them in a horizontal flex row.
    /// </summary>
    public partial class XFSpreadsheetToolbar : XForgeComponentBase
    {
        /// <summary>
        /// Child toolbar tool components to render inside the toolbar container.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override string BaseClass => "xf-spreadsheet-toolbar flex items-center gap-1 px-2 py-1 border-b border-surface-200 bg-bg-surface";
    }
}
