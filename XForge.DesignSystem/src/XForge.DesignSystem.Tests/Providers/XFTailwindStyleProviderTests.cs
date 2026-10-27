using Xunit;
using XForge.DesignSystem.Providers;

namespace XForge.DesignSystem.Tests.Providers;

public sealed class XFTailwindStyleProviderTests
{
    private readonly XFTailwindStyleProvider _provider = new();

    [Fact]
    public void ProviderName_ReturnsTailwind4()
    {
        Assert.Equal("Tailwind4", _provider.ProviderName);
    }

    [Fact]
    public void Token_SpacingSm_Returns0_5rem()
    {
        Assert.Equal("0.5rem", _provider.Token("spacing.sm"));
    }

    [Fact]
    public void Token_SpacingMd_Returns1rem()
    {
        Assert.Equal("1rem", _provider.Token("spacing.md"));
    }

    [Fact]
    public void Token_SpacingLg_Returns1_5rem()
    {
        Assert.Equal("1.5rem", _provider.Token("spacing.lg"));
    }

    [Fact]
    public void Token_ColorPrimary_ReturnsHex()
    {
        Assert.Equal("#2563eb", _provider.Token("color.primary"));
    }

    [Fact]
    public void Token_ColorDanger_ReturnsHex()
    {
        Assert.Equal("#dc2626", _provider.Token("color.danger"));
    }

    [Fact]
    public void Token_UnknownKey_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, _provider.Token("nonexistent.key"));
    }

    [Fact]
    public void Inline_ReturnsCssVar()
    {
        Assert.Equal("--color: red;", _provider.Inline("color", "red"));
    }

    [Fact]
    public void Inline_EmptyValue_ReturnsFormat()
    {
        Assert.Equal("--key: ;", _provider.Inline("key", string.Empty));
    }
}
