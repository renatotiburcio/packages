using Xunit;
using XForge.DesignSystem.Components.AI.Accounting;
using XForge.DesignSystem.Components.AI.Audio;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.Financial;
using XForge.DesignSystem.Components.AI.Fiscal;
using XForge.DesignSystem.Components.AI.Forms;
using XForge.DesignSystem.Components.AI.MultiModal;
using XForge.DesignSystem.Components.AI.Stock;
using XForge.DesignSystem.Components.AI.Utility;
using XForge.DesignSystem.Components.AI.Vision;

// Disambiguate: Common namespace types take precedence for DTOs used in AI domain tests
using AiEmbeddingRequest = XForge.DesignSystem.Services.AI.AiEmbeddingRequest;
using AiEmbeddingResponse = XForge.DesignSystem.Services.AI.AiEmbeddingResponse;
using AiHealthStatus = XForge.DesignSystem.Services.AI.AiHealthStatus;
using AiSpeechRequest = XForge.DesignSystem.Components.AI.Common.AiSpeechRequest;
using AiSpeechResponse = XForge.DesignSystem.Components.AI.Common.AiSpeechResponse;
using AiVisionRequest = XForge.DesignSystem.Components.AI.Common.AiVisionRequest;
using AiVisionResponse = XForge.DesignSystem.Components.AI.Common.AiVisionResponse;
using AiTranscriptionRequest = XForge.DesignSystem.Components.AI.Common.AiTranscriptionRequest;
using AiTranscriptionResponse = XForge.DesignSystem.Components.AI.Common.AiTranscriptionResponse;
using AiTranscriptionSegment = XForge.DesignSystem.Components.AI.Common.AiTranscriptionSegment;
using AiTranscriptionWord = XForge.DesignSystem.Components.AI.Common.AiTranscriptionWord;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for AI domain DTOs at 0% coverage: Accounting, Financial, Fiscal, Forms,
/// Stock, MultiModal, Utility, Vision, Audio, and Service DTOs.
/// These are sealed classes/records with property getters/setters — testing construction,
/// default values, property assignment, and collection behavior.
/// </summary>
public sealed class XFAiDomainDtosTest
{
    #region Accounting DTOs

    [Fact]
    public void XFAiAccountingInsight_Defaults_AreCorrect()
    {
        var insight = new XFAiAccountingInsight();
        Assert.NotEmpty(insight.Id);
        Assert.Equal(8, insight.Id.Length);
        Assert.Null(insight.NaturalLanguageQuery);
        Assert.Equal(string.Empty, insight.AiSummary);
        Assert.Equal(0, insight.Confidence);
        Assert.Empty(insight.SuggestedEntries);
        Assert.Empty(insight.Classifications);
        Assert.Empty(insight.AuditItems);
        Assert.Null(insight.DreSummary);
    }

    [Fact]
    public void XFAiAccountingInsight_PropertyAssignment_Works()
    {
        var entry = new XFAiSuggestedEntry();
        var classification = new XFAiAccountingClassification();
        var auditItem = new XFAiAccountingAuditItem();
        var dre = new XFAiDreSummary();

        var insight = new XFAiAccountingInsight
        {
            Id = "abc12345",
            NaturalLanguageQuery = "Show me revenue",
            AiSummary = "Revenue is up",
            Confidence = 0.95,
            SuggestedEntries = [entry],
            Classifications = [classification],
            AuditItems = [auditItem],
            DreSummary = dre
        };

        Assert.Equal("abc12345", insight.Id);
        Assert.Equal("Show me revenue", insight.NaturalLanguageQuery);
        Assert.Equal("Revenue is up", insight.AiSummary);
        Assert.Equal(0.95, insight.Confidence);
        Assert.Single(insight.SuggestedEntries);
        Assert.Single(insight.Classifications);
        Assert.Single(insight.AuditItems);
        Assert.Same(dre, insight.DreSummary);
    }

    [Fact]
    public void XFAiSuggestedEntry_Defaults_AreCorrect()
    {
        var entry = new XFAiSuggestedEntry();
        Assert.Equal(string.Empty, entry.Description);
        Assert.Equal(string.Empty, entry.DebitAccount);
        Assert.Equal(string.Empty, entry.CreditAccount);
        Assert.Equal(0m, entry.Amount);
        Assert.Null(entry.CostCenter);
        Assert.Null(entry.Historical);
        Assert.Equal(0, entry.Confidence);
    }

    [Fact]
    public void XFAiSuggestedEntry_PropertyAssignment_Works()
    {
        var entry = new XFAiSuggestedEntry
        {
            Description = "Sale of goods",
            DebitAccount = "1.1.3",
            CreditAccount = "3.1.1",
            Amount = 1500.50m,
            CostCenter = "CC01",
            Historical = "Similar to entry #123",
            Confidence = 0.89
        };
        Assert.Equal("Sale of goods", entry.Description);
        Assert.Equal("1.1.3", entry.DebitAccount);
        Assert.Equal("3.1.1", entry.CreditAccount);
        Assert.Equal(1500.50m, entry.Amount);
        Assert.Equal("CC01", entry.CostCenter);
        Assert.Equal("Similar to entry #123", entry.Historical);
        Assert.Equal(0.89, entry.Confidence);
    }

    [Fact]
    public void XFAiAccountingClassification_Defaults_AreCorrect()
    {
        var cls = new XFAiAccountingClassification();
        Assert.Equal(string.Empty, cls.TransactionDescription);
        Assert.Equal(string.Empty, cls.SuggestedAccount);
        Assert.Equal(string.Empty, cls.AccountName);
        Assert.Equal(string.Empty, cls.ClassificationType);
        Assert.Equal(0, cls.Confidence);
    }

    [Fact]
    public void XFAiAccountingClassification_PropertyAssignment_Works()
    {
        var cls = new XFAiAccountingClassification
        {
            TransactionDescription = "Invoice payment",
            SuggestedAccount = "1.1.1",
            AccountName = "Cash",
            ClassificationType = "Asset",
            Confidence = 0.92
        };
        Assert.Equal("Invoice payment", cls.TransactionDescription);
        Assert.Equal("1.1.1", cls.SuggestedAccount);
        Assert.Equal("Cash", cls.AccountName);
        Assert.Equal("Asset", cls.ClassificationType);
        Assert.Equal(0.92, cls.Confidence);
    }

    [Fact]
    public void XFAiAccountingAuditItem_Defaults_AreCorrect()
    {
        var item = new XFAiAccountingAuditItem();
        Assert.Equal(string.Empty, item.Type);
        Assert.Equal("medium", item.Severity);
        Assert.Equal(string.Empty, item.Description);
        Assert.Null(item.Suggestion);
    }

