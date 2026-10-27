namespace XForge.DesignSystem.Components.DataDisplay.Gauge
{
    /// <summary>
    /// Size presets for gauge components.
    /// </summary>
/// <summary>
    /// Orientation for linear gauge.
    /// </summary>
/// <summary>
    /// Position of value text relative to the linear gauge bar.
    /// </summary>
    public enum XFGaugeValuePosition
    {
        /// <summary>At the start of the bar.</summary>
        Start,
        /// <summary>Centered over the bar.</summary>
        Center,
        /// <summary>At the end of the bar.</summary>
        End
    }

    /// <summary>
    /// Defines a colored range band on a gauge scale.
    /// </summary>
    public class XFGaugeRange
    {
        /// <summary>Range start value (inclusive).</summary>
        public double From { get; set; }

        /// <summary>Range end value (inclusive).</summary>
        public double To { get; set; }

        /// <summary>Range band color (Tailwind class or CSS color).</summary>
        public string? Color { get; set; }
    }

    /// <summary>
    /// Context passed to <see cref="XFGauge.ValueTemplate"/> and <see cref="XFLinearGauge.ValueTemplate"/>.
    /// </summary>
    public class XFGaugeValueContext
    {
        /// <summary>Current clamped value.</summary>
        public double Value { get; set; }

        /// <summary>Value as percentage of range (0–100).</summary>
        public double Percent { get; set; }

        /// <summary>Formatted display text.</summary>
        public string FormattedValue { get; set; } = "";
    }
}
