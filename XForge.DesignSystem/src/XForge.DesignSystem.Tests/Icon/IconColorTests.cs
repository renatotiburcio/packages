using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconColorTests
{
    [Fact]
    public void Accent_Maps_ToAccentCssVar()
    {
        Assert.Contains("--xf-color-accent", XFIconColorMap.ToCssVariable(XFIconColor.Accent));
    }

    [Fact]
    public void Success_Maps_ToSuccessCssVar()
    {
        Assert.Contains("--xf-color-success", XFIconColorMap.ToCssVariable(XFIconColor.Success));
    }

    [Fact]
    public void Error_Maps_ToErrorCssVar()
    {
        Assert.Contains("--xf-color-error", XFIconColorMap.ToCssVariable(XFIconColor.Error));
    }

    [Fact]
    public void Custom_Maps_ToCustomColorVar()
    {
        Assert.Contains("--xf-icon-custom-color", XFIconColorMap.ToCssVariable(XFIconColor.Custom));
    }

    [Fact]
    public void All_SemanticColors_Return_NonEmpty()
    {
        foreach (var c in System.Enum.GetValues<XFIconColor>())
            Assert.False(string.IsNullOrEmpty(XFIconColorMap.ToCssVariable(c)));
    }
}