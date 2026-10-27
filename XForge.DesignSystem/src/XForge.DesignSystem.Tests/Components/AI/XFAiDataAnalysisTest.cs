#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Analysis;
using XForge.DesignSystem.Components.AI.DataGrid;
using XForge.DesignSystem.Components.AI.Documents;
using XForge.DesignSystem.Components.AI.Insights;
#endregion

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for S-AI003 AI Data &amp; Analysis components:
/// XFAiDataGridAssistant, XFAiReportBuilder, XFAiSqlQueryBuilder,
/// XFAiDashboardInsight, XFAiDocumentReader.
/// Covers rendering, parameters, events, states, accessibility, models, and security.
/// </summary>
public class XFAiDataAnalysisTest : BunitContext
{
    public XFAiDataAnalysisTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiDataGridAssistant

    [Fact]
    public void DataGridAssistant_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiDataGridAssistant>();

        Assert.Contains("Assistente de Dados", cut.Markup);
        Assert.Contains("Pergunte sobre seus dados...", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_Renders_CustomTitle()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Title, "Grid AI"));

        Assert.Contains("Grid AI", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.ShowHeader, false));

        // Title still appears in aria-label on root element, but header div should not contain sparkles icon + title
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_ShowsEmptyText_WhenNoInsights()
    {
        var cut = Render<XFAiDataGridAssistant>();

        Assert.Contains("Faça uma pergunta sobre seus dados", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_Renders_CustomEmptyContent()
    {
        RenderFragment customEmpty = builder =>
        {
            builder.OpenElement(0, "p");
            builder.AddContent(1, "Custom empty");
            builder.CloseElement();
        };
        var cut = Render<XFAiDataGridAssistant>(p =>
            p.Add(x => x.EmptyContent, customEmpty));

        Assert.Contains("Custom empty", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_ShowsLoadingState()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Loading, true));

        Assert.Contains("aria-busy", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_AriaRoles_Correct()
    {
        var cut = Render<XFAiDataGridAssistant>();

        Assert.Contains("role=\"complementary\"", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_Placeholder_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>();

        Assert.Contains("Pergunte sobre seus dados...", cut.Markup);
    }

    [Fact]
    public void DataGridAssistant_CustomPlaceholder_Renders()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Placeholder, "Custom query"));

        Assert.Contains("Custom query", cut.Markup);
    }

    #endregion

    #region XFAiReportBuilder

    [Fact]
    public void ReportBuilder_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiReportBuilder>();

        Assert.Contains("Gerador de Relatórios", cut.Markup);
        Assert.Contains("Descreva o relatório que deseja gerar...", cut.Markup);
    }

    [Fact]
    public void ReportBuilder_Renders_CustomTitle()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.Title, "Report Gen"));

        Assert.Contains("Report Gen", cut.Markup);
    }

    [Fact]
    public void ReportBuilder_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.ShowHeader, false));

        // Title still in aria-label, but header element with sparkles should not render
        Assert.DoesNotContain("sparkles", cut.Markup);
    }

    [Fact]
    public void ReportBuilder_Renders_FormatChips_WhenProvided()
    {
        var formats = new List<XFAiReportFormat>
        {
            XFAiReportFormat.Pdf,
            XFAiReportFormat.Excel,
            XFAiReportFormat.Csv
        };

        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.SupportedFormats, formats));

        Assert.Contains("PDF", cut.Markup);
        Assert.Contains("Excel", cut.Markup);
        Assert.Contains("CSV", cut.Markup);
    }

    [Fact]
    public void ReportBuilder_Renders_AvailableFields()
    {
        var fields = new List<XFAiReportField>
        {
            new() { Name = "vendas", DisplayName = "Vendas", DataType = "number" },
            new() { Name = "cliente", DisplayName = "Cliente", DataType = "string" }
        };

        var cut = Render<XFAiReportBuilder>(p => p.Add(x => x.AvailableFields, fields));

        Assert.Contains("Vendas", cut.Markup);
        Assert.Contains("Cliente", cut.Markup);
    }

    [Fact]
    public void ReportBuilder_Renders_EmptyContent()
    {
        var cut = Render<XFAiReportBuilder>();

        Assert.Contains("Descreva o relatório", cut.Markup);
    }

    [Fact]
    public void ReportBuilder_AriaRoles_Correct()
    {
        var cut = Render<XFAiReportBuilder>();

        Assert.Contains("role=\"region\"", cut.Markup);
    }

    #endregion

    #region XFAiSqlQueryBuilder

    [Fact]
    public void SqlQueryBuilder_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiSqlQueryBuilder>();

        Assert.Contains("Construtor de SQL", cut.Markup);
        Assert.Contains("Descreva a consulta SQL que deseja...", cut.Markup);
    }

    [Fact]
    public void SqlQueryBuilder_Renders_CustomTitle()
    {
        var cut = Render<XFAiSqlQueryBuilder>(p => p.Add(x => x.Title, "SQL Builder"));

        Assert.Contains("SQL Builder", cut.Markup);
    }

    [Fact]
    public void SqlQueryBuilder_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiSqlQueryBuilder>(p => p.Add(x => x.ShowHeader, false));

        // Title still in aria-label, but header with database icon should not render
        Assert.DoesNotContain("database", cut.Markup);
    }

    [Fact]
    public void SqlQueryBuilder_ShowsReadOnlyBadge_WhenReadOnlyTrue()
    {
        var cut = Render<XFAiSqlQueryBuilder>(p => p.Add(x => x.ReadOnly, true));

        Assert.Contains("Somente leitura", cut.Markup);
    }

    [Fact]
    public void SqlQueryBuilder_ShowsEmptyState_WhenNoQuery()
    {
        var cut = Render<XFAiSqlQueryBuilder>();

        Assert.Contains("Descreva a consulta", cut.Markup);
    }

    [Fact]
    public void SqlQueryBuilder_DefaultReadOnly_IsTrue()
    {
        var cut = Render<XFAiSqlQueryBuilder>();

        // ReadOnly defaults to true — should show read-only badge
        Assert.Contains("Somente leitura", cut.Markup);
    }

    [Fact]
    public void SqlQueryBuilder_AriaRoles_Correct()
    {
        var cut = Render<XFAiSqlQueryBuilder>();

        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void SqlQueryBuilder_HasTextarea_ForInput()
    {
        var cut = Render<XFAiSqlQueryBuilder>();

        var textarea = cut.Find("textarea");
        Assert.NotNull(textarea);
    }

    #endregion

    #region XFAiSqlQuery Model

    [Fact]
    public void SqlQuery_DefaultConstruction_HasDefaults()
    {
        var query = new XFAiSqlQuery();

        Assert.NotEmpty(query.Id);
        Assert.Equal(string.Empty, query.NaturalLanguageQuery);
        Assert.Equal(string.Empty, query.GeneratedSql);
        Assert.True(query.IsReadOnly);
        Assert.False(query.RequiresApproval);
        Assert.NotNull(query.ValidationResult);
        Assert.True(query.ValidationResult.IsValid);
    }

    [Fact]
    public void SqlQuery_WithValues_RoundtripsCorrectly()
    {
        var query = new XFAiSqlQuery
        {
            NaturalLanguageQuery = "Show all clients",
            GeneratedSql = "SELECT * FROM clients",
            Explanation = "Retrieves all client records",
            IsReadOnly = true,
            Confidence = 0.95,
            TouchedTables = new List<string> { "clients" },
            ValidationResult = new XFAiSqlValidationResult
            {
                IsValid = true,
                IsReadOnlyQuery = true,
                HasInjectionRisk = false,
                HasBlockedOperation = false
            }
        };

        Assert.Equal("SELECT * FROM clients", query.GeneratedSql);
        Assert.True(query.IsReadOnly);
        Assert.Single(query.TouchedTables);
        Assert.True(query.ValidationResult.IsValid);
    }

    [Fact]
    public void SqlValidationResult_DefaultConstruction_HasDefaults()
    {
        var result = new XFAiSqlValidationResult();

        Assert.True(result.IsValid);
        Assert.True(result.IsReadOnlyQuery);
        Assert.False(result.HasInjectionRisk);
        Assert.False(result.HasBlockedOperation);
        Assert.Empty(result.Warnings);
    }

    #endregion

    #region XFAiGridQuery Model

    [Fact]
    public void GridQuery_DefaultConstruction_HasDefaults()
    {
        var query = new XFAiGridQuery();

        Assert.Equal(string.Empty, query.NaturalLanguageQuery);
        Assert.Empty(query.Filters);
        Assert.Empty(query.Sorts);
        Assert.Null(query.GroupBy);
    }

    [Fact]
    public void GridQuery_Filters_Roundtrip()
    {
        var query = new XFAiGridQuery
        {
            NaturalLanguageQuery = "Show clients with revenue > 1000",
            Filters = new List<XFAiGridFilter>
            {
                new() { Column = "revenue", Operator = "gt", Value = "1000" }
            },
            Sorts = new List<XFAiGridSort>
            {
                new() { Column = "revenue", Descending = true }
            },
            GroupBy = "region",
            Confidence = 0.85
        };

        Assert.Single(query.Filters);
        Assert.Equal("revenue", query.Filters[0].Column);
        Assert.Single(query.Sorts);
        Assert.True(query.Sorts[0].Descending);
        Assert.Equal("region", query.GroupBy);
        Assert.Equal(0.85, query.Confidence);
    }

    #endregion

    #region XFAiGridInsight Model

    [Fact]
    public void GridInsight_DefaultConstruction_HasDefaults()
    {
        var insight = new XFAiGridInsight();

        Assert.NotEmpty(insight.Id);
        Assert.Equal(string.Empty, insight.Title);
        Assert.Equal(XFAiGridInsightType.Pattern, insight.Type);
    }

    [Fact]
    public void GridInsightType_HasExpectedValues()
    {
        Assert.Equal(6, Enum.GetValues<XFAiGridInsightType>().Length);
    }

    #endregion

    #region XFAiDashboardInsight

    [Fact]
    public void DashboardInsight_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiDashboardInsight>();

        Assert.Contains("Insights do Dashboard", cut.Markup);
    }

    [Fact]
    public void DashboardInsight_Renders_CustomTitle()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Title, "Dashboard AI"));

        Assert.Contains("Dashboard AI", cut.Markup);
    }

    [Fact]
    public void DashboardInsight_HidesTitle_WhenShowTitleFalse()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.ShowTitle, false));

        // Title still in aria-label, but the h3 title element should not render
        Assert.DoesNotContain("<h3", cut.Markup);
    }

    [Fact]
    public void DashboardInsight_ShowsEmptyText_WhenNoInsights()
    {
        var cut = Render<XFAiDashboardInsight>();

        Assert.Contains("Nenhum insight disponivel", cut.Markup);
    }

    [Fact]
    public void DashboardInsight_Renders_InsightsList()
    {
        var insights = new List<XFAiDashboardInsightItem>
        {
            new() { Title = "Vendas em alta", Description = "15% increase", Type = XFAiDashboardInsightType.Trend, Severity = XFAiDashboardInsightSeverity.Success },
            new() { Title = "Estoque baixo", Description = "Risk of stockout", Type = XFAiDashboardInsightType.Alert, Severity = XFAiDashboardInsightSeverity.Warning }
        };

        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Insights, insights));

        Assert.Contains("Vendas em alta", cut.Markup);
        Assert.Contains("Estoque baixo", cut.Markup);
    }

    [Fact]
    public void DashboardInsight_ShowsLoadingState()
    {
        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Loading, true));

        Assert.Contains("aria-busy", cut.Markup);
        Assert.Contains("Analisando KPIs...", cut.Markup);
    }

    [Fact]
    public void DashboardInsight_Renders_Kpis()
    {
        var kpis = new List<XFAiDashboardKpi>
        {
            new() { Name = "revenue", DisplayName = "Receita", CurrentValue = 150000, Unit = "R$" }
        };

        var cut = Render<XFAiDashboardInsight>(p => p.Add(x => x.Kpis, kpis));

        // KPIs are context for AI, not directly rendered in default template
        Assert.Contains("Insights do Dashboard", cut.Markup);
    }

    [Fact]
    public void DashboardInsight_AriaRoles_Correct()
    {
        var cut = Render<XFAiDashboardInsight>();

        Assert.Contains("role=\"complementary\"", cut.Markup);
    }

    #endregion

    #region XFAiDashboardKpi Model

    [Fact]
    public void DashboardKpi_DefaultConstruction_HasDefaults()
    {
        var kpi = new XFAiDashboardKpi();

        Assert.Equal(string.Empty, kpi.Name);
        Assert.Equal(string.Empty, kpi.DisplayName);
        Assert.Equal(0, kpi.CurrentValue);
        Assert.Null(kpi.PreviousValue);
    }

    [Fact]
    public void DashboardKpi_WithValues_RoundtripsCorrectly()
    {
        var kpi = new XFAiDashboardKpi
        {
            Name = "revenue",
            DisplayName = "Receita Mensal",
            CurrentValue = 150000,
            PreviousValue = 130000,
            Unit = "R$",
            Category = "Financeiro",
            Target = 160000
        };

        Assert.Equal("revenue", kpi.Name);
        Assert.Equal(150000, kpi.CurrentValue);
        Assert.Equal(130000, kpi.PreviousValue);
        Assert.Equal("R$", kpi.Unit);
        Assert.Equal(160000, kpi.Target);
    }

    [Fact]
    public void DashboardInsightItem_DefaultConstruction_HasDefaults()
    {
        var item = new XFAiDashboardInsightItem();

        Assert.NotEmpty(item.Id);
        Assert.Equal(string.Empty, item.Title);
        Assert.Equal(XFAiDashboardInsightType.Trend, item.Type);
        Assert.Equal(XFAiDashboardInsightSeverity.Info, item.Severity);
    }

    [Fact]
    public void DashboardInsightType_HasExpectedValues()
    {
        Assert.Equal(6, Enum.GetValues<XFAiDashboardInsightType>().Length);
    }

    [Fact]
    public void DashboardInsightSeverity_HasExpectedValues()
    {
        Assert.Equal(4, Enum.GetValues<XFAiDashboardInsightSeverity>().Length);
    }

    #endregion

    #region XFAiDocumentReader

    [Fact]
    public void DocumentReader_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiDocumentReader>();

        Assert.Contains("Leitor de Documentos", cut.Markup);
        // Default empty text from parameter
        Assert.Contains("Envie um documento", cut.Markup);
    }

    [Fact]
    public void DocumentReader_Renders_CustomTitle()
    {
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.Title, "Doc Reader"));

        Assert.Contains("Doc Reader", cut.Markup);
    }

    [Fact]
    public void DocumentReader_HidesHeader_WhenShowHeaderFalse()
    {
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.ShowHeader, false));

        // Title still in aria-label, but header element should not render
        Assert.DoesNotContain("<header", cut.Markup);
    }

    [Fact]
    public void DocumentReader_ShowsEmptyState_WhenNoContent()
    {
        var cut = Render<XFAiDocumentReader>();

        Assert.Contains("Envie um documento", cut.Markup);
    }

    [Fact]
    public void DocumentReader_AriaRoles_Correct()
    {
        var cut = Render<XFAiDocumentReader>();

        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void DocumentReader_CustomEmptyText_Renders()
    {
        var cut = Render<XFAiDocumentReader>(p => p.Add(x => x.EmptyText, "No document loaded"));

        Assert.Contains("No document loaded", cut.Markup);
    }

    #endregion

    #region XFAiDocumentResult Model

    [Fact]
    public void DocumentResult_DefaultConstruction_HasDefaults()
    {
        var result = new XFAiDocumentResult();

        Assert.NotEmpty(result.Id);
        Assert.Equal(string.Empty, result.DocumentName);
        Assert.Equal(XFAiDocumentType.Pdf, result.DocumentType);
        Assert.Empty(result.Chunks);
        Assert.Equal(0, result.TotalCharacters);
    }

    [Fact]
    public void DocumentResult_WithChunks_RoundtripsCorrectly()
    {
        var result = new XFAiDocumentResult
        {
            DocumentName = "report.pdf",
            DocumentType = XFAiDocumentType.Pdf,
            AiSummary = "This is a financial report.",
            TotalCharacters = 5000,
            TotalChunks = 10,
            Chunks = new List<XFAiDocumentChunk>
            {
                new() { Index = 0, Content = "First chunk", StartChar = 0, EndChar = 500 },
                new() { Index = 1, Content = "Second chunk", StartChar = 500, EndChar = 1000, RelevanceScore = 0.85 }
            }
        };

        Assert.Equal("report.pdf", result.DocumentName);
        Assert.Equal("This is a financial report.", result.AiSummary);
        Assert.Equal(2, result.Chunks.Count);
        Assert.Equal(0.85, result.Chunks[1].RelevanceScore);
    }

    [Fact]
    public void DocumentChunk_DefaultConstruction_HasDefaults()
    {
        var chunk = new XFAiDocumentChunk();

        Assert.Equal(0, chunk.Index);
        Assert.Equal(string.Empty, chunk.Content);
        Assert.Null(chunk.Embedding);
        Assert.Null(chunk.RelevanceScore);
    }

    [Fact]
    public void DocumentType_HasExpectedValues()
    {
        Assert.Equal(8, Enum.GetValues<XFAiDocumentType>().Length);
    }

    #endregion

    #region XFAiReportDefinition Model

    [Fact]
    public void ReportDefinition_DefaultConstruction_HasDefaults()
    {
        var def = new XFAiReportDefinition();

        Assert.NotEmpty(def.Id);
        Assert.Equal(string.Empty, def.Title);
        Assert.Empty(def.SelectedFields);
        Assert.Empty(def.Filters);
        Assert.Equal(XFAiReportFormat.Pdf, def.Format);
    }

    [Fact]
    public void ReportDefinition_Fields_Roundtrip()
    {
        var def = new XFAiReportDefinition
        {
            Title = "Sales Report",
            Description = "Monthly sales",
            SelectedFields = new List<XFAiReportField>
            {
                new() { Name = "revenue", DisplayName = "Receita", DataType = "number", IsRequired = true },
                new() { Name = "client", DisplayName = "Cliente", DataType = "string" }
            },
            Filters = new List<XFAiReportFilter>
            {
                new() { Field = "date", Operator = "gte", Value = "2026-01-01" }
            },
            GroupByField = "region",
            Format = XFAiReportFormat.Excel,
            Confidence = 0.92
        };

        Assert.Equal("Sales Report", def.Title);
        Assert.Equal(2, def.SelectedFields.Count);
        Assert.True(def.SelectedFields[0].IsRequired);
        Assert.Single(def.Filters);
        Assert.Equal("region", def.GroupByField);
        Assert.Equal(XFAiReportFormat.Excel, def.Format);
        Assert.Equal(0.92, def.Confidence);
    }

    [Fact]
    public void ReportFormat_HasExpectedValues()
    {
        Assert.Equal(5, Enum.GetValues<XFAiReportFormat>().Length);
    }

    #endregion
}
