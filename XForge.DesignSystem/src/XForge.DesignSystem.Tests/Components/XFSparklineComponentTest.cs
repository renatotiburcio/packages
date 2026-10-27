using Bunit;
using XForge.DesignSystem.Components.DataDisplay.Sparkline;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFSparklineComponentTest : BunitContext
{
    private sealed record TestItem(string Label, double Value);

    [Fact]
    public void Render_Line_RendersSvgPolyline()
    {
        var items = new[] { new TestItem("A", 10), new TestItem("B", 20), new TestItem("C", 15) };
        var cut = Render<XFSparkline<TestItem>>(p => p
            .Add(x => x.Data, items)
            .Add(x => x.ValueSelector, new Func<TestItem, double>(i => i.Value))
        );

        Assert.Contains("<polyline", cut.Markup);
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Render_Area_RendersSvgPath()
    {
        var items = new[] { new TestItem("A", 10), new TestItem("B", 20), new TestItem("C", 15) };
        var cut = Render<XFSparkline<TestItem>>(p => p
            .Add(x => x.Data, items)
            .Add(x => x.ValueSelector, new Func<TestItem, double>(i => i.Value))
            .Add(x => x.Type, XFSparklineType.Area)
        );

        Assert.Contains("<path", cut.Markup);
        Assert.Contains("linearGradient", cut.Markup);
    }

    [Fact]
    public void Render_Bar_RendersSvgRects()
    {
        var items = new[] { new TestItem("A", 10), new TestItem("B", 20), new TestItem("C", 15) };
        var cut = Render<XFSparkline<TestItem>>(p => p
            .Add(x => x.Data, items)
            .Add(x => x.ValueSelector, new Func<TestItem, double>(i => i.Value))
            .Add(x => x.Type, XFSparklineType.Bar)
        );

        Assert.Contains("<rect", cut.Markup);
    }

    [Fact]
    public void Render_WithColor_AppliesStrokeColor()
    {
        var items = new[] { new TestItem("A", 10), new TestItem("B", 20) };
        var cut = Render<XFSparkline<TestItem>>(p => p
            .Add(x => x.Data, items)
            .Add(x => x.ValueSelector, new Func<TestItem, double>(i => i.Value))
            .Add(x => x.Color, "#ff0000")
        );

        Assert.Contains("#ff0000", cut.Markup);
    }

    [Fact]
    public void Render_WithDots_RendersCircles()
    {
        var items = new[] { new TestItem("A", 10), new TestItem("B", 20), new TestItem("C", 15) };
        var cut = Render<XFSparkline<TestItem>>(p => p
            .Add(x => x.Data, items)
            .Add(x => x.ValueSelector, new Func<TestItem, double>(i => i.Value))
            .Add(x => x.ShowDots, true)
        );

        Assert.Contains("<circle", cut.Markup);
    }

    [Fact]
    public void Render_EmptyData_RendersNothing()
    {
        var cut = Render<XFSparkline<TestItem>>(p => p
            .Add(x => x.Data, Array.Empty<TestItem>())
            .Add(x => x.ValueSelector, new Func<TestItem, double>(i => i.Value))
        );

        Assert.DoesNotContain("<svg", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle_HasAriaLabel()
    {
        var items = new[] { new TestItem("A", 10), new TestItem("B", 20) };
        var cut = Render<XFSparkline<TestItem>>(p => p
            .Add(x => x.Data, items)
            .Add(x => x.ValueSelector, new Func<TestItem, double>(i => i.Value))
            .Add(x => x.AriaLabel, "Vendas diárias")
        );

        Assert.Contains("aria-label=\"Vendas diárias\"", cut.Markup);
        Assert.Contains("role=\"img\"", cut.Markup);
    }
}