    [Fact]
    public void XFAiAccountingAuditItem_PropertyAssignment_Works()
    {
        var item = new XFAiAccountingAuditItem
        {
            Type = "MissingDocument",
            Severity = "high",
            Description = "No supporting document",
            Suggestion = "Upload the invoice"
        };
        Assert.Equal("MissingDocument", item.Type);
        Assert.Equal("high", item.Severity);
        Assert.Equal("No supporting document", item.Description);
        Assert.Equal("Upload the invoice", item.Suggestion);
    }

    [Fact]
    public void XFAiDreSummary_Defaults_AreCorrect()
    {
        var dre = new XFAiDreSummary();
        Assert.Equal(0m, dre.Revenue);
        Assert.Equal(0m, dre.CostOfGoods);
        Assert.Equal(0m, dre.GrossProfit);
        Assert.Equal(0m, dre.OperatingExpenses);
        Assert.Equal(0m, dre.OperatingIncome);
        Assert.Equal(0m, dre.NetIncome);
        Assert.Equal(0, dre.GrossMargin);
        Assert.Equal(0, dre.NetMargin);
        Assert.Equal(string.Empty, dre.AiAnalysis);
    }

    [Fact]
    public void XFAiDreSummary_PropertyAssignment_Works()
    {
        var dre = new XFAiDreSummary
        {
            Revenue = 100000m,
            CostOfGoods = 60000m,
            GrossProfit = 40000m,
            OperatingExpenses = 20000m,
            OperatingIncome = 20000m,
            NetIncome = 15000m,
            GrossMargin = 0.40,
            NetMargin = 0.15,
            AiAnalysis = "Healthy margins"
        };
        Assert.Equal(100000m, dre.Revenue);
        Assert.Equal(0.40, dre.GrossMargin);
        Assert.Equal(0.15, dre.NetMargin);
        Assert.Equal("Healthy margins", dre.AiAnalysis);
    }

    #endregion

    #region Financial DTOs

    [Fact]
    public void XFAiSalesGoalInsight_Defaults_AreCorrect()
    {
        var insight = new XFAiSalesGoalInsight();
        Assert.NotEmpty(insight.Id);
        Assert.Equal(8, insight.Id.Length);
        Assert.Null(insight.NaturalLanguageQuery);
        Assert.Equal(string.Empty, insight.AiSummary);
        Assert.Equal(0, insight.Confidence);
        Assert.Empty(insight.Rankings);
        Assert.Empty(insight.Goals);
        Assert.Empty(insight.Recommendations);
        Assert.Null(insight.PerformanceSummary);
    }

    [Fact]
    public void XFAiSalespersonRanking_Defaults_AreCorrect()
    {
        var ranking = new XFAiSalespersonRanking();
        Assert.Equal(0, ranking.Position);
        Assert.Equal(string.Empty, ranking.SalespersonName);
        Assert.Equal(0m, ranking.TotalSales);
        Assert.Equal(0m, ranking.GoalAmount);
        Assert.Equal(0, ranking.AchievementPercentage);
        Assert.Equal(0m, ranking.Commission);
        Assert.Null(ranking.Badge);
    }

    [Fact]
    public void XFAiSalespersonRanking_PropertyAssignment_Works()
    {
        var ranking = new XFAiSalespersonRanking
        {
            Position = 1,
            SalespersonName = "Joao Silva",
            TotalSales = 50000m,
            GoalAmount = 40000m,
            AchievementPercentage = 125.0,
            Commission = 2500m,
            Badge = "top-seller"
        };
        Assert.Equal(1, ranking.Position);
        Assert.Equal("Joao Silva", ranking.SalespersonName);
        Assert.Equal(50000m, ranking.TotalSales);
        Assert.Equal(125.0, ranking.AchievementPercentage);
        Assert.Equal("top-seller", ranking.Badge);
    }

    [Fact]
    public void XFAiGoalProgress_Defaults_AreCorrect()
    {
        var goal = new XFAiGoalProgress();
        Assert.Equal(string.Empty, goal.GoalName);
        Assert.Equal(0m, goal.TargetAmount);
        Assert.Equal(0m, goal.CurrentAmount);
        Assert.Equal(0, goal.ProgressPercentage);
        Assert.Equal(0, goal.DaysRemaining);
        Assert.Equal(string.Empty, goal.PredictedOutcome);
        Assert.Equal(0m, goal.PredictedFinalAmount);
    }

    [Fact]
    public void XFAiGoalProgress_PropertyAssignment_Works()
    {
        var goal = new XFAiGoalProgress
        {
            GoalName = "Q2 Revenue",
            TargetAmount = 100000m,
            CurrentAmount = 75000m,
            ProgressPercentage = 75.0,
            DaysRemaining = 30,
            PredictedOutcome = "On track",
            PredictedFinalAmount = 105000m
        };
        Assert.Equal("Q2 Revenue", goal.GoalName);
        Assert.Equal(75.0, goal.ProgressPercentage);
        Assert.Equal("On track", goal.PredictedOutcome);
    }

    [Fact]
    public void XFAiSalesRecommendation_Defaults_AreCorrect()
    {
        var rec = new XFAiSalesRecommendation();
        Assert.Equal(string.Empty, rec.Title);
        Assert.Equal(string.Empty, rec.Description);
        Assert.Equal("action", rec.Type);
        Assert.Null(rec.ProductMix);
        Assert.Null(rec.EstimatedImpact);
    }

    [Fact]
    public void XFAiSalesRecommendation_PropertyAssignment_Works()
    {
        var rec = new XFAiSalesRecommendation
        {
            Title = "Focus on Product A",
            Description = "Product A has highest margin",
            Type = "strategy",
            ProductMix = "A:60%, B:40%",
            EstimatedImpact = 15000m
        };
        Assert.Equal("Focus on Product A", rec.Title);
        Assert.Equal("strategy", rec.Type);
        Assert.Equal(15000m, rec.EstimatedImpact);
    }

    [Fact]
    public void XFAiSalesPerformanceSummary_Defaults_AreCorrect()
    {
        var summary = new XFAiSalesPerformanceSummary();
        Assert.Equal(0m, summary.TotalRevenue);
        Assert.Equal(0m, summary.AverageTicket);
        Assert.Equal(0, summary.TotalTransactions);
        Assert.Equal(0, summary.GrowthRate);
        Assert.Equal(string.Empty, summary.BestPerformingRegion);
        Assert.Equal(string.Empty, summary.BestPerformingProduct);
    }

