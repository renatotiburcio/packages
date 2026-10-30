using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
    public partial class XFDataGridColumn<TItem> : IDisposable
    {
        [CascadingParameter] internal XFDataGrid<TItem>? Grid { get; set; }

        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Property { get; set; }
        [Parameter] public string? SortKey { get; set; }
        [Parameter] public string? FormatString { get; set; }
        [Parameter] public bool Sortable { get; set; } = true;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Hideable { get; set; } = true;
        [Parameter] public bool Resizable { get; set; } = true;
        [Parameter] public int? WidthPixels { get; set; }
        [Parameter] public int MinWidthPixels { get; set; } = 80;
        [Parameter] public int MaxWidthPixels { get; set; } = 640;
        [Parameter] public int ResizeStepPixels { get; set; } = 24;
        [Parameter] public bool Sticky { get; set; }
        [Parameter] public XFDataGridStickySide StickySide { get; set; } = XFDataGridStickySide.Start;
        [Parameter] public XFDataGridStickyOffset StickyOffset { get; set; } = XFDataGridStickyOffset.None;
        [Parameter] public XFDataGridAlign Align { get; set; } = XFDataGridAlign.Start;
        [Parameter] public XFDataGridColumnWidth Width { get; set; } = XFDataGridColumnWidth.Auto;
        [Parameter] public RenderFragment? HeaderTemplate { get; set; }
        [Parameter] public RenderFragment<TItem>? Template { get; set; }
        [Parameter] public RenderFragment? FooterTemplate { get; set; }
        [Parameter] public bool Filterable { get; set; } = true;
        [Parameter] public XFDataGridFilterOperator FilterOperator { get; set; } = XFDataGridFilterOperator.Contains;
        [Parameter] public string? FilterValue { get; set; }
        [Parameter] public XFDataGridFilterType FilterType { get; set; } = XFDataGridFilterType.Auto;
        [Parameter] public RenderFragment<XFDataGridColumn<TItem>>? FilterTemplate { get; set; }
        [Parameter] public bool Aggregatable { get; set; } = false;
        [Parameter] public XFDataGridAggregateFunction AggregateFunction { get; set; } = XFDataGridAggregateFunction.Sum;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Title, Property, SortKey, FormatString, Sortable, Visible, Hideable, Resizable, WidthPixels, MinWidthPixels, MaxWidthPixels, ResizeStepPixels, Sticky, StickySide, StickyOffset, Align, Width, HeaderTemplate, Template, FooterTemplate, Filterable, FilterOperator, FilterValue, FilterType, FilterTemplate, Aggregatable, AggregateFunction);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        internal string ResolvedTitle => !string.IsNullOrWhiteSpace(Title) ? Title! : Property ?? "";

        internal string? ResolvedSortKey => !string.IsNullOrWhiteSpace(SortKey) ? SortKey : Property;

        internal string ResolvedKey => !string.IsNullOrWhiteSpace(Property) ? Property! : ResolvedTitle;

        protected override void OnInitialized()
        {
            Grid?.Register(this);
        }

        public void Dispose()
        {
            Grid?.Unregister(this);
        }
    }
}
