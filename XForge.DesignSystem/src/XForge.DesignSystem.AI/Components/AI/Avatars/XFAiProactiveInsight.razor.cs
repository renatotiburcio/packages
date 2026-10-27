using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// Proactive insights panel for AI agents. Displays unsolicited suggestions,
/// warnings, optimizations, anomalies, and reminders based on user behavior.
/// Supports dismissal, action callbacks, and priority-based styling.
/// </summary>
public partial class XFAiProactiveInsight : XForgeComponentBase, IDisposable
{
    // -- Parameters ------------------------------------------------------

    /// <summary>Title displayed in the header.</summary>
    [Parameter] public string Title { get; set; } = "Insights Proativos";

    /// <summary>Whether to show the header.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter] public string EmptyText { get; set; } = "Nenhum insight no momento.";

    /// <summary>ARIA label for the component.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Insights proativos do agente IA";

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom content rendered below the insights list.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>External insights list (two-way binding).</summary>
    [Parameter] public IReadOnlyList<XFAiProactiveInsightItem> Insights { get; set; } = Array.Empty<XFAiProactiveInsightItem>();

    /// <summary>Fired when insights change.</summary>
    [Parameter] public EventCallback<IReadOnlyList<XFAiProactiveInsightItem>> InsightsChanged { get; set; }

    /// <summary>Fired when an insight action is clicked.</summary>
    [Parameter] public EventCallback<XFAiProactiveInsightItem> OnAction { get; set; }

    /// <summary>Fired when an insight is dismissed.</summary>
    [Parameter] public EventCallback<string> OnDismiss { get; set; }

    // -- Internal State --------------------------------------------------

    private readonly List<XFAiProactiveInsightItem> _insights = new();

    private IReadOnlyList<XFAiProactiveInsightItem> ActiveInsights => _insights
        .Where(i => !i.IsDismissed && (i.ExpiresAt is null || i.ExpiresAt > DateTimeOffset.UtcNow))
        .OrderByDescending(i => i.Priority)
        .ThenByDescending(i => i.GeneratedAt)
        .ToList()
        .AsReadOnly();

    // -- Lifecycle -------------------------------------------------------

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_insights, Insights) && !_insights.SequenceEqual(Insights))
        {
            _insights.Clear();
            _insights.AddRange(Insights);
        }
    }

    // -- Public Methods --------------------------------------------------

    /// <summary>
    /// Adds a new proactive insight.
    /// </summary>
    public void AddInsight(XFAiProactiveInsightItem insight)
    {
        _insights.Add(insight);
        InsightsChanged.InvokeAsync(_insights.ToList());
        StateHasChanged();
    }

    /// <summary>
    /// Clears all insights.
    /// </summary>
    public void ClearAll()
    {
        _insights.Clear();
        InsightsChanged.InvokeAsync(_insights.ToList());
        StateHasChanged();
    }

    // -- Internal --------------------------------------------------------

    private async Task HandleActionAsync(XFAiProactiveInsightItem insight)
    {
        if (OnAction.HasDelegate)
        {
            await OnAction.InvokeAsync(insight);
        }
    }

    private async Task DismissInsightAsync(string insightId)
    {
        var index = _insights.FindIndex(i => i.Id == insightId);
        if (index >= 0)
        {
            _insights[index] = _insights[index] with { IsDismissed = true };
            await OnDismiss.InvokeAsync(insightId);
            await InsightsChanged.InvokeAsync(_insights.ToList());
            StateHasChanged();
        }
    }

    // -- CSS Helpers -----------------------------------------------------

    private string RootClass => Join(
        "xf-ai-proactive-insight overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class);

    private static string GetInsightRowClass(XFAiProactiveInsightItem insight) => insight.Priority switch
    {
        XFAiInsightPriority.Critical => "border-l-2 border-l-danger-500 px-4 py-3 border-b border-b-surface-100 last:border-b-0",
        XFAiInsightPriority.High => "border-l-2 border-l-amber-500 px-4 py-3 border-b border-b-surface-100 last:border-b-0",
        _ => "px-4 py-3 border-b border-b-surface-100 last:border-b-0"
    };

    private static string GetTypeColorClass(XFAiInsightType type) => type switch
    {
        XFAiInsightType.Suggestion => "bg-violet-100 text-violet-700 dark:bg-violet-900/30 dark:text-violet-300",
        XFAiInsightType.Warning => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        XFAiInsightType.Info => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFAiInsightType.Optimization => "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-300",
        XFAiInsightType.Anomaly => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-300",
        XFAiInsightType.Reminder => "bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300",
        _ => "bg-gray-100 text-gray-700"
    };

    private static string GetTypeIcon(XFAiInsightType type) => type switch
    {
        XFAiInsightType.Suggestion => "💡",
        XFAiInsightType.Warning => "⚠",
        XFAiInsightType.Info => "ℹ",
        XFAiInsightType.Optimization => "⚡",
        XFAiInsightType.Anomaly => "🔍",
        XFAiInsightType.Reminder => "🔔",
        _ => "●"
    };

    private static string FormatTimestamp(DateTimeOffset timestamp)
    {
        var diff = DateTimeOffset.UtcNow - timestamp;
        if (diff.TotalMinutes < 1) return "agora";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m atrás";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h atrás";
        return timestamp.ToString("dd/MM HH:mm");
    }

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
