using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Button;
using XForge.DesignSystem.Components.Button.FAB;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Button;

public class XFFABExtendedTest : BunitContext
{
    public XFFABExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFFAB> RenderFAB(
        Action<ComponentParameterCollectionBuilder<XFFAB>>? configure = null)
    {
        return Render<XFFAB>(p =>
        {
            p.Add(x => x.AriaLabel, "FAB");
            configure?.Invoke(p);
        });
    }

    // PlacementClass BottomStart (line 41)
    [Fact]
    public void Extended_PlacementBottomStart_RendersBottomStart()
    {
        var cut = RenderFAB(p => p.Add(x => x.Placement, XFFABPlacement.BottomStart));
        Assert.Contains("bottom-6 left-6", cut.Markup);
    }

    // PlacementClass TopEnd (line 42)
    [Fact]
    public void Extended_PlacementTopEnd_RendersTopEnd()
    {
        var cut = RenderFAB(p => p.Add(x => x.Placement, XFFABPlacement.TopEnd));
        Assert.Contains("right-6 top-6", cut.Markup);
    }

    // PlacementClass TopStart (line 43)
    [Fact]
    public void Extended_PlacementTopStart_RendersTopStart()
    {
        var cut = RenderFAB(p => p.Add(x => x.Placement, XFFABPlacement.TopStart));
        Assert.Contains("left-6 top-6", cut.Markup);
    }

    // PlacementClass default (line 44)
    [Fact]
    public void Extended_PlacementBottomEnd_RendersBottomEnd()
    {
        var cut = RenderFAB(p => p.Add(x => x.Placement, XFFABPlacement.BottomEnd));
        Assert.Contains("bottom-6 right-6", cut.Markup);
    }

    // SizeClass Xs (line 56)
    [Fact]
    public void Extended_SizeXs_RendersExtraSmall()
    {
        var cut = RenderFAB(p => p.Add(x => x.Size, XFExtendedSize.Xs));
        Assert.Contains("h-8 w-8", cut.Markup);
    }

    // SizeClass Sm (line 57)
    [Fact]
    public void Extended_SizeSm_RendersSmall()
    {
        var cut = RenderFAB(p => p.Add(x => x.Size, XFExtendedSize.Sm));
        Assert.Contains("h-11 w-11", cut.Markup);
    }

    // SizeClass Md (line 58)
    [Fact]
    public void Extended_SizeMd_RendersMedium()
    {
        var cut = RenderFAB(p => p.Add(x => x.Size, XFExtendedSize.Md));
        Assert.Contains("h-12 w-12", cut.Markup);
    }

    // SizeClass Xl (line 59)
    [Fact]
    public void Extended_SizeXl_RendersExtraLarge()
    {
        var cut = RenderFAB(p => p.Add(x => x.Size, XFExtendedSize.Xl));
        Assert.Contains("h-16 w-16", cut.Markup);
    }

    // SizeClassMini Xs (line 65)
    [Fact]
    public void Extended_MiniSizeXs_RendersMiniExtraSmall()
    {
        var cut = RenderFAB(p =>
        {
            p.Add(x => x.Mini, true);
            p.Add(x => x.Size, XFExtendedSize.Xs);
        });
        Assert.Contains("h-6 w-6", cut.Markup);
    }

    // SizeClassMini Sm (line 66)
    [Fact]
    public void Extended_MiniSizeSm_RendersMiniSmall()
    {
        var cut = RenderFAB(p =>
        {
            p.Add(x => x.Mini, true);
            p.Add(x => x.Size, XFExtendedSize.Sm);
        });
        Assert.Contains("h-8 w-8", cut.Markup);
    }

    // SizeClassMini Md (line 67)
    [Fact]
    public void Extended_MiniSizeMd_RendersMiniMedium()
    {
        var cut = RenderFAB(p =>
        {
            p.Add(x => x.Mini, true);
            p.Add(x => x.Size, XFExtendedSize.Md);
        });
        Assert.Contains("h-10 w-10", cut.Markup);
    }

    // SizeClassMini Xl (line 68)
    [Fact]
    public void Extended_MiniSizeXl_RendersMiniExtraLarge()
    {
        var cut = RenderFAB(p =>
        {
            p.Add(x => x.Mini, true);
            p.Add(x => x.Size, XFExtendedSize.Xl);
        });
        Assert.Contains("h-14 w-14", cut.Markup);
    }

    // IconClass Xs (line 74)
    [Fact]
    public void Extended_IconSizeXs_RendersSmallIcon()
    {
        var cut = RenderFAB(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xs);
            p.Add(x => x.Icon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    // IconClass Xl (line 75)
    [Fact]
    public void Extended_IconSizeXl_RendersLargeIcon()
    {
        var cut = RenderFAB(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xl);
            p.Add(x => x.Icon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("h-7 w-7", cut.Markup);
    }

    // ToneClass Secondary (line 81)
    [Fact]
    public void Extended_ColorSecondary_RendersSecondaryTone()
    {
        var cut = RenderFAB(p => p.Add(x => x.Color, XFButtonColor.Secondary));
        Assert.Contains("bg-secondary", cut.Markup);
    }

    // ToneClass Success (line 82)
    [Fact]
    public void Extended_ColorSuccess_RendersSuccessTone()
    {
        var cut = RenderFAB(p => p.Add(x => x.Color, XFButtonColor.Success));
        Assert.Contains("bg-success", cut.Markup);
    }

    // ToneClass Warning (line 83)
    [Fact]
    public void Extended_ColorWarning_RendersWarningTone()
    {
        var cut = RenderFAB(p => p.Add(x => x.Color, XFButtonColor.Warning));
        Assert.Contains("bg-warning", cut.Markup);
    }

    // ToneClass Danger (line 84)
    [Fact]
    public void Extended_ColorDanger_RendersDangerTone()
    {
        var cut = RenderFAB(p => p.Add(x => x.Color, XFButtonColor.Danger));
        Assert.Contains("bg-danger", cut.Markup);
    }

    // ToneClass Neutral (line 85)
    [Fact]
    public void Extended_ColorNeutral_RendersNeutralTone()
    {
        var cut = RenderFAB(p => p.Add(x => x.Color, XFButtonColor.Neutral));
        Assert.Contains("bg-surface-950", cut.Markup);
    }

    // ToneClass Info (line 86)
    [Fact]
    public void Extended_ColorInfo_RendersInfoTone()
    {
        var cut = RenderFAB(p => p.Add(x => x.Color, XFButtonColor.Info));
        Assert.Contains("bg-info", cut.Markup);
    }

    // IsExtended (line 49)
    [Fact]
    public void Extended_WithText_RendersExtendedButton()
    {
        var cut = RenderFAB(p => p.Add(x => x.Text, "Add"));
        Assert.Contains("rounded-full px-4", cut.Markup);
        Assert.Contains("gap-2", cut.Markup);
    }

    // Disabled (line 52)
    [Fact]
    public void Extended_Disabled_RendersDisabledButton()
    {
        var cut = RenderFAB(p => p.Add(x => x.Disabled, true));
        Assert.Contains("opacity-60", cut.Markup);
    }

    // MenuClass (line 90)
    [Fact]
    public void Extended_WithMenu_RendersMenuClass()
    {
        var cut = RenderFAB(p =>
        {
            p.Add(x => x.Items, new[] { new XFFABMenuItem { Text = "Action 1" } });
            p.Add(x => x.Open, true);
        });
        Assert.Contains("grid gap-2", cut.Markup);
    }
}
