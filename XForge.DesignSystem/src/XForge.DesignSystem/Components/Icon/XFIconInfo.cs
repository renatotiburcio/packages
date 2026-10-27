using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Immutable record describing a single icon entry in <see cref="XFIconCatalog"/>.
/// </summary>
/// <param name="Name">Stable icon name (Font Awesome identifier).</param>
/// <param name="Variant">Visual style variant.</param>
/// <param name="Size">Standard or custom size.</param>
/// <param name="Keywords">3-5 search keywords for picker UX.</param>
/// <param name="Category">Inferred category (navigation, communication, media, etc.).</param>
/// <param name="Content">Sanitized inline SVG (no comments, no external license traces).</param>
public sealed record XFIconInfo(
    string Name,
    XFIconVariant Variant,
    XFIconSize Size,
    IReadOnlyList<string> Keywords,
    string Category,
    string Content);