namespace XForge.DesignSystem.Components.DataDisplay.Calendar
{
    using XForge.DesignSystem.Core.Primitives;

    /// <summary>
    /// Represents a highlight event on a specific date in the calendar.
    /// </summary>
    public sealed record XFCalendarHighlight(
        DateOnly Date,
        string Label,
        XFSemanticColor Color = XFSemanticColor.Primary);
}
