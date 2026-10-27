namespace XForge.DesignSystem.Shared.Ecosystem;

/// <summary>
/// Describes ownership and distribution intent for an XForge package.
/// </summary>
/// <param name="PackageId">NuGet package identifier.</param>
/// <param name="ProductLine">Product line that owns the package.</param>
/// <param name="IsCommercial">Whether the package is commercial-only.</param>
public sealed record XForgePackageBoundary(
    string PackageId,
    XForgeProductLine ProductLine,
    bool IsCommercial);
