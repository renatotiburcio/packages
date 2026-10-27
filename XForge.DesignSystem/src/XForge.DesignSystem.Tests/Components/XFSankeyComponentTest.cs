using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Sankey;
using Xunit;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFSankeyComponentTest : BunitContext
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

    private IRenderedComponent<XFSankey<FlowItem>> RenderWithLabels()
        => Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.SourceLabelSelector, new Func<FlowItem, string>(f => f.SourceLabel ?? f.Source))
            .Add(x => x.TargetLabelSelector, new Func<FlowItem, string>(f => f.TargetLabel ?? f.Target))
        );

    [Fact]
    public void Render_WithData_RendersSankeyContainer()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-sankey", cut.Markup);
        Assert.Contains("role=\"img\"", cut.Markup);
    }

    [Fact]
    public void Render_WithData_RendersSvgElement()
    {
        var cut = RenderDefault();
        Assert.Contains("<svg", cut.Markup);
        Assert.Contains("viewBox=", cut.Markup);
        Assert.Contains("preserveAspectRatio=\"xMidYMid meet\"", cut.Markup);
    }

    [Fact]
    public void Render_WithData_RendersNodeRects()
    {
        var cut = RenderDefault();
        // 4 unique nodes: A, B, C, D
        Assert.Contains("<rect", cut.Markup);
        Assert.Contains("xf-sankey-node", cut.Markup);
    }

    [Fact]
    public void Render_WithData_RendersLinkPaths()
    {
        var cut = RenderDefault();
        // 4 links
        Assert.Contains("xf-sankey-link", cut.Markup);
        Assert.Contains(" d=\"M", cut.Markup);
    }

    [Fact]
    public void Render_WithLabels_RendersLabels()
    {
        var cut = RenderWithLabels();
        Assert.Contains("Node A", cut.Markup);
        Assert.Contains("Node B", cut.Markup);
        Assert.Contains("Node C", cut.Markup);
        Assert.Contains("Node D", cut.Markup);
    }

    [Fact]
    public void Render_WithoutLabels_RendersNodeIdsAsLabels()
    {
        var cut = RenderDefault();
        // When no label selectors, node IDs are used as labels
        Assert.Contains(">A<", cut.Markup);
        Assert.Contains(">B<", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomId_RendersId()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Id, "my-sankey")
        );
        Assert.Contains("id=\"my-sankey\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomClass_IncludesClass()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Class, "my-custom-class")
        );
        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomStyle_RendersStyle()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Style, "border:1px solid red")
        );
        Assert.Contains("border:1px solid red", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_RendersAttributes()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "sankey" } })
        );
        Assert.Contains("data-testid=\"sankey\"", cut.Markup);
    }

    [Fact]
    public void Render_WithEmptyData_ShowsEmptyText()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, new List<FlowItem>())
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.EmptyText, "Nenhum fluxo")
        );
        Assert.Contains("Nenhum fluxo", cut.Markup);
    }

    [Fact]
    public void Render_WithNullData_ShowsEmptyText()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, null)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.EmptyText, "Sem dados")
        );
        Assert.Contains("Sem dados", cut.Markup);
    }

    [Fact]
    public void Render_WithTheme_RendersDataTheme()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Theme, "dark")
        );
        Assert.Contains("data-theme=\"dark\"", cut.Markup);
    }

    [Fact]
    public void Render_WithAnimated_RendersFlowGradients()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Animated, true)
        );
        Assert.Contains("xf-sankey-flow-gradient", cut.Markup);
        Assert.Contains("linearGradient", cut.Markup);
    }

    [Fact]
    public void Render_WithSmallSize_RendersSmClass()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Size, XFSize.Sm)
        );
        Assert.Contains("h-48", cut.Markup);
    }

    [Fact]
    public void Render_WithLargeSize_RendersLgClass()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Size, XFSize.Lg)
        );
        Assert.Contains("h-96", cut.Markup);
    }

    [Fact]
    public void Render_HasAriaLabel()
    {
        var cut = RenderDefault();
        Assert.Contains("aria-label=\"Diagrama Sankey\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomAriaLabel_RendersCustomLabel()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.AriaLabel, "Fluxo de vendas")
        );
        Assert.Contains("aria-label=\"Fluxo de vendas\"", cut.Markup);
    }

    [Fact]
    public void Render_WithNodeFills_RendersCustomColors()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.NodeFills, new[] { "#ff0000", "#00ff00", "#0000ff", "#ffff00" })
        );
        Assert.Contains("#ff0000", cut.Markup);
    }

    [Fact]
    public void Render_WithAlignmentLeft_RendersCorrectLayout()
    {
        var cut = Render<XFSankey<FlowItem>>(p => p
            .Add(x => x.Data, SampleFlows)
            .Add(x => x.SourceSelector, new Func<FlowItem, string>(f => f.Source))
            .Add(x => x.TargetSelector, new Func<FlowItem, string>(f => f.Target))
            .Add(x => x.ValueSelector, new Func<FlowItem, double>(f => f.Value))
            .Add(x => x.Alignment, XFSankeyAlignment.Left)
        );
        // Should still render without errors
        Assert.Contains("xf-sankey", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void Render_DefaultParameterValues_AreCorrect()
    {
        var cut = RenderDefault();
        // Default size is Md (h-64)
        Assert.Contains("h-64", cut.Markup);
        // Default alignment is Justify (no special class needed)
        Assert.Contains("xf-sankey", cut.Markup);
    }

    [Fact]
    public void Enum_XFSankeyAlignment_HasExpectedValues()
    {
        Assert.Equal(4, Enum.GetValues<XFSankeyAlignment>().Length);
        Assert.Contains(XFSankeyAlignment.Justify, Enum.GetValues<XFSankeyAlignment>());
        Assert.Contains(XFSankeyAlignment.Left, Enum.GetValues<XFSankeyAlignment>());
        Assert.Contains(XFSankeyAlignment.Right, Enum.GetValues<XFSankeyAlignment>());
        Assert.Contains(XFSankeyAlignment.Center, Enum.GetValues<XFSankeyAlignment>());
    }

    [Fact]
    public void Enum_XFSankeySize_HasExpectedValues()
    {
        Assert.Equal(3, Enum.GetValues<XFSize>().Length);
        Assert.Contains(XFSize.Sm, Enum.GetValues<XFSize>());
        Assert.Contains(XFSize.Md, Enum.GetValues<XFSize>());
        Assert.Contains(XFSize.Lg, Enum.GetValues<XFSize>());
    }
}
