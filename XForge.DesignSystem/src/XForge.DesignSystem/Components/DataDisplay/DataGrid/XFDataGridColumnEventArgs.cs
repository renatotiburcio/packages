namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
    public sealed record XFDataGridColumnReorderedEventArgs(string ColumnKey, int OldIndex, int NewIndex);

    public sealed record XFDataGridColumnVisibilityChangedEventArgs(string ColumnKey, bool Visible);

    public sealed record XFDataGridColumnResizedEventArgs(string ColumnKey, int WidthPixels);
}
