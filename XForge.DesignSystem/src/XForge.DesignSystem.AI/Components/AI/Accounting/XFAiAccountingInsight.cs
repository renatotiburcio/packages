namespace XForge.DesignSystem.Components.AI.Accounting;

public sealed class XFAiAccountingInsight
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string? NaturalLanguageQuery { get; set; }
    public string AiSummary { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public IReadOnlyList<XFAiSuggestedEntry> SuggestedEntries { get; set; } = Array.Empty<XFAiSuggestedEntry>();
    public IReadOnlyList<XFAiAccountingClassification> Classifications { get; set; } = Array.Empty<XFAiAccountingClassification>();
    public IReadOnlyList<XFAiAccountingAuditItem> AuditItems { get; set; } = Array.Empty<XFAiAccountingAuditItem>();
    public XFAiDreSummary? DreSummary { get; set; }
}

public sealed class XFAiSuggestedEntry
{
    public string Description { get; set; } = string.Empty;
    public string DebitAccount { get; set; } = string.Empty;
    public string CreditAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CostCenter { get; set; }
    public string? Historical { get; set; }
    public double Confidence { get; set; }
}

public sealed class XFAiAccountingClassification
{
    public string TransactionDescription { get; set; } = string.Empty;
    public string SuggestedAccount { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string ClassificationType { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

public sealed class XFAiAccountingAuditItem
{
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = "medium";
    public string Description { get; set; } = string.Empty;
    public string? Suggestion { get; set; }
}

public sealed class XFAiDreSummary
{
    public decimal Revenue { get; set; }
    public decimal CostOfGoods { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal OperatingExpenses { get; set; }
    public decimal OperatingIncome { get; set; }
    public decimal NetIncome { get; set; }
    public double GrossMargin { get; set; }
    public double NetMargin { get; set; }
    public string AiAnalysis { get; set; } = string.Empty;
}
