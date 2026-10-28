using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Icon;

namespace XForge.DesignSystem.Tests.Components;

public class XFIconColorPickerComponentTest : XForgeTestContext
{
    public XFIconColorPickerComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_Default_RendersContainer()
    {
        var cut = Render<XFIconColorPicker>();
        Assert.Contains("xf-icon-color-picker", cut.Markup);
    }

    [Fact]
    public void Render_HasRadiogroupRole()
    {
        var cut = Render<XFIconColorPicker>();
        Assert.Contains("role=\"radiogroup\"", cut.Markup);
    }

    [Fact]
    public void Render_RendersColorSwatches()
    {
        var cut = Render<XFIconColorPicker>();
        Assert.Contains("xf-icon-color-picker__swatch", cut.Markup);
        Assert.Contains("data-color", cut.Markup);
    }

    [Fact]
    public void Render_RendersCustomColorInput()
    {
        var cut = Render<XFIconColorPicker>();
        Assert.Contains("xf-icon-color-picker__custom", cut.Markup);
        Assert.Contains("type=\"color\"", cut.Markup);
    }

    [Fact]
    public void Render_DefaultColorIsAccent()
    {
        var cut = Render<XFIconColorPicker>(parameters =>
            parameters.Add(p => p.Color, XFIconColor.Accent));
        Assert.Contains("data-color=\"Accent\"", cut.Markup);
    }
}
