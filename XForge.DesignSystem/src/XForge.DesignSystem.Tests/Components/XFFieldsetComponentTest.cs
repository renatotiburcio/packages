#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Field;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFFieldsetComponentTest : Bunit.BunitContext
{
    public XFFieldsetComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFFieldset>();
        Assert.Contains("xf-fieldset", cut.Markup);
        Assert.Contains("fieldset", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFFieldset>(parameters =>
            parameters.AddChildContent("<span>Test content</span>"));
        Assert.Contains("Test content", cut.Markup);
    }

    [Fact]
    public void Render_WithLegend()
    {
        var cut = Render<XFFieldset>(parameters =>
            parameters.Add(p => p.Legend, "Personal Info"));
        Assert.Contains("Personal Info", cut.Markup);
        Assert.Contains("<legend", cut.Markup);
    }

    [Fact]
    public void Render_WithDescription()
    {
        var cut = Render<XFFieldset>(parameters =>
        {
            parameters.Add(p => p.Legend, "Address");
            parameters.Add(p => p.Description, "Enter your mailing address");
        });
        Assert.Contains("Enter your mailing address", cut.Markup);
    }

    [Fact]
    public void Render_WithBorderedFalse()
    {
        var cut = Render<XFFieldset>(parameters =>
            parameters.Add(p => p.Bordered, false));
        Assert.Contains("xf-fieldset", cut.Markup);
        Assert.DoesNotContain("rounded-lg border", cut.Markup);
    }

    [Fact]
    public void Render_WithLegendContent()
    {
        var cut = Render<XFFieldset>(parameters =>
            parameters.Add(p => p.LegendContent, builder =>
                builder.AddContent(0, "Custom Legend")));
        Assert.Contains("Custom Legend", cut.Markup);
    }

    [Fact]
    public void Render_Disabled()
    {
        var cut = Render<XFFieldset>(parameters =>
            parameters.Add(p => p.Disabled, true));
        Assert.Contains("disabled", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }
}
