namespace XForge.DesignSystem.Core.Primitives;

/// <summary>
/// Unified severity level enum for AI risk assessment, dashboard insights,
/// and performance metrics. Replaces XFAiRiskLevel, XFAiDashboardInsightSeverity,
/// and XFAiMetricSeverity.
/// </summary>
public enum XFSeverity
{
    /// <summary>Informational — no action required.</summary>
    Info,

    /// <summary>Success — positive outcome.</summary>
    Success,

    /// <summary>Good — within acceptable range.</summary>
    Good,

    /// <summary>Warning — attention needed.</summary>
    Warning,

    /// <summary>Low risk — minimal concern.</summary>
    Low,

    /// <summary>Medium risk — moderate concern.</summary>
    Medium,

    /// <summary>High risk — significant concern.</summary>
    High,

    /// <summary>Critical — immediate action required.</summary>
    Critical
}
