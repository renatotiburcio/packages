namespace XForge.DesignSystem.Components.Overlays.Popover
{
    public sealed class XFPopoverTriggerContext
    {
        public XFPopoverTriggerContext(bool open, Func<Task> toggleAsync, Func<Task> openAsync, Func<Task> closeAsync)
        {
            Open = open;
            ToggleAsync = toggleAsync;
            OpenAsync = openAsync;
            CloseAsync = closeAsync;
        }

        public bool Open { get; }
        public Func<Task> ToggleAsync { get; }
        public Func<Task> OpenAsync { get; }
        public Func<Task> CloseAsync { get; }
    }
}
