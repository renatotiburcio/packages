using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFPixKeyInputTest : XForgeTestContext
{
    public XFPixKeyInputTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFPixKeyInput>();
        Assert.Contains("xf-pix-input", cut.Markup);
    }

    [Fact]
    public void Render_ShowsPixTypeOptions()
    {
        var cut = Render<XFPixKeyInput>();
        Assert.Contains("CPF", cut.Markup);
        Assert.Contains("CNPJ", cut.Markup);
        Assert.Contains("Email", cut.Markup);
    }
}
