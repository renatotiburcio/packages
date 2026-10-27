using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.DropDownDataGrid
{
    public class XFDropDownDataGridColumn<TItem>
    {
        public string? Title { get; set; }
        public RenderFragment<TItem>? Template { get; set; }
    }
}
