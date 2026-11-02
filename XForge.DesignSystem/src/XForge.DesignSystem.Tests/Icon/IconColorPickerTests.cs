using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconColorPickerTests
{
    [Fact]
    public void ColorMap_HasAllSemanticColors()
    {
        Assert.Equal("var(--xf-color-accent)", XFIconColorMap.ToCssVariable(XFIconColor.Accent));
        Assert.Equal("var(--xf-color-success)", XFIconColorMap.ToCssVariable(XFIconColor.Success));
        Assert.Equal("var(--xf-color-warning)", XFIconColorMap.ToCssVariable(XFIconColor.Warning));
        Assert.Equal("var(--xf-color-error)", XFIconColorMap.ToCssVariable(XFIconColor.Error));
    }
}