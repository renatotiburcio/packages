using Microsoft.Extensions.Options;
using Xunit;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Tests.Services;

public class BrandMetadataServiceTest
{
    [Fact]
    public void GetBrand_UsesDefaultBrands()
    {
        var service = new BrandMetadataService();

        Assert.True(service.IsBrandKnown("forms"));
        Assert.Equal("Forms", service.GetBrandTitle("forms"));
        Assert.Equal("#ec4899", service.GetBrandColor("forms"));
    }

    [Fact]
    public void GetBrand_WithOptions_OverridesDefaultBrand()
    {
        var service = new BrandMetadataService(Options.Create(new BrandMetadataOptions
        {
            Brands =
            {
                ["forms"] = new BrandMetadata("Formularios", "#111827", "<svg />", "#ffffff")
            }
        }));

        Assert.Equal("Formularios", service.GetBrandTitle("forms"));
        Assert.Equal("#111827", service.GetBrandColor("forms"));
        Assert.Equal("#ffffff", service.GetBrandContrastColor("forms"));
    }

    [Fact]
    public void GetBrand_WithReplaceDefaults_UsesOnlyConfiguredBrands()
    {
        var service = new BrandMetadataService(Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands =
            {
                ["tenant"] = new BrandMetadata("Tenant", "#f59e0b", "")
            }
        }));

        Assert.True(service.IsBrandKnown("tenant"));
        Assert.False(service.IsBrandKnown("forms"));
    }

    [Theory]
    [InlineData("light", "#f59e0b", "#0f172a")]
    [InlineData("dark", "#111827", "#ffffff")]
    public void GetBrandContrastColor_WhenNotProvided_ComputesReadableContrast(string key, string color, string expected)
    {
        var service = new BrandMetadataService(Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands =
            {
                [key] = new BrandMetadata(key, color, "")
            }
        }));

        Assert.Equal(expected, service.GetBrandContrastColor(key));
    }

    [Fact]
    public void GetAllBrandMetadata_DoesNotExposeMutableDictionary()
    {
        var service = new BrandMetadataService();

        var metadata = service.GetAllBrandMetadata();

        Assert.False(metadata is Dictionary<string, BrandMetadata>);
        Assert.True(metadata.ContainsKey("forms"));
    }

#pragma warning disable CS0618 // Type or member is obsolete
    [Fact]
    public void BuildBrandCssVariables_GeneratesRuntimePrimaryScale()
    {
        var service = new BrandMetadataService(Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands =
            {
                ["tenant"] = new BrandMetadata("Tenant", "#0f766e", "")
            }
        }));

        var css = service.BuildBrandCssVariables("tenant", ":root[data-brand=\"tenant\"]");

        Assert.Contains(":root[data-brand=\"tenant\"]", css);
        Assert.Contains("--color-primary-50:", css);
        Assert.Contains("--color-primary-950:", css);
        Assert.Contains("--color-primary: var(--color-primary-500);", css);
        Assert.Contains("--color-primary-contrast: #ffffff;", css);
    }

    [Fact]
    public void BuildAllBrandCssVariables_GeneratesCssForConfiguredBrands()
    {
        var service = new BrandMetadataService(Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands =
            {
                ["tenant"] = new BrandMetadata("Tenant", "#0f766e", ""),
                ["billing"] = new BrandMetadata("Billing", "#ec4899", "")
            }
        }));

        var css = service.BuildAllBrandCssVariables(brand => $".brand-{brand}");

        Assert.Contains(".brand-tenant", css);
        Assert.Contains(".brand-billing", css);
        Assert.Equal(2, CountOccurrences(css, "--color-primary: var(--color-primary-500);"));
    }

    [Fact]
    public void BuildAllBrandCssVariables_DefaultSelectorEscapesAttributeValue()
    {
        var service = new BrandMetadataService(Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands =
            {
                ["tenant\"admin"] = new BrandMetadata("Tenant", "#0f766e", "")
            }
        }));

        var css = service.BuildAllBrandCssVariables();

        Assert.Contains(":root[data-brand=\"tenant\\\"admin\"]", css);
    }

