using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Drawer;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Overlays;

public sealed class XFDrawerInteractionTest : XForgeTestContext
{
    public XFDrawerInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFDrawer>();
    [Fact] public void Renders_Open() => Render<XFDrawer>(p => p.Add(x => x.Open, true));
    [Fact] public void Renders_WithTitle() { var cut = Render<XFDrawer>(p => { p.Add(x => x.Open, true); p.Add(x => x.Title, "Drawer"); }); Assert.Contains("Drawer", cut.Markup); }
    [Fact] public void Renders_WithDescription() => Render<XFDrawer>(p => { p.Add(x => x.Open, true); p.Add(x => x.Description, "Desc"); });
    [Fact] public void Renders_WithChildContent() => Render<XFDrawer>(p => p.Add(x => x.ChildContent, builder => builder.AddMarkupContent(0, "<div>Content</div>")));
    [Fact] public void Renders_WithHeader() => Render<XFDrawer>(p => p.Add(x => x.Header, builder => builder.AddMarkupContent(0, "<div>Header</div>")));
    [Fact] public void Renders_WithFooter() => Render<XFDrawer>(p => p.Add(x => x.Footer, builder => builder.AddMarkupContent(0, "<div>Footer</div>")));
    [Fact] public void Renders_PlacementRight() => Render<XFDrawer>(p => p.Add(x => x.Placement, XFDrawerPlacement.Right));
    [Fact] public void Renders_PlacementLeft() => Render<XFDrawer>(p => p.Add(x => x.Placement, XFDrawerPlacement.Left));
    [Fact] public void Renders_PlacementTop() => Render<XFDrawer>(p => p.Add(x => x.Placement, XFDrawerPlacement.Top));
    [Fact] public void Renders_PlacementBottom() => Render<XFDrawer>(p => p.Add(x => x.Placement, XFDrawerPlacement.Bottom));
    [Fact] public void Renders_AriaLabel() => Render<XFDrawer>(p => p.Add(x => x.AriaLabel, "drawer"));
    [Fact] public void OpenChanged_CallbackSet() { var cut = Render<XFDrawer>(p => p.Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void OnClose_CallbackSet() { var cut = Render<XFDrawer>(p => p.Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { }))); Assert.NotNull(cut); }
    [Fact] public void Opened_CallbackSet() { var cut = Render<XFDrawer>(p => p.Add(x => x.Opened, EventCallback.Factory.Create(this, () => { }))); Assert.NotNull(cut); }
    [Fact] public void Closed_CallbackSet() { var cut = Render<XFDrawer>(p => p.Add(x => x.Closed, EventCallback.Factory.Create(this, () => { }))); Assert.NotNull(cut); }
    [Fact] public void Enums_DrawerPlacement() => Assert.Equal(4, Enum.GetValues<XFDrawerPlacement>().Length);
    [Fact] public void Enums_DrawerCloseReason() => Assert.Equal(4, Enum.GetValues<XFDrawerCloseReason>().Length);
    [Fact] public void Enums_DrawerResponsiveBreakpoint() => Assert.Equal(4, Enum.GetValues<XFDrawerResponsiveBreakpoint>().Length);
    [Fact] public void DrawerOpeningEventArgs_CancelDefaultFalse() { var args = new XFDrawerOpeningEventArgs(); Assert.False(args.Cancel); }
    [Fact] public void DrawerClosingEventArgs_Created() { var args = new XFDrawerClosingEventArgs(XFDrawerCloseReason.User); Assert.Equal(XFDrawerCloseReason.User, args.Reason); Assert.False(args.Cancel); }
}
