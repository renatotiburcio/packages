using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Tabs;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFTabsInteractionTest : XForgeTestContext
{
    public XFTabsInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFTabs>();
    [Fact] public void Renders_WithActiveId() => Render<XFTabs>(p => p.Add(x => x.ActiveId, "tab1"));
    [Fact] public void Renders_VariantUnderline() => Render<XFTabs>(p => p.Add(x => x.Variant, XFTabsVariant.Underline));
    [Fact] public void Renders_VariantPills() => Render<XFTabs>(p => p.Add(x => x.Variant, XFTabsVariant.Pills));
    [Fact] public void Renders_VariantPlain() => Render<XFTabs>(p => p.Add(x => x.Variant, XFTabsVariant.Subtle));
    [Fact] public void Renders_SizeSm() => Render<XFTabs>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFTabs>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_OrientationHorizontal() => Render<XFTabs>(p => p.Add(x => x.Orientation, XFTabsOrientation.Horizontal));
    [Fact] public void Renders_OrientationVertical() => Render<XFTabs>(p => p.Add(x => x.Orientation, XFTabsOrientation.Vertical));
    [Fact] public void Renders_ActivationModeAutomatic() => Render<XFTabs>(p => p.Add(x => x.ActivationMode, XFTabsActivationMode.Automatic));
    [Fact] public void Renders_ActivationModeManual() => Render<XFTabs>(p => p.Add(x => x.ActivationMode, XFTabsActivationMode.Manual));
    [Fact] public void Renders_RenderModeDefault() => Render<XFTabs>(p => p.Add(x => x.RenderMode, XFTabsRenderMode.Default));
    [Fact] public void Renders_FullWidth() => Render<XFTabs>(p => p.Add(x => x.FullWidth, true));
    [Fact] public void Renders_ShowIndicatorFalse() => Render<XFTabs>(p => p.Add(x => x.ShowIndicator, false));
    [Fact] public void Renders_AllowClose() => Render<XFTabs>(p => p.Add(x => x.AllowClose, true));
    [Fact] public void Renders_AriaLabel() { var cut = Render<XFTabs>(p => p.Add(x => x.AriaLabel, "Tabs")); Assert.Contains("Tabs", cut.Markup); }
    [Fact] public void TabChanged_CallbackSet() { var cut = Render<XFTabs>(p => p.Add(x => x.TabChanged, EventCallback.Factory.Create<XFTabChangeEventArgs>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void TabClose_CallbackSet() { var cut = Render<XFTabs>(p => p.Add(x => x.TabClose, EventCallback.Factory.Create<XFTabCloseEventArgs>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void Enums_TabsVariant() => Assert.Equal(4, Enum.GetValues<XFTabsVariant>().Length);
    [Fact] public void Enums_TabsOrientation() => Assert.Equal(4, Enum.GetValues<XFTabsOrientation>().Length);
    [Fact] public void Enums_TabsActivationMode() => Assert.Equal(2, Enum.GetValues<XFTabsActivationMode>().Length);
    [Fact] public void Enums_TabsRenderMode() => Assert.Equal(3, Enum.GetValues<XFTabsRenderMode>().Length);
}
