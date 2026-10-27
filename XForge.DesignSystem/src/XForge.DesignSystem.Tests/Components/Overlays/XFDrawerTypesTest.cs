using Xunit;
using XForge.DesignSystem.Components.Overlays.Drawer;

namespace XForge.DesignSystem.Tests.Components.Overlays;

/// <summary>
/// Tests for Drawer types at 0% coverage: XFDrawerPlacement, XFDrawerResponsiveBreakpoint,
/// XFDrawerCloseReason, XFDrawerOpeningEventArgs, XFDrawerClosingEventArgs.
/// </summary>
public sealed class XFDrawerTypesTest
{
    #region XFDrawerPlacement

    [Fact]
    public void XFDrawerPlacement_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFDrawerPlacement>().Length);
        Assert.True(Enum.IsDefined<XFDrawerPlacement>(XFDrawerPlacement.Right));
        Assert.True(Enum.IsDefined<XFDrawerPlacement>(XFDrawerPlacement.Left));
        Assert.True(Enum.IsDefined<XFDrawerPlacement>(XFDrawerPlacement.Top));
        Assert.True(Enum.IsDefined<XFDrawerPlacement>(XFDrawerPlacement.Bottom));
    }

    #endregion

    #region XFDrawerResponsiveBreakpoint

    [Fact]
    public void XFDrawerResponsiveBreakpoint_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFDrawerResponsiveBreakpoint>().Length);
        Assert.True(Enum.IsDefined<XFDrawerResponsiveBreakpoint>(XFDrawerResponsiveBreakpoint.None));
        Assert.True(Enum.IsDefined<XFDrawerResponsiveBreakpoint>(XFDrawerResponsiveBreakpoint.Sm));
        Assert.True(Enum.IsDefined<XFDrawerResponsiveBreakpoint>(XFDrawerResponsiveBreakpoint.Md));
        Assert.True(Enum.IsDefined<XFDrawerResponsiveBreakpoint>(XFDrawerResponsiveBreakpoint.Lg));
    }

    #endregion

    #region XFDrawerCloseReason

    [Fact]
    public void XFDrawerCloseReason_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFDrawerCloseReason>().Length);
        Assert.True(Enum.IsDefined<XFDrawerCloseReason>(XFDrawerCloseReason.User));
        Assert.True(Enum.IsDefined<XFDrawerCloseReason>(XFDrawerCloseReason.Escape));
        Assert.True(Enum.IsDefined<XFDrawerCloseReason>(XFDrawerCloseReason.Overlay));
        Assert.True(Enum.IsDefined<XFDrawerCloseReason>(XFDrawerCloseReason.Programmatic));
    }

    #endregion

    #region XFDrawerOpeningEventArgs

    [Fact]
    public void XFDrawerOpeningEventArgs_Default_IsNotCancelled()
    {
        var args = new XFDrawerOpeningEventArgs();
        Assert.False(args.Cancel);
    }

    [Fact]
    public void XFDrawerOpeningEventArgs_Cancel_CanBeSet()
    {
        var args = new XFDrawerOpeningEventArgs { Cancel = true };
        Assert.True(args.Cancel);
    }

    #endregion

    #region XFDrawerClosingEventArgs

    [Fact]
    public void XFDrawerClosingEventArgs_Constructs_WithReason()
    {
        var args = new XFDrawerClosingEventArgs(XFDrawerCloseReason.Escape);
        Assert.Equal(XFDrawerCloseReason.Escape, args.Reason);
        Assert.False(args.Cancel);
    }

    [Fact]
    public void XFDrawerClosingEventArgs_Cancel_CanBeSet()
    {
        var args = new XFDrawerClosingEventArgs(XFDrawerCloseReason.User) { Cancel = true };
        Assert.Equal(XFDrawerCloseReason.User, args.Reason);
        Assert.True(args.Cancel);
    }

    [Fact]
    public void XFDrawerClosingEventArgs_AllReasons_Work()
    {
        foreach (XFDrawerCloseReason reason in Enum.GetValues<XFDrawerCloseReason>())
        {
            var args = new XFDrawerClosingEventArgs(reason);
            Assert.Equal(reason, args.Reason);
        }
    }

    #endregion
}
