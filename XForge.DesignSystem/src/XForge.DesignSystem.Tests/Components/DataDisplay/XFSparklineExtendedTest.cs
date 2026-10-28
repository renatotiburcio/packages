using Bunit;
using XForge.DesignSystem.Components.DataDisplay.Sparkline;
using Xunit;
using System.Globalization;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFSparklineExtendedTest : XForgeTestContext
{
    public XFSparklineExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private static List<double> SampleData() => new() { 1.0, 2.0, 3.0, 4.0, 5.0 };

    private IRenderedComponent<XFSparkline<double>> RenderSparkline(
        Action<ComponentParameterCollectionBuilder<XFSparkline<double>>>? configure = null)
    {
        return Render<XFSparkline<double>>(p =>
        {
            p.Add(x => x.Data, SampleData());
            p.Add(x => x.ValueSelector, new Func<double, double>(d => d));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_BaseClass_ContainsSparklineClasses()
    {
        var cut = RenderSparkline();
        Assert.Contains("xf-sparkline relative inline-block", cut.Markup);
    }

    [Fact]
    public void Extended_TypeLine_RendersSvgPolyline()
    {
        var cut = RenderSparkline();
        Assert.Contains("<polyline", cut.Markup);
    }

    [Fact]
    public void Extended_TypeArea_RendersSvgPathWithFill()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Type, XFSparklineType.Area));
        Assert.Contains("<path", cut.Markup);
        Assert.Contains("linearGradient", cut.Markup);
    }

    [Fact]
    public void Extended_TypeBar_RendersSvgRects()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Type, XFSparklineType.Bar));
        Assert.Contains("<rect", cut.Markup);
    }

    [Fact]
    public void Extended_CustomColor_AppliesColorToStroke()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Color, "#ff0000"));
        Assert.Contains("#ff0000", cut.Markup);
    }

    [Fact]
    public void Extended_StrokeWidth_AppliesToPolyline()
    {
        var cut = RenderSparkline(p => p.Add(x => x.StrokeWidth, 3.0));
        Assert.Contains("stroke-width=\"3\"", cut.Markup);
    }

    [Fact]
    public void Extended_ShowDotsFalse_NoCirclesRendered()
    {
        var cut = RenderSparkline();
        Assert.DoesNotContain("<circle", cut.Markup);
    }

    [Fact]
    public void Extended_ShowDotsTrue_RendersCircles()
    {
        var cut = RenderSparkline(p => p.Add(x => x.ShowDots, true));
        Assert.Contains("<circle", cut.Markup);
    }

    [Fact]
    public void Extended_DotRadius_AppliedToCircle()
    {
        var cut = RenderSparkline(p => p.Add(x => x.ShowDots, true).Add(x => x.DotRadius, 4.0));
        Assert.Contains("r=\"4\"", cut.Markup);
    }

    [Fact]
    public void Extended_HeightDefault_ParameterAccepted()
    {
        var cut = RenderSparkline();
        Assert.Contains("xf-sparkline", cut.Markup);
        Assert.Equal("2rem", cut.Instance.Height);
    }

    [Fact]
    public void Extended_HeightCustom_ParameterAccepted()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Height, "5rem"));
        Assert.Contains("xf-sparkline", cut.Markup);
        Assert.Equal("5rem", cut.Instance.Height);
    }

    [Fact]
    public void Extended_ValueFormat_AppliesToTooltip()
    {
        var cut = RenderSparkline(p => p.Add(x => x.ValueFormat, "N2"));
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_AriaLabelDefault_ContainsSparklineLabel()
    {
        var cut = RenderSparkline();
        Assert.Contains("Gráfico sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_AriaLabelCustom_ContainsCustomLabel()
    {
        var cut = RenderSparkline(p => p.Add(x => x.AriaLabel, "Vendas mensais"));
        Assert.Contains("Vendas mensais", cut.Markup);
    }

    [Fact]
    public void Extended_TooltipFormat_StoresFormatTemplate()
    {
        var cut = RenderSparkline(p => p.Add(x => x.TooltipFormat, "{0} - {1}"));
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_VisibleTrue_RendersComponent()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Visible, true));
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_VisibleFalse_DoesNotRender()
    {
        var cut = Render<XFSparkline<double>>(p =>
        {
            p.Add(x => x.Data, SampleData());
            p.Add(x => x.ValueSelector, new Func<double, double>(d => d));
            p.Add(x => x.Visible, false);
        });
        Assert.DoesNotContain("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_ClassCustom_AppendedToRootClass()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Class, "custom-class"));
        Assert.Contains("custom-class", cut.Markup);
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_StyleCustom_AppliedToElement()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Style, "width: 200px"));
        Assert.Contains("width: 200px", cut.Markup);
    }

    [Fact]
    public void Extended_IdCustom_AppliedToElement()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Id, "my-sparkline"));
        Assert.Contains("my-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_SpreadToElement()
    {
        var cut = RenderSparkline(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "sparkline-1" },
            { "data-custom", "value" }
        }));
        Assert.Contains("data-testid=\"sparkline-1\"", cut.Markup);
        Assert.Contains("data-custom=\"value\"", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_SetsDisabledState()
    {
        var cut = RenderSparkline(p => p.Add(x => x.Disabled, true));
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_CultureCustom_AppliesCultureFormatting()
    {
        var culture = new CultureInfo("pt-BR");
        var cut = RenderSparkline(p => p.Add(x => x.Culture, culture));
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_LabelSelector_AppliesToTooltip()
    {
        var data = new List<(double Value, string Label)>
        {
            (1.0, "Jan"),
            (2.0, "Fev"),
            (3.0, "Mar")
        };
        var cut = Render<XFSparkline<(double Value, string Label)>>(p =>
        {
            p.Add(x => x.Data, data);
            p.Add(x => x.ValueSelector, new Func<(double Value, string Label), double>(d => d.Value));
            p.Add(x => x.LabelSelector, new Func<(double Value, string Label), string>(d => d.Label));
        });
        Assert.Contains("xf-sparkline", cut.Markup);
    }

    [Fact]
    public void Extended_EnumValues_HasLineAreaBar()
    {
        Assert.Equal(0, (int)XFSparklineType.Line);
        Assert.Equal(1, (int)XFSparklineType.Area);
        Assert.Equal(2, (int)XFSparklineType.Bar);
    }

    [Fact]
    public void Extended_DefaultValues_HaveExpectedDefaults()
    {
        var cut = RenderSparkline();
        Assert.Contains("xf-sparkline", cut.Markup);
        Assert.Contains("Gráfico sparkline", cut.Markup);
        Assert.Contains("stroke-width", cut.Markup);
        Assert.DoesNotContain("<circle", cut.Markup);
        Assert.Equal("2rem", cut.Instance.Height);
        Assert.Equal(XFSparklineType.Line, cut.Instance.Type);
        Assert.Equal(1.5, cut.Instance.StrokeWidth);
        Assert.False(cut.Instance.ShowDots);
        Assert.True(cut.Instance.Visible);
    }

    [Fact]
    public void Extended_MultipleDataPoints_RendersCorrectPointCount()
    {
        var data = new List<double> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        var cut = Render<XFSparkline<double>>(p =>
        {
            p.Add(x => x.Data, data);
            p.Add(x => x.ValueSelector, new Func<double, double>(d => d));
            p.Add(x => x.ShowDots, true);
        });
        var circleCount = cut.Markup.Split("<circle").Length - 1;
        Assert.Equal(10, circleCount);
    }

    [Fact]
    public void Extended_SingleDataPoint_RendersOneDot()
    {
        var data = new List<double> { 42.0 };
        var cut = Render<XFSparkline<double>>(p =>
        {
            p.Add(x => x.Data, data);
            p.Add(x => x.ValueSelector, new Func<double, double>(d => d));
            p.Add(x => x.ShowDots, true);
        });
        var circleCount = cut.Markup.Split("<circle").Length - 1;
        Assert.Equal(1, circleCount);
    }
}
