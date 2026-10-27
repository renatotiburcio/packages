using XForge.DesignSystem.Components.CrossPlatform;
using XForge.DesignSystem.Components.Utilities.BreakpointObserver;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFResponsiveContainerComponentTest
{
    [Fact]
    public void XFResponsiveContext_CreatesWithCorrectProperties()
    {
        var context = new XFResponsiveContext(XFBreakpoint.Md, false, true, false);

        Assert.Equal(XFBreakpoint.Md, context.Breakpoint);
        Assert.False(context.IsMobile);
        Assert.True(context.IsTablet);
        Assert.False(context.IsDesktop);
    }

    [Fact]
    public void XFResponsiveContext_MobileBreakpoint_SetsIsMobileTrue()
    {
        var context = new XFResponsiveContext(XFBreakpoint.Sm, true, false, false);

        Assert.True(context.IsMobile);
        Assert.False(context.IsTablet);
        Assert.False(context.IsDesktop);
    }

    [Fact]
    public void XFResponsiveContext_DesktopBreakpoint_SetsIsDesktopTrue()
    {
        var context = new XFResponsiveContext(XFBreakpoint.Xl, false, false, true);

        Assert.False(context.IsMobile);
        Assert.False(context.IsTablet);
        Assert.True(context.IsDesktop);
    }

    [Fact]
    public void XFResponsiveContext_ImplementsEquality()
    {
        var context1 = new XFResponsiveContext(XFBreakpoint.Lg, false, true, false);
        var context2 = new XFResponsiveContext(XFBreakpoint.Lg, false, true, false);

        Assert.Equal(context1, context2);
    }
}