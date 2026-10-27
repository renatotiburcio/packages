using XForge.DesignSystem.DesignTokens;
using Xunit;

namespace XForge.DesignSystem.Tests;

public class FluentUIToTailwindTests
{
    [Fact]
    public void Accent_Maps_ToBlueTailwind()
    {
        Assert.Contains("text-blue-600", FluentUIToTailwindMap.ToTailwindColor("Accent"));
    }

    [Fact]
    public void Size24_Maps_ToW6H6()
    {
        Assert.Equal("w-6 h-6", FluentUIToTailwindMap.ToTailwindSize(24));
    }

    [Fact]
    public void UnknownColor_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, FluentUIToTailwindMap.ToTailwindColor("Mystery"));
    }

    [Fact]
    public void CustomSize_UsesArbitraryValue()
    {
        Assert.Contains("w-[", FluentUIToTailwindMap.ToTailwindSize(17));
    }
}