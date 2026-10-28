#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Carousel;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFCarouselComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithSlidesControlsAndIndicators()
    {
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.AriaLabel, "Featured content");
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
                RenderSlide20(builder, "three", "Slide three");
            });
        });

        Assert.Contains("xf-carousel", cut.Markup);
        Assert.Contains("aria-roledescription=\"carousel\"", cut.Markup);
        Assert.Contains("aria-label=\"Featured content\"", cut.Markup);
        Assert.Equal(3, cut.FindAll("[aria-roledescription=slide]").Count);
        Assert.Equal(3, cut.FindAll("button[role=tab]").Count);
        Assert.Contains("Slide anterior", cut.Markup);
        Assert.Contains("Proximo slide", cut.Markup);
    }

    [Fact]
    public async Task NextAndPrevious_UpdateSelectedIndex()
    {
        var selected = 0;
        XFCarouselChangeEventArgs? changed = null;
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.SelectedIndex, selected);
            parameters.Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, next => selected = next));
            parameters.Add(p => p.Change, EventCallback.Factory.Create<XFCarouselChangeEventArgs>(this, args => changed = args));
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
            });
        });

        await cut.Find("button[aria-label='Proximo slide']").ClickAsync();

        Assert.Equal(1, selected);
        Assert.NotNull(changed);
        Assert.Equal("two", changed!.SlideId);

        await cut.Find("button[aria-label='Slide anterior']").ClickAsync();

        Assert.Equal(0, selected);
    }

    [Fact]
    public async Task Indicator_GoesToSlide()
    {
        var selected = 0;
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, next => selected = next));
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
                RenderSlide20(builder, "three", "Slide three");
            });
        });

        await cut.Find("button[aria-label='Ir para slide 3']").ClickAsync();

        Assert.Equal(2, selected);
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomIndicatorLabels_UsesLocalizedLabels()
    {
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.IndicatorsLabel, "Carousel pages");
            parameters.Add(p => p.IndicatorLabelFormat, "Open slide {0}");
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
            });
        });

        Assert.Contains("role=\"tablist\" aria-label=\"Carousel pages\"", cut.Markup);
        Assert.Contains("aria-label=\"Open slide 1\"", cut.Markup);
        Assert.Contains("aria-label=\"Open slide 2\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Slides\"", cut.Markup);
    }

    [Fact]
    public async Task Keyboard_NavigatesSlides()
    {
        var selected = 0;
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, next => selected = next));
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
                RenderSlide20(builder, "three", "Slide three");
            });
        });

        await cut.Find("section").KeyDownAsync(new KeyboardEventArgs { Key = "End" });

        Assert.Equal(2, selected);

        await cut.Find("section").KeyDownAsync(new KeyboardEventArgs { Key = "Home" });

        Assert.Equal(0, selected);
    }

    [Fact]
    public void Render_AutoplayUsesOffLiveRegionAndPauseDescription()
    {
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.Autoplay, true);
            parameters.Add(p => p.Interval, 1000);
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
            });
        });

        Assert.Contains("aria-live=\"off\"", cut.Markup);
        Assert.Contains("A reproducao automatica pausa ao passar o mouse ou focar.", cut.Markup);
    }

    [Fact]
    public void Render_ReducedMotionAddsMotionReduceClasses()
    {
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.RespectReducedMotion, true);
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
            });
        });

        Assert.Contains("motion-reduce:transition-none", cut.Markup);
    }

    [Fact]
    public void Render_FadeTransitionUsesOnlyActiveSlide()
    {
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.Transition, XFCarouselTransition.Fade);
            parameters.Add(p => p.SelectedIndex, 1);
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
            });
        });

        Assert.Contains("opacity-100", cut.Markup);
        Assert.Contains("opacity-0", cut.Markup);
    }

    [Fact]
    public async Task Swipe_NavigatesSlidesWhenThresholdIsReached()
    {
        var selected = 0;
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, next => selected = next));
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
                RenderSlide20(builder, "three", "Slide three");
            });
        });

        var viewport = cut.Find("[data-xf-carousel-viewport]");

        await viewport.PointerDownAsync(new PointerEventArgs { ClientX = 160, ClientY = 20 });
        await viewport.PointerUpAsync(new PointerEventArgs { ClientX = 70, ClientY = 24 });

        Assert.Equal(1, selected);

        await viewport.PointerDownAsync(new PointerEventArgs { ClientX = 70, ClientY = 20 });
        await viewport.PointerUpAsync(new PointerEventArgs { ClientX = 150, ClientY = 24 });

        Assert.Equal(0, selected);
    }

    [Fact]
    public async Task Swipe_IgnoresSmallOrVerticalMovements()
    {
        var selected = 0;
        var cut = Render<XFCarousel>(parameters =>
        {
            parameters.Add(p => p.SwipeThreshold, 60);
            parameters.Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, next => selected = next));
            parameters.Add(p => p.ChildContent, builder =>
            {
                RenderSlide0(builder, "one", "Slide one");
                RenderSlide10(builder, "two", "Slide two");
            });
        });

        var viewport = cut.Find("[data-xf-carousel-viewport]");

        await viewport.PointerDownAsync(new PointerEventArgs { ClientX = 100, ClientY = 20 });
        await viewport.PointerUpAsync(new PointerEventArgs { ClientX = 55, ClientY = 24 });

        Assert.Equal(0, selected);

        await viewport.PointerDownAsync(new PointerEventArgs { ClientX = 100, ClientY = 20 });
        await viewport.PointerUpAsync(new PointerEventArgs { ClientX = 20, ClientY = 140 });

        Assert.Equal(0, selected);
    }

    private static void RenderSlide0(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, string id, string label)
    {
        builder.OpenComponent<XFCarouselSlide>(0);
        builder.AddAttribute(1, "SlideId", id);
        builder.AddAttribute(2, "Label", label);
        builder.AddAttribute(3, "ChildContent", (RenderFragment)(childBuilder => childBuilder.AddContent(0, label)));
        builder.CloseComponent();
    }

    private static void RenderSlide10(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, string id, string label)
    {
        builder.OpenComponent<XFCarouselSlide>(10);
        builder.AddAttribute(11, "SlideId", id);
        builder.AddAttribute(12, "Label", label);
        builder.AddAttribute(13, "ChildContent", (RenderFragment)(childBuilder => childBuilder.AddContent(0, label)));
        builder.CloseComponent();
    }

    private static void RenderSlide20(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, string id, string label)
    {
        builder.OpenComponent<XFCarouselSlide>(20);
        builder.AddAttribute(21, "SlideId", id);
        builder.AddAttribute(22, "Label", label);
        builder.AddAttribute(23, "ChildContent", (RenderFragment)(childBuilder => childBuilder.AddContent(0, label)));
        builder.CloseComponent();
    }
}
