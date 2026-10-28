using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Utilities.Animate;

namespace XForge.DesignSystem.Tests.Components.Utilities;

public sealed class XFAnimateTest : XForgeTestContext
{
    public XFAnimateTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void Animate_Renders_Default()
    {
        var cut = Render<XFAnimate>();
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithChildContent()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "content"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithVisibleFalse()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Visible, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithKeepMounted()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.KeepMounted, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithSlideUpEffect()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Effect, XFAnimateEffect.SlideUp));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithScaleEffect()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Effect, XFAnimateEffect.Scale));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithDuration()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Duration, 500));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithDelay()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Delay, 200));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithCustomEasing()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Easing, "ease-in-out"));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithOnceFalse()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Once, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithMirror()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Mirror, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithThreshold()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Threshold, 0.5));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithRootMargin()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.RootMargin, "0px"));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithCustomClass()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Class, "custom-animate"));
        Assert.NotNull(cut);
    }

    [Fact] public void Animate_Renders_WithTriggerManual()
    {
        var cut = Render<XFAnimate>(p => p.Add(x => x.Trigger, XFAnimateTrigger.Manual));
        Assert.NotNull(cut);
    }
}
