using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Enterprise AI source citation list component. Displays ranked source citations
/// with confidence scores, snippets, source types, and links.
/// </summary>
public partial class XFAiSourceCitationList : XForgeComponentBase
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Citations to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiSourceCitation> Citations { get; set; } = Array.Empty<XFAiSourceCitation>();

    /// <summary>Panel title.</summary>
    [Parameter]
    public string Title { get; set; } = "Fontes";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show confidence scores.</summary>
    [Parameter]
    public bool ShowConfidence { get; set; } = true;

    /// <summary>Whether to show text snippets.</summary>
    [Parameter]
    public bool ShowSnippets { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhuma fonte citada.";

    /// <summary>Maximum citations to display.</summary>
    [Parameter]
    public int MaxCitations { get; set; } = 20;

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when a citation is clicked.</summary>
    [Parameter]
    public EventCallback<XFAiSourceCitation> OnCitationClick { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom citation template.</summary>
    [Parameter]
    public RenderFragment<XFAiSourceCitation>? CitationTemplate { get; set; }

    /// <summary>Custom empty state content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Computed ────────────────────────────────────────────────────────

    private IReadOnlyList<XFAiSourceCitation> SortedCitations =>
        Citations.OrderBy(c => c.Rank).Take(MaxCitations).ToList();

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-source-citation-list overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class).Trim();

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string CountBadgeClass =>
        "inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-surface-100 px-1.5 text-[10px] font-bold text-text-secondary";

    private string ListClass => "divide-y divide-border-light";

    private string ItemClass =>
        "flex items-start gap-3 px-4 py-3";

    private string RankClass =>
        "mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-primary/10 text-[11px] font-bold text-primary";

    private string TitleClass =>
        "text-sm font-medium text-text-primary leading-5";

    private string TitleLinkClass =>
        "text-sm font-medium text-primary leading-5 underline decoration-primary/30 underline-offset-2 hover:decoration-primary focus-visible:ring-2 focus-visible:ring-border-focus";

    private string SourceTypeBadgeClass =>
        "inline-flex items-center rounded-full bg-surface-100 px-1.5 py-0.5 text-[10px] font-semibold text-text-secondary";

    private string ConfidenceTrackClass =>
        "h-1 w-12 overflow-hidden rounded-full bg-surface-200";

    private string ConfidenceFillClass =>
        "h-full rounded-full transition-all duration-300 bg-primary";

    private string SnippetClass =>
        "mt-2 flex items-start gap-1.5 rounded-md bg-surface-50 px-2.5 py-2 text-xs text-text-secondary italic ring-1 ring-border-light";

    private string EmptyClass =>
        "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

    private string EmptyTextClass => "text-sm text-text-secondary";
}
