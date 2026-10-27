using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Navigation.Toolbar
{
    public sealed class XFToolbarItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string? Text { get; set; }
        public string? AriaLabel { get; set; }
        public string? Title { get; set; }
        public string? Value { get; set; }
        public string? Href { get; set; }
        public string? Target { get; set; }
        public bool Disabled { get; set; }
        public bool Active { get; set; }
        public XFToolbarItemKind Kind { get; set; } = XFToolbarItemKind.Action;
        public RenderFragment? Icon { get; set; }
        public RenderFragment<XFToolbarItem>? Template { get; set; }
        public IReadOnlyList<XFToolbarItem> Children { get; set; } = Array.Empty<XFToolbarItem>();
    }
}
