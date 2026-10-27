using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

#pragma warning disable CS0618

namespace XForge.DesignSystem.Components.AI.Audit;

/// <summary>
/// Enterprise AI audit timeline component. Displays immutable audit trail entries
/// with before/after diffs, AI reasoning, hash verification, and digital signatures.
/// </summary>
public partial class XFAiAuditTimeline : XForgeComponentBase
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Audit entries to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiAuditEntry> Entries { get; set; } = Array.Empty<XFAiAuditEntry>();

    /// <summary>Timeline title.</summary>
    [Parameter]
    public string Title { get; set; } = "Auditoria";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the type filter dropdown.</summary>
    [Parameter]
    public bool ShowFilter { get; set; } = true;

    /// <summary>Whether to show before/after diffs.</summary>
    [Parameter]
    public bool ShowDiff { get; set; } = true;

    /// <summary>Whether to show hash/signature info.</summary>
    [Parameter]
    public bool ShowHash { get; set; } = true;

    /// <summary>Whether to show AI entry type icons.</summary>
    [Parameter]
    public bool ShowIcons { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhuma entrada de auditoria.";

    /// <summary>Maximum entries to display.</summary>
    [Parameter]
    public int MaxEntries { get; set; } = 100;

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when an entry is clicked.</summary>
    [Parameter]
    public EventCallback<XFAiAuditEntry> OnEntryClick { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom empty state content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private string? _filterValue;

    private IReadOnlyList<XFAiAuditEntry> FilteredEntries
    {
        get
        {
            var entries = Entries.Take(MaxEntries);
            if (string.IsNullOrEmpty(_filterValue)) return entries.ToList();

            return _filterValue switch
            {
                "User" => entries.Where(e => !e.IsAiAction && e.Actor != "System").ToList(),
                "Ai" => entries.Where(e => e.IsAiAction).ToList(),
                "System" => entries.Where(e => e.Actor == "System").ToList(),
                _ => entries.ToList()
            };
        }
    }

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-audit-timeline overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class).Trim();

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string CountBadgeClass =>
        "inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-surface-100 px-1.5 text-[10px] font-bold text-text-secondary";

    private string FilterClass =>
        "rounded-md border border-surface-200 bg-bg-surface px-2 py-1 text-xs text-text-primary outline-none focus:border-[var(--color-primary)] focus:ring-1 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]";

    private string TimelineClass => "px-4 py-3";

    private string ItemClass(XFAiAuditEntry entry) =>
        $"relative flex gap-3 {(!string.IsNullOrEmpty(_filterValue) ? "" : "")}";

    private string DotContainerClass =>
        "flex flex-col items-center";

    private string DotClass(XFAiAuditEntry entry) =>
        $"mt-1.5 h-2.5 w-2.5 shrink-0 rounded-full ring-2 ring-bg-surface {GetDotColor(entry)}";

    private string VersionBadgeClass =>
        "inline-flex items-center rounded-full bg-surface-100 px-1.5 py-0.5 text-[10px] font-semibold text-text-secondary";

    private string CategoryBadgeClass =>
        "inline-flex items-center rounded-full bg-primary/10 px-1.5 py-0.5 text-[10px] font-semibold text-primary";

    private string DiffBoxClass =>
        "rounded-md bg-surface-50 px-2.5 py-2 ring-1 ring-border-light";

    private string EmptyClass =>
        "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

    private string EmptyTextClass => "text-sm text-text-secondary";

    // ── Helpers ─────────────────────────────────────────────────────────

    private string GetDotColor(XFAiAuditEntry entry)
    {
        if (entry.Actor == "System") return "bg-slate-400";
        if (entry.IsAiAction) return "bg-purple-500";
        return "bg-blue-500";
    }

    private static string GetActorIcon(XFAiAuditEntry entry)
    {
        if (entry.Actor == "System") return "gear";
        if (entry.IsAiAction) return "robot";
        return "user";
    }

    private void HandleFilterChange(ChangeEventArgs e)
    {
        _filterValue = e.Value?.ToString();
    }
}
