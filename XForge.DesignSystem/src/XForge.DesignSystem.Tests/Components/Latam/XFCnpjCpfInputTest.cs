using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFCnpjCpfInputTest : XForgeTestContext
{
    public XFCnpjCpfInputTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFCnpjCpfInput>();
        Assert.Contains("xf-cnpjcpf-input", cut.Markup);
    }

    [Fact]
    public void Render_ShowsTypeSelector()
    {
        var cut = Render<XFCnpjCpfInput>();
        Assert.Contains("CPF", cut.Markup);
        Assert.Contains("CNPJ", cut.Markup);
    }
}