    [Fact]
    public void XFAiSalesPerformanceSummary_PropertyAssignment_Works()
    {
        var summary = new XFAiSalesPerformanceSummary
        {
            TotalRevenue = 500000m,
            AverageTicket = 2500m,
            TotalTransactions = 200,
            GrowthRate = 12.5,
            BestPerformingRegion = "Sudeste",
            BestPerformingProduct = "Widget Pro"
        };
        Assert.Equal(500000m, summary.TotalRevenue);
        Assert.Equal(12.5, summary.GrowthRate);
        Assert.Equal("Sudeste", summary.BestPerformingRegion);
    }

    [Fact]
    public void XFAiReconciliationResult_Defaults_AreCorrect()
    {
        var result = new XFAiReconciliationResult();
        Assert.NotEmpty(result.Id);
        Assert.Null(result.NaturalLanguageQuery);
        Assert.Equal(string.Empty, result.AiSummary);
        Assert.Equal(0, result.Confidence);
        Assert.Empty(result.Matches);
        Assert.Empty(result.UnmatchedTransactions);
        Assert.Empty(result.Duplicates);
        Assert.Empty(result.FraudIndicators);
        Assert.Null(result.CashflowSummary);
    }

    [Fact]
    public void XFAiReconciliationMatch_Defaults_AreCorrect()
    {
        var match = new XFAiReconciliationMatch();
        Assert.Equal(string.Empty, match.TransactionDescription);
        Assert.Equal(string.Empty, match.BankDescription);
        Assert.Equal(0m, match.TransactionAmount);
        Assert.Equal(0m, match.BankAmount);
        Assert.Equal(0, match.MatchScore);
        Assert.Equal("exact", match.MatchType);
    }

    [Fact]
    public void XFAiReconciliationMatch_PropertyAssignment_Works()
    {
        var match = new XFAiReconciliationMatch
        {
            TransactionDescription = "INV-001",
            BankDescription = "PIX INV-001",
            TransactionAmount = 1500m,
            BankAmount = 1500m,
            MatchScore = 0.98,
            MatchType = "fuzzy"
        };
        Assert.Equal("INV-001", match.TransactionDescription);
        Assert.Equal(0.98, match.MatchScore);
        Assert.Equal("fuzzy", match.MatchType);
    }

    [Fact]
    public void XFAiReconciliationUnmatched_Defaults_AreCorrect()
    {
        var unmatched = new XFAiReconciliationUnmatched();
        Assert.Equal(string.Empty, unmatched.Description);
        Assert.Equal(0m, unmatched.Amount);
        Assert.Equal(string.Empty, unmatched.Source);
        Assert.Null(unmatched.PossibleReason);
    }

    [Fact]
    public void XFAiDuplicateDetection_Defaults_AreCorrect()
    {
        var dup = new XFAiDuplicateDetection();
        Assert.Equal(string.Empty, dup.Description1);
        Assert.Equal(string.Empty, dup.Description2);
        Assert.Equal(0m, dup.Amount);
        Assert.Equal(0, dup.Similarity);
        Assert.Equal(string.Empty, dup.Recommendation);
    }

    [Fact]
    public void XFAiFraudIndicator_Defaults_AreCorrect()
    {
        var fraud = new XFAiFraudIndicator();
        Assert.Equal(string.Empty, fraud.Type);
        Assert.Equal(string.Empty, fraud.Description);
        Assert.Equal("medium", fraud.Severity);
        Assert.Null(fraud.Amount);
        Assert.Null(fraud.Recommendation);
    }

    [Fact]
    public void XFAiFraudIndicator_PropertyAssignment_Works()
    {
        var fraud = new XFAiFraudIndicator
        {
            Type = "DuplicatePayment",
            Description = "Same amount to same vendor",
            Severity = "high",
            Amount = 5000m,
            Recommendation = "Investigate vendor"
        };
        Assert.Equal("DuplicatePayment", fraud.Type);
        Assert.Equal(5000m, fraud.Amount);
    }

    [Fact]
    public void XFAiCashflowSummary_Defaults_AreCorrect()
    {
        var cashflow = new XFAiCashflowSummary();
        Assert.Equal(0m, cashflow.TotalInflow);
        Assert.Equal(0m, cashflow.TotalOutflow);
        Assert.Equal(0m, cashflow.NetCashflow);
        Assert.Equal(0m, cashflow.MatchedPercentage);
        Assert.Equal(0, cashflow.UnmatchedCount);
    }

    [Fact]
    public void XFAiCashflowSummary_PropertyAssignment_Works()
    {
        var cashflow = new XFAiCashflowSummary
        {
            TotalInflow = 100000m,
            TotalOutflow = 80000m,
            NetCashflow = 20000m,
            MatchedPercentage = 95.5m,
            UnmatchedCount = 3
        };
        Assert.Equal(20000m, cashflow.NetCashflow);
        Assert.Equal(95.5m, cashflow.MatchedPercentage);
    }

    #endregion

    #region Fiscal DTOs

    [Fact]
    public void XFAiNFeAnalysisResult_Defaults_AreCorrect()
    {
        var result = new XFAiNFeAnalysisResult();
        Assert.NotEmpty(result.Id);
        Assert.Equal(8, result.Id.Length);
        Assert.Null(result.NFeKey);
        Assert.Null(result.IssuerCnpj);
        Assert.Null(result.IssuerName);
        Assert.Null(result.RecipientCnpj);
        Assert.Null(result.RecipientName);
        Assert.Null(result.InvoiceNumber);
        Assert.Null(result.Series);
        Assert.Null(result.IssueDate);
        Assert.Equal(0m, result.TotalValue);
        Assert.Equal(0m, result.IcmsTotal);
        Assert.Equal(0m, result.IpiTotal);
        Assert.Equal(0m, result.PisTotal);
        Assert.Equal(0m, result.CofinsTotal);
        Assert.Empty(result.Items);
        Assert.Empty(result.TaxSummary);
        Assert.Empty(result.Inconsistencies);
        Assert.Null(result.AiSummary);
        Assert.Equal(XFAiNFeSpedStatus.NotValidated, result.SpedStatus);
        Assert.Null(result.CbsIbsReadiness);
        Assert.Equal(XFAiNFeRiskLevel.Low, result.RiskLevel);
        Assert.Equal(0, result.Confidence);
        Assert.Null(result.NaturalLanguageQuery);
    }

