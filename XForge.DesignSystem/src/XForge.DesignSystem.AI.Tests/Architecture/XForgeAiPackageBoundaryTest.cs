using XForge.DesignSystem.AI;
using XForge.DesignSystem.Shared.Ecosystem;
using Xunit;

namespace XForge.DesignSystem.AI.Tests.Architecture;

public sealed class XForgeAiPackageBoundaryTest
{
    [Fact]
    public void AiAssemblyBoundary_ShouldBeCommercialAiProductLine()
    {
        var boundary = XForgeDesignSystemAiAssembly.Boundary;

        Assert.Equal("XForge.DesignSystem.AI", boundary.PackageId);
        Assert.Equal(XForgeProductLine.AI, boundary.ProductLine);
        Assert.True(boundary.IsCommercial);
    }
}
