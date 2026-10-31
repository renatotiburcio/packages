using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFSpedViewerTest : XForgeTestContext
{
    public XFSpedViewerTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFSpedViewer>();
        Assert.Contains("xf-sped-viewer", cut.Markup);
    }

    [Fact]
    public void Render_ShowsTypeOptions()
    {
        var cut = Render<XFSpedViewer>();
        Assert.Contains("EFD_ICMS_IPI", cut.Markup);
        Assert.Contains("EFD Contribuicoes", cut.Markup);
        Assert.Contains("ECD (Contabil)", cut.Markup);
    }
}
