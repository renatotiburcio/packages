using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Enterprise AI memory viewer component. Visualizes AI memory entries
/// with type filtering, relevance scores, access tracking, and tags.
/// </summary>
public partial class XFAiMemoryViewer : XForgeComponentBase
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Memory entries to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiMemoryEntry> Memories { get; set; } = Array.Empty<XFAiMemoryEntry>();

    /// <summary>Panel title.</summary>
    [Parameter]
    public string Title { get; set; } = "Memoria IA";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the statistics bar.</summary>
    [Parameter]
    public bool ShowStats { get; set; } = true;

    /// <summary>Whether to show relevance bars.</summary>
    [Parameter]
    public bool ShowRelevance { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhuma memoria registrada.";

    /// <summary>Maximum entries to display.</summary>
    [Parameter]
    public int MaxEntries { get; set; } = 50;

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when a memory entry is clicked.</summary>
    [Parameter]
    public EventCallback<XFAiMemoryEntry> OnMemoryClick { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom memory entry template.</summary>
    [Parameter]
    public RenderFragment<XFAiMemoryEntry>? MemoryTemplate { get; set; }

    /// <summary>Custom empty state content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private readonly HashSet<XFAiMemoryType> _activeFilters = new();

    private IReadOnlyList<XFAiMemoryType> AvailableTypes => Enum.GetValues<XFAiMemoryType>();

    private IReadOnlyList<XFAiMemoryEntry> FilteredMemories
    {
        get
        {
            var entries = Memories.Take(MaxEntries);
            if (_activeFilters.Count == 0) return entries.ToList();
            return entries.Where(m => _activeFilters.Contains(m.Type)).ToList();
        }
    }

    private double AverageRelevance => Memories.Count > 0
        ? Memories.Average(m => m.Relevance)
        : 0;

    private int TotalAccesses => Memories.Sum(m => m.AccessCount);

    private bool IsTypeActive(XFAiMemoryType type) => _activeFilters.Contains(type);

    private void ToggleTypeFilter(XFAiMemoryType type)
    {
        if (!_activeFilters.Remove(type))
            _activeFilters.Add(type);
    }

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-memory-viewer overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class).Trim();

    private string HeaderClass =>
        "flex flex-wrap items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string StatsClass =>
        "border-b border-border-light bg-surface-50/50 px-4 py-2";

    private string ListClass => "divide-y divide-border-light";

    private string ItemClass =>
        "flex items-start gap-3 px-4 py-3";

    private string FilterChipClass(bool active) => active
        ? "inline-flex h-6 items-center rounded-full bg-primary px-2 text-[10px] font-semibold text-primary-contrast outline-none transition-all hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus"
        : "inline-flex h-6 items-center rounded-full bg-surface-100 px-2 text-[10px] font-medium text-text-secondary outline-none transition-all hover:bg-surface-200 focus-visible:ring-2 focus-visible:ring-border-focus";

    private string IconContainerClass(XFAiMemoryType type) =>
        $"mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-lg {TypeBgClass(type)}";

    private string IconClass(XFAiMemoryType type) =>
        $"h-4 w-4 {TypeTextClass(type)}";

    private string TypeBadgeClass(XFAiMemoryType type) =>
        $"inline-flex items-center rounded-full px-1.5 py-0.5 text-[10px] font-semibold {TypeBgClass(type)} {TypeTextClass(type)}";

    private string RelevanceTrackClass =>
        "h-1 w-16 overflow-hidden rounded-full bg-surface-200";

    private string RelevanceFillClass(XFAiMemoryType type) =>
        $"h-full rounded-full transition-all duration-300 {TypeBarClass(type)}";

    private string EmptyClass =>
        "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

    private string EmptyTextClass => "text-sm text-text-secondary";

    // ── Type Styling ────────────────────────────────────────────────────

    private static string TypeBgClass(XFAiMemoryType type) => type switch
    {
        XFAiMemoryType.Session => "bg-blue-50",
        XFAiMemoryType.LongTerm => "bg-purple-50",
        XFAiMemoryType.Semantic => "bg-emerald-50",
        XFAiMemoryType.Preferences => "bg-amber-50",
        XFAiMemoryType.Factual => "bg-slate-100",
        _ => "bg-surface-100"
    };

    private static string TypeTextClass(XFAiMemoryType type) => type switch
    {
        XFAiMemoryType.Session => "text-blue-600",
        XFAiMemoryType.LongTerm => "text-purple-600",
        XFAiMemoryType.Semantic => "text-emerald-600",
        XFAiMemoryType.Preferences => "text-amber-600",
        XFAiMemoryType.Factual => "text-slate-600",
        _ => "text-text-muted"
    };

    private static string TypeBarClass(XFAiMemoryType type) => type switch
    {
        XFAiMemoryType.Session => "bg-blue-500",
        XFAiMemoryType.LongTerm => "bg-purple-500",
        XFAiMemoryType.Semantic => "bg-emerald-500",
        XFAiMemoryType.Preferences => "bg-amber-500",
        XFAiMemoryType.Factual => "bg-slate-400",
        _ => "bg-surface-300"
    };

    private static string GetTypeLabel(XFAiMemoryType type) => type switch
    {
        XFAiMemoryType.Session => "Sessao",
        XFAiMemoryType.LongTerm => "Longo prazo",
        XFAiMemoryType.Semantic => "Semantica",
        XFAiMemoryType.Preferences => "Preferencias",
        XFAiMemoryType.Factual => "Factual",
        _ => type.ToString()
    };

    private static string GetTypeIcon(XFAiMemoryType type) => type switch
    {
        XFAiMemoryType.Session => "comments",
        XFAiMemoryType.LongTerm => "database",
        XFAiMemoryType.Semantic => "diagram-project",
        XFAiMemoryType.Preferences => "sliders",
        XFAiMemoryType.Factual => "book",
        _ => "circle"
    };
}
