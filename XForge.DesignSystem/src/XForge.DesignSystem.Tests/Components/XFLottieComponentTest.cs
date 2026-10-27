using Bunit;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Feedback.Lottie;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFLottieComponentTest : BunitContext
{
    [Fact]
    public void Render_Default_RendersContainer()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/animation.json")
        );

        Assert.Contains("role=\"img\"", cut.Markup);
        Assert.Contains("aria-label=\"Animação\"", cut.Markup);
        Assert.Contains("<div", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomAriaLabel_RendersLabel()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.AriaLabel, "Animação de carregamento")
        );

        Assert.Contains("aria-label=\"Animação de carregamento\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomId_RendersId()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.Id, "my-lottie")
        );

        Assert.Contains("id=\"my-lottie\"", cut.Markup);
    }

    [Fact]
    public void Render_WithContainerClass_RendersClass()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.ContainerClass, "w-64 h-64")
        );

        Assert.Contains("w-64", cut.Markup);
        Assert.Contains("h-64", cut.Markup);
    }

    [Fact]
    public void Render_WithStyle_RendersStyle()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.Style, "width: 200px; height: 200px;")
        );

        Assert.Contains("style=\"width: 200px; height: 200px;\"", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_RendersAttributes()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "lottie-player"
            })
        );

        Assert.Contains("data-testid=\"lottie-player\"", cut.Markup);
    }

    [Fact]
    public void Render_WithContainerClass_ContainsExpectedClass()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.ContainerClass, "xf-lottie-custom")
        );

        Assert.Contains("xf-lottie-custom", cut.Markup);
    }

    [Fact]
    public void Render_WithContainerClass_RendersContainerClass()
    {
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.ContainerClass, "xf-lottie rounded-lg")
        );

        Assert.Contains("xf-lottie", cut.Markup);
        Assert.Contains("rounded-lg", cut.Markup);
    }

    [Fact]
    public void Parameters_HaveCorrectDefaults()
    {
        XFLottie? component = null;
        var cut = Render<XFLottie>(p => p
            .Add(x => x.Path, "https://example.com/anim.json")
            .Add(x => x.AriaLabel, "test")
        );

        // Verify defaults via component instance
        component = cut.Instance;
        Assert.True(component.Autoplay);
        Assert.True(component.Loop);
        Assert.Equal(1.0, component.Speed);
        Assert.Equal(XFLottieDirection.Forward, component.Direction);
        Assert.Equal(XFLottieRenderer.Svg, component.Renderer);
        Assert.False(component.Paused);
    }

    [Fact]
    public void Renderer_Enum_HasThreeValues()
    {
        Assert.Equal(3, Enum.GetValues<XFLottieRenderer>().Length);
        Assert.Contains(XFLottieRenderer.Svg, Enum.GetValues<XFLottieRenderer>());
        Assert.Contains(XFLottieRenderer.Canvas, Enum.GetValues<XFLottieRenderer>());
        Assert.Contains(XFLottieRenderer.Html, Enum.GetValues<XFLottieRenderer>());
    }

    [Fact]
    public void Direction_Enum_HasCorrectValues()
    {
        Assert.Equal(1, (int)XFLottieDirection.Forward);
        Assert.Equal(-1, (int)XFLottieDirection.Backward);
    }
}
