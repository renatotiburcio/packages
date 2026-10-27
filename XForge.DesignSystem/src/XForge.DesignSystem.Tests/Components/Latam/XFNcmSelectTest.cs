using Bunit;
using Xunit;
using XForge.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFNcmSelectTest : Bunit.BunitContext
{
    public XFNcmSelectTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFNcmSelect>();
        Assert.Contains("xf-ncm-select", cut.Markup);
    }

    [Fact]
    public void Render_ShowsNcmOptions()
    {
        var cut = Render<XFNcmSelect>();
        Assert.Contains("8471.30.19", cut.Markup);
        Assert.Contains("Notebooks", cut.Markup);
    }
}
