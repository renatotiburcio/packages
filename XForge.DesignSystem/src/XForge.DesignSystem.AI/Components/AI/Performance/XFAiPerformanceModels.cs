using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.AI.Performance;

// ── Enums ──────────────────────────────────────────────────────────────────

/// <summary>Virtual scroller rendering strategy.</summary>
public enum XFAiVirtualRenderStrategy
{
    FixedHeight,
    DynamicHeight,
    Grid
}

/// <summary>Lazy component loading state.</summary>
public enum XFAiLazyLoadState
{
    NotLoaded,
    Loading,
    Loaded,
    Error,
    Timeout
}

/// <summary>Performance metric severity level.</summary>
[Obsolete("Use XFSeverity from XForge.DesignSystem.Core.Primitives instead. This enum will be removed in v2.0.")]
public enum XFAiMetricSeverity
{
    Good,
    Warning,
    Critical
}

/// <summary>Bundle module category.</summary>
public enum XFAiBundleCategory
{
    Components,
    Services,
    Styles,
    Scripts,
    Icons,
    Fonts,
    ThirdParty,
    Other
}

/// <summary>Memory allocation zone.</summary>
public enum XFAiMemoryZone
{
    Managed,
    Unmanaged,
    Interop,
    Components,
    Services,
    Cache,
    Other
}

/// <summary>Performance monitoring mode.</summary>
public enum XFAiMonitorMode
{
    Realtime,
    Summary,
    Detailed
}

/// <summary>Chart type for performance visualization.</summary>
public enum XFAiChartType
{
    Bar,
    Line,
    Pie,
    Heatmap,
    Sparkline
}

/// <summary>Benchmark comparison operator.</summary>
public enum XFAiBenchmarkOperator
{
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    Equal
}

// ── Records ────────────────────────────────────────────────────────────────

/// <summary>A virtual scroll range (visible window of items).</summary>
public record XFAiVirtualRange
{
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
    public int Count => EndIndex - StartIndex + 1;
    public double OffsetTop { get; init; }
    public double TotalHeight { get; init; }
}

/// <summary>A rendered item in the virtual scroller.</summary>
public record XFAiVirtualItem<T>
{
    public T Data { get; init; } = default!;
    public int Index { get; init; }
    public double Top { get; init; }
    public double Height { get; init; }
    public bool IsVisible { get; init; }
}

/// <summary>A performance metric data point.</summary>
public record XFAiMetricDataPoint
{
    public string Label { get; init; } = "";
    public double Value { get; init; }
    public string Unit { get; init; } = "ms";
    public XFSeverity Severity { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

/// <summary>A render performance snapshot.</summary>
public record XFAiRenderSnapshot
{
    public string ComponentName { get; init; } = "";
    public double RenderTimeMs { get; init; }
    public int ParametersChanged { get; init; }
    public int ChildrenRendered { get; init; }
    public DateTime CapturedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>A bundle module entry.</summary>
public record XFAiBundleModule
{
    public string Name { get; init; } = "";
    public string Path { get; init; } = "";
    public long SizeBytes { get; init; }
    public long GzipSizeBytes { get; init; }
    public XFAiBundleCategory Category { get; init; }
    public bool IsTreeShakeable { get; init; }
    public double Percentage { get; init; }
}

/// <summary>Bundle analysis summary.</summary>
public record XFAiBundleSummary
{
    public long TotalSizeBytes { get; init; }
    public long TotalGzipSizeBytes { get; init; }
    public int ModuleCount { get; init; }
    public int TreeShakeableCount { get; init; }
    public IReadOnlyList<XFAiBundleModule> Modules { get; init; } = Array.Empty<XFAiBundleModule>();
    public IReadOnlyList<XFAiBundleCategoryBreakdown> Breakdown { get; init; } = Array.Empty<XFAiBundleCategoryBreakdown>();
}

/// <summary>Bundle category breakdown.</summary>
public record XFAiBundleCategoryBreakdown
{
    public XFAiBundleCategory Category { get; init; }
    public long SizeBytes { get; init; }
    public double Percentage { get; init; }
    public int ModuleCount { get; init; }
}

/// <summary>Memory allocation entry.</summary>
public record XFAiMemoryAllocation
{
    public XFAiMemoryZone Zone { get; init; }
    public long AllocatedBytes { get; init; }
    public long MaxBytes { get; init; }
    public double UsagePercent => MaxBytes > 0 ? (double)AllocatedBytes / MaxBytes * 100 : 0;
    public string Label { get; init; } = "";
}

/// <summary>Memory profiling snapshot.</summary>
public record XFAiMemorySnapshot
{
    public long TotalManagedBytes { get; init; }
    public long TotalAllocatedBytes { get; init; }
    public int Gen0Collections { get; init; }
    public int Gen1Collections { get; init; }
    public int Gen2Collections { get; init; }
    public IReadOnlyList<XFAiMemoryAllocation> Allocations { get; init; } = Array.Empty<XFAiMemoryAllocation>();
    public DateTime CapturedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>A lazy component placeholder configuration.</summary>
public record XFAiLazyConfig
{
    public string PlaceholderText { get; init; } = "Carregando...";
    public int TimeoutMs { get; init; } = 30000;
    public bool ShowProgress { get; init; }
    public string? ErrorText { get; init; }
}

/// <summary>A benchmark target definition.</summary>
public record XFAiBenchmarkTarget
{
    public string Name { get; init; } = "";
    public double ThresholdMs { get; init; }
    public XFAiBenchmarkOperator Operator { get; init; } = XFAiBenchmarkOperator.LessThanOrEqual;
    public string Unit { get; init; } = "ms";
}

/// <summary>A benchmark result.</summary>
public record XFAiBenchmarkResult
{
    public string Name { get; init; } = "";
    public double MeasuredValue { get; init; }
    public double ThresholdValue { get; init; }
    public XFAiBenchmarkOperator Operator { get; init; }
    public bool Passed { get; init; }
    public string Unit { get; init; } = "ms";
}
