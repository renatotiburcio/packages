using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFVehiclePlateInputTest : XForgeTestContext
{
    public XFVehiclePlateInputTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFVehiclePlateInput>();
        Assert.Contains("xf-plate", cut.Markup);
    }

    [Fact]
    public void Render_ShowsPlaceholder()
    {
        var cut = Render<XFVehiclePlateInput>();
        Assert.Contains("ABC1D23", cut.Markup);
    }

    [Fact]
    public void IsValid_ValidPlate_ReturnsTrue()
    {
        Assert.True(XFVehiclePlateInput.IsValid("ABC1D23"));
    }

    [Fact]
    public void IsValid_InvalidPlate_ReturnsFalse()
    {
        Assert.False(XFVehiclePlateInput.IsValid("1234567"));
    }

    [Fact]
    public void IsValid_WithHyphen_ReturnsTrue()
    {
        Assert.True(XFVehiclePlateInput.IsValid("ABC-1D23"));
    }
}
