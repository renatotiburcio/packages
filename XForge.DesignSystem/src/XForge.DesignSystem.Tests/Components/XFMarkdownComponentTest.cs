#region Using directives
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Markdown;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFMarkdownComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_BasicMarkdownBlocks()
    {
        var markdown = """
            # Title

            Hello **strong** and *emphasis* with `code`.

            - First
            - Second

            1. One
            2. Two

            > Quote
            """;

        var cut = Render<XFMarkdown>(parameters => parameters.Add(p => p.Text, markdown));

        Assert.Contains("<h1", cut.Markup);
        Assert.Contains("<strong", cut.Markup);
        Assert.Contains("<em", cut.Markup);
        Assert.Contains("<code", cut.Markup);
        Assert.Contains("<ul", cut.Markup);
        Assert.Contains("<ol", cut.Markup);
        Assert.Contains("<blockquote", cut.Markup);
    }

    [Fact]
    public void Render_CodeFenceWithLanguage()
    {
        var cut = Render<XFMarkdown>(parameters => parameters.Add(p => p.Text, "```csharp\nvar x = 1;\n```"));

        Assert.Contains("csharp", cut.Markup);
        Assert.Contains("Copy", cut.Markup);
        Assert.Contains("var x = 1;", cut.Markup);
        Assert.Contains("<pre", cut.Markup);
    }

    [Fact]
    public void Render_CodeFence_WhenCopyDisabled_HidesCopyButton()
    {
        var cut = Render<XFMarkdown>(parameters =>
        {
            parameters.Add(p => p.Text, "```csharp\nvar x = 1;\n```");
            parameters.Add(p => p.ShowCodeCopy, false);
        });

        Assert.DoesNotContain("Copy", cut.Markup);
    }

    [Fact]
    public void Render_EscapesHtmlInsteadOfRenderingIt()
    {
        var cut = Render<XFMarkdown>(parameters => parameters.Add(p => p.Text, "<script>alert(1)</script>"));

        Assert.DoesNotContain("<script>", cut.Markup);
        Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", cut.Markup);
    }

    [Fact]
    public void Render_RemovesUnsafeJavascriptLink()
    {
        var cut = Render<XFMarkdown>(parameters => parameters.Add(p => p.Text, "[bad](javascript:alert(1))"));

        Assert.DoesNotContain("href=\"javascript:alert(1)\"", cut.Markup);
        Assert.Contains("bad", cut.Markup);
    }

    [Fact]
    public void Render_SafeLinkWithNewTabAttributes()
    {
        var cut = Render<XFMarkdown>(parameters =>
        {
            parameters.Add(p => p.Text, "[docs](https://example.com)");
            parameters.Add(p => p.OpenLinksInNewTab, true);
        });

        Assert.Contains("href=\"https://example.com\"", cut.Markup);
        Assert.Contains("target=\"_blank\"", cut.Markup);
        Assert.Contains("rel=\"noopener noreferrer\"", cut.Markup);
    }

    [Fact]
    public void Render_EmptyState()
    {
        var cut = Render<XFMarkdown>(parameters => parameters.Add(p => p.EmptyText, "Nothing here."));

        Assert.Contains("Nothing here.", cut.Markup);
    }
}
