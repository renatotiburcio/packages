using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Performance;

/// <summary>
/// WASM memory profiler with gauge visualization, zone breakdown,
/// GC collection stats, and memory history chart. Pure Blazor, no JS interop.
/// </summary>
public partial class XFAiMemoryProfiler : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Memory Profiler";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Perfilador de memoria WASM";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the footer bar.</summary>
    [Parameter]
    public bool ShowFooter { get; set; } = true;

    /// <summary>Whether to show the circular gauge.</summary>
    [Parameter]
    public bool ShowGauge { get; set; } = true;

    /// <summary>Whether to show memory zone breakdown.</summary>
    [Parameter]
    public bool ShowZones { get; set; } = true;

    /// <summary>Whether to show GC collection stats.</summary>
    [Parameter]
    public bool ShowGCStats { get; set; } = true;

    /// <summary>Whether to show memory usage history chart.</summary>
    [Parameter]
    public bool ShowHistory { get; set; } = true;

    /// <summary>Maximum memory budget in MB.</summary>
    [Parameter]
    public double MaxUsageMB { get; set; } = 50;

    /// <summary>Memory snapshot data.</summary>
    [Parameter]
    public XFAiMemorySnapshot? Snapshot { get; set; }

    /// <summary>Fired when a snapshot is captured.</summary>
    [Parameter]
    public EventCallback<XFAiMemorySnapshot> OnSnapshotCaptured { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFAiMemorySnapshot? _snapshot;
    private double _currentUsageMB;
    private double _maxUsageMB;
    private double _usagePercent;
    private int _gen0Collections;
    private int _gen1Collections;
    private int _gen2Collections;
    private readonly List<double> _history = new();
    private const double _circumference = 2 * Math.PI * 50; // r=50

    private double _dashOffset => _circumference * (1 - _usagePercent / 100);

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (Snapshot is not null)
        {
            _snapshot = Snapshot;
            UpdateFromSnapshot();
        }
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Loads a memory snapshot.</summary>
    public void LoadSnapshot(XFAiMemorySnapshot snapshot)
    {
        _snapshot = snapshot;
        UpdateFromSnapshot();
        StateHasChanged();
    }

    /// <summary>Captures a synthetic snapshot for testing.</summary>
    public async Task CaptureSnapshotAsync()
    {
        var snapshot = new XFAiMemorySnapshot
        {
            TotalManagedBytes = GC.GetTotalMemory(false),
            TotalAllocatedBytes = GC.GetTotalAllocatedBytes(),
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
            Allocations = new List<XFAiMemoryAllocation>
            {
                new() { Zone = XFAiMemoryZone.Managed, AllocatedBytes = GC.GetTotalMemory(false), MaxBytes = (long)(MaxUsageMB * 1024 * 1024), Label = "Managed Heap" },
                new() { Zone = XFAiMemoryZone.Components, AllocatedBytes = Random.Shared.Next(1, 10) * 1024 * 1024, MaxBytes = (long)(MaxUsageMB * 1024 * 1024 * 0.5), Label = "Componentes" },
                new() { Zone = XFAiMemoryZone.Cache, AllocatedBytes = Random.Shared.Next(1, 5) * 1024 * 1024, MaxBytes = (long)(MaxUsageMB * 1024 * 1024 * 0.3), Label = "Cache" },
            }
        };

        LoadSnapshot(snapshot);

        if (OnSnapshotCaptured.HasDelegate)
            await OnSnapshotCaptured.InvokeAsync(snapshot);
    }

    // ── Internal ────────────────────────────────────────────────────────

    private void UpdateFromSnapshot()
    {
        if (_snapshot is null) return;

        _currentUsageMB = _snapshot.TotalAllocatedBytes / (1024.0 * 1024.0);
        _maxUsageMB = MaxUsageMB;
        _usagePercent = _maxUsageMB > 0 ? Math.Min(100, _currentUsageMB / _maxUsageMB * 100) : 0;
        _gen0Collections = _snapshot.Gen0Collections;
        _gen1Collections = _snapshot.Gen1Collections;
        _gen2Collections = _snapshot.Gen2Collections;

        _history.Add(_currentUsageMB);
        if (_history.Count > 50) _history.RemoveAt(0);
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private string GetUsageBadgeClass()
    {
        if (_usagePercent < 60) return "rounded-full bg-emerald-100 px-2 py-0.5 text-xs text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300";
        if (_usagePercent < 80) return "rounded-full bg-amber-100 px-2 py-0.5 text-xs text-amber-700 dark:bg-amber-900/30 dark:text-amber-300";
        return "rounded-full bg-red-100 px-2 py-0.5 text-xs text-red-700 dark:bg-red-900/30 dark:text-red-300";
    }

    private string GetGaugeColor()
    {
        if (_usagePercent < 60) return "text-emerald-500";
        if (_usagePercent < 80) return "text-amber-500";
        return "text-red-500";
    }

    private static string GetZoneColor(XFAiMemoryZone zone) => zone switch
    {
        XFAiMemoryZone.Managed => "bg-blue-500",
        XFAiMemoryZone.Unmanaged => "bg-purple-500",
        XFAiMemoryZone.Interop => "bg-amber-500",
        XFAiMemoryZone.Components => "bg-emerald-500",
        XFAiMemoryZone.Services => "bg-cyan-500",
        XFAiMemoryZone.Cache => "bg-pink-500",
        _ => "bg-slate-400"
    };

    private static string GetZoneLabel(XFAiMemoryZone zone) => zone switch
    {
        XFAiMemoryZone.Managed => "Gerenciado",
        XFAiMemoryZone.Unmanaged => "Nao gerenc.",
        XFAiMemoryZone.Interop => "Interop",
        XFAiMemoryZone.Components => "Componentes",
        XFAiMemoryZone.Services => "Servicos",
        XFAiMemoryZone.Cache => "Cache",
        _ => "Outros"
    };

    private static string FormatBytes(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _ => $"{bytes / (1024.0 * 1024.0):F2} MB"
    };

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-memory-profiler overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
