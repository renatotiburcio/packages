using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Emoji;

namespace XForge.DesignSystem.Tests.Components;

public class XFSkintonePickerComponentTest : BunitContext
{
    public XFSkintonePickerComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_RendersSkintonePickerContainer()
    {
        var cut = Render<XFSkintonePicker>();
        Assert.Contains("xf-skintone-picker", cut.Markup);
    }

    [Fact]
    public void Render_RendersSixToneSwatches()
    {
        var cut = Render<XFSkintonePicker>();
        var matches = System.Text.RegularExpressions.Regex.Matches(cut.Markup, "xf-skintone-picker__swatch");
        Assert.Equal(6, matches.Count);
    }

    [Fact]
    public void Render_HasRadioGroupRole()
    {
        var cut = Render<XFSkintonePicker>();
        Assert.Contains("role=\"radiogroup\"", cut.Markup);
    }

    [Fact]
    public void Render_WithDefaultSkintone_DefaultToneSelected()
    {
        var cut = Render<XFSkintonePicker>();
        Assert.Contains("data-tone=\"Default\"", cut.Markup);
    }
}
