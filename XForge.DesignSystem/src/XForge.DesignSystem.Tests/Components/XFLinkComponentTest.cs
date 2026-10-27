using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Link;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFLinkComponentTest : BunitContext
{
    public XFLinkComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithText_RendersLink()
    {
        var cut = Render<XFLink>(parameters =>
            parameters.Add(p => p.Text, "Click here"));

        Assert.Contains("Click here", cut.Markup);
        Assert.Contains("xf-link", cut.Markup);
        Assert.Contains("<a", cut.Markup);
    }

    [Fact]
    public void Render_WithHref_SetsHrefAttribute()
    {
        var cut = Render<XFLink>(parameters =>
        {
            parameters.Add(p => p.Text, "Home");
            parameters.Add(p => p.Href, "/home");
        });

        Assert.Contains("href=\"/home\"", cut.Markup);
    }

    [Fact]
    public void Render_WithTarget_SetsTargetAttribute()
    {
        var cut = Render<XFLink>(parameters =>
        {
            parameters.Add(p => p.Text, "External");
            parameters.Add(p => p.Href, "https://example.com");
            parameters.Add(p => p.Target, "_blank");
        });

        Assert.Contains("target=\"_blank\"", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_RendersContent()
    {
        var cut = Render<XFLink>(parameters =>
        {
            parameters.Add(p => p.ChildContent, "Custom content");
        });

        Assert.Contains("Custom content", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_PrecedesText()
    {
        var cut = Render<XFLink>(parameters =>
        {
            parameters.Add(p => p.Text, "Ignored");
            parameters.Add(p => p.ChildContent, "Visible");
        });

        Assert.Contains("Visible", cut.Markup);
    }

    [Fact]
    public void Render_WithIcon_RendersIcon()
    {
        var cut = Render<XFLink>(parameters =>
        {
            parameters.Add(p => p.Text, "With icon");
            parameters.Add(p => p.Icon, (RenderFragment)(b => b.AddContent(0, "\u2197")));
        });

        Assert.Contains("\u2197", cut.Markup);
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_Disabled_SetsDisabledAttributes()
    {
        var cut = Render<XFLink>(parameters =>
        {
            parameters.Add(p => p.Text, "Disabled");
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
        Assert.Contains("tabindex=\"-1\"", cut.Markup);
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Render_WithoutDisabled_NoDisabledAttributes()
    {
        var cut = Render<XFLink>(parameters =>
            parameters.Add(p => p.Text, "Enabled"));

        Assert.Contains("aria-disabled=\"false\"", cut.Markup);
        Assert.DoesNotContain("cursor-not-allowed", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel_SetsAttribute()
    {
        var cut = Render<XFLink>(parameters =>
        {
            parameters.Add(p => p.Text, "Link");
            parameters.Add(p => p.AriaLabel, "Go to dashboard");
        });

        Assert.Contains("aria-label=\"Go to dashboard\"", cut.Markup);
    }
}
