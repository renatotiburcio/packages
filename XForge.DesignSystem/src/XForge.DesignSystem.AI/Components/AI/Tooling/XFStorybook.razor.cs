using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Tooling;

/// <summary>
/// Storybook-like component explorer with component list, category filter, search,
/// prop controls, preview tabs, and status badges. Pure Blazor, no JS interop.
/// </summary>
public partial class XFStorybook : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "XForge Storybook";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Explorador de componentes";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the search input.</summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    /// <summary>Whether to show the category filter.</summary>
    [Parameter]
    public bool ShowCategoryFilter { get; set; } = true;

    /// <summary>Whether to show the preview panel.</summary>
    [Parameter]
    public bool ShowPreview { get; set; } = true;

    /// <summary>Components to display in the explorer.</summary>
    [Parameter]
    public IReadOnlyList<XFStorybookComponent> Components { get; set; } = Array.Empty<XFStorybookComponent>();

    /// <summary>Controls for the selected component.</summary>
    [Parameter]
    public IReadOnlyList<XFStorybookControl>? ComponentControls { get; set; }

    /// <summary>Fired when a component is selected.</summary>
    [Parameter]
    public EventCallback<XFStorybookComponent> OnComponentSelected { get; set; }

    /// <summary>Fired when a control value changes.</summary>
    [Parameter]
    public EventCallback<(string Parameter, string? Value)> OnControlChanged { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private string _searchTerm = "";
    private string? _selectedCategory;
    private XFStorybookComponent? _selectedComponent;
    private readonly HashSet<string> _collapsedGroups = new();
    private string _previewTab = "render";
    private string _previewCode = "";
    private List<XFStorybookControl> _controls = new();
    private readonly Dictionary<string, string?> _controlValues = new();

    private List<string> _categories =>
        Components.Select(c => c.Category).Distinct().OrderBy(c => c).ToList();

    private List<XFStorybookComponent> FilteredComponents
    {
        get
        {
            var query = Components.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(_searchTerm))
                query = query.Where(c => c.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         c.Description.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(_selectedCategory))
                query = query.Where(c => c.Category == _selectedCategory);
            return query.OrderBy(c => c.Category).ThenBy(c => c.Name).ToList();
        }
    }

    private Dictionary<string, List<XFStorybookComponent>> GroupedComponents
    {
        get
        {
            var grouped = new Dictionary<string, List<XFStorybookComponent>>();
            foreach (var comp in FilteredComponents)
            {
                if (!grouped.ContainsKey(comp.Category))
                    grouped[comp.Category] = new List<XFStorybookComponent>();
                grouped[comp.Category].Add(comp);
            }
            return grouped;
        }
    }

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (ComponentControls is not null)
            _controls = ComponentControls.ToList();
    }

    // ── Handlers ────────────────────────────────────────────────────────

    private void OnSearchChanged(ChangeEventArgs e)
    {
        _searchTerm = e.Value?.ToString() ?? "";
    }

    private void OnCategoryChanged(ChangeEventArgs e)
    {
        _selectedCategory = e.Value?.ToString();
        if (string.IsNullOrEmpty(_selectedCategory))
            _selectedCategory = null;
    }

    private void ToggleGroup(string group)
    {
        if (!_collapsedGroups.Add(group))
            _collapsedGroups.Remove(group);
    }

    private void SelectComponent(XFStorybookComponent comp)
    {
        _selectedComponent = comp;
        _previewTab = "render";
        _previewCode = $"<{comp.Name} />";
        _controls = ComponentControls?.ToList() ?? new List<XFStorybookControl>();
        _controlValues.Clear();
        foreach (var c in _controls)
            _controlValues[c.ParameterName] = c.DefaultValue;
        OnComponentSelected.InvokeAsync(comp);
    }

    private void HandleControlChanged(string parameter, string? value)
    {
        _controlValues[parameter] = value;
        RebuildPreviewCode();
        OnControlChanged.InvokeAsync((parameter, value));
    }

    private void RebuildPreviewCode()
    {
        if (_selectedComponent is null) return;

        var attrs = new List<string>();
        foreach (var kv in _controlValues.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            var isString = _controls.FirstOrDefault(c => c.ParameterName == kv.Key)?.Type == "string";
            attrs.Add(isString ? $"{kv.Key}=\"{kv.Value}\"" : $"{kv.Key}=\"@{kv.Value}\"");
        }

        _previewCode = attrs.Any()
            ? $"<{_selectedComponent.Name}\n    {string.Join("\n    ", attrs)} />"
            : $"<{_selectedComponent.Name} />";
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetStatusColor(XFStorybookComponentStatus status) => status switch
    {
        XFStorybookComponentStatus.Stable => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFStorybookComponentStatus.Beta => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFStorybookComponentStatus.Deprecated => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-300",
        XFStorybookComponentStatus.Experimental => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        _ => "bg-slate-100 text-slate-600"
    };

    private static string GetStatusLabel(XFStorybookComponentStatus status) => status switch
    {
        XFStorybookComponentStatus.Stable => "Stable",
        XFStorybookComponentStatus.Beta => "Beta",
        XFStorybookComponentStatus.Deprecated => "Deprecated",
        XFStorybookComponentStatus.Experimental => "Experimental",
        _ => ""
    };

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-storybook overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
