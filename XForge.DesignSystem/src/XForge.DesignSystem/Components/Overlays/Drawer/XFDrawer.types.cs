namespace XForge.DesignSystem.Components.Overlays.Drawer
{
    public enum XFDrawerPlacement
    {
        Right,
        Left,
        Top,
        Bottom
    }
public enum XFDrawerResponsiveBreakpoint
    {
        None,
        Sm,
        Md,
        Lg
    }

    public enum XFDrawerCloseReason
    {
        User,
        Escape,
        Overlay,
        Programmatic
    }

    public sealed class XFDrawerOpeningEventArgs
    {
        public bool Cancel { get; set; }
    }

    public sealed class XFDrawerClosingEventArgs
    {
        public XFDrawerClosingEventArgs(XFDrawerCloseReason reason)
        {
            Reason = reason;
        }

        public XFDrawerCloseReason Reason { get; }
        public bool Cancel { get; set; }
    }
}
