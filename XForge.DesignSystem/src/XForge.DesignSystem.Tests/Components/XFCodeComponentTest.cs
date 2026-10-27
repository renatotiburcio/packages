using XForge.DesignSystem.Core.Primitives;
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Code;

namespace XForge.DesignSystem.Tests.Components;

public class XFCodeComponentTest : BunitContext
{
    [Fact]
    public void Render_Inline_RendersCodeElement()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Text, "data-theme");
            parameters.Add(p => p.Tone, XFSemanticColor.Primary);
        });

        var code = cut.Find("code");
        Assert.Equal("data-theme", code.TextContent.Trim());
        Assert.Contains("text-primary", code.GetAttribute("class"));
    }

    [Fact]
    public void Render_Block_RendersPreAndHeader()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Language, "razor");
            parameters.Add(p => p.Title, "Example");
            parameters.Add(p => p.Text, "<XFButton />");
        });

        Assert.NotNull(cut.Find("pre"));
        Assert.Contains("razor", cut.Markup);
        Assert.Contains("Example", cut.Markup);
        Assert.Contains("&lt;XFButton /&gt;", cut.Markup);
    }

    [Fact]
    public void Render_Copyable_AddsCopyButton()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Text, "Console.WriteLine();");
            parameters.Add(p => p.Copyable, true);
            parameters.Add(p => p.CopyLabel, "Copy code");
            parameters.Add(p => p.CopyText, "Copy");
        });

        var button = cut.Find("button");
        Assert.Equal("Copy code", button.GetAttribute("aria-label"));
        Assert.Contains("Copy", button.TextContent);
    }

    [Fact]
    public void Highlighter_MarksMatchingText()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "Buscar componentes e tokens");
            parameters.Add(p => p.HighlightedTexts, new[] { "componentes", "tokens" });
        });

        var marks = cut.FindAll("mark");
        Assert.Equal(2, marks.Count);
        Assert.Equal("componentes", marks[0].TextContent);
        Assert.Equal("tokens", marks[1].TextContent);
    }

    [Fact]
    public void Highlighter_WhenNoTerm_RendersPlainText()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "Sem destaque");
        });

        Assert.Empty(cut.FindAll("mark"));
        Assert.Contains("Sem destaque", cut.Markup);
    }

    [Fact]
    public void Render_Inline_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.Add(p => p.Text, "hidden");
        });

        Assert.DoesNotContain("hidden", cut.Markup);
    }

    [Fact]
    public void Render_Block_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Visible, false);
            parameters.Add(p => p.Text, "hidden");
        });

        Assert.DoesNotContain("hidden", cut.Markup);
    }

    [Fact]
    public void Render_Inline_PrimaryTone_HasPrimaryClass()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Text, "primary");
            parameters.Add(p => p.Variant, XFCodeVariant.Inline);
            parameters.Add(p => p.Tone, XFSemanticColor.Primary);
        });

        Assert.Contains("bg-primary/8", cut.Markup);
        Assert.Contains("text-primary", cut.Markup);
    }

    [Fact]
    public void Render_Inline_SuccessTone_HasSuccessClass()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Text, "success");
            parameters.Add(p => p.Variant, XFCodeVariant.Inline);
            parameters.Add(p => p.Tone, XFSemanticColor.Success);
        });

        Assert.Contains("bg-success/8", cut.Markup);
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Render_Inline_WarningTone_HasWarningClass()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Text, "warning");
            parameters.Add(p => p.Variant, XFCodeVariant.Inline);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
        });

        Assert.Contains("bg-warning/10", cut.Markup);
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Render_Inline_DangerTone_HasDangerClass()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Text, "danger");
            parameters.Add(p => p.Variant, XFCodeVariant.Inline);
            parameters.Add(p => p.Tone, XFSemanticColor.Danger);
        });

        Assert.Contains("bg-danger/8", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Render_Block_WrapTrue_HasPreWrapClass()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Wrap, true);
            parameters.Add(p => p.Text, "wrapped");
        });

        Assert.Contains("whitespace-pre-wrap", cut.Markup);
    }

    [Fact]
    public void Render_Block_WrapFalse_HasPreClass()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Wrap, false);
            parameters.Add(p => p.Text, "not-wrapped");
        });

        Assert.Contains("whitespace-pre", cut.Markup);
        Assert.DoesNotContain("pre-wrap", cut.Markup);
    }

    [Fact]
    public void Render_Block_NoTitleNoLanguage_HidesHeader()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Text, "no header");
            parameters.Add(p => p.Copyable, false);
        });

        Assert.DoesNotContain("border-b", cut.Markup);
    }

    [Fact]
    public void Render_Block_LanguageOnly_ShowsLanguageBadge()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Language, "csharp");
            parameters.Add(p => p.Text, "code");
        });

        Assert.Contains("csharp", cut.Markup);
        Assert.Contains("Language: csharp", cut.Markup);
    }

    [Fact]
    public void Render_Block_TitleOnly_ShowsTitle()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Title, "My Code");
            parameters.Add(p => p.Text, "code");
        });

        Assert.Contains("My Code", cut.Markup);
    }

    [Fact]
    public void Render_Inline_WithCustomClass()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Text, "custom");
            parameters.Add(p => p.Class, "my-custom-class");
        });

        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Render_Block_PrimaryTone_HasPrimaryBorder()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Tone, XFSemanticColor.Primary);
            parameters.Add(p => p.Text, "primary block");
        });

        Assert.Contains("border-primary/18", cut.Markup);
    }

    [Fact]
    public void Render_Block_DangerTone_HasDangerBorder()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Tone, XFSemanticColor.Danger);
            parameters.Add(p => p.Text, "danger block");
        });

        Assert.Contains("border-danger/18", cut.Markup);
    }

    [Fact]
    public void Render_Inline_RoleCodeAttribute()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Text, "role test");
        });

        var code = cut.Find("code");
        Assert.Equal("code", code.GetAttribute("role"));
    }

    [Fact]
    public void Render_Block_PreHasRegionRole()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Text, "region test");
        });

        var pre = cut.Find("pre");
        Assert.Equal("region", pre.GetAttribute("role"));
    }

    [Fact]
    public void Render_Block_PreHasAriaLabel()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Title, "My snippet");
            parameters.Add(p => p.Text, "snippet");
        });

        var pre = cut.Find("pre");
        Assert.Equal("My snippet", pre.GetAttribute("aria-label"));
    }

    [Fact]
    public void Render_Copyable_DisabledButton()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Text, "disabled");
            parameters.Add(p => p.Copyable, true);
            parameters.Add(p => p.Disabled, true);
        });

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_Copyable_CopiedTextCustom()
    {
        var cut = Render<XFCode>(parameters =>
        {
            parameters.Add(p => p.Variant, XFCodeVariant.Block);
            parameters.Add(p => p.Text, "code");
            parameters.Add(p => p.Copyable, true);
            parameters.Add(p => p.CopyText, "Copiar");
            parameters.Add(p => p.CopiedText, "Copiado");
            parameters.Add(p => p.CopyLabel, "Copiar codigo");
        });

        Assert.Contains("Copiar", cut.Markup);
        Assert.Contains("Copiar codigo", cut.Markup);
    }

    [Fact]
    public void Highlighter_CaseSensitive_MatchesExactCase()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "Hello HELLO hello");
            parameters.Add(p => p.HighlightedText, "hello");
            parameters.Add(p => p.CaseSensitive, true);
        });

        var marks = cut.FindAll("mark");
        Assert.Single(marks);
        Assert.Equal("hello", marks[0].TextContent);
    }

    [Fact]
    public void Highlighter_CaseInsensitive_MatchesAllCases()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "Hello HELLO hello");
            parameters.Add(p => p.HighlightedText, "hello");
            parameters.Add(p => p.CaseSensitive, false);
        });

        var marks = cut.FindAll("mark");
        Assert.Equal(3, marks.Count);
    }

    [Fact]
    public void Highlighter_WholeWord_DoesNotMatchPartial()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "testing tester tests");
            parameters.Add(p => p.HighlightedText, "test");
            parameters.Add(p => p.WholeWord, true);
        });

        var marks = cut.FindAll("mark");
        Assert.Empty(marks);
    }

    [Fact]
    public void Highlighter_WholeWord_MatchesExactWord()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "the test is a test");
            parameters.Add(p => p.HighlightedText, "test");
            parameters.Add(p => p.WholeWord, true);
        });

        var marks = cut.FindAll("mark");
        Assert.Equal(2, marks.Count);
    }

    [Fact]
    public void Highlighter_WhenNotVisible_DoesNotRender()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.Add(p => p.Text, "hidden");
        });

        Assert.DoesNotContain("hidden", cut.Markup);
    }

    [Fact]
    public void Highlighter_SingleHighlightText()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "Highlight this word");
            parameters.Add(p => p.HighlightedText, "this");
        });

        var marks = cut.FindAll("mark");
        Assert.Single(marks);
        Assert.Equal("this", marks[0].TextContent);
    }

    [Fact]
    public void Highlighter_EmptyText_RendersNoMarks()
    {
        var cut = Render<XFHighlighter>(parameters =>
        {
            parameters.Add(p => p.Text, "");
            parameters.Add(p => p.HighlightedText, "test");
        });

        Assert.Empty(cut.FindAll("mark"));
    }
}
