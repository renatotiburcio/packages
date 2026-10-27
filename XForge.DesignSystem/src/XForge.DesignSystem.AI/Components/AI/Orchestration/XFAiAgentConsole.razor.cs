using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Orchestration;

/// <summary>
/// Enterprise AI agent orchestration console. Displays multi-agent hierarchy
/// with monitoring, logs, memory tracking, tool execution, queue and cost monitoring.
/// </summary>
public partial class XFAiAgentConsole : XForgeComponentBase
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Agents to display in the console.</summary>
    [Parameter]
    public IReadOnlyList<XFAiAgentInfo> Agents { get; set; } = Array.Empty<XFAiAgentInfo>();

    /// <summary>Log entries to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiAgentLogEntry> Logs { get; set; } = Array.Empty<XFAiAgentLogEntry>();

    /// <summary>Console title.</summary>
    [Parameter]
    public string Title { get; set; } = "Console de Agentes";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the agent hierarchy panel.</summary>
    [Parameter]
    public bool ShowAgents { get; set; } = true;

    /// <summary>Whether to show the log panel.</summary>
    [Parameter]
    public bool ShowLogs { get; set; } = true;

    /// <summary>Whether to show cost/token summary.</summary>
    [Parameter]
    public bool ShowCostSummary { get; set; } = true;

    /// <summary>Whether to show tool execution stats.</summary>
    [Parameter]
    public bool ShowToolStats { get; set; } = true;

    /// <summary>Maximum log entries to display.</summary>
    [Parameter]
    public int MaxLogs { get; set; } = 200;

    /// <summary>Empty state text for agents.</summary>
    [Parameter]
    public string EmptyAgentsText { get; set; } = "Nenhum agente registrado.";

    /// <summary>Empty state text for logs.</summary>
    [Parameter]
    public string EmptyLogsText { get; set; } = "Nenhum log de agente.";

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when an agent is selected.</summary>
    [Parameter]
    public EventCallback<XFAiAgentInfo> OnAgentSelected { get; set; }

    /// <summary>Fired when a log entry is clicked.</summary>
    [Parameter]
    public EventCallback<XFAiAgentLogEntry> OnLogEntryClick { get; set; }

    // ── RenderFragments ─────────────────────────────────────────────────

    /// <summary>Custom agent card template.</summary>
    [Parameter]
    public RenderFragment<XFAiAgentInfo>? AgentTemplate { get; set; }

    /// <summary>Custom log entry template.</summary>
    [Parameter]
    public RenderFragment<XFAiAgentLogEntry>? LogTemplate { get; set; }

    /// <summary>Custom empty state content for agents.</summary>
    [Parameter]
    public RenderFragment? EmptyAgentsContent { get; set; }

    /// <summary>Custom empty state content for logs.</summary>
    [Parameter]
    public RenderFragment? EmptyLogsContent { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private string? _selectedAgentId;
    private string? _logLevelFilter;
    private string? _logSearchText;

    private XFAiAgentInfo? SelectedAgent =>
        _selectedAgentId is not null ? Agents.FirstOrDefault(a => a.Id == _selectedAgentId) : null;

    private IReadOnlyList<XFAiAgentInfo> HierarchyAgents
    {
        get
        {
            // Show CEO first, then directors, then specialists, then workers
            return Agents
                .OrderBy(a => a.Role switch
                {
                    XFAiAgentRole.CEO => 0,
                    XFAiAgentRole.Director => 1,
                    XFAiAgentRole.Specialist => 2,
                    XFAiAgentRole.Worker => 3,
                    _ => 4
                })
                .ThenBy(a => a.Name)
                .ToList();
        }
    }

    private IReadOnlyList<XFAiAgentLogEntry> FilteredLogs
    {
        get
        {
            var logs = Logs.Take(MaxLogs);
            if (_selectedAgentId is not null)
                logs = logs.Where(l => l.AgentId == _selectedAgentId);
            if (!string.IsNullOrEmpty(_logLevelFilter))
                logs = _logLevelFilter switch
                {
                    "Debug" => logs.Where(l => l.Level == XFAiAgentLogLevel.Debug),
                    "Info" => logs.Where(l => l.Level == XFAiAgentLogLevel.Info),
                    "Warning" => logs.Where(l => l.Level == XFAiAgentLogLevel.Warning),
                    "Error" => logs.Where(l => l.Level == XFAiAgentLogLevel.Error),
                    "Tool" => logs.Where(l => l.Level == XFAiAgentLogLevel.Tool),
                    "Memory" => logs.Where(l => l.Level == XFAiAgentLogLevel.Memory),
                    _ => logs
                };
            if (!string.IsNullOrWhiteSpace(_logSearchText))
                logs = logs.Where(l =>
                    l.Message.Contains(_logSearchText, StringComparison.OrdinalIgnoreCase) ||
                    l.AgentName.Contains(_logSearchText, StringComparison.OrdinalIgnoreCase) ||
                    (l.ToolName?.Contains(_logSearchText, StringComparison.OrdinalIgnoreCase) ?? false));
            return logs.ToList();
        }
    }

    private int TotalAgents => Agents.Count;
    private int RunningAgents => Agents.Count(a => a.Status == XFAiAgentStatus.Running);
    private int ErrorAgents => Agents.Count(a => a.Status == XFAiAgentStatus.Error);
    private long TotalTokens => Agents.Sum(a => a.TotalTokens);
    private decimal TotalCost => Agents.Sum(a => a.CostUsd);
    private int TotalToolCalls => Agents.Sum(a => a.ToolCalls);
    private int TotalMemoryEntries => Agents.Sum(a => a.MemoryEntries);
    private int TotalQueueDepth => Agents.Sum(a => a.QueueDepth);

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-agent-console overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class).Trim();

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string SummaryGridClass =>
        "grid grid-cols-2 gap-2 border-b border-border-light px-4 py-3 sm:grid-cols-4 lg:grid-cols-6";

    private string AgentCardClass(XFAiAgentInfo agent)
    {
        var selected = agent.Id == _selectedAgentId;
        var baseClass = "relative flex cursor-pointer flex-col gap-1.5 rounded-lg border p-3 transition-colors";
        var borderClass = selected
            ? "border-[var(--color-primary)] bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]"
            : "border-border-light bg-bg-surface hover:bg-surface-50";
        return $"{baseClass} {borderClass}";
    }

    private string LogEntryClass(XFAiAgentLogEntry entry) =>
        $"flex items-start gap-2 rounded-md px-3 py-2 text-xs transition-colors hover:bg-surface-50 cursor-pointer {GetLogLevelBg(entry.Level)}";

    private string PanelClass =>
        "overflow-y-auto";

    private string FilterClass =>
        "rounded-md border border-surface-200 bg-bg-surface px-2 py-1 text-xs text-text-primary outline-none focus:border-[var(--color-primary)] focus:ring-1 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]";

    private string EmptyClass =>
        "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

    private string EmptyTextClass => "text-sm text-text-secondary";

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetRoleIcon(XFAiAgentRole role) => role switch
    {
        XFAiAgentRole.CEO => "crown",
        XFAiAgentRole.Director => "user-tie",
        XFAiAgentRole.Specialist => "user-gear",
        XFAiAgentRole.Worker => "robot",
        _ => "circle"
    };

    private static string GetRoleLabel(XFAiAgentRole role) => role switch
    {
        XFAiAgentRole.CEO => "CEO",
        XFAiAgentRole.Director => "Diretor",
        XFAiAgentRole.Specialist => "Especialista",
        XFAiAgentRole.Worker => "Worker",
        _ => "Agente"
    };

    private static string GetStatusColor(XFAiAgentStatus status) => status switch
    {
        XFAiAgentStatus.Idle => "bg-slate-400",
        XFAiAgentStatus.Running => "bg-emerald-500",
        XFAiAgentStatus.Error => "bg-red-500",
        XFAiAgentStatus.Stopped => "bg-amber-500",
        _ => "bg-slate-400"
    };

    private static string GetStatusText(XFAiAgentStatus status) => status switch
    {
        XFAiAgentStatus.Idle => "Ocioso",
        XFAiAgentStatus.Running => "Executando",
        XFAiAgentStatus.Error => "Erro",
        XFAiAgentStatus.Stopped => "Parado",
        _ => "Desconhecido"
    };

    private static string GetLogLevelColor(XFAiAgentLogLevel level) => level switch
    {
        XFAiAgentLogLevel.Debug => "text-slate-400",
        XFAiAgentLogLevel.Info => "text-blue-500",
        XFAiAgentLogLevel.Warning => "text-amber-500",
        XFAiAgentLogLevel.Error => "text-red-500",
        XFAiAgentLogLevel.Tool => "text-purple-500",
        XFAiAgentLogLevel.Memory => "text-emerald-500",
        _ => "text-text-muted"
    };

    private static string GetLogLevelBg(XFAiAgentLogLevel level) => level switch
    {
        XFAiAgentLogLevel.Error => "bg-red-50",
        _ => ""
    };

    private static string GetLogLevelIcon(XFAiAgentLogLevel level) => level switch
    {
        XFAiAgentLogLevel.Debug => "bug",
        XFAiAgentLogLevel.Info => "circle-info",
        XFAiAgentLogLevel.Warning => "triangle-exclamation",
        XFAiAgentLogLevel.Error => "circle-xmark",
        XFAiAgentLogLevel.Tool => "wrench",
        XFAiAgentLogLevel.Memory => "database",
        _ => "circle"
    };

    private static string FormatUptime(int seconds)
    {
        if (seconds < 60) return $"{seconds}s";
        if (seconds < 3600) return $"{seconds / 60}m {seconds % 60}s";
        return $"{seconds / 3600}h {(seconds % 3600) / 60}m";
    }

    private static string FormatTokens(long tokens)
    {
        if (tokens < 1000) return tokens.ToString();
        if (tokens < 1_000_000) return $"{tokens / 1000.0:F1}k";
        return $"{tokens / 1_000_000.0:F1}M";
    }

    private void SelectAgent(XFAiAgentInfo agent)
    {
        _selectedAgentId = _selectedAgentId == agent.Id ? null : agent.Id;
        if (_selectedAgentId is not null)
            OnAgentSelected.InvokeAsync(agent);
    }

    private void HandleLogLevelChange(ChangeEventArgs e)
    {
        _logLevelFilter = e.Value?.ToString();
    }

    private void HandleLogSearch(ChangeEventArgs e)
    {
        _logSearchText = e.Value?.ToString();
    }
}
