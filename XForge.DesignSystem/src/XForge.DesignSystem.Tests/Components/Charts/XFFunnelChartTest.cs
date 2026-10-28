using Bunit;
using Xunit;
using XForge.Components.Charts;

namespace XForge.DesignSystem.Tests.Components.Charts;

public class XFFunnelChartTest : XForgeTestContext
{
    public XFFunnelChartTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFFunnelChart>();
        Assert.Contains("xf-funnel", cut.Markup);
    }

    [Fact]
    public void Render_WithStages_ShowsStages()
    {
        var stages = new List<XFFunnelChart.FunnelStage>
        {
            new("Visitors", 1.0),
            new("Leads", 0.6),
            new("Customers", 0.2)
        };

        var cut = Render<XFFunnelChart>(p => p.Add(x => x.Stages, stages));
        Assert.Contains("data-stages=\"3\"", cut.Markup);
        Assert.Contains("Visitors", cut.Markup);
        Assert.Contains("Leads", cut.Markup);
        Assert.Contains("Customers", cut.Markup);
    }

    [Fact]
    public void Render_EmptyStages()
    {
        var cut = Render<XFFunnelChart>(p => p.Add(x => x.Stages, new List<XFFunnelChart.FunnelStage>()));
        Assert.Contains("data-stages=\"0\"", cut.Markup);
    }
}
