using Bunit;
using Xunit;
using XForge.Components.DataGrid;

namespace XForge.DesignSystem.Tests.Components.DataGrid;

public class XFTreeGridTest : Bunit.BunitContext
{
    public XFTreeGridTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFTreeGrid>();
        Assert.Contains("xf-treegrid", cut.Markup);
    }

    [Fact]
    public void Render_WithNodes_ShowsNodes()
    {
        var nodes = new List<XFTreeGrid.TreeNode>
        {
            new() { Id = "1", Label = "Root", Children = new List<XFTreeGrid.TreeNode>
            {
                new() { Id = "1.1", Label = "Child 1" },
                new() { Id = "1.2", Label = "Child 2" }
            }}
        };

        var cut = Render<XFTreeGrid>(p => p.Add(x => x.Nodes, nodes));
        Assert.Contains("Root", cut.Markup);
        Assert.Contains("Child 1", cut.Markup);
        Assert.Contains("Child 2", cut.Markup);
    }
}
