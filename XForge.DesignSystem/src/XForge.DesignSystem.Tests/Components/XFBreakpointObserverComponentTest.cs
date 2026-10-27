using Bunit;
using Microsoft.JSInterop;
using Xunit;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;

namespace XForge.DesignSystem.Tests.Components;

public class XFBreakpointObserverComponentTest : BunitContext
{
    [Fact]
    public void XFBreakpoint_HasExpectedValues()
    {
        Assert.Equal(6, Enum.GetValues<XFBreakpoint>().Length);
        Assert.Contains(XFBreakpoint.Xs, Enum.GetValues<XFBreakpoint>());
        Assert.Contains(XFBreakpoint.Sm, Enum.GetValues<XFBreakpoint>());
        Assert.Contains(XFBreakpoint.Md, Enum.GetValues<XFBreakpoint>());
        Assert.Contains(XFBreakpoint.Lg, Enum.GetValues<XFBreakpoint>());
        Assert.Contains(XFBreakpoint.Xl, Enum.GetValues<XFBreakpoint>());
        Assert.Contains(XFBreakpoint.Xxl, Enum.GetValues<XFBreakpoint>());
    }

    [Fact]
    public void Service_DefaultBreakpoint_IsXs()
    {
        var service = new XFBreakpointService(new MockJSRuntime());
        Assert.Equal(XFBreakpoint.Xs, service.CurrentBreakpoint);
    }

    [Fact]
    public void Service_OnBreakpointChanged_UpdatesCurrentBreakpoint()
    {
        var service = new XFBreakpointService(new MockJSRuntime());

        service.OnBreakpointChanged("md");
        Assert.Equal(XFBreakpoint.Md, service.CurrentBreakpoint);

        service.OnBreakpointChanged("lg");
        Assert.Equal(XFBreakpoint.Lg, service.CurrentBreakpoint);

        service.OnBreakpointChanged("xl");
        Assert.Equal(XFBreakpoint.Xl, service.CurrentBreakpoint);
    }

    [Fact]
    public void Service_OnBreakpointChanged_FiresEvent()
    {
        var service = new XFBreakpointService(new MockJSRuntime());
        XFBreakpoint? notified = null;
        service.BreakpointChanged += bp => notified = bp;

        service.OnBreakpointChanged("sm");

        Assert.Equal(XFBreakpoint.Sm, notified);
    }

    [Fact]
    public void Service_OnBreakpointChanged_IgnoresInvalidValue()
    {
        var service = new XFBreakpointService(new MockJSRuntime());
        service.OnBreakpointChanged("invalid");
        Assert.Equal(XFBreakpoint.Xs, service.CurrentBreakpoint); // unchanged
    }

    [Fact]
    public async Task Service_DisposeAsync_DoesNotThrow()
    {
        var service = new XFBreakpointService(new MockJSRuntime());
        await service.DisposeAsync();
    }

    [Fact]
    public void Service_BreakpointChanged_NoSubscribers_DoesNotThrow()
    {
        var service = new XFBreakpointService(new MockJSRuntime());
        service.OnBreakpointChanged("lg");
        Assert.Equal(XFBreakpoint.Lg, service.CurrentBreakpoint);
    }

    private class MockJSRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => default;
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => default;
    }
}
