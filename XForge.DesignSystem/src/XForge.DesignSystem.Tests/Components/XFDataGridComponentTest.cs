using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.DataGrid;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFDataGridComponentTest : XForgeTestContext
{
    public XFDataGridComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_RendersColumnsAndRows()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Equal(2, cut.FindAll("th").Count);
        Assert.Contains("Finance", cut.Markup);
        Assert.Contains("98", cut.Markup);
        Assert.Contains("px-4 py-3", cut.Markup);
        Assert.Contains("min-w-32", cut.Markup);
        Assert.DoesNotContain(" class=\"- ", cut.Markup);
        Assert.DoesNotContain(" class=\"-\"", cut.Markup);
    }

    [Fact]
    public void Render_Empty_RendersEmptyText()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<Row>());
            parameters.Add(p => p.EmptyText, "Sem registros");
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("Sem registros", cut.Markup);
    }

    [Fact]
    public void Render_Loading_RendersSkeletonRowsAndBusyState()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Loading, true);
            parameters.Add(p => p.PageSize, 2);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Equal("true", cut.Find("table").GetAttribute("aria-busy"));
        Assert.Equal(3, cut.FindAll("tbody tr").Count);
        Assert.Contains("animate-pulse", cut.Markup);
        Assert.DoesNotContain("Finance", cut.Markup);
    }

    [Fact]
    public void Render_ControlledSort_OrdersRows()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.SortKey, nameof(Row.Score));
            parameters.Add(p => p.SortDirection, XFDataGridSortDirection.Descending);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        var firstCell = cut.Find("tbody tr td");
        Assert.Contains("Finance", firstCell.TextContent);
    }

    [Fact]
    public void Click_Header_RaisesSortChanged()
    {
        XFDataGridSortChangedEventArgs? args = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Columns, BasicColumns());
            parameters.Add(p => p.SortChanged, value => args = value);
        });

        cut.Find("th button").Click();

        Assert.NotNull(args);
        Assert.Equal(nameof(Row.Name), args!.SortKey);
        Assert.Equal(XFDataGridSortDirection.Ascending, args.Direction);
        Assert.Equal("ascending", cut.Find("th").GetAttribute("aria-sort"));
        Assert.Equal("Ordenar Workspace decrescente", cut.Find("th button").GetAttribute("aria-label"));
        Assert.DoesNotContain("â", cut.Markup);
    }

    [Fact]
    public void Render_ControlledSort_UsesSemanticAriaSortOnHeader()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.SortKey, nameof(Row.Score));
            parameters.Add(p => p.SortDirection, XFDataGridSortDirection.Descending);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Equal("descending", cut.FindAll("th")[1].GetAttribute("aria-sort"));
        Assert.Equal("Remover ordenacao de Score", cut.FindAll("th button")[1].GetAttribute("aria-label"));
        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("width=\"10\"", cut.Markup);
        Assert.Contains("height=\"10\"", cut.Markup);
        Assert.Contains("viewBox", cut.FindAll("th span").Last().InnerHtml);
    }

    [Fact]
    public void Render_Paging_LimitsRows()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.AllowPaging, true);
            parameters.Add(p => p.PageSize, 1);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Single(cut.FindAll("tbody tr"));
        Assert.Contains("xf-pagination", cut.Markup);
    }

    [Fact]
    public void Render_StickyAndViewport_AddsClasses()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.ViewportRows, 10);
            parameters.Add(p => p.StickyFooter, true);
            parameters.Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<XFDataGridColumn<Row>>(0);
                builder.AddAttribute(1, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Name));
                builder.AddAttribute(2, nameof(XFDataGridColumn<Row>.Sticky), true);
                builder.AddAttribute(3, nameof(XFDataGridColumn<Row>.FooterTemplate), (RenderFragment)(footerBuilder => footerBuilder.AddContent(0, "Total")));
                builder.CloseComponent();
            });
        });

        Assert.DoesNotContain("min-h-[34rem]", cut.Markup);
        Assert.Contains("max-h-[34rem]", cut.Markup);
        Assert.Contains("overflow-auto", cut.Find(".xf-datagrid").ParentElement!.GetAttribute("class"));
        Assert.Contains("sticky", cut.Find("th").GetAttribute("class"));
        Assert.Contains("top-0", cut.Find("th").GetAttribute("class"));
        Assert.Contains("left-0", cut.Find("th").GetAttribute("class"));
        Assert.Contains("sticky", cut.Find("tfoot td").GetAttribute("class"));
        Assert.Contains("bottom-0", cut.Find("tfoot td").GetAttribute("class"));
    }

    [Fact]
    public void Render_Virtualize_AddsVirtualizedBodyContract()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Virtualize, true);
            parameters.Add(p => p.ItemSize, 40);
            parameters.Add(p => p.OverscanCount, 5);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Equal("true", cut.Find("tbody").GetAttribute("data-xf-datagrid-virtualized"));
        Assert.Contains("Finance", cut.Markup);
    }

    [Fact]
    public void Render_Virtualize_WithPaging_UsesPagedItems()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Virtualize, true);
            parameters.Add(p => p.AllowPaging, true);
            parameters.Add(p => p.PageSize, 1);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("Finance", cut.Markup);
        Assert.DoesNotContain("Billing", cut.Markup);
        Assert.Contains("xf-pagination", cut.Markup);
    }

    [Fact]
    public void Click_ColumnChooser_TogglesColumnVisibility()
    {
        XFDataGridColumnVisibilityChangedEventArgs? args = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.ShowColumnChooser, true);
            parameters.Add(p => p.ColumnVisibilityChanged, value => args = value);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.Find("button[aria-label='Ocultar Score']").Click();

        Assert.NotNull(args);
        Assert.Equal(nameof(Row.Score), args!.ColumnKey);
        Assert.False(args.Visible);
        Assert.Single(cut.FindAll("th"));
        Assert.DoesNotContain("98", cut.Markup);
        Assert.Equal("false", cut.Find("button[aria-label='Mostrar Score']").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Render_ColumnChooser_ExposesToolbarAndPreventsHidingLastColumn()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.ShowColumnChooser, true);
            parameters.Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<XFDataGridColumn<Row>>(0);
                builder.AddAttribute(1, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Name));
                builder.AddAttribute(2, nameof(XFDataGridColumn<Row>.Title), "Workspace");
                builder.CloseComponent();
            });
        });

        var chooser = cut.Find("[role='toolbar']");
        Assert.Equal("Colunas", chooser.GetAttribute("aria-label"));
        Assert.Equal("true", cut.Find("button[aria-label='Ocultar Workspace']").GetAttribute("aria-pressed"));
        Assert.True(cut.Find("button[aria-label='Ocultar Workspace']").HasAttribute("disabled"));
    }

    [Fact]
    public void Click_ColumnReorder_MovesColumnAndRaisesEvent()
    {
        XFDataGridColumnReorderedEventArgs? args = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.AllowColumnReorder, true);
            parameters.Add(p => p.ColumnReordered, value => args = value);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.Find("button[aria-label='Mover Workspace para direita']").Click();

        Assert.NotNull(args);
        Assert.Equal(nameof(Row.Name), args!.ColumnKey);
        Assert.Equal(0, args.OldIndex);
        Assert.Equal(1, args.NewIndex);
        Assert.Contains("Score", cut.FindAll("th")[0].TextContent);
        Assert.Contains("Workspace", cut.FindAll("th")[1].TextContent);
        Assert.Contains("98", cut.Find("tbody tr td").TextContent);
    }

    [Fact]
    public void Render_ColumnOperations_ExposeLabelsAndDisabledBoundaries()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.AllowColumnReorder, true);
            parameters.Add(p => p.AllowColumnResize, true);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.True(cut.Find("button[aria-label='Mover Workspace para esquerda']").HasAttribute("disabled"));
        Assert.False(cut.Find("button[aria-label='Mover Workspace para direita']").HasAttribute("disabled"));
        Assert.False(cut.Find("button[aria-label='Aumentar largura de Workspace']").HasAttribute("disabled"));
        Assert.Contains("svg", cut.Find("button[aria-label='Mover Workspace para direita']").InnerHtml);
    }

    [Fact]
    public void Render_ColumnAndSortOperations_UseCustomLocalizedLabels()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.ShowColumnChooser, true);
            parameters.Add(p => p.AllowColumnReorder, true);
            parameters.Add(p => p.AllowColumnResize, true);
            parameters.Add(p => p.HideColumnLabelFormat, "Hide {0}");
            parameters.Add(p => p.ShowColumnLabelFormat, "Show {0}");
            parameters.Add(p => p.MoveColumnLeftLabelFormat, "Move {0} left");
            parameters.Add(p => p.MoveColumnRightLabelFormat, "Move {0} right");
            parameters.Add(p => p.DecreaseColumnWidthLabelFormat, "Shrink {0}");
            parameters.Add(p => p.IncreaseColumnWidthLabelFormat, "Grow {0}");
            parameters.Add(p => p.SortAscendingLabelFormat, "Sort {0} ascending");
            parameters.Add(p => p.SortDescendingLabelFormat, "Sort {0} descending");
            parameters.Add(p => p.ClearSortLabelFormat, "Clear {0} sort");
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Equal("Hide Workspace", cut.Find("button[aria-label='Hide Workspace']").GetAttribute("aria-label"));
        Assert.NotNull(cut.Find("button[aria-label='Sort Workspace ascending']"));
        Assert.True(cut.Find("button[aria-label='Move Workspace left']").HasAttribute("disabled"));
        Assert.False(cut.Find("button[aria-label='Move Workspace right']").HasAttribute("disabled"));
        Assert.NotNull(cut.Find("button[aria-label='Shrink Workspace']"));
        Assert.NotNull(cut.Find("button[aria-label='Grow Workspace']"));

        cut.Find("th button[aria-label='Sort Workspace ascending']").Click();

        Assert.NotNull(cut.Find("th button[aria-label='Sort Workspace descending']"));
    }

    [Fact]
    public void Click_ColumnResize_UpdatesColumnWidthAndRaisesEvent()
    {
        XFDataGridColumnResizedEventArgs? args = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.AllowColumnResize, true);
            parameters.Add(p => p.ColumnResized, value => args = value);
            parameters.Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<XFDataGridColumn<Row>>(0);
                builder.AddAttribute(1, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Name));
                builder.AddAttribute(2, nameof(XFDataGridColumn<Row>.Title), "Workspace");
                builder.AddAttribute(3, nameof(XFDataGridColumn<Row>.WidthPixels), 160);
                builder.AddAttribute(4, nameof(XFDataGridColumn<Row>.ResizeStepPixels), 32);
                builder.CloseComponent();
            });
        });

        cut.Find("button[aria-label='Aumentar largura de Workspace']").Click();

        Assert.NotNull(args);
        Assert.Equal(nameof(Row.Name), args!.ColumnKey);
        Assert.Equal(192, args.WidthPixels);
        Assert.Contains("width: 192px; min-width: 192px;", cut.Find("th").GetAttribute("style"));
        Assert.Contains("width: 192px; min-width: 192px;", cut.Find("tbody td").GetAttribute("style"));
    }

    [Fact]
    public void Render_GroupByProperty_RendersGroupHeadersAndRows()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.GroupBy, nameof(Row.Status));
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("Review", cut.Markup);
        Assert.Contains("Blocked", cut.Markup);
        Assert.Contains("Stable", cut.Markup);
        Assert.Contains("(1)", cut.Markup);
        Assert.Equal(6, cut.FindAll("tbody tr").Count);
    }

    [Fact]
    public void Render_GroupTemplates_ReceiveGroupContext()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.GroupKeySelector, row => row.Score >= 90 ? "High" : "Normal");
            parameters.Add(p => p.GroupHeaderTemplate, group => builder => builder.AddContent(0, $"Group {group.Label}: {group.Count}"));
            parameters.Add(p => p.GroupFooterTemplate, group => builder => builder.AddContent(0, $"Footer {group.Label}: {group.Items.Count}"));
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("Group High: 1", cut.Markup);
        Assert.Contains("Group Normal: 2", cut.Markup);
        Assert.Contains("Footer High: 1", cut.Markup);
        Assert.Contains("Footer Normal: 2", cut.Markup);
    }

    [Fact]
    public void Render_GroupByNullKey_UsesCustomEmptyGroupLabel()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows.Take(1));
            parameters.Add(p => p.GroupKeySelector, _ => null);
            parameters.Add(p => p.EmptyGroupLabel, "No group");
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("No group", cut.Markup);
        Assert.DoesNotContain("Sem grupo", cut.Markup);
    }

    [Fact]
    public void Render_DetailTemplate_RendersDetailRows()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows.Take(2));
            parameters.Add(p => p.DetailTemplate, row => builder => builder.AddContent(0, $"Status: {row.Status}"));
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("Status: Review", cut.Markup);
        Assert.Contains("Status: Blocked", cut.Markup);
        Assert.Equal(4, cut.FindAll("tbody tr").Count);
        Assert.Equal("2", cut.FindAll("tbody tr")[1].QuerySelector("td")!.GetAttribute("colspan"));
    }

    [Fact]
    public void Click_EditActions_RendersEditTemplateAndRaisesEvents()
    {
        Row? started = null;
        Row? committed = null;
        Row? editing = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows.Take(1));
            parameters.Add(p => p.AllowEditing, true);
            parameters.Add(p => p.EditTemplate, row => builder => builder.AddContent(0, $"Editing {row.Name}"));
            parameters.Add(p => p.EditingItemChanged, value => editing = value);
            parameters.Add(p => p.RowEditStarted, value => started = value);
            parameters.Add(p => p.RowEditCommitted, value => committed = value);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.Find("button[aria-label='Editar Finance']").Click();

        Assert.Equal(Rows[0], started);
        Assert.Equal(Rows[0], editing);
        Assert.Contains("Editing Finance", cut.Markup);
        Assert.Equal(3, cut.FindAll("th").Count);

        cut.Find("button").ParentElement!.QuerySelectorAll("button");
        cut.FindAll("button").First(button => button.TextContent.Contains("Salvar", StringComparison.Ordinal)).Click();

        Assert.Equal(Rows[0], committed);
        Assert.Null(editing);
        Assert.DoesNotContain("Editing Finance", cut.Markup);
    }

    [Fact]
    public void Render_EditActions_ExposeContextualSaveAndCancelLabels()
    {
        Row? canceled = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows.Take(1));
            parameters.Add(p => p.AllowEditing, true);
            parameters.Add(p => p.EditTemplate, row => builder => builder.AddContent(0, $"Editing {row.Name}"));
            parameters.Add(p => p.RowEditCanceled, value => canceled = value);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.Find("button[aria-label='Editar Finance']").Click();

        Assert.NotNull(cut.Find("button[aria-label='Salvar Finance']"));
        cut.Find("button[aria-label='Cancelar Finance']").Click();

        Assert.Equal(Rows[0], canceled);
        Assert.DoesNotContain("Editing Finance", cut.Markup);
    }

    [Fact]
    public void Render_EditActions_UseCustomLocalizedActionLabelFormats()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows.Take(1));
            parameters.Add(p => p.AllowEditing, true);
            parameters.Add(p => p.EditActionLabelFormat, "{1}: {0}");
            parameters.Add(p => p.SaveActionLabelFormat, "{1}: {0}");
            parameters.Add(p => p.CancelActionLabelFormat, "{1}: {0}");
            parameters.Add(p => p.EditTemplate, row => builder => builder.AddContent(0, $"Editing {row.Name}"));
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.Find("button[aria-label='Finance: Editar']").Click();

        Assert.NotNull(cut.Find("button[aria-label='Finance: Salvar']"));
        Assert.NotNull(cut.Find("button[aria-label='Finance: Cancelar']"));
    }

    [Fact]
    public void Render_LoadingAndEmpty_WithEditing_UseFullColumnSpan()
    {
        var loading = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.AllowEditing, true);
            parameters.Add(p => p.Loading, true);
            parameters.Add(p => p.LoadingTemplate, builder => builder.AddContent(0, "Loading"));
            parameters.Add(p => p.Columns, BasicColumns());
        });

        var empty = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<Row>());
            parameters.Add(p => p.AllowEditing, true);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Equal("3", loading.Find("tbody td").GetAttribute("colspan"));
        Assert.Equal("3", empty.Find("tbody td").GetAttribute("colspan"));
    }

    [Fact]
    public void CaptureState_ReturnsCurrentGridState()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.FilterText, "review");
            parameters.Add(p => p.GroupBy, nameof(Row.Status));
            parameters.Add(p => p.SortKey, nameof(Row.Score));
            parameters.Add(p => p.SortDirection, XFDataGridSortDirection.Descending);
            parameters.Add(p => p.ShowColumnChooser, true);
            parameters.Add(p => p.AllowColumnResize, true);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.Find("button[aria-label='Ocultar Score']").Click();
        var state = cut.Instance.CaptureState();

        Assert.Equal(nameof(Row.Score), state.SortKey);
        Assert.Equal(XFDataGridSortDirection.Descending, state.SortDirection);
        Assert.Equal("review", state.FilterText);
        Assert.Equal(nameof(Row.Status), state.GroupBy);
        Assert.Equal([nameof(Row.Name), nameof(Row.Score)], state.ColumnOrder);
        Assert.False(state.ColumnVisibility[nameof(Row.Score)]);
    }

    [Fact]
    public async Task ApplyStateAsync_RestoresColumnStateAndSort()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.AllowColumnResize, true);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        var state = new XFDataGridState(
            1,
            10,
            nameof(Row.Score),
            XFDataGridSortDirection.Descending,
            null,
            null,
            [nameof(Row.Score), nameof(Row.Name)],
            new Dictionary<string, bool>
            {
                [nameof(Row.Name)] = true,
                [nameof(Row.Score)] = true
            },
            new Dictionary<string, int>
            {
                [nameof(Row.Score)] = 188
            });

        await cut.Instance.ApplyStateAsync(state);
        cut.Render();

        Assert.Contains("Score", cut.FindAll("th")[0].TextContent);
        Assert.Contains("Workspace", cut.FindAll("th")[1].TextContent);
        Assert.Contains("width: 188px; min-width: 188px;", cut.Find("th").GetAttribute("style"));
        Assert.Equal("descending", cut.Find("th").GetAttribute("aria-sort"));
    }

    [Fact]
    public void Click_Row_SingleSelection_RaisesSelectedItemChanged()
    {
        Row? selected = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.SelectionMode, XFDataGridSelectionMode.Single);
            parameters.Add(p => p.SelectedItemChanged, value => selected = value);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.FindAll("tbody tr")[1].Click();

        Assert.Equal(Rows[1], selected);
        Assert.Equal("true", cut.FindAll("tbody tr")[1].GetAttribute("aria-selected"));
    }

    [Fact]
    public void Click_Row_MultipleSelection_TogglesSelectedItems()
    {
        IReadOnlyList<Row> selected = [];
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.SelectionMode, XFDataGridSelectionMode.Multiple);
            parameters.Add(p => p.SelectedItemsChanged, value => selected = value);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.FindAll("tbody tr")[0].Click();
        cut.FindAll("tbody tr")[2].Click();

        Assert.Equal([Rows[0], Rows[2]], selected);

        cut.FindAll("tbody tr")[0].Click();

        Assert.Equal([Rows[2]], selected);
    }

    [Fact]
    public async Task KeyDown_Row_ActivatesSelectionAndRowClicked()
    {
        Row? clicked = null;
        Row? selected = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.SelectionMode, XFDataGridSelectionMode.Single);
            parameters.Add(p => p.RowClicked, value => clicked = value);
            parameters.Add(p => p.SelectedItemChanged, value => selected = value);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.FindAll("tbody tr")[2].KeyDown("Enter");

        Assert.Equal(Rows[2], clicked);
        Assert.Equal(Rows[2], selected);
    }

    [Fact]
    public void Render_FilterText_FiltersByVisibleColumnProperties()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.FilterText, "bill");
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Single(cut.FindAll("tbody tr"));
        Assert.Contains("Billing", cut.Markup);
        Assert.DoesNotContain("Finance", cut.Markup);
    }

    [Fact]
    public void Render_FilterKeys_FiltersByExplicitProperties()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.FilterText, "blocked");
            parameters.Add(p => p.FilterKeys, [nameof(Row.Status)]);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Single(cut.FindAll("tbody tr"));
        Assert.Contains("Billing", cut.Markup);
        Assert.DoesNotContain("Support", cut.Markup);
    }

    [Fact]
    public void Render_FilterPredicate_UsesCustomFilter()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.FilterText, "high");
            parameters.Add(p => p.FilterPredicate, (Row row, string filter) => filter == "high" && row.Score >= 90);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Single(cut.FindAll("tbody tr"));
        Assert.Contains("Finance", cut.Markup);
        Assert.DoesNotContain("Support", cut.Markup);
    }

    [Fact]
    public void Render_ServerSide_UsesItemsAsCurrentPageAndExternalTotal()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows.Take(2));
            parameters.Add(p => p.ServerSide, true);
            parameters.Add(p => p.TotalItems, 30);
            parameters.Add(p => p.AllowPaging, true);
            parameters.Add(p => p.PageSize, 10);
            parameters.Add(p => p.FilterText, "does-not-filter-locally");
            parameters.Add(p => p.SortKey, nameof(Row.Score));
            parameters.Add(p => p.SortDirection, XFDataGridSortDirection.Descending);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Equal(2, cut.FindAll("tbody tr").Count);
        Assert.Contains("Finance", cut.Markup);
        Assert.Contains("Billing", cut.Markup);
        Assert.Contains("xf-pagination", cut.Markup);
    }

    [Fact]
    public void Render_ItemsProvider_UsesProviderItemsAndTotal()
    {
        XFDataGridDataRequest? request = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.ItemsProvider, providerRequest =>
            {
                request = providerRequest;
                return ValueTask.FromResult(new XFDataGridDataResult<Row>(Rows.Take(2).ToArray(), 30));
            });
            parameters.Add(p => p.AllowPaging, true);
            parameters.Add(p => p.PageSize, 10);
            parameters.Add(p => p.FilterText, "blocked");
            parameters.Add(p => p.FilterKeys, [nameof(Row.Status)]);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("tbody tr").Count));
        Assert.NotNull(request);
        Assert.Equal(1, request!.Page);
        Assert.Equal(10, request.PageSize);
        Assert.Equal("blocked", request.FilterText);
        Assert.Equal([nameof(Row.Status)], request.FilterKeys);
        Assert.Contains("Finance", cut.Markup);
        Assert.Contains("Billing", cut.Markup);
        Assert.Contains("xf-pagination", cut.Markup);
    }

    [Fact]
    public void Click_Header_WithItemsProvider_RequestsSortedData()
    {
        var requests = new List<XFDataGridDataRequest>();
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.ItemsProvider, request =>
            {
                requests.Add(request);
                return ValueTask.FromResult(new XFDataGridDataResult<Row>(Rows, Rows.Count));
            });
            parameters.Add(p => p.Columns, BasicColumns());
        });

        cut.WaitForAssertion(() => Assert.NotEmpty(requests));
        cut.Find("th button").Click();

        cut.WaitForAssertion(() => Assert.Contains(requests, request =>
            request.SortKey == nameof(Row.Name)
            && request.SortDirection == XFDataGridSortDirection.Ascending));
    }

    [Fact]
    public void Render_EmptyTemplate_ReplacesEmptyText()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<Row>());
            parameters.Add(p => p.EmptyTemplate, builder => builder.AddContent(0, "Custom empty state"));
            parameters.Add(p => p.EmptyText, "Default empty state");
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("Custom empty state", cut.Markup);
        Assert.DoesNotContain("Default empty state", cut.Markup);
    }

    [Fact]
    public void Render_LoadingTemplate_ReplacesSkeletonRows()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Loading, true);
            parameters.Add(p => p.LoadingTemplate, builder => builder.AddContent(0, "Loading custom grid"));
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("Loading custom grid", cut.Markup);
        Assert.DoesNotContain("animate-pulse", cut.Markup);
    }

    #region Per-Column Filter Tests

    [Fact]
    public void Render_FilterModeRow_RendersFilterRowInHeader()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.FilterMode, XFDataGridFilterMode.Row);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.Contains("tr", cut.Find("thead").InnerHtml);
        var headerRows = cut.FindAll("thead tr");
        Assert.Equal(2, headerRows.Count);
    }

    [Fact]
    public void Render_FilterModeRow_FilterableColumnRendersInput()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.FilterMode, XFDataGridFilterMode.Row);
            parameters.Add(p => p.Columns, FilterableColumns());
        });

        var filterRow = cut.FindAll("thead tr").Last();
        Assert.Contains("<input", filterRow.InnerHtml);
    }

    [Fact]
    public void Render_FilterModeNone_NoFilterRowRendered()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.FilterMode, XFDataGridFilterMode.None);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        var headerRows = cut.FindAll("thead tr");
        Assert.Single(headerRows);
    }

    [Fact]
    public void FilterOperator_Enum_HasExpectedValues()
    {
        Assert.Equal(16, Enum.GetValues<XFDataGridFilterOperator>().Length);
        Assert.Contains(XFDataGridFilterOperator.Contains, Enum.GetValues<XFDataGridFilterOperator>());
        Assert.Contains(XFDataGridFilterOperator.Equals, Enum.GetValues<XFDataGridFilterOperator>());
        Assert.Contains(XFDataGridFilterOperator.In, Enum.GetValues<XFDataGridFilterOperator>());
        Assert.Contains(XFDataGridFilterOperator.NotIn, Enum.GetValues<XFDataGridFilterOperator>());
    }

    [Fact]
    public void FilterType_Enum_HasExpectedValues()
    {
        Assert.Equal(6, Enum.GetValues<XFDataGridFilterType>().Length);
        Assert.Contains(XFDataGridFilterType.Text, Enum.GetValues<XFDataGridFilterType>());
        Assert.Contains(XFDataGridFilterType.Number, Enum.GetValues<XFDataGridFilterType>());
        Assert.Contains(XFDataGridFilterType.Date, Enum.GetValues<XFDataGridFilterType>());
        Assert.Contains(XFDataGridFilterType.Boolean, Enum.GetValues<XFDataGridFilterType>());
    }

    #endregion

    #region Multi-Sort Tests

    [Fact]
    public void Click_HeaderTwice_RemovesSortDescriptor()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Columns, BasicColumns());
            parameters.Add(p => p.AllowMultiColumnSorting, true);
        });

        var nameHeader = cut.Find("th button");
        nameHeader.Click(); // Ascending
        nameHeader.Click(); // Descending
        nameHeader.Click(); // None (removed)

        Assert.DoesNotContain("sort-index", cut.Markup);
    }

    [Fact]
    public void MultiSort_AllowMultiColumnSorting_SingleClickStillSorts()
    {
        XFDataGridSortChangedEventArgs? sortArgs = null;
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Columns, BasicColumns());
            parameters.Add(p => p.AllowMultiColumnSorting, true);
            parameters.Add(p => p.SortChanged, value => sortArgs = value);
        });

        var allHeaders = cut.FindAll("th button");
        Assert.True(allHeaders.Count >= 2);

        // Click first header - Ascending
        allHeaders[0].Click();
        Assert.NotNull(sortArgs);
        Assert.Equal(nameof(Row.Name), sortArgs!.SortKey);
        Assert.Equal(XFDataGridSortDirection.Ascending, sortArgs.Direction);
        Assert.Equal("ascending", cut.Find("th").GetAttribute("aria-sort"));
    }

    [Fact]
    public void MultiSort_HiddenIndex_NoSortBadges()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.Columns, BasicColumns());
            parameters.Add(p => p.AllowMultiColumnSorting, true);
            parameters.Add(p => p.ShowMultiColumnSortingIndex, false);
        });

        cut.Find("th button").Click();
        cut.FindAll("th button")[1].Click();

        Assert.DoesNotContain("sort-index", cut.Markup);
    }

    [Fact]
    public void SortDescriptor_Record_StoresKeyDirectionIndex()
    {
        var descriptor = new XFDataGridSortDescriptor("Name", XFDataGridSortDirection.Ascending, 1);
        Assert.Equal("Name", descriptor.Key);
        Assert.Equal(XFDataGridSortDirection.Ascending, descriptor.Direction);
        Assert.Equal(1, descriptor.Index);
    }

    #endregion

    #region Aggregate Tests

    [Fact]
    public void Render_ShowAggregates_RendersAggregateRowInFooter()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.ShowAggregates, true);
            parameters.Add(p => p.Columns, AggregateColumns());
        });

        Assert.Contains("<tfoot", cut.Markup);
    }

    [Fact]
    public void Render_HiddenAggregates_NoFooterRow()
    {
        var cut = Render<XFDataGrid<Row>>(parameters =>
        {
            parameters.Add(p => p.Items, Rows);
            parameters.Add(p => p.ShowAggregates, false);
            parameters.Add(p => p.Columns, BasicColumns());
        });

        Assert.DoesNotContain("<tfoot", cut.Markup);
    }

    [Fact]
    public void AggregateFunction_Enum_HasExpectedValues()
    {
        Assert.Equal(5, Enum.GetValues<XFDataGridAggregateFunction>().Length);
        Assert.Contains(XFDataGridAggregateFunction.Sum, Enum.GetValues<XFDataGridAggregateFunction>());
        Assert.Contains(XFDataGridAggregateFunction.Average, Enum.GetValues<XFDataGridAggregateFunction>());
        Assert.Contains(XFDataGridAggregateFunction.Count, Enum.GetValues<XFDataGridAggregateFunction>());
        Assert.Contains(XFDataGridAggregateFunction.Min, Enum.GetValues<XFDataGridAggregateFunction>());
        Assert.Contains(XFDataGridAggregateFunction.Max, Enum.GetValues<XFDataGridAggregateFunction>());
    }

    [Fact]
    public void Aggregate_Record_StoresColumnKeyAndFunction()
    {
        var aggregate = new XFDataGridAggregate("Score", XFDataGridAggregateFunction.Sum);
        Assert.Equal("Score", aggregate.ColumnKey);
        Assert.Equal(XFDataGridAggregateFunction.Sum, aggregate.Function);
        Assert.Null(aggregate.FormatString);
    }

    [Fact]
    public void Aggregate_Record_WithFormatString()
    {
        var aggregate = new XFDataGridAggregate("Score", XFDataGridAggregateFunction.Average, "N2");
        Assert.Equal("N2", aggregate.FormatString);
    }

    #endregion

    #region ColumnFilterEventArgs Tests

    [Fact]
    public void ColumnFilterEventArgs_StoresProperties()
    {
        var args = new XFDataGridColumnFilterEventArgs("Name", "test", XFDataGridFilterOperator.Contains);
        Assert.Equal("Name", args.ColumnKey);
        Assert.Equal("test", args.FilterValue);
        Assert.Equal(XFDataGridFilterOperator.Contains, args.Operator);
    }

    [Fact]
    public void ColumnFilterEventArgs_NullFilterValue()
    {
        var args = new XFDataGridColumnFilterEventArgs("Score", null, XFDataGridFilterOperator.IsNull);
        Assert.Null(args.FilterValue);
        Assert.Equal(XFDataGridFilterOperator.IsNull, args.Operator);
    }

    #endregion

    #region FilterMode Enum Tests

    [Fact]
    public void FilterMode_Enum_HasExpectedValues()
    {
        Assert.Equal(3, Enum.GetValues<XFDataGridFilterMode>().Length);
        Assert.Contains(XFDataGridFilterMode.None, Enum.GetValues<XFDataGridFilterMode>());
        Assert.Contains(XFDataGridFilterMode.Row, Enum.GetValues<XFDataGridFilterMode>());
        Assert.Contains(XFDataGridFilterMode.Advanced, Enum.GetValues<XFDataGridFilterMode>());
    }

    #endregion

    private static RenderFragment FilterableColumns() => builder =>
    {
        builder.OpenComponent<XFDataGridColumn<Row>>(0);
        builder.AddAttribute(1, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Name));
        builder.AddAttribute(2, nameof(XFDataGridColumn<Row>.Title), "Workspace");
        builder.AddAttribute(3, nameof(XFDataGridColumn<Row>.Filterable), true);
        builder.CloseComponent();

        builder.OpenComponent<XFDataGridColumn<Row>>(4);
        builder.AddAttribute(5, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Score));
        builder.AddAttribute(6, nameof(XFDataGridColumn<Row>.Title), "Score");
        builder.AddAttribute(7, nameof(XFDataGridColumn<Row>.Filterable), true);
        builder.AddAttribute(8, nameof(XFDataGridColumn<Row>.FilterType), XFDataGridFilterType.Number);
        builder.CloseComponent();
    };

    private static RenderFragment AggregateColumns() => builder =>
    {
        builder.OpenComponent<XFDataGridColumn<Row>>(0);
        builder.AddAttribute(1, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Name));
        builder.AddAttribute(2, nameof(XFDataGridColumn<Row>.Title), "Workspace");
        builder.CloseComponent();

        builder.OpenComponent<XFDataGridColumn<Row>>(3);
        builder.AddAttribute(4, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Score));
        builder.AddAttribute(5, nameof(XFDataGridColumn<Row>.Title), "Score");
        builder.AddAttribute(6, nameof(XFDataGridColumn<Row>.Aggregatable), true);
        builder.AddAttribute(7, nameof(XFDataGridColumn<Row>.AggregateFunction), XFDataGridAggregateFunction.Sum);
        builder.CloseComponent();
    };

    private static RenderFragment BasicColumns() => builder =>
    {
        builder.OpenComponent<XFDataGridColumn<Row>>(0);
        builder.AddAttribute(1, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Name));
        builder.AddAttribute(2, nameof(XFDataGridColumn<Row>.Title), "Workspace");
        builder.CloseComponent();

        builder.OpenComponent<XFDataGridColumn<Row>>(3);
        builder.AddAttribute(4, nameof(XFDataGridColumn<Row>.Property), nameof(Row.Score));
        builder.AddAttribute(5, nameof(XFDataGridColumn<Row>.Title), "Score");
        builder.AddAttribute(6, nameof(XFDataGridColumn<Row>.Align), XFDataGridAlign.End);
        builder.CloseComponent();
    };

    private static readonly IReadOnlyList<Row> Rows =
    [
        new("Finance", "Review", 98),
        new("Billing", "Blocked", 73),
        new("Support", "Stable", 88)
    ];

    private sealed record Row(string Name, string Status, int Score);
}
