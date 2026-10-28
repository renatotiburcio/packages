using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Progress;

namespace XForge.DesignSystem.Tests.Components;

public class XFProgressAriaTest : XForgeTestContext
{
    [Fact]
    public void Role_IsProgressbar()
    {
        var cut = Render<XFProgress>(parameters => parameters.Add(p => p.Value, 50));

        Assert.Contains("role=\"progressbar\"", cut.Markup);
    }

    [Fact]
    public void AriaValueMinMaxNow_ArePresent()
    {
        var cut = Render<XFProgress>(parameters =>
        {
            parameters.Add(p => p.Value, 50);
            parameters.Add(p => p.Min, 0);
            parameters.Add(p => p.Max, 200);
        });

        var progress = cut.Find("[role='progressbar']");
        Assert.Equal("0", progress.GetAttribute("aria-valuemin"));
        Assert.Equal("200", progress.GetAttribute("aria-valuemax"));
        Assert.Equal("50", progress.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void AriaValueText_ShowsPercentage()
    {
        var cut = Render<XFProgress>(parameters =>
        {
            parameters.Add(p => p.Value, 75);
            parameters.Add(p => p.Min, 0);
            parameters.Add(p => p.Max, 100);
        });

        var progress = cut.Find("[role='progressbar']");
        Assert.Equal("75%", progress.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void AriaValueNow_WhenIndeterminate_IsAbsent()
    {
        var cut = Render<XFProgress>(parameters =>
        {
            parameters.Add(p => p.Indeterminate, true);
        });

        var progress = cut.Find("[role='progressbar']");
        Assert.False(progress.HasAttribute("aria-valuenow"));
    }

    [Fact]
    public void AriaValueText_WhenIndeterminate_IsCarregando()
    {
        var cut = Render<XFProgress>(parameters =>
        {
            parameters.Add(p => p.Indeterminate, true);
        });

        var progress = cut.Find("[role='progressbar']");
        Assert.Equal("Carregando", progress.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void AriaLabel_WhenSet_IsPresent()
    {
        var cut = Render<XFProgress>(parameters =>
        {
            parameters.Add(p => p.Value, 50);
            parameters.Add(p => p.AriaLabel, "Upload progress");
        });

        var progress = cut.Find("[role='progressbar']");
        Assert.Equal("Upload progress", progress.GetAttribute("aria-label"));
    }
}
