using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Components.Navigation.Menu;
using XForge.DesignSystem.Docs.Shared.Contracts;
using XForge.DesignSystem.Services;

namespace XForge.DesignSystem.Docs.Shared.Navigation;

/// <summary>
/// Builds the shared docs rail and sidebar model used by the public and commercial docs apps.
/// </summary>
public static class XForgeDocsNavigation
{
    private static readonly IReadOnlyDictionary<string, string> IconByCoreBrand = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["components"] = "layer-group",
        ["display"] = "table-cells",
        ["forms"] = "input-text",
        ["feedback"] = "message-exclamation",
        ["layout"] = "window-frame",
        ["navigation"] = "route",
        ["overlays"] = "window-restore",
        ["tokens"] = "palette",
        ["home"] = "house"
    };

    private static readonly IReadOnlyDictionary<string, string> IconByAiCategory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["core"] = "sparkles",
        ["agents"] = "diagram-project",
        ["business"] = "briefcase",
        ["multimodal"] = "eye",
        ["governance"] = "shield-check"
    };

    private static readonly IReadOnlyDictionary<string, string> AiCategoryTitles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["core"] = "AI Core",
        ["agents"] = "Agents",
        ["business"] = "Business AI",
        ["multimodal"] = "Multimodal",
        ["governance"] = "Governance"
    };

    public static IReadOnlyList<RailNavItem> GetRailItems(XForgeDocsProduct product)
    {
        return product switch
        {
            XForgeDocsProduct.AI => GetAiRailItems(),
            _ => GetCoreRailItems()
        };
    }

    public static IReadOnlyList<XForgeDocsSidebarSection> GetSidebarSections(XForgeDocsProduct product)
    {
        return product switch
        {
            XForgeDocsProduct.AI => GetAiSidebarSections(),
            _ => GetCoreSidebarSections()
        };
    }

    public static string GetProductName(XForgeDocsProduct product) =>
        product switch
        {
            XForgeDocsProduct.AI => "XForge DesignSystem AI Docs",
            _ => "XForge DesignSystem Docs"
        };

    public static string GetBrandTitle(string brand, XForgeDocsProduct product)
    {
        var sections = GetSidebarSections(product);
        return sections.FirstOrDefault(section => section.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase))?.Title
            ?? (IsAiProduct(product) ? "AI Commercial" : "XForge Docs");
    }

    private static IReadOnlyList<RailNavItem> GetCoreRailItems() =>
    [
        Rail("components", "Components", "layer-group", ["/components"], "/components/overview"),
        Rail("display", "Display", "table-cells", ["/display"], "/display/overview"),
        Rail("forms", "Forms", "input-text", ["/forms"], "/forms/overview"),
        Rail("feedback", "Feedback", "message-exclamation", ["/feedback"], "/feedback/overview"),
        Rail("layout", "Layout", "window-frame", ["/layout"], "/layout/overview"),
        Rail("navigation", "Navigation", "route", ["/navigation"], "/navigation/overview"),
        Rail("overlays", "Overlays", "window-restore", ["/overlays"], "/overlays/overview"),
        Rail("tokens", "Tokens", "palette", ["/tokens"], "/tokens/overview"),
        new() { Type = RailNavItemType.Separator, Brand = string.Empty, Label = "Separador" },
        Rail("home", "Inicio", "house", ["/"], "/", true)
    ];

    private static IReadOnlyList<RailNavItem> GetAiRailItems()
    {
        return XForgeDocsCatalog.AiItems
            .GroupBy(item => GetAiCategory(item.Group))
            .Select((group, index) => Rail(
                GetAiBrand(group.Key),
                GetAiCategoryTitle(group.Key),
                IconByAiCategory.GetValueOrDefault(group.Key) ?? "sparkles",
                index == 0 ? group.Select(item => item.Href).Prepend("/").ToArray() : group.Select(item => item.Href).ToArray(),
                group.First().Href,
                index == 0))
            .ToArray();
    }



    private static IReadOnlyList<XForgeDocsSidebarSection> GetCoreSidebarSections()
    {
        return XForgeDocsCatalog.CoreItems
            .GroupBy(item => GetBrandFromHref(item.Href))
            .Select(group => new XForgeDocsSidebarSection(
                group.Key,
                GetSectionTitle(group.Key, group.First().Group),
                group.Select(ToMenuItem).ToArray()))
            .ToArray();
    }

    private static IReadOnlyList<XForgeDocsSidebarSection> GetAiSidebarSections()
    {
        return XForgeDocsCatalog.AiItems
            .GroupBy(item => GetAiCategory(item.Group))
            .Select(group => new XForgeDocsSidebarSection(
                GetAiBrand(group.Key),
                GetAiCategoryTitle(group.Key),
                group.Select(ToAiMenuItem).ToArray()))
            .ToArray();
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

    private static XFMenuItem ToMenuItem(XForgeDocsNavItem item) =>
        new()
        {
            Id = item.Href.Trim('/').Replace('/', '-'),
            Text = item.Label,
            Description = item.ComponentName,
            Href = item.Href,
            Icon = Icon("file-lines")
        };

    private static XFMenuItem ToAiMenuItem(XForgeDocsNavItem item) =>
        new()
        {
            Id = item.Href.Trim('/').Replace('/', '-'),
            Text = item.Label,
            Description = item.Group,
            Href = item.Href,
            Icon = Icon("file-lines")
        };

    private static RenderFragment Icon(string name) => builder => builder.AddMarkupContent(0, XFSvgIcons.Get(name));

    private static string GetBrandFromHref(string href)
    {
        var normalized = href.Trim('/');
        var slash = normalized.IndexOf('/', StringComparison.Ordinal);
        return slash < 0 ? normalized : normalized[..slash];
    }

    private static string GetSectionTitle(string brand, string fallback) =>
        brand switch
        {
            "components" => "Components",
            "display" => "Data Display",
            "forms" => "Forms",
            "feedback" => "Feedback",
            "layout" => "Layout and Shell",
            "navigation" => "Navigation",
            "overlays" => "Overlays",
            "tokens" => "Design Tokens",
            _ => fallback
        };

    public static string GetCoreIconName(string brand) =>
        IconByCoreBrand.GetValueOrDefault(brand) ?? "circle";

    public static string GetAiBrand(string category) =>
        $"ai-{Slug(category)}";

    public static string GetAiCategoryTitle(string category) =>
        AiCategoryTitles.GetValueOrDefault(category) ?? category;

    public static string GetAiCategory(string group) =>
        group switch
        {
            "Prompting" or "Chat" or "Utility" or "Services" or "Insights" or "Avatars" => "core",
            "Workflow" or "Orchestration" => "agents",
            "Accounting" or "Analysis" or "Financial" or "DataGrid" or "Forms" or "Fiscal" or "Stock" or "Documents" => "business",
            "Vision" or "Audio" or "MultiModal" => "multimodal",
            "Audit" or "Performance" or "Tooling" or "Enterprise" or "CrossPlatform" => "governance",
            _ => "core"
        };

    public static bool IsAiProduct(XForgeDocsProduct product) =>
        product is XForgeDocsProduct.AI;

    private static string GetCoreBrandFromGroup(string group) =>
        group switch
        {
            "Display" => "display",
            "Forms" => "forms",
            "Feedback" => "feedback",
            "Layout" => "layout",
            "Navigation" => "navigation",
            "Overlays" => "overlays",
            "Tokens" => "tokens",
            _ => "components"
        };

    private static string Slug(string value) =>
        value.Trim().ToLowerInvariant().Replace(" ", "-", StringComparison.Ordinal);
}
