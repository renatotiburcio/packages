using Xunit;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Core.Primitives;

public class XFFormCssTests
{
    [Fact]
    public void SizeClass_Sm_ReturnsExpected() =>
        Assert.Equal("h-9 py-1.5", XFFormCss.SizeClass(XFSize.Sm));

    [Fact]
    public void SizeClass_Lg_ReturnsExpected() =>
        Assert.Equal("h-12 py-2.5", XFFormCss.SizeClass(XFSize.Lg));

    [Fact]
    public void SizeClass_Default_ReturnsMd() =>
        Assert.Equal("h-10 py-2", XFFormCss.SizeClass(XFSize.Md));

    [Fact]
    public void SizeTextClass_Sm_ReturnsExpected() =>
        Assert.Equal("text-sm", XFFormCss.SizeTextClass(XFSize.Sm));

    [Fact]
    public void SizeTextClass_Lg_ReturnsExpected() =>
        Assert.Equal("text-base", XFFormCss.SizeTextClass(XFSize.Lg));

    [Fact]
    public void SizeTextClass_Default_ReturnsSm() =>
        Assert.Equal("text-sm", XFFormCss.SizeTextClass(XFSize.Md));

    [Fact]
    public void VariantClass_Filled_ReturnsExpected()
    {
        var css = XFFormCss.VariantClass(XFFormVariant.Filled);
        Assert.Contains("border-transparent", css);
        Assert.Contains("bg-surface-100", css);
    }

    [Fact]
    public void VariantClass_Soft_ReturnsExpected()
    {
        var css = XFFormCss.VariantClass(XFFormVariant.Soft);
        Assert.Contains("color-mix", css);
        Assert.Contains("var(--color-primary)", css);
    }

    [Fact]
    public void VariantClass_Ghost_ReturnsExpected()
    {
        var css = XFFormCss.VariantClass(XFFormVariant.Ghost);
        Assert.Contains("bg-transparent", css);
        Assert.Contains("ring-transparent", css);
    }

    [Fact]
    public void VariantClass_Default_ReturnsOutlined()
    {
        var css = XFFormCss.VariantClass(XFFormVariant.Outline);
        Assert.Contains("border-surface-200", css);
        Assert.Contains("bg-bg-surface", css);
    }

    [Fact]
    public void ValidationToneClass_ContainsDangerTokens()
    {
        var css = XFFormCss.ValidationToneClass();
        Assert.Contains("var(--color-danger)", css);
        Assert.Contains("ring-2", css);
    }

    [Fact]
    public void ToneClass_Primary_ContainsPrimary()
    {
        var css = XFFormCss.ToneClass(XFSemanticColor.Primary);
        Assert.Contains("var(--color-primary)", css);
        Assert.Contains("ring-2", css);
    }

    [Fact]
    public void ToneClass_Secondary_ContainsSecondary()
    {
        var css = XFFormCss.ToneClass(XFSemanticColor.Secondary);
        Assert.Contains("var(--color-secondary)", css);
    }

    [Fact]
    public void ToneClass_Success_ContainsSuccess()
    {
        var css = XFFormCss.ToneClass(XFSemanticColor.Success);
        Assert.Contains("var(--color-success)", css);
    }

    [Fact]
    public void ToneClass_Warning_ContainsWarning()
    {
        var css = XFFormCss.ToneClass(XFSemanticColor.Warning);
        Assert.Contains("var(--color-warning)", css);
    }

    [Fact]
    public void ToneClass_Danger_ContainsDanger()
    {
        var css = XFFormCss.ToneClass(XFSemanticColor.Danger);
        Assert.Contains("var(--color-danger)", css);
    }

    [Fact]
    public void ToneClass_Default_ReturnsPrimary()
    {
        var cssDefault = XFFormCss.ToneClass(XFSemanticColor.Default);
        var cssPrimary = XFFormCss.ToneClass(XFSemanticColor.Primary);
        Assert.Equal(cssPrimary, cssDefault);
    }

    [Fact]
    public void ToneClass_Info_ReturnsPrimary()
    {
        var cssInfo = XFFormCss.ToneClass(XFSemanticColor.Info);
        var cssPrimary = XFFormCss.ToneClass(XFSemanticColor.Primary);
        Assert.Equal(cssPrimary, cssInfo);
    }

    [Fact]
    public void ToneClass_Neutral_ReturnsPrimary()
    {
        var cssNeutral = XFFormCss.ToneClass(XFSemanticColor.Neutral);
        var cssPrimary = XFFormCss.ToneClass(XFSemanticColor.Primary);
        Assert.Equal(cssPrimary, cssNeutral);
    }