#pragma warning restore CS0618 // Type or member is obsolete

    #region Additional Coverage — GetBrand, GetAllBrands, TryParseHex

    [Fact]
    public void GetBrand_NullBrand_ReturnsNull()
    {
        var service = new BrandMetadataService();
        Assert.Null(service.GetBrand(null!));
    }

    [Fact]
    public void GetBrand_EmptyBrand_ReturnsNull()
    {
        var service = new BrandMetadataService();
        Assert.Null(service.GetBrand(""));
    }

    [Fact]
    public void GetBrand_UnknownBrand_ReturnsNull()
    {
        var service = new BrandMetadataService();
        Assert.Null(service.GetBrand("nonexistent"));
    }

    [Fact]
    public void GetBrandTitle_UnknownBrand_CapitalizesFirstLetter()
    {
        var service = new BrandMetadataService();
        Assert.Equal("Custom", service.GetBrandTitle("custom"));
    }

    [Fact]
    public void GetBrandTitle_EmptyBrand_UsesDefaultTitle()
    {
        var service = new BrandMetadataService();
        Assert.Equal("XForge Docs", service.GetBrandTitle(""));
    }

    [Fact]
    public void GetBrandTitle_HomeBrand_UsesDefaultTitle()
    {
        var service = new BrandMetadataService();
        Assert.Equal("XForge Docs", service.GetBrandTitle("home"));
    }

    [Fact]
    public void GetBrandColor_UnknownBrand_UsesDefaultColor()
    {
        var service = new BrandMetadataService();
        Assert.Equal("#6366f1", service.GetBrandColor("nonexistent"));
    }

    [Fact]
    public void GetBrandIconSvg_UnknownBrand_UsesDefaultIcon()
    {
        var service = new BrandMetadataService();
        Assert.Equal(string.Empty, service.GetBrandIconSvg("nonexistent"));
    }

    [Fact]
    public void GetAllBrands_ReturnsAllDefaultBrands()
    {
        var service = new BrandMetadataService();
        var brands = service.GetAllBrands();
        Assert.Contains("components", brands);
        Assert.Contains("forms", brands);
        Assert.Contains("display", brands);
        Assert.Contains("feedback", brands);
        Assert.Contains("layout", brands);
        Assert.Contains("tokens", brands);
        Assert.Contains("home", brands);
    }

    [Fact]
    public void IsBrandKnown_EmptyBrand_ReturnsFalse()
    {
        var service = new BrandMetadataService();
        Assert.False(service.IsBrandKnown(""));
    }

    [Fact]
    public void IsBrandKnown_NullBrand_ReturnsFalse()
    {
        var service = new BrandMetadataService();
        Assert.False(service.IsBrandKnown(null!));
    }

    [Fact]
    public void BrandMetadata_Constructor_WithAllParams_SetsProperties()
    {
        var metadata = new BrandMetadata("Title", "#ff0000", "<svg>icon</svg>", "#000000");
        Assert.Equal("Title", metadata.Title);
        Assert.Equal("#ff0000", metadata.Color);
        Assert.Equal("<svg>icon</svg>", metadata.IconSvg);
        Assert.Equal("#000000", metadata.ContrastColor);
    }

    [Fact]
    public void BrandMetadata_DefaultConstructor_HasDefaultValues()
    {
        var metadata = new BrandMetadata();
        Assert.Equal(string.Empty, metadata.Title);
        Assert.Equal("#6366f1", metadata.Color);
        Assert.Equal(string.Empty, metadata.IconSvg);
        Assert.Null(metadata.ContrastColor);
    }

    [Fact]
    public void BrandMetadataOptions_DefaultReplaceDefaults_IsFalse()
    {
        var options = new BrandMetadataOptions();
        Assert.False(options.ReplaceDefaults);
    }

    [Fact]
    public void BrandMetadataService_WithEmptyKeyBrand_IsSkipped()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            Brands = { [""] = new BrandMetadata("Empty", "#000", "") }
        }));
        Assert.False(service.IsBrandKnown(""));
    }

    [Fact]
    public void BrandMetadataService_WithWhitespaceKeyBrand_IsSkipped()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            Brands = { ["   "] = new BrandMetadata("Whitespace", "#000", "") }
        }));
        Assert.False(service.IsBrandKnown("   "));
    }

    #endregion

    #region BuildAllBrandThemeCssRules — Dark/Light Theme CSS

