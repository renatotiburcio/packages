namespace XForge.DesignSystem.Components.DataDisplay.Scheduler
{
    public sealed record XFSchedulerAppointmentContext<TItem>(
        TItem Item,
        DateTime Start,
        DateTime End,
        string Text,
        bool IsAllDay,
        bool IsMultiDay,
        string? ResourceId,
        string? ResourceText);
}
