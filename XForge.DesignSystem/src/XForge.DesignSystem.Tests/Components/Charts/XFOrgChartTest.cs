using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Charts;

namespace XForge.DesignSystem.Tests.Components.Charts;

public class XFOrgChartTest : XForgeTestContext
{
    public XFOrgChartTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFOrgChart>();
        Assert.Contains("xf-orgchart", cut.Markup);
    }

    [Fact]
    public void Render_WithRootNode_ShowsNode()
    {
        var root = new XFOrgChart.OrgNode
        {
            Name = "CEO",
            Title = "Chief Executive Officer",
            Children = new List<XFOrgChart.OrgNode>
            {
                new() { Name = "CTO", Title = "Chief Technology Officer" }
            }
        };

        var cut = Render<XFOrgChart>(p => p.Add(x => x.Root, root));
        Assert.Contains("CEO", cut.Markup);
        Assert.Contains("CTO", cut.Markup);
    }

    [Fact]
    public void Render_NullRoot_EmptyChart()
    {
        var cut = Render<XFOrgChart>(p => p.Add(x => x.Root, (XFOrgChart.OrgNode?)null));
        Assert.Contains("xf-orgchart", cut.Markup);
    }
}
