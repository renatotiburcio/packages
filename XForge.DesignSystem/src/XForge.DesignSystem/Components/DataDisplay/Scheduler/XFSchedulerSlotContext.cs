namespace XForge.DesignSystem.Components.DataDisplay.Scheduler
{
    public sealed record XFSchedulerSlotContext<TItem>(
        DateTime Start,
        DateTime End,
        XFSchedulerView View,
        IReadOnlyList<TItem> Appointments,
        string? ResourceId,
        string? ResourceText);
}
