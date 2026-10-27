using XForge.DesignSystem.Components.Emoji;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class EmojiCatalogTests
{
    [Fact]
    public void Zip_AndUnzip_RoundTripsSvg()
    {
        var svg = "<svg><text>rocket</text></svg>";
        var zipped = XFEmojiCompress.Zip(svg);
        Assert.True(zipped.Length < svg.Length);
        Assert.Equal(svg, XFEmojiCompress.Unzip(zipped));
    }

    [Fact]
    public void Shortcode_Resolves_KnownCodes()
    {
        Assert.Equal("🚀", XFEmojiShortcodes.Resolve(":rocket:"));
        Assert.Equal("🔥", XFEmojiShortcodes.Resolve(":fire:"));
    }

    [Fact]
    public void Shortcode_Unknown_ReturnsNull()
    {
        Assert.Null(XFEmojiShortcodes.Resolve(":nonexistent-emoji-xyz:"));
    }

    [Fact]
    public void Register_AndGet_RoundTrips()
    {
        var info = new XFEmojiInfo("rocket", XFEmojiGroup.TravelPlaces,
            XFEmojiStyle.Color, XFEmojiSkintone.Default, XFEmojiSize.Size32,
            new[] { "launch", "ship" }, XFEmojiCompress.Zip("<svg/>"));
        XFEmojiCatalog.Register(info);
        var fetched = XFEmojiCatalog.Get("rocket", XFEmojiStyle.Color, XFEmojiSize.Size32);
        Assert.NotNull(fetched);
    }

    [Fact]
    public void Search_EmptyTerm_ReturnsNothing()
    {
        Assert.Empty(XFEmojiCatalog.Search(""));
    }
}