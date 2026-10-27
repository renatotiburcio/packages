using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Accordion;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFAccordionItemTest : BunitContext
{
    public XFAccordionItemTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void Renders_Default()
    {
        var cut = Render<XFAccordionItem>();
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithTitle()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "Section 1"));
        Assert.Contains("Section 1", cut.Markup);
    }

    [Fact] public void Renders_WithDescription()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Description, "Desc"));
        Assert.Contains("Desc", cut.Markup);
    }

    [Fact] public void Renders_WithExpandedTrue()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Expanded, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithExpandedFalse()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Expanded, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithStartIcon()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.StartIcon, (RenderFragment)(b => b.AddContent(0, "icon"))));
        Assert.Contains("icon", cut.Markup);
    }

    [Fact] public void Renders_WithEndContent()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.EndContent, (RenderFragment)(b => b.AddContent(0, "end"))));
        Assert.Contains("end", cut.Markup);
    }

    [Fact] public void Renders_WithHeader()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Header, (RenderFragment)(b => b.AddContent(0, "custom header"))));
        Assert.Contains("custom header", cut.Markup);
    }

    [Fact] public void Renders_WithChildContent()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Expanded, true).Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "content"))));
        Assert.Contains("content", cut.Markup);
    }

    [Fact] public void Renders_WithLazyTrue()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Lazy, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithKeepAliveFalse()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.KeepAlive, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithExpandTitle()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.ExpandTitle, "Open me"));
        Assert.Contains("Open me", cut.Markup);
    }

    [Fact] public void Renders_WithCollapseTitle()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Expanded, true).Add(x => x.CollapseTitle, "Close me"));
        Assert.Contains("Close me", cut.Markup);
    }

    [Fact] public void Renders_WithExpandTitleFormat()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.ExpandTitleFormat, "Open {0}"));
        Assert.Contains("Open T", cut.Markup);
    }

    [Fact] public void Renders_WithCollapseTitleFormat()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Expanded, true).Add(x => x.CollapseTitleFormat, "Close {0}"));
        Assert.Contains("Close T", cut.Markup);
    }

    [Fact] public void Renders_WithClass()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Class, "my-item"));
        Assert.Contains("my-item", cut.Markup);
    }

    [Fact] public void Renders_WithId()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Id, "item1"));
        Assert.NotNull(cut.Find("#item1"));
    }

    [Fact] public void Renders_WithDisabled()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithExpandedChanged()
    {
        bool? changed = null;
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.ExpandedChanged, EventCallback.Factory.Create<bool>(this, val => changed = val)));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithChangeEvent()
    {
        XFAccordionItemChangeEventArgs? args = null;
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Change, EventCallback.Factory.Create<XFAccordionItemChangeEventArgs>(this, a => args = a)));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithStyle()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.Style, "color:red"));
        Assert.NotNull(cut);
    }

    [Fact] public void Renders_WithAdditionalAttributes()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T").Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-test", "value" } }));
        Assert.NotNull(cut);
    }

    [Fact] public void Dispose_DoesNotThrow()
    {
        var cut = Render<XFAccordionItem>(p => p.Add(x => x.Title, "T"));
        cut.Instance.Dispose();
    }
}
