using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

#pragma warning disable CS0618

namespace XForge.DesignSystem.Components.AI.Enterprise;

/// <summary>
/// Enterprise audit trail component — displays filterable, searchable audit log entries
/// with severity indicators, action tracking, and export capability.
/// </summary>
public partial class XFAuditLogger : XForgeComponentBase, IDisposable
{
    private XFAuditSeverity? _severityFilter;
    private XFAuditAction? _actionFilter;
    private string? _searchText;

    // ─── Parameters ────────────────────────────────────────────────────────────

    /// <summary>Audit log entries to display.</summary>
    [Parameter] public IReadOnlyList<XFAuditEntry> Entries { get; set; } = Array.Empty<XFAuditEntry>();

    /// <summary>Component title.</summary>
    [Parameter] public string Title { get; set; } = "Auditoria";

    /// <summary>Show filter controls.</summary>
    [Parameter] public bool ShowFilters { get; set; } = true;

    /// <summary>Show export button.</summary>
    [Parameter] public bool ShowExportButton { get; set; } = true;

    /// <summary>Maximum visible entries before "load more".</summary>
    [Parameter] public int MaxVisible { get; set; } = 50;

    // ─── Labels (i18n) ─────────────────────────────────────────────────────────

    /// <summary>Export button label.</summary>
    [Parameter] public string ExportLabel { get; set; } = "Exportar";

    /// <summary>All severities filter option label.</summary>
    [Parameter] public string AllSeveritiesLabel { get; set; } = "Todas as severidades";

    /// <summary>All actions filter option label.</summary>
    [Parameter] public string AllActionsLabel { get; set; } = "Todas as acoes";

    /// <summary>Search input placeholder.</summary>
    [Parameter] public string SearchPlaceholder { get; set; } = "Pesquisar...";

    /// <summary>Empty state label.</summary>
    [Parameter] public string EmptyLabel { get; set; } = "Nenhum registro de auditoria";

    /// <summary>Load more button label.</summary>
    [Parameter] public string LoadMoreLabel { get; set; } = "Carregar mais";

    /// <summary>Remaining count label.</summary>
    [Parameter] public string RemainingLabel { get; set; } = "restantes";

    // ─── Events ────────────────────────────────────────────────────────────────

    /// <summary>Raised when user clicks export.</summary>
    [Parameter] public EventCallback<IReadOnlyList<XFAuditEntry>> OnExport { get; set; }

    /// <summary>Raised when filters change.</summary>
    [Parameter] public EventCallback<XFAuditFilter> OnFilterChange { get; set; }

    /// <summary>Raised when "load more" is clicked.</summary>
    [Parameter] public EventCallback OnLoadMore { get; set; }

    // ─── Computed ───────────────────────────────────────────────────────────────

    private string RootClass => $"xf-audit-logger {Class}".Trim();

