using XForge.DesignSystem.Components.Emoji;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class FluentEmojiTests
{
    [Fact]
    public void XFEmojiStyle_HasThreeValues()
    {
        var values = System.Enum.GetValues<XFEmojiStyle>();
        Assert.Equal(3, values.Length);
        Assert.Contains(XFEmojiStyle.Color, values);
        Assert.Contains(XFEmojiStyle.Flat, values);
        Assert.Contains(XFEmojiStyle.HighContrast, values);
    }

    [Fact]
    public void XFEmojiSkintone_HasSixValues()
    {
        var values = System.Enum.GetValues<XFEmojiSkintone>();
        Assert.Equal(6, values.Length);
        Assert.Contains(XFEmojiSkintone.Default, values);
        Assert.Contains(XFEmojiSkintone.Light, values);
        Assert.Contains(XFEmojiSkintone.Medium, values);
        Assert.Contains(XFEmojiSkintone.Dark, values);
    }

    [Fact]
    public void XFEmojiSize_HasThreeValues()
    {
        var values = System.Enum.GetValues<XFEmojiSize>();
        Assert.Equal(3, values.Length);
    }

    [Fact]
    public void XFEmojiGroup_HasNineGroups()
    {
        var values = System.Enum.GetValues<XFEmojiGroup>();
        Assert.Equal(9, values.Length);
    }

    [Fact]
    public void Parameters_DocumentationPresent()
    {
        Assert.False(string.IsNullOrEmpty(XFFluentEmojiParameters.ValueDoc));
        Assert.False(string.IsNullOrEmpty(XFFluentEmojiParameters.SkintoneDoc));
    }
}