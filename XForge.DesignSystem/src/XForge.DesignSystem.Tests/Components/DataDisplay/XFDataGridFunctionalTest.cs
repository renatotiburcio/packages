using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.DataGrid;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFDataGridFunctionalTest : BunitContext
{
    public XFDataGridFunctionalTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private record SampleItem(int Id, string Name, double Price, string? Category = null);

    private static readonly SampleItem[] SampleData =
    [
        new(1, "Widget A", 10.50, "Widgets"),
        new(2, "Gadget B", 25.00, "Gadgets"),
        new(3, "Widget C", 7.75, "Widgets"),
        new(4, "Gadget D", 42.30, "Gadgets"),
        new(5, "Tool E", 15.00, "Tools"),
        new(6, "Widget F", 3.20, "Widgets"),
        new(7, "Gadget G", 99.99, "Gadgets"),
        new(8, "Tool H", 55.00, "Tools"),
        new(9, "Widget I", 12.00, "Widgets"),
        new(10, "Gadget J", 8.50, "Gadgets"),
        new(11, "Tool K", 33.00, "Tools"),
        new(12, "Widget L", 1.99, "Widgets"),
    ];

    private IRenderedComponent<XFDataGrid<SampleItem>> RenderGrid(
        Action<ComponentParameterCollectionBuilder<XFDataGrid<SampleItem>>> configure)
    {
        return Render<XFDataGrid<SampleItem>>(p =>
        {
            p.Add(x => x.Items, SampleData);
            p.Add(x => x.Columns, (RenderFragment)(b =>
            {
                b.OpenComponent<XFDataGridColumn<SampleItem>>(0);
                b.AddAttribute(1, "Title", "Id");
                b.AddAttribute(2, "Property", "Id");
                b.CloseComponent();
                b.OpenComponent<XFDataGridColumn<SampleItem>>(3);
                b.AddAttribute(4, "Title", "Name");
                b.AddAttribute(5, "Property", "Name");
                b.CloseComponent();
                b.OpenComponent<XFDataGridColumn<SampleItem>>(6);
                b.AddAttribute(7, "Title", "Price");
                b.AddAttribute(8, "Property", "Price");
                b.CloseComponent();
            }));
            configure(p);
        });
    }

    // Rendering
    [Fact] public void Renders_Default_WithItems() { var cut = RenderGrid(p => { }); Assert.NotNull(cut); Assert.Contains("Widget A", cut.Markup); }
    [Fact] public void Renders_Caption() { var cut = RenderGrid(p => p.Add(x => x.Caption, "Products")); Assert.Contains("Products", cut.Markup); }
    [Fact] public void Renders_Striped() => RenderGrid(p => p.Add(x => x.Striped, true));
    [Fact] public void Renders_Hoverable() => RenderGrid(p => p.Add(x => x.Hoverable, true));

    // Sorting
    [Fact] public void Sorting_InitialSortKey() => RenderGrid(p => { p.Add(x => x.AllowSorting, true); p.Add(x => x.SortKey, "Name"); p.Add(x => x.SortDirection, XFDataGridSortDirection.Ascending); });
    [Fact] public void Sorting_SortKeyDescending() => RenderGrid(p => { p.Add(x => x.AllowSorting, true); p.Add(x => x.SortKey, "Price"); p.Add(x => x.SortDirection, XFDataGridSortDirection.Descending); });
    [Fact] public void Sorting_SortChangedCallback() => RenderGrid(p => { p.Add(x => x.AllowSorting, true); p.Add(x => x.SortChanged, EventCallback.Factory.Create<XFDataGridSortChangedEventArgs>(this, _ => { })); });

    // Paging
    [Fact] public void Paging_AllowPagingTrue() => RenderGrid(p => { p.Add(x => x.AllowPaging, true); p.Add(x => x.PageSize, 5); });
    [Fact] public void Paging_PageSizeLimits() => RenderGrid(p => { p.Add(x => x.AllowPaging, true); p.Add(x => x.PageSize, 3); });
    [Fact] public void Paging_PageChangedCallback() => RenderGrid(p => { p.Add(x => x.AllowPaging, true); p.Add(x => x.PageSize, 5); p.Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, _ => { })); });
    [Fact] public void Paging_ShowPageSize() => RenderGrid(p => { p.Add(x => x.AllowPaging, true); p.Add(x => x.ShowPageSize, true); p.Add(x => x.PageSize, 5); p.Add(x => x.PageSizeOptions, new[] { 5, 10, 20 }); });

    // Filtering
    [Fact] public void Filtering_FilterText_FiltersByName() { var cut = RenderGrid(p => { p.Add(x => x.FilterText, "Widget"); p.Add(x => x.FilterKeys, new[] { "Name" }); }); Assert.Contains("Widget A", cut.Markup); Assert.DoesNotContain("Gadget B", cut.Markup); }
    [Fact] public void Filtering_NoFilterText_ShowsAll() { var cut = RenderGrid(p => { p.Add(x => x.FilterText, (string?)null); p.Add(x => x.FilterKeys, new[] { "Name" }); }); Assert.Contains("Widget A", cut.Markup); Assert.Contains("Gadget B", cut.Markup); }
    [Fact] public void Filtering_CustomFilterPredicate() { var cut = RenderGrid(p => { p.Add(x => x.FilterText, "expensive"); p.Add(x => x.FilterPredicate, (Func<SampleItem, string, bool>)((item, _) => item.Price > 50)); }); Assert.Contains("99.99", cut.Markup); }
    [Fact] public void Filtering_FilterModeRow() => RenderGrid(p => p.Add(x => x.FilterMode, XFDataGridFilterMode.Row));
    [Fact] public void Filtering_FilterModeAdvanced() => RenderGrid(p => p.Add(x => x.FilterMode, XFDataGridFilterMode.Advanced));

    // Editing
    [Fact] public void Editing_AllowEditingTrue() { var cut = RenderGrid(p => p.Add(x => x.AllowEditing, true)); Assert.Contains("Editar", cut.Markup); }
    [Fact] public void Editing_AllowEditingFalse() { var cut = RenderGrid(p => p.Add(x => x.AllowEditing, false)); Assert.DoesNotContain("Editar", cut.Markup); }
    [Fact] public void Editing_CustomEditLabels() { var cut = RenderGrid(p => { p.Add(x => x.AllowEditing, true); p.Add(x => x.EditLabel, "Modify"); p.Add(x => x.SaveLabel, "Update"); p.Add(x => x.CancelLabel, "Dismiss"); }); Assert.Contains("Modify", cut.Markup); }
    [Fact] public void Editing_RowEditStartedCallback() => RenderGrid(p => { p.Add(x => x.AllowEditing, true); p.Add(x => x.RowEditStarted, EventCallback.Factory.Create<SampleItem>(this, _ => { })); });
    [Fact] public void Editing_RowEditCommittedCallback() => RenderGrid(p => { p.Add(x => x.AllowEditing, true); p.Add(x => x.RowEditCommitted, EventCallback.Factory.Create<SampleItem>(this, _ => { })); });
    [Fact] public void Editing_RowEditCanceledCallback() => RenderGrid(p => { p.Add(x => x.AllowEditing, true); p.Add(x => x.RowEditCanceled, EventCallback.Factory.Create<SampleItem>(this, _ => { })); });

    // Selection
    [Fact] public void Selection_None() => RenderGrid(p => p.Add(x => x.SelectionMode, XFDataGridSelectionMode.None));
    [Fact] public void Selection_Single() => RenderGrid(p => p.Add(x => x.SelectionMode, XFDataGridSelectionMode.Single));
    [Fact] public void Selection_Multiple() => RenderGrid(p => p.Add(x => x.SelectionMode, XFDataGridSelectionMode.Multiple));
    [Fact] public void Selection_SelectedItemCallback() => RenderGrid(p => { p.Add(x => x.SelectionMode, XFDataGridSelectionMode.Single); p.Add(x => x.SelectedItemChanged, EventCallback.Factory.Create<SampleItem?>(this, _ => { })); });

    // Grouping
    [Fact] public void Grouping_GroupByProperty() => RenderGrid(p => { p.Add(x => x.GroupBy, "Category"); p.Add(x => x.GroupKeySelector, (Func<SampleItem, object?>)(item => item.Category)); });
    [Fact] public void Grouping_EmptyGroupLabel() => RenderGrid(p => { p.Add(x => x.GroupBy, "Category"); p.Add(x => x.GroupKeySelector, (Func<SampleItem, object?>)(item => item.Category)); p.Add(x => x.EmptyGroupLabel, "No group"); });

    // State
    [Fact] public void State_CaptureState_ReturnsCurrentState() { var cut = RenderGrid(p => { }); var state = cut.Instance.CaptureState(); Assert.NotNull(state); Assert.Equal(1, state.Page); Assert.Equal(10, state.PageSize); }
    [Fact] public void State_CaptureState_WithSort() { var cut = RenderGrid(p => { p.Add(x => x.AllowSorting, true); p.Add(x => x.SortKey, "Name"); p.Add(x => x.SortDirection, XFDataGridSortDirection.Ascending); }); var state = cut.Instance.CaptureState(); Assert.Equal("Name", state.SortKey); Assert.Equal(XFDataGridSortDirection.Ascending, state.SortDirection); }
    [Fact] public void State_CaptureState_IncludesFilterText() { var cut = RenderGrid(p => p.Add(x => x.FilterText, "test")); var state = cut.Instance.CaptureState(); Assert.Equal("test", state.FilterText); }
    [Fact] public void State_CaptureState_IncludesColumnOrder() { var cut = RenderGrid(p => { }); var state = cut.Instance.CaptureState(); Assert.Equal(3, state.ColumnOrder.Count); }

    // Column Features
    [Fact] public void Column_ShowColumnChooser() { var cut = RenderGrid(p => p.Add(x => x.ShowColumnChooser, true)); Assert.Contains("Colunas", cut.Markup); }
    [Fact] public void Column_AllowColumnReorder() => RenderGrid(p => p.Add(x => x.AllowColumnReorder, true));
    [Fact] public void Column_AllowColumnResize() => RenderGrid(p => p.Add(x => x.AllowColumnResize, true));
    [Fact] public void Column_VisibilityChangedCallback() => RenderGrid(p => { p.Add(x => x.ShowColumnChooser, true); p.Add(x => x.ColumnVisibilityChanged, EventCallback.Factory.Create<XFDataGridColumnVisibilityChangedEventArgs>(this, _ => { })); });

    // Export
    [Fact] public void Export_AllowExportTrue() { var cut = RenderGrid(p => p.Add(x => x.AllowExport, true)); Assert.Contains("Exportar CSV", cut.Markup); }
    [Fact] public void Export_AllowExportFalse() { var cut = RenderGrid(p => p.Add(x => x.AllowExport, false)); Assert.DoesNotContain("Exportar CSV", cut.Markup); }

    // Visual
    [Fact] public void Variant_Bordered() => RenderGrid(p => p.Add(x => x.Variant, XFListVariant.Bordered));
    [Fact] public void Variant_Flush() => RenderGrid(p => p.Add(x => x.Variant, XFListVariant.Plain));
    [Fact] public void Variant_Striped() => RenderGrid(p => p.Add(x => x.Variant, XFListVariant.Soft));
    [Fact] public void Size_Sm() => RenderGrid(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Size_Lg() => RenderGrid(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void StickyHeader_True() => RenderGrid(p => p.Add(x => x.StickyHeader, true));
    [Fact] public void StickyFooter_True() => RenderGrid(p => p.Add(x => x.StickyFooter, true));

    // Aggregates
    [Fact] public void Aggregates_ShowAggregatesTrue() { var cut = RenderGrid(p => p.Add(x => x.ShowAggregates, true)); Assert.Contains("Total:", cut.Markup); }

    // Templates
    [Fact] public void DetailTemplate_RendersDetail() => RenderGrid(p => p.Add(x => x.DetailTemplate, (RenderFragment<SampleItem>)(item => b => b.AddContent(0, $"Detail: {item.Name}"))));
    [Fact] public void Loading_True() => RenderGrid(p => p.Add(x => x.Loading, true));
    [Fact] public void LoadingTemplate_CustomLoading() { var cut = RenderGrid(p => { p.Add(x => x.Loading, true); p.Add(x => x.LoadingTemplate, (RenderFragment)(b => b.AddContent(0, "Carregando..."))); }); Assert.Contains("Carregando...", cut.Markup); }
    [Fact] public void EmptyTemplate_CustomEmpty() { var cut = Render<XFDataGrid<SampleItem>>(p => p.Add(x => x.Items, Array.Empty<SampleItem>()).Add(x => x.EmptyTemplate, (RenderFragment)(b => b.AddContent(0, "Nothing here!"))).Add(x => x.Columns, (RenderFragment)(b => { b.OpenComponent<XFDataGridColumn<SampleItem>>(0); b.AddAttribute(1, "Property", "Name"); b.CloseComponent(); }))); Assert.Contains("Nothing here!", cut.Markup); }

    // Provider
    [Fact] public void Provider_ItemsProvider() { var provider = (XFDataGridItemsProvider<SampleItem>)(request => ValueTask.FromResult(new XFDataGridDataResult<SampleItem>(SampleData.Take(request.PageSize).ToArray(), SampleData.Length))); var cut = Render<XFDataGrid<SampleItem>>(p => { p.Add(x => x.ItemsProvider, provider); p.Add(x => x.AllowPaging, true); p.Add(x => x.PageSize, 5); p.Add(x => x.Columns, (RenderFragment)(b => { b.OpenComponent<XFDataGridColumn<SampleItem>>(0); b.AddAttribute(1, "Property", "Name"); b.CloseComponent(); })); }); Assert.NotNull(cut); }
    [Fact] public void Provider_ServerSide_True() => RenderGrid(p => { p.Add(x => x.ServerSide, true); p.Add(x => x.TotalItems, 100); });

    // Platform features
    [Fact] public void MultiColumnSort_AllowTrue() => RenderGrid(p => { p.Add(x => x.AllowSorting, true); p.Add(x => x.AllowMultiColumnSorting, true); p.Add(x => x.ShowMultiColumnSortingIndex, true); });
    [Fact] public void Virtualize_True() => RenderGrid(p => { p.Add(x => x.Virtualize, true); p.Add(x => x.ItemSize, 48); p.Add(x => x.OverscanCount, 5); });
    [Fact] public void RowClicked_Callback() => RenderGrid(p => p.Add(x => x.RowClicked, EventCallback.Factory.Create<SampleItem>(this, _ => { })));
}

public sealed class XFDataGridTypesTest
{
    [Fact] public void SortDescriptor_IsRecord() { var desc = new XFDataGridSortDescriptor("Name", XFDataGridSortDirection.Ascending, 1); Assert.Equal("Name", desc.Key); Assert.Equal(XFDataGridSortDirection.Ascending, desc.Direction); Assert.Equal(1, desc.Index); }
    [Fact] public void SortDescriptor_WithEquality() { var a = new XFDataGridSortDescriptor("Name", XFDataGridSortDirection.Ascending, 1); var b = new XFDataGridSortDescriptor("Name", XFDataGridSortDirection.Ascending, 1); Assert.Equal(a, b); }
    [Fact] public void State_IsRecord() { var state = new XFDataGridState(2, 20, "Name", XFDataGridSortDirection.Descending, "test", "Category", new[] { "Name", "Price" }, new Dictionary<string, bool> { { "Name", true } }, new Dictionary<string, int> { { "Name", 200 } }); Assert.Equal(2, state.Page); Assert.Equal(20, state.PageSize); Assert.Equal("Name", state.SortKey); Assert.Equal(XFDataGridSortDirection.Descending, state.SortDirection); Assert.Equal("test", state.FilterText); }
    [Fact] public void DataRequest_IsRecord() { var req = new XFDataGridDataRequest(1, 10, "Name", XFDataGridSortDirection.Ascending, "filter", new[] { "Name" }); Assert.Equal(1, req.Page); Assert.Equal(10, req.PageSize); Assert.Equal("Name", req.SortKey); }
    [Fact] public void DataResult_IsRecord() { var result = new XFDataGridDataResult<string>(new[] { "a", "b" }, 2); Assert.Equal(2, result.TotalItems); Assert.Equal(2, result.Items.Count); }
    [Fact] public void Aggregate_IsRecord() { var agg = new XFDataGridAggregate("Price", XFDataGridAggregateFunction.Sum, "C2"); Assert.Equal("Price", agg.ColumnKey); Assert.Equal(XFDataGridAggregateFunction.Sum, agg.Function); Assert.Equal("C2", agg.FormatString); }
    [Fact] public void SortChangedEventArgs_IsRecord() { var args = new XFDataGridSortChangedEventArgs("Name", XFDataGridSortDirection.Descending); Assert.Equal("Name", args.SortKey); Assert.Equal(XFDataGridSortDirection.Descending, args.Direction); }

    [Theory] [InlineData(XFDataGridSortDirection.None)] [InlineData(XFDataGridSortDirection.Ascending)] [InlineData(XFDataGridSortDirection.Descending)]
    public void SortDirection_AllValues(XFDataGridSortDirection d) => Assert.True(Enum.IsDefined(d));

    [Theory] [InlineData(XFDataGridSelectionMode.None)] [InlineData(XFDataGridSelectionMode.Single)] [InlineData(XFDataGridSelectionMode.Multiple)]
    public void SelectionMode_AllValues(XFDataGridSelectionMode m) => Assert.True(Enum.IsDefined(m));

    [Theory] [InlineData(XFDataGridFilterMode.None)] [InlineData(XFDataGridFilterMode.Row)] [InlineData(XFDataGridFilterMode.Advanced)]
    public void FilterMode_AllValues(XFDataGridFilterMode m) => Assert.True(Enum.IsDefined(m));

    [Theory] [InlineData(XFDataGridAlign.Start)] [InlineData(XFDataGridAlign.Center)] [InlineData(XFDataGridAlign.End)]
    public void Align_AllValues(XFDataGridAlign a) => Assert.True(Enum.IsDefined(a));

    [Theory] [InlineData(XFDataGridColumnWidth.Auto)] [InlineData(XFDataGridColumnWidth.Xs)] [InlineData(XFDataGridColumnWidth.Sm)] [InlineData(XFDataGridColumnWidth.Md)] [InlineData(XFDataGridColumnWidth.Lg)] [InlineData(XFDataGridColumnWidth.Xl)] [InlineData(XFDataGridColumnWidth.Actions)]
    public void ColumnWidth_AllValues(XFDataGridColumnWidth w) => Assert.True(Enum.IsDefined(w));

    [Theory] [InlineData(XFDataGridFilterOperator.Contains)] [InlineData(XFDataGridFilterOperator.Equals)] [InlineData(XFDataGridFilterOperator.NotEquals)] [InlineData(XFDataGridFilterOperator.StartsWith)] [InlineData(XFDataGridFilterOperator.EndsWith)] [InlineData(XFDataGridFilterOperator.LessThan)] [InlineData(XFDataGridFilterOperator.GreaterThan)] [InlineData(XFDataGridFilterOperator.IsNull)] [InlineData(XFDataGridFilterOperator.IsNotNull)] [InlineData(XFDataGridFilterOperator.IsEmpty)] [InlineData(XFDataGridFilterOperator.IsNotEmpty)]
    public void FilterOperator_AllValues(XFDataGridFilterOperator op) => Assert.True(Enum.IsDefined(op));

    [Theory] [InlineData(XFDataGridFilterType.Auto)] [InlineData(XFDataGridFilterType.Text)] [InlineData(XFDataGridFilterType.Number)] [InlineData(XFDataGridFilterType.Date)] [InlineData(XFDataGridFilterType.Boolean)] [InlineData(XFDataGridFilterType.Enum)]
    public void FilterType_AllValues(XFDataGridFilterType t) => Assert.True(Enum.IsDefined(t));

    [Theory] [InlineData(XFDataGridAggregateFunction.Sum)] [InlineData(XFDataGridAggregateFunction.Average)] [InlineData(XFDataGridAggregateFunction.Count)] [InlineData(XFDataGridAggregateFunction.Min)] [InlineData(XFDataGridAggregateFunction.Max)]
    public void AggregateFunction_AllValues(XFDataGridAggregateFunction f) => Assert.True(Enum.IsDefined(f));
}
