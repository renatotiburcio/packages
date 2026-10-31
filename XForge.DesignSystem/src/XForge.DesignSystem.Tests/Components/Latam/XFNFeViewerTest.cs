using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Latam;

namespace XForge.DesignSystem.Tests.Components.Latam;

public class XFNFeViewerTest : XForgeTestContext
{
    public XFNFeViewerTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFNFeViewer>();
        Assert.Contains("xf-nfe-viewer", cut.Markup);
    }

    [Fact]
    public void Render_WithAccessKey_ShowsKey()
    {
        var cut = Render<XFNFeViewer>(p => p.Add(x => x.AccessKey, "12345678901234567890123456789012345678901234"));
        Assert.Contains("12345678901234567890123456789012345678901234", cut.Markup);
    }

    [Fact]
    public void Render_WithIssuer_ShowsIssuerName()
    {
        var cut = Render<XFNFeViewer>(p =>
        {
            p.Add(x => x.IssuerName, "Empresa ABC");
            p.Add(x => x.IssuerCnpj, "12345678000195");
        });

        Assert.Contains("Empresa ABC", cut.Markup);
        Assert.Contains("12345678000195", cut.Markup);
    }
}
