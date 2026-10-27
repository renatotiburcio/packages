using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFCfopSelectTest : Bunit.BunitContext
{
    public XFCfopSelectTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFCfopSelect>();
        Assert.Contains("xf-cfop-select", cut.Markup);
    }

    [Fact]
    public void Render_ShowsCfopOptions()
    {
        var cut = Render<XFCfopSelect>();
        Assert.Contains("1101", cut.Markup);
        Assert.Contains("5101", cut.Markup);
    }
}
