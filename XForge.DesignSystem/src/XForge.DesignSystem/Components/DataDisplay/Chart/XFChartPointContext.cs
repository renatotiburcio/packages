namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    public sealed record XFChartPointContext<TItem>(
        TItem Item,
        string Label,
        double Value,
        double Percent,
        int Index);
}
