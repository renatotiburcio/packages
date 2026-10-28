using Bunit;
using Xunit;
using XForge.Components.Editors;

namespace XForge.DesignSystem.Tests.Components.Editors;

public class XFBlockEditorTest : XForgeTestContext
{
    public XFBlockEditorTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFBlockEditor>();
        Assert.Contains("xf-blockeditor", cut.Markup);
    }

    [Fact]
    public void Render_WithBlocks_ShowsBlocks()
    {
        var blocks = new List<XFBlockEditor.Block>
        {
            new("b1", "h1", "<h1>Title</h1>"),
            new("b2", "p", "<p>Content</p>")
        };

        var cut = Render<XFBlockEditor>(p => p.Add(x => x.Blocks, blocks));
        Assert.Contains("data-blocks=\"2\"", cut.Markup);
        Assert.Contains("Title", cut.Markup);
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void Render_WithSlashMenu_ShowsMenu()
    {
        var cut = Render<XFBlockEditor>(p => p.Add(x => x.ShowSlashMenu, true));
        Assert.Contains("xf-slash-menu", cut.Markup);
    }
}
