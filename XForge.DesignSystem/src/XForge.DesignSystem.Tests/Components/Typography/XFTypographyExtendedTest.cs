using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Typography;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Typography;

public class XFTypographyExtendedTest : BunitContext
{
    public XFTypographyExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFTypography> RenderTypography(
        Action<ComponentParameterCollectionBuilder<XFTypography>>? configure = null)
    {
        return Render<XFTypography>(p =>
        {
            p.AddChildContent("Text");
            configure?.Invoke(p);
        });
    }

    // GetVariantClass Display (line 93)
    [Fact]
    public void Extended_VariantDisplay_RendersDisplay()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Display));
        Assert.Contains("text-4xl", cut.Markup);
        Assert.Contains("font-black", cut.Markup);
    }

    // GetVariantClass Hero (line 94)
    [Fact]
    public void Extended_VariantHero_RendersHero()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Hero));
        Assert.Contains("text-4xl", cut.Markup);
        Assert.Contains("font-bold", cut.Markup);
    }

    // GetVariantClass H1 (line 95)
    [Fact]
    public void Extended_VariantH1_RendersH1()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.H1));
        Assert.Contains("text-4xl", cut.Markup);
    }

    // GetVariantClass H2 (line 96)
    [Fact]
    public void Extended_VariantH2_RendersH2()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.H2));
        Assert.Contains("text-3xl", cut.Markup);
    }

    // GetVariantClass H3 (line 97)
    [Fact]
    public void Extended_VariantH3_RendersH3()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.H3));
        Assert.Contains("text-2xl", cut.Markup);
    }

    // GetVariantClass H4 (line 98)
    [Fact]
    public void Extended_VariantH4_RendersH4()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.H4));
        Assert.Contains("text-xl", cut.Markup);
    }

    // GetVariantClass H5 (line 99)
    [Fact]
    public void Extended_VariantH5_RendersH5()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.H5));
        Assert.Contains("text-lg", cut.Markup);
    }

    // GetVariantClass H6 (line 100)
    [Fact]
    public void Extended_VariantH6_RendersH6()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.H6));
        Assert.Contains("text-base", cut.Markup);
    }

    // GetVariantClass Subtitle1 (line 101)
    [Fact]
    public void Extended_VariantSubtitle1_RendersSubtitle1()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Subtitle1));
        Assert.Contains("text-lg", cut.Markup);
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    // GetVariantClass Subtitle2 (line 102)
    [Fact]
    public void Extended_VariantSubtitle2_RendersSubtitle2()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Subtitle2));
        Assert.Contains("text-base", cut.Markup);
    }

    // GetVariantClass Body2 (line 103)
    [Fact]
    public void Extended_VariantBody2_RendersBody2()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Body2));
        Assert.Contains("text-sm", cut.Markup);
    }

    // GetVariantClass Caption (line 104)
    [Fact]
    public void Extended_VariantCaption_RendersCaption()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Caption));
        Assert.Contains("text-xs", cut.Markup);
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // GetVariantClass Overline (line 105)
    [Fact]
    public void Extended_VariantOverline_RendersOverline()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Overline));
        Assert.Contains("uppercase", cut.Markup);
        Assert.Contains("tracking-wider", cut.Markup);
    }

    // GetVariantClass Code (line 106)
    [Fact]
    public void Extended_VariantCode_RendersCode()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Code));
        Assert.Contains("font-mono", cut.Markup);
    }

    // GetVariantClass Body1 default (line 107)
    [Fact]
    public void Extended_VariantBody1_RendersBody1()
    {
        var cut = RenderTypography();
        Assert.Contains("text-base", cut.Markup);
        Assert.Contains("text-text-primary", cut.Markup);
    }

    // GetColorClass Secondary (line 115)
    [Fact]
    public void Extended_ColorSecondary_RendersTextSecondary()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Secondary));
        Assert.Contains("text-text-secondary", cut.Markup);
    }

    // GetColorClass Muted (line 116)
    [Fact]
    public void Extended_ColorMuted_RendersTextMuted()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Muted));
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // GetColorClass Subtle (line 117)
    [Fact]
    public void Extended_ColorSubtle_RendersTextSubtle()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Subtle));
        Assert.Contains("text-text-subtle", cut.Markup);
    }

    // GetColorClass Inverse (line 118)
    [Fact]
    public void Extended_ColorInverse_RendersTextInverse()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Inverse));
        Assert.Contains("text-text-inverse", cut.Markup);
    }

    // GetColorClass Primary (line 119)
    [Fact]
    public void Extended_ColorPrimary_RendersTextPrimary()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Primary));
        Assert.Contains("text-primary", cut.Markup);
    }

    // GetColorClass Success (line 120)
    [Fact]
    public void Extended_ColorSuccess_RendersTextSuccess()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Success));
        Assert.Contains("text-success", cut.Markup);
    }

    // GetColorClass Warning (line 121)
    [Fact]
    public void Extended_ColorWarning_RendersTextWarning()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Warning));
        Assert.Contains("text-warning", cut.Markup);
    }

    // GetColorClass Danger (line 122)
    [Fact]
    public void Extended_ColorDanger_RendersTextDanger()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Danger));
        Assert.Contains("text-danger", cut.Markup);
    }

    // GetColorClass Default (line 123)
    [Fact]
    public void Extended_ColorDefault_NoExtraColor()
    {
        var cut = RenderTypography(p => p.Add(x => x.Color, XFTypographyColor.Default));
        // Default adds no extra color class (empty string)
        Assert.DoesNotContain("text-text-secondary", cut.Markup);
    }

    // GetWeightClass Normal (line 131)
    [Fact]
    public void Extended_WeightNormal_RendersFontNormal()
    {
        var cut = RenderTypography(p => p.Add(x => x.Weight, XFTypographyWeight.Normal));
        Assert.Contains("font-normal", cut.Markup);
    }

    // GetWeightClass Medium (line 132)
    [Fact]
    public void Extended_WeightMedium_RendersFontMedium()
    {
        var cut = RenderTypography(p => p.Add(x => x.Weight, XFTypographyWeight.Medium));
        Assert.Contains("font-medium", cut.Markup);
    }

    // GetWeightClass Bold (line 133)
    [Fact]
    public void Extended_WeightBold_RendersFontBold()
    {
        var cut = RenderTypography(p => p.Add(x => x.Weight, XFTypographyWeight.Bold));
        Assert.Contains("font-bold", cut.Markup);
    }

    // GetWeightClass Semibold (line 134)
    [Fact]
    public void Extended_WeightSemibold_RendersFontSemibold()
    {
        var cut = RenderTypography(p => p.Add(x => x.Weight, XFTypographyWeight.Semibold));
        Assert.Contains("font-semibold", cut.Markup);
    }

    // GetWeightClass Black (line 135)
    [Fact]
    public void Extended_WeightBlack_RendersFontBlack()
    {
        var cut = RenderTypography(p => p.Add(x => x.Weight, XFTypographyWeight.Black));
        Assert.Contains("font-black", cut.Markup);
    }

    // GetWeightClass Inherit (line 136)
    [Fact]
    public void Extended_WeightInherit_NoWeightClass()
    {
        var cut = RenderTypography(p => p.Add(x => x.Weight, XFTypographyWeight.Inherit));
        Assert.DoesNotContain("font-", cut.Markup);
    }

    // GetAlignClass Center (line 144)
    [Fact]
    public void Extended_AlignCenter_RendersTextCenter()
    {
        var cut = RenderTypography(p => p.Add(x => x.Align, XFTypographyAlign.Center));
        Assert.Contains("text-center", cut.Markup);
    }

    // GetAlignClass End (line 145)
    [Fact]
    public void Extended_AlignEnd_RendersTextEnd()
    {
        var cut = RenderTypography(p => p.Add(x => x.Align, XFTypographyAlign.End));
        Assert.Contains("text-end", cut.Markup);
    }

    // GetAlignClass Justify (line 146)
    [Fact]
    public void Extended_AlignJustify_RendersTextJustify()
    {
        var cut = RenderTypography(p => p.Add(x => x.Align, XFTypographyAlign.Justify));
        Assert.Contains("text-justify", cut.Markup);
    }

    // GetAlignClass Start (line 147)
    [Fact]
    public void Extended_AlignStart_RendersTextStart()
    {
        var cut = RenderTypography(p => p.Add(x => x.Align, XFTypographyAlign.Start));
        Assert.Contains("text-start", cut.Markup);
    }

    // GetTransformClass Uppercase (line 155)
    [Fact]
    public void Extended_TransformUppercase_RendersUppercase()
    {
        var cut = RenderTypography(p => p.Add(x => x.Transform, XFTypographyTransform.Uppercase));
        Assert.Contains("uppercase", cut.Markup);
    }

    // GetTransformClass Lowercase (line 156)
    [Fact]
    public void Extended_TransformLowercase_RendersLowercase()
    {
        var cut = RenderTypography(p => p.Add(x => x.Transform, XFTypographyTransform.Lowercase));
        Assert.Contains("lowercase", cut.Markup);
    }

    // GetTransformClass Capitalize (line 157)
    [Fact]
    public void Extended_TransformCapitalize_RendersCapitalize()
    {
        var cut = RenderTypography(p => p.Add(x => x.Transform, XFTypographyTransform.Capitalize));
        Assert.Contains("capitalize", cut.Markup);
    }

    // GetTransformClass None (line 158)
    [Fact]
    public void Extended_TransformNone_NoTransformClass()
    {
        var cut = RenderTypography(p => p.Add(x => x.Transform, XFTypographyTransform.None));
        Assert.DoesNotContain("uppercase", cut.Markup);
        Assert.DoesNotContain("lowercase", cut.Markup);
        Assert.DoesNotContain("capitalize", cut.Markup);
    }

    // GetFlowClass NoWrap (line 167)
    [Fact]
    public void Extended_NoWrap_RendersWhitespaceNowrap()
    {
        var cut = RenderTypography(p => p.Add(x => x.NoWrap, true));
        Assert.Contains("whitespace-nowrap", cut.Markup);
    }

    // GetFlowClass Truncate (line 170)
    [Fact]
    public void Extended_Truncate_RendersTruncate()
    {
        var cut = RenderTypography(p => p.Add(x => x.Truncate, true));
        Assert.Contains("truncate", cut.Markup);
    }

    // GetFlowClass GutterBottom (line 173)
    [Fact]
    public void Extended_GutterBottom_RendersMb2()
    {
        var cut = RenderTypography(p => p.Add(x => x.GutterBottom, true));
        Assert.Contains("mb-2", cut.Markup);
    }

    // GetTagName Display renders h1 (line 32)
    [Fact]
    public void Extended_TagNameDisplay_RendersH1()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Display));
        Assert.Contains("<h1", cut.Markup);
    }

    // GetTagName Caption renders span (line 42)
    [Fact]
    public void Extended_TagNameCaption_RendersSpan()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Caption));
        Assert.Contains("<span", cut.Markup);
    }

    // GetTagName Code renders code (line 44)
    [Fact]
    public void Extended_TagNameCode_RendersCode()
    {
        var cut = RenderTypography(p => p.Add(x => x.Variant, XFTypographyVariant.Code));
        Assert.Contains("<code", cut.Markup);
    }

    // GetTagName Body1 renders p (line 45)
    [Fact]
    public void Extended_TagNameBody1_RendersP()
    {
        var cut = RenderTypography();
        Assert.Contains("<p", cut.Markup);
    }

    // GetTagName with As override (line 26-27)
    [Fact]
    public void Extended_TagNameWithAs_UsesCustomTag()
    {
        var cut = RenderTypography(p => p.Add(x => x.As, "div"));
        Assert.Contains("<div", cut.Markup);
    }

    // BaseClass included (line 51)
    [Fact]
    public void Extended_BaseClass_Included()
    {
        var cut = RenderTypography();
        Assert.Contains("xf-typography", cut.Markup);
    }
}
