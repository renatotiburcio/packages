namespace XForge.DesignSystem.Components.AI.Stock;

public sealed class XFAiStockInsight
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string? NaturalLanguageQuery { get; set; }
    public string AiSummary { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public IReadOnlyList<XFAiStockForecast> Forecasts { get; set; } = Array.Empty<XFAiStockForecast>();
    public IReadOnlyList<XFAiStockAlert> Alerts { get; set; } = Array.Empty<XFAiStockAlert>();
    public IReadOnlyList<XFAiStockReorderSuggestion> ReorderSuggestions { get; set; } = Array.Empty<XFAiStockReorderSuggestion>();
    public XFAiAbcClassification? AbcClassification { get; set; }
    public XFAiStockSummary? Summary { get; set; }
}

public sealed class XFAiStockForecast
{
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal PredictedDemand30Days { get; set; }
    public decimal PredictedDemand90Days { get; set; }
    public int DaysUntilStockout { get; set; }
    public string TrendDirection { get; set; } = "stable";
    public string? SeasonalityNote { get; set; }
}

public sealed class XFAiStockAlert
{
    public XFAiStockAlertType Type { get; set; }
    public string Severity { get; set; } = "medium";
    public string ProductName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RecommendedAction { get; set; }
}

public sealed class XFAiStockReorderSuggestion
{
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public decimal SuggestedQuantity { get; set; }
    public decimal EstimatedCost { get; set; }
    public string? PreferredSupplier { get; set; }
    public int UrgencyDays { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public sealed class XFAiAbcClassification
{
    public IReadOnlyList<string> ClassAProducts { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ClassBProducts { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ClassCProducts { get; set; } = Array.Empty<string>();
    public decimal ClassAPercentage { get; set; }
    public decimal ClassBPercentage { get; set; }
    public decimal ClassCPercentage { get; set; }
}

public sealed class XFAiStockSummary
{
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OverstockProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public double AverageTurnoverDays { get; set; }
    public decimal WastedStockPercentage { get; set; }
}

public enum XFAiStockAlertType
{
    RuptureRisk,
    Overstock,
    LowMargin,
    SlowMoving,
    SeasonalDemand,
    SupplierIssue
}