using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataGrid
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

        // assinatura de render: wrapper fino — o estado interativo vive no componente
        // interno; aqui o render e funcao dos parametros
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ServerSide, VirtualScrolling, ColumnChooser, ColumnReorder, ColumnResize, MasterDetail, InlineEdit, ExcelFilters, PrintMode, Page, PageSize, TotalCount, VisibleCount);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private RenderFragment RenderHeader() => __builder => { };
        private RenderFragment RenderBody() => __builder => { };
        private RenderFragment RenderPager() => __builder => { };
    }
}
