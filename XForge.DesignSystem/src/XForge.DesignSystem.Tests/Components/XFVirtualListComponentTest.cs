using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.VirtualList;

namespace XForge.DesignSystem.Tests.Components;

public class XFVirtualListComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithItems_ShowsContent()
    {
        var items = Enumerable.Range(1, 100).Select(i => $"Item {i}").ToArray();

        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.Items, items);
            parameters.Add(p => p.ItemTemplate, (string item) => (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, item)));
        });

        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact]
    public void Render_WithEmptyItems_ShowsEmptyText()
    {
        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<string>());
            parameters.Add(p => p.EmptyText, "No items found");
        });

        Assert.Contains("No items found", cut.Markup);
    }

    [Fact]
    public void Render_WithNullItems_ShowsEmptyText()
    {
        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.EmptyText, "Nothing to display");
        });

        Assert.Contains("Nothing to display", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomEmptyContent_RendersContent()
    {
        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.EmptyContent, (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                b.AddMarkupContent(0, "<div>Custom empty</div>")));
        });

        Assert.Contains("Custom empty", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle_ShowsTitle()
    {
        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "A" });
            parameters.Add(p => p.Title, "Employees");
        });

        Assert.Contains("Employees", cut.Markup);
    }

    [Fact]
    public void Render_BorderedVariant_RendersBorder()
    {
        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "A" });
            parameters.Add(p => p.Variant, XFVirtualListVariant.Bordered);
        });

        Assert.Contains("border border-border-light", cut.Markup);
    }

    [Fact]
    public void Render_CompactVariant_RendersNoExtra()
    {
        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "A" });
            parameters.Add(p => p.Variant, XFVirtualListVariant.Compact);
        });

        Assert.DoesNotContain("rounded-lg", cut.Markup);
    }

    [Fact]
    public void XFVirtualListVariant_HasExpectedValues()
    {
        Assert.Equal(3, Enum.GetValues<XFVirtualListVariant>().Length);
    }

    [Fact]
    public void Render_WithItemsProvider_ShowsVirtualizedContent()
    {
        var items = Enumerable.Range(1, 1000).Select(i => $"Employee {i}").ToList();

        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.ItemsProvider, new Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderDelegate<string>((request) =>
            {
                var page = items.Skip(request.StartIndex).Take(request.Count).ToList();
                return new ValueTask<Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderResult<string>>(
                    new Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderResult<string>(page, items.Count));
            }));
            parameters.Add(p => p.ItemTemplate, (string item) => (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, item)));
        });

        Assert.Contains("xf-virtual-list", cut.Markup);
    }

    [Fact]
    public void Render_DefaultVariant_RendersBgSurface()
    {
        var cut = Render<XFVirtualList<string>>(parameters =>
        {
            parameters.Add(p => p.Items, new[] { "A" });
        });

        Assert.Contains("bg-bg-surface", cut.Markup);
    }
}
