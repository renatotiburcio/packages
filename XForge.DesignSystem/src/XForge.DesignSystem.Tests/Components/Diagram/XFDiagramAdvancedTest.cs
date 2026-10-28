using Bunit;
using Xunit;
using XForge.Components.Diagram;

namespace XForge.DesignSystem.Tests.Components.Diagram;

public class XFDiagramAdvancedTest : XForgeTestContext
{
    public XFDiagramAdvancedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFDiagramAdvanced>();
        Assert.Contains("xf-diagram-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithNodesAndEdges()
    {
        var nodes = new List<XFDiagramAdvanced.DiagramNode>
        {
            new() { Id = "n1", Label = "A", X = 100, Y = 100 },
            new() { Id = "n2", Label = "B", X = 200, Y = 100 }
        };
        var edges = new List<XFDiagramAdvanced.DiagramEdge>
        {
            new() { FromId = "n1", ToId = "n2" }
        };

        var cut = Render<XFDiagramAdvanced>(p =>
        {
            p.Add(x => x.Nodes, nodes);
            p.Add(x => x.Edges, edges);
        });

        Assert.Contains("data-nodes=\"2\"", cut.Markup);
        Assert.Contains("data-edges=\"1\"", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }
}
