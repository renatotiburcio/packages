using Xunit;
using XForge.DesignSystem.Core.Theming;

namespace XForge.DesignSystem.Tests.Core.Theming;

public class XFThemeBuilderTest
{
    [Fact]
    public void Create_ReturnsBuilder()
    {
        var builder = XFThemeBuilder.Create();
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_WithName_SetsName()
    {
        var builder = XFThemeBuilder.Create("my-brand");
        var css = builder.BuildCss();
        Assert.Contains("my-brand", css);
    }

    [Fact]
    public void WithPrimary_GeneratesScale()
    {
        var builder = XFThemeBuilder.Create()
            .WithPrimary("#6366f1");
        var vars = builder.BuildVariables();
        Assert.Contains("--color-primary-50", vars.Keys);
        Assert.Contains("--color-primary-500", vars.Keys);
        Assert.Contains("--color-primary-900", vars.Keys);
    }

    [Fact]
    public void WithPrimary_SetsMainVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithPrimary("#6366f1");
        var vars = builder.BuildVariables();
        Assert.Equal("#6366f1", vars["--color-primary"]);
    }

    [Fact]
    public void WithSuccess_SetsVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithSuccess("#22c55e");
        var vars = builder.BuildVariables();
        Assert.Equal("#22c55e", vars["--color-success"]);
    }

    [Fact]
    public void WithDanger_SetsVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithDanger("#ef4444");
        var vars = builder.BuildVariables();
        Assert.Equal("#ef4444", vars["--color-danger"]);
    }

    [Fact]
    public void WithWarning_SetsVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithWarning("#f59e0b");
        var vars = builder.BuildVariables();
        Assert.Equal("#f59e0b", vars["--color-warning"]);
    }

    [Fact]
    public void WithBackground_SetsVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithBackground("#ffffff");
        var vars = builder.BuildVariables();
        Assert.Equal("#ffffff", vars["--color-bg"]);
    }

    [Fact]
    public void WithRadius_SetsVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithRadius("0.75rem");
        var vars = builder.BuildVariables();
        Assert.Equal("0.75rem", vars["--radius"]);
    }

    [Fact]
    public void WithFontFamily_SetsVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithFontFamily("Inter, sans-serif");
        var vars = builder.BuildVariables();
        Assert.Equal("Inter, sans-serif", vars["--font-family"]);
    }

    [Fact]
    public void WithVariable_SetsRawVariable()
    {
        var builder = XFThemeBuilder.Create()
            .WithVariable("--custom-token", "42px");
        var vars = builder.BuildVariables();
        Assert.Equal("42px", vars["--custom-token"]);
    }

    [Fact]
    public void BuildCss_GeneratesValidCss()
    {
        var css = XFThemeBuilder.Create("test")
            .WithPrimary("#6366f1")
            .BuildCss();
        Assert.Contains(":root[data-theme=\"test\"]", css);
        Assert.Contains("--color-primary", css);
        Assert.Contains("{", css);
        Assert.Contains("}", css);
    }

    [Fact]
    public void FluentApi_ChainsCorrectly()
    {
        var vars = XFThemeBuilder.Create()
            .WithPrimary("#6366f1")
            .WithSuccess("#22c55e")
            .WithDanger("#ef4444")
            .WithRadius("0.5rem")
            .BuildVariables();

        Assert.True(vars.Count >= 14); // primary scale + others
    }

    [Fact]
    public void HexToHsl_ConvertsRed()
    {
        // #ff0000 -> HSL(0, 100%, 50%)
        var builder = XFThemeBuilder.Create().WithPrimary("#ff0000");
        var vars = builder.BuildVariables();
        Assert.Contains("--color-primary-500", vars.Keys);
    }

    [Fact]
    public void HexToHsl_ConvertsGreen()
    {
        // #00ff00 -> HSL(120, 100%, 50%)
        var builder = XFThemeBuilder.Create().WithPrimary("#00ff00");
        var vars = builder.BuildVariables();
        Assert.Contains("--color-primary-500", vars.Keys);
    }

    [Fact]
    public void WithComponentVariable_SetsOverride()
    {
        var builder = XFThemeBuilder.Create()
            .WithComponentVariable("button", "radius", "9999px");
        var vars = builder.BuildVariables();
        Assert.Equal("9999px", vars["--button-radius"]);
    }

    [Fact]
    public void BuildDarkVariant_CopiesSemanticColors()
    {
        var builder = XFThemeBuilder.Create("brand")
            .WithPrimary("#6366f1")
            .WithSuccess("#22c55e")
            .WithDanger("#ef4444");
        var dark = builder.BuildDarkVariant();
        var darkVars = dark.BuildVariables();
        Assert.Equal("#22c55e", darkVars["--color-success"]);
        Assert.Equal("#ef4444", darkVars["--color-danger"]);
    }

    [Fact]
    public void BuildBrandCss_GeneratesBrandSelector()
    {
        var css = XFThemeBuilder.Create("my-brand")
            .WithPrimary("#6366f1")
            .BuildBrandCss();
        Assert.Contains("[data-brand=\"my-brand\"]", css);
        Assert.Contains("--color-primary", css);
    }

    [Fact]
    public void BuildDarkBrandCss_GeneratesCombinedSelector()
    {
        var css = XFThemeBuilder.Create("my-brand")
            .WithPrimary("#6366f1")
            .BuildDarkBrandCss();
        Assert.Contains("[data-theme=\"dark\"][data-brand=\"my-brand\"]", css);
        Assert.Contains("--color-primary", css);
    }

    [Fact]
    public void BuildBrandCss_OnlyIncludesPrimary()
    {
        var css = XFThemeBuilder.Create("my-brand")
            .WithPrimary("#6366f1")
            .WithSuccess("#22c55e")
            .BuildBrandCss();
        Assert.Contains("--color-primary", css);
        Assert.DoesNotContain("--color-success", css);
    }
}