    [Fact]
    public void StateClass_Disabled_ReturnsExpected()
    {
        var css = XFFormCss.StateClass(true);
        Assert.Contains("cursor-not-allowed", css);
        Assert.Contains("opacity-60", css);
    }

    [Fact]
    public void StateClass_Enabled_ContainsHoverTokens()
    {
        var css = XFFormCss.StateClass(false);
        Assert.Contains("hover:border-", css);
        Assert.Contains("var(--color-primary)", css);
    }

    [Fact]
    public void FieldShellBase_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.FieldShellBase);
        Assert.NotEmpty(XFFormCss.FieldShellBase);
    }

    [Fact]
    public void FieldShellBase_ContainsExpectedTokens()
    {
        Assert.Contains("rounded-lg", XFFormCss.FieldShellBase);
        Assert.Contains("border", XFFormCss.FieldShellBase);
        Assert.Contains("transition-all", XFFormCss.FieldShellBase);
    }

    [Fact]
    public void SelectFieldShellBase_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.SelectFieldShellBase);
        Assert.NotEmpty(XFFormCss.SelectFieldShellBase);
    }

    [Fact]
    public void SelectFieldShellBase_ContainsExpectedTokens()
    {
        Assert.Contains("cursor-pointer", XFFormCss.SelectFieldShellBase);
        Assert.Contains("rounded-lg", XFFormCss.SelectFieldShellBase);
        Assert.Contains("border", XFFormCss.SelectFieldShellBase);
    }

    [Fact]
    public void LabelClass_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.LabelClass);
        Assert.NotEmpty(XFFormCss.LabelClass);
    }

    [Fact]
    public void LabelClass_ContainsExpectedTokens()
    {
        Assert.Contains("text-sm", XFFormCss.LabelClass);
        Assert.Contains("font-medium", XFFormCss.LabelClass);
    }

    [Fact]
    public void HelperClass_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.HelperClass);
        Assert.NotEmpty(XFFormCss.HelperClass);
    }

    [Fact]
    public void HelperClass_ContainsExpectedTokens()
    {
        Assert.Contains("text-xs", XFFormCss.HelperClass);
        Assert.Contains("text-text-secondary", XFFormCss.HelperClass);
    }

    [Fact]
    public void ErrorClass_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.ErrorClass);
        Assert.NotEmpty(XFFormCss.ErrorClass);
    }

    [Fact]
    public void ErrorClass_ContainsExpectedTokens()
    {
        Assert.Contains("text-xs", XFFormCss.ErrorClass);
        Assert.Contains("text-danger", XFFormCss.ErrorClass);
        Assert.Contains("font-medium", XFFormCss.ErrorClass);
    }

    [Fact]
    public void ClearButtonClass_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.ClearButtonClass);
        Assert.NotEmpty(XFFormCss.ClearButtonClass);
    }

    [Fact]
    public void ClearButtonClass_ContainsExpectedTokens()
    {
        Assert.Contains("rounded-md", XFFormCss.ClearButtonClass);
        Assert.Contains("bg-transparent", XFFormCss.ClearButtonClass);
        Assert.Contains("focus-visible:ring-2", XFFormCss.ClearButtonClass);
    }

    [Fact]
    public void AdornmentClass_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.AdornmentClass);
        Assert.NotEmpty(XFFormCss.AdornmentClass);
    }

    [Fact]
    public void AdornmentClass_ContainsExpectedTokens()
    {
        Assert.Contains("text-text-muted", XFFormCss.AdornmentClass);
        Assert.Contains("group-focus-within:text-primary", XFFormCss.AdornmentClass);
    }

    [Fact]
    public void SelectNativeBase_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.SelectNativeBase);
        Assert.NotEmpty(XFFormCss.SelectNativeBase);
    }

    [Fact]
    public void SelectNativeBase_ContainsExpectedTokens()
    {
        Assert.Contains("cursor-pointer", XFFormCss.SelectNativeBase);
        Assert.Contains("appearance-none", XFFormCss.SelectNativeBase);
        Assert.Contains("outline-none", XFFormCss.SelectNativeBase);
    }

    [Fact]
    public void OptionClass_IsNotNullOrEmpty()
    {
        Assert.NotNull(XFFormCss.OptionClass);
        Assert.NotEmpty(XFFormCss.OptionClass);
    }

    [Fact]
    public void OptionClass_ContainsExpectedTokens()
    {
        Assert.Contains("bg-bg-surface", XFFormCss.OptionClass);
        Assert.Contains("text-text-primary", XFFormCss.OptionClass);
        Assert.Contains("checked:bg-[", XFFormCss.OptionClass);
    }
}
