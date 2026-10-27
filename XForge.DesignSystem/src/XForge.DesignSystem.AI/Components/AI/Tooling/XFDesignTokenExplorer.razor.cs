using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Tooling;

/// <summary>
/// Design token explorer with search, category filter, grouping, copy, and
/// multi-format export (CSS/SCSS/JSON/TypeScript). Pure Blazor, no JS interop.
/// </summary>
public partial class XFDesignTokenExplorer : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Design Tokens";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Explorador de design tokens";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the search input.</summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    /// <summary>Whether to show the category filter.</summary>
    [Parameter]
    public bool ShowCategoryFilter { get; set; } = true;

    /// <summary>Whether to show export buttons.</summary>
    [Parameter]
    public bool ShowExport { get; set; } = true;

    /// <summary>Whether to show group headers with collapse.</summary>
    [Parameter]
    public bool ShowGroupHeaders { get; set; } = true;

    /// <summary>Whether to show copy button per token.</summary>
    [Parameter]
    public bool ShowCopyButton { get; set; } = true;

    /// <summary>Design tokens to display.</summary>
    [Parameter]
    public IReadOnlyList<XFDesignToken> Tokens { get; set; } = Array.Empty<XFDesignToken>();

    /// <summary>Fired when a token is copied.</summary>
    [Parameter]
    public EventCallback<XFDesignToken> OnTokenCopied { get; set; }

    /// <summary>Fired when tokens are exported.</summary>
    [Parameter]
    public EventCallback<string> OnExport { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private string _searchTerm = "";
    private XFDesignTokenCategory? _selectedCategory;
    private string _exportOutput = "";
    private string _exportFormat = "";
    private readonly HashSet<string> _collapsedGroups = new();

    private List<XFDesignTokenCategory> _categories =>
        Tokens.Select(t => t.Category).Distinct().OrderBy(c => c).ToList();

    private List<XFDesignToken> FilteredTokens
    {
        get
        {
            var query = Tokens.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(_searchTerm))
                query = query.Where(t => t.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         t.CssVariable.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         t.Description?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) == true);
            if (_selectedCategory.HasValue)
                query = query.Where(t => t.Category == _selectedCategory.Value);
            return query.OrderBy(t => t.Category).ThenBy(t => t.Name).ToList();
        }
    }

    private Dictionary<string, List<XFDesignToken>> GroupedTokens
    {
        get
        {
            var grouped = new Dictionary<string, List<XFDesignToken>>();
            foreach (var token in FilteredTokens)
            {
                var group = token.Group ?? GetCategoryLabel(token.Category);
                if (!grouped.ContainsKey(group))
                    grouped[group] = new List<XFDesignToken>();
                grouped[group].Add(token);
            }
            return grouped;
        }
    }

    // ── Handlers ────────────────────────────────────────────────────────

    private void OnSearchChanged(ChangeEventArgs e)
    {
        _searchTerm = e.Value?.ToString() ?? "";
    }

    private void OnCategoryChanged(ChangeEventArgs e)
    {
        var val = e.Value?.ToString();
        _selectedCategory = string.IsNullOrEmpty(val) ? null : Enum.Parse<XFDesignTokenCategory>(val);
    }

    private void ToggleGroup(string group)
    {
        if (!_collapsedGroups.Add(group))
            _collapsedGroups.Remove(group);
    }

    private async Task CopyToken(XFDesignToken token)
    {
        await OnTokenCopied.InvokeAsync(token);
    }

    private void ExportTokens(string format)
    {
        _exportFormat = format;
        var tokens = FilteredTokens;

        _exportOutput = format switch
        {
            "CSS" => ExportAsCss(tokens),
            "SCSS" => ExportAsScss(tokens),
            "JSON" => ExportAsJson(tokens),
            "TS" => ExportAsTypeScript(tokens),
            _ => ""
        };

        OnExport.InvokeAsync(_exportOutput);
    }

    // ── Export Formatters ───────────────────────────────────────────────

    private static string ExportAsCss(List<XFDesignToken> tokens)
    {
        var lines = new List<string> { ":root {" };
        foreach (var t in tokens)
            lines.Add($"  {t.CssVariable}: {t.Value};");
        lines.Add("}");
        return string.Join("\n", lines);
    }

    private static string ExportAsScss(List<XFDesignToken> tokens)
    {
        var lines = new List<string>();
        foreach (var t in tokens)
            lines.Add($"${t.Name.ToLower().Replace("-", "_")}: {t.Value};");
        return string.Join("\n", lines);
    }

    private static string ExportAsJson(List<XFDesignToken> tokens)
    {
        var dict = tokens.ToDictionary(t => t.CssVariable, t => t.Value);
        return System.Text.Json.JsonSerializer.Serialize(dict, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }

    private static string ExportAsTypeScript(List<XFDesignToken> tokens)
    {
        var lines = new List<string> { "export const designTokens = {" };
        foreach (var t in tokens)
        {
            var key = t.CssVariable.Replace("--", "").Replace("-", "_");
            lines.Add($"  {key}: \"{t.Value}\",");
        }
        lines.Add("} as const;");
        return string.Join("\n", lines);
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetCategoryLabel(XFDesignTokenCategory category) => category switch
    {
        XFDesignTokenCategory.Color => "Cores",
        XFDesignTokenCategory.Typography => "Tipografia",
        XFDesignTokenCategory.Spacing => "Espacamento",
        XFDesignTokenCategory.Border => "Bordas",
        XFDesignTokenCategory.Shadow => "Sombras",
        XFDesignTokenCategory.Motion => "Movimento",
        XFDesignTokenCategory.Breakpoint => "Breakpoints",
        XFDesignTokenCategory.ZIndex => "Z-Index",
        XFDesignTokenCategory.Opacity => "Opacidade",
        _ => "Outros"
    };

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-design-token-explorer overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
