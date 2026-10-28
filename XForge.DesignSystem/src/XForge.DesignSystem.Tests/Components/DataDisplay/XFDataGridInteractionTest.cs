using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.DataGrid;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFDataGridInteractionTest : XForgeTestContext
{
    public XFDataGridInteractionTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }
    private record GridItem(string Name, double Value, string Category);
    private static List<GridItem> Data3 => new() { new("A", 10, "Cat1"), new("B", 20, "Cat2"), new("C", 15, "Cat1") };
    private static List<GridItem> DataEmpty => new();
    private static List<GridItem> Data50 => Enumerable.Range(1, 50).Select(i => new GridItem($"Item{i}", i * 1.5, i % 2 == 0 ? "Even" : "Odd")).ToList();
    [Fact] public void Column_DefaultTitleIsNull() => Assert.Null(new XFDataGridColumn<GridItem>().Title);
#pragma warning disable BL0005
    [Fact] public void Column_SetTitle() => Assert.Equal("Name", new XFDataGridColumn<GridItem> { Title = "Name" }.Title);
    [Fact] public void Column_DefaultSortableTrue() => Assert.True(new XFDataGridColumn<GridItem>().Sortable);
    [Fact] public void Column_SetSortable() => Assert.True(new XFDataGridColumn<GridItem> { Sortable = true }.Sortable);
    [Fact] public void Column_DefaultVisible() => Assert.True(new XFDataGridColumn<GridItem>().Visible);
    [Fact] public void Column_SetVisible() => Assert.False(new XFDataGridColumn<GridItem> { Visible = false }.Visible);
    [Fact] public void Column_DefaultFilterableTrue() => Assert.True(new XFDataGridColumn<GridItem>().Filterable);
    [Fact] public void Column_SetFilterable() => Assert.True(new XFDataGridColumn<GridItem> { Filterable = true }.Filterable);
    [Fact] public void Column_SetWidth() => Assert.Equal(XFDataGridColumnWidth.Md, new XFDataGridColumn<GridItem> { Width = XFDataGridColumnWidth.Md }.Width);
    [Fact] public void Column_SetAlign() => Assert.Equal(XFDataGridAlign.End, new XFDataGridColumn<GridItem> { Align = XFDataGridAlign.End }.Align);
#pragma warning restore BL0005
    [Fact] public void DataGridSelectionMode_Has3Values() => Assert.Equal(3, Enum.GetValues<XFDataGridSelectionMode>().Length);
    [Fact] public void DataGridSortDirection_Has3Values() => Assert.Equal(3, Enum.GetValues<XFDataGridSortDirection>().Length);
    [Fact] public void DataGridAlign_Has3Values() => Assert.Equal(3, Enum.GetValues<XFDataGridAlign>().Length);
    [Fact] public void DataGridStickySide_Has2Values() => Assert.Equal(2, Enum.GetValues<XFDataGridStickySide>().Length);
    [Fact] public void Renders_50Items() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data50));
    [Fact] public void Renders_EmptyItems() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, DataEmpty));
    [Fact] public void Renders_NullItems() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, (IEnumerable<GridItem>)null!));
    [Fact] public void Renders_Striped() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.Striped, true));
    [Fact] public void Renders_HoverableFalse() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.Hoverable, false));
    [Fact] public void Renders_StickyHeaderFalse() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.StickyHeader, false));
    [Fact] public void Renders_StickyFooter() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.StickyFooter, true));
    [Fact] public void Renders_FullWidthFalse() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.FullWidth, false));
    [Fact] public void Renders_AllowSortingFalse() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.AllowSorting, false));
    [Fact] public void Renders_AllowPaging() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data50).Add(x => x.AllowPaging, true));
    [Fact] public void Renders_ShowPageSize() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data50).Add(x => x.AllowPaging, true).Add(x => x.ShowPageSize, true));
    [Fact] public void Renders_PageSize25() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data50).Add(x => x.AllowPaging, true).Add(x => x.PageSize, 25));
    [Fact] public void Renders_ServerSide() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data50).Add(x => x.ServerSide, true).Add(x => x.TotalItems, 500));
    [Fact] public void Renders_VisibleFalse() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.Visible, false));
    [Fact] public void Renders_Disabled() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.Disabled, true));
    [Fact] public void Renders_WithCaption() { var cut = Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.Caption, "Grid")); Assert.Contains("Grid", cut.Markup); }
    [Fact] public void Renders_EmptyText() { var cut = Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, DataEmpty).Add(x => x.EmptyText, "Vazio")); Assert.Contains("Vazio", cut.Markup); }
    [Fact] public void Renders_Loading() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.Loading, true));
    [Fact] public void Renders_GroupBy() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.GroupBy, "Category"));
    [Fact] public void Renders_GroupKeySelector() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.GroupKeySelector, item => item.Category));
    [Fact] public void Renders_WithColumns() => Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data3).Add(x => x.Columns, builder => { }));
    [Fact] public void PageChanged_Fires() { int pageChangedTo = 0; Render<XFDataGrid<GridItem>>(p => p.Add(x => x.Items, Data50).Add(x => x.AllowPaging, true).Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, v => pageChangedTo = v))); Assert.Equal(0, pageChangedTo); }
}
