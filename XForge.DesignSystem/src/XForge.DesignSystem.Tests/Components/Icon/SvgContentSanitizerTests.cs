using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Icon;

public class SvgContentSanitizerTests
{
    [Fact]
    public void Sanitize_Null_ReturnsEmpty()
    {
        var result = SvgContentSanitizer.Sanitize(null);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Sanitize_Empty_ReturnsEmpty()
    {
        var result = SvgContentSanitizer.Sanitize(string.Empty);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Sanitize_Whitespace_ReturnsWhitespace()
    {
        var result = SvgContentSanitizer.Sanitize("   ");
        Assert.Equal("   ", result);
    }

    [Fact]
    public void Sanitize_NoComments_ReturnsSame()
    {
        var input = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path d=\"M12 2L2 22h20z\"/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.Equal(input.Trim(), result);
    }

    [Fact]
    public void Sanitize_StripsSimpleComment()
    {
        var input = "<svg><!-- comment --><path d=\"M0 0h24v24H0z\"/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.DoesNotContain("<!--", result);
        Assert.DoesNotContain("-->", result);
        Assert.Contains("<path", result);
    }

    [Fact]
    public void Sanitize_StripsMultiLineComment()
    {
        var input = "<svg><!-- foo \n bar --><path d=\"M0 0h24v24H0z\"/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.DoesNotContain("<!--", result);
        Assert.DoesNotContain("foo", result);
        Assert.DoesNotContain("bar", result);
    }

    [Fact]
    public void Sanitize_StripsFontAwesomeLicenseComment()
    {
        var input = "<!--! Font Awesome Pro 7.1.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2025 Fonticons, Inc. --><svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 512 512\"><path d=\"M...\"/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.DoesNotContain("Font Awesome", result);
        Assert.DoesNotContain("<!--", result);
        Assert.Contains("<svg", result);
    }

    [Fact]
    public void Sanitize_StripsMultipleComments()
    {
        var input = "<!-- first --><svg><!-- second --><path d=\"M1\"/><!-- third --></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.DoesNotContain("<!--", result);
        Assert.DoesNotContain("-->", result);
    }

    [Fact]
    public void Sanitize_PreservesSvgContent()
    {
        var input = "<!-- remove --><svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path d=\"M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z\"/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.Contains("<svg", result);
        Assert.Contains("<path", result);
        Assert.Contains("viewBox", result);
    }

    [Fact]
    public void Sanitize_PreservesTitleElement()
    {
        var input = "<svg xmlns=\"http://www.w3.org/2000/svg\"><title>My Icon</title><path d=\"M0 0h24v24H0z\"/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.Contains("<title>", result);
        Assert.Contains("My Icon", result);
        Assert.Contains("</title>", result);
    }

    [Fact]
    public void Sanitize_TrimsResult()
    {
        var result = SvgContentSanitizer.Sanitize("  <svg><path d=\"M1\"/></svg>  ");
        Assert.Equal("<svg><path d=\"M1\"/></svg>", result);
    }

    [Fact]
    public void Sanitize_EmptySvgAfterCommentRemoval_ReturnsEmpty()
    {
        var result = SvgContentSanitizer.Sanitize("<!-- only comments here -->");
        Assert.Equal(string.Empty, result);
    }
}
