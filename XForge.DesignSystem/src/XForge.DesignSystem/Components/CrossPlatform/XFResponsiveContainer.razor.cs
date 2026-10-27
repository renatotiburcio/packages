using XForge.DesignSystem.Components.Utilities.BreakpointObserver;

namespace XForge.DesignSystem.Components.CrossPlatform;

/// <summary>Context provided by XFResponsiveContainer with current breakpoint and device category.</summary>
public record XFResponsiveContext
{
    public XFBreakpoint Breakpoint { get; }
    public bool IsMobile { get; }
    public bool IsTablet { get; }
    public bool IsDesktop { get; }

    public XFResponsiveContext(XFBreakpoint breakpoint, bool isMobile, bool isTablet, bool isDesktop)
    {
        Breakpoint = breakpoint;
        IsMobile = isMobile;
        IsTablet = isTablet;
        IsDesktop = isDesktop;
    }
}
