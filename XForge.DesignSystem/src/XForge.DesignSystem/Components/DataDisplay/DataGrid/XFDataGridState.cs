namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
    public sealed record XFDataGridState(
        int Page,
        int PageSize,
        string? SortKey,
        XFDataGridSortDirection SortDirection,
        string? FilterText,
        string? GroupBy,
        IReadOnlyList<string> ColumnOrder,
        IReadOnlyDictionary<string, bool> ColumnVisibility,
        IReadOnlyDictionary<string, int> ColumnWidths);
}
