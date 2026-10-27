using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFCpfInputTest : Bunit.BunitContext
{
    public XFCpfInputTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFCpfInput>();
        Assert.Contains("xf-cpf-input", cut.Markup);
    }

    [Fact]
    public void Render_ShowsPlaceholder()
    {
        var cut = Render<XFCpfInput>();
        Assert.Contains("000.000.000-00", cut.Markup);
    }

    [Fact]
    public void IsValid_ValidCpf_ReturnsTrue()
    {
        Assert.True(XFCpfInput.IsValid("52998224725"));
    }

    [Fact]
    public void IsValid_InvalidCpf_ReturnsFalse()
    {
        Assert.False(XFCpfInput.IsValid("12345678901"));
    }

    [Fact]
    public void IsValid_WrongLength_ReturnsFalse()
    {
        Assert.False(XFCpfInput.IsValid("1234567890"));
    }
}
