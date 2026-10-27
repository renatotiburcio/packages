using XForge.DesignSystem.Components.Emoji;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class EmojiDataUriTests
{
    [Fact]
    public void ToDataUri_GeneratesBase64Svg()
    {
        var info = new XFEmojiInfo("rocket", XFEmojiGroup.TravelPlaces,
            XFEmojiStyle.Color, XFEmojiSkintone.Default, XFEmojiSize.Size32,
            System.Array.Empty<string>(), XFEmojiCompress.Zip("<svg><text>🚀</text></svg>"));
        var uri = XFEmojiDataUri.ToDataUri(info);
        Assert.StartsWith("data:image/svg+xml;base64,", uri);
    }

    [Fact]
    public void ToDataUri_EmptyData_ReturnsEmpty()
    {
        var info = new XFEmojiInfo("empty", XFEmojiGroup.Objects,
            XFEmojiStyle.Color, XFEmojiSkintone.Default, XFEmojiSize.Size32,
            System.Array.Empty<string>(), System.Array.Empty<byte>());
        Assert.Equal(string.Empty, XFEmojiDataUri.ToDataUri(info));
    }
}