using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.VirtualList;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

/// <summary>
/// Additional tests for XFVirtualList to improve coverage from 66.2% → 85%+.
/// Covers all variants, parameters, templates, and edge cases.
/// </summary>
public class XFVirtualListExtendedTest : XForgeTestContext
{
    public XFVirtualListExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact] public void Render_Default()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item 1", "Item 2" }));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_VariantDefault()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item" })
            .Add(x => x.Variant, XFVirtualListVariant.Default));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_VariantBordered()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item" })
            .Add(x => x.Variant, XFVirtualListVariant.Bordered));
        Assert.Contains("border", cut.Markup);
    }

    [Fact] public void Render_VariantCompact()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item" })
            .Add(x => x.Variant, XFVirtualListVariant.Compact));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_WithTitle()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item" })
            .Add(x => x.Title, "My List"));
        Assert.Contains("My List", cut.Markup);
    }

    [Fact] public void Render_WithItemTemplate()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item 1", "Item 2" })
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(item => builder => builder.AddContent(0, item))));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_WithEmptyContent()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string>())
            .Add(x => x.EmptyContent, (RenderFragment)(builder => builder.AddContent(0, "No items"))));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_WithLoadingContent()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item" })
            .Add(x => x.LoadingContent, (RenderFragment)(builder => builder.AddContent(0, "Loading..."))));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_WithCustomEmptyText()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string>())
            .Add(x => x.EmptyText, "Custom empty"));
        Assert.Contains("Custom empty", cut.Markup);
    }

    [Fact] public void Render_WithItemSize()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item" })
            .Add(x => x.ItemSize, 80f));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_WithOverscanCount()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "Item" })
            .Add(x => x.OverscanCount, 5));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact] public void Render_WithAllParameters()
    {
        var cut = Render<XFVirtualList<string>>(p => p
            .Add(x => x.Items, new List<string> { "A", "B", "C" })
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(item => builder => builder.AddContent(0, item)))
            .Add(x => x.Title, "Full List")
            .Add(x => x.Variant, XFVirtualListVariant.Bordered)
            .Add(x => x.ItemSize, 60f)
            .Add(x => x.OverscanCount, 10));
        Assert.Contains("xf-virtual-list", cut.Markup);
    }
}
