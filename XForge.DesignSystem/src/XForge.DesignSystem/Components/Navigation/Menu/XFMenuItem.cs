using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Navigation.Menu
{
    public sealed class XFMenuItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Badge { get; set; }
        public string? Meta { get; set; }
        public string? Href { get; set; }
        public string? Target { get; set; }
        public object? Value { get; set; }
        public XFMenuItemKind Kind { get; set; } = XFMenuItemKind.Item;
        public XFSemanticColor BadgeTone { get; set; } = XFSemanticColor.Neutral;
        public XFSemanticColor? ActiveTone { get; set; }
        public bool Disabled { get; set; }
        public bool Selected { get; set; }
        public bool Expanded { get; set; }
        public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
        public RenderFragment? Icon { get; set; }
        public RenderFragment<XFMenuItem>? Template { get; set; }
        public IReadOnlyList<XFMenuItem> Children { get; set; } = Array.Empty<XFMenuItem>();
    }
}
