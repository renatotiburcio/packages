namespace XForge.DesignSystem.Components.Utilities.KeyCode;

public sealed class XFKeyCodeShortcut
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Key { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool Ctrl { get; set; }
    public bool Shift { get; set; }
    public bool Alt { get; set; }
    public bool Meta { get; set; }
    public bool Disabled { get; set; }
}
