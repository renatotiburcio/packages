namespace XForge.DesignSystem.Docs.Shared.Contracts;

/// <summary>
/// Runtime product context for the shared docs shell.
/// </summary>
public sealed class XForgeDocsProductContext
{
    public XForgeDocsProductContext(XForgeDocsProduct product)
    {
        Product = product;
    }

    public XForgeDocsProduct Product { get; }
}
