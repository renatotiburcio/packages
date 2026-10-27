using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Breadcrumb;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFBreadcrumbToolbarTest : BunitContext
{
    public XFBreadcrumbToolbarTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Breadcrumb_Renders() => Render<XFBreadcrumb>();
    [Fact] public void Breadcrumb_WithItems() { var items = new List<XFBreadcrumbItemModel> { new() { Text = "Home" }, new() { Text = "Page" } }; Render<XFBreadcrumb>(p => p.Add(x => x.Items, items)); }
    [Fact] public void Breadcrumb_SizeSm() => Render<XFBreadcrumb>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Breadcrumb_SizeLg() => Render<XFBreadcrumb>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Breadcrumb_VariantPlain() => Render<XFBreadcrumb>(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
    [Fact] public void Breadcrumb_VariantSoft() => Render<XFBreadcrumb>(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
    [Fact] public void Breadcrumb_MaxVisibleItems() => Render<XFBreadcrumb>(p => p.Add(x => x.MaxVisibleItems, 3));
    [Fact] public void Breadcrumb_CollapseMiddleFalse() => Render<XFBreadcrumb>(p => p.Add(x => x.CollapseMiddle, false));
    [Fact] public void Breadcrumb_AriaLabel() => Render<XFBreadcrumb>(p => p.Add(x => x.AriaLabel, "Breadcrumb"));
    [Fact] public void BreadcrumbItemModel_Created() { var item = new XFBreadcrumbItemModel { Text = "Home", Href = "/" }; Assert.Equal("Home", item.Text); Assert.Equal("/", item.Href); }
    [Fact] public void BreadcrumbItemModel_DefaultCurrent() => Assert.False(new XFBreadcrumbItemModel().Current);
    [Fact] public void BreadcrumbItemModel_SetCurrent() => Assert.True(new XFBreadcrumbItemModel { Current = true }.Current);
    [Fact] public void BreadcrumbItemModel_WithIcon() => Assert.NotNull(new XFBreadcrumbItemModel { Icon = builder => builder.AddMarkupContent(0, "<i>icon</i>") }.Icon);
}
