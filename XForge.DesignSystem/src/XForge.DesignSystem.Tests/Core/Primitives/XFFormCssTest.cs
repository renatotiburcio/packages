using Xunit;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Core.Primitives;

/// <summary>
/// Tests for XFFormCss at 0% coverage — static Tailwind CSS class mapping methods.
/// Tests SizeClass, SizeTextClass, VariantClass, ToneClass, StateClass, ValidationToneClass,
/// and constant field values.
/// </summary>
public sealed class XFFormCssTest
{
    #region SizeClass

    [Theory]
    [InlineData(XFSize.Sm, "h-9 py-1.5")]
    [InlineData(XFSize.Lg, "h-12 py-2.5")]
    [InlineData(XFSize.Md, "h-10 py-2")]
    public void SizeClass_ReturnsCorrectClass(XFSize size, string expected)
    {
        Assert.Equal(expected, XFFormCss.SizeClass(size));
    }

    [Fact]
    public void SizeClass_UnknownValue_ReturnsDefault()
    {
        var result = XFFormCss.SizeClass((XFSize)99);
        Assert.Equal("h-10 py-2", result);
    }

    #endregion

    #region SizeTextClass

    [Theory]
    [InlineData(XFSize.Sm, "text-sm")]
    [InlineData(XFSize.Lg, "text-base")]
    [InlineData(XFSize.Md, "text-sm")]
    public void SizeTextClass_ReturnsCorrectClass(XFSize size, string expected)
    {
        Assert.Equal(expected, XFFormCss.SizeTextClass(size));
    }

    #endregion

    #region VariantClass

    [Fact]
    public void VariantClass_Outlined_ReturnsDefault()
    {
        var result = XFFormCss.VariantClass(XFFormVariant.Outline);
        Assert.Contains("border-surface-200", result);
        Assert.Contains("bg-bg-surface", result);
    }

    [Fact]
    public void VariantClass_Filled_ReturnsFilled()
    {
        var result = XFFormCss.VariantClass(XFFormVariant.Filled);
        Assert.Contains("border-transparent", result);
        Assert.Contains("bg-surface-100", result);
    }

    [Fact]
    public void VariantClass_Soft_ReturnsSoft()
    {
        var result = XFFormCss.VariantClass(XFFormVariant.Soft);
        Assert.Contains("var(--color-primary)", result);
    }

    [Fact]
    public void VariantClass_Ghost_ReturnsGhost()
    {
        var result = XFFormCss.VariantClass(XFFormVariant.Ghost);
        Assert.Contains("bg-transparent", result);
    }

    #endregion

    #region ToneClass

    [Fact]
    public void ToneClass_Primary_ContainsPrimaryColor()
    {
        var result = XFFormCss.ToneClass(XFSemanticColor.Primary);
        Assert.Contains("var(--color-primary)", result);
        Assert.Contains("focus-within:ring-2", result);
    }

    [Fact]
    public void ToneClass_Success_ContainsSuccessColor()
    {
        var result = XFFormCss.ToneClass(XFSemanticColor.Success);
        Assert.Contains("var(--color-success)", result);
    }

    [Fact]
    public void ToneClass_Danger_ContainsDangerColor()
    {
        var result = XFFormCss.ToneClass(XFSemanticColor.Danger);
        Assert.Contains("var(--color-danger)", result);
    }

    [Fact]
    public void ToneClass_Warning_ContainsWarningColor()
    {
        var result = XFFormCss.ToneClass(XFSemanticColor.Warning);
        Assert.Contains("var(--color-warning)", result);
    }

    [Fact]
    public void ToneClass_Secondary_ContainsSecondaryColor()
    {
        var result = XFFormCss.ToneClass(XFSemanticColor.Secondary);
        Assert.Contains("var(--color-secondary)", result);
    }

    [Fact]
    public void ToneClass_Default_ReturnsPrimaryTone()
    {
        var result = XFFormCss.ToneClass((XFSemanticColor)999);
        Assert.Contains("var(--color-primary)", result);
    }

    #endregion

    #region StateClass

    [Fact]
    public void StateClass_Disabled_ReturnsDisabledStyles()
    {
        var result = XFFormCss.StateClass(disabled: true);
        Assert.Contains("cursor-not-allowed", result);
        Assert.Contains("opacity-60", result);
    }

    [Fact]
    public void StateClass_Enabled_ReturnsHoverStyles()
    {
        var result = XFFormCss.StateClass(disabled: false);
        Assert.Contains("hover:border-", result);
        Assert.Contains("hover:ring-", result);
    }

    #endregion

    #region ValidationToneClass

    [Fact]
    public void ValidationToneClass_ContainsDangerColor()
    {
        var result = XFFormCss.ValidationToneClass();
        Assert.Contains("var(--color-danger)", result);
        Assert.Contains("ring-2", result);
    }

    #endregion

    #region Constants

    [Fact]
    public void FieldShellBase_ContainsRequiredClasses()
    {
        Assert.Contains("group", XFFormCss.FieldShellBase);
        Assert.Contains("rounded-lg", XFFormCss.FieldShellBase);
        Assert.Contains("border", XFFormCss.FieldShellBase);
    }

    [Fact]
    public void SelectFieldShellBase_ContainsCursorPointer()
    {
        Assert.Contains("cursor-pointer", XFFormCss.SelectFieldShellBase);
    }

    [Fact]
    public void LabelClass_ContainsTextStyles()
    {
        Assert.Contains("text-sm", XFFormCss.LabelClass);
        Assert.Contains("font-medium", XFFormCss.LabelClass);
    }

    [Fact]
    public void HelperClass_ContainsSmallText()
    {
        Assert.Contains("text-xs", XFFormCss.HelperClass);
    }

    [Fact]
    public void ErrorClass_ContainsDangerText()
    {
        Assert.Contains("text-danger", XFFormCss.ErrorClass);
    }

    [Fact]
    public void ClearButtonClass_ContainsTransition()
    {
        Assert.Contains("transition-all", XFFormCss.ClearButtonClass);
        Assert.Contains("hover:bg-", XFFormCss.ClearButtonClass);
    }

    [Fact]
    public void AdornmentClass_ContainsFlexAndShrink()
    {
        Assert.Contains("flex", XFFormCss.AdornmentClass);
        Assert.Contains("shrink-0", XFFormCss.AdornmentClass);
    }

    [Fact]
    public void SelectNativeBase_ContainsAppearanceNone()
    {
        Assert.Contains("appearance-none", XFFormCss.SelectNativeBase);
        Assert.Contains("cursor-pointer", XFFormCss.SelectNativeBase);
    }

    [Fact]
    public void OptionClass_ContainsCheckedStyles()
    {
        Assert.Contains("checked:bg-", XFFormCss.OptionClass);
    }

    #endregion
}
