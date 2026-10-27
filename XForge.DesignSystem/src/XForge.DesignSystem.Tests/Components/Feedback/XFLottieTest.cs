using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Feedback.Lottie;

namespace XForge.DesignSystem.Tests.Components.Feedback;

public sealed class XFLottieTest : BunitContext
{
    public XFLottieTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void Lottie_Renders_Default()
    {
        var cut = Render<XFLottie>();
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithPath()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Path, "animation.json"));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithAutoplayFalse()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Autoplay, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithLoopFalse()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Loop, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithSpeed()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Speed, 0.5));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithDirectionBackward()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Direction, XFLottieDirection.Backward));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithRendererCanvas()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Renderer, XFLottieRenderer.Canvas));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithRendererHtml()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Renderer, XFLottieRenderer.Html));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithPaused()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Paused, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithBackgroundColor()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.BackgroundColor, "#ffffff"));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithCustomAriaLabel()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.AriaLabel, "Loading animation"));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithContainerClass()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.ContainerClass, "lottie-container"));
        Assert.NotNull(cut);
    }

    [Fact] public void Lottie_Renders_WithCustomClass()
    {
        var cut = Render<XFLottie>(p => p.Add(x => x.Class, "custom-lottie"));
        Assert.NotNull(cut);
    }
}
