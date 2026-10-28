using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Navigation.Breadcrumb;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public class XFBreadcrumbExtendedTest : XForgeTestContext
{
    public XFBreadcrumbExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFBreadcrumb> RenderBreadcrumb(
        Action<ComponentParameterCollectionBuilder<XFBreadcrumb>>? configure = null)
    {
        return Render<XFBreadcrumb>(p =>
        {
            p.Add(x => x.Items, new List<XFBreadcrumbItemModel>
            {
                new() { Text = "Home", Href = "/" },
                new() { Text = "Category", Href = "/cat" },
                new() { Text = "Current", Current = true }
            });
            configure?.Invoke(p);
        });
    }

    // VariantClass Plain (line 133)
    [Fact]
    public void Extended_VariantPlain_NoExtraClass()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.Contains("xf-breadcrumb", cut.Markup);
    }

    // VariantClass Soft (line 131)
    [Fact]
    public void Extended_VariantSoft_RendersSoft()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.Contains("px-2", cut.Markup);
    }

    // VariantClass Surface (line 132)
    [Fact]
    public void Extended_VariantSurface_RendersSurface()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.Variant, XFContainerVariant.Surface));
        Assert.Contains("border", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    // SeparatorClass Sm (line 124)
    [Fact]
    public void Extended_SeparatorSizeSm()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-5", cut.Markup);
        Assert.Contains("w-5", cut.Markup);
    }

    // SeparatorClass Lg (line 125)
    [Fact]
    public void Extended_SeparatorSizeLg()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-7", cut.Markup);
        Assert.Contains("w-7", cut.Markup);
    }

    // SeparatorClass default Md (line 126)
    [Fact]
    public void Extended_SeparatorSizeDefault()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("h-6", cut.Markup);
        Assert.Contains("w-6", cut.Markup);
    }

    // GetItemClass Sm (line 140)
    [Fact]
    public void Extended_ItemSizeSm()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-7", cut.Markup);
        Assert.Contains("text-xs", cut.Markup);
    }

    // GetItemClass Lg (line 141)
    [Fact]
    public void Extended_ItemSizeLg()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-9", cut.Markup);
        Assert.Contains("text-base", cut.Markup);
    }

    // GetItemClass default Md (line 142)
    [Fact]
    public void Extended_ItemSizeDefault()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("h-8", cut.Markup);
        Assert.Contains("text-sm", cut.Markup);
    }

    // GetItemClass Current item (line 146)
    [Fact]
    public void Extended_ItemCurrent_RendersCursorDefault()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("cursor-default", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // GetItemClass non-current item (line 147)
    [Fact]
    public void Extended_ItemNonCurrent_RendersHover()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    // ListClass (line 107)
    [Fact]
    public void Extended_ListClass_RendersFlex()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("items-center", cut.Markup);
    }

    // ItemShellClass (line 109)
    [Fact]
    public void Extended_ItemShellClass_Rendered()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("inline-flex", cut.Markup);
    }

    // CollapsedClass (line 111) — test via items that fit within MaxVisibleItems
    [Fact]
    public void Extended_CollapsedClass_NotTriggeredWhenFit()
    {
        // When items fit within MaxVisibleItems, no collapsed indicator is rendered
        var cut = RenderBreadcrumb(p =>
        {
            p.Add(x => x.MaxVisibleItems, 10);
            p.Add(x => x.CollapseMiddle, true);
        });
        Assert.DoesNotContain("bg-surface-100", cut.Markup);
    }

    // CollapseMiddle false shows all items (line 31)
    [Fact]
    public void Extended_CollapseMiddleFalse_ShowsAll()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.CollapseMiddle, false));
        Assert.DoesNotContain("hidden items", cut.Markup);
    }

    // MaxVisibleItems >= Items.Count shows all (line 31)
    [Fact]
    public void Extended_MaxVisibleItemsLarge_ShowsAll()
    {
        var cut = RenderBreadcrumb(p => p.Add(x => x.MaxVisibleItems, 10));
        Assert.DoesNotContain("hidden items", cut.Markup);
    }

    // SeparatorClass includes text-text-muted (line 124-126)
    [Fact]
    public void Extended_SeparatorClass_IncludesMuted()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // BaseClass included (line 26)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderBreadcrumb();
        Assert.Contains("xf-breadcrumb", cut.Markup);
    }
}
