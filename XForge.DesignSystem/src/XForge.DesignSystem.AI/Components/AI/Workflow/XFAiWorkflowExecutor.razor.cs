using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Workflow;

/// <summary>
/// Runtime workflow execution engine with step-by-step progression,
/// progress tracking, log filtering, and cancel support. Pure Blazor.
/// </summary>
public partial class XFAiWorkflowExecutor : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Current execution state.</summary>
    [Parameter]
    public XFAiWfExecutionState? Execution { get; set; }

    /// <summary>Executor title.</summary>
    [Parameter]
    public string Title { get; set; } = "Executor de Workflow";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Executor de workflow";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Configure um workflow para executar.";

    /// <summary>Custom empty content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when the user requests execution start.</summary>
    [Parameter]
    public EventCallback OnStart { get; set; }

    /// <summary>Fired when the user requests execution cancel.</summary>
    [Parameter]
    public EventCallback OnCancel { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFAiWfLogLevel? _logFilter;

    private IEnumerable<XFAiWfExecLog> FilteredLogs =>
        Execution?.Logs.Where(l => _logFilter == null || l.Level == _logFilter) ?? Enumerable.Empty<XFAiWfExecLog>();

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-workflow-executor overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    private string ProgressColorClass => Execution?.Status switch
    {
        XFAiWfExecutionStatus.Running => "bg-emerald-500",
        XFAiWfExecutionStatus.Completed => "bg-emerald-500",
        XFAiWfExecutionStatus.Failed => "bg-red-500",
        XFAiWfExecutionStatus.Cancelled => "bg-amber-500",
        _ => "bg-slate-300"
    };

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetStatusColor(XFAiWfExecutionStatus status) => status switch
    {
        XFAiWfExecutionStatus.Pending => "bg-slate-100 text-slate-600 dark:bg-slate-800 dark:text-slate-400",
        XFAiWfExecutionStatus.Running => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFAiWfExecutionStatus.Completed => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFAiWfExecutionStatus.Failed => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-300",
        XFAiWfExecutionStatus.Cancelled => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        XFAiWfExecutionStatus.WaitingApproval => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFAiWfExecutionStatus.Skipped => "bg-slate-100 text-slate-500",
        _ => "bg-slate-100 text-slate-500"
    };

    private static string GetStatusLabel(XFAiWfExecutionStatus status) => status switch
    {
        XFAiWfExecutionStatus.Pending => "Pendente",
        XFAiWfExecutionStatus.Running => "Executando",
        XFAiWfExecutionStatus.Completed => "Concluido",
        XFAiWfExecutionStatus.Failed => "Falhou",
        XFAiWfExecutionStatus.Cancelled => "Cancelado",
        XFAiWfExecutionStatus.WaitingApproval => "Aguardando Aprovacao",
        XFAiWfExecutionStatus.Skipped => "Ignorado",
        _ => "Desconhecido"
    };

    private string GetElapsedTime()
    {
        if (Execution is null) return "—";
        var end = Execution.CompletedAt ?? DateTime.UtcNow;
        var elapsed = end - Execution.StartedAt;
        return elapsed.TotalMinutes >= 1
            ? $"{elapsed.Minutes}m {elapsed.Seconds}s"
            : $"{elapsed.TotalSeconds:F1}s";
    }

    private static string GetLogLevelClass(XFAiWfLogLevel level) => level switch
    {
        XFAiWfLogLevel.Error => "text-red-600 dark:text-red-400",
        XFAiWfLogLevel.Warning => "text-amber-600 dark:text-amber-400",
        XFAiWfLogLevel.Success => "text-emerald-600 dark:text-emerald-400",
        XFAiWfLogLevel.Debug => "text-slate-400",
        _ => "text-blue-600 dark:text-blue-400"
    };

    private static string GetLogLevelLabel(XFAiWfLogLevel level) => level switch
    {
        XFAiWfLogLevel.Error => "ERRO",
        XFAiWfLogLevel.Warning => "AVISO",
        XFAiWfLogLevel.Success => "OK",
        XFAiWfLogLevel.Debug => "DEBUG",
        _ => "INFO"
    };

    private static string GetLogBgClass(XFAiWfLogLevel level) => level switch
    {
        XFAiWfLogLevel.Error => "bg-red-50 dark:bg-red-900/10",
        XFAiWfLogLevel.Warning => "bg-amber-50 dark:bg-amber-900/10",
        _ => ""
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
