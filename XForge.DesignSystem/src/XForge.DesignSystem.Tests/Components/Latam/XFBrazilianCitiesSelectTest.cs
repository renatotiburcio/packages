using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFBrazilianCitiesSelectTest : Bunit.BunitContext
{
    public XFBrazilianCitiesSelectTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFBrazilianCitiesSelect>();
        Assert.Contains("xf-br-cities", cut.Markup);
    }

    [Fact]
    public void Render_WithUf_ShowsCities()
    {
        var cut = Render<XFBrazilianCitiesSelect>(p => p.Add(x => x.Uf, "SP"));
        Assert.Contains("Sao Paulo", cut.Markup);
        Assert.Contains("Campinas", cut.Markup);
    }
}
