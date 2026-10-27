using Xunit;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;

namespace XForge.DesignSystem.Tests.Components.Utilities;

/// <summary>
/// Tests for XFBreakpoint enum and XFBreakpointService pure .NET logic.
/// Note: JS interop methods (InitializeAsync, DisposeAsync) cannot be fully tested
/// without a JS runtime, but we can test OnBreakpointChanged and enum values.
/// </summary>
public sealed class XFBreakpointServiceTest
{
    #region XFBreakpoint Enum

    [Fact]
    public void XFBreakpoint_HasSixValues()
    {
        Assert.Equal(6, Enum.GetValues<XFBreakpoint>().Length);
        Assert.Equal(XFBreakpoint.Xs, (XFBreakpoint)0);
        Assert.Equal(XFBreakpoint.Sm, (XFBreakpoint)1);
        Assert.Equal(XFBreakpoint.Md, (XFBreakpoint)2);
        Assert.Equal(XFBreakpoint.Lg, (XFBreakpoint)3);
        Assert.Equal(XFBreakpoint.Xl, (XFBreakpoint)4);
        Assert.Equal(XFBreakpoint.Xxl, (XFBreakpoint)5);
    }

    #endregion

    #region XFBreakpointService — OnBreakpointChanged

    [Fact]
    public void OnBreakpointChanged_ValidBreakpoint_UpdatesCurrentBreakpoint()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);

        // Simulate JS callback
        service.OnBreakpointChanged("Md");

        Assert.Equal(XFBreakpoint.Md, service.CurrentBreakpoint);
    }

    [Fact]
    public void OnBreakpointChanged_CaseInsensitive_ParsesCorrectly()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);

        service.OnBreakpointChanged("lg");

        Assert.Equal(XFBreakpoint.Lg, service.CurrentBreakpoint);
    }

    [Fact]
    public void OnBreakpointChanged_InvalidString_DoesNotUpdateBreakpoint()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);

        service.OnBreakpointChanged("invalid");

        // Should stay at default (Xs)
        Assert.Equal(XFBreakpoint.Xs, service.CurrentBreakpoint);
    }

    [Fact]
    public void OnBreakpointChanged_FiresBreakpointChangedEvent()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);
        XFBreakpoint? receivedBreakpoint = null;
        service.BreakpointChanged += bp => receivedBreakpoint = bp;

        service.OnBreakpointChanged("Xl");

        Assert.Equal(XFBreakpoint.Xl, receivedBreakpoint);
    }

    [Fact]
    public void OnBreakpointChanged_InvalidString_DoesNotFireEvent()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);
        bool eventFired = false;
        service.BreakpointChanged += _ => eventFired = true;

        service.OnBreakpointChanged("notabreakpoint");

        Assert.False(eventFired);
    }

    [Fact]
    public void OnBreakpointChanged_AllBreakpoints_ParseCorrectly()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);

        foreach (XFBreakpoint bp in Enum.GetValues<XFBreakpoint>())
        {
            service.OnBreakpointChanged(bp.ToString());
            Assert.Equal(bp, service.CurrentBreakpoint);
        }
    }

    #endregion

    #region XFBreakpointService — Default State

    [Fact]
    public void XFBreakpointService_DefaultCurrentBreakpoint_IsXs()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);
        Assert.Equal(XFBreakpoint.Xs, service.CurrentBreakpoint);
    }

    #endregion

    #region XFBreakpointService — Dispose

    [Fact]
    public void XFBreakpointService_Dispose_CanBeCalledMultipleTimes()
    {
        var jsRuntime = new MockJsRuntime();
        var service = new XFBreakpointService(jsRuntime);
        service.Dispose();
        service.Dispose(); // Should not throw
    }

    #endregion

    #region Helpers

    private sealed class MockJsRuntime : Microsoft.JSInterop.IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => default;
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => default;
    }

    #endregion
}
