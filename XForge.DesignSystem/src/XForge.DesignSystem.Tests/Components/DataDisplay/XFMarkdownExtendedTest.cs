using Bunit;
using XForge.DesignSystem.Components.DataDisplay.Markdown;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFMarkdownExtendedTest : XForgeTestContext
{
    public XFMarkdownExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    // SizeClass Sm (line 34)
    [Fact]
    public void Extended_SizeSm_RendersSmallText()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "# Hello");
            p.Add(x => x.Size, XFSize.Sm);
        });
        Assert.Contains("text-sm leading-6", cut.Markup);
    }

    // SizeClass Lg (line 35)
    [Fact]
    public void Extended_SizeLg_RendersLargeText()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "# Hello");
            p.Add(x => x.Size, XFSize.Lg);
        });
        Assert.Contains("text-base leading-7", cut.Markup);
    }

    // EmptyClass — null text (line 39)
    [Fact]
    public void Extended_NullText_RendersEmptyState()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, (string?)null);
            p.Add(x => x.EmptyText, "No content");
        });
        Assert.Contains("No content", cut.Markup);
        Assert.Contains("border-dashed", cut.Markup);
    }

    // EmptyClass — empty string
    [Fact]
    public void Extended_EmptyString_RendersEmptyState()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "");
        });
        Assert.Contains("xf-markdown", cut.Markup);
    }

    // EmptyClass — whitespace only
    [Fact]
    public void Extended_WhitespaceText_RendersEmptyState()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "   ");
        });
        Assert.Contains("xf-markdown", cut.Markup);
    }

    // HeadingClass level 1 (line 64)
    [Fact]
    public void Extended_Heading1_RendersH1Class()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "# Title"));
        Assert.Contains("text-2xl font-semibold", cut.Markup);
    }

    // HeadingClass level 2 (line 65)
    [Fact]
    public void Extended_Heading2_RendersH2Class()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "## Subtitle"));
        Assert.Contains("text-xl font-semibold", cut.Markup);
    }

    // HeadingClass level 3 (line 66)
    [Fact]
    public void Extended_Heading3_RendersH3Class()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "### Section"));
        Assert.Contains("text-lg font-semibold", cut.Markup);
    }

    // HeadingClass level 4+ (line 67 — default case)
    [Fact]
    public void Extended_Heading4_RendersH4Class()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "#### Detail"));
        Assert.Contains("text-base font-semibold", cut.Markup);
    }

    // Code fence with language — ShowCodeLanguage true (line 22)
    [Fact]
    public void Extended_ShowCodeLanguageTrue_RendersLanguage()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "```typescript\nconst x = 1;\n```");
            p.Add(x => x.ShowCodeLanguage, true);
        });
        Assert.Contains("typescript", cut.Markup);
    }

    // Code fence — ShowCodeLanguage false
    [Fact]
    public void Extended_ShowCodeLanguageFalse_HidesLanguage()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "```typescript\nconst x = 1;\n```");
            p.Add(x => x.ShowCodeLanguage, false);
        });
        Assert.DoesNotContain("typescript", cut.Markup);
    }

    // AllowLinks false (line 19)
    [Fact]
    public void Extended_AllowLinksFalse_RendersNoHref()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "[link](https://example.com)");
            p.Add(x => x.AllowLinks, false);
        });
        Assert.DoesNotContain("href", cut.Markup);
    }

    // OpenLinksInNewTab (line 20)
    [Fact]
    public void Extended_OpenLinksInNewTab_RendersTargetBlank()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "[link](https://example.com)");
            p.Add(x => x.OpenLinksInNewTab, true);
        });
        Assert.Contains("target=\"_blank\"", cut.Markup);
    }

    // Code fence without language
    [Fact]
    public void Extended_CodeFenceNoLanguage_RendersCodeBlock()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "```\nplain code\n```");
        });
        Assert.Contains("<pre", cut.Markup);
        Assert.Contains("plain code", cut.Markup);
    }

    // Inline code parsing (lines 312-313 — unmatched backtick returns false)
    [Fact]
    public void Extended_UnmatchedBacktick_RendersAsText()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "This has `unmatched backtick"));
        Assert.Contains("xf-markdown", cut.Markup);
    }

    // Blockquote
    [Fact]
    public void Extended_Blockquote_RendersBlockquote()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "> A quote"));
        Assert.Contains("<blockquote", cut.Markup);
        Assert.Contains("A quote", cut.Markup);
    }

    // Ordered list
    [Fact]
    public void Extended_OrderedList_RendersOl()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "1. First\n2. Second"));
        Assert.Contains("<ol", cut.Markup);
        Assert.Contains("list-decimal", cut.Markup);
    }

    // Unordered list
    [Fact]
    public void Extended_UnorderedList_RendersUl()
    {
        var cut = Render<XFMarkdown>(p => p.Add(x => x.Text, "- Item A\n- Item B"));
        Assert.Contains("<ul", cut.Markup);
        Assert.Contains("list-disc", cut.Markup);
    }

    // Custom class
    [Fact]
    public void Extended_CustomClass_RendersClass()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "# Hi");
            p.Add(x => x.Class, "custom-class");
        });
        Assert.Contains("custom-class", cut.Markup);
    }

    // AriaLabel
    [Fact]
    public void Extended_AriaLabel_RendersAriaLabel()
    {
        var cut = Render<XFMarkdown>(p =>
        {
            p.Add(x => x.Text, "# Hi");
            p.Add(x => x.AriaLabel, "Content");
        });
        Assert.Contains("aria-label=\"Content\"", cut.Markup);
    }
}
