using XForge.DesignSystem.Components.Emoji;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Emoji;

public class XFEmojiShortcodeParserTests
{
    [Fact]
    public void Parse_Null_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, XFEmojiShortcodeParser.Parse(null!));
    }

    [Fact]
    public void Parse_Empty_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, XFEmojiShortcodeParser.Parse(string.Empty));
    }

    [Fact]
    public void Parse_NoShortcodes_ReturnsSame()
    {
        Assert.Equal("hello world", XFEmojiShortcodeParser.Parse("hello world"));
    }

    [Fact]
    public void Parse_ValidShortcode_Replaces()
    {
        var result = XFEmojiShortcodeParser.Parse(":rocket:");
        Assert.Equal("🚀", result);
    }

    [Fact]
    public void Parse_MultipleShortcodes_ReplacesAll()
    {
        var result = XFEmojiShortcodeParser.Parse(":rocket: :fire:");
        Assert.Contains("🚀", result);
        Assert.Contains("🔥", result);
    }

    [Fact]
    public void Parse_CaseInsensitive()
    {
        var result = XFEmojiShortcodeParser.Parse(":Rocket:");
        Assert.Equal("🚀", result);
    }

    [Fact]
    public void Parse_UnknownShortcode_KeepsAsIs()
    {
        var result = XFEmojiShortcodeParser.Parse(":unknowncode123:");
        Assert.Equal(":unknowncode123:", result);
    }

    [Fact]
    public void Parse_MixedTextAndShortcodes()
    {
        var result = XFEmojiShortcodeParser.Parse("Hello :smile: world");
        Assert.Contains("Hello", result);
        Assert.Contains("world", result);
        Assert.DoesNotContain(":smile:", result);
    }

    [Fact]
    public void Reverse_Null_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, XFEmojiShortcodeParser.Reverse(null!));
    }

    [Fact]
    public void Reverse_Empty_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, XFEmojiShortcodeParser.Reverse(string.Empty));
    }

    [Fact]
    public void Reverse_NoEmoji_ReturnsSame()
    {
        Assert.Equal("hello world", XFEmojiShortcodeParser.Reverse("hello world"));
    }

    [Fact]
    public void Reverse_KnownEmoji_ReplacesWithShortcode()
    {
        var result = XFEmojiShortcodeParser.Reverse("🚀");
        Assert.Equal(":rocket:", result);
    }

    [Fact]
    public void TryResolve_ValidShortcode_ReturnsTrue()
    {
        var ok = XFEmojiShortcodeParser.TryResolve(":rocket:", out var emoji);
        Assert.True(ok);
        Assert.Equal("🚀", emoji);
    }

    [Fact]
    public void TryResolve_InvalidShortcode_ReturnsFalse()
    {
        var ok = XFEmojiShortcodeParser.TryResolve(":notarealshortcode:", out var emoji);
        Assert.False(ok);
        Assert.Empty(emoji);
    }

    [Fact]
    public void TryResolve_Null_ReturnsFalse()
    {
        var ok = XFEmojiShortcodeParser.TryResolve("", out var emoji);
        Assert.False(ok);
        Assert.Empty(emoji);
    }

    [Fact]
    public void RoundTrip_ParseThenReverse_ReturnsOriginal()
    {
        var original = "Hello :rocket: world";
        var parsed = XFEmojiShortcodeParser.Parse(original);
        var reversed = XFEmojiShortcodeParser.Reverse(parsed);
        Assert.Equal(original, reversed);
    }
}
