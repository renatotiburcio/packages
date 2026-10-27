namespace XForge.DesignSystem.Components.AI.Financial;

public sealed class XFAiReconciliationResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string? NaturalLanguageQuery { get; set; }
    public string AiSummary { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public IReadOnlyList<XFAiReconciliationMatch> Matches { get; set; } = Array.Empty<XFAiReconciliationMatch>();
    public IReadOnlyList<XFAiReconciliationUnmatched> UnmatchedTransactions { get; set; } = Array.Empty<XFAiReconciliationUnmatched>();
    public IReadOnlyList<XFAiDuplicateDetection> Duplicates { get; set; } = Array.Empty<XFAiDuplicateDetection>();
    public IReadOnlyList<XFAiFraudIndicator> FraudIndicators { get; set; } = Array.Empty<XFAiFraudIndicator>();
    public XFAiCashflowSummary? CashflowSummary { get; set; }
}

public sealed class XFAiReconciliationMatch
{
    public string TransactionDescription { get; set; } = string.Empty;
    public string BankDescription { get; set; } = string.Empty;
    public decimal TransactionAmount { get; set; }
    public decimal BankAmount { get; set; }
    public double MatchScore { get; set; }
    public string MatchType { get; set; } = "exact";
}

public sealed class XFAiReconciliationUnmatched
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? PossibleReason { get; set; }
}

public sealed class XFAiDuplicateDetection
{
    public string Description1 { get; set; } = string.Empty;
    public string Description2 { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Similarity { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

public sealed class XFAiFraudIndicator
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "medium";
    public decimal? Amount { get; set; }
    public string? Recommendation { get; set; }
}

public sealed class XFAiCashflowSummary
{
    public decimal TotalInflow { get; set; }
    public decimal TotalOutflow { get; set; }
    public decimal NetCashflow { get; set; }
    public decimal MatchedPercentage { get; set; }
    public int UnmatchedCount { get; set; }
}
