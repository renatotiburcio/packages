using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Editors;

namespace XForge.DesignSystem.Tests.Components.Editors;

public class XFDocumentEditorTest : XForgeTestContext
{
    public XFDocumentEditorTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFDocumentEditor>();
        Assert.Contains("xf-documenteditor", cut.Markup);
    }

    [Fact]
    public void Render_WithPages_ShowsPages()
    {
        var cut = Render<XFDocumentEditor>(p => p.Add(x => x.Pages, 3));
        Assert.Contains("data-pages=\"3\"", cut.Markup);
    }

    [Fact]
    public void Render_WithToolbar_ShowsCommands()
    {
        var cut = Render<XFDocumentEditor>();
        Assert.Contains("xf-de-toolbar", cut.Markup);
        Assert.Contains("Bold", cut.Markup);
    }

    [Fact]
    public void Render_WithOrientation()
    {
        var cut = Render<XFDocumentEditor>(p => p.Add(x => x.Orientation, "landscape"));
        Assert.Contains("landscape", cut.Markup);
    }
}
