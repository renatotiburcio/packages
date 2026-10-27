using XForge.DesignSystem.Components.Emoji;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class EmojiShortcodeParserTests
{
    [Fact]
    public void Parse_Rocket_ReturnsCorrectEmoji()
    {
        Assert.Equal("Ready 🚀", XFEmojiShortcodeParser.Parse("Ready :rocket:"));
    }

    [Fact]
    public void Parse_CaseInsensitive()
    {
        Assert.Equal("🚀", XFEmojiShortcodeParser.Parse(":ROCKET:"));
        Assert.Equal("🚀", XFEmojiShortcodeParser.Parse(":Rocket:"));
    }

    [Fact]
    public void Parse_UnknownShortcode_LeavesAsIs()
    {
        Assert.Equal(":nonexistent-xyz:", XFEmojiShortcodeParser.Parse(":nonexistent-xyz:"));
    }

    [Fact]
    public void Reverse_Rocket_ReturnsShortcode()
    {
        Assert.Equal(":rocket:", XFEmojiShortcodeParser.Reverse("🚀"));
    }

    [Fact]
    public void TryResolve_KnownShortcode_ReturnsTrue()
    {
        Assert.True(XFEmojiShortcodeParser.TryResolve(":fire:", out var emoji));
        Assert.Equal("🔥", emoji);
    }

    [Fact]
    public void TryResolve_UnknownShortcode_ReturnsFalse()
    {
        Assert.False(XFEmojiShortcodeParser.TryResolve(":xyz:", out _));
    }

    [Fact]
    public void Parse_EmptyOrNull_ReturnsInput()
    {
        Assert.Equal(string.Empty, XFEmojiShortcodeParser.Parse(string.Empty));
        Assert.Equal(string.Empty, XFEmojiShortcodeParser.Parse(null));
    }
}