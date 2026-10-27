#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Alert;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFAlertComponentTest : Bunit.BunitContext
{
    public XFAlertComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFAlert>();

        Assert.Contains("xf-alert", cut.Markup);
    }

    [Fact]
    public void Render_WithIntent_Success()
    {
        var cut = Render<XFAlert>(parameters =>
            parameters.Add(p => p.Intent, XFIntent.Success));

        Assert.Contains("xf-alert", cut.Markup);
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Render_WithIntent_Danger()
    {
        var cut = Render<XFAlert>(parameters =>
            parameters.Add(p => p.Intent, XFIntent.Danger));

        Assert.Contains("xf-alert", cut.Markup);
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Render_WithIntent_Warning()
    {
        var cut = Render<XFAlert>(parameters =>
            parameters.Add(p => p.Intent, XFIntent.Warning));

        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Render_WithDismissible()
    {
        var cut = Render<XFAlert>(parameters =>
        {
            parameters.Add(p => p.Title, "Alert Title");
            parameters.Add(p => p.Dismissible, true);
        });

        Assert.Contains("xf-alert", cut.Markup);
        Assert.Contains("Fechar alerta", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle()
    {
        var cut = Render<XFAlert>(parameters =>
            parameters.Add(p => p.Title, "Important Alert"));

        Assert.Contains("Important Alert", cut.Markup);
    }

    [Fact]
    public void Render_WithText()
    {
        var cut = Render<XFAlert>(parameters =>
            parameters.Add(p => p.Text, "Alert description text"));

        Assert.Contains("Alert description text", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFAlert>(parameters =>
            parameters.AddChildContent("Custom alert content"));

        Assert.Contains("Custom alert content", cut.Markup);
    }

    [Fact]
    public void Render_WithVariant_Outline()
    {
        var cut = Render<XFAlert>(parameters =>
        {
            parameters.Add(p => p.Intent, XFIntent.Info);
            parameters.Add(p => p.Variant, XFAlertVariant.Outline);
        });

        Assert.Contains("bg-bg-surface", cut.Markup);
        Assert.Contains("text-info", cut.Markup);
    }

    [Fact]
    public void Render_WhenVisibleFalse_DoesNotRender()
    {
        var cut = Render<XFAlert>(parameters =>
            parameters.Add(p => p.Visible, false));

        Assert.Equal(string.Empty, cut.Markup);
    }
}
