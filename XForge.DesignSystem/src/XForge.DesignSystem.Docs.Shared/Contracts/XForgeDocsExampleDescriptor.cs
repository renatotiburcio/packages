namespace XForge.DesignSystem.Docs.Shared.Contracts;

/// <summary>
/// Metadata for a renderable documentation example.
/// </summary>
/// <param name="Id">Stable example identifier.</param>
/// <param name="Title">Human-readable title.</param>
/// <param name="Product">Documentation product that owns the example.</param>
/// <param name="ComponentName">Component demonstrated by the example.</param>
public sealed record XForgeDocsExampleDescriptor(
    string Id,
    string Title,
    XForgeDocsProduct Product,
    string ComponentName);
