using Bunit;
using Xunit;
using XForge.Components.Editors;

namespace XForge.DesignSystem.Tests.Components.Editors;

public class XFImageEditorTest : XForgeTestContext
{
    public XFImageEditorTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFImageEditor>();
        Assert.Contains("xf-imageeditor", cut.Markup);
    }

    [Fact]
    public void Render_WithSource_ShowsImage()
    {
        var cut = Render<XFImageEditor>(p => p.Add(x => x.Source, "test.png"));
        Assert.Contains("test.png", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomSize()
    {
        var cut = Render<XFImageEditor>(p =>
        {
            p.Add(x => x.Width, 800);
            p.Add(x => x.Height, 600);
        });

        Assert.Contains("800px", cut.Markup);
        Assert.Contains("600px", cut.Markup);
    }

    [Fact]
    public void Render_ShowsToolbarButtons()
    {
        var cut = Render<XFImageEditor>();
        Assert.Contains("Rotate Left", cut.Markup);
        Assert.Contains("Rotate Right", cut.Markup);
        Assert.Contains("Crop", cut.Markup);
        Assert.Contains("Save", cut.Markup);
    }
}