    [Fact]
    public void XFAiNFeAnalysisResult_PropertyAssignment_Works()
    {
        var result = new XFAiNFeAnalysisResult
        {
            Id = "nfe1234",
            NFeKey = "12345678901234567890123456789012345678901234",
            IssuerCnpj = "12345678000189",
            IssuerName = "Empresa LTDA",
            RecipientCnpj = "98765432000198",
            RecipientName = "Cliente SA",
            InvoiceNumber = "001",
            Series = "1",
            IssueDate = "2026-06-05",
            TotalValue = 10000m,
            IcmsTotal = 1800m,
            IpiTotal = 500m,
            PisTotal = 65m,
            CofinsTotal = 300m,
            Items = [new XFAiNFeItem()],
            TaxSummary = [new XFAiNFeTaxSummary()],
            Inconsistencies = [new XFAiNFeInconsistency()],
            AiSummary = "Valid NFe",
            SpedStatus = XFAiNFeSpedStatus.Valid,
            CbsIbsReadiness = "Partial",
            RiskLevel = XFAiNFeRiskLevel.Medium,
            Confidence = 0.85,
            NaturalLanguageQuery = "Analyze this NFe"
        };
        Assert.Equal("nfe1234", result.Id);
        Assert.Equal(44, result.NFeKey!.Length);
        Assert.Equal(10000m, result.TotalValue);
        Assert.Equal(XFAiNFeSpedStatus.Valid, result.SpedStatus);
        Assert.Equal(XFAiNFeRiskLevel.Medium, result.RiskLevel);
        Assert.Single(result.Items);
        Assert.Single(result.TaxSummary);
        Assert.Single(result.Inconsistencies);
    }

    [Fact]
    public void XFAiNFeItem_Defaults_AreCorrect()
    {
        var item = new XFAiNFeItem();
        Assert.Equal(0, item.Sequence);
        Assert.Null(item.Description);
        Assert.Null(item.Ncm);
        Assert.Null(item.Cfop);
        Assert.Null(item.Cst);
        Assert.Equal(0m, item.Quantity);
        Assert.Equal(0m, item.UnitValue);
        Assert.Equal(0m, item.TotalValue);
        Assert.Equal(0m, item.IcmsValue);
        Assert.Equal(0m, item.IpiValue);
        Assert.Null(item.ItemClassification);
    }

    [Fact]
    public void XFAiNFeTaxSummary_Defaults_AreCorrect()
    {
        var summary = new XFAiNFeTaxSummary();
        Assert.Equal(string.Empty, summary.Cfop);
        Assert.Equal(string.Empty, summary.Description);
        Assert.Equal(0, summary.ItemCount);
        Assert.Equal(0m, summary.TotalValue);
        Assert.Equal(0m, summary.TotalTax);
        Assert.Equal(0, summary.EffectiveRate);
    }

    [Fact]
    public void XFAiNFeInconsistency_Defaults_AreCorrect()
    {
        var inc = new XFAiNFeInconsistency();
        Assert.Equal(XFAiNFeInconsistencySeverity.Info, inc.Severity);
        Assert.Equal(string.Empty, inc.Code);
        Assert.Equal(string.Empty, inc.Message);
        Assert.Null(inc.Field);
        Assert.Null(inc.Suggestion);
    }

    [Fact]
    public void XFAiNFeInconsistency_PropertyAssignment_Works()
    {
        var inc = new XFAiNFeInconsistency
        {
            Severity = XFAiNFeInconsistencySeverity.Critical,
            Code = "ICMS-001",
            Message = "ICMS rate mismatch",
            Field = "vICMS",
            Suggestion = "Verify CST and ICMS rate"
        };
        Assert.Equal(XFAiNFeInconsistencySeverity.Critical, inc.Severity);
        Assert.Equal("ICMS-001", inc.Code);
    }

    [Fact]
    public void XFAiNFeEnums_AllValues_AreCorrect()
    {
        Assert.Equal(4, Enum.GetValues<XFAiNFeInconsistencySeverity>().Length);
        Assert.Equal(4, Enum.GetValues<XFAiNFeSpedStatus>().Length);
        Assert.Equal(4, Enum.GetValues<XFAiNFeRiskLevel>().Length);

        Assert.True(Enum.IsDefined<XFAiNFeInconsistencySeverity>(XFAiNFeInconsistencySeverity.Critical));
        Assert.True(Enum.IsDefined<XFAiNFeSpedStatus>(XFAiNFeSpedStatus.WithWarnings));
        Assert.True(Enum.IsDefined<XFAiNFeRiskLevel>(XFAiNFeRiskLevel.High));
    }

    #endregion

    #region Forms DTOs

    [Fact]
    public void XFAiFormInsight_Defaults_AreCorrect()
    {
        var insight = new XFAiFormInsight();
        Assert.NotEmpty(insight.Id);
        Assert.Null(insight.NaturalLanguageQuery);
        Assert.Equal(string.Empty, insight.AiSummary);
        Assert.Equal(0, insight.Confidence);
        Assert.Empty(insight.AutofillSuggestions);
        Assert.Empty(insight.ValidationFixes);
        Assert.Empty(insight.Inconsistencies);
    }

    [Fact]
    public void XFAiAutofillSuggestion_Defaults_AreCorrect()
    {
        var suggestion = new XFAiAutofillSuggestion();
        Assert.Equal(string.Empty, suggestion.FieldName);
        Assert.Equal(string.Empty, suggestion.FieldLabel);
        Assert.Equal(string.Empty, suggestion.SuggestedValue);
        Assert.Equal(string.Empty, suggestion.Source);
        Assert.Equal(0, suggestion.Confidence);
    }

    [Fact]
    public void XFAiAutofillSuggestion_PropertyAssignment_Works()
    {
        var suggestion = new XFAiAutofillSuggestion
        {
            FieldName = "cnpj",
            FieldLabel = "CNPJ",
            SuggestedValue = "12.345.678/0001-89",
            Source = "Previous submission",
            Confidence = 0.95
        };
        Assert.Equal("cnpj", suggestion.FieldName);
        Assert.Equal(0.95, suggestion.Confidence);
    }

    [Fact]
    public void XFAiValidationFix_Defaults_AreCorrect()
    {
        var fix = new XFAiValidationFix();
        Assert.Equal(string.Empty, fix.FieldName);
        Assert.Equal(string.Empty, fix.CurrentValue);
        Assert.Equal(string.Empty, fix.SuggestedFix);
        Assert.Equal(string.Empty, fix.ValidationRule);
        Assert.Equal(string.Empty, fix.Explanation);
    }

    [Fact]
    public void XFAiValidationFix_PropertyAssignment_Works()
    {
        var fix = new XFAiValidationFix
        {
            FieldName = "email",
            CurrentValue = "invalid",
            SuggestedFix = "user@example.com",
            ValidationRule = "EmailFormat",
            Explanation = "Must be valid email format"
        };
        Assert.Equal("email", fix.FieldName);
        Assert.Equal("user@example.com", fix.SuggestedFix);
    }

    [Fact]
    public void XFAiFormInconsistency_Defaults_AreCorrect()
    {
        var inc = new XFAiFormInconsistency();
        Assert.Equal(string.Empty, inc.Field1);
        Assert.Equal(string.Empty, inc.Field2);
        Assert.Equal(string.Empty, inc.Description);
        Assert.Equal("medium", inc.Severity);
        Assert.Null(inc.Suggestion);
    }

