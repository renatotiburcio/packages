using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Navigation.Pagination;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFPaginationExtendedTest : XForgeTestContext
{
    public XFPaginationExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFPagination> RenderPagination(
        int totalItems = 100, int page = 3,
        Action<ComponentParameterCollectionBuilder<XFPagination>>? configure = null)
    {
        return Render<XFPagination>(p =>
        {
            p.Add(x => x.TotalItems, totalItems);
            p.Add(x => x.Page, page);
            configure?.Invoke(p);
        });
    }

    // GetComponentClass variant branches (lines 121-123)
    [Fact]
    public void Extended_VariantPlain_RendersPlainClass()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.Contains("gap-3", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_RendersSoftClass()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSurface_RendersSurfaceClass()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Variant, XFContainerVariant.Surface));
        Assert.Contains("border-border-light", cut.Markup);
    }

    // SummaryText empty (lines 94-95)
    [Fact]
    public void Extended_ZeroTotalItems_RendersEmptySummary()
    {
        var cut = RenderPagination(totalItems: 0);
        Assert.Contains("0 itens", cut.Markup);
    }

    // SummaryClass size branches (line 281)
    [Fact]
    public void Extended_SizeSm_RendersSmallSummary()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("text-xs", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_RendersLargeSummary()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("text-base", cut.Markup);
    }

    // EllipsisClass size branches (lines 291-292)
    [Fact]
    public void Extended_SizeSm_RendersSmallEllipsis()
    {
        var cut = RenderPagination(totalItems: 2400, page: 42,
            configure: p => p.Add(x => x.Size, XFSize.Sm));
        Assert.Contains("h-7", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_RendersLargeEllipsis()
    {
        var cut = RenderPagination(totalItems: 2400, page: 42,
            configure: p => p.Add(x => x.Size, XFSize.Lg));
        Assert.Contains("h-10", cut.Markup);
    }

    // NavButtonClass Lg (line 317)
    [Fact]
    public void Extended_SizeLg_RendersLargeNavButtons()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Size, XFSize.Lg));
        var firstBtn = cut.Find("[data-xf-pagination-first]");
        Assert.Contains("h-10 w-10", firstBtn.GetAttribute("class"));
    }

    // PageButtonClass Lg (line 301)
    [Fact]
    public void Extended_SizeLg_RendersLargePageButtons()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Size, XFSize.Lg));
        var pageBtn = cut.Find("[data-xf-pagination-page='3']");
        Assert.Contains("h-10", pageBtn.GetAttribute("class"));
    }

    // PageButtonClass Sm (line 300)
    [Fact]
    public void Extended_SizeSm_RendersSmallPageButtons()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Size, XFSize.Sm));
        var pageBtn = cut.Find("[data-xf-pagination-page='3']");
        Assert.Contains("h-7", pageBtn.GetAttribute("class"));
    }

    // OnParametersSet validation (lines 107-109, 112-114)
    [Fact]
    public void Extended_NegativeSiblingCount_ClampedToZero()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.SiblingCount, -1));
        Assert.Contains("xf-pagination", cut.Markup);
    }

    [Fact]
    public void Extended_NegativeBoundaryCount_ClampedToZero()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.BoundaryCount, -1));
        Assert.Contains("xf-pagination", cut.Markup);
    }

    // PageTemplate (lines 245-247)
    [Fact]
    public void Extended_PageTemplate_RendersCustomTemplate()
    {
        var cut = RenderPagination(configure: p =>
        {
            p.Add(x => x.PageTemplate, (int page) => builder => builder.AddContent(0, $"P{page}"));
        });
        Assert.Contains("P3", cut.Markup);
    }

    // ShowPageSize
    [Fact]
    public void Extended_ShowPageSize_RendersPageSizeSelector()
    {
        var cut = RenderPagination(configure: p =>
        {
            p.Add(x => x.ShowPageSize, true);
            p.Add(x => x.PageSizeOptions, new[] { 10, 25, 50 });
        });
        Assert.Contains("Itens por página", cut.Markup);
    }

    // Density Compact (line 276)
    [Fact]
    public void Extended_DensityCompact_RendersCompactButtons()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Density, XFDensity.Compact));
        Assert.Contains("gap-1", cut.Markup);
    }

    // Disabled state
    [Fact]
    public void Extended_Disabled_RendersDisabledButtons()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.Disabled, true));
        var firstBtn = cut.Find("[data-xf-pagination-first]");
        Assert.NotNull(firstBtn.GetAttribute("disabled"));
    }

    // ShowSummary false
    [Fact]
    public void Extended_ShowSummaryFalse_HidesSummary()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.ShowSummary, false));
        Assert.DoesNotContain("de 100", cut.Markup);
    }

    // ShowFirstLast false
    [Fact]
    public void Extended_ShowFirstLastFalse_HidesFirstLastButtons()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.ShowFirstLast, false));
        Assert.DoesNotContain("data-xf-pagination-first", cut.Markup);
        Assert.DoesNotContain("data-xf-pagination-last", cut.Markup);
    }

    // ShowPrevNext false
    [Fact]
    public void Extended_ShowPrevNextFalse_HidesPrevNextButtons()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.ShowPrevNext, false));
        Assert.DoesNotContain("data-xf-pagination-previous", cut.Markup);
        Assert.DoesNotContain("data-xf-pagination-next", cut.Markup);
    }

    // ShowPageNumbers false
    [Fact]
    public void Extended_ShowPageNumbersFalse_HidesPageNumbers()
    {
        var cut = RenderPagination(configure: p => p.Add(x => x.ShowPageNumbers, false));
        Assert.DoesNotContain("data-xf-pagination-page", cut.Markup);
    }
}
