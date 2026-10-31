using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFBrazilianStatesSelectTest : XForgeTestContext
{
    public XFBrazilianStatesSelectTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFBrazilianStatesSelect>();
        Assert.Contains("xf-br-states", cut.Markup);
    }

    [Fact]
    public void Render_ShowsAllStates()
    {
        var cut = Render<XFBrazilianStatesSelect>();
        Assert.Contains("SP", cut.Markup);
        Assert.Contains("RJ", cut.Markup);
        Assert.Contains("MG", cut.Markup);
        Assert.Contains("Sao Paulo", cut.Markup);
    }
}
