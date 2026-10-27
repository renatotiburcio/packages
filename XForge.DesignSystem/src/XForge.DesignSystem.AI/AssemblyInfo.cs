using XForge.DesignSystem.Shared.Ecosystem;

namespace XForge.DesignSystem.AI;

/// <summary>
/// Assembly-level metadata for the commercial AI package boundary.
/// </summary>
public static class XForgeDesignSystemAiAssembly
{
    /// <summary>
    /// Package boundary declaration used by architecture tests and docs.
    /// </summary>
    public static XForgePackageBoundary Boundary { get; } = new(
        "XForge.DesignSystem.AI",
        XForgeProductLine.AI,
        IsCommercial: true);
}
