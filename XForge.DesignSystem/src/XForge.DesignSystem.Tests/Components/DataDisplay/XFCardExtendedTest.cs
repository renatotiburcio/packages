using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Card;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public class XFCardExtendedTest : BunitContext
{
    public XFCardExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFCard> RenderCard(
        Action<ComponentParameterCollectionBuilder<XFCard>>? configure = null)
    {
        return Render<XFCard>(p =>
        {
            p.AddChildContent("Card body");
            configure?.Invoke(p);
        });
    }

    // PaddingClass None (line 70)
    [Fact]
    public void Extended_PaddingNone_RendersP0()
    {
        var cut = RenderCard(p => p.Add(x => x.Padding, XFPadding.None));
        Assert.Contains("p-0", cut.Markup);
    }

    // PaddingClass Sm (line 71)
    [Fact]
    public void Extended_PaddingSm_RendersP3()
    {
        var cut = RenderCard(p => p.Add(x => x.Padding, XFPadding.Sm));
        Assert.Contains("p-3", cut.Markup);
    }

    // PaddingClass Lg (line 72)
    [Fact]
    public void Extended_PaddingLg_RendersP6()
    {
        var cut = RenderCard(p => p.Add(x => x.Padding, XFPadding.Lg));
        Assert.Contains("p-6", cut.Markup);
    }

    // PaddingClass Xl (line 73)
    [Fact]
    public void Extended_PaddingXl_RendersP8()
    {
        var cut = RenderCard(p => p.Add(x => x.Padding, XFPadding.Xl));
        Assert.Contains("p-8", cut.Markup);
    }

    // RadiusClass Sm (line 79)
    [Fact]
    public void Extended_RadiusSm_RendersRoundedMd()
    {
        var cut = RenderCard(p => p.Add(x => x.Radius, XFCardRadius.Sm));
        Assert.Contains("rounded-md", cut.Markup);
    }

    // RadiusClass Md (line 80)
    [Fact]
    public void Extended_RadiusMd_RendersRoundedLg()
    {
        var cut = RenderCard(p => p.Add(x => x.Radius, XFCardRadius.Md));
        Assert.Contains("rounded-lg", cut.Markup);
    }

    // RadiusClass Xl (line 81)
    [Fact]
    public void Extended_RadiusXl_RendersRoundedXl()
    {
        var cut = RenderCard(p => p.Add(x => x.Radius, XFCardRadius.Xl));
        Assert.Contains("rounded-xl", cut.Markup);
    }

    // VariantClass Outlined (line 87)
    [Fact]
    public void Extended_VariantOutlined_RendersOutlined()
    {
        var cut = RenderCard(p => p.Add(x => x.Variant, XFCardVariant.Outlined));
        Assert.Contains("border border-border-light", cut.Markup);
        Assert.Contains("bg-bg-surface", cut.Markup);
    }

    // VariantClass Filled (line 88)
    [Fact]
    public void Extended_VariantFilled_RendersFilled()
    {
        var cut = RenderCard(p => p.Add(x => x.Variant, XFCardVariant.Filled));
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    // VariantClass Flat (line 89)
    [Fact]
    public void Extended_VariantFlat_RendersFlat()
    {
        var cut = RenderCard(p => p.Add(x => x.Variant, XFCardVariant.Flat));
        Assert.Contains("border-transparent", cut.Markup);
        Assert.Contains("shadow-none", cut.Markup);
    }

    // VariantClass Ghost (line 90)
    [Fact]
    public void Extended_VariantGhost_RendersGhost()
    {
        var cut = RenderCard(p => p.Add(x => x.Variant, XFCardVariant.Ghost));
        Assert.Contains("bg-transparent", cut.Markup);
        Assert.Contains("shadow-none", cut.Markup);
    }

    // ToneClass Primary (line 102)
    [Fact]
    public void Extended_TonePrimary_RendersPrimaryAccent()
    {
        var cut = RenderCard(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.Contains("border-l-[var(--color-primary)]", cut.Markup);
    }

    // ToneClass Secondary (line 103)
    [Fact]
    public void Extended_ToneSecondary_RendersSecondaryAccent()
    {
        var cut = RenderCard(p => p.Add(x => x.Tone, XFSemanticColor.Secondary));
        Assert.Contains("border-l-[var(--color-secondary)]", cut.Markup);
    }

    // ToneClass Success (line 104)
    [Fact]
    public void Extended_ToneSuccess_RendersSuccessAccent()
    {
        var cut = RenderCard(p => p.Add(x => x.Tone, XFSemanticColor.Success));
        Assert.Contains("border-l-[var(--color-success)]", cut.Markup);
    }

    // ToneClass Warning (line 105)
    [Fact]
    public void Extended_ToneWarning_RendersWarningAccent()
    {
        var cut = RenderCard(p => p.Add(x => x.Tone, XFSemanticColor.Warning));
        Assert.Contains("border-l-[var(--color-warning)]", cut.Markup);
    }

    // ToneClass Danger (line 106)
    [Fact]
    public void Extended_ToneDanger_RendersDangerAccent()
    {
        var cut = RenderCard(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
        Assert.Contains("border-l-[var(--color-danger)]", cut.Markup);
    }

    // ToneClass Neutral (line 107)
    [Fact]
    public void Extended_ToneNeutral_RendersNeutralAccent()
    {
        var cut = RenderCard(p => p.Add(x => x.Tone, XFSemanticColor.Neutral));
        Assert.Contains("bg-surface-50", cut.Markup);
    }

    // Dividers false — header without border-bottom (line 52)
    [Fact]
    public void Extended_DividersFalse_HeaderWithoutBorder()
    {
        var cut = RenderCard(p =>
        {
            p.Add(x => x.Header, builder => builder.AddContent(0, "Header"));
            p.Add(x => x.Dividers, false);
        });
        // Header is the first div inside the article after the component class
        // When Dividers=false, GetHeaderClass() returns " p-5" (no border-b)
        var headerDiv = cut.Find("article > div");
        var headerClass = headerDiv.GetAttribute("class") ?? "";
        Assert.DoesNotContain("border-b", headerClass);
    }

    // Dividers true — header with border
    [Fact]
    public void Extended_DividersTrue_HeaderWithBorder()
    {
        var cut = RenderCard(p =>
        {
            p.Add(x => x.Header, builder => builder.AddContent(0, "Header"));
            p.Add(x => x.Dividers, true);
        });
        Assert.Contains("border-b", cut.Markup);
    }

    // Media section (line 46-48)
    [Fact]
    public void Extended_Media_RendersMediaSection()
    {
        var cut = RenderCard(p =>
        {
            p.Add(x => x.Media, builder => builder.AddContent(0, "<img src='test.jpg' />"));
        });
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // Actions section (line 62-66)
    [Fact]
    public void Extended_Actions_RendersActionsSection()
    {
        var cut = RenderCard(p =>
        {
            p.Add(x => x.Actions, builder => builder.AddContent(0, "Actions"));
        });
        Assert.Contains("flex flex-wrap items-center justify-end gap-2", cut.Markup);
    }

    // Footer section (line 56-60)
    [Fact]
    public void Extended_Footer_RendersFooterSection()
    {
        var cut = RenderCard(p =>
        {
            p.Add(x => x.Footer, builder => builder.AddContent(0, "Footer"));
        });
        Assert.Contains("Footer", cut.Markup);
    }

    // Interactive card
    [Fact]
    public void Extended_Interactive_RendersButtonRole()
    {
        var cut = RenderCard(p => p.Add(x => x.Interactive, true));
        Assert.Contains("role=\"button\"", cut.Markup);
        Assert.Contains("cursor-pointer", cut.Markup);
    }

    // AreaRestricted false
    [Fact]
    public void Extended_AreaRestrictedFalse_RendersOverflowVisible()
    {
        var cut = RenderCard(p => p.Add(x => x.AreaRestricted, false));
        Assert.Contains("overflow-visible", cut.Markup);
    }
}
