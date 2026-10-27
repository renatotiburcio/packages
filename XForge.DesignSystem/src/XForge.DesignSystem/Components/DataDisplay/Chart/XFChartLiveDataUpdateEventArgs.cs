namespace XForge.DesignSystem.Components.DataDisplay.Chart
{
    /// <summary>
    /// Event arguments for chart live streaming data updates.
    /// </summary>
    public class XFChartLiveDataUpdateEventArgs<TItem>
    {
        /// <summary>The new data item added.</summary>
        public TItem? NewItem { get; set; }

        /// <summary>Current number of data points.</summary>
        public int PointCount { get; set; }

        /// <summary>Timestamp of the update.</summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>Whether data was trimmed (max points exceeded).</summary>
        public bool WasTrimmed { get; set; }
    }
}
