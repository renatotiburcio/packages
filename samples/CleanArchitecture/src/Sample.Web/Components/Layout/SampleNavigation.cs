using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Components.Navigation.Menu;
using XForge.DesignSystem.Services;

namespace Sample.Web.Components.Layout;

/// <summary>
/// Modelo de navegação do front — mesmo padrão do manual de componentes:
/// itens de <see cref="RailNavItem"/> para o rail (com separador e ações) e
/// <see cref="XFMenuItem"/> para a sidebar de cada seção.
/// </summary>
public static class SampleNavigation
{
    // Nenhum item com IsDefault: o NavigationStateService trata IsDefault como
    // "item de rodape" e some com ele da lista principal quando o path e "/".
    public static IReadOnlyList<RailNavItem> RailItems { get; } = new[]
    {
        Rail("clientes", "Clientes", "users", ["/clientes"], "/clientes"),
        Rail("arquitetura", "Arquitetura", "layer-group", ["/arquitetura"], "/arquitetura"),
        Separator(),
        FooterItem("inicio", "Início", "house", "/clientes"),
        FooterItem("sobre", "Sobre", "circle-info", "/arquitetura")
    };

    public static IReadOnlyList<SampleSection> Sections { get; } = new[]
    {
        new SampleSection("clientes", "Clientes", new[]
        {
            Menu("clientes", "Lista de clientes", "Grade com os clientes cadastrados", "/clientes", "table-cells"),
            Menu("clientes-novo", "Novo cliente", "Formulário de cadastro validado pelo Mediator", "/clientes/novo", "user-plus"),
        }),
        new SampleSection("arquitetura", "Arquitetura", new[]
        {
            Menu("arquitetura", "Como o sample é montado", "Camadas, pacotes e este front Blazor", "/arquitetura", "layer-group"),
        })
    };

    public static IReadOnlyList<XFMenuItem> MenuItems(string sectionBrand)
    {
        var section = Sections.FirstOrDefault(s => s.Brand.Equals(sectionBrand, StringComparison.OrdinalIgnoreCase));
        return section?.Items ?? Array.Empty<XFMenuItem>();
    }

    private static RailNavItem Rail(
        string brand,
        string label,
        string icon,
        string[] prefixes,
        string route,
        bool isDefault = false) =>
        new()
        {
            Brand = brand,
            Label = label,
            Icon = XFSvgIcons.Get(icon),
            RoutePrefixes = prefixes,
            OverviewRoute = route,
            IsDefault = isDefault
        };

    private static RailNavItem Separator() =>
        new() { Type = RailNavItemType.Separator, Brand = string.Empty, Label = "Separador" };

    private static RailNavItem FooterItem(string brand, string label, string icon, string route, bool isDefault = false) =>
        new()
        {
            Type = RailNavItemType.Route,
            Brand = brand,
            Label = label,
            Icon = XFSvgIcons.Get(icon),
            RoutePrefixes = new[] { route },
            OverviewRoute = route,
            IsDefault = isDefault
        };

    private static XFMenuItem Menu(string id, string text, string description, string href, string icon) =>
        new()
        {
            Id = id,
            Text = text,
            Description = description,
            Href = href,
            Icon = Svg(icon)
        };

    private static RenderFragment Svg(string name) => builder =>
    {
        var markup = XFSvgIcons.Get(name);
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "inline-flex h-4 w-4 items-center justify-center");
        builder.AddContent(2, (MarkupString)markup);
        builder.CloseElement();
    };

    public sealed record SampleSection(string Brand, string Title, IReadOnlyList<XFMenuItem> Items);
}
