namespace XForge.DesignSystem.Components.DataDisplay.DataGrid
{
    public sealed record XFDataGridColumnFilterEventArgs(
        string ColumnKey,
        string? FilterValue,
        XFDataGridFilterOperator Operator);
}
