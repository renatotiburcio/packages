using XForge.DesignSystem.Components.Navigation.Menu;

namespace XForge.DesignSystem.Docs.Shared.Navigation;

/// <summary>
/// Navigation section rendered inside the docs sidebar.
/// </summary>
public sealed record XForgeDocsSidebarSection(
    string Brand,
    string Title,
    IReadOnlyList<XFMenuItem> Items);
