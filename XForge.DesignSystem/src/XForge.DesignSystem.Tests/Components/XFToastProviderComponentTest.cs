using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Toast;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public class XFToastProviderComponentTest : XForgeTestContext
{
    public XFToastProviderComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton(new XFToastService());
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFToastProvider>();
        Assert.Contains("xf-toast-provider", cut.Markup);
    }

    [Fact]
    public void Render_WithPlacement_TopStart()
    {
        var cut = Render<XFToastProvider>(parameters =>
            parameters.Add(p => p.Placement, XFToastPlacement.TopStart));
        Assert.Contains("xf-toast-provider", cut.Markup);
        Assert.Contains("top-", cut.Markup);
    }

    [Fact]
    public void Render_WithPlacement_BottomEnd()
    {
        var cut = Render<XFToastProvider>(parameters =>
            parameters.Add(p => p.Placement, XFToastPlacement.BottomEnd));
        Assert.Contains("xf-toast-provider", cut.Markup);
        Assert.Contains("bottom-", cut.Markup);
    }

    [Fact]
    public void Render_WithGap()
    {
        var cut = Render<XFToastProvider>(parameters =>
            parameters.Add(p => p.Gap, 20));
        Assert.Contains("--xf-toast-gap:20px", cut.Markup);
    }

    [Fact]
    public void Render_WithOffset()
    {
        var cut = Render<XFToastProvider>(parameters =>
            parameters.Add(p => p.Offset, 30));
        Assert.Contains("--xf-toast-offset:30px", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLive()
    {
        var cut = Render<XFToastProvider>(parameters =>
            parameters.Add(p => p.Live, XFAriaLive.Assertive));
        Assert.Contains("assertive", cut.Markup);
    }
}
