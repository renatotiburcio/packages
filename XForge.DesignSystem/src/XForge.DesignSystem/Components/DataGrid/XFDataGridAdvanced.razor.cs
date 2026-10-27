using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.DataGrid
{
    public partial class XFDataGridAdvanced : XForgeComponentBase
    {
        [Parameter] public bool ServerSide { get; set; }
        [Parameter] public bool VirtualScrolling { get; set; }
        [Parameter] public bool ColumnChooser { get; set; } = true;
        [Parameter] public bool ColumnReorder { get; set; } = true;
        [Parameter] public bool ColumnResize { get; set; } = true;
        [Parameter] public bool MasterDetail { get; set; } = false;
        [Parameter] public bool InlineEdit { get; set; } = true;
        [Parameter] public bool ExcelFilters { get; set; } = false;
        [Parameter] public bool PrintMode { get; set; } = false;
        [Parameter] public int Page { get; set; } = 1;
        [Parameter] public int PageSize { get; set; } = 25;
        [Parameter] public int TotalCount { get; set; }
        [Parameter] public int VisibleCount { get; set; }
        [Parameter] public EventCallback<int> PageChanged { get; set; }

        private RenderFragment RenderHeader() => __builder => { };
        private RenderFragment RenderBody() => __builder => { };
        private RenderFragment RenderPager() => __builder => { };
    }
}
