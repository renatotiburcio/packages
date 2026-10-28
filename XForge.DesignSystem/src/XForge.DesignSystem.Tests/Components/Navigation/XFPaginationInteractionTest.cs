using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Pagination;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFPaginationInteractionTest : XForgeTestContext
{
    public XFPaginationInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFPagination>();
    [Fact] public void Renders_WithPage() => Render<XFPagination>(p => p.Add(x => x.Page, 2));
    [Fact] public void Renders_WithPageSize() => Render<XFPagination>(p => p.Add(x => x.PageSize, 25));
    [Fact] public void Renders_WithTotalItems() => Render<XFPagination>(p => p.Add(x => x.TotalItems, 100));
    [Fact] public void Renders_WithPageSizeOptions() => Render<XFPagination>(p => p.Add(x => x.PageSizeOptions, new[] { 10, 25, 50 }));
    [Fact] public void Renders_VariantSurface() => Render<XFPagination>(p => p.Add(x => x.Variant, XFContainerVariant.Surface));
    [Fact] public void Renders_VariantOutlined() => Render<XFPagination>(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
    [Fact] public void Renders_SizeSm() => Render<XFPagination>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFPagination>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_DensityCompact() => Render<XFPagination>(p => p.Add(x => x.Density, XFDensity.Compact));
    [Fact] public void Renders_AutoHide() => Render<XFPagination>(p => p.Add(x => x.AutoHide, true));
    [Fact] public void Renders_AllowReload() => Render<XFPagination>(p => p.Add(x => x.AllowReload, true));
    [Fact] public void Renders_ShowSummaryFalse() => Render<XFPagination>(p => p.Add(x => x.ShowSummary, false));
    [Fact] public void Renders_AriaLabel() => Render<XFPagination>(p => p.Add(x => x.AriaLabel, "Pagination"));
    [Fact] public void PageChanged_CallbackSet() { var cut = Render<XFPagination>(p => p.Add(x => x.PageChanged, EventCallback.Factory.Create<int>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void PageSizeChanged_CallbackSet() { var cut = Render<XFPagination>(p => p.Add(x => x.PageSizeChanged, EventCallback.Factory.Create<int>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void Change_CallbackSet() { var cut = Render<XFPagination>(p => p.Add(x => x.Change, EventCallback.Factory.Create<XFPaginationChangedEventArgs>(this, v => { }))); Assert.NotNull(cut); }
    [Fact] public void OnReload_CallbackSet() { var cut = Render<XFPagination>(p => p.Add(x => x.OnReload, EventCallback.Factory.Create<XFPaginationChangedEventArgs>(this, v => { }))); Assert.NotNull(cut); }
}
