namespace XForge.DesignSystem.Components.AI.Insights;

/// <summary>
/// Represents a KPI data point for AI dashboard analysis.
/// </summary>
public sealed class XFAiDashboardKpi
{
    /// <summary>Internal identifier for the KPI.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Display name shown in the dashboard.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Current value of the KPI.</summary>
    public double CurrentValue { get; set; }

    /// <summary>Previous period value for trend/change calculation.</summary>
    public double? PreviousValue { get; set; }

    /// <summary>Unit of measurement (e.g., "%", "R$", "unidades").</summary>
    public string? Unit { get; set; }

    /// <summary>Category for grouping KPIs (e.g., "Financeiro", "Vendas").</summary>
    public string? Category { get; set; }

    /// <summary>Target value for the KPI.</summary>
    public double? Target { get; set; }

    /// <summary>Arbitrary key-value metadata for extensibility.</summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Represents an AI-generated insight about dashboard KPIs.
/// </summary>
public sealed class XFAiDashboardInsightItem
{
    /// <summary>Unique identifier for the insight.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Short title displayed as the insight heading.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Detailed description of the insight.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Insight category that determines visual styling.</summary>
    public XFAiDashboardInsightType Type { get; set; }

    /// <summary>Severity level that determines icon and urgency.</summary>
    public XFAiDashboardInsightSeverity Severity { get; set; }

    /// <summary>AI confidence score between 0.0 and 1.0.</summary>
    public double? Confidence { get; set; }

    /// <summary>Name of the related KPI.</summary>
    public string? RelatedKpi { get; set; }

    /// <summary>Percentage change detected (positive or negative).</summary>
    public double? ChangePercent { get; set; }

    /// <summary>Label for the action button.</summary>
    public string? ActionLabel { get; set; }

    /// <summary>AI recommendation text.</summary>
    public string? Recommendation { get; set; }

    /// <summary>Arbitrary key-value metadata for extensibility.</summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Determines the visual style and semantic meaning of a dashboard insight.
/// </summary>
public enum XFAiDashboardInsightType
{
    /// <summary>Trend detection — purple accent.</summary>
    Trend,

    /// <summary>Outlier detection — red accent.</summary>
    Outlier,

    /// <summary>Forecast or prediction — blue accent.</summary>
    Forecast,

    /// <summary>Alert or warning — amber accent.</summary>
    Alert,

    /// <summary>Optimization or recommendation — emerald accent.</summary>
    Recommendation,

    /// <summary>Anomaly detection — orange accent.</summary>
    Anomaly
}

/// <summary>
/// Determines the severity and icon of a dashboard insight.
/// </summary>
public enum XFAiDashboardInsightSeverity
{
    /// <summary>Informational — circle-info icon.</summary>
    Info,

    /// <summary>Warning — triangle-exclamation icon.</summary>
    Warning,

    /// <summary>Critical — circle-exclamation icon.</summary>
    Critical,

    /// <summary>Success — circle-check icon.</summary>
    Success
}
