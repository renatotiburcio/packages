namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
    public delegate ValueTask<XFDataGridDataResult<TItem>> XFDataGridItemsProvider<TItem>(XFDataGridDataRequest request);

    public sealed record XFDataGridDataRequest(
        int Page,
        int PageSize,
        string? SortKey,
        XFDataGridSortDirection SortDirection,
        string? FilterText,
        IReadOnlyList<string> FilterKeys);

    public sealed record XFDataGridDataResult<TItem>(
        IReadOnlyList<TItem> Items,
        int TotalItems);
}
