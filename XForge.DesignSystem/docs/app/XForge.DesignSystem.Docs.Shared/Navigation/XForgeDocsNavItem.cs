namespace XForge.DesignSystem.Docs.Shared.Navigation;

/// <summary>
/// Navigation item shared by the core and AI documentation apps.
/// </summary>
/// <param name="Href">Route for the documentation page.</param>
/// <param name="Label">Display label.</param>
/// <param name="Group">Logical group used by sidebars and migration tracking.</param>
/// <param name="ComponentName">Component or topic represented by the page.</param>
public sealed record XForgeDocsNavItem(
    string Href,
    string Label,
    string Group,
    string ComponentName);