    [Fact]
    public void XFAiFormInsight_WithAllCollections_Works()
    {
        var insight = new XFAiFormInsight
        {
            AutofillSuggestions = [new XFAiAutofillSuggestion { FieldName = "name" }],
            ValidationFixes = [new XFAiValidationFix { FieldName = "email" }],
            Inconsistencies = [new XFAiFormInconsistency { Field1 = "start", Field2 = "end" }]
        };
        Assert.Single(insight.AutofillSuggestions);
        Assert.Single(insight.ValidationFixes);
        Assert.Single(insight.Inconsistencies);
    }

    #endregion

    #region Stock DTOs

    [Fact]
    public void XFAiStockInsight_Defaults_AreCorrect()
    {
        var insight = new XFAiStockInsight();
        Assert.NotEmpty(insight.Id);
        Assert.Null(insight.NaturalLanguageQuery);
        Assert.Equal(string.Empty, insight.AiSummary);
        Assert.Equal(0, insight.Confidence);
        Assert.Empty(insight.Forecasts);
        Assert.Empty(insight.Alerts);
        Assert.Empty(insight.ReorderSuggestions);
        Assert.Null(insight.AbcClassification);
        Assert.Null(insight.Summary);
    }

    [Fact]
    public void XFAiStockForecast_Defaults_AreCorrect()
    {
        var forecast = new XFAiStockForecast();
        Assert.Equal(string.Empty, forecast.ProductName);
        Assert.Null(forecast.ProductSku);
        Assert.Equal(0m, forecast.CurrentStock);
        Assert.Equal(0m, forecast.PredictedDemand30Days);
        Assert.Equal(0m, forecast.PredictedDemand90Days);
        Assert.Equal(0, forecast.DaysUntilStockout);
        Assert.Equal("stable", forecast.TrendDirection);
        Assert.Null(forecast.SeasonalityNote);
    }

    [Fact]
    public void XFAiStockForecast_PropertyAssignment_Works()
    {
        var forecast = new XFAiStockForecast
        {
            ProductName = "Widget A",
            ProductSku = "SKU-001",
            CurrentStock = 500m,
            PredictedDemand30Days = 200m,
            PredictedDemand90Days = 600m,
            DaysUntilStockout = 75,
            TrendDirection = "increasing",
            SeasonalityNote = "Higher demand in Q4"
        };
        Assert.Equal("Widget A", forecast.ProductName);
        Assert.Equal(75, forecast.DaysUntilStockout);
        Assert.Equal("increasing", forecast.TrendDirection);
    }

    [Fact]
    public void XFAiStockAlert_Defaults_AreCorrect()
    {
        var alert = new XFAiStockAlert();
        Assert.Equal(XFAiStockAlertType.RuptureRisk, alert.Type);
        Assert.Equal("medium", alert.Severity);
        Assert.Equal(string.Empty, alert.ProductName);
        Assert.Equal(string.Empty, alert.Message);
        Assert.Null(alert.RecommendedAction);
    }

    [Fact]
    public void XFAiStockAlert_PropertyAssignment_Works()
    {
        var alert = new XFAiStockAlert
        {
            Type = XFAiStockAlertType.Overstock,
            Severity = "low",
            ProductName = "Widget B",
            Message = "Stock exceeds 120% of demand",
            RecommendedAction = "Reduce reorder quantity"
        };
        Assert.Equal(XFAiStockAlertType.Overstock, alert.Type);
        Assert.Equal("low", alert.Severity);
    }

    [Fact]
    public void XFAiStockAlertType_AllValues_AreCorrect()
    {
        var values = Enum.GetValues<XFAiStockAlertType>();
        Assert.Equal(6, values.Length);
        Assert.True(Enum.IsDefined(XFAiStockAlertType.RuptureRisk));
        Assert.True(Enum.IsDefined(XFAiStockAlertType.Overstock));
        Assert.True(Enum.IsDefined(XFAiStockAlertType.LowMargin));
        Assert.True(Enum.IsDefined(XFAiStockAlertType.SlowMoving));
        Assert.True(Enum.IsDefined(XFAiStockAlertType.SeasonalDemand));
        Assert.True(Enum.IsDefined(XFAiStockAlertType.SupplierIssue));
    }

    [Fact]
    public void XFAiStockReorderSuggestion_Defaults_AreCorrect()
    {
        var reorder = new XFAiStockReorderSuggestion();
        Assert.Equal(string.Empty, reorder.ProductName);
        Assert.Null(reorder.ProductSku);
        Assert.Equal(0m, reorder.SuggestedQuantity);
        Assert.Equal(0m, reorder.EstimatedCost);
        Assert.Null(reorder.PreferredSupplier);
        Assert.Equal(0, reorder.UrgencyDays);
        Assert.Equal(string.Empty, reorder.Reason);
    }

    [Fact]
    public void XFAiAbcClassification_Defaults_AreCorrect()
    {
        var abc = new XFAiAbcClassification();
        Assert.Empty(abc.ClassAProducts);
        Assert.Empty(abc.ClassBProducts);
        Assert.Empty(abc.ClassCProducts);
        Assert.Equal(0m, abc.ClassAPercentage);
        Assert.Equal(0m, abc.ClassBPercentage);
        Assert.Equal(0m, abc.ClassCPercentage);
    }

    [Fact]
    public void XFAiAbcClassification_PropertyAssignment_Works()
    {
        var abc = new XFAiAbcClassification
        {
            ClassAProducts = ["Widget A"],
            ClassBProducts = ["Widget B", "Widget C"],
            ClassCProducts = ["Widget D"],
            ClassAPercentage = 80m,
            ClassBPercentage = 15m,
            ClassCPercentage = 5m
        };
        Assert.Single(abc.ClassAProducts);
        Assert.Equal(2, abc.ClassBProducts.Count);
        Assert.Equal(80m, abc.ClassAPercentage);
    }

    [Fact]
    public void XFAiStockSummary_Defaults_AreCorrect()
    {
        var summary = new XFAiStockSummary();
        Assert.Equal(0, summary.TotalProducts);
        Assert.Equal(0, summary.LowStockProducts);
        Assert.Equal(0, summary.OverstockProducts);
        Assert.Equal(0m, summary.TotalStockValue);
        Assert.Equal(0, summary.AverageTurnoverDays);
        Assert.Equal(0m, summary.WastedStockPercentage);
    }

