#region Using directives
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Typography;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFTypographyComponentTest : Bunit.BunitContext
{
    [Fact]
    public void Render_DefaultBody()
    {
        var cut = Render<XFTypography>(parameters =>
            parameters.AddChildContent("Body text"));

        Assert.Contains("xf-typography", cut.Markup);
        Assert.Contains("<p", cut.Markup);
        Assert.Contains("Body text", cut.Markup);
    }

    [Fact]
    public void Render_H3_UsesSemanticHeading()
    {
        var cut = Render<XFTypography>(parameters =>
        {
            parameters.Add(p => p.Variant, XFTypographyVariant.H3);
            parameters.AddChildContent("Section");
        });

        Assert.Contains("<h3", cut.Markup);
        Assert.Contains("text-2xl", cut.Markup);
    }

    [Fact]
    public void Render_WithAs_OverridesTag()
    {
        var cut = Render<XFTypography>(parameters =>
        {
            parameters.Add(p => p.As, "span");
            parameters.Add(p => p.Variant, XFTypographyVariant.H2);
            parameters.AddChildContent("Styled span");
        });

        Assert.Contains("<span", cut.Markup);
        Assert.Contains("text-3xl", cut.Markup);
    }

    [Fact]
    public void Render_WithText_ColorAndAlignment()
    {
        var cut = Render<XFTypography>(parameters =>
        {
            parameters.Add(p => p.Text, "Danger");
            parameters.Add(p => p.Color, XFTypographyColor.Danger);
            parameters.Add(p => p.Align, XFTypographyAlign.Center);
        });

        Assert.Contains("Danger", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
        Assert.Contains("text-center", cut.Markup);
    }

    [Fact]
    public void Render_WithTruncateAndGutter()
    {
        var cut = Render<XFTypography>(parameters =>
        {
            parameters.Add(p => p.Truncate, true);
            parameters.Add(p => p.GutterBottom, true);
        });

        Assert.Contains("truncate", cut.Markup);
        Assert.Contains("mb-2", cut.Markup);
    }
}
