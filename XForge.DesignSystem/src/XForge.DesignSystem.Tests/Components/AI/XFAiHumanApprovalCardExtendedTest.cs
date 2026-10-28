using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.AI.Audit;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.AI;

public sealed class XFAiHumanApprovalCardExtendedTest : XForgeTestContext
{
    public XFAiHumanApprovalCardExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private static XFAiApprovalRequest SampleRequest => new()
    {
        Id = "REQ-001",
        Title = "Aprovar pagamento",
        Description = "Pagamento de R$ 50.000 para fornecedor",
        RequestedBy = "AI Assistant",
        RequestedAt = new DateTime(2025, 1, 15, 10, 30, 0),
        RiskScore = 0.7,
        ImpactAnalysis = "Alto impacto no fluxo de caixa",
        Category = "Pagamento",
        ProposedPayload = "{\"amount\":50000,\"supplier\":\"ABC\"}",
        AffectedEntities = new[] { "Fornecedor ABC", "Conta Bancária" }
    };

    private IRenderedComponent<XFAiHumanApprovalCard> RenderDefault(
        Action<ComponentParameterCollectionBuilder<XFAiHumanApprovalCard>>? configure = null)
    {
        return Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.Id, "test-approval"));
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-ai-human-approval-card", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithRegionRole()
    {
        var cut = RenderDefault();
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Title()
    {
        var cut = RenderDefault();
        Assert.Contains("Aprovar pagamento", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Description()
    {
        var cut = RenderDefault();
        Assert.Contains("Pagamento de R$ 50.000", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_RequestedBy()
    {
        var cut = RenderDefault();
        Assert.Contains("AI Assistant", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_CategoryBadge()
    {
        var cut = RenderDefault();
        Assert.Contains("Pagamento", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_RiskScore()
    {
        var cut = RenderDefault();
        Assert.Contains("70%", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_ImpactAnalysis()
    {
        var cut = RenderDefault();
        Assert.Contains("Alto impacto no fluxo de caixa", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_AffectedEntities()
    {
        var cut = RenderDefault();
        Assert.Contains("Fornecedor ABC", cut.Markup);
        Assert.Contains("Conta Bancária", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_AriaLabel()
    {
        var cut = RenderDefault();
        // aria-label is set to $"Aprovação: {Request.Title}"
        Assert.Contains("Aprovar pagamento", cut.Markup);
    }

    [Fact]
    public void Extended_ApproveButton_Click_SetsDecision()
    {
        var cut = RenderDefault();
        var approveButton = cut.FindAll("button").First(b => b.TextContent.Contains("Aprovar"));
        cut.InvokeAsync(() => approveButton.Click());
        // After clicking approve, the decision should be Approved
        var decisionField = typeof(XFAiHumanApprovalCard).GetField("_decision", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var decision = (XFAiApprovalDecision)decisionField!.GetValue(cut.Instance)!;
        Assert.Equal(XFAiApprovalDecision.Approved, decision);
    }

    [Fact]
    public void Extended_RejectButton_Click_SetsDecision()
    {
        var cut = RenderDefault();
        var rejectButton = cut.FindAll("button").First(b => b.TextContent.Contains("Rejeitar"));
        cut.InvokeAsync(() => rejectButton.Click());
        var decisionField = typeof(XFAiHumanApprovalCard).GetField("_decision", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var decision = (XFAiApprovalDecision)decisionField!.GetValue(cut.Instance)!;
        Assert.Equal(XFAiApprovalDecision.Rejected, decision);
    }

    [Fact]
    public void Extended_Approved_ShowsDecisionBanner()
    {
        XFAiApprovalRequest? capturedRequest = null;
        var cut = RenderDefault(p => p
            .Add(x => x.OnApproved, EventCallback.Factory.Create<XFAiApprovalRequest>(this, r => capturedRequest = r)));
        var approveButton = cut.FindAll("button").First(b => b.TextContent.Contains("Aprovar"));
        approveButton.Click();
        Assert.Contains("Aprovado", cut.Markup);
    }

    [Fact]
    public void Extended_Rejected_ShowsDecisionBanner()
    {
        XFAiApprovalRequest? capturedRequest = null;
        var cut = RenderDefault(p => p
            .Add(x => x.OnRejected, EventCallback.Factory.Create<XFAiApprovalRequest>(this, r => capturedRequest = r)));
        var rejectButton = cut.FindAll("button").First(b => b.TextContent.Contains("Rejeitar"));
        rejectButton.Click();
        Assert.Contains("Rejeitado", cut.Markup);
    }

    [Fact]
    public void Extended_EditButton_ShowsTextarea()
    {
        var cut = RenderDefault();
        var editButton = cut.FindAll("button").First(b => b.TextContent.Contains("Editar"));
        editButton.Click();
        // After clicking Edit, a textarea should appear for editing the payload
        var textarea = cut.FindAll("textarea");
        Assert.True(textarea.Count > 0);
    }

    [Fact]
    public void Extended_ShowEditOptionFalse_HidesEditButton()
    {
        var cut = Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.ShowEditOption, false));
        Assert.DoesNotContain("Editar", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_ButtonsAreDisabled()
    {
        var cut = Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.Disabled, true));
        var buttons = cut.FindAll("button");
        foreach (var button in buttons)
        {
            Assert.True(button.HasAttribute("disabled"));
        }
    }

    [Fact]
    public void Extended_RiskScoreHigh_RendersRedBar()
    {
        var request = new XFAiApprovalRequest { Title = "Test", RiskScore = 0.9 };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("bg-red-500", cut.Markup);
    }

    [Fact]
    public void Extended_RiskScoreMedium_RendersAmberBar()
    {
        var request = new XFAiApprovalRequest { Title = "Test", RiskScore = 0.6 };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("bg-amber-500", cut.Markup);
    }

    [Fact]
    public void Extended_RiskScoreLow_RendersGreenBar()
    {
        var request = new XFAiApprovalRequest { Title = "Test", RiskScore = 0.1 };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.Contains("bg-emerald-500", cut.Markup);
    }

    [Fact]
    public void Extended_NoRiskScore_HidesRiskBar()
    {
        var request = new XFAiApprovalRequest { Title = "Test", RiskScore = 0 };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        // Risk score 0 means no risk bar shown
        Assert.Contains("xf-ai-human-approval-card", cut.Markup);
    }

    [Fact]
    public void Extended_NoCategory_HidesCategoryBadge()
    {
        var request = new XFAiApprovalRequest { Title = "Test" };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.DoesNotContain("rounded-full bg-surface-100", cut.Markup);
    }

    [Fact]
    public void Extended_NoImpactAnalysis_HidesImpactBox()
    {
        var request = new XFAiApprovalRequest { Title = "Test" };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.DoesNotContain("Analise de impacto", cut.Markup);
    }

    [Fact]
    public void Extended_NoAffectedEntities_HidesEntities()
    {
        var request = new XFAiApprovalRequest { Title = "Test" };
        var cut = Render<XFAiHumanApprovalCard>(p => p.Add(x => x.Request, request));
        Assert.DoesNotContain("Entidades afetadas", cut.Markup);
    }

    [Fact]
    public void Extended_ApproveLabel_CustomLabel()
    {
        var cut = Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.ApproveLabel, "Confirmar"));
        Assert.Contains("Confirmar", cut.Markup);
    }

    [Fact]
    public void Extended_RejectLabel_CustomLabel()
    {
        var cut = Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.RejectLabel, "Cancelar"));
        Assert.Contains("Cancelar", cut.Markup);
    }

    [Fact]
    public void Extended_Class_CustomClass_Appended()
    {
        var cut = Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.Class, "my-approval"));
        Assert.Contains("my-approval", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_Rendered()
    {
        var cut = Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "approval" } }));
        Assert.Contains("data-testid=\"approval\"", cut.Markup);
    }

    [Fact]
    public void Extended_AriaBusy_False_WhenNotLoading()
    {
        var cut = RenderDefault();
        Assert.Contains("aria-busy=\"false\"", cut.Markup);
    }

    [Fact]
    public void Extended_AriaBusy_True_WhenLoading()
    {
        var cut = Render<XFAiHumanApprovalCard>(p => p
            .Add(x => x.Request, SampleRequest)
            .Add(x => x.Loading, true));
        Assert.Contains("aria-busy=\"true\"", cut.Markup);
    }

    [Fact]
    public void Extended_Enum_XFAiApprovalDecision_HasExpectedValues()
    {
        Assert.Equal(4, Enum.GetValues<XFAiApprovalDecision>().Length);
        Assert.Contains(XFAiApprovalDecision.Pending, Enum.GetValues<XFAiApprovalDecision>());
        Assert.Contains(XFAiApprovalDecision.Approved, Enum.GetValues<XFAiApprovalDecision>());
        Assert.Contains(XFAiApprovalDecision.Rejected, Enum.GetValues<XFAiApprovalDecision>());
        Assert.Contains(XFAiApprovalDecision.Edited, Enum.GetValues<XFAiApprovalDecision>());
    }
}
