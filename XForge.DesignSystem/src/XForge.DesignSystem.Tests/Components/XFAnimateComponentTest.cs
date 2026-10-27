using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Utilities.Animate;

namespace XForge.DesignSystem.Tests.Components;

public class XFAnimateComponentTest : BunitContext
{
    [Fact]
    public void Render_VisibleContentWithEnteredState()
    {
        var cut = Render<XFAnimate>(parameters =>
        {
            parameters.AddChildContent("Conteudo");
        });

        Assert.Contains("data-state=\"entered\"", cut.Markup);
        Assert.Contains("opacity-100", cut.Markup);
        Assert.Contains("Conteudo", cut.Markup);
    }

    [Fact]
    public void Render_HiddenWithoutKeepMountedDoesNotRenderContent()
    {
        var cut = Render<XFAnimate>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Conteudo");
        });

        Assert.DoesNotContain("Conteudo", cut.Markup);
    }

    [Fact]
    public void Render_HiddenKeepMountedUsesExitedState()
    {
        var cut = Render<XFAnimate>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.Add(p => p.KeepMounted, true);
            parameters.Add(p => p.Effect, XFAnimateEffect.SlideUp);
            parameters.AddChildContent("Conteudo");
        });

        Assert.Contains("data-state=\"exited\"", cut.Markup);
        Assert.Contains("translate-y-3", cut.Markup);
        Assert.Contains("aria-hidden=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_DurationDelayAndEasing()
    {
        var cut = Render<XFAnimate>(parameters =>
        {
            parameters.Add(p => p.Duration, 320);
            parameters.Add(p => p.Delay, 80);
            parameters.Add(p => p.Easing, "linear");
            parameters.AddChildContent("Conteudo");
        });

        Assert.Contains("transition-duration:320ms", cut.Markup);
        Assert.Contains("transition-delay:80ms", cut.Markup);
        Assert.Contains("transition-timing-function:linear", cut.Markup);
    }

    [Fact]
    public void Render_PulseUsesAnimationClass()
    {
        var cut = Render<XFAnimate>(parameters =>
        {
            parameters.Add(p => p.Effect, XFAnimateEffect.Pulse);
            parameters.AddChildContent("Pulse");
        });

        Assert.Contains("animate-pulse", cut.Markup);
    }
}
