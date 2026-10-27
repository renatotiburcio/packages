using XForge.DesignSystem.Components.Icon;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class IconColorPickerTests
{
    [Fact]
    public void ColorMap_HasAllSemanticColors()
    {
        Assert.Contains("Accent", XFIconColorMap.ToCssVariable(XFIconColor.Accent));
        Assert.Contains("Success", XFIconColorMap.ToCssVariable(XFIconColor.Success));
        Assert.Contains("Warning", XFIconColorMap.ToCssVariable(XFIconColor.Warning));
        Assert.Contains("Error", XFIconColorMap.ToCssVariable(XFIconColor.Error));
    }
}