namespace XForge.DesignSystem.Components.DataDisplay.Calendar
{
    public sealed record XFCalendarDayContext(
        DateOnly Date,
        bool IsCurrentMonth,
        bool IsSelected,
        bool IsToday,
        bool IsDisabled,
        IReadOnlyList<XFCalendarHighlight> Highlights,
        bool IsInRange = false,
        bool IsRangeStart = false,
        bool IsRangeEnd = false,
        bool IsMultipleSelected = false)
    {
        /// <summary>
        /// True when at least one highlight exists for this date.
        /// </summary>
        public bool IsHighlighted => Highlights.Count > 0;
    }
}
