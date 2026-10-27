using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class SvgContentSanitizerTests
{
    [Fact]
    public void Sanitize_RemovesSingleLineComment()
    {
        var input = "<svg><!-- comment --><path/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.DoesNotContain("<!--", result);
    }

    [Fact]
    public void Sanitize_RemovesMultiLineComment()
    {
        var input = "<svg><!-- line1`r`nline2 --><path/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.DoesNotContain("<!--", result);
        Assert.Contains("<path/>", result);
    }

    [Fact]
    public void Sanitize_PreservesTitleElement()
    {
        var input = "<svg><title>Home</title><path/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.Contains("<title>Home</title>", result);
    }

    [Fact]
    public void Sanitize_RemovesFontAwesomeMetadata()
    {
        var input = "<!--! Font Awesome Pro 7.1.0 - Commercial License --><svg><path/></svg>";
        var result = SvgContentSanitizer.Sanitize(input);
        Assert.DoesNotContain("Font Awesome", result);
        Assert.DoesNotContain("Commercial License", result);
    }

    [Fact]
    public void Sanitize_HandlesEmptyAndNull()
    {
        Assert.Equal(string.Empty, SvgContentSanitizer.Sanitize(string.Empty));
        Assert.Equal(string.Empty, SvgContentSanitizer.Sanitize(null));
    }
}