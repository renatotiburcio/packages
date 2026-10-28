using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.DataGrid;

namespace XForge.DesignSystem.Tests.Components;

public class XFDataGridColumnComponentTest : XForgeTestContext
{
    public XFDataGridColumnComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_AsChildOfDataGrid_RegistersColumn()
    {
        var cut = Render<XFDataGrid<object>>(parameters => parameters
            .Add(p => p.Items, Array.Empty<object>())
            .AddChildContent<XFDataGridColumn<object>>(col => col
                .Add(c => c.Title, "ColumnTitle")
                .Add(c => c.Property, "Name")));
        Assert.Contains("ColumnTitle", cut.Markup);
    }

    [Fact]
    public void Render_ColumnWithSortKey_SortableEnabled()
    {
        var cut = Render<XFDataGrid<object>>(parameters => parameters
            .Add(p => p.Items, Array.Empty<object>())
            .AddChildContent<XFDataGridColumn<object>>(col => col
                .Add(c => c.Title, "Sortable")
                .Add(c => c.SortKey, "sort-field")));
        Assert.Contains("Sortable", cut.Markup);
    }

    [Fact]
    public void Render_ColumnWhenNotVisible_DoesNotShowHeader()
    {
        var cut = Render<XFDataGrid<object>>(parameters => parameters
            .Add(p => p.Items, Array.Empty<object>())
            .AddChildContent<XFDataGridColumn<object>>(col => col
                .Add(c => c.Title, "Invisible")
                .Add(c => c.Visible, false)));
        Assert.DoesNotContain("Invisible", cut.Markup);
    }
}