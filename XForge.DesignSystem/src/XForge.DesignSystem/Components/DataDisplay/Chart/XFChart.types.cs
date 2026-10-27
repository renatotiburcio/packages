namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    public enum XFChartType
    {
        Line,
        Bar,
        Pie,
        Scatter,
        Candlestick,
        Radar,
        Heatmap,
        Graph,
        Tree,
        Treemap,
        Sunburst,
        Sankey,
        Funnel,
        Gauge,
        ThemeRiver,
        Calendar,
        Chord
    }

    public enum XFChartDataZoomMode
    {
        None,
        Slider,
        Inside,
        SliderAndInside
    }

    /// <summary>
    /// Stack mode for bar, line, and area charts.
    /// </summary>
    public enum XFChartStackMode
    {
        /// <summary>No stacking (default). Each series renders independently.</summary>
        None,
        /// <summary>Stacked. Series are stacked on top of each other.</summary>
        Stacked,
        /// <summary>Full stacked (100%). Series are stacked to fill 100% of the chart.</summary>
        FullStacked
    }

    public enum XFChartAnnotationAxis
    {
        Y,
        X
    }

    public sealed record ChartDataPoint(
        string Label = "",
        double Value = 0,
        string Category = "");
}