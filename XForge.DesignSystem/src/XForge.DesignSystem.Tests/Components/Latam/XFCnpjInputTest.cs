using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFCnpjInputTest : XForgeTestContext
{
    public XFCnpjInputTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFCnpjInput>();
        Assert.Contains("xf-cnpj-input", cut.Markup);
    }

    [Fact]
    public void Render_ShowsPlaceholder()
    {
        var cut = Render<XFCnpjInput>();
        Assert.Contains("00.000.000/0000-00", cut.Markup);
    }

    [Fact]
    public void IsValid_ValidCnpj_ReturnsTrue()
    {
        Assert.True(XFCnpjInput.IsValid("11222333000181"));
    }

    [Fact]
    public void IsValid_InvalidCnpj_ReturnsFalse()
    {
        Assert.False(XFCnpjInput.IsValid("12345678000100"));
    }
}
