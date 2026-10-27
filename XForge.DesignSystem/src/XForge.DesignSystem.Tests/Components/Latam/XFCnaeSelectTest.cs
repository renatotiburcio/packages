using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFCnaeSelectTest : Bunit.BunitContext
{
    public XFCnaeSelectTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFCnaeSelect>();
        Assert.Contains("xf-cnae-select", cut.Markup);
    }

    [Fact]
    public void Render_ShowsSearchInput()
    {
        var cut = Render<XFCnaeSelect>();
        Assert.Contains("Search CNAE", cut.Markup);
    }
}
