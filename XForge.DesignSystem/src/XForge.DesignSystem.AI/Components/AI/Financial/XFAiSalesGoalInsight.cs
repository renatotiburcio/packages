namespace XForge.DesignSystem.Components.AI.Financial;

public sealed class XFAiSalesGoalInsight
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string? NaturalLanguageQuery { get; set; }
    public string AiSummary { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public IReadOnlyList<XFAiSalespersonRanking> Rankings { get; set; } = Array.Empty<XFAiSalespersonRanking>();
    public IReadOnlyList<XFAiGoalProgress> Goals { get; set; } = Array.Empty<XFAiGoalProgress>();
    public IReadOnlyList<XFAiSalesRecommendation> Recommendations { get; set; } = Array.Empty<XFAiSalesRecommendation>();
    public XFAiSalesPerformanceSummary? PerformanceSummary { get; set; }
}

public sealed class XFAiSalespersonRanking
{
    public int Position { get; set; }
    public string SalespersonName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal GoalAmount { get; set; }
    public double AchievementPercentage { get; set; }
    public decimal Commission { get; set; }
    public string? Badge { get; set; }
}

public sealed class XFAiGoalProgress
{
    public string GoalName { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public double ProgressPercentage { get; set; }
    public int DaysRemaining { get; set; }
    public string PredictedOutcome { get; set; } = string.Empty;
    public decimal PredictedFinalAmount { get; set; }
}

public sealed class XFAiSalesRecommendation
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "action";
    public string? ProductMix { get; set; }
    public decimal? EstimatedImpact { get; set; }
}

public sealed class XFAiSalesPerformanceSummary
{
    public decimal TotalRevenue { get; set; }
    public decimal AverageTicket { get; set; }
    public int TotalTransactions { get; set; }
    public double GrowthRate { get; set; }
    public string BestPerformingRegion { get; set; } = string.Empty;
    public string BestPerformingProduct { get; set; } = string.Empty;
}
