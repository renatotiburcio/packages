namespace XForge.DesignSystem.Components.CrossPlatform;

/// <summary>Context provided by XFGestureHandler with last gesture and current state.</summary>
public record XFGestureContext
{
    public XFGestureEvent? LastGesture { get; }
    public XFGestureState State { get; }

    public XFGestureContext(XFGestureEvent? lastGesture, XFGestureState state)
    {
        LastGesture = lastGesture;
        State = state;
    }
}
