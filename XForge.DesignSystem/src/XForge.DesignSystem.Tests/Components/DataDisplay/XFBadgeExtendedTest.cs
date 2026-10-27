using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.DataDisplay.Badge;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFBadgeExtendedTest : BunitContext
{
    public XFBadgeExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFBadge> RenderBadge(
        Action<ComponentParameterCollectionBuilder<XFBadge>>? configure = null)
    {
        return Render<XFBadge>(p =>
        {
            p.Add(x => x.Text, "Badge");
            configure?.Invoke(p);
        });
    }

    // SizeClass Xs (line 48)
    [Fact]
    public void Extended_SizeXs_RendersExtraSmall()
    {
        var cut = RenderBadge(p => p.Add(x => x.Size, XFBadgeSize.Xs));
        Assert.Contains("min-h-5", cut.Markup);
        Assert.Contains("text-[0.6875rem]", cut.Markup);
    }

    // SizeClass Sm (line 49)
    [Fact]
    public void Extended_SizeSm_RendersSmall()
    {
        var cut = RenderBadge(p => p.Add(x => x.Size, XFBadgeSize.Sm));
        Assert.Contains("min-h-6", cut.Markup);
    }

    // SizeClass Lg (line 50)
    [Fact]
    public void Extended_SizeLg_RendersLarge()
    {
        var cut = RenderBadge(p => p.Add(x => x.Size, XFBadgeSize.Lg));
        Assert.Contains("min-h-8", cut.Markup);
        Assert.Contains("text-sm", cut.Markup);
    }

    // SizeClass default Md (line 51)
    [Fact]
    public void Extended_SizeDefault_RendersMedium()
    {
        var cut = RenderBadge();
        Assert.Contains("min-h-7", cut.Markup);
    }

    // Shape Circle (line 56)
    [Fact]
    public void Extended_ShapeCircle_RendersFullRound()
    {
        var cut = RenderBadge(p => p.Add(x => x.Shape, XFBadgeShape.Circle));
        Assert.Contains("rounded-full", cut.Markup);
        Assert.Contains("aspect-square", cut.Markup);
    }

    // Shape Pill (line 57)
    [Fact]
    public void Extended_ShapePill_RendersPill()
    {
        var cut = RenderBadge(p => p.Add(x => x.Shape, XFBadgeShape.Pill));
        Assert.Contains("rounded-full", cut.Markup);
    }

    // Shape Rounded with Rounded=true (line 58)
    [Fact]
    public void Extended_ShapeRounded_WithRoundedTrue()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Shape, XFBadgeShape.Rounded);
            p.Add(x => x.Rounded, true);
        });
        Assert.Contains("rounded-full", cut.Markup);
    }

    // Shape Rounded default (line 59)
    [Fact]
    public void Extended_ShapeRounded_Default()
    {
        var cut = RenderBadge(p => p.Add(x => x.Shape, XFBadgeShape.Rounded));
        Assert.Contains("rounded-md", cut.Markup);
    }

    // ToneClass Solid + Primary (line 84)
    [Fact]
    public void Extended_VariantSolid_Primary()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Solid);
            p.Add(x => x.Color, XFBadgeColor.Primary);
        });
        Assert.Contains("bg-primary", cut.Markup);
        Assert.Contains("text-primary-contrast", cut.Markup);
    }

    // ToneClass Solid + Danger (line 84/119)
    [Fact]
    public void Extended_VariantSolid_Danger()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Solid);
            p.Add(x => x.Color, XFBadgeColor.Danger);
        });
        Assert.Contains("bg-danger", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    // ToneClass Outline + Primary (line 85)
    [Fact]
    public void Extended_VariantOutline_Primary()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Outline);
            p.Add(x => x.Color, XFBadgeColor.Primary);
        });
        Assert.Contains("text-primary", cut.Markup);
        Assert.Contains("border-primary", cut.Markup);
    }

    // ToneClass Subtle + Info (line 86)
    [Fact]
    public void Extended_VariantSubtle_Info()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Subtle);
            p.Add(x => x.Color, XFBadgeColor.Info);
        });
        Assert.Contains("text-info", cut.Markup);
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    // ToneClass Ghost + Warning (line 87)
    [Fact]
    public void Extended_VariantGhost_Warning()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Ghost);
            p.Add(x => x.Color, XFBadgeColor.Warning);
        });
        Assert.Contains("text-warning", cut.Markup);
        Assert.Contains("border-transparent", cut.Markup);
    }

    // ToneClass Soft (default) + Secondary (line 88/127)
    [Fact]
    public void Extended_VariantSoft_Secondary()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Soft);
            p.Add(x => x.Color, XFBadgeColor.Secondary);
        });
        Assert.Contains("text-secondary", cut.Markup);
    }

    // ToneClass Solid + Neutral (line 84/121)
    [Fact]
    public void Extended_VariantSolid_Neutral()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Solid);
            p.Add(x => x.Color, XFBadgeColor.Neutral);
        });
        Assert.Contains("bg-surface-950", cut.Markup);
        Assert.Contains("text-white", cut.Markup);
    }

    // ToneClass Outline + Neutral (line 85/110)
    [Fact]
    public void Extended_VariantOutline_Neutral()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Outline);
            p.Add(x => x.Color, XFBadgeColor.Neutral);
        });
        Assert.Contains("text-text-secondary", cut.Markup);
        Assert.Contains("border-border", cut.Markup);
    }

    // Interactive class when OnClick.HasDelegate (line 62-63)
    [Fact]
    public void Extended_OnClick_HasDelegate_RendersInteractive()
    {
        var cut = RenderBadge(p => p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));
        Assert.Contains("cursor-pointer", cut.Markup);
        Assert.Contains("hover:ring-2", cut.Markup);
    }

    // Dot renders dot span (line 9)
    [Fact]
    public void Extended_Dot_RendersDotSpan()
    {
        var cut = RenderBadge(p => p.Add(x => x.Dot, true));
        Assert.Contains("h-2", cut.Markup);
        Assert.Contains("rounded-full", cut.Markup);
    }

    // DotClass Sm size (line 76)
    [Fact]
    public void Extended_DotSizeSm()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Size, XFBadgeSize.Sm);
            p.Add(x => x.Dot, true);
        });
        Assert.Contains("h-1.5", cut.Markup);
    }

    // Dismiss button when OnDismiss.HasDelegate (line 33)
    [Fact]
    public void Extended_OnDismiss_HasDelegate_RendersDismissButton()
    {
        var cut = RenderBadge(p => p.Add(x => x.OnDismiss, EventCallback.Factory.Create<MouseEventArgs>(this, _ => { })));
        var button = cut.Find("button");
        Assert.NotNull(button);
    }

    // IconClass Xs/Sm (line 72)
    [Fact]
    public void Extended_IconSizeXs()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Size, XFBadgeSize.Xs);
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("h-3.5", cut.Markup);
    }

    // IconClass Md/Lg (line 73)
    [Fact]
    public void Extended_IconSizeMd()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Size, XFBadgeSize.Md);
            p.Add(x => x.StartIcon, builder => builder.AddContent(0, "icon"));
        });
        Assert.Contains("h-4", cut.Markup);
    }

    // ToneDotClass Primary (line 137)
    [Fact]
    public void Extended_ToneDot_Primary()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Color, XFBadgeColor.Primary);
            p.Add(x => x.Dot, true);
        });
        Assert.Contains("bg-primary", cut.Markup);
    }

    // ToneDotClass Neutral (line 143)
    [Fact]
    public void Extended_ToneDot_Neutral()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Color, XFBadgeColor.Neutral);
            p.Add(x => x.Dot, true);
        });
        Assert.Contains("bg-surface-500", cut.Markup);
    }

    // ToneTextClass Success (line 95)
    [Fact]
    public void Extended_ToneText_Success()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Outline);
            p.Add(x => x.Color, XFBadgeColor.Success);
        });
        Assert.Contains("text-success", cut.Markup);
    }

    // ToneBorderClass Warning (line 107)
    [Fact]
    public void Extended_ToneBorder_Warning()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Outline);
            p.Add(x => x.Color, XFBadgeColor.Warning);
        });
        Assert.Contains("border-warning", cut.Markup);
    }

    // ToneSoftClass Danger (line 130)
    [Fact]
    public void Extended_ToneSoft_Danger()
    {
        var cut = RenderBadge(p =>
        {
            p.Add(x => x.Variant, XFBadgeVariant.Soft);
            p.Add(x => x.Color, XFBadgeColor.Danger);
        });
        Assert.Contains("text-danger", cut.Markup);
    }
}
