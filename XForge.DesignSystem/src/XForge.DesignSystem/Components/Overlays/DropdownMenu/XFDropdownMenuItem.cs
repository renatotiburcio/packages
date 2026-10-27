using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Overlays.DropdownMenu
{
    public sealed class XFDropdownMenuItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Shortcut { get; set; }
        public string? Badge { get; set; }
        public object? Value { get; set; }
        public XFDropdownMenuItemKind Kind { get; set; } = XFDropdownMenuItemKind.Action;
        public XFSemanticColor Tone { get; set; } = XFSemanticColor.Neutral;
        public bool Disabled { get; set; }
        public bool Checked { get; set; }
        public string? Group { get; set; }
        public RenderFragment? Icon { get; set; }
        public IReadOnlyList<XFDropdownMenuItem> Children { get; set; } = Array.Empty<XFDropdownMenuItem>();
    }
}
