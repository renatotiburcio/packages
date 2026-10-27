#region Using directives
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.AI.Utility;
using XForge.DesignSystem.Components.AI.Audit;
#endregion

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for S-AI005 AI Utility components:
/// XFAiRiskBadge, XFAiExplainButton, XFAiRewriteButton,
/// XFAiValidationSummary, XFAiAuditTimeline, XFAiHumanApprovalCard,
/// XFAiSourceCitationList, XFAiMemoryViewer.
/// Covers rendering, parameters, states, accessibility, and models.
/// </summary>
public class XFAiUtilityComponentsTest : BunitContext
{
    public XFAiUtilityComponentsTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiRiskBadge

    [Fact]
    public void RiskBadge_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiRiskBadge>();
        Assert.Contains("Baixo", cut.Markup);
        Assert.Contains("xf-ai-risk-badge", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Renders_CriticalLevel()
    {
        var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Level, XFAiRiskLevel.Critical));
        Assert.Contains("Critico", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Renders_CustomLabel()
    {
        var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Label, "Risco Alto"));
        Assert.Contains("Risco Alto", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Renders_WithScore()
    {
        var cut = Render<XFAiRiskBadge>(p =>
        {
            p.Add(x => x.Score, 0.85);
            p.Add(x => x.ShowScore, true);
        });
        Assert.Contains("85%", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Renders_FiscalContext()
    {
        var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Context, XFAiRiskContext.Fiscal));
        Assert.Contains("Fiscal", cut.Markup);
    }

    [Fact]
    public void RiskBadge_AriaRoles_Correct()
    {
        var cut = Render<XFAiRiskBadge>();
        Assert.Contains("role=\"status\"", cut.Markup);
        Assert.Contains("aria-label", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Renders_SmallSize()
    {
        var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Size, XFAiRiskBadgeSize.Sm));
        Assert.Contains("text-[10px]", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Renders_LargeSize()
    {
        var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Size, XFAiRiskBadgeSize.Lg));
        Assert.Contains("text-sm", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Renders_AllRiskLevels()
    {
        foreach (var level in Enum.GetValues<XFAiRiskLevel>())
        {
            var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Level, level));
            Assert.Contains("xf-ai-risk-badge", cut.Markup);
        }
    }

    [Fact]
    public void RiskBadge_Renders_AllContexts()
    {
        foreach (var ctx in Enum.GetValues<XFAiRiskContext>())
        {
            var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Context, ctx));
            Assert.Contains("xf-ai-risk-badge", cut.Markup);
        }
    }

    [Fact]
    public void RiskBadge_Renders_DataAttributes()
    {
        var cut = Render<XFAiRiskBadge>(p =>
        {
            p.Add(x => x.Level, XFAiRiskLevel.High);
            p.Add(x => x.Context, XFAiRiskContext.Fraud);
        });
        Assert.Contains("data-risk=\"high\"", cut.Markup);
        Assert.Contains("data-context=\"fraud\"", cut.Markup);
    }

    [Fact]
    public void RiskBadge_Has_CustomId()
    {
        var cut = Render<XFAiRiskBadge>(p => p.Add(x => x.Id, "my-risk"));
        Assert.Contains("id=\"my-risk\"", cut.Markup);
    }

    #endregion

    #region XFAiExplainButton

    [Fact]
    public void ExplainButton_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiExplainButton>();
        Assert.Contains("xf-ai-explain-button", cut.Markup);
        Assert.Contains("Explicar", cut.Markup);
    }

    [Fact]
    public void ExplainButton_HidesLabel_WhenShowLabelFalse()
    {
        var cut = Render<XFAiExplainButton>(p => p.Add(x => x.ShowLabel, false));
        // Label text should not appear as visible button text, but aria-label still contains it
        Assert.DoesNotContain("<span>Explicar</span>", cut.Markup);
    }

    [Fact]
    public void ExplainButton_Renders_CustomAriaLabel()
    {
        var cut = Render<XFAiExplainButton>(p => p.Add(x => x.AriaLabel, "Help me understand"));
        Assert.Contains("aria-label=\"Help me understand\"", cut.Markup);
    }

    [Fact]
    public void ExplainButton_Disabled_WhenNoProvider()
    {
        var cut = Render<XFAiExplainButton>();
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void ExplainButton_Disabled_WhenNoContext()
    {
        var cut = Render<XFAiExplainButton>();
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void ExplainButton_AriaRoles_Correct()
    {
        var cut = Render<XFAiExplainButton>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void ExplainButton_Renders_GhostVariant()
    {
        var cut = Render<XFAiExplainButton>(p => p.Add(x => x.Variant, XFAiExplainVariant.Ghost));
        Assert.Contains("xf-ai-explain-button", cut.Markup);
    }

    [Fact]
    public void ExplainButton_Has_CustomId()
    {
        var cut = Render<XFAiExplainButton>(p => p.Add(x => x.Id, "my-explain"));
        Assert.Contains("id=\"my-explain\"", cut.Markup);
    }

    #endregion

    #region XFAiRewriteButton

    [Fact]
    public void RewriteButton_Renders_WithDefaultParameters()
    {
        var cut = Render<XFAiRewriteButton>();
        Assert.Contains("xf-ai-rewrite-button", cut.Markup);
        Assert.Contains("Formalizar", cut.Markup);
    }

    [Fact]
    public void RewriteButton_Renders_CustomModes()
    {
        var cut = Render<XFAiRewriteButton>(p => p.Add(x => x.AvailableModes,
            new[] { XFAiRewriteMode.Translate, XFAiRewriteMode.Summarize }));
        Assert.Contains("Traduzir", cut.Markup);
        Assert.Contains("Resumir", cut.Markup);
        Assert.DoesNotContain("Formalizar", cut.Markup);
    }

    [Fact]
    public void RewriteButton_Renders_CustomTitle()
    {
        var cut = Render<XFAiRewriteButton>(p => p.Add(x => x.Title, "Rewrite Text"));
        Assert.Contains("Rewrite Text", cut.Markup);
    }

    [Fact]
    public void RewriteButton_Disabled_WhenNoProvider()
    {
        var cut = Render<XFAiRewriteButton>();
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void RewriteButton_Has_CustomId()
    {
        var cut = Render<XFAiRewriteButton>(p => p.Add(x => x.Id, "my-rewrite"));
        Assert.Contains("id=\"my-rewrite\"", cut.Markup);
    }

    #endregion

    #region XFAiValidationSummary

    [Fact]
    public void ValidationSummary_Renders_EmptyState()
    {
        var cut = Render<XFAiValidationSummary>();
        Assert.Contains("Nenhuma validacao pendente", cut.Markup);
    }

    [Fact]
    public void ValidationSummary_Renders_WithItems()
    {
        var items = new List<XFAiValidationItem>
        {
            new() { FieldName = "CNPJ", Message = "CNPJ invalido", Severity = XFAiValidationSeverity.Error },
            new() { FieldName = "Email", Message = "Email obrigatorio", Severity = XFAiValidationSeverity.Warning }
        };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, items));
        Assert.Contains("CNPJ", cut.Markup);
        Assert.Contains("Email", cut.Markup);
    }

    [Fact]
    public void ValidationSummary_Renders_CustomTitle()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Title, "Erros de validacao"));
        Assert.Contains("Erros de validacao", cut.Markup);
    }

    [Fact]
    public void ValidationSummary_ShowsCount()
    {
        var items = new List<XFAiValidationItem>
        {
            new() { FieldName = "A", Message = "Error 1", Severity = XFAiValidationSeverity.Error },
            new() { FieldName = "B", Message = "Error 2", Severity = XFAiValidationSeverity.Error }
        };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, items));
        Assert.Contains("2", cut.Markup);
    }

    [Fact]
    public void ValidationSummary_AriaRoles_Correct()
    {
        var cut = Render<XFAiValidationSummary>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void ValidationSummary_Renders_SeverityBadges()
    {
        var items = new List<XFAiValidationItem>
        {
            new() { FieldName = "Field", Message = "Critical error", Severity = XFAiValidationSeverity.Critical }
        };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, items));
        Assert.Contains("Critico", cut.Markup);
    }

    [Fact]
    public void ValidationSummary_Renders_AiExplanation()
    {
        var items = new List<XFAiValidationItem>
        {
            new() { FieldName = "CNPJ", Message = "Invalido", Severity = XFAiValidationSeverity.Error,
                    AiExplanation = "O CNPJ deve ter 14 digitos", AiFixSuggestion = "Verifique o numero" }
        };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, items));
        Assert.Contains("O CNPJ deve ter 14 digitos", cut.Markup);
        Assert.Contains("Verifique o numero", cut.Markup);
    }

    [Fact]
    public void ValidationSummary_Has_CustomId()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Id, "my-validation"));
        Assert.Contains("id=\"my-validation\"", cut.Markup);
    }

    #endregion

    #region XFAiAuditTimeline

    [Fact]
    public void AuditTimeline_Renders_EmptyState()
    {
        var cut = Render<XFAiAuditTimeline>();
        Assert.Contains("Nenhuma entrada de auditoria", cut.Markup);
    }

    [Fact]
    public void AuditTimeline_Renders_WithEntries()
    {
        var entries = new List<XFAiAuditEntry>
        {
            new() { Action = "Nota fiscal emitida", Actor = "user@example.com", Timestamp = new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc) },
            new() { Action = "Analise IA concluida", Actor = "AI", IsAiAction = true, Timestamp = new DateTime(2026, 5, 20, 10, 5, 0, DateTimeKind.Utc) }
        };
        var cut = Render<XFAiAuditTimeline>(p => p.Add(x => x.Entries, entries));
        Assert.Contains("Nota fiscal emitida", cut.Markup);
        Assert.Contains("Analise IA concluida", cut.Markup);
    }

    [Fact]
    public void AuditTimeline_Renders_CustomTitle()
    {
        var cut = Render<XFAiAuditTimeline>(p => p.Add(x => x.Title, "Historico"));
        Assert.Contains("Historico", cut.Markup);
    }

    [Fact]
    public void AuditTimeline_AriaRoles_Correct()
    {
        var cut = Render<XFAiAuditTimeline>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void AuditTimeline_ShowsBeforeAfter()
    {
        var entries = new List<XFAiAuditEntry>
        {
            new() { Action = "Campo alterado", Actor = "user@test.com",
                    BeforeState = "valor_antigo", AfterState = "valor_novo" }
        };
        var cut = Render<XFAiAuditTimeline>(p =>
        {
            p.Add(x => x.Entries, entries);
            p.Add(x => x.ShowDiff, true);
        });
        Assert.Contains("Antes", cut.Markup);
        Assert.Contains("Depois", cut.Markup);
    }

    [Fact]
    public void AuditTimeline_ShowsHash()
    {
        var entries = new List<XFAiAuditEntry>
        {
            new() { Action = "Teste", Actor = "System", Hash = "abc123def456789012345678", Signature = "sig123" }
        };
        var cut = Render<XFAiAuditTimeline>(p =>
        {
            p.Add(x => x.Entries, entries);
            p.Add(x => x.ShowHash, true);
        });
        Assert.Contains("abc123def456789", cut.Markup);
        Assert.Contains("Assinado", cut.Markup);
    }

    [Fact]
    public void AuditTimeline_Has_CustomId()
    {
        var cut = Render<XFAiAuditTimeline>(p => p.Add(x => x.Id, "my-audit"));
        Assert.Contains("id=\"my-audit\"", cut.Markup);
    }

    #endregion

    #region XFAiHumanApprovalCard

    [Fact]
    public void ApprovalCard_Renders_WithRequest()
    {
        var request = new XFAiApprovalRequest
        {
            Title = "Excluir registro fiscal",
            Description = "Remover NF-e #12345 do sistema",
            RequestedBy = "AI Assistant",
            RiskScore = 0.75,
            Category = "Fiscal"
        };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("Excluir registro fiscal", cut.Markup);
        Assert.Contains("Remover NF-e #12345", cut.Markup);
        Assert.Contains("Fiscal", cut.Markup);
    }

    [Fact]
    public void ApprovalCard_ShowsRiskScore()
    {
        var request = new XFAiApprovalRequest { Title = "Test", RiskScore = 0.85 };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("85%", cut.Markup);
    }

    [Fact]
    public void ApprovalCard_ShowsImpactAnalysis()
    {
        var request = new XFAiApprovalRequest
        {
            Title = "Test",
            ImpactAnalysis = "Afetara 50 notas fiscais"
        };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("Afetara 50 notas fiscais", cut.Markup);
    }

    [Fact]
    public void ApprovalCard_ShowsAffectedEntities()
    {
        var request = new XFAiApprovalRequest
        {
            Title = "Test",
            AffectedEntities = new[] { "NF-e #001", "NF-e #002" }
        };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("NF-e #001", cut.Markup);
        Assert.Contains("NF-e #002", cut.Markup);
    }

    [Fact]
    public void ApprovalCard_AriaRoles_Correct()
    {
        var request = new XFAiApprovalRequest { Title = "Test" };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void ApprovalCard_ShowsApproveRejectButtons()
    {
        var request = new XFAiApprovalRequest { Title = "Test" };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("Aprovar", cut.Markup);
        Assert.Contains("Rejeitar", cut.Markup);
    }

    [Fact]
    public void ApprovalCard_ShowsEditButton()
    {
        var request = new XFAiApprovalRequest { Title = "Test" };
        var cut = Render<XFAiHumanApprovalCard>(p =>
        {
            p.Add(x => x.Request, request);
            p.Add(x => x.ShowEditOption, true);
        });
        Assert.Contains("Editar", cut.Markup);
    }

    [Fact]
    public void ApprovalCard_Has_CustomId()
    {
        var request = new XFAiApprovalRequest { Title = "Test" };
        var cut = Render<XFAiHumanApprovalCard>(p =>
        {
            p.Add(x => x.Request, request);
            p.Add(x => x.Id, "my-approval");
        });
        Assert.Contains("id=\"my-approval\"", cut.Markup);
    }

    #endregion

    #region XFAiSourceCitationList

    [Fact]
    public void CitationList_Renders_EmptyState()
    {
        var cut = Render<XFAiSourceCitationList>();
        Assert.Contains("Nenhuma fonte citada", cut.Markup);
    }

    [Fact]
    public void CitationList_Renders_WithCitations()
    {
        var citations = new List<XFAiSourceCitation>
        {
            new() { Title = "RICMS-SP Art. 42", Rank = 1, Confidence = 0.95, SourceType = "Lei" },
            new() { Title = "Manual SAP", Rank = 2, Confidence = 0.80, SourceType = "Manual" }
        };
        var cut = Render<XFAiSourceCitationList>(p => p.Add(x => x.Citations, citations));
        Assert.Contains("RICMS-SP Art. 42", cut.Markup);
        Assert.Contains("Manual SAP", cut.Markup);
    }

    [Fact]
    public void CitationList_Renders_CustomTitle()
    {
        var cut = Render<XFAiSourceCitationList>(p => p.Add(x => x.Title, "Referencias"));
        Assert.Contains("Referencias", cut.Markup);
    }

    [Fact]
    public void CitationList_ShowsConfidence()
    {
        var citations = new List<XFAiSourceCitation>
        {
            new() { Title = "Source", Rank = 1, Confidence = 0.90 }
        };
        var cut = Render<XFAiSourceCitationList>(p =>
        {
            p.Add(x => x.Citations, citations);
            p.Add(x => x.ShowConfidence, true);
        });
        Assert.Contains("90%", cut.Markup);
    }

    [Fact]
    public void CitationList_ShowsSnippet()
    {
        var citations = new List<XFAiSourceCitation>
        {
            new() { Title = "Source", Rank = 1, Snippet = "Relevant text excerpt" }
        };
        var cut = Render<XFAiSourceCitationList>(p =>
        {
            p.Add(x => x.Citations, citations);
            p.Add(x => x.ShowSnippets, true);
        });
        Assert.Contains("Relevant text excerpt", cut.Markup);
    }

    [Fact]
    public void CitationList_AriaRoles_Correct()
    {
        var cut = Render<XFAiSourceCitationList>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void CitationList_RendersSourceTypeBadge()
    {
        var citations = new List<XFAiSourceCitation>
        {
            new() { Title = "Source", Rank = 1, SourceType = "Documento" }
        };
        var cut = Render<XFAiSourceCitationList>(p => p.Add(x => x.Citations, citations));
        Assert.Contains("Documento", cut.Markup);
    }

    [Fact]
    public void CitationList_Has_CustomId()
    {
        var cut = Render<XFAiSourceCitationList>(p => p.Add(x => x.Id, "my-citations"));
        Assert.Contains("id=\"my-citations\"", cut.Markup);
    }

    #endregion

    #region XFAiMemoryViewer

    [Fact]
    public void MemoryViewer_Renders_EmptyState()
    {
        var cut = Render<XFAiMemoryViewer>();
        Assert.Contains("Nenhuma memoria registrada", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_Renders_WithMemories()
    {
        var memories = new List<XFAiMemoryEntry>
        {
            new() { Content = "Usuario prefere relatorios em PDF", Type = XFAiMemoryType.Preferences, Relevance = 0.9 },
            new() { Content = "NFe #12345 emitida em 20/05/2026", Type = XFAiMemoryType.LongTerm, Relevance = 0.7 }
        };
        var cut = Render<XFAiMemoryViewer>(p => p.Add(x => x.Memories, memories));
        Assert.Contains("Usuario prefere relatorios em PDF", cut.Markup);
        Assert.Contains("NFe #12345 emitida", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_Renders_CustomTitle()
    {
        var cut = Render<XFAiMemoryViewer>(p => p.Add(x => x.Title, "Memory Board"));
        Assert.Contains("Memory Board", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_ShowsStats()
    {
        var memories = new List<XFAiMemoryEntry>
        {
            new() { Content = "Test", Type = XFAiMemoryType.Session, Relevance = 0.8, AccessCount = 5 }
        };
        var cut = Render<XFAiMemoryViewer>(p =>
        {
            p.Add(x => x.Memories, memories);
            p.Add(x => x.ShowStats, true);
        });
        Assert.Contains("Total:", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_ShowsRelevanceBar()
    {
        var memories = new List<XFAiMemoryEntry>
        {
            new() { Content = "Test", Type = XFAiMemoryType.Semantic, Relevance = 0.85 }
        };
        var cut = Render<XFAiMemoryViewer>(p =>
        {
            p.Add(x => x.Memories, memories);
            p.Add(x => x.ShowRelevance, true);
        });
        Assert.Contains("85%", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_AriaRoles_Correct()
    {
        var cut = Render<XFAiMemoryViewer>();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_Renders_TypeLabels()
    {
        var memories = new List<XFAiMemoryEntry>
        {
            new() { Content = "Test", Type = XFAiMemoryType.Session },
            new() { Content = "Test2", Type = XFAiMemoryType.Semantic }
        };
        var cut = Render<XFAiMemoryViewer>(p => p.Add(x => x.Memories, memories));
        Assert.Contains("Sessao", cut.Markup);
        Assert.Contains("Semantica", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_Renders_Tags()
    {
        var memories = new List<XFAiMemoryEntry>
        {
            new() { Content = "Test", Type = XFAiMemoryType.Factual, Tags = new[] { "fiscal", "nfe" } }
        };
        var cut = Render<XFAiMemoryViewer>(p => p.Add(x => x.Memories, memories));
        Assert.Contains("#fiscal", cut.Markup);
        Assert.Contains("#nfe", cut.Markup);
    }

    [Fact]
    public void MemoryViewer_Has_CustomId()
    {
        var cut = Render<XFAiMemoryViewer>(p => p.Add(x => x.Id, "my-memory"));
        Assert.Contains("id=\"my-memory\"", cut.Markup);
    }

    #endregion

    #region Model Tests

    [Fact]
    public void XFAiRiskLevel_HasCorrectValues()
    {
        Assert.Equal(0, (int)XFAiRiskLevel.Low);
        Assert.Equal(1, (int)XFAiRiskLevel.Medium);
        Assert.Equal(2, (int)XFAiRiskLevel.High);
        Assert.Equal(3, (int)XFAiRiskLevel.Critical);
    }

    [Fact]
    public void XFAiRiskContext_HasCorrectValues()
    {
        Assert.Equal(0, (int)XFAiRiskContext.General);
        Assert.Equal(1, (int)XFAiRiskContext.Credit);
        Assert.Equal(2, (int)XFAiRiskContext.Fiscal);
        Assert.Equal(3, (int)XFAiRiskContext.Stock);
        Assert.Equal(4, (int)XFAiRiskContext.Fraud);
        Assert.Equal(5, (int)XFAiRiskContext.Delinquency);
    }

    [Fact]
    public void XFAiRewriteMode_HasCorrectValues()
    {
        Assert.Equal(0, (int)XFAiRewriteMode.Formalize);
        Assert.Equal(5, (int)XFAiRewriteMode.Translate);
    }

    [Fact]
    public void XFAiValidationSeverity_HasCorrectValues()
    {
        Assert.Equal(0, (int)XFAiValidationSeverity.Info);
        Assert.Equal(3, (int)XFAiValidationSeverity.Critical);
    }

    [Fact]
    public void XFAiApprovalDecision_HasCorrectValues()
    {
        Assert.Equal(0, (int)XFAiApprovalDecision.Pending);
        Assert.Equal(1, (int)XFAiApprovalDecision.Approved);
        Assert.Equal(2, (int)XFAiApprovalDecision.Rejected);
        Assert.Equal(3, (int)XFAiApprovalDecision.Edited);
    }

    [Fact]
    public void XFAiMemoryType_HasCorrectValues()
    {
        Assert.Equal(0, (int)XFAiMemoryType.Session);
        Assert.Equal(4, (int)XFAiMemoryType.Factual);
    }

    [Fact]
    public void XFAiAuditEntry_DefaultValues()
    {
        var entry = new XFAiAuditEntry();
        Assert.False(string.IsNullOrEmpty(entry.Id));
        Assert.NotNull(entry.Action);
        Assert.Equal(0, entry.Version);
    }

    [Fact]
    public void XFAiApprovalRequest_DefaultValues()
    {
        var request = new XFAiApprovalRequest();
        Assert.False(string.IsNullOrEmpty(request.Id));
        Assert.Equal(0.0, request.RiskScore);
    }

    [Fact]
    public void XFAiSourceCitation_DefaultValues()
    {
        var citation = new XFAiSourceCitation();
        Assert.False(string.IsNullOrEmpty(citation.Id));
        Assert.Equal(0, citation.Rank);
    }

    [Fact]
    public void XFAiMemoryEntry_DefaultValues()
    {
        var entry = new XFAiMemoryEntry();
        Assert.False(string.IsNullOrEmpty(entry.Id));
        Assert.Equal(0, entry.AccessCount);
    }

    [Fact]
    public void XFAiExplainResult_DefaultValues()
    {
        var result = new XFAiExplainResult();
        Assert.Equal(string.Empty, result.ElementName);
        Assert.Equal(string.Empty, result.Explanation);
        Assert.Equal(0.0, result.Confidence);
    }

    [Fact]
    public void XFAiRewriteResult_DefaultValues()
    {
        var result = new XFAiRewriteResult();
        Assert.Equal(string.Empty, result.OriginalText);
        Assert.Equal(string.Empty, result.RewrittenText);
    }

    [Fact]
    public void XFAiValidationItem_DefaultValues()
    {
        var item = new XFAiValidationItem();
        Assert.False(string.IsNullOrEmpty(item.Id));
        Assert.False(item.IsResolved);
    }

    [Fact]
    public void XFAiAuditEntry_WithHashAndSignature()
    {
        var entry = new XFAiAuditEntry
        {
            Hash = "abc123",
            Signature = "sig456",
            Version = 3
        };
        Assert.Equal("abc123", entry.Hash);
        Assert.Equal("sig456", entry.Signature);
        Assert.Equal(3, entry.Version);
    }

    [Fact]
    public void XFAiRiskBadgeContext_Properties()
    {
        var ctx = new XFAiRiskBadgeContext
        {
            Level = XFAiRiskLevel.High,
            Context = XFAiRiskContext.Fiscal,
            Label = "Custom"
        };
        Assert.Equal(XFAiRiskLevel.High, ctx.Level);
        Assert.Equal(XFAiRiskContext.Fiscal, ctx.Context);
        Assert.Equal("Custom", ctx.Label);
    }

    [Fact]
    public void XFAiApprovalEditResult_Properties()
    {
        var result = new XFAiApprovalEditResult
        {
            Request = new XFAiApprovalRequest { Title = "Test" },
            EditedPayload = "modified payload",
            ReviewerNotes = "Fixed formatting"
        };
        Assert.Equal("Test", result.Request.Title);
        Assert.Equal("modified payload", result.EditedPayload);
        Assert.Equal("Fixed formatting", result.ReviewerNotes);
    }

    #endregion
}
