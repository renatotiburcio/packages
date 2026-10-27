namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Risk severity levels for XFAiRiskBadge.
/// </summary>
public enum XFAiRiskLevel
{
    /// <summary>Low risk — green accent.</summary>
    Low,

    /// <summary>Medium risk — yellow accent.</summary>
    Medium,

    /// <summary>High risk — orange accent.</summary>
    High,

    /// <summary>Critical risk — red accent.</summary>
    Critical
}

/// <summary>
/// Business context for risk badge coloring and labeling.
/// </summary>
public enum XFAiRiskContext
{
    /// <summary>General purpose risk.</summary>
    General,

    /// <summary>Credit risk assessment.</summary>
    Credit,

    /// <summary>Fiscal/tax risk assessment.</summary>
    Fiscal,

    /// <summary>Stock/inventory risk assessment.</summary>
    Stock,

    /// <summary>Fraud risk assessment.</summary>
    Fraud,

    /// <summary>Delinquency/default risk assessment.</summary>
    Delinquency
}
