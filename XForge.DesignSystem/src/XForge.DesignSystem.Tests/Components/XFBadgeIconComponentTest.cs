using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Icon;

namespace XForge.DesignSystem.Tests.Components;

public class XFBadgeIconComponentTest : BunitContext
{
    public XFBadgeIconComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithBrand_RendersSpan()
    {
        var cut = Render<XFBadgeIcon>(parameters =>
            parameters.Add(p => p.Brand, "github"));
        Assert.Contains("xf-badge-icon", cut.Markup);
        Assert.Contains("github", cut.Markup);
    }

    [Fact]
    public void Render_WithSize_SetsDimensions()
    {
        var cut = Render<XFBadgeIcon>(parameters =>
        {
            parameters.Add(p => p.Brand, "twitter");
            parameters.Add(p => p.Size, 48);
        });
        Assert.Contains("width:48px", cut.Markup);
        Assert.Contains("height:48px", cut.Markup);
    }

    [Fact]
    public void Render_HasImageRole()
    {
        var cut = Render<XFBadgeIcon>(parameters =>
            parameters.Add(p => p.Brand, "azure"));
        Assert.Contains("role=\"img\"", cut.Markup);
    }

    [Fact]
    public void Render_UsesBrandAsAriaLabel()
    {
        var cut = Render<XFBadgeIcon>(parameters =>
            parameters.Add(p => p.Brand, "microsoft"));
        Assert.Contains("aria-label=\"microsoft\"", cut.Markup);
    }

    [Fact]
    public void Render_DefaultSize_Is32()
    {
        var cut = Render<XFBadgeIcon>(parameters =>
            parameters.Add(p => p.Brand, "google"));
        Assert.Contains("width:32px", cut.Markup);
    }
}