#pragma warning disable CS0618 // Type or member is obsolete
    [Fact]
    public void BuildAllBrandThemeCssRules_GeneratesLightAndDarkRules()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands = { ["forms"] = new BrandMetadata("Forms", "#ec4899", "") }
        }));

        var css = service.BuildAllBrandThemeCssRules();

        Assert.Contains(":root[data-brand=\"brand-forms\"]", css);
        Assert.Contains(":root[data-theme=\"dark\"][data-brand=\"brand-forms\"]", css);
        Assert.Contains("--color-primary-500", css);
        Assert.Contains("--color-primary: var(--color-primary-500)", css);
        Assert.Contains("--color-primary-contrast", css);
    }

    [Fact]
    public void BuildAllBrandThemeCssRules_DarkTheme_UsesLighterPrimaryStep()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands = { ["forms"] = new BrandMetadata("Forms", "#ec4899", "") }
        }));

        var css = service.BuildAllBrandThemeCssRules();

        // Dark theme uses primary-400 instead of 500
        Assert.Contains("--color-primary: var(--color-primary-400)", css);
        Assert.Contains("--color-primary-hover: var(--color-primary-300)", css);
        Assert.Contains("--color-primary-active: var(--color-primary-500)", css);
    }

    [Fact]
    public void BuildAllBrandThemeCssRules_InvalidHex_UsesFallbackColor()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands = { ["test"] = new BrandMetadata("Test", "invalid-color", "") }
        }));

        var css = service.BuildAllBrandThemeCssRules();

        // Should not throw and should still produce CSS with fallback color
        Assert.Contains("brand-test", css);
        Assert.Contains("--color-primary-500", css);
    }
#pragma warning restore CS0618 // Type or member is obsolete

    #endregion

    #region BuildBrandCssVariables — Edge Cases

#pragma warning disable CS0618 // Type or member is obsolete
    [Fact]
    public void BuildBrandCssVariables_EmptySelector_Throws()
    {
        var service = new BrandMetadataService();
        Assert.Throws<ArgumentException>(() => service.BuildBrandCssVariables("forms", ""));
    }

    [Fact]
    public void BuildBrandCssVariables_WhitespaceSelector_Throws()
    {
        var service = new BrandMetadataService();
        Assert.Throws<ArgumentException>(() => service.BuildBrandCssVariables("forms", "   "));
    }

    [Fact]
    public void BuildAllBrandCssVariables_NullSelectorFactory_SkipsEmptySelectors()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands = { ["forms"] = new BrandMetadata("Forms", "#ec4899", "") }
        }));

        var css = service.BuildAllBrandCssVariables(brand => string.IsNullOrWhiteSpace(brand) ? null : $".{brand}");

        Assert.Contains(".forms", css);
    }
#pragma warning restore CS0618 // Type or member is obsolete

    #endregion

    #region Color Scale — TryParseHex Edge Cases

    [Fact]
    public void GetBrandContrastColor_DarkColor_ReturnsWhiteContrast()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands = { ["dark"] = new BrandMetadata("Dark", "#111827", "") }
        }));
        // #111827 is dark => contrast should be white
        Assert.Equal("#ffffff", service.GetBrandContrastColor("dark"));
    }

    [Fact]
    public void GetBrandContrastColor_LightColor_ReturnsDarkContrast()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands = { ["light"] = new BrandMetadata("Light", "#f59e0b", "") }
        }));
        // #f59e0b is light => contrast should be dark
        Assert.Equal("#0f172a", service.GetBrandContrastColor("light"));
    }

    [Fact]
    public void GetBrandContrastColor_3DigitHex_ParsesCorrectly()
    {
        var service = new BrandMetadataService(Microsoft.Extensions.Options.Options.Create(new BrandMetadataOptions
        {
            ReplaceDefaults = true,
            Brands = { ["short"] = new BrandMetadata("Short", "#fff", "") }
        }));
        // #fff => white => dark text
        Assert.Equal("#0f172a", service.GetBrandContrastColor("short"));
    }

    #endregion

    private static int CountOccurrences(string value, string pattern)
    {
        var count = 0;
        var index = 0;
        while ((index = value.IndexOf(pattern, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += pattern.Length;
        }

        return count;
    }
}
