using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.AI.Performance;

/// <summary>
/// Real-time render performance monitor. Tracks component render times,
/// displays metrics cards, sparklines, timeline bars, and benchmark results.
/// Pure Blazor, no JS interop.
/// </summary>
public partial class XFAiPerformanceMonitor : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Performance Monitor";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Monitor de performance em tempo real";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the footer bar.</summary>
    [Parameter]
    public bool ShowFooter { get; set; } = true;

    /// <summary>Whether to show metric cards.</summary>
    [Parameter]
    public bool ShowMetrics { get; set; } = true;

    /// <summary>Whether to show the timeline chart.</summary>
    [Parameter]
    public bool ShowTimeline { get; set; } = true;

    /// <summary>Whether to show sparklines in metric cards.</summary>
    [Parameter]
    public bool ShowSparkline { get; set; } = true;

    /// <summary>Whether to show benchmark results.</summary>
    [Parameter]
    public bool ShowBenchmarks { get; set; } = true;

    /// <summary>Monitoring mode.</summary>
    [Parameter]
    public XFAiMonitorMode Mode { get; set; } = XFAiMonitorMode.Realtime;

    /// <summary>Initial metrics to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiMetricDataPoint> Metrics { get; set; } = Array.Empty<XFAiMetricDataPoint>();

    /// <summary>Benchmark targets to evaluate.</summary>
    [Parameter]
    public IReadOnlyList<XFAiBenchmarkTarget> BenchmarkTargets { get; set; } = Array.Empty<XFAiBenchmarkTarget>();

    /// <summary>Maximum render time for 60fps benchmark.</summary>
    [Parameter]
    public double MaxRenderTimeMs { get; set; } = 16;

    /// <summary>Number of metric columns in the grid.</summary>
    [Parameter]
    public int GridCols { get; set; } = 4;

    /// <summary>Timeline bar height in pixels.</summary>
    [Parameter]
    public int BarHeight { get; set; } = 48;

    /// <summary>Fired when a snapshot is recorded.</summary>
    [Parameter]
    public EventCallback<XFAiRenderSnapshot> OnSnapshotRecorded { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private readonly List<XFAiRenderSnapshot> _snapshots = new();
    private readonly List<XFAiMetricDataPoint> _metrics = new();
    private readonly List<XFAiBenchmarkResult> _benchmarkResults = new();
    private int _displayCount = 20;

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (Metrics.Count > 0 && _metrics.Count == 0)
        {
            _metrics.AddRange(Metrics);
        }
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Records a render snapshot.</summary>
    public async Task RecordSnapshotAsync(XFAiRenderSnapshot snapshot)
    {
        _snapshots.Add(snapshot);

        // Update render time metric
        var existing = _metrics.FindIndex(m => m.Label == "Render Time");
        var point = new XFAiMetricDataPoint
        {
            Label = "Render Time",
            Value = snapshot.RenderTimeMs,
            Unit = "ms",
            Severity = snapshot.RenderTimeMs < 16 ? XFSeverity.Good :
                       snapshot.RenderTimeMs < 50 ? XFSeverity.Warning :
                       XFSeverity.Critical
        };

        if (existing >= 0)
            _metrics[existing] = point;
        else
            _metrics.Add(point);

        if (OnSnapshotRecorded.HasDelegate)
            await OnSnapshotRecorded.InvokeAsync(snapshot);

        StateHasChanged();
    }

    /// <summary>Adds a custom metric.</summary>
    public void AddMetric(XFAiMetricDataPoint metric)
    {
        var existing = _metrics.FindIndex(m => m.Label == metric.Label);
        if (existing >= 0)
            _metrics[existing] = metric;
        else
            _metrics.Add(metric);
        StateHasChanged();
    }

    /// <summary>Runs benchmark evaluation against targets.</summary>
    public void RunBenchmarks(IReadOnlyDictionary<string, double> measuredValues)
    {
        _benchmarkResults.Clear();
        foreach (var target in BenchmarkTargets)
        {
            if (!measuredValues.TryGetValue(target.Name, out var measured))
                continue;

            var passed = target.Operator switch
            {
                XFAiBenchmarkOperator.LessThan => measured < target.ThresholdMs,
                XFAiBenchmarkOperator.LessThanOrEqual => measured <= target.ThresholdMs,
                XFAiBenchmarkOperator.GreaterThan => measured > target.ThresholdMs,
                XFAiBenchmarkOperator.GreaterThanOrEqual => measured >= target.ThresholdMs,
                XFAiBenchmarkOperator.Equal => Math.Abs(measured - target.ThresholdMs) < 0.001,
                _ => false
            };

            _benchmarkResults.Add(new XFAiBenchmarkResult
            {
                Name = target.Name,
                MeasuredValue = measured,
                ThresholdValue = target.ThresholdMs,
                Operator = target.Operator,
                Passed = passed,
                Unit = target.Unit
            });
        }
        StateHasChanged();
    }

    /// <summary>Clears all recorded snapshots.</summary>
    public void ClearSnapshots()
    {
        _snapshots.Clear();
        StateHasChanged();
    }

    /// <summary>Gets the average render time.</summary>
    public double GetAverageRenderTime()
    {
        return _snapshots.Count == 0 ? 0 : _snapshots.Average(s => s.RenderTimeMs);
    }

    /// <summary>Gets the P95 render time.</summary>
    public double GetP95RenderTime()
    {
        if (_snapshots.Count == 0) return 0;
        var sorted = _snapshots.OrderBy(s => s.RenderTimeMs).ToArray();
        var index = (int)Math.Ceiling(sorted.Length * 0.95) - 1;
        return sorted[Math.Max(0, index)].RenderTimeMs;
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private string GetOverallBadgeClass()
    {
        var avg = GetAverageRenderTime();
        if (avg < 16) return "rounded-full bg-emerald-100 px-2 py-0.5 text-xs text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300";
        if (avg < 50) return "rounded-full bg-amber-100 px-2 py-0.5 text-xs text-amber-700 dark:bg-amber-900/30 dark:text-amber-300";
        return "rounded-full bg-red-100 px-2 py-0.5 text-xs text-red-700 dark:bg-red-900/30 dark:text-red-300";
    }

    private string GetOverallLabel()
    {
        var avg = GetAverageRenderTime();
        if (_snapshots.Count == 0) return "Sem dados";
        if (avg < 16) return "60fps OK";
        if (avg < 50) return "Lento";
        return "Critico";
    }

    private string GetMetricCardClass(XFSeverity severity) => severity switch
    {
        XFSeverity.Good => "flex flex-col gap-1 rounded-lg border border-emerald-200 bg-emerald-50/50 p-3 dark:border-emerald-800 dark:bg-emerald-900/20",
        XFSeverity.Warning => "flex flex-col gap-1 rounded-lg border border-amber-200 bg-amber-50/50 p-3 dark:border-amber-800 dark:bg-amber-900/20",
        XFSeverity.Critical => "flex flex-col gap-1 rounded-lg border border-red-200 bg-red-50/50 p-3 dark:border-red-800 dark:bg-red-900/20",
        _ => "flex flex-col gap-1 rounded-lg border border-border-light bg-bg-surface p-3"
    };

    private string GetMetricValueClass(XFSeverity severity) => severity switch
    {
        XFSeverity.Good => "text-lg font-bold text-emerald-700 dark:text-emerald-300",
        XFSeverity.Warning => "text-lg font-bold text-amber-700 dark:text-amber-300",
        XFSeverity.Critical => "text-lg font-bold text-red-700 dark:text-red-300",
        _ => "text-lg font-bold text-text-primary"
    };

    private string GetSparklineColor(XFSeverity severity) => severity switch
    {
        XFSeverity.Good => "text-emerald-500",
        XFSeverity.Warning => "text-amber-500",
        XFSeverity.Critical => "text-red-500",
        _ => "text-primary"
    };

    private string GetSparklinePoints(string metricLabel)
    {
        var recentSnapshots = _snapshots.TakeLast(10).ToArray();
        if (recentSnapshots.Length < 2) return "0,10 100,10";

        var values = recentSnapshots.Select(s => s.RenderTimeMs).ToArray();
        var max = values.Max();
        var step = 100.0 / (values.Length - 1);

        return string.Join(" ", values.Select((v, i) =>
        {
            var x = i * step;
            var y = max > 0 ? 20 - (v / max * 20) : 10;
            return $"{x.ToString("F0")},{y.ToString("F0")}";
        }));
    }

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-performance-monitor overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