    [Fact]
    public void XFAiStockSummary_PropertyAssignment_Works()
    {
        var summary = new XFAiStockSummary
        {
            TotalProducts = 150,
            LowStockProducts = 12,
            OverstockProducts = 5,
            TotalStockValue = 250000m,
            AverageTurnoverDays = 45.5,
            WastedStockPercentage = 2.3m
        };
        Assert.Equal(150, summary.TotalProducts);
        Assert.Equal(45.5, summary.AverageTurnoverDays);
    }

    #endregion

    #region MultiModal DTOs

    [Fact]
    public void XFAiAttachmentType_AllValues_AreCorrect()
    {
        var values = Enum.GetValues<XFAiAttachmentType>();
        Assert.Equal(4, values.Length);
        Assert.True(Enum.IsDefined(XFAiAttachmentType.Image));
        Assert.True(Enum.IsDefined(XFAiAttachmentType.Audio));
        Assert.True(Enum.IsDefined(XFAiAttachmentType.File));
        Assert.True(Enum.IsDefined(XFAiAttachmentType.Url));
    }

    [Fact]
    public void XFAiAttachment_RequiredProperties_Work()
    {
        var attachment = new XFAiAttachment
        {
            Type = XFAiAttachmentType.Image,
            Name = "chart.png",
            Data = "iVBORw0KGgo=",
            MimeType = "image/png",
            SizeBytes = 2048
        };
        Assert.Equal(XFAiAttachmentType.Image, attachment.Type);
        Assert.Equal("chart.png", attachment.Name);
        Assert.Equal("iVBORw0KGgo=", attachment.Data);
        Assert.Equal("image/png", attachment.MimeType);
        Assert.Equal(2048, attachment.SizeBytes);
    }

    [Fact]
    public void XFAiAttachment_MinimalRequired_Works()
    {
        var attachment = new XFAiAttachment
        {
            Type = XFAiAttachmentType.Url,
            Name = "link",
            Data = "https://example.com"
        };
        Assert.Equal(XFAiAttachmentType.Url, attachment.Type);
        Assert.Null(attachment.MimeType);
        Assert.Null(attachment.SizeBytes);
    }

