namespace XForge.DesignSystem.Components.Forms.TransferList;

public sealed class XFTransferListItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Disabled { get; set; }
}
