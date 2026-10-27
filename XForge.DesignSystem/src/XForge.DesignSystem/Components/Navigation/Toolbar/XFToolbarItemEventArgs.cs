namespace XForge.DesignSystem.Components.Navigation.Toolbar
{
    public sealed record XFToolbarItemEventArgs(
        XFToolbarItem Item,
        string Id,
        string? Value,
        string? Href);
}
