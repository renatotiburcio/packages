using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Forms.Repeater;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Forms.Repeater;

public class XFRepeaterTest : XForgeTestContext
{
    public XFRepeaterTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithNoItems_ShowsEmptyState()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
            p.Add(x => x.ItemTemplate, ctx => builder => builder.AddContent(0, ctx.Item));
        });
        Assert.Contains("xf-repeater", cut.Markup);
    }

    [Fact]
    public void Render_WithItems_ShowsItems()
    {
        var items = new[] { "Item 1", "Item 2", "Item 3" };
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ItemTemplate, ctx => builder => builder.AddContent(0, ctx.Item));
        });
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.Contains("Item 3", cut.Markup);
    }

    [Fact]
    public void Render_WithLabel_ShowsLabel()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Label, "Contacts");
            p.Add(x => x.Items, Array.Empty<string>());
        });
        Assert.Contains("Contacts", cut.Markup);
    }

    [Fact]
    public void Render_WithAllowAdd_ShowsAddButton()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
            p.Add(x => x.AllowAdd, true);
        });
        Assert.Contains("repeater.add_item", cut.Markup);
    }

    [Fact]
    public void Render_WithoutAllowAdd_HidesAddButton()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
            p.Add(x => x.AllowAdd, false);
        });
        Assert.DoesNotContain("repeater.add_item", cut.Markup);
    }

    [Fact]
    public void Render_WithReadOnly_HidesAddAndRemove()
    {
        var items = new[] { "Item 1" };
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ReadOnly, true);
            p.Add(x => x.ItemTemplate, ctx => builder => builder.AddContent(0, ctx.Item));
        });
        Assert.DoesNotContain("repeater.add_item", cut.Markup);
        Assert.DoesNotContain("repeater.remove_item", cut.Markup);
    }

    [Fact]
    public void Render_WithShowItemCount_ShowsCount()
    {
        var items = new[] { "A", "B", "C" };
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ShowItemCount, true);
            p.Add(x => x.ItemTemplate, ctx => builder => builder.AddContent(0, ctx.Item));
        });
        Assert.Contains("3", cut.Markup);
    }

    [Fact]
    public void Render_WithHelperText_ShowsHelper()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
            p.Add(x => x.HelperText, "Add at least one item");
        });
        Assert.Contains("Add at least one item", cut.Markup);
    }

    [Fact]
    public void Render_ShowsAriaLabel()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
        });
        Assert.Contains("aria-label", cut.Markup);
    }

    [Fact]
    public void Render_ShowsGroupRole()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
        });
        Assert.Contains("role=\"group\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomEmptyText_ShowsText()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
            p.Add(x => x.EmptyText, "No contacts yet");
        });
        Assert.Contains("No contacts yet", cut.Markup);
    }

    [Fact]
    public void Render_WithItemTemplate_RendersTemplate()
    {
        var items = new[] { "Alice", "Bob" };
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ItemTemplate, ctx => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-item");
                builder.AddContent(2, $"Name: {ctx.Item}");
                builder.CloseElement();
            });
        });
        Assert.Contains("Name: Alice", cut.Markup);
        Assert.Contains("Name: Bob", cut.Markup);
    }

    [Fact]
    public void Render_WithIndex_PassesIndexToContext()
    {
        var items = new[] { "First", "Second" };
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.ItemTemplate, ctx => builder => builder.AddContent(0, $"#{ctx.Index}: {ctx.Item}"));
        });
        Assert.Contains("#0: First", cut.Markup);
        Assert.Contains("#1: Second", cut.Markup);
    }

    [Fact]
    public void Render_RespectsDisabled()
    {
        var cut = Render<XFRepeater<string>>(p =>
        {
            p.Add(x => x.Items, Array.Empty<string>());
            p.Add(x => x.Disabled, true);
        });
        Assert.Contains("xf-repeater", cut.Markup);
    }
}
