using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Carousel;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFCarouselExtendedTest : BunitContext
{
    public XFCarouselExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFCarousel> RenderCarousel(
        Action<ComponentParameterCollectionBuilder<XFCarousel>>? configure = null)
    {
        return Render<XFCarousel>(p =>
        {
            configure?.Invoke(p);
        });
    }

    private IRenderedComponent<XFCarousel> RenderCarouselWithSlides(
        Action<ComponentParameterCollectionBuilder<XFCarousel>>? configure = null)
    {
        return Render<XFCarousel>(p =>
        {
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFCarouselSlide>(0);
                builder.AddAttribute(1, "Label", "Slide 1");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Content 1")));
                builder.CloseComponent();
                builder.OpenComponent<XFCarouselSlide>(3);
                builder.AddAttribute(4, "Label", "Slide 2");
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Content 2")));
                builder.CloseComponent();
            }));
            configure?.Invoke(p);
        });
    }

    // ── RootClass ──────────────────────────────────────────────────────────

    [Fact]
    public void Extended_RootClass_ContainsBaseClasses()
    {
        var cut = RenderCarousel();
        Assert.Contains("xf-carousel", cut.Markup);
        Assert.Contains("relative", cut.Markup);
        Assert.Contains("isolate", cut.Markup);
        Assert.Contains("overflow-hidden", cut.Markup);
        Assert.Contains("rounded-lg", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_Disabled_AddsOpacity60()
    {
        var cut = RenderCarousel(p => p.Add(x => x.Disabled, true));
        Assert.Contains("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_NotDisabled_NoOpacity60()
    {
        var cut = RenderCarousel();
        Assert.DoesNotContain("opacity-60", cut.Markup);
    }

    [Fact]
    public void Extended_RootClass_RendersAsSection()
    {
        var cut = RenderCarousel();
        Assert.NotNull(cut.Find("section"));
    }

    // ── ViewportClass ──────────────────────────────────────────────────────

    [Fact]
    public void Extended_ViewportClass_ContainsBaseClasses()
    {
        var cut = RenderCarousel();
        var viewport = cut.Find("[data-xf-carousel-viewport]");
        Assert.NotNull(viewport);
        Assert.Contains("relative", viewport.GetAttribute("class"));
        Assert.Contains("min-h-48", viewport.GetAttribute("class"));
        Assert.Contains("overflow-hidden", viewport.GetAttribute("class"));
    }

    [Fact]
    public void Extended_ViewportClass_SwipeEnabled_AddsTouchPanY()
    {
        var cut = RenderCarousel(p => p.Add(x => x.SwipeEnabled, true));
        var viewport = cut.Find("[data-xf-carousel-viewport]");
        Assert.Contains("touch-pan-y", viewport.GetAttribute("class"));
        Assert.Contains("select-none", viewport.GetAttribute("class"));
    }

    [Fact]
    public void Extended_ViewportClass_SwipeDisabled_RemovesTouchPanY()
    {
        var cut = RenderCarousel(p => p.Add(x => x.SwipeEnabled, false));
        var viewport = cut.Find("[data-xf-carousel-viewport]");
        Assert.DoesNotContain("touch-pan-y", viewport.GetAttribute("class"));
        Assert.DoesNotContain("select-none", viewport.GetAttribute("class"));
    }

    [Fact]
    public void Extended_ViewportClass_Disabled_RemovesTouchPanY()
    {
        var cut = RenderCarousel(p =>
        {
            p.Add(x => x.SwipeEnabled, true);
            p.Add(x => x.Disabled, true);
        });
        var viewport = cut.Find("[data-xf-carousel-viewport]");
        Assert.DoesNotContain("touch-pan-y", viewport.GetAttribute("class"));
    }

    // ── TrackClass ─────────────────────────────────────────────────────────

    [Fact]
    public void Extended_TrackClass_SlideTransition_ContainsFlex()
    {
        var cut = RenderCarousel(p => p.Add(x => x.Transition, XFCarouselTransition.Slide));
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("transition-transform", cut.Markup);
        Assert.Contains("duration-500", cut.Markup);
        Assert.Contains("ease-out", cut.Markup);
    }

    [Fact]
    public void Extended_TrackClass_FadeTransition_ContainsRelative()
    {
        var cut = RenderCarousel(p => p.Add(x => x.Transition, XFCarouselTransition.Fade));
        Assert.Contains("relative", cut.Markup);
        Assert.DoesNotContain("transition-transform", cut.Markup);
    }

    [Fact]
    public void Extended_TrackClass_NoneTransition_ContainsRelative()
    {
        var cut = RenderCarousel(p => p.Add(x => x.Transition, XFCarouselTransition.None));
        Assert.Contains("relative", cut.Markup);
        Assert.DoesNotContain("transition-transform", cut.Markup);
    }

    [Fact]
    public void Extended_TrackClass_SlideTransition_ContainsMinH48()
    {
        var cut = RenderCarousel(p => p.Add(x => x.Transition, XFCarouselTransition.Slide));
        Assert.Contains("min-h-48", cut.Markup);
    }

    // ── ControlsClass ──────────────────────────────────────────────────────

    [Fact]
    public void Extended_ControlsClass_ShowControlsWithSlides_RendersControls()
    {
        var cut = RenderCarouselWithSlides(p => p.Add(x => x.ShowControls, true));
        Assert.Contains("pointer-events-none", cut.Markup);
        Assert.Contains("absolute", cut.Markup);
        Assert.Contains("inset-x-3", cut.Markup);
        Assert.Contains("z-20", cut.Markup);
        Assert.Contains("items-center", cut.Markup);
        Assert.Contains("justify-between", cut.Markup);
    }

    [Fact]
    public void Extended_ControlsClass_ShowControlsFalse_HidesControls()
    {
        var cut = RenderCarouselWithSlides(p => p.Add(x => x.ShowControls, false));
        Assert.DoesNotContain("pointer-events-none", cut.Markup);
    }

    [Fact]
    public void Extended_ControlsClass_NoSlides_HidesControls()
    {
        var cut = RenderCarousel(p => p.Add(x => x.ShowControls, true));
        Assert.DoesNotContain("pointer-events-none", cut.Markup);
    }

    // ── ControlButtonClass ─────────────────────────────────────────────────

    [Fact]
    public void Extended_ControlButtonClass_ContainsAccessibilityClasses()
    {
        var cut = RenderCarouselWithSlides(p => p.Add(x => x.ShowControls, true));
        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 2, "Expected at least 2 control buttons");
        var buttonClass = buttons[0].GetAttribute("class");
        Assert.Contains("pointer-events-auto", buttonClass);
        Assert.Contains("rounded-full", buttonClass);
        Assert.Contains("focus-visible:ring-2", buttonClass);
    }

    // ── IndicatorShellClass ────────────────────────────────────────────────

    [Fact]
    public void Extended_IndicatorShellClass_BottomPosition()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowIndicators, true);
            p.Add(x => x.IndicatorPosition, XFCarouselIndicatorPosition.Bottom);
        });
        Assert.Contains("border-t", cut.Markup);
        Assert.Contains("border-border-light", cut.Markup);
    }

    [Fact]
    public void Extended_IndicatorShellClass_TopPosition()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowIndicators, true);
            p.Add(x => x.IndicatorPosition, XFCarouselIndicatorPosition.Top);
        });
        Assert.Contains("border-b", cut.Markup);
    }

    [Fact]
    public void Extended_IndicatorShellClass_OverlayPosition()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowIndicators, true);
            p.Add(x => x.IndicatorPosition, XFCarouselIndicatorPosition.Overlay);
        });
        Assert.Contains("bg-gradient-to-t", cut.Markup);
        Assert.Contains("from-surface-950/45", cut.Markup);
    }

    [Fact]
    public void Extended_IndicatorShellClass_ShowIndicatorsFalse_HidesIndicators()
    {
        var cut = RenderCarouselWithSlides(p => p.Add(x => x.ShowIndicators, false));
        // When ShowIndicators=false, indicator shell classes should not be present
        Assert.DoesNotContain("bg-gradient-to-t", cut.Markup);
        Assert.DoesNotContain("indicator-shell", cut.Markup);
    }

    // ── GetIndicatorClass ──────────────────────────────────────────────────

    [Fact]
    public void Extended_IndicatorClass_ActiveSlide()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowIndicators, true);
            p.Add(x => x.IndicatorPosition, XFCarouselIndicatorPosition.Bottom);
        });
        Assert.Contains("w-7", cut.Markup);
        Assert.Contains("bg-primary", cut.Markup);
    }

    [Fact]
    public void Extended_IndicatorClass_InactiveSlide()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowIndicators, true);
            p.Add(x => x.IndicatorPosition, XFCarouselIndicatorPosition.Bottom);
        });
        Assert.Contains("w-2.5", cut.Markup);
        Assert.Contains("bg-surface-300", cut.Markup);
    }

    // ── Autoplay ───────────────────────────────────────────────────────────

    [Fact]
    public void Extended_Autoplay_RendersPauseDescription()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.Autoplay, true);
            p.Add(x => x.PauseOnHover, true);
        });
        Assert.Contains("sr-only", cut.Markup);
    }

    [Fact]
    public void Extended_AutoplayFalse_HidesPauseDescription()
    {
        var cut = RenderCarouselWithSlides(p => p.Add(x => x.Autoplay, false));
        Assert.DoesNotContain("sr-only", cut.Markup);
    }

    // ── AriaLabel ──────────────────────────────────────────────────────────

    [Fact]
    public void Extended_AriaLabel_RenderedOnRootElement()
    {
        var cut = RenderCarousel(p => p.Add(x => x.AriaLabel, "My carousel"));
        var section = cut.Find("section");
        Assert.Equal("My carousel", section.GetAttribute("aria-label"));
    }

    [Fact]
    public void Extended_AriaLabel_Null_NotRendered()
    {
        var cut = RenderCarousel();
        var section = cut.Find("section");
        Assert.Null(section.GetAttribute("aria-label"));
    }

    // ── Aria attributes ────────────────────────────────────────────────────

    [Fact]
    public void Extended_Root_HasCarouselRoleDescription()
    {
        var cut = RenderCarousel();
        var section = cut.Find("section");
        Assert.Equal("carousel", section.GetAttribute("aria-roledescription"));
    }

    [Fact]
    public void Extended_Viewport_HasAriaLive()
    {
        var cut = RenderCarousel();
        var viewport = cut.Find("[data-xf-carousel-viewport]");
        Assert.NotNull(viewport.GetAttribute("aria-live"));
    }

    // ── Loop ───────────────────────────────────────────────────────────────

    [Fact]
    public void Extended_Loop_Enabled_ByDefault()
    {
        var cut = RenderCarouselWithSlides(p => p.Add(x => x.ShowControls, true));
        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 2);
        // With loop enabled and 2 slides, neither button should be disabled at index 0
        Assert.Null(buttons[0].GetAttribute("disabled"));
        Assert.Null(buttons[1].GetAttribute("disabled"));
    }

    [Fact]
    public void Extended_Loop_Disabled_PreviousDisabledAtFirstSlide()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowControls, true);
            p.Add(x => x.Loop, false);
        });
        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 2);
        // Previous button should be disabled at first slide with loop disabled
        Assert.NotNull(buttons[0].GetAttribute("disabled"));
    }

    // ── Transition default ─────────────────────────────────────────────────

    [Fact]
    public void Extended_Transition_DefaultIsSlide()
    {
        var cut = RenderCarousel();
        Assert.Contains("flex", cut.Markup);
        Assert.Contains("transition-transform", cut.Markup);
    }

    // ── Indicator role ─────────────────────────────────────────────────────

    [Fact]
    public void Extended_Indicators_HaveTablistRole()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowIndicators, true);
            p.Add(x => x.IndicatorPosition, XFCarouselIndicatorPosition.Bottom);
        });
        Assert.NotNull(cut.Find("[role='tablist']"));
    }

    [Fact]
    public void Extended_Indicators_ButtonsHaveTabRole()
    {
        var cut = RenderCarouselWithSlides(p =>
        {
            p.Add(x => x.ShowIndicators, true);
            p.Add(x => x.IndicatorPosition, XFCarouselIndicatorPosition.Bottom);
        });
        var tabs = cut.FindAll("[role='tab']");
        Assert.Equal(2, tabs.Count);
    }

    // ── SlideCount > 1 controls visibility ─────────────────────────────────

    [Fact]
    public void Extended_SingleSlide_HidesControls()
    {
        var cut = Render<XFCarousel>(p =>
        {
            p.Add(x => x.ShowControls, true);
            p.Add(x => x.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<XFCarouselSlide>(0);
                builder.AddAttribute(1, "Label", "Only Slide");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Only Content")));
                builder.CloseComponent();
            }));
        });
        Assert.DoesNotContain("pointer-events-none", cut.Markup);
    }

    // ── Reduced motion ─────────────────────────────────────────────────────

    [Fact]
    public void Extended_RespectReducedMotion_AddsMotionReduceClasses()
    {
        var cut = RenderCarousel(p =>
        {
            p.Add(x => x.RespectReducedMotion, true);
            p.Add(x => x.Transition, XFCarouselTransition.Slide);
        });
        Assert.Contains("motion-reduce:transition-none", cut.Markup);
        Assert.Contains("motion-reduce:transform-none", cut.Markup);
    }

    [Fact]
    public void Extended_NoRespectReducedMotion_OmitsMotionReduceClasses()
    {
        var cut = RenderCarousel(p =>
        {
            p.Add(x => x.RespectReducedMotion, false);
            p.Add(x => x.Transition, XFCarouselTransition.Slide);
        });
        Assert.DoesNotContain("motion-reduce:transition-none", cut.Markup);
    }
}
