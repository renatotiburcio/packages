using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Button.FAB;

public sealed class XFFABMenuItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Text { get; set; } = string.Empty;
    public object? Value { get; set; }
    public bool Disabled { get; set; }
    public RenderFragment? Icon { get; set; }
}
