using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFBoletoGeneratorTest : XForgeTestContext
{
    public XFBoletoGeneratorTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFBoletoGenerator>();
        Assert.Contains("xf-boleto-generator", cut.Markup);
    }

    [Fact]
    public void Render_ShowsGenerateButton()
    {
        var cut = Render<XFBoletoGenerator>();
        Assert.Contains("Generate Boleto", cut.Markup);
    }

    [Fact]
    public void IsValidLinhaDigitavel_CorrectLength_ReturnsTrue()
    {
        Assert.True(XFBoletoGenerator.IsValidLinhaDigitavel("00190500954014481606906809350314337370000000100"));
    }

    [Fact]
    public void IsValidLinhaDigitavel_WrongLength_ReturnsFalse()
    {
        Assert.False(XFBoletoGenerator.IsValidLinhaDigitavel("12345"));
    }
}
