using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Services;

namespace Sample.Web.Components.Layout;

/// <summary>
/// Itens do rail — mesmo modelo que o manual usa (<see cref="RailNavItem"/>).
/// O <see cref="RailNavItem.OverviewRoute"/> aponta para a primeira pagina de cada marca.
/// </summary>
public static class SampleNavigation
{
    public static IReadOnlyList<RailNavItem> RailItems { get; } = new[]
    {
        new RailNavItem
        {
            Type = RailNavItemType.Route,
            Brand = "clientes",
            Label = "Clientes",
            Icon = XFSvgIcons.Get("users"),
            OverviewRoute = "/clientes",
            RoutePrefixes = new[] { "/clientes" },
            IsDefault = true
        },
        new RailNavItem
        {
            Type = RailNavItemType.Route,
            Brand = "arquitetura",
            Label = "Arquitetura",
            Icon = XFSvgIcons.Get("layer-group"),
            OverviewRoute = "/arquitetura",
            RoutePrefixes = new[] { "/arquitetura" }
        }
    };
}