    private IReadOnlyList<XFAuditEntry> FilteredEntries
    {
        get
        {
            var query = Entries.AsEnumerable();

            if (_severityFilter.HasValue)
                query = query.Where(e => e.Severity == _severityFilter.Value);

            if (_actionFilter.HasValue)
                query = query.Where(e => e.Action == _actionFilter.Value);

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var search = _searchText.Trim();
                query = query.Where(e =>
                    (e.Details?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Resource?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.UserName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.CustomAction?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return query.OrderByDescending(e => e.Timestamp).ToList();
        }
    }

    // ─── Handlers ──────────────────────────────────────────────────────────────

    private void HandleSeverityFilter(ChangeEventArgs e)
    {
        _severityFilter = Enum.TryParse<XFAuditSeverity>(e.Value?.ToString(), out var s) ? s : null;
        NotifyFilterChange();
    }

    private void HandleActionFilter(ChangeEventArgs e)
    {
        _actionFilter = Enum.TryParse<XFAuditAction>(e.Value?.ToString(), out var a) ? a : null;
        NotifyFilterChange();
    }

    private void HandleSearchInput(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString();
        NotifyFilterChange();
    }

    private void NotifyFilterChange()
    {
        if (OnFilterChange.HasDelegate)
        {
            var filter = new XFAuditFilter
            {
                Severity = _severityFilter,
                Action = _actionFilter,
                SearchText = _searchText,
                Limit = MaxVisible
            };
            _ = OnFilterChange.InvokeAsync(filter);
        }
    }

    private async Task HandleExportAsync()
    {
        if (OnExport.HasDelegate)
            await OnExport.InvokeAsync(FilteredEntries);
    }

    private async Task LoadMoreAsync()
    {
        if (OnLoadMore.HasDelegate)
            await OnLoadMore.InvokeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static string GetSeverityBgClass(XFAuditSeverity severity) => severity switch
    {
        XFAuditSeverity.Warning => "border-l-2 border-amber-400 dark:border-amber-600",
        XFAuditSeverity.Error => "border-l-2 border-red-400 dark:border-red-600",
        XFAuditSeverity.Critical => "border-l-2 border-red-600 dark:border-red-400",
        _ => ""
    };

    private static MarkupString GetSeverityIcon(XFAuditSeverity severity) => severity switch
    {
        XFAuditSeverity.Warning => (MarkupString)"<svg class=\"w-3.5 h-3.5 text-amber-500 shrink-0 mt-0.5\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\" stroke=\"currentColor\" stroke-width=\"2\" aria-hidden=\"true\"><path stroke-linecap=\"round\" stroke-linejoin=\"round\" d=\"M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.072 16.5c-.77.833.192 2.5 1.732 2.5z\" /></svg>",
        XFAuditSeverity.Error => (MarkupString)"<svg class=\"w-3.5 h-3.5 text-red-500 shrink-0 mt-0.5\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\" stroke=\"currentColor\" stroke-width=\"2\" aria-hidden=\"true\"><path stroke-linecap=\"round\" stroke-linejoin=\"round\" d=\"M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z\" /></svg>",
        XFAuditSeverity.Critical => (MarkupString)"<svg class=\"w-3.5 h-3.5 text-red-600 shrink-0 mt-0.5\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\" stroke=\"currentColor\" stroke-width=\"2\" aria-hidden=\"true\"><path stroke-linecap=\"round\" stroke-linejoin=\"round\" d=\"M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.072 16.5c-.77.833.192 2.5 1.732 2.5z\" /></svg>",
        _ => (MarkupString)"<svg class=\"w-3.5 h-3.5 text-gray-400 shrink-0 mt-0.5\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\" stroke=\"currentColor\" stroke-width=\"2\" aria-hidden=\"true\"><path stroke-linecap=\"round\" stroke-linejoin=\"round\" d=\"M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z\" /></svg>"
    };

    private static string GetActionLabel(XFAuditAction action) => action switch
    {
        XFAuditAction.Create => "Criar",
        XFAuditAction.Read => "Ler",
        XFAuditAction.Update => "Atualizar",
        XFAuditAction.Delete => "Excluir",
        XFAuditAction.Login => "Login",
        XFAuditAction.Logout => "Logout",
        XFAuditAction.Export => "Exportar",
        XFAuditAction.Import => "Importar",
        XFAuditAction.Approve => "Aprovar",
        XFAuditAction.Reject => "Rejeitar",
        XFAuditAction.Custom => "Custom",
        _ => action.ToString()
    };

    private static string FormatTimestamp(DateTimeOffset ts)
    {
        var diff = DateTimeOffset.UtcNow - ts;
        if (diff.TotalSeconds < 60) return "agora";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}min atras";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h atras";
        return ts.ToLocalTime().ToString("dd/MM HH:mm");
    }

    private static string FormatDuration(TimeSpan dur)
    {
        if (dur.TotalMilliseconds < 1000) return $"{(int)dur.TotalMilliseconds}ms";
        if (dur.TotalSeconds < 60) return $"{dur.TotalSeconds:F1}s";
        return $"{(int)dur.TotalMinutes}m{dur.Seconds:D2}s";
    }

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
