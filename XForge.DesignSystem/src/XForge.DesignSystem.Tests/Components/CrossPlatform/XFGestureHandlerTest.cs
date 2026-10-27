using Bunit;
using Xunit;
using XForge.DesignSystem.Components.CrossPlatform;

namespace XForge.DesignSystem.Tests.Components.CrossPlatform;

public class XFGestureHandlerTest : BunitContext
{
    public XFGestureHandlerTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void XFGestureContext_CreatedWithNullGesture_HasNoGesture()
    {
        var context = new XFGestureContext(null, XFGestureState.Idle);
        Assert.Null(context.LastGesture);
        Assert.Equal(XFGestureState.Idle, context.State);
    }

    [Fact]
    public void XFGestureContext_CreatedWithGesture_HasGesture()
    {
        var gesture = new XFGestureEvent
        {
            Type = XFGestureType.Tap,
            State = XFGestureState.Recognized
        };
        var context = new XFGestureContext(gesture, XFGestureState.Recognized);
        Assert.NotNull(context.LastGesture);
        Assert.Equal(XFGestureState.Recognized, context.State);
    }

    [Fact]
    public void XFGestureContext_Equality_Works()
    {
        var a = new XFGestureContext(null, XFGestureState.Idle);
        var b = new XFGestureContext(null, XFGestureState.Idle);
        Assert.Equal(a, b);
    }

    [Fact]
    public void XFGestureEvent_CreatedWithInit_HasProperties()
    {
        var gesture = new XFGestureEvent
        {
            Type = XFGestureType.SwipeLeft,
            State = XFGestureState.Recognized,
            Distance = 100.0
        };
        Assert.Equal(XFGestureType.SwipeLeft, gesture.Type);
        Assert.Equal(100.0, gesture.Distance);
    }
}
