#region Using directives
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Progress;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFPageProgressProviderComponentTest : Bunit.BunitContext
{
    public XFPageProgressProviderComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<XFPageProgressService>();
    }

    [Fact]
    public void Render_InitiallyHidden()
    {
        var cut = Render<XFPageProgressProvider>();
        Assert.DoesNotContain("progressbar", cut.Markup);
    }

    [Fact]
    public void Service_Indeterminate_ShowsBar()
    {
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(null);
        cut.Render();

        Assert.Contains("progressbar", cut.Markup);
        Assert.Contains("xf-progress-indeterminate-animate", cut.Markup);
    }

    [Fact]
    public void Service_Determinate_ShowsPercentage()
    {
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(75);
        cut.Render();

        var bar = cut.Find("[role='progressbar']");
        Assert.Equal("75", bar.GetAttribute("aria-valuenow"));
        Assert.Equal("75%", bar.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void Service_Hide_RemovesBar()
    {
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(50);
        cut.Render();
        Assert.Contains("progressbar", cut.Markup);

        service.Go(-1);
        cut.Render();
        Assert.DoesNotContain("progressbar", cut.Markup);
    }

    [Fact]
    public void Service_Color_ChangesBarColor()
    {
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(50, XFProgressColor.Danger);
        cut.Render();

        Assert.Contains("bg-danger", cut.Markup);
    }
}
