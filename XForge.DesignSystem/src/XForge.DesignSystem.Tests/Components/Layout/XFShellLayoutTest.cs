using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Layout.Shell;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Layout;

public sealed class XFShellLayoutTest : XForgeTestContext
{
    public XFShellLayoutTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void AppShell_Renders() => Render<XFAppShell>();
    [Fact] public void AppShell_WithTheme() => Render<XFAppShell>(p => p.Add(x => x.Theme, "dark"));
    [Fact] public void AppShell_WithBrand() => Render<XFAppShell>(p => p.Add(x => x.Brand, "MyApp"));
    [Fact] public void AppShell_FullViewport() => Render<XFAppShell>(p => p.Add(x => x.FullViewport, true));
    [Fact] public void AppShell_MainId() => Render<XFAppShell>(p => p.Add(x => x.MainId, "main"));
    [Fact] public void AppShell_ContainsMainContent() { var cut = Render<XFAppShell>(p => p.Add(x => x.ChildContent, "Content")); Assert.Contains("Content", cut.Markup); }
    [Fact] public void AppBar_Renders() => Render<XFAppBar>();
    [Fact] public void Footer_Renders() => Render<XFFooter>();
    [Fact] public void Footer_WithStatusText() => Render<XFFooter>(p => p.Add(x => x.StatusText, "Stable"));
    [Fact] public void Footer_WithUserName() => Render<XFFooter>(p => p.Add(x => x.UserName, "Admin"));
    [Fact] public void Footer_WithAppName() => Render<XFFooter>(p => p.Add(x => x.AppName, "XForge"));
    [Fact] public void Footer_WithVersion() => Render<XFFooter>(p => p.Add(x => x.Version, "v2.0.0"));
    [Fact] public void Footer_ShowStatusDotFalse() => Render<XFFooter>(p => p.Add(x => x.ShowStatusDot, false));
    [Fact] public void Footer_StartContent() => Render<XFFooter>(p => p.Add(x => x.StartContent, builder => builder.AddMarkupContent(0, "<div>Start</div>")));
    [Fact] public void Footer_EndContent() => Render<XFFooter>(p => p.Add(x => x.EndContent, builder => builder.AddMarkupContent(0, "<div>End</div>")));
    
    
    
    
    
    [Fact] public void Enums_Orientation() => Assert.Equal(2, Enum.GetValues<XFOrientation>().Length);
    [Fact] public void Enums_ContainerVariant() => Assert.Equal(3, Enum.GetValues<XFContainerVariant>().Length);
}
