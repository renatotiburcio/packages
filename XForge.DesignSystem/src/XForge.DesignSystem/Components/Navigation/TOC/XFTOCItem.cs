namespace XForge.DesignSystem.Components.Navigation.TOC;

public sealed class XFTOCItem
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? Href { get; set; }
    public string? Icon { get; set; }
    public bool Disabled { get; set; }
    public IReadOnlyList<XFTOCItem> Children { get; set; } = Array.Empty<XFTOCItem>();
}
