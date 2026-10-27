using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Icon;

namespace XForge.DesignSystem.Tests.Components;

public class XFIconPickerComponentTest : BunitContext
{
    public XFIconPickerComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_Default_RendersContainer()
    {
        var cut = Render<XFIconPicker>();
        Assert.Contains("xf-icon-picker", cut.Markup);
    }

    [Fact]
    public void Render_HasGridRole()
    {
        var cut = Render<XFIconPicker>();
        Assert.Contains("role=\"grid\"", cut.Markup);
    }

    [Fact]
    public void Render_SearchInput_Placeholder()
    {
        var cut = Render<XFIconPicker>();
        Assert.Contains("Search icons...", cut.Markup);
    }

    [Fact]
    public void Render_VariantDropdown_Options()
    {
        var cut = Render<XFIconPicker>();
        Assert.Contains("Regular", cut.Markup);
        Assert.Contains("Filled", cut.Markup);
        Assert.Contains("Light", cut.Markup);
        Assert.Contains("Color", cut.Markup);
    }

    [Fact]
    public void Render_SizeDropdown_Options()
    {
        var cut = Render<XFIconPicker>();
        Assert.Contains("Size16", cut.Markup);
        Assert.Contains("Size24", cut.Markup);
        Assert.Contains("Size48", cut.Markup);
    }
}
