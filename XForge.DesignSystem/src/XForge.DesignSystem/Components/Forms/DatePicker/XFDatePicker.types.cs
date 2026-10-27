namespace XForge.DesignSystem.Components.Forms.DatePicker
{
public enum XFDatePickerView
    {
        Month,
        Year,
        Decade
    }

    public sealed class XFDatePickerDateRenderArgs
    {
        public DateOnly Date { get; init; }
        public bool IsCurrentMonth { get; init; }
        public bool IsSelected { get; init; }
        public bool IsToday { get; init; }
        public bool IsDisabled { get; set; }
        public string? CssClass { get; set; }
    }

    internal sealed record XFDatePickerDay(
        DateOnly Date,
        bool IsCurrentMonth,
        bool IsSelected,
        bool IsToday,
        bool IsDisabled,
        string? CustomCssClass);
}
