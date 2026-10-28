using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Utilities.LiveRegion;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFLiveRegionComponentTest : XForgeTestContext
{
    public XFLiveRegionComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_PoliteMode()
    {
        var cut = Render<XFLiveRegion>(parameters =>
            parameters.Add(p => p.Text, "Loading complete"));

        Assert.Contains("Loading complete", cut.Markup);
        Assert.Contains("aria-live=\"polite\"", cut.Markup);
        Assert.Contains("role=\"status\"", cut.Markup);
        Assert.Contains("aria-atomic=\"true\"", cut.Markup);
        Assert.Contains("xf-live-region", cut.Markup);
    }

    [Fact]
    public void Render_Assertive_SetsAssertiveAriaLive()
    {
        var cut = Render<XFLiveRegion>(parameters =>
        {
            parameters.Add(p => p.Text, "Error occurred");
            parameters.Add(p => p.Assertive, true);
        });

        Assert.Contains("aria-live=\"assertive\"", cut.Markup);
        Assert.Contains("role=\"alert\"", cut.Markup);
    }

    [Fact]
    public void Render_AssertiveFalse_SetsPolite()
    {
        var cut = Render<XFLiveRegion>(parameters =>
        {
            parameters.Add(p => p.Text, "Update");
            parameters.Add(p => p.Assertive, false);
        });

        Assert.Contains("aria-live=\"polite\"", cut.Markup);
        Assert.Contains("role=\"status\"", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_RendersContent()
    {
        var cut = Render<XFLiveRegion>(parameters =>
            parameters.Add(p => p.ChildContent, "Dynamic message"));

        Assert.Contains("Dynamic message", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_PrecedesText()
    {
        var cut = Render<XFLiveRegion>(parameters =>
        {
            parameters.Add(p => p.Text, "Ignored");
            parameters.Add(p => p.ChildContent, "Visible");
        });

        Assert.Contains("Visible", cut.Markup);
    }

    [Fact]
    public void Render_Always_HasAriaAtomicTrue()
    {
        var cut = Render<XFLiveRegion>(parameters =>
            parameters.Add(p => p.Text, "Content"));

        Assert.Contains("aria-atomic=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_HasVisuallyHiddenStyles()
    {
        var cut = Render<XFLiveRegion>(parameters =>
            parameters.Add(p => p.Text, "Hidden from view"));

        Assert.Contains("overflow-hidden", cut.Markup);
        Assert.Contains("whitespace-nowrap", cut.Markup);
    }
}
