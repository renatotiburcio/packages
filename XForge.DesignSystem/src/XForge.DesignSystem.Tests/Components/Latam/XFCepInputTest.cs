using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFCepInputTest : XForgeTestContext
{
    public XFCepInputTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFCepInput>();
        Assert.Contains("xf-cep-input", cut.Markup);
    }

    [Fact]
    public void Render_ShowsLookupButton()
    {
        var cut = Render<XFCepInput>();
        Assert.Contains("Buscar", cut.Markup);
    }

    [Fact]
    public void Render_ShowsPlaceholder()
    {
        var cut = Render<XFCepInput>();
        Assert.Contains("00000-000", cut.Markup);
    }
}
