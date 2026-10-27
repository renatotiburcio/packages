namespace XForge.DesignSystem.Components.DataDisplay.Scheduler
{
    public sealed record XFSchedulerSlotSelectEventArgs(DateTime Start, DateTime End, XFSchedulerView View);

    public sealed record XFSchedulerAppointmentSelectEventArgs<TItem>(TItem Item, DateTime Start, DateTime End, string Text);

    public sealed record XFSchedulerAppointmentMoveEventArgs<TItem>(TItem Item, DateTime Start, DateTime End, DateTime NewStart, DateTime NewEnd, string? ResourceId);

    public sealed record XFSchedulerAppointmentResizeEventArgs<TItem>(TItem Item, DateTime Start, DateTime End, DateTime NewStart, DateTime NewEnd, string? ResourceId);
}
