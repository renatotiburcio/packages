using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Button;
using XForge.DesignSystem.Components.Button.SplitButton;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Button;

public sealed class XFSplitButtonExtendedTest : BunitContext
{
    public XFSplitButtonExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFSplitButton> RenderSplitButton(
        Action<ComponentParameterCollectionBuilder<XFSplitButton>>? configure = null)
    {
        return Render<XFSplitButton>(p =>
        {
            p.Add(x => x.Text, "Action");
            p.Add(x => x.MenuAriaLabel, "Menu");
            configure?.Invoke(p);
        });
    }

    // IconClass Xs/Sm (lines 41-43) — requires StartIcon to be set
    private static RenderFragment IconContent() => builder => builder.AddContent(0, "icon");

    [Fact]
    public void Extended_SizeXs_RendersSmallIcon()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Xs);
            p.Add(x => x.StartIcon, IconContent());
        });
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    [Fact]
    public void Extended_SizeSm_RendersSmallIcon()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Sm);
            p.Add(x => x.StartIcon, IconContent());
        });
        Assert.Contains("h-4 w-4", cut.Markup);
    }

    [Fact]
    public void Extended_SizeMd_RendersLargeIcon()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Size, XFExtendedSize.Md);
            p.Add(x => x.StartIcon, IconContent());
        });
        Assert.Contains("h-5 w-5", cut.Markup);
    }

    // ToneTextClass branches (lines 88-93)
    [Fact]
    public void Extended_ColorSecondary_Outline_RendersSecondaryText()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Secondary);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_ColorSuccess_Outline_RendersSuccessText()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-success", cut.Markup);
    }

    [Fact]
    public void Extended_ColorWarning_Outline_RendersWarningText()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-warning", cut.Markup);
    }

    [Fact]
    public void Extended_ColorDanger_Outline_RendersDangerText()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Extended_ColorNeutral_Outline_RendersNeutralText()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Neutral);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-text-primary", cut.Markup);
    }

    [Fact]
    public void Extended_ColorInfo_Outline_RendersInfoText()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("text-info", cut.Markup);
    }

    // ToneSolidClass branches (lines 109-114)
    [Fact]
    public void Extended_ColorSecondary_Solid_RendersSecondarySolid()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Secondary);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-secondary", cut.Markup);
    }

    [Fact]
    public void Extended_ColorSuccess_Solid_RendersSuccessSolid()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-success", cut.Markup);
    }

    [Fact]
    public void Extended_ColorWarning_Solid_RendersWarningSolid()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-warning", cut.Markup);
    }

    [Fact]
    public void Extended_ColorDanger_Solid_RendersDangerSolid()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-danger", cut.Markup);
    }

    [Fact]
    public void Extended_ColorNeutral_Solid_RendersNeutralSolid()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Neutral);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-surface-950", cut.Markup);
    }

    [Fact]
    public void Extended_ColorInfo_Solid_RendersInfoSolid()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Solid);
        });
        Assert.Contains("bg-info", cut.Markup);
    }

    // ToneSoftClass branches (lines 120-125)
    [Fact]
    public void Extended_ColorSecondary_Soft_RendersSecondarySoft()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Secondary);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_ColorSuccess_Soft_RendersSuccessSoft()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-success)", cut.Markup);
    }

    [Fact]
    public void Extended_ColorWarning_Soft_RendersWarningSoft()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    [Fact]
    public void Extended_ColorDanger_Soft_RendersDangerSoft()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    [Fact]
    public void Extended_ColorNeutral_Soft_RendersNeutralSoft()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Neutral);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_ColorInfo_Soft_RendersInfoSoft()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Soft);
        });
        Assert.Contains("var(--color-info)", cut.Markup);
    }

    // ToneBorderClass branches (lines 100-103)
    [Fact]
    public void Extended_ColorSuccess_Border_RendersSuccessBorder()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Success);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-success", cut.Markup);
    }

    [Fact]
    public void Extended_ColorWarning_Border_RendersWarningBorder()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Warning);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-warning", cut.Markup);
    }

    [Fact]
    public void Extended_ColorDanger_Border_RendersDangerBorder()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Danger);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-danger", cut.Markup);
    }

    [Fact]
    public void Extended_ColorInfo_Border_RendersInfoBorder()
    {
        var cut = RenderSplitButton(p =>
        {
            p.Add(x => x.Color, XFButtonColor.Info);
            p.Add(x => x.Variant, XFButtonVariant.Outline);
        });
        Assert.Contains("border-info", cut.Markup);
    }

    // ChevronClass (line 129)
    [Fact]
    public void Extended_Chevron_RendersChevronClass()
    {
        var cut = RenderSplitButton();
        Assert.Contains("h-3.5 w-3.5", cut.Markup);
    }

    // MenuPanelClass (line 132) — only rendered when Open = true
    [Fact]
    public void Extended_MenuPanel_RendersPanelClass()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.Open, true));
        Assert.Contains("min-w-56", cut.Markup);
    }

    // FullWidth (line 47)
    [Fact]
    public void Extended_FullWidth_RendersFullWidth()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.FullWidth, true));
        Assert.Contains("w-full", cut.Markup);
    }

    // Disabled (line 56)
    [Fact]
    public void Extended_Disabled_RendersDisabledState()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.Disabled, true));
        Assert.Contains("opacity-60", cut.Markup);
        Assert.Contains("pointer-events-none", cut.Markup);
    }

    // Type (line 37)
    [Fact]
    public void Extended_TypeSubmit_RendersSubmitType()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.Type, XFButtonType.Submit));
        Assert.Contains("submit", cut.Markup);
    }

    // Ghost variant (line 80)
    [Fact]
    public void Extended_VariantGhost_RendersGhost()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.Variant, XFButtonVariant.Ghost));
        Assert.Contains("border-transparent", cut.Markup);
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // Subtle variant (line 82)
    [Fact]
    public void Extended_VariantSubtle_RendersSubtle()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.Variant, XFButtonVariant.Subtle));
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    // SizeLg (line 63)
    [Fact]
    public void Extended_SizeLg_RendersLarge()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.Size, XFExtendedSize.Lg));
        Assert.Contains("h-11", cut.Markup);
    }

    // SizeXl (line 64)
    [Fact]
    public void Extended_SizeXl_RendersExtraLarge()
    {
        var cut = RenderSplitButton(p => p.Add(x => x.Size, XFExtendedSize.Xl));
        Assert.Contains("h-12", cut.Markup);
    }
}
