using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Progress;

namespace XForge.DesignSystem.Tests.Components;

public class XFPageProgressComponentTest : BunitContext
{
    [Fact]
    public void Render_InitiallyHidden()
    {
        Services.AddSingleton<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        Assert.DoesNotContain("progressbar", cut.Markup);
    }

    [Fact]
    public void Service_Indeterminate_ShowsBar()
    {
        Services.AddSingleton<XFPageProgressService>();
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
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(75);
        cut.Render();

        var bar = cut.Find("[role='progressbar']");
        Assert.Equal("75", bar.GetAttribute("aria-valuenow"));
        Assert.Equal("75%", bar.GetAttribute("aria-valuetext"));
        Assert.Contains("width: 75%;", cut.Markup);
    }

    [Fact]
    public void Service_Hide_RemovesBar()
    {
        Services.AddSingleton<XFPageProgressService>();
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
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(50, XFProgressColor.Danger);
        cut.Render();

        Assert.Contains("bg-danger", cut.Markup);
    }

    [Fact]
    public void Service_ZeroPercent_ShowsZero()
    {
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(0);
        cut.Render();

        var bar = cut.Find("[role='progressbar']");
        Assert.Equal("0", bar.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void Container_HasFixedPositioning()
    {
        Services.AddSingleton<XFPageProgressService>();
        var service = Services.GetRequiredService<XFPageProgressService>();
        var cut = Render<XFPageProgressProvider>();

        service.Go(50);
        cut.Render();

        Assert.Contains("xf-page-progress-container", cut.Markup);
    }
}
