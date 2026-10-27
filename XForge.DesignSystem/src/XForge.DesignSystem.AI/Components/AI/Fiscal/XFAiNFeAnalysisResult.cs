namespace XForge.DesignSystem.Components.AI.Fiscal;

/// <summary>
/// Represents the complete analysis result of an NF-e XML document.
/// </summary>
public sealed class XFAiNFeAnalysisResult
{
    /// <summary>Unique identifier for this analysis.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>NFe access key (44 digits).</summary>
    public string? NFeKey { get; set; }

    /// <summary>Issuer CNPJ.</summary>
    public string? IssuerCnpj { get; set; }

    /// <summary>Issuer name (razao social).</summary>
    public string? IssuerName { get; set; }

    /// <summary>Recipient CNPJ.</summary>
    public string? RecipientCnpj { get; set; }

    /// <summary>Recipient name.</summary>
    public string? RecipientName { get; set; }

    /// <summary>Invoice number.</summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>Invoice series.</summary>
    public string? Series { get; set; }

    /// <summary>Issue date.</summary>
    public string? IssueDate { get; set; }

    /// <summary>Total invoice value.</summary>
    public decimal TotalValue { get; set; }

    /// <summary>ICMS total tax value.</summary>
    public decimal IcmsTotal { get; set; }

    /// <summary>IPI total tax value.</summary>
    public decimal IpiTotal { get; set; }

    /// <summary>PIS total tax value.</summary>
    public decimal PisTotal { get; set; }

    /// <summary>COFINS total tax value.</summary>
    public decimal CofinsTotal { get; set; }

    /// <summary>Invoice items parsed from XML.</summary>
    public IReadOnlyList<XFAiNFeItem> Items { get; set; } = Array.Empty<XFAiNFeItem>();

    /// <summary>Tax summary by CFOP.</summary>
    public IReadOnlyList<XFAiNFeTaxSummary> TaxSummary { get; set; } = Array.Empty<XFAiNFeTaxSummary>();

    /// <summary>Detected inconsistencies and issues.</summary>
    public IReadOnlyList<XFAiNFeInconsistency> Inconsistencies { get; set; } = Array.Empty<XFAiNFeInconsistency>();

    /// <summary>AI-generated analysis summary.</summary>
    public string? AiSummary { get; set; }

    /// <summary>SPED validation status.</summary>
    public XFAiNFeSpedStatus SpedStatus { get; set; } = XFAiNFeSpedStatus.NotValidated;

    /// <summary>CBS/IBS readiness assessment.</summary>
    public string? CbsIbsReadiness { get; set; }

    /// <summary>Overall risk level.</summary>
    public XFAiNFeRiskLevel RiskLevel { get; set; } = XFAiNFeRiskLevel.Low;

    /// <summary>AI confidence score (0.0–1.0).</summary>
    public double Confidence { get; set; }

    /// <summary>Original natural language query.</summary>
    public string? NaturalLanguageQuery { get; set; }
}

/// <summary>
/// Represents a single item (product/service) in an NF-e.
/// </summary>
public sealed class XFAiNFeItem
{
    public int Sequence { get; set; }
    public string? Description { get; set; }
    public string? Ncm { get; set; }
    public string? Cfop { get; set; }
    public string? Cst { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitValue { get; set; }
    public decimal TotalValue { get; set; }
    public decimal IcmsValue { get; set; }
    public decimal IpiValue { get; set; }
    public string? ItemClassification { get; set; }
}

/// <summary>
/// Tax summary grouped by CFOP code.
/// </summary>
public sealed class XFAiNFeTaxSummary
{
    public string Cfop { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal TotalTax { get; set; }
    public double EffectiveRate { get; set; }
}

/// <summary>
/// Represents an inconsistency or issue detected during NFe analysis.
/// </summary>
public sealed class XFAiNFeInconsistency
{
    public XFAiNFeInconsistencySeverity Severity { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Field { get; set; }
    public string? Suggestion { get; set; }
}

/// <summary>Severity of an NFe inconsistency.</summary>
public enum XFAiNFeInconsistencySeverity
{
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>SPED validation status.</summary>
public enum XFAiNFeSpedStatus
{
    NotValidated,
    Valid,
    WithWarnings,
    Invalid
}

/// <summary>Risk level for NFe analysis.</summary>
public enum XFAiNFeRiskLevel
{
    Low,
    Medium,
    High,
    Critical
}
