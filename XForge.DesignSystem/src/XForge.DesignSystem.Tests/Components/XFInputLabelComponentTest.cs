using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Forms.Label;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFInputLabelComponentTest : XForgeTestContext
{
    public XFInputLabelComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithText_RendersLabel()
    {
        var cut = Render<XFInputLabel>(parameters =>
            parameters.Add(p => p.Text, "Name"));

        Assert.Contains("Name", cut.Markup);
        Assert.Contains("xf-input-label", cut.Markup);
    }

    [Fact]
    public void Render_WithFor_SetsForAttribute()
    {
        var cut = Render<XFInputLabel>(parameters =>
        {
            parameters.Add(p => p.Text, "Email");
            parameters.Add(p => p.For, "email-input");
        });

        Assert.Contains("for=\"email-input\"", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_RendersContent()
    {
        var cut = Render<XFInputLabel>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Custom Label");
        });

        Assert.Contains("Custom Label", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_PrecedesText()
    {
        var cut = Render<XFInputLabel>(parameters =>
        {
            parameters.Add(p => p.Text, "Ignored");
            parameters.Add(p => p.ChildContent, "Visible");
        });

        Assert.Contains("Visible", cut.Markup);
    }

    [Fact]
    public void Render_Required_ShowsRequiredIndicator()
    {
        var cut = Render<XFInputLabel>(parameters =>
        {
            parameters.Add(p => p.Text, "Username");
            parameters.Add(p => p.Required, true);
        });

        Assert.Contains("text-danger", cut.Markup);
        Assert.Contains("&ast;", cut.Markup);
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithoutRequired_NoIndicator()
    {
        var cut = Render<XFInputLabel>(parameters =>
            parameters.Add(p => p.Text, "Username"));

        Assert.DoesNotContain("text-danger", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel_SetsAttribute()
    {
        var cut = Render<XFInputLabel>(parameters =>
        {
            parameters.Add(p => p.Text, "Label");
            parameters.Add(p => p.AriaLabel, "Accessible label");
        });

        Assert.Contains("aria-label=\"Accessible label\"", cut.Markup);
    }
}
