#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Image;
#endregion

namespace XForge.DesignSystem.Tests.Components.DataDisplay.Image;

public class XFImageExtendedTest : BunitContext
{
    // ─── 1. Default render - renders xf-image class ───────────────────────────

    [Fact]
    public void DefaultRender_ContainsXfImageClass()
    {
        var cut = Render<XFImage>();

        var wrapper = cut.Find(".xf-image");
        Assert.NotNull(wrapper);
    }

    // ─── 2. Src renders img tag with src ──────────────────────────────────────

    [Fact]
    public void Src_RendersImgTagWithSrc()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Src, "/photo.png"));

        var img = cut.Find("img");
        Assert.Equal("/photo.png", img.GetAttribute("src"));
    }

    // ─── 3. Alt text renders on img ───────────────────────────────────────────

    [Fact]
    public void Alt_RendersOnImg()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Alt, "A beautiful sunset");
        });

        var img = cut.Find("img");
        Assert.Equal("A beautiful sunset", img.GetAttribute("alt"));
    }

    // ─── 4. Decorative renders role="presentation" and empty alt ───────────────

    [Fact]
    public void Decorative_RendersRolePresentationAndEmptyAlt()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/deco.png");
            parameters.Add(p => p.Decorative, true);
        });

        var img = cut.Find("img");
        Assert.Equal("presentation", img.GetAttribute("role"));
        Assert.Equal("", img.GetAttribute("alt"));
    }

    // ─── 5. Custom Alt overrides default ──────────────────────────────────────

    [Fact]
    public void CustomAlt_OverridesDefaultAltText()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Alt, "Custom description");
        });

        var img = cut.Find("img");
        Assert.Equal("Custom description", img.GetAttribute("alt"));
    }

    // ─── 6. Fluid true adds w-full ────────────────────────────────────────────

    [Fact]
    public void FluidTrue_AddsWFull()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Fluid, true));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("w-full", wrapper.GetAttribute("class"));
    }

    // ─── 7. Fluid false adds inline-block ─────────────────────────────────────

    [Fact]
    public void FluidFalse_AddsInlineBlock()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Fluid, false));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("inline-block", wrapper.GetAttribute("class"));
    }

    // ─── 8. Lazy true adds loading="lazy" ─────────────────────────────────────

    [Fact]
    public void LazyTrue_AddsLoadingLazy()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Lazy, true);
        });

        var img = cut.Find("img");
        Assert.Equal("lazy", img.GetAttribute("loading"));
    }

    // ─── 9. Lazy false adds loading="eager" ───────────────────────────────────

    [Fact]
    public void LazyFalse_AddsLoadingEager()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Lazy, false);
        });

        var img = cut.Find("img");
        Assert.Equal("eager", img.GetAttribute("loading"));
    }

    // ─── 10. Visible false hides component ────────────────────────────────────

    [Fact]
    public void VisibleFalse_HidesComponent()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Visible, false));

        Assert.DoesNotContain("xf-image", cut.Markup);
    }

    // ─── 11. Visible true shows component ─────────────────────────────────────

    [Fact]
    public void VisibleTrue_ShowsComponent()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Visible, true));

        Assert.Contains("xf-image", cut.Markup);
    }

    // ─── 12. FallbackSrc renders when Src is empty ────────────────────────────

    [Fact]
    public void FallbackSrc_RendersWhenSrcEmpty()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.FallbackSrc, "/fallback.png"));

        var img = cut.Find("img");
        Assert.Equal("/fallback.png", img.GetAttribute("src"));
    }

    // ─── 13. AspectRatio Square adds aspect-square ────────────────────────────

    [Fact]
    public void AspectRatioSquare_AddsAspectSquare()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.AspectRatio, XFImageAspectRatio.Square));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("aspect-square", wrapper.GetAttribute("class"));
    }

    // ─── 14. AspectRatio Video adds aspect-video ──────────────────────────────

    [Fact]
    public void AspectRatioVideo_AddsAspectVideo()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.AspectRatio, XFImageAspectRatio.Video));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("aspect-video", wrapper.GetAttribute("class"));
    }

    // ─── 15. Fit Contain adds object-contain ──────────────────────────────────

    [Fact]
    public void FitContain_AddsObjectContain()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Fit, XFImageFit.Contain);
        });

        var img = cut.Find("img");
        Assert.Contains("object-contain", img.GetAttribute("class"));
    }

    // ─── 16. Fit Fill adds object-fill ────────────────────────────────────────

    [Fact]
    public void FitFill_AddsObjectFill()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Fit, XFImageFit.Fill);
        });

        var img = cut.Find("img");
        Assert.Contains("object-fill", img.GetAttribute("class"));
    }

    // ─── 17. Radius None adds rounded-none ────────────────────────────────────

    [Fact]
    public void RadiusNone_AddsRoundedNone()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Radius, XFImageRadius.None));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("rounded-none", wrapper.GetAttribute("class"));
    }

    // ─── 18. Radius Full adds rounded-full ────────────────────────────────────

    [Fact]
    public void RadiusFull_AddsRoundedFull()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Radius, XFImageRadius.Full));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("rounded-full", wrapper.GetAttribute("class"));
    }

    // ─── 19. FallbackContent renders when Src is empty ────────────────────────

    [Fact]
    public void FallbackContent_RendersWhenSrcEmpty()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.FallbackContent, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-fallback");
                builder.AddContent(2, "No image");
                builder.CloseElement();
            })));

        var fallback = cut.Find(".custom-fallback");
        Assert.NotNull(fallback);
        Assert.Equal("No image", fallback.TextContent);
    }

    // ─── 20. Interactive (OnClick set) adds cursor-pointer and role="button" ───

    [Fact]
    public void Interactive_OnClickSet_AddsCursorPointerAndRoleButton()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => { }));
        });

        var img = cut.Find("img");
        Assert.Equal("button", img.GetAttribute("role"));
        Assert.Contains("cursor-pointer", img.GetAttribute("class"));
    }

    // ─── 21. Non-interactive without OnClick - no cursor-pointer ───────────────

    [Fact]
    public void NonInteractive_NoOnClick_NoCursorPointer()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Src, "/photo.png"));

        var img = cut.Find("img");
        Assert.DoesNotContain("cursor-pointer", img.GetAttribute("class"));
        Assert.Null(img.GetAttribute("role"));
    }

    // ─── 22. Disabled - no interactive class ──────────────────────────────────

    [Fact]
    public void Disabled_NoInteractiveClass()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => { }));
        });

        var img = cut.Find("img");
        Assert.DoesNotContain("cursor-pointer", img.GetAttribute("class"));
        Assert.Null(img.GetAttribute("role"));
    }

    // ─── 23. Click event fires OnClick ────────────────────────────────────────

    [Fact]
    public async Task ClickEvent_FiresOnClick()
    {
        var clicked = false;
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });

        await cut.Find("img").ClickAsync();
        Assert.True(clicked);
    }

    // ─── 24. KeyDown Enter fires OnClick ──────────────────────────────────────

    [Fact]
    public async Task KeyDownEnter_FiresOnClick()
    {
        var clicked = false;
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });

        await cut.Find("img").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        Assert.True(clicked);
    }

    // ─── 25. KeyDown Space fires OnClick ──────────────────────────────────────

    [Fact]
    public async Task KeyDownSpace_FiresOnClick()
    {
        var clicked = false;
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });

        await cut.Find("img").KeyDownAsync(new KeyboardEventArgs { Key = " " });
        Assert.True(clicked);
    }

    // ─── 26. KeyDown other key does nothing ───────────────────────────────────

    [Fact]
    public async Task KeyDownOtherKey_DoesNothing()
    {
        var clicked = false;
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });

        await cut.Find("img").KeyDownAsync(new KeyboardEventArgs { Key = "Tab" });
        Assert.False(clicked);
    }

    // ─── 27. Image error switches to fallback source ──────────────────────────

    [Fact]
    public async Task ImageError_SwitchesToFallbackSource()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/broken.png");
            parameters.Add(p => p.FallbackSrc, "/fallback.png");
        });

        var img = cut.Find("img");
        Assert.Equal("/broken.png", img.GetAttribute("src"));

        cut.Find("img").TriggerEvent("onerror", new EventArgs());

        img = cut.Find("img");
        Assert.Equal("/fallback.png", img.GetAttribute("src"));
    }

    // ─── 28. Both sources fail - renders fallback div ─────────────────────────

    [Fact]
    public void BothSourcesFail_RendersFallbackDiv()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/broken.png");
            parameters.Add(p => p.FallbackSrc, "/also-broken.png");
        });

        var img = cut.Find("img");

        // First error: primary fails, switches to fallback
        img.TriggerEvent("onerror", new EventArgs());

        // Second error: fallback also fails
        img = cut.Find("img");
        img.TriggerEvent("onerror", new EventArgs());

        // Should now render the fallback div instead of img
        Assert.DoesNotContain("<img", cut.Markup);
        var fallbackDiv = cut.Find(".xf-image > div");
        Assert.NotNull(fallbackDiv);
    }

    // ─── 29. FallbackText renders in fallback div ─────────────────────────────

    [Fact]
    public void FallbackText_RendersInFallbackDiv()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/broken.png");
            parameters.Add(p => p.FallbackText, "Custom fallback");
        });

        var img = cut.Find("img");
        img.TriggerEvent("onerror", new EventArgs());

        Assert.Contains("Custom fallback", cut.Markup);
    }

    // ─── 30. UnavailableLabel as aria-label when no Src ───────────────────────

    [Fact]
    public void UnavailableLabel_AriaLabelWhenNoSrc()
    {
        var cut = Render<XFImage>();

        var fallbackDiv = cut.Find(".xf-image > div");
        Assert.NotNull(fallbackDiv);
        Assert.Equal("Imagem indisponivel", fallbackDiv.GetAttribute("aria-label"));
    }

    // ─── 31. Default Alt text when no Alt provided ─────────────────────────────

    [Fact]
    public void DefaultAltText_UsedWhenNoAltProvided()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Src, "/photo.png"));

        var img = cut.Find("img");
        Assert.Equal("Imagem", img.GetAttribute("alt"));
    }

    // ─── 32. Decorative has no aria-label on fallback ─────────────────────────

    [Fact]
    public void DecorativeFallback_HasNoAriaLabel()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Decorative, true));

        var fallbackDiv = cut.Find(".xf-image > div");
        Assert.Null(fallbackDiv.GetAttribute("aria-label"));
    }

    // ─── 33. Interactive sets tabindex on img ─────────────────────────────────

    [Fact]
    public void Interactive_SetsTabIndexOnImg()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => { }));
        });

        var img = cut.Find("img");
        Assert.Equal("0", img.GetAttribute("tabindex"));
    }

    // ─── 34. Non-interactive has no tabindex on img ───────────────────────────

    [Fact]
    public void NonInteractive_NoTabIndexOnImg()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Src, "/photo.png"));

        var img = cut.Find("img");
        Assert.Null(img.GetAttribute("tabindex"));
    }

    // ─── 35. Decoding attribute renders on img ────────────────────────────────

    [Fact]
    public void Decoding_RendersOnImg()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Decoding, "async");
        });

        var img = cut.Find("img");
        Assert.Equal("async", img.GetAttribute("decoding"));
    }

    // ─── 36. Default Radius is Lg (rounded-xl) ────────────────────────────────

    [Fact]
    public void DefaultRadius_IsLg()
    {
        var cut = Render<XFImage>();

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("rounded-xl", wrapper.GetAttribute("class"));
    }

    // ─── 37. AspectRatio Wide adds aspect-[21/9] ──────────────────────────────

    [Fact]
    public void AspectRatioWide_AddsAspect21x9()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.AspectRatio, XFImageAspectRatio.Wide));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("aspect-[21/9]", wrapper.GetAttribute("class"));
    }

    // ─── 38. AspectRatio Portrait adds aspect-[3/4] ───────────────────────────

    [Fact]
    public void AspectRatioPortrait_AddsAspect3x4()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.AspectRatio, XFImageAspectRatio.Portrait));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("aspect-[3/4]", wrapper.GetAttribute("class"));
    }

    // ─── 39. Fit None adds object-none ────────────────────────────────────────

    [Fact]
    public void FitNone_AddsObjectNone()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Fit, XFImageFit.None);
        });

        var img = cut.Find("img");
        Assert.Contains("object-none", img.GetAttribute("class"));
    }

    // ─── 40. Default Fit is Cover (object-cover) ──────────────────────────────

    [Fact]
    public void DefaultFit_IsCover()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Src, "/photo.png"));

        var img = cut.Find("img");
        Assert.Contains("object-cover", img.GetAttribute("class"));
    }

    // ─── 41. Radius Sm adds rounded-md ────────────────────────────────────────

    [Fact]
    public void RadiusSm_AddsRoundedMd()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Radius, XFImageRadius.Sm));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("rounded-md", wrapper.GetAttribute("class"));
    }

    // ─── 42. Radius Md adds rounded-lg ────────────────────────────────────────

    [Fact]
    public void RadiusMd_AddsRoundedLg()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Radius, XFImageRadius.Md));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("rounded-lg", wrapper.GetAttribute("class"));
    }

    // ─── 43. Radius Xl adds rounded-2xl ───────────────────────────────────────

    [Fact]
    public void RadiusXl_AddsRounded2xl()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Radius, XFImageRadius.Xl));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("rounded-2xl", wrapper.GetAttribute("class"));
    }

    // ─── 44. Custom class is appended ─────────────────────────────────────────

    [Fact]
    public void CustomClass_IsAppended()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.Class, "my-custom-class"));

        var wrapper = cut.Find(".xf-image");
        Assert.Contains("my-custom-class", wrapper.GetAttribute("class"));
    }

    // ─── 45. Custom DefaultAltText overrides default ──────────────────────────

    [Fact]
    public void CustomDefaultAltText_OverridesDefault()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.DefaultAltText, "Minha imagem");
        });

        var img = cut.Find("img");
        Assert.Equal("Minha imagem", img.GetAttribute("alt"));
    }

    // ─── 46. Custom UnavailableLabel overrides default ────────────────────────

    [Fact]
    public void CustomUnavailableLabel_OverridesDefault()
    {
        var cut = Render<XFImage>(parameters =>
            parameters.Add(p => p.UnavailableLabel, "Imagem nao disponivel"));

        var fallbackDiv = cut.Find(".xf-image > div");
        Assert.Equal("Imagem nao disponivel", fallbackDiv.GetAttribute("aria-label"));
    }

    // ─── 47. Click on disabled component does not fire OnClick ────────────────

    [Fact]
    public async Task ClickDisabled_DoesNotFireOnClick()
    {
        var clicked = false;
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });

        await cut.Find("img").ClickAsync();
        Assert.False(clicked);
    }

    // ─── 48. KeyDown Enter on disabled does not fire OnClick ──────────────────

    [Fact]
    public async Task KeyDownEnterDisabled_DoesNotFireOnClick()
    {
        var clicked = false;
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/photo.png");
            parameters.Add(p => p.Disabled, true);
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
        });

        await cut.Find("img").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        Assert.False(clicked);
    }

    // ─── 49. Src takes priority over FallbackSrc when both provided ────────────

    [Fact]
    public void SrcTakesPriority_OverFallbackSrc()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.Src, "/primary.png");
            parameters.Add(p => p.FallbackSrc, "/fallback.png");
        });

        var img = cut.Find("img");
        Assert.Equal("/primary.png", img.GetAttribute("src"));
    }

    // ─── 50. Interactive fallback div has role="button" ───────────────────────

    [Fact]
    public void InteractiveFallbackDiv_HasRoleButton()
    {
        var cut = Render<XFImage>(parameters =>
        {
            parameters.Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => { }));
        });

        var fallbackDiv = cut.Find(".xf-image > div");
        Assert.Equal("button", fallbackDiv.GetAttribute("role"));
    }
}
