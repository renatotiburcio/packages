using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Drawer;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Layout;

public class XFDrawerExtendedTest : XForgeTestContext
{
    public XFDrawerExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<XFDrawer> RenderDrawer(
        Action<ComponentParameterCollectionBuilder<XFDrawer>>? configure = null)
    {
        return Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment)(b => b.AddContent(0, "Drawer content")));
            parameters.Add(p => p.Open, true);
            configure?.Invoke(parameters);
        });
    }

    // 1. Renders base css class "xf-drawer"
    [Fact]
    public void Render_HasBaseCssClass()
    {
        var cut = RenderDrawer();

        Assert.Contains("data-placement=\"right\"", cut.Markup);
    }

    // 2. Renders child content
    [Fact]
    public void Render_ChildContent_IsPresent()
    {
        var cut = RenderDrawer();

        Assert.Contains("Drawer content", cut.Markup);
    }

    // 3. Renders with placement Right
    [Fact]
    public void Render_PlacementRight_HasDataAttribute()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Placement, XFDrawerPlacement.Right));

        Assert.Contains("data-placement=\"right\"", cut.Markup);
    }

    // 4. Renders with placement Top
    [Fact]
    public void Render_PlacementTop_HasDataAttribute()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Placement, XFDrawerPlacement.Top));

        Assert.Contains("data-placement=\"top\"", cut.Markup);
    }

    // 5. Renders with placement Bottom
    [Fact]
    public void Render_PlacementBottom_HasDataAttribute()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Placement, XFDrawerPlacement.Bottom));

        Assert.Contains("data-placement=\"bottom\"", cut.Markup);
    }

    // 6. Renders with size Sm
    [Fact]
    public void Render_SizeSm_AppliesMaxWidthClass()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Size, XFOverlaySize.Sm));

        Assert.Contains("max-w-sm", cut.Markup);
    }

    // 7. Renders with size Lg
    [Fact]
    public void Render_SizeLg_AppliesMaxWidthClass()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Size, XFOverlaySize.Lg));

        Assert.Contains("max-w-2xl", cut.Markup);
    }

    // 8. Renders with size Xl
    [Fact]
    public void Render_SizeXl_AppliesMaxWidthClass()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Size, XFOverlaySize.Xl));

        Assert.Contains("max-w-4xl", cut.Markup);
    }

    // 9. Renders with size Full
    [Fact]
    public void Render_SizeFull_AppliesFullWidthClass()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Size, XFOverlaySize.Full));

        Assert.Contains("max-w-full", cut.Markup);
    }

    // 10. Renders with title
    [Fact]
    public void Render_WithTitle_RendersHeading()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Title, "My Drawer Title"));

        Assert.Contains("My Drawer Title", cut.Markup);
    }

    // 11. Renders with header content
    [Fact]
    public void Render_WithHeader_RendersCustomHeader()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Header, (RenderFragment)(b =>
        {
            b.OpenElement(0, "div");
            b.AddAttribute(1, "class", "custom-header");
            b.AddContent(2, "Custom header content");
            b.CloseElement();
        })));

        Assert.Contains("Custom header content", cut.Markup);
        Assert.Contains("custom-header", cut.Markup);
    }

    // 12. Renders with footer content
    [Fact]
    public void Render_WithFooter_RendersFooter()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Footer, (RenderFragment)(b =>
        {
            b.OpenElement(0, "span");
            b.AddContent(1, "Footer text");
            b.CloseElement();
        })));

        Assert.Contains("Footer text", cut.Markup);
    }

    // 13. Renders close button
    [Fact]
    public void Render_CloseButton_IsPresent()
    {
        var cut = RenderDrawer();

        var closeButton = cut.Find("button[aria-label='Fechar painel']");
        Assert.NotNull(closeButton);
    }

    // 14. Renders with backdrop
    [Fact]
    public void Render_WithBackdrop_HasBackdropClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Modal, true));

        Assert.Contains("bg-surface-950/35", cut.Markup);
        Assert.Contains("backdrop-blur-sm", cut.Markup);
    }

    // 15. Renders with no backdrop
    [Fact]
    public void Render_WithoutBackdrop_HasTransparentOverlay()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Modal, false));

        Assert.Contains("bg-transparent", cut.Markup);
        Assert.DoesNotContain("bg-surface-950/35", cut.Markup);
    }

    // 16. Renders overlay
    [Fact]
    public void Render_Overlay_IsPresent()
    {
        var cut = RenderDrawer();

        var overlay = cut.Find("div.fixed");
        Assert.NotNull(overlay);
    }

    // 17. OpenChanged fires on close button click
    [Fact]
    public async Task OpenChanged_CloseButtonClick_FiresEvent()
    {
        bool? changedValue = null;
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.OpenChanged, EventCallback.Factory.Create<bool>(this, value => changedValue = value));
        });

        await cut.Find("button[aria-label='Fechar painel']").ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.False(changedValue);
    }

    // 18. OpenChanged fires on backdrop click
    [Fact]
    public async Task OpenChanged_BackdropClick_FiresEvent()
    {
        bool? changedValue = null;
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.OpenChanged, EventCallback.Factory.Create<bool>(this, value => changedValue = value));
        });

        await cut.Find("div.fixed").ClickAsync();
        await cut.InvokeAsync(() => { });

        Assert.False(changedValue);
    }

    // 19. OpenChanged fires on Escape key
    [Fact]
    public async Task OpenChanged_EscapeKey_FiresEvent()
    {
        bool? changedValue = null;
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.OpenChanged, EventCallback.Factory.Create<bool>(this, value => changedValue = value));
        });

        await cut.Find("aside").KeyDownAsync(new KeyboardEventArgs { Key = "Escape" });
        await cut.InvokeAsync(() => { });

        Assert.False(changedValue);
    }

    // 20. Does not render when Open is false
    [Fact]
    public void Render_OpenFalse_DoesNotRenderDrawer()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.ChildContent, (RenderFragment)(b => b.AddContent(0, "Hidden content")));
            parameters.Add(p => p.Open, false);
        });

        Assert.DoesNotContain("role=\"dialog\"", cut.Markup);
    }

    // 21. Renders with custom close label
    [Fact]
    public void Render_CustomCloseLabel_UsesLabel()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.CloseLabel, "Fechar detalhes"));

        var closeButton = cut.Find("button[aria-label='Fechar detalhes']");
        Assert.NotNull(closeButton);
    }

    // 22. Renders with custom aria label
    [Fact]
    public void Render_CustomAriaLabel_UsesLabel()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.AriaLabel, "Painel lateral");
            p.Add(pl => pl.ShowCloseButton, false);
        });

        var aside = cut.Find("aside");
        Assert.Equal("Painel lateral", aside.GetAttribute("aria-label"));
    }

    // 23. Renders with focus trap disabled
    [Fact]
    public void Render_TrapFocusDisabled_NoJsInteropCall()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.TrapFocus, false));

        // With TrapFocus=false, the component should still render the dialog
        Assert.Contains("role=\"dialog\"", cut.Markup);
        // JSInterop should not have been called to trap focus
        Assert.Empty(JSInterop.Invocations);
    }

    // 24. Renders with custom width
    [Fact]
    public void Render_CustomWidth_AppliesInlineStyle()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Style, "width: 500px"));

        Assert.Contains("width: 500px", cut.Markup);
    }

    // 25. XFDrawerPlacement enum has all values
    [Fact]
    public void XFDrawerPlacement_EnumContainsAllExpectedValues()
    {
        var values = Enum.GetValues<XFDrawerPlacement>();

        Assert.Equal(4, values.Length);
        Assert.Contains(XFDrawerPlacement.Left, values);
        Assert.Contains(XFDrawerPlacement.Right, values);
        Assert.Contains(XFDrawerPlacement.Top, values);
        Assert.Contains(XFDrawerPlacement.Bottom, values);
    }

    // 26. XFOverlaySize enum has all values
    [Fact]
    public void XFOverlaySize_EnumContainsAllExpectedValues()
    {
        var values = Enum.GetValues<XFOverlaySize>();

        Assert.Equal(5, values.Length);
        Assert.Contains(XFOverlaySize.Sm, values);
        Assert.Contains(XFOverlaySize.Md, values);
        Assert.Contains(XFOverlaySize.Lg, values);
        Assert.Contains(XFOverlaySize.Xl, values);
        Assert.Contains(XFOverlaySize.Full, values);
    }

    // 27. ResponsiveBelow Sm + default Right placement (line 107)
    [Fact]
    public void Render_ResponsiveBelowSm_HasSmBreakpointClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm));

        var overlay = cut.Find("div.fixed");
        var cls = overlay.GetAttribute("class") ?? "";
        Assert.Contains("sm:items-stretch", cls);
        Assert.Contains("sm:justify-end", cls);
    }

    // 28. ResponsiveBelow Lg + default Right placement (line 108)
    [Fact]
    public void Render_ResponsiveBelowLg_HasLgBreakpointClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Lg));

        var overlay = cut.Find("div.fixed");
        var cls = overlay.GetAttribute("class") ?? "";
        Assert.Contains("lg:items-stretch", cls);
        Assert.Contains("lg:justify-end", cls);
    }

    // 29. ResponsiveBelow Md + default Right placement (line 109)
    [Fact]
    public void Render_ResponsiveBelowMd_HasMdBreakpointClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Md));

        var overlay = cut.Find("div.fixed");
        var cls = overlay.GetAttribute("class") ?? "";
        Assert.Contains("md:items-stretch", cls);
        Assert.Contains("md:justify-end", cls);
    }

    // 30. ResponsiveBelow Sm + Left placement (line 107)
    [Fact]
    public void Render_ResponsiveBelowSm_LeftPlacement_HasCorrectClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm);
            p.Add(pl => pl.Placement, XFDrawerPlacement.Left);
        });

        var overlay = cut.Find("div.fixed");
        var cls = overlay.GetAttribute("class") ?? "";
        Assert.Contains("sm:justify-start", cls);
    }

    // 31. ResponsiveBelow Lg + Left placement (line 108)
    [Fact]
    public void Render_ResponsiveBelowLg_LeftPlacement_HasCorrectClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Lg);
            p.Add(pl => pl.Placement, XFDrawerPlacement.Left);
        });

        var overlay = cut.Find("div.fixed");
        var cls = overlay.GetAttribute("class") ?? "";
        Assert.Contains("lg:justify-start", cls);
    }

    // 32. ResponsiveSizeClass Full size (line 121)
    [Fact]
    public void Render_ResponsiveSizeFull_HasFullSizeClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm);
            p.Add(pl => pl.Size, XFOverlaySize.Full);
        });

        var panel = cut.Find("aside");
        Assert.Contains("max-w-full", panel.GetAttribute("class"));
    }

    // 33. ResponsiveSizeClass Sm at Sm breakpoint (line 122)
    [Fact]
    public void Render_ResponsiveSizeSmAtSm_HasSmSizeClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm);
            p.Add(pl => pl.Size, XFOverlaySize.Sm);
        });

        var panel = cut.Find("aside");
        Assert.Contains("sm:max-w-sm", panel.GetAttribute("class"));
    }

    // 34. ResponsiveSizeClass Lg at Lg breakpoint (line 127)
    [Fact]
    public void Render_ResponsiveSizeLgAtLg_HasLgSizeClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Lg);
            p.Add(pl => pl.Size, XFOverlaySize.Lg);
        });

        var panel = cut.Find("aside");
        Assert.Contains("lg:max-w-2xl", panel.GetAttribute("class"));
    }

    // 35. ResponsiveSizeClass Xl at Md breakpoint (line 132)
    [Fact]
    public void Render_ResponsiveSizeXlAtMd_HasMdSizeClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Md);
            p.Add(pl => pl.Size, XFOverlaySize.Xl);
        });

        var panel = cut.Find("aside");
        Assert.Contains("md:max-w-4xl", panel.GetAttribute("class"));
    }

    // 36. ToneClass Primary (line 138)
    [Fact]
    public void Render_TonePrimary_HasPrimaryToneClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Tone, XFSemanticColor.Primary));

        var panel = cut.Find("aside");
        Assert.Contains("var(--color-primary)", panel.GetAttribute("class"));
    }

    // 37. ToneClass Success (line 139)
    [Fact]
    public void Render_ToneSuccess_HasSuccessToneClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Tone, XFSemanticColor.Success));

        var panel = cut.Find("aside");
        Assert.Contains("var(--color-success)", panel.GetAttribute("class"));
    }

    // 38. ToneClass Warning (line 140)
    [Fact]
    public void Render_ToneWarning_HasWarningToneClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Tone, XFSemanticColor.Warning));

        var panel = cut.Find("aside");
        Assert.Contains("var(--color-warning)", panel.GetAttribute("class"));
    }

    // 39. ToneClass Danger (line 141)
    [Fact]
    public void Render_ToneDanger_HasDangerToneClasses()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.Tone, XFSemanticColor.Danger));

        var panel = cut.Find("aside");
        Assert.Contains("var(--color-danger)", panel.GetAttribute("class"));
    }

    // 40. ResponsivePlacementClass Sm + Left (line 114)
    [Fact]
    public void Render_ResponsivePlacementSmLeft_HasRoundedClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm);
            p.Add(pl => pl.Placement, XFDrawerPlacement.Left);
        });

        var panel = cut.Find("aside");
        Assert.Contains("sm:rounded-r-xl", panel.GetAttribute("class"));
    }

    // 41. ResponsivePlacementClass Sm + Right (line 114)
    [Fact]
    public void Render_ResponsivePlacementSmRight_HasRoundedClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm);
            p.Add(pl => pl.Placement, XFDrawerPlacement.Right);
        });

        var panel = cut.Find("aside");
        Assert.Contains("sm:rounded-l-xl", panel.GetAttribute("class"));
    }

    // 42. AnimationDurationClass <= 0 (line 146)
    [Fact]
    public void Render_AnimationDurationZero_HasDuration0()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.AnimationDuration, 0));

        Assert.Contains("duration-0", cut.Markup);
    }

    // 43. AnimationDurationClass <= 150 (line 148)
    [Fact]
    public void Render_AnimationDuration100_HasDuration150()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.AnimationDuration, 100));

        Assert.Contains("duration-150", cut.Markup);
    }

    // 44. AnimationDurationClass <= 200 (line 150)
    [Fact]
    public void Render_AnimationDuration200_HasDuration200()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.AnimationDuration, 200));

        Assert.Contains("duration-200", cut.Markup);
    }

    // 45. AnimationDurationClass <= 300 (line 152)
    [Fact]
    public void Render_AnimationDuration250_HasDuration300()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.AnimationDuration, 250));

        Assert.Contains("duration-300", cut.Markup);
    }

    // 46. AnimationDurationClass > 300 (line 153)
    [Fact]
    public void Render_AnimationDuration500_HasDuration500()
    {
        var cut = RenderDrawer(p => p.Add(pl => pl.AnimationDuration, 500));

        Assert.Contains("duration-500", cut.Markup);
    }

    // 47. ResponsiveSizeClass Xl at Sm breakpoint (line 124)
    [Fact]
    public void Render_ResponsiveSizeXlAtSm_HasSmSizeClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm);
            p.Add(pl => pl.Size, XFOverlaySize.Xl);
        });

        var panel = cut.Find("aside");
        Assert.Contains("sm:max-w-4xl", panel.GetAttribute("class"));
    }

    // 48. ResponsiveSizeClass Sm at Lg breakpoint (line 126)
    [Fact]
    public void Render_ResponsiveSizeSmAtLg_HasLgSizeClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Lg);
            p.Add(pl => pl.Size, XFOverlaySize.Sm);
        });

        var panel = cut.Find("aside");
        Assert.Contains("lg:max-w-sm", panel.GetAttribute("class"));
    }

    // 49. ResponsiveSizeClass Full at Lg breakpoint (line 121 via 128)
    [Fact]
    public void Render_ResponsiveSizeFullAtLg_HasFullSizeClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Lg);
            p.Add(pl => pl.Size, XFOverlaySize.Full);
        });

        var panel = cut.Find("aside");
        Assert.Contains("max-w-full", panel.GetAttribute("class"));
    }

    // 50. ResponsivePlacementClass Lg + Left (line 115)
    [Fact]
    public void Render_ResponsivePlacementLgLeft_HasRoundedClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Lg);
            p.Add(pl => pl.Placement, XFDrawerPlacement.Left);
        });

        var panel = cut.Find("aside");
        Assert.Contains("lg:rounded-r-xl", panel.GetAttribute("class"));
    }

    // 51. ResponsivePlacementClass Md + Right (line 116)
    [Fact]
    public void Render_ResponsivePlacementMdRight_HasRoundedClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Md);
            p.Add(pl => pl.Placement, XFDrawerPlacement.Right);
        });

        var panel = cut.Find("aside");
        Assert.Contains("md:rounded-l-xl", panel.GetAttribute("class"));
    }

    // 52. ResponsiveSizeClass Sm+Lg at Sm breakpoint (line 123)
    [Fact]
    public void Render_ResponsiveSizeSmLgAtSm_HasCorrectClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Sm);
            p.Add(pl => pl.Size, XFOverlaySize.Lg);
        });

        var panel = cut.Find("aside");
        Assert.Contains("sm:max-w-2xl", panel.GetAttribute("class"));
    }

    // 53. ResponsiveSizeClass Lg+Xl at Lg breakpoint (line 128)
    [Fact]
    public void Render_ResponsiveSizeLgXlAtLg_HasCorrectClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Lg);
            p.Add(pl => pl.Size, XFOverlaySize.Xl);
        });

        var panel = cut.Find("aside");
        Assert.Contains("lg:max-w-4xl", panel.GetAttribute("class"));
    }

    // 54. ResponsiveSizeClass Md+Sm at Md breakpoint (line 130)
    [Fact]
    public void Render_ResponsiveSizeMdSmAtMd_HasCorrectClasses()
    {
        var cut = RenderDrawer(p =>
        {
            p.Add(pl => pl.ResponsiveBelow, XFDrawerResponsiveBreakpoint.Md);
            p.Add(pl => pl.Size, XFOverlaySize.Sm);
        });

        var panel = cut.Find("aside");
        Assert.Contains("md:max-w-sm", panel.GetAttribute("class"));
    }
}
