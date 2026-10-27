using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Components.Navigation.TOC;
using XForge.DesignSystem.Components.Navigation.Toolbar;
using XForge.DesignSystem.Components.Utilities.Collapse;
using XForge.DesignSystem.Components.Utilities.FocusTrap;
using XForge.DesignSystem.Components.Utilities.Animate;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public sealed class XFLayoutTest : BunitContext
{
    public XFLayoutTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    // Collapse
    [Fact] public void Collapse_Renders() => Render<XFCollapse>();
    [Fact] public void Collapse_Expanded() => Render<XFCollapse>(p => p.Add(x => x.Expanded, true));
    [Fact] public void Collapse_Title() => Assert.Contains("Section", Render<XFCollapse>(p => p.Add(x => x.Title, "Section")).Markup);
    [Fact] public void Collapse_Lazy() => Render<XFCollapse>(p => p.Add(x => x.Lazy, true));

    // FocusTrap
    [Fact] public void FocusTrap_Renders() => Render<XFFocusTrap>();
    [Fact] public void FocusTrap_Active() => Render<XFFocusTrap>(p => p.Add(x => x.Active, false));
    [Fact] public void FocusTrap_RestoreFocus() => Render<XFFocusTrap>(p => p.Add(x => x.RestoreFocus, false));

    // Animate
    [Fact] public void Animate_Renders() => Render<XFAnimate>();

    // AppShell
    [Fact] public void AppShell_Renders() => Render<XFAppShell>();
    [Fact] public void AppShell_Theme() => Render<XFAppShell>(p => p.Add(x => x.Theme, "dark"));
    [Fact] public void AppShell_Brand() => Assert.Contains("MyApp", Render<XFAppShell>(p => p.Add(x => x.Brand, "MyApp")).Markup);
    [Fact] public void AppShell_FullViewport() => Render<XFAppShell>(p => p.Add(x => x.FullViewport, true));

    // AppBar
    [Fact] public void AppBar_Renders() => Render<XFAppBar>();

    // Footer
    [Fact] public void Footer_Renders() => Render<XFFooter>();
    [Fact] public void Footer_StatusText() => Render<XFFooter>(p => p.Add(x => x.StatusText, "Stable"));
    [Fact] public void Footer_UserName() => Render<XFFooter>(p => p.Add(x => x.UserName, "Admin"));
    [Fact] public void Footer_ShowStatusDotFalse() => Render<XFFooter>(p => p.Add(x => x.ShowStatusDot, false));

    // Rail
    // Sidebar
    [Fact] public void Sidebar_Renders() => Render<XFSidebar>();

    // TOC
    [Fact] public void TOC_Renders() => Render<XFTOC>();
    [Fact] public void TOC_ActiveId() => Render<XFTOC>(p => p.Add(x => x.ActiveId, "section1"));
    [Fact] public void TOC_Title() => Render<XFTOC>(p => p.Add(x => x.Title, "TOC Title"));

    // Toolbar
    [Fact] public void Toolbar_Renders() => Render<XFToolbar>();
    [Fact] public void Toolbar_Variant() => Render<XFToolbar>(p => p.Add(x => x.Variant, XFToolbarVariant.Surface));
    [Fact] public void Toolbar_SizeSm() => Render<XFToolbar>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Toolbar_Dense() => Render<XFToolbar>(p => p.Add(x => x.Dense, true));
    [Fact] public void Toolbar_Wrap() => Render<XFToolbar>(p => p.Add(x => x.Wrap, false));
}