namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    public sealed record XFChartPointSelectedEventArgs<TItem>(
        TItem Item,
        string Label,
        double Value,
        double Percent,
        int Index);
}
