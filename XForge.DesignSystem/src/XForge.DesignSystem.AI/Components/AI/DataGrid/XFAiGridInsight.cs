namespace XForge.DesignSystem.Components.AI.DataGrid;

/// <summary>
/// Represents an AI-generated insight derived from grid data analysis.
/// Insights include patterns, anomalies, KPIs, trends, summaries, and recommendations.
/// </summary>
public sealed class XFAiGridInsight
{
    /// <summary>Unique identifier for the insight.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Short title displayed as the insight heading.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Longer description explaining the insight.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Insight category that determines visual styling.</summary>
    public XFAiGridInsightType Type { get; set; }

    /// <summary>AI confidence score between 0.0 and 1.0.</summary>
    public double? Confidence { get; set; }

    /// <summary>Optional action button label (e.g., "Aplicar filtro", "Ver detalhes").</summary>
    public string? ActionLabel { get; set; }

    /// <summary>Arbitrary key-value metadata for extensibility.</summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Determines the visual style and semantic meaning of an <see cref="XFAiGridInsight"/>.
/// </summary>
public enum XFAiGridInsightType
{
    /// <summary>Data pattern detected by AI — blue accent.</summary>
    Pattern,

    /// <summary>Data anomaly or outlier — red accent.</summary>
    Anomaly,

    /// <summary>Key performance indicator — emerald accent.</summary>
    Kpi,

    /// <summary>Data trend over time — purple accent.</summary>
    Trend,

    /// <summary>Data summary or aggregation — slate accent.</summary>
    Summary,

    /// <summary>AI recommendation or action — amber accent.</summary>
    Recommendation
}