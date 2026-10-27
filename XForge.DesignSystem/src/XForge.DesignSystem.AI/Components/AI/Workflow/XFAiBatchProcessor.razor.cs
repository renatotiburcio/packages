using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Workflow;

/// <summary>
/// Batch operations processor with progress tracking, item-level status,
/// filtering, and cancel/retry support. Pure Blazor.
/// </summary>
public partial class XFAiBatchProcessor : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Items to process in the batch.</summary>
    [Parameter]
    public IReadOnlyList<XFAiBatchItem> Items { get; set; } = Array.Empty<XFAiBatchItem>();

    /// <summary>Current batch processing state.</summary>
    [Parameter]
    public XFAiBatchState? State { get; set; }

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Processador em Lote";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Processador em lote";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the item list.</summary>
    [Parameter]
    public bool ShowItems { get; set; } = true;

    /// <summary>Max visible items before scrolling.</summary>
    [Parameter]
    public int MaxVisibleItems { get; set; } = 50;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhum item para processar.";

    /// <summary>Custom empty content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when processing starts.</summary>
    [Parameter]
    public EventCallback OnStart { get; set; }

    /// <summary>Fired when processing is cancelled.</summary>
    [Parameter]
    public EventCallback OnCancel { get; set; }

    /// <summary>Fired when a specific item is retried.</summary>
    [Parameter]
    public EventCallback<XFAiBatchItem> OnRetryItem { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFAiBatchItemStatus? _statusFilter;

    private IEnumerable<XFAiBatchItem> FilteredItems =>
        Items.Where(i => _statusFilter == null || i.Status == _statusFilter);

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-batch-processor overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetStatusColor(XFAiWfExecutionStatus status) => status switch
    {
        XFAiWfExecutionStatus.Pending => "bg-slate-100 text-slate-600 dark:bg-slate-800 dark:text-slate-400",
        XFAiWfExecutionStatus.Running => "bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-300",
        XFAiWfExecutionStatus.Completed => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFAiWfExecutionStatus.Failed => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-300",
        XFAiWfExecutionStatus.Cancelled => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        _ => "bg-slate-100 text-slate-500"
    };

    private static string GetStatusLabel(XFAiWfExecutionStatus status) => status switch
    {
        XFAiWfExecutionStatus.Pending => "Pendente",
        XFAiWfExecutionStatus.Running => "Processando",
        XFAiWfExecutionStatus.Completed => "Concluido",
        XFAiWfExecutionStatus.Failed => "Falhou",
        XFAiWfExecutionStatus.Cancelled => "Cancelado",
        _ => "—"
    };

    private static string GetItemStatusColor(XFAiBatchItemStatus status) => status switch
    {
        XFAiBatchItemStatus.Pending => "bg-slate-100 text-slate-600",
        XFAiBatchItemStatus.Processing => "bg-blue-100 text-blue-700",
        XFAiBatchItemStatus.Completed => "bg-emerald-100 text-emerald-700",
        XFAiBatchItemStatus.Failed => "bg-red-100 text-red-700",
        XFAiBatchItemStatus.Cancelled => "bg-amber-100 text-amber-700",
        XFAiBatchItemStatus.Retrying => "bg-purple-100 text-purple-700",
        _ => "bg-slate-100 text-slate-500"
    };

    private static string GetItemStatusLabel(XFAiBatchItemStatus status) => status switch
    {
        XFAiBatchItemStatus.Pending => "Pendente",
        XFAiBatchItemStatus.Processing => "Processando",
        XFAiBatchItemStatus.Completed => "OK",
        XFAiBatchItemStatus.Failed => "Falha",
        XFAiBatchItemStatus.Cancelled => "Cancelado",
        XFAiBatchItemStatus.Retrying => "Retry",
        _ => "—"
    };

    // ── Interactions ────────────────────────────────────────────────────

    private Task HandleStart() => OnStart.InvokeAsync();
    private Task HandleCancel() => OnCancel.InvokeAsync();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
