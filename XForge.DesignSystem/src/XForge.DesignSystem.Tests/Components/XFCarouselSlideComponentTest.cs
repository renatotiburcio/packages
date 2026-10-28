using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Carousel;

namespace XForge.DesignSystem.Tests.Components;

public class XFCarouselSlideComponentTest : XForgeTestContext
{
    public XFCarouselSlideComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_AsChildOfCarousel()
    {
        var cut = Render<XFCarousel>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFCarouselSlide>(0);
            builder.AddAttribute(1, nameof(XFCarouselSlide.ChildContent), (RenderFragment)(slide =>
                slide.AddContent(0, "Slide content")));
            builder.CloseComponent();
        }));
        Assert.Contains("Slide content", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFCarousel>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFCarouselSlide>(0);
            builder.AddAttribute(1, nameof(XFCarouselSlide.ChildContent), (RenderFragment)(slide =>
                slide.AddContent(0, "Inner")));
            builder.CloseComponent();
        }));
        Assert.Contains("Inner", cut.Markup);
    }

    [Fact]
    public void Render_SlideWrapperContainsRelativeClass()
    {
        var cut = Render<XFCarousel>(parameters => parameters.AddChildContent(builder =>
        {
            builder.OpenComponent<XFCarouselSlide>(0);
            builder.AddAttribute(1, nameof(XFCarouselSlide.ChildContent), (RenderFragment)(slide =>
                slide.AddContent(0, "Content")));
            builder.CloseComponent();
        }));
        Assert.Contains("min-w-full", cut.Markup);
    }

    [Fact]
    public void Render_WithFadeTransition_SetsOpacityClasses()
    {
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.Transition, XFCarouselTransition.Fade);
            parameters.AddChildContent(builder =>
            {
                builder.OpenComponent<XFCarouselSlide>(0);
                builder.AddAttribute(1, nameof(XFCarouselSlide.ChildContent), (RenderFragment)(slide =>
                    slide.AddContent(0, "Fade")));
                builder.CloseComponent();
            });
        });
        Assert.Contains("transition-opacity", cut.Markup);
    }
}
