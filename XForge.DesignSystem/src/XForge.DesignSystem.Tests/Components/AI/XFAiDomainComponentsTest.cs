#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Fiscal;
using XForge.DesignSystem.Components.AI.Stock;
using XForge.DesignSystem.Components.AI.Financial;
using XForge.DesignSystem.Components.AI.Accounting;
using XForge.DesignSystem.Components.AI.Forms;
#endregion

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for S-AI004 AI Domain-Specific components:
/// XFAiNFeAnalyzer, XFAiStockAdvisor, XFAiSalesGoalAdvisor,
/// XFAiFinancialConciliator, XFAiAccountingAssistant, XFAiFormAssistant.
/// Covers rendering, parameters, states, accessibility, and models.
/// </summary>
public class XFAiDomainComponentsTest : XForgeTestContext
{
    public XFAiDomainComponentsTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiNFeAnalyzer

    [Fact]
    public void NFeAnalyzer_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiNFeAnalyzer>();
        Assert.Contains("Analisador de NF-e", cut.Markup);
        Assert.Contains("Cole o XML da NF-e", cut.Markup);
    }

    [Fact]
    public void NFeAnalyzer_Renders_CustomTitle()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Title, "Fiscal AI"));
        Assert.Contains("Fiscal AI", cut.Markup);
    }

    [Fact]
    public void NFeAnalyzer_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void NFeAnalyzer_Renders_CustomPlaceholder()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Placeholder, "Custom XML input"));
        Assert.Contains("Custom XML input", cut.Markup);
    }

    [Fact]
    public void NFeAnalyzer_ShowsLoadingState()
    {
        var cut = Render<XFAiNFeAnalyzer>(p => p.Add(x => x.Loading, true));
        Assert.Contains("aria-busy", cut.Markup);
    }

    [Fact]
    public void NFeAnalyzer_AriaRoles_Correct()
    {
        var cut = Render<XFAiNFeAnalyzer>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    #endregion

    #region XFAiStockAdvisor

    [Fact]
    public void StockAdvisor_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiStockAdvisor>();
        Assert.Contains("Assistente de Estoque", cut.Markup);
        Assert.Contains("Pergunte sobre previsao de demanda", cut.Markup);
    }

    [Fact]
    public void StockAdvisor_Renders_CustomTitle()
    {
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Title, "Stock AI"));
        Assert.Contains("Stock AI", cut.Markup);
    }

    [Fact]
    public void StockAdvisor_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void StockAdvisor_ShowsLoadingState()
    {
        var cut = Render<XFAiStockAdvisor>(p => p.Add(x => x.Loading, true));
        Assert.Contains("aria-busy", cut.Markup);
    }

    [Fact]
    public void StockAdvisor_AriaRoles_Correct()
    {
        var cut = Render<XFAiStockAdvisor>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    #endregion

    #region XFAiSalesGoalAdvisor

    [Fact]
    public void SalesGoalAdvisor_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiSalesGoalAdvisor>();
        Assert.Contains("Assistente de Metas", cut.Markup);
        Assert.Contains("Pergunte sobre ranking", cut.Markup);
    }

    [Fact]
    public void SalesGoalAdvisor_Renders_CustomTitle()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Title, "Goals AI"));
        Assert.Contains("Goals AI", cut.Markup);
    }

    [Fact]
    public void SalesGoalAdvisor_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void SalesGoalAdvisor_ShowsLoadingState()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Loading, true));
        Assert.Contains("aria-busy", cut.Markup);
    }

    [Fact]
    public void SalesGoalAdvisor_Renders_CustomPlaceholder()
    {
        var cut = Render<XFAiSalesGoalAdvisor>(p => p.Add(x => x.Placeholder, "Custom goals query"));
        Assert.Contains("Custom goals query", cut.Markup);
    }

    #endregion

    #region XFAiFinancialConciliator

    [Fact]
    public void FinancialConciliator_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiFinancialConciliator>();
        Assert.Contains("Conciliador Financeiro", cut.Markup);
        Assert.Contains("Descreva lancamentos", cut.Markup);
    }

    [Fact]
    public void FinancialConciliator_Renders_CustomTitle()
    {
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.Title, "Conciliador AI"));
        Assert.Contains("Conciliador AI", cut.Markup);
    }

    [Fact]
    public void FinancialConciliator_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void FinancialConciliator_ShowsLoadingState()
    {
        var cut = Render<XFAiFinancialConciliator>(p => p.Add(x => x.Loading, true));
        Assert.Contains("aria-busy", cut.Markup);
    }

    [Fact]
    public void FinancialConciliator_AriaRoles_Correct()
    {
        var cut = Render<XFAiFinancialConciliator>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    #endregion

    #region XFAiAccountingAssistant

    [Fact]
    public void AccountingAssistant_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiAccountingAssistant>();
        Assert.Contains("Assistente Contabil", cut.Markup);
        Assert.Contains("Pergunte sobre lancamentos", cut.Markup);
    }

    [Fact]
    public void AccountingAssistant_Renders_CustomTitle()
    {
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.Title, "Contabilidade AI"));
        Assert.Contains("Contabilidade AI", cut.Markup);
    }

    [Fact]
    public void AccountingAssistant_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void AccountingAssistant_ShowsLoadingState()
    {
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.Loading, true));
        Assert.Contains("aria-busy", cut.Markup);
    }

    [Fact]
    public void AccountingAssistant_Renders_CustomEmptyContent()
    {
        RenderFragment customEmpty = builder =>
        {
            builder.OpenElement(0, "p");
            builder.AddContent(1, "Custom accounting empty");
            builder.CloseElement();
        };
        var cut = Render<XFAiAccountingAssistant>(p => p.Add(x => x.EmptyContent, customEmpty));
        Assert.Contains("Custom accounting empty", cut.Markup);
    }

    #endregion

    #region XFAiFormAssistant

    [Fact]
    public void FormAssistant_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiFormAssistant>();
        Assert.Contains("Assistente de Formularios", cut.Markup);
        Assert.Contains("Pergunte sobre preenchimento", cut.Markup);
    }

    [Fact]
    public void FormAssistant_Renders_CustomTitle()
    {
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.Title, "Form AI"));
        Assert.Contains("Form AI", cut.Markup);
    }

    [Fact]
    public void FormAssistant_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void FormAssistant_ShowsLoadingState()
    {
        var cut = Render<XFAiFormAssistant>(p => p.Add(x => x.Loading, true));
        Assert.Contains("aria-busy", cut.Markup);
    }

    [Fact]
    public void FormAssistant_AriaRoles_Correct()
    {
        var cut = Render<XFAiFormAssistant>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    #endregion

    #region Models

    [Fact]
    public void NFeAnalysisResult_HasDefaultValues()
    {
        var result = new XFAiNFeAnalysisResult();
        Assert.NotEmpty(result.Id);
        Assert.Equal(0m, result.TotalValue);
        Assert.Empty(result.Items);
        Assert.Empty(result.Inconsistencies);
        Assert.Equal(XFAiNFeRiskLevel.Low, result.RiskLevel);
    }

    [Fact]
    public void NFeItem_HasDefaultValues()
    {
        var item = new XFAiNFeItem();
        Assert.Equal(0, item.Sequence);
        Assert.Equal(0m, item.TotalValue);
    }

    [Fact]
    public void StockInsight_HasDefaultValues()
    {
        var result = new XFAiStockInsight();
        Assert.NotEmpty(result.Id);
        Assert.Empty(result.Forecasts);
        Assert.Empty(result.Alerts);
        Assert.Empty(result.ReorderSuggestions);
    }

    [Fact]
    public void SalesGoalInsight_HasDefaultValues()
    {
        var result = new XFAiSalesGoalInsight();
        Assert.NotEmpty(result.Id);
        Assert.Empty(result.Rankings);
        Assert.Empty(result.Goals);
        Assert.Empty(result.Recommendations);
    }

    [Fact]
    public void ReconciliationResult_HasDefaultValues()
    {
        var result = new XFAiReconciliationResult();
        Assert.NotEmpty(result.Id);
        Assert.Empty(result.Matches);
        Assert.Empty(result.UnmatchedTransactions);
        Assert.Empty(result.Duplicates);
        Assert.Empty(result.FraudIndicators);
    }

    [Fact]
    public void AccountingInsight_HasDefaultValues()
    {
        var result = new XFAiAccountingInsight();
        Assert.NotEmpty(result.Id);
        Assert.Empty(result.SuggestedEntries);
        Assert.Empty(result.Classifications);
        Assert.Empty(result.AuditItems);
    }

    [Fact]
    public void FormInsight_HasDefaultValues()
    {
        var result = new XFAiFormInsight();
        Assert.NotEmpty(result.Id);
        Assert.Empty(result.AutofillSuggestions);
        Assert.Empty(result.ValidationFixes);
        Assert.Empty(result.Inconsistencies);
    }

    #endregion
}
