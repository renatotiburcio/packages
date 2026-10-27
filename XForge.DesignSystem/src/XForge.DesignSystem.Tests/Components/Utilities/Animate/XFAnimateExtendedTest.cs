#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Utilities.Animate;
#endregion

namespace XForge.DesignSystem.Tests.Components.Utilities.Animate;

public sealed class XFAnimateExtendedTest : BunitContext
{
    public XFAnimateExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFAnimate> RenderAnimate(
        Action<ComponentParameterCollectionBuilder<XFAnimate>>? configure = null)
    {
        return Render<XFAnimate>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment)(b => b.AddContent(0, "animated")));
            configure?.Invoke(parameters);
        });
    }

    // ── 1. Default render ──────────────────────────────────────────────

    [Fact]
    public void Default_Renders_XfAnimateBaseClass()
    {
        var cut = RenderAnimate();
        var div = cut.Find("div");

        Assert.Contains("xf-animate", div.GetAttribute("class"));
    }

    [Fact]
    public void Default_Renders_TransitionUtilityClasses()
    {
        var cut = RenderAnimate();
        var div = cut.Find("div");
        var cls = div.GetAttribute("class")!;

        Assert.Contains("transform-gpu", cls);
        Assert.Contains("transition-[opacity,transform,filter]", cls);
        Assert.Contains("will-change-[opacity,transform,filter]", cls);
        Assert.Contains("[backface-visibility:hidden]", cls);
    }

    // ── 2. Visible true renders content ────────────────────────────────

    [Fact]
    public void VisibleTrue_Renders_ChildContent()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Visible, true));

        Assert.Contains("animated", cut.Markup);
    }

    // ── 3. Visible false hides content ─────────────────────────────────

    [Fact]
    public void VisibleFalse_Hides_Content()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Visible, false));

        Assert.DoesNotContain("animated", cut.Markup);
    }

    // ── 4. KeepMounted true renders even when Visible false ────────────

    [Fact]
    public void KeepMountedTrue_VisibleFalse_Renders_Content()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("animated", cut.Markup);
    }

    // ── 5. KeepMounted false hides when Visible false ──────────────────

    [Fact]
    public void KeepMountedFalse_VisibleFalse_Hides_Content()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, false);
        });

        Assert.DoesNotContain("animated", cut.Markup);
    }

    // ── 6. data-state="entered" when visible ───────────────────────────

    [Fact]
    public void VisibleTrue_DataState_Is_Entered()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Visible, true));

        Assert.Equal("entered", cut.Find("div").GetAttribute("data-state"));
    }

    // ── 7. data-state="exited" when not visible ────────────────────────

    [Fact]
    public void VisibleFalse_KeepMountedTrue_DataState_Is_Exited()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Equal("exited", cut.Find("div").GetAttribute("data-state"));
    }

    // ── 8. aria-hidden="true" when not visible ─────────────────────────

    [Fact]
    public void VisibleFalse_KeepMountedTrue_AriaHidden_Is_True()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Equal("true", cut.Find("div").GetAttribute("aria-hidden"));
    }

    // ── 9. aria-hidden="false" when visible ────────────────────────────

    [Fact]
    public void VisibleTrue_AriaHidden_Is_False()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Visible, true));

        Assert.Equal("false", cut.Find("div").GetAttribute("aria-hidden"));
    }

    // ── 10. Effect Fade – exited class has opacity-0 ───────────────────

    [Fact]
    public void EffectFade_VisibleFalse_ExitedClass_Contains_Opacity0()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.Fade);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("opacity-0", cut.Find("div").GetAttribute("class"));
    }

    // ── 11. Effect SlideUp – exited class has translate-y-3 ────────────

    [Fact]
    public void EffectSlideUp_VisibleFalse_ExitedClass_Contains_TranslateY3()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.SlideUp);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("translate-y-3", cut.Find("div").GetAttribute("class"));
    }

    // ── 12. Effect SlideDown – exited class has -translate-y-3 ─────────

    [Fact]
    public void EffectSlideDown_VisibleFalse_ExitedClass_Contains_NegTranslateY3()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.SlideDown);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("-translate-y-3", cut.Find("div").GetAttribute("class"));
    }

    // ── 13. Effect Scale – exited class has scale-95 ───────────────────

    [Fact]
    public void EffectScale_VisibleFalse_ExitedClass_Contains_Scale95()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.Scale);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("scale-95", cut.Find("div").GetAttribute("class"));
    }

    // ── 14. Effect Pulse – adds animate-pulse class ────────────────────

    [Fact]
    public void EffectPulse_Adds_AnimatePulse_Class()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Effect, XFAnimateEffect.Pulse));

        Assert.Contains("animate-pulse", cut.Find("div").GetAttribute("class"));
    }

    // ── 15. Effect ZoomIn – exited class has scale-90 ──────────────────

    [Fact]
    public void EffectZoomIn_VisibleFalse_ExitedClass_Contains_Scale90()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.ZoomIn);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("scale-90", cut.Find("div").GetAttribute("class"));
    }

    // ── 16. Effect FadeBlur – exited class has blur-sm ─────────────────

    [Fact]
    public void EffectFadeBlur_VisibleFalse_ExitedClass_Contains_BlurSm()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.FadeBlur);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("blur-sm", cut.Find("div").GetAttribute("class"));
    }

    // ── 17. Duration renders in style ──────────────────────────────────

    [Fact]
    public void Duration_Renders_In_InlineStyle()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Duration, 500));

        Assert.Contains("transition-duration:500ms", cut.Find("div").GetAttribute("style"));
    }

    // ── 18. Delay renders in style ─────────────────────────────────────

    [Fact]
    public void Delay_Renders_In_InlineStyle()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Delay, 200));

        Assert.Contains("transition-delay:200ms", cut.Find("div").GetAttribute("style"));
    }

    // ── 19. Easing renders in style ────────────────────────────────────

    [Fact]
    public void Easing_Renders_In_InlineStyle()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Easing, "ease-in-out"));

        Assert.Contains("transition-timing-function:ease-in-out", cut.Find("div").GetAttribute("style"));
    }

    // ── 20. Custom class appended ──────────────────────────────────────

    [Fact]
    public void CustomClass_Appended_To_RootClass()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Class, "my-custom"));

        Assert.Contains("my-custom", cut.Find("div").GetAttribute("class"));
    }

    // ── 21. Style attribute renders ────────────────────────────────────

    [Fact]
    public void StyleAttribute_Renders_In_InlineStyle()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Style, "z-index:99"));

        Assert.Contains("z-index:99", cut.Find("div").GetAttribute("style"));
    }

    // ── 22. Id attribute renders ───────────────────────────────────────

    [Fact]
    public void IdAttribute_Renders_On_Div()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Id, "my-animate"));

        Assert.Equal("my-animate", cut.Find("div").GetAttribute("id"));
    }

    // ── 23. AdditionalAttributes render ────────────────────────────────

    [Fact]
    public void AdditionalAttributes_Render_On_Div()
    {
        var cut = RenderAnimate(p => p.AddUnmatched("data-testid", "animate-wrapper"));

        Assert.Equal("animate-wrapper", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── 24. FlipUp effect renders transform-origin in style ────────────

    [Fact]
    public void EffectFlipUp_Renders_TransformOrigin_In_Style()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Effect, XFAnimateEffect.FlipUp));

        var style = cut.Find("div").GetAttribute("style")!;
        Assert.Contains("transform-origin:top", style);
        Assert.Contains("rotateX(", style);
    }

    // ── 25. ChildContent renders correctly ─────────────────────────────

    [Fact]
    public void ChildContent_Renders_Correctly()
    {
        var cut = Render<XFAnimate>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddContent(1, "Hello World");
                b.CloseElement();
            }));
        });

        var span = cut.Find("span");
        Assert.Equal("Hello World", span.TextContent);
    }

    // ── 26. OnVisibleIn callback accepted ──────────────────────────────

    [Fact]
    public void OnVisibleIn_Callback_Accepted()
    {
        var fired = false;
        var cut = RenderAnimate(p =>
            p.Add(x => x.OnVisibleIn, EventCallback.Factory.Create(this, () => fired = true)));

        Assert.NotNull(cut);
    }

    // ── 27. OnVisibleOut callback accepted ─────────────────────────────

    [Fact]
    public void OnVisibleOut_Callback_Accepted()
    {
        var fired = false;
        var cut = RenderAnimate(p =>
            p.Add(x => x.OnVisibleOut, EventCallback.Factory.Create(this, () => fired = true)));

        Assert.NotNull(cut);
    }

    // ── 28. Trigger OnVisible accepted ─────────────────────────────────

    [Fact]
    public void TriggerOnVisible_Accepted()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Trigger, XFAnimateTrigger.OnVisible));

        Assert.Equal("OnVisible", cut.Find("div").GetAttribute("data-trigger"));
    }

    // ── 29. Threshold parameter accepted ───────────────────────────────

    [Fact]
    public void Threshold_Accepted()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Threshold, 0.75));

        Assert.NotNull(cut);
    }

    // ── 30. RootMargin parameter accepted ──────────────────────────────

    [Fact]
    public void RootMargin_Accepted()
    {
        var cut = RenderAnimate(p => p.Add(x => x.RootMargin, "10px 20px"));

        Assert.NotNull(cut);
    }

    // ── Additional: Default Duration is 180ms ──────────────────────────

    [Fact]
    public void DefaultDuration_Is_180ms()
    {
        var cut = RenderAnimate();

        Assert.Contains("transition-duration:180ms", cut.Find("div").GetAttribute("style"));
    }

    // ── Additional: Default Easing is cubic-bezier ─────────────────────

    [Fact]
    public void DefaultEasing_Is_CubicBezier()
    {
        var cut = RenderAnimate();

        Assert.Contains("transition-timing-function:cubic-bezier(0.4,0,0.2,1)", cut.Find("div").GetAttribute("style"));
    }

    // ── Additional: Effect Fade entered class has opacity-100 ───────────

    [Fact]
    public void EffectFade_VisibleTrue_EnteredClass_Contains_Opacity100()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.Fade);
            p.Add(x => x.Visible, true);
        });

        Assert.Contains("opacity-100", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect SlideUp entered class has translate-y-0 ─────

    [Fact]
    public void EffectSlideUp_VisibleTrue_EnteredClass_Contains_TranslateY0()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.SlideUp);
            p.Add(x => x.Visible, true);
        });

        Assert.Contains("translate-y-0", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect Scale entered class has scale-100 ───────────

    [Fact]
    public void EffectScale_VisibleTrue_EnteredClass_Contains_Scale100()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.Scale);
            p.Add(x => x.Visible, true);
        });

        Assert.Contains("scale-100", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect FadeBlur entered class has blur-0 ───────────

    [Fact]
    public void EffectFadeBlur_VisibleTrue_EnteredClass_Contains_Blur0()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.FadeBlur);
            p.Add(x => x.Visible, true);
        });

        Assert.Contains("blur-0", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect Pop exited class has scale-75 ───────────────

    [Fact]
    public void EffectPop_VisibleFalse_ExitedClass_Contains_Scale75()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.Pop);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("scale-75", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect ZoomOut exited class has scale-110 ──────────

    [Fact]
    public void EffectZoomOut_VisibleFalse_ExitedClass_Contains_Scale110()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.ZoomOut);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("scale-110", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect FlipDown renders transform-origin bottom ─────

    [Fact]
    public void EffectFlipDown_Renders_TransformOrigin_Bottom()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Effect, XFAnimateEffect.FlipDown));

        var style = cut.Find("div").GetAttribute("style")!;
        Assert.Contains("transform-origin:bottom", style);
    }

    // ── Additional: Effect FlipLeft renders transform-origin left ───────

    [Fact]
    public void EffectFlipLeft_Renders_TransformOrigin_Left()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Effect, XFAnimateEffect.FlipLeft));

        var style = cut.Find("div").GetAttribute("style")!;
        Assert.Contains("transform-origin:left", style);
        Assert.Contains("rotateY(", style);
    }

    // ── Additional: Effect FlipRight renders transform-origin right ─────

    [Fact]
    public void EffectFlipRight_Renders_TransformOrigin_Right()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Effect, XFAnimateEffect.FlipRight));

        var style = cut.Find("div").GetAttribute("style")!;
        Assert.Contains("transform-origin:right", style);
        Assert.Contains("rotateY(", style);
    }

    // ── Additional: Effect Rise exited class has translate-y-6 ──────────

    [Fact]
    public void EffectRise_VisibleFalse_ExitedClass_Contains_TranslateY6()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.Rise);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("translate-y-6", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect Drop exited class has -translate-y-6 ────────

    [Fact]
    public void EffectDrop_VisibleFalse_ExitedClass_Contains_NegTranslateY6()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.Drop);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("-translate-y-6", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect SlideLeft exited class has translate-x-3 ────

    [Fact]
    public void EffectSlideLeft_VisibleFalse_ExitedClass_Contains_TranslateX3()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.SlideLeft);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("translate-x-3", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect SlideRight exited class has -translate-x-3 ──

    [Fact]
    public void EffectSlideRight_VisibleFalse_ExitedClass_Contains_NegTranslateX3()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.SlideRight);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("-translate-x-3", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Effect SoftBounce exited class has scale-95 and translate-y-2 ─

    [Fact]
    public void EffectSoftBounce_VisibleFalse_ExitedClass_Contains_Scale95_And_TranslateY2()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.SoftBounce);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        var cls = cut.Find("div").GetAttribute("class")!;
        Assert.Contains("scale-95", cls);
        Assert.Contains("translate-y-2", cls);
    }

    // ── Additional: Effect ScaleSoft exited class has scale-[.98] ──────

    [Fact]
    public void EffectScaleSoft_VisibleFalse_ExitedClass_Contains_Scale98()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Effect, XFAnimateEffect.ScaleSoft);
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("scale-[.98]", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Trigger Manual renders data-trigger attribute ───────

    [Fact]
    public void TriggerManual_Renders_DataTrigger()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Trigger, XFAnimateTrigger.Manual));

        Assert.Equal("Manual", cut.Find("div").GetAttribute("data-trigger"));
    }

    // ── Additional: Once parameter accepted ─────────────────────────────

    [Fact]
    public void Once_Accepted()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Once, false));

        Assert.NotNull(cut);
    }

    // ── Additional: Mirror parameter accepted ───────────────────────────

    [Fact]
    public void Mirror_Accepted()
    {
        var cut = RenderAnimate(p => p.Add(x => x.Mirror, true));

        Assert.NotNull(cut);
    }

    // ── Additional: Exited classes contain pointer-events-none ──────────

    [Fact]
    public void VisibleFalse_ExitedClass_Contains_PointerEventsNone()
    {
        var cut = RenderAnimate(p =>
        {
            p.Add(x => x.Visible, false);
            p.Add(x => x.KeepMounted, true);
        });

        Assert.Contains("pointer-events-none", cut.Find("div").GetAttribute("class"));
    }

    // ── Additional: Multiple effects all render without error ───────────

    [Fact]
    public void AllEffects_Render_WithoutError()
    {
        foreach (var effect in Enum.GetValues<XFAnimateEffect>())
        {
            var cut = RenderAnimate(p => p.Add(x => x.Effect, effect));
            Assert.NotNull(cut);
            Assert.Contains("xf-animate", cut.Find("div").GetAttribute("class"));
        }
    }
}
