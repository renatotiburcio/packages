using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Workflow;

/// <summary>
/// AI-powered task scheduler with calendar view, frequency configuration,
/// job management, and AI scheduling suggestions. Pure Blazor.
/// </summary>
public partial class XFAiScheduler : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Scheduled jobs to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiScheduledJob> Jobs { get; set; } = Array.Empty<XFAiScheduledJob>();

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Agendador de Tarefas";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Agendador de tarefas";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the mini calendar.</summary>
    [Parameter]
    public bool ShowCalendar { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhuma tarefa agendada.";

    /// <summary>Custom empty content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when the user requests adding a new scheduled job.</summary>
    [Parameter]
    public EventCallback OnAddJob { get; set; }

    /// <summary>Fired when a job is paused.</summary>
    [Parameter]
    public EventCallback<XFAiScheduledJob> OnPauseJob { get; set; }

    /// <summary>Fired when a job is resumed.</summary>
    [Parameter]
    public EventCallback<XFAiScheduledJob> OnResumeJob { get; set; }

    /// <summary>Fired when a job is selected.</summary>
    [Parameter]
    public EventCallback<XFAiScheduledJob> OnJobSelected { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private DateTime _weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

    private List<DateTime> _calendarDates => Enumerable.Range(0, 7)
        .Select(i => _weekStart.AddDays(i))
        .ToList();

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-scheduler overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── Helpers ─────────────────────────────────────────────────────────

    private string GetWeekLabel()
    {
        var start = _weekStart;
        var end = _weekStart.AddDays(6);
        return start.Month == end.Month
            ? $"{start:dd} – {end:dd} {start:MMM yyyy}"
            : $"{start:dd MMM} – {end:dd MMM yyyy}";
    }

    private int GetJobsOnDate(DateTime date) =>
        Jobs.Count(j => j.NextRunAt?.Date == date.Date);

    private void PreviousWeek() => _weekStart = _weekStart.AddDays(-7);
    private void NextWeek() => _weekStart = _weekStart.AddDays(7);

    private static string GetJobStatusColor(XFAiScheduledJobStatus status) => status switch
    {
        XFAiScheduledJobStatus.Active => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFAiScheduledJobStatus.Paused => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        XFAiScheduledJobStatus.Completed => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFAiScheduledJobStatus.Failed => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-300",
        XFAiScheduledJobStatus.Expired => "bg-slate-100 text-slate-500",
        _ => "bg-slate-100 text-slate-500"
    };

    private static string GetJobStatusLabel(XFAiScheduledJobStatus status) => status switch
    {
        XFAiScheduledJobStatus.Active => "Ativo",
        XFAiScheduledJobStatus.Paused => "Pausado",
        XFAiScheduledJobStatus.Completed => "Concluido",
        XFAiScheduledJobStatus.Failed => "Falhou",
        XFAiScheduledJobStatus.Expired => "Expirado",
        _ => "—"
    };

    private static string GetFrequencyColor(XFAiScheduleFrequency freq) => freq switch
    {
        XFAiScheduleFrequency.Once => "bg-slate-100 text-slate-600 dark:bg-slate-800 dark:text-slate-400",
        XFAiScheduleFrequency.Minutely => "bg-red-100 text-red-600",
        XFAiScheduleFrequency.Hourly => "bg-orange-100 text-orange-600",
        XFAiScheduleFrequency.Daily => "bg-blue-100 text-blue-600",
        XFAiScheduleFrequency.Weekly => "bg-indigo-100 text-indigo-600",
        XFAiScheduleFrequency.Monthly => "bg-purple-100 text-purple-600",
        XFAiScheduleFrequency.Custom => "bg-teal-100 text-teal-600",
        _ => "bg-slate-100 text-slate-500"
    };

    private static string GetFrequencyLabel(XFAiScheduleFrequency freq) => freq switch
    {
        XFAiScheduleFrequency.Once => "Unica",
        XFAiScheduleFrequency.Minutely => "Minuto",
        XFAiScheduleFrequency.Hourly => "Hora",
        XFAiScheduleFrequency.Daily => "Diario",
        XFAiScheduleFrequency.Weekly => "Semanal",
        XFAiScheduleFrequency.Monthly => "Mensal",
        XFAiScheduleFrequency.Custom => "Personalizado",
        _ => "—"
    };

    // ── Interactions ────────────────────────────────────────────────────

    private Task HandleAddJob() => OnAddJob.InvokeAsync();
    private Task HandlePauseJob(XFAiScheduledJob job) => OnPauseJob.InvokeAsync(job);
    private Task HandleResumeJob(XFAiScheduledJob job) => OnResumeJob.InvokeAsync(job);

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
