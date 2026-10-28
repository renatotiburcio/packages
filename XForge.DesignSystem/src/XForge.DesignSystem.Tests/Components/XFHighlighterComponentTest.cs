using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Code;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public class XFHighlighterComponentTest : XForgeTestContext
{
    public XFHighlighterComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithNoText_EmptyOutput()
    {
        var cut = Render<XFHighlighter>();
        Assert.DoesNotContain("mark", cut.Markup);
    }

    [Fact]
    public void Render_WithTextOnly()
    {
        var cut = Render<XFHighlighter>(parameters =>
            parameters.Add(p => p.Text, "Hello world"));
        Assert.Contains("Hello world", cut.Markup);
    }

    [Fact]
    public void Render_WithHighlightedText_WrapsInMark()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "Search for keyword here");
            parameters.Add(p => p.HighlightedText, "keyword");
        });
        Assert.Contains("<mark", cut.Markup);
        Assert.Contains("keyword", cut.Markup);
    }

    [Fact]
    public void Render_WithHighlightedTerms_AllWrapped()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "Use var and string types");
            parameters.Add(p => p.HighlightedTexts, new[] { "var", "string" });
        });
        Assert.Equal(2, cut.FindAll("mark").Count);
    }

    [Fact]
    public void Render_CaseSensitive_OnlyMatchesExactCase()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "NET net Net");
            parameters.Add(p => p.HighlightedText, "NET");
            parameters.Add(p => p.CaseSensitive, true);
        });
        Assert.Single(cut.FindAll("mark"));
    }

    [Fact]
    public void Render_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.Add(p => p.Text, "Hidden text");
        });
        Assert.DoesNotContain("xf-highlighter", cut.Markup);
    }
}
