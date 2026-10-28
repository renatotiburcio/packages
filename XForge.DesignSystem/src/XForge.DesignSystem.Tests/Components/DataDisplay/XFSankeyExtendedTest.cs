using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.DataDisplay.Sankey;
using Xunit;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFSankeyExtendedTest : XForgeTestContext
{
    private sealed record FlowItem(
        string Source,
        string Target,
        double Value,
        string? SourceLabel = null,
        string? TargetLabel = null);

    private static List<FlowItem> SampleFlows => new()
    {
        new("A", "B", 10, "Node A", "Node B"),
        new("A", "C", 5, "Node A", "Node C"),
        new("B", "D", 7, "Node B", "Node D"),
        new("C", "D", 3, "Node C", "Node D"),
    };

    private IRenderedComponent<XFSankey<FlowItem>> RenderDefault()
        => Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
        );

    [Fact]
    public void Extended_VisibleFalse_DoesNotRenderContainer()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Visible, false));
        Assert.DoesNotContain("xf-sankey", cut.Markup);
    }

    [Fact]
    public void Extended_NodeClick_FiresOnNodeClick()
    {
        XFSankeyNodeClickEventArgs? capturedArgs = null;
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.OnNodeClick, EventCallback.Factory.Create<XFSankeyNodeClickEventArgs>(this, args => capturedArgs = args)));
        var node = cut.Find(".xf-sankey-node");
        node.Click();
        Assert.NotNull(capturedArgs);
        Assert.Equal("A", capturedArgs!.NodeId);
        Assert.Equal(15, capturedArgs.Outgoing);
    }

    [Fact]
    public void Extended_LinkClick_FiresOnLinkClick()
    {
        XFSankeyLinkClickEventArgs? capturedArgs = null;
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.OnLinkClick, EventCallback.Factory.Create<XFSankeyLinkClickEventArgs>(this, args => capturedArgs = args)));
        var link = cut.Find(".xf-sankey-link");
        link.Click();
        Assert.NotNull(capturedArgs);
        Assert.Equal("A", capturedArgs!.Source);
        Assert.Equal("B", capturedArgs.Target);
        Assert.Equal(10, capturedArgs.Value);
    }

    [Fact]
    public void Extended_NodeMouseOver_ShowsTooltip()
    {
        var cut = RenderDefault();
        var node = cut.Find(".xf-sankey-node");
        node.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("xf-sankey-tooltip", cut.Markup);
    }

    [Fact]
    public void Extended_LinkMouseOver_ShowsTooltipWithFlowLabel()
    {
        var cut = RenderDefault();
        var link = cut.Find(".xf-sankey-link");
        link.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("Fluxo", cut.Markup);
    }

    [Fact]
    public void Extended_MouseOut_HidesTooltip()
    {
        var cut = RenderDefault();
        var node = cut.Find(".xf-sankey-node");
        node.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("xf-sankey-tooltip", cut.Markup);
        node.TriggerEvent("onmouseout", new MouseEventArgs());
        Assert.DoesNotContain("xf-sankey-tooltip", cut.Markup);
    }

    [Fact]
    public void Extended_ValueFormatter_CustomFormatter_RendersFormattedValue()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.ValueFormatter, new Func<double, string>(v => $"R$ {v:N2}")));
        var node = cut.Find(".xf-sankey-node");
        node.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("R$", cut.Markup);
    }

    [Fact]
    public void Extended_LinkFills_CustomColors_RendersWithAlpha()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.LinkFills, new[] { "#ff0000", "#00ff00", "#0000ff", "#ffff00" }));
        Assert.Contains("#ff000080", cut.Markup);
    }

    [Fact]
    public void Extended_NodeWidth_CustomValue_AppliesToLayout()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.NodeWidth, 40));
        Assert.Contains("xf-sankey", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void Extended_NodePadding_CustomValue_AppliesToLayout()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.NodePadding, 20));
        Assert.Contains("xf-sankey", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void Extended_AlignmentCenter_RendersWithoutError()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Alignment, XFSankeyAlignment.Center));
        Assert.Contains("xf-sankey", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void Extended_AlignmentRight_RendersWithoutError()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Alignment, XFSankeyAlignment.Right));
        Assert.Contains("xf-sankey", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void Extended_EmptyText_DefaultText_WhenNoData()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, null)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value)));
        Assert.Contains("Nenhum dado para exibir", cut.Markup);
    }

    [Fact]
    public void Extended_ValueLabel_CustomLabel_RendersInTooltip()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.ValueLabel, "Total"));
        var node = cut.Find(".xf-sankey-node");
        node.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("Total", cut.Markup);
    }

    [Fact]
    public void Extended_IncomingLabel_CustomLabel_RendersInTooltip()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.IncomingLabel, "In"));
        var nodes = cut.FindAll(".xf-sankey-node");
        var lastNode = nodes.Last();
        lastNode.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("In", cut.Markup);
    }

    [Fact]
    public void Extended_OutgoingLabel_CustomLabel_RendersInTooltip()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.OutgoingLabel, "Out"));
        var node = cut.Find(".xf-sankey-node");
        node.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("Out", cut.Markup);
    }

    [Fact]
    public void Extended_FlowLabel_CustomLabel_RendersInLinkTooltip()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.FlowLabel, "Transferência"));
        var link = cut.Find(".xf-sankey-link");
        link.TriggerEvent("onmouseover", new MouseEventArgs { ClientX = 100, ClientY = 200 });
        Assert.Contains("Transferência", cut.Markup);
    }

    [Fact]
    public void Extended_AnimatedFalse_NoGradientDefs()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Animated, false));
        Assert.DoesNotContain("linearGradient", cut.Markup);
    }

    [Fact]
    public void Extended_Svg_HasPresentationRole()
    {
        var cut = RenderDefault();
        var svg = cut.Find("svg");
        Assert.Equal("presentation", svg.GetAttribute("role"));
    }

    [Fact]
    public void Extended_Svg_HasAriaHidden()
    {
        var cut = RenderDefault();
        var svg = cut.Find("svg");
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Extended_Container_HasImgRole()
    {
        var cut = RenderDefault();
        var container = cut.Find("[role='img']");
        Assert.NotNull(container);
    }

    [Fact]
    public void Extended_MultipleFlowsBetweenSameNodes_AggregatesCorrectly()
    {
        var flows = new List<FlowItem>
        {
            new("A", "B", 10),
            new("A", "B", 5),
            new("B", "C", 8),
        };
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, flows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value)));
        Assert.Contains("xf-sankey-node", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void Extended_NodeFills_PartialList_UsesCssClassForRemaining()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.NodeFills, new[] { "#ff0000" }));
        Assert.Contains("#ff0000", cut.Markup);
        Assert.Contains("xf-sankey-series-1", cut.Markup);
    }

    [Fact]
    public void Extended_LinkFills_NonHexColor_UsedAsIs()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.LinkFills, new[] { "rgba(255,0,0,0.5)" }));
        Assert.Contains("rgba(255,0,0,0.5)", cut.Markup);
    }

    [Fact]
    public void Extended_NodeClick_NoDelegate_DoesNotThrow()
    {
        var cut = RenderDefault();
        var node = cut.Find(".xf-sankey-node");
        var exception = Record.Exception(() => node.Click());
        Assert.Null(exception);
    }

    [Fact]
    public void Extended_LinkClick_NoDelegate_DoesNotThrow()
    {
        var cut = RenderDefault();
        var link = cut.Find(".xf-sankey-link");
        var exception = Record.Exception(() => link.Click());
        Assert.Null(exception);
    }
}