    [Fact]
    public void XFAiMultiModalMessage_RequiredProperties_Work()
    {
        var msg = new XFAiMultiModalMessage
        {
            Role = AiRole.User,
            Content = "Analyze this image"
        };
        Assert.Equal(AiRole.User, msg.Role);
        Assert.Equal("Analyze this image", msg.Content);
        Assert.Null(msg.Attachments);
        Assert.Null(msg.Usage);
        Assert.True(msg.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void XFAiMultiModalMessage_WithAttachments_Works()
    {
        var attachment = new XFAiAttachment
        {
            Type = XFAiAttachmentType.Image,
            Name = "photo.jpg",
            Data = "base64data"
        };
        var msg = new XFAiMultiModalMessage
        {
            Role = AiRole.User,
            Content = "What do you see?",
            Attachments = [attachment],
            Usage = new AiTokenUsage { PromptTokens = 100 }
        };
        Assert.Single(msg.Attachments!);
        Assert.Equal(100, msg.Usage!.PromptTokens);
    }

    #endregion

    #region Vision DTOs

    [Fact]
    public void XFAiVisionAnalysisType_AllValues_AreCorrect()
    {
        var values = Enum.GetValues<XFAiVisionAnalysisType>();
        Assert.Equal(6, values.Length);
        Assert.True(Enum.IsDefined(XFAiVisionAnalysisType.Describe));
        Assert.True(Enum.IsDefined(XFAiVisionAnalysisType.Ocr));
        Assert.True(Enum.IsDefined(XFAiVisionAnalysisType.DetectObjects));
        Assert.True(Enum.IsDefined(XFAiVisionAnalysisType.AnalyzeChart));
        Assert.True(Enum.IsDefined(XFAiVisionAnalysisType.ReadDocument));
        Assert.True(Enum.IsDefined(XFAiVisionAnalysisType.Compare));
    }

    [Fact]
    public void XFAiVisionResult_RequiredProperties_Work()
    {
        var result = new XFAiVisionResult
        {
            Text = "An invoice for R$ 1500",
            AnalysisType = XFAiVisionAnalysisType.ReadDocument,
            Confidence = 0.92f,
            ImageCount = 1
        };
        Assert.Equal("An invoice for R$ 1500", result.Text);
        Assert.Equal(XFAiVisionAnalysisType.ReadDocument, result.AnalysisType);
        Assert.Equal(0.92f, result.Confidence);
        Assert.Equal(1, result.ImageCount);
        Assert.Null(result.StructuredData);
        Assert.True(result.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void XFAiVisionResult_WithStructuredData_Works()
    {
        var result = new XFAiVisionResult
        {
            Text = "Chart shows upward trend",
            StructuredData = """{"trend":"up","value":42.5}""",
            AnalysisType = XFAiVisionAnalysisType.AnalyzeChart,
            ImageCount = 2
        };
        Assert.NotNull(result.StructuredData);
        Assert.Contains("trend", result.StructuredData);
    }

    #endregion

    #region Audio DTOs

    [Fact]
    public void XFAiVoiceState_AllValues_AreCorrect()
    {
        var values = Enum.GetValues<XFAiVoiceState>();
        Assert.Equal(5, values.Length);
        Assert.True(Enum.IsDefined(XFAiVoiceState.Idle));
        Assert.True(Enum.IsDefined(XFAiVoiceState.Listening));
        Assert.True(Enum.IsDefined(XFAiVoiceState.Processing));
        Assert.True(Enum.IsDefined(XFAiVoiceState.Speaking));
        Assert.True(Enum.IsDefined(XFAiVoiceState.Error));
    }

    [Fact]
    public void XFAiTranscriptionResult_RequiredProperties_Work()
    {
        var result = new XFAiTranscriptionResult
        {
            Text = "Hello world",
            Language = "pt-BR",
            DurationSeconds = 5.2f,
            Confidence = 0.95f,
            IsPartial = false
        };
        Assert.Equal("Hello world", result.Text);
        Assert.Equal("pt-BR", result.Language);
        Assert.Equal(5.2f, result.DurationSeconds);
        Assert.Equal(0.95f, result.Confidence);
        Assert.False(result.IsPartial);
        Assert.True(result.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void XFAiTranscriptionResult_Partial_Works()
    {
        var result = new XFAiTranscriptionResult
        {
            Text = "Hel",
            IsPartial = true
        };
        Assert.True(result.IsPartial);
        Assert.Null(result.Language);
        Assert.Null(result.DurationSeconds);
        Assert.Null(result.Confidence);
    }

    #endregion

    #region Utility DTOs

    [Fact]
    public void XFAiValidationItem_Defaults_AreCorrect()
    {
        var item = new XFAiValidationItem();
        Assert.NotEmpty(item.Id);
        Assert.Equal(8, item.Id.Length);
        Assert.Equal(string.Empty, item.FieldName);
        Assert.Equal(string.Empty, item.Message);
        Assert.Equal(XFAiValidationSeverity.Info, item.Severity);
        Assert.Null(item.AiExplanation);
        Assert.Null(item.AiFixSuggestion);
        Assert.Null(item.TriggerValue);
        Assert.False(item.IsResolved);
    }

    [Fact]
    public void XFAiValidationItem_PropertyAssignment_Works()
    {
        var item = new XFAiValidationItem
        {
            Id = "v123",
            FieldName = "cnpj",
            Message = "Invalid CNPJ",
            Severity = XFAiValidationSeverity.Error,
            AiExplanation = "Check digit mismatch",
            AiFixSuggestion = "Verify the 14-digit CNPJ",
            TriggerValue = "12345678000100",
            IsResolved = true
        };
        Assert.Equal("v123", item.Id);
        Assert.Equal(XFAiValidationSeverity.Error, item.Severity);
        Assert.Equal("Check digit mismatch", item.AiExplanation);
        Assert.True(item.IsResolved);
    }

    [Fact]
    public void XFAiValidationSeverity_AllValues_AreCorrect()
    {
        var values = Enum.GetValues<XFAiValidationSeverity>();
        Assert.Equal(4, values.Length);
        Assert.True(Enum.IsDefined(XFAiValidationSeverity.Info));
        Assert.True(Enum.IsDefined(XFAiValidationSeverity.Warning));
        Assert.True(Enum.IsDefined(XFAiValidationSeverity.Error));
        Assert.True(Enum.IsDefined(XFAiValidationSeverity.Critical));
    }

    [Fact]
    public void XFAiRewriteResult_Defaults_AreCorrect()
    {
        var result = new XFAiRewriteResult();
        Assert.Equal(string.Empty, result.OriginalText);
        Assert.Equal(string.Empty, result.RewrittenText);
        Assert.Equal(XFAiRewriteMode.Formalize, result.Mode);
        Assert.Equal(0, result.Confidence);
        Assert.Null(result.ChangeDescription);
    }

    [Fact]
    public void XFAiRewriteResult_PropertyAssignment_Works()
    {
        var result = new XFAiRewriteResult
        {
            OriginalText = "hey what's up",
            RewrittenText = "Dear Sir/Madam, I hope this message finds you well.",
            Mode = XFAiRewriteMode.Formalize,
            Confidence = 0.88,
            ChangeDescription = "Informal to formal tone"
        };
        Assert.Equal("hey what's up", result.OriginalText);
        Assert.Equal(XFAiRewriteMode.Formalize, result.Mode);
        Assert.Equal("Informal to formal tone", result.ChangeDescription);
    }

    [Fact]
    public void XFAiRewriteMode_AllValues_AreCorrect()
    {
        var values = Enum.GetValues<XFAiRewriteMode>();
        Assert.Equal(6, values.Length);
        Assert.True(Enum.IsDefined(XFAiRewriteMode.Formalize));
        Assert.True(Enum.IsDefined(XFAiRewriteMode.Summarize));
        Assert.True(Enum.IsDefined(XFAiRewriteMode.Expand));
        Assert.True(Enum.IsDefined(XFAiRewriteMode.FixGrammar));
        Assert.True(Enum.IsDefined(XFAiRewriteMode.AdjustTone));
        Assert.True(Enum.IsDefined(XFAiRewriteMode.Translate));
    }

    [Fact]
    public void XFAiExplainResult_Defaults_AreCorrect()
    {
        var result = new XFAiExplainResult();
        Assert.Equal(string.Empty, result.ElementName);
        Assert.Equal(string.Empty, result.Explanation);
        Assert.Empty(result.Details);
        Assert.Equal(0, result.Confidence);
        Assert.Empty(result.Sources);
    }

    [Fact]
    public void XFAiExplainResult_PropertyAssignment_Works()
    {
        var result = new XFAiExplainResult
        {
            ElementName = "ICMS",
            Explanation = "Tax on goods and services",
            Details = [new XFAiExplainDetail { Label = "Rate", Value = "18%" }],
            Confidence = 0.90,
            Sources = ["CTN Art. 155"]
        };
        Assert.Equal("ICMS", result.ElementName);
        Assert.Single(result.Details);
        Assert.Equal("Rate", result.Details[0].Label);
        Assert.Single(result.Sources);
    }

    [Fact]
    public void XFAiExplainDetail_Defaults_AreCorrect()
    {
        var detail = new XFAiExplainDetail();
        Assert.Equal(string.Empty, detail.Label);
        Assert.Equal(string.Empty, detail.Value);
        Assert.Null(detail.Icon);
    }

    #endregion

    #region Service DTOs (AiEmbeddingRequest, AiEmbeddingResponse, AiHealthStatus)

    [Fact]
    public void AiEmbeddingRequest_Defaults_AreCorrect()
    {
        var request = new AiEmbeddingRequest { Inputs = [] };
        Assert.Null(request.Model);
        Assert.Empty(request.Inputs);
        Assert.Null(request.Dimensions);
    }

    [Fact]
    public void AiEmbeddingRequest_PropertyAssignment_Works()
    {
        var request = new AiEmbeddingRequest
        {
            Inputs = ["hello", "world"],
            Model = "text-embedding-3-small",
            Dimensions = 1536
        };
        Assert.Equal("text-embedding-3-small", request.Model);
        Assert.Equal(2, request.Inputs.Count);
        Assert.Equal(1536, request.Dimensions);
    }

    [Fact]
    public void AiEmbeddingResponse_Defaults_AreCorrect()
    {
        var response = new AiEmbeddingResponse { Embeddings = [] };
        Assert.Empty(response.Embeddings);
        Assert.Null(response.Model);
        Assert.Null(response.Usage);
    }

    [Fact]
    public void AiEmbeddingResponse_PropertyAssignment_Works()
    {
        var response = new AiEmbeddingResponse
        {
            Embeddings = [new float[] { 0.1f, 0.2f, 0.3f }],
            Model = "text-embedding-3-small",
            Usage = new AiTokenUsage { PromptTokens = 10 }
        };
        Assert.Single(response.Embeddings);
        Assert.Equal(3, response.Embeddings[0].Length);
        Assert.Equal("text-embedding-3-small", response.Model);
        Assert.NotNull(response.Usage);
    }

    [Fact]
    public void AiHealthStatus_Constructor_WithMessage_Works()
    {
        var health = new AiHealthStatus(true, "OK", 42);
        Assert.True(health.IsHealthy);
        Assert.Equal("OK", health.Message);
        Assert.Equal(42, health.LatencyMs);
    }

    [Fact]
    public void AiHealthStatus_Unhealthy_Works()
    {
        var health = new AiHealthStatus(false, "Connection refused");
        Assert.False(health.IsHealthy);
        Assert.Equal("Connection refused", health.Message);
        Assert.Null(health.LatencyMs);
    }

    #endregion

    #region Common DTOs — remaining 0% items

    [Fact]
    public void AiChatResponse_Defaults_AreCorrect()
    {
        var response = new AiChatResponse();
        Assert.NotNull(response.Message);
        Assert.Equal(AiRole.Assistant, response.Message.Role);
        Assert.Null(response.FinishReason);
        Assert.Null(response.ResponseId);
        Assert.Null(response.Usage);
        Assert.Null(response.Duration);
    }

    [Fact]
    public void AiChatResponse_PropertyAssignment_Works()
    {
        var response = new AiChatResponse
        {
            Message = new AiMessage { Role = AiRole.Assistant, Content = "Hello" },
            FinishReason = "stop",
            ResponseId = "resp-123",
            Usage = new AiTokenUsage { PromptTokens = 5, CompletionTokens = 10 }
        };
        Assert.NotNull(response.Message);
        Assert.Equal("Hello", response.Message.Content);
        Assert.Equal("stop", response.FinishReason);
        Assert.Equal("resp-123", response.ResponseId);
        Assert.Equal(5, response.Usage!.PromptTokens);
        Assert.Equal(10, response.Usage.CompletionTokens);
    }

    [Fact]
    public void AiToolCallDelta_Defaults_AreCorrect()
    {
        var delta = new AiToolCallDelta();
        Assert.Null(delta.Id);
        Assert.Null(delta.Name);
        Assert.Null(delta.ArgumentsDelta);
        Assert.Equal(0, delta.Index);
    }

    [Fact]
    public void AiToolCallDelta_PropertyAssignment_Works()
    {
        var delta = new AiToolCallDelta
        {
            Id = "tc-1",
            Name = "get_weather",
            ArgumentsDelta = """{"city":""",
            Index = 2
        };
        Assert.Equal("tc-1", delta.Id);
        Assert.Equal("get_weather", delta.Name);
        Assert.Equal(2, delta.Index);
    }

    [Fact]
    public void AiToolDefinition_Defaults_AreCorrect()
    {
        var def = new AiToolDefinition { Name = "test", Description = "Test tool" };
        Assert.Equal("test", def.Name);
        Assert.Equal("Test tool", def.Description);
        Assert.Null(def.ParametersSchema);
    }

    [Fact]
    public void AiToolDefinition_PropertyAssignment_Works()
    {
        var def = new AiToolDefinition
        {
            Name = "calculator",
            Description = "Performs arithmetic",
            ParametersSchema = """{"type":"object","properties":{"expr":{"type":"string"}}}"""
        };
        Assert.Equal("calculator", def.Name);
        Assert.Equal("Performs arithmetic", def.Description);
        Assert.Contains("properties", def.ParametersSchema!);
    }

    [Fact]
    public void AiSpeechRequest_PropertyAssignment_Works()
    {
        var request = new AiSpeechRequest
        {
            Text = "Hello world",
            Voice = "alloy",
            Language = "en",
            Speed = 1.5f
        };
        Assert.Equal("Hello world", request.Text);
        Assert.Equal(1.5f, request.Speed);
        Assert.Equal("alloy", request.Voice);
        Assert.Equal("en", request.Language);
    }

    [Fact]
    public void AiSpeechResponse_PropertyAssignment_Works()
    {
        var response = new AiSpeechResponse
        {
            AudioBytes = new byte[] { 1, 2, 3 },
            ContentType = "audio/mp3"
        };
        Assert.Equal(3, response.AudioBytes!.Length);
        Assert.Equal("audio/mp3", response.ContentType);
    }

    [Fact]
    public void AiVisionRequest_PropertyAssignment_Works()
    {
        var request = new AiVisionRequest
        {
            Prompt = "Describe this image",
            Images = ["base64imagedata"],
            MimeTypes = ["image/png"],
            Temperature = 0.5,
            MaxTokens = 500
        };
        Assert.Equal("Describe this image", request.Prompt);
        Assert.Single(request.Images);
        Assert.Equal(0.5, request.Temperature);
        Assert.Equal(500, request.MaxTokens);
    }

    [Fact]
    public void AiVisionResponse_PropertyAssignment_Works()
    {
        var response = new AiVisionResponse
        {
            Text = "A cat sitting on a mat",
            StructuredData = """{"objects":["cat","mat"]}""",
            Confidence = 0.95f
        };
        Assert.Equal("A cat sitting on a mat", response.Text);
        Assert.Equal(0.95f, response.Confidence);
        Assert.NotNull(response.StructuredData);
    }

    [Fact]
    public void AiTranscriptionRequest_PropertyAssignment_Works()
    {
        var request = new AiTranscriptionRequest
        {
            Audio = "base64audio",
            Language = "pt-BR",
            ContentType = "audio/wav",
            IncludeTimestamps = true,
            Format = "json"
        };
        Assert.Equal("base64audio", request.Audio);
        Assert.Equal("pt-BR", request.Language);
        Assert.True(request.IncludeTimestamps);
    }

    [Fact]
    public void AiTranscriptionResponse_PropertyAssignment_Works()
    {
        var response = new AiTranscriptionResponse
        {
            Text = "Transcribed text",
            DetectedLanguage = "pt-BR",
            DurationSeconds = 10.5f,
            Segments = [new AiTranscriptionSegment { Text = "Hello" }]
        };
        Assert.Equal("Transcribed text", response.Text);
        Assert.Equal("pt-BR", response.DetectedLanguage);
        Assert.Equal(10.5f, response.DurationSeconds);
        Assert.Single(response.Segments!);
    }

    [Fact]
    public void AiTranscriptionSegment_PropertyAssignment_Works()
    {
        var segment = new AiTranscriptionSegment
        {
            Text = "Hello world",
            Start = 5.0f,
            End = 10.5f,
            Words = [new AiTranscriptionWord { Text = "Hello", Start = 5.0f, End = 5.5f, Confidence = 0.95f }]
        };
        Assert.Equal("Hello world", segment.Text);
        Assert.Equal(5.0f, segment.Start);
        Assert.Equal(10.5f, segment.End);
        Assert.Single(segment.Words!);
    }

    [Fact]
    public void AiTranscriptionWord_PropertyAssignment_Works()
    {
        var word = new AiTranscriptionWord
        {
            Text = "Hello",
            Start = 1.0f,
            End = 1.5f,
            Confidence = 0.98f
        };
        Assert.Equal("Hello", word.Text);
        Assert.Equal(1.5f, word.End);
        Assert.Equal(0.98f, word.Confidence);
    }

    #endregion
}
