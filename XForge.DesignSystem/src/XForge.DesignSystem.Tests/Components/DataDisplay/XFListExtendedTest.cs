using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.List;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFListExtendedTest : BunitContext
{
    public XFListExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFList> RenderList(
        Action<ComponentParameterCollectionBuilder<XFList>>? configure = null)
    {
        return Render<XFList>(p =>
        {
            p.AddChildContent("<div>Item</div>");
            configure?.Invoke(p);
        });
    }

    // VariantClass Plain (line 36)
    [Fact]
    public void Extended_VariantPlain_RendersGrid()
    {
        var cut = RenderList(p => p.Add(x => x.Variant, XFListVariant.Plain));
        Assert.Contains("grid", cut.Markup);
        Assert.DoesNotContain("rounded-lg", cut.Markup);
    }

    // VariantClass Soft (line 37)
    [Fact]
    public void Extended_VariantSoft_RendersSoft()
    {
        var cut = RenderList(p => p.Add(x => x.Variant, XFListVariant.Soft));
        Assert.Contains("bg-surface-50", cut.Markup);
        Assert.Contains("ring-1", cut.Markup);
        Assert.Contains("ring-border-light", cut.Markup);
    }

    // VariantClass Bordered (default) (line 38)
    [Fact]
    public void Extended_VariantBordered_RendersBordered()
    {
        var cut = RenderList(p => p.Add(x => x.Variant, XFListVariant.Bordered));
        Assert.Contains("border", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    // SelectionMode None renders role=list (line 25)
    [Fact]
    public void Extended_SelectionModeNone_RendersListRole()
    {
        var cut = RenderList(p => p.Add(x => x.SelectionMode, XFListSelectionMode.None));
        Assert.Equal("list", cut.Find("div").GetAttribute("role"));
    }

    // SelectionMode Single renders role=listbox (line 25)
    [Fact]
    public void Extended_SelectionModeSingle_RendersListboxRole()
    {
        var cut = RenderList(p => p.Add(x => x.SelectionMode, XFListSelectionMode.Single));
        Assert.Equal("listbox", cut.Find("div").GetAttribute("role"));
    }

    // SelectionMode Multiple renders aria-multiselectable (line 27)
    [Fact]
    public void Extended_SelectionModeMultiple_RendersMultiselectable()
    {
        var cut = RenderList(p => p.Add(x => x.SelectionMode, XFListSelectionMode.Multiple));
        Assert.Equal("true", cut.Find("div").GetAttribute("aria-multiselectable"));
    }

    // SelectionMode None renders no aria-multiselectable (line 27)
    [Fact]
    public void Extended_SelectionModeNone_NoMultiselectable()
    {
        var cut = RenderList(p => p.Add(x => x.SelectionMode, XFListSelectionMode.None));
        Assert.Null(cut.Find("div").GetAttribute("aria-multiselectable"));
    }

    // EmptyContent renders when no ChildContent (line 17-19)
    [Fact]
    public void Extended_EmptyContent_RendersCustom()
    {
        var cut = Render<XFList>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment?)null);
            p.Add(x => x.EmptyContent, builder => builder.AddContent(0, "No items"));
        });
        Assert.Contains("No items", cut.Markup);
    }

    // EmptyText renders default empty (line 22-23)
    [Fact]
    public void Extended_EmptyText_RendersDefault()
    {
        var cut = Render<XFList>(p => p.Add(x => x.ChildContent, (RenderFragment?)null));
        Assert.Contains("Nenhum item para exibir", cut.Markup);
    }

    // EmptyClass renders with proper styling (line 86)
    [Fact]
    public void Extended_EmptyClass_RendersStyled()
    {
        var cut = Render<XFList>(p => p.Add(x => x.ChildContent, (RenderFragment?)null));
        Assert.Contains("px-4", cut.Markup);
        Assert.Contains("py-8", cut.Markup);
        Assert.Contains("text-center", cut.Markup);
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // Visible false renders nothing (line 4)
    [Fact]
    public void Extended_VisibleFalse_RendersNothing()
    {
        var cut = RenderList(p => p.Add(x => x.Visible, false));
        Assert.Equal(string.Empty, cut.Markup);
    }

    // GetComponentClass includes BaseClass (line 31)
    [Fact]
    public void Extended_GetComponentClass_IncludesBaseClass()
    {
        var cut = RenderList();
        Assert.Contains("xf-list", cut.Markup);
    }

    // VariantClass default (Bordered) includes shadow (line 38)
    [Fact]
    public void Extended_VariantBordered_IncludesShadow()
    {
        var cut = RenderList(p => p.Add(x => x.Variant, XFListVariant.Bordered));
        Assert.Contains("shadow-", cut.Markup);
    }

    // VariantClass Plain has no border/ring/bg (line 36)
    [Fact]
    public void Extended_VariantPlain_NoBorderOrBg()
    {
        var cut = RenderList(p => p.Add(x => x.Variant, XFListVariant.Plain));
        Assert.DoesNotContain("border", cut.Markup);
        Assert.DoesNotContain("ring-", cut.Markup);
        Assert.DoesNotContain("bg-surface", cut.Markup);
    }
}
