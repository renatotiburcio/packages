using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Pdf;

namespace XForge.DesignSystem.Tests.Components.Pdf;

public class XFPdfViewerAdvancedTest : XForgeTestContext
{
    public XFPdfViewerAdvancedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFPdfViewerAdvanced>();
        Assert.Contains("xf-pdf-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithPages()
    {
        var cut = Render<XFPdfViewerAdvanced>(p => p.Add(x => x.Pages, 10));
        Assert.Contains("data-pages=\"10\"", cut.Markup);
    }

    [Fact]
    public void Render_ShowsToolbar()
    {
        var cut = Render<XFPdfViewerAdvanced>();
        Assert.Contains("xf-pdf-toolbar", cut.Markup);
        Assert.Contains("Annotate", cut.Markup);
        Assert.Contains("Sign", cut.Markup);
        Assert.Contains("Form Fill", cut.Markup);
    }
}
