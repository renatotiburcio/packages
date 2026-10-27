namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
    public sealed record XFDataGridGroup<TItem>(
        object? Key,
        string Label,
        IReadOnlyList<TItem> Items,
        int Count);
}
