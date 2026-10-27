using XForge.DesignSystem.Components.Emoji;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class SkintonePickerTests
{
    [Fact]
    public void Skintones_AreOrdered_FromLightToDark()
    {
        var tones = System.Enum.GetValues<XFEmojiSkintone>();
        Assert.Equal(6, tones.Length);
    }

    [Fact]
    public void Default_Is_First()
    {
        Assert.Equal(0, (int)XFEmojiSkintone.Default);
    }
}