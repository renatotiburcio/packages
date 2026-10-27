using XForge.DesignSystem.Components.CrossPlatform;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFGestureHandlerComponentTest
{
    [Fact]
    public void XFGestureContext_CreatesWithCorrectProperties()
    {
        var gestureEvent = new XFGestureEvent
        {
            Type = XFGestureType.Tap,
            State = XFGestureState.Recognized,
            StartX = 10,
            StartY = 20,
            CurrentX = 15,
            CurrentY = 25,
            DeltaX = 5,
            DeltaY = 5,
            Distance = 7.07,
            VelocityX = 1.5,
            VelocityY = 1.5,
            Scale = 1,
            Rotation = 0,
            DurationMs = 100,
            PointerCount = 1
        };
        var context = new XFGestureContext(gestureEvent, XFGestureState.Recognized);

        Assert.NotNull(context.LastGesture);
        Assert.Equal(XFGestureType.Tap, context.LastGesture.Type);
        Assert.Equal(XFGestureState.Recognized, context.State);
        Assert.Equal(10, context.LastGesture.StartX);
    }

    [Fact]
    public void XFGestureContext_CreatesWithNullGesture()
    {
        var context = new XFGestureContext(null, XFGestureState.Idle);

        Assert.Null(context.LastGesture);
        Assert.Equal(XFGestureState.Idle, context.State);
    }

    [Fact]
    public void XFGestureContext_ImplementsEquality()
    {
        var context1 = new XFGestureContext(null, XFGestureState.Tracking);
        var context2 = new XFGestureContext(null, XFGestureState.Tracking);

        Assert.Equal(context1, context2);
    }
}