using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Enterprise;

/// <summary>
/// LGPD/GDPR compliance dashboard — displays data retention policies, consent management,
/// deletion requests, and compliance summary with regulation badges.
/// </summary>
public partial class XFCompliancePanel : XForgeComponentBase, IDisposable
{
    // ─── Parameters ────────────────────────────────────────────────────────────

    /// <summary>Compliance report data.</summary>
    [Parameter] public XFComplianceReport? Report { get; set; }

    /// <summary>Pending deletion requests.</summary>
    [Parameter] public IReadOnlyList<XFDeletionRequest> DeletionRequests { get; set; } = Array.Empty<XFDeletionRequest>();

    /// <summary>Component title.</summary>
    [Parameter] public string Title { get; set; } = "Conformidade";

    /// <summary>Show refresh button.</summary>
    [Parameter] public bool ShowRefreshButton { get; set; } = true;

    /// <summary>Show approve/reject buttons on deletion requests.</summary>
    [Parameter] public bool ShowActionButtons { get; set; } = true;

    /// <summary>Maximum visible deletion requests.</summary>
    [Parameter] public int MaxVisibleRequests { get; set; } = 10;

    // ─── Labels (i18n) ─────────────────────────────────────────────────────────

    [Parameter] public string EmptyLabel { get; set; } = "Nenhum relatorio de conformidade";
    [Parameter] public string UsersLabel { get; set; } = "Usuarios";
    [Parameter] public string ActiveConsentsLabel { get; set; } = "Consentimentos ativos";
    [Parameter] public string RevokedConsentsLabel { get; set; } = "Revogados";
    [Parameter] public string PendingDeletionsLabel { get; set; } = "Exclusoes pendentes";
    [Parameter] public string DeletionRequestsLabel { get; set; } = "Solicitacoes de exclusao";
    [Parameter] public string RetentionPoliciesLabel { get; set; } = "Politicas de retencao";
    [Parameter] public string ConsentByCategoryLabel { get; set; } = "Consentimentos por categoria";
    [Parameter] public string ApproveLabel { get; set; } = "Aprovar";
    [Parameter] public string RejectLabel { get; set; } = "Rejeitar";
    [Parameter] public string DaysLabel { get; set; } = "dias";

    // ─── Events ────────────────────────────────────────────────────────────────

    /// <summary>Raised when user approves a deletion request.</summary>
    [Parameter] public EventCallback<XFDeletionRequest> OnApproveDeletion { get; set; }

    /// <summary>Raised when user rejects a deletion request.</summary>
    [Parameter] public EventCallback<XFDeletionRequest> OnRejectDeletion { get; set; }

    /// <summary>Raised when refresh button is clicked.</summary>
    [Parameter] public EventCallback OnRefresh { get; set; }

    // ─── Computed ───────────────────────────────────────────────────────────────

    private string RootClass => $"xf-compliance-panel {Class}".Trim();

    // ─── Handlers ──────────────────────────────────────────────────────────────

    private async Task HandleApproveDeletionAsync(XFDeletionRequest request)
    {
        if (OnApproveDeletion.HasDelegate)
            await OnApproveDeletion.InvokeAsync(request);
    }

    private async Task HandleRejectDeletionAsync(XFDeletionRequest request)
    {
        if (OnRejectDeletion.HasDelegate)
            await OnRejectDeletion.InvokeAsync(request);
    }

    private async Task HandleRefreshAsync()
    {
        if (OnRefresh.HasDelegate)
            await OnRefresh.InvokeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static string GetRegulationLabel(XFComplianceRegulation reg) => reg switch
    {
        XFComplianceRegulation.LGPD => "LGPD",
        XFComplianceRegulation.GDPR => "GDPR",
        XFComplianceRegulation.CCPA => "CCPA",
        XFComplianceRegulation.HIPAA => "HIPAA",
        XFComplianceRegulation.SOX => "SOX",
        XFComplianceRegulation.PCI => "PCI-DSS",
        XFComplianceRegulation.Custom => "Custom",
        _ => reg.ToString()
    };

    private static string GetCategoryLabel(XFDataCategory cat) => cat switch
    {
        XFDataCategory.Personal => "Dados pessoais",
        XFDataCategory.Financial => "Dados financeiros",
        XFDataCategory.Health => "Dados de saude",
        XFDataCategory.Behavioral => "Dados comportamentais",
        XFDataCategory.Biometric => "Dados biometricos",
        XFDataCategory.Location => "Dados de localizacao",
        XFDataCategory.Custom => "Outros",
        _ => cat.ToString()
    };

    private static string GetLegalBasisLabel(XFLegalBasis basis) => basis switch
    {
        XFLegalBasis.Consent => "Consentimento",
        XFLegalBasis.Contract => "Contrato",
        XFLegalBasis.LegalObligation => "Obrigacao legal",
        XFLegalBasis.VitalInterest => "Interesse vital",
        XFLegalBasis.PublicTask => "Tarefa publica",
        XFLegalBasis.LegitimateInterest => "Interesse legitimo",
        _ => basis.ToString()
    };

    private static MarkupString GetDeletionStatusBadge(XFDeletionRequestStatus status) => status switch
    {
        XFDeletionRequestStatus.Pending => (MarkupString)"<span class=\"inline-flex items-center px-1.5 py-0.5 rounded text-[10px] font-medium bg-amber-100 dark:bg-amber-900/30 text-amber-700 dark:text-amber-300\">Pendente</span>",
        XFDeletionRequestStatus.Approved => (MarkupString)"<span class=\"inline-flex items-center px-1.5 py-0.5 rounded text-[10px] font-medium bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300\">Aprovado</span>",
        XFDeletionRequestStatus.Processing => (MarkupString)"<span class=\"inline-flex items-center px-1.5 py-0.5 rounded text-[10px] font-medium bg-purple-100 dark:bg-purple-900/30 text-purple-700 dark:text-purple-300\">Processando</span>",
        XFDeletionRequestStatus.Completed => (MarkupString)"<span class=\"inline-flex items-center px-1.5 py-0.5 rounded text-[10px] font-medium bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300\">Concluido</span>",
        XFDeletionRequestStatus.Rejected => (MarkupString)"<span class=\"inline-flex items-center px-1.5 py-0.5 rounded text-[10px] font-medium bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300\">Rejeitado</span>",
        _ => (MarkupString)$"<span class=\"text-[10px] text-gray-500\">{status}</span>"
    };

    private static string FormatTimestamp(DateTimeOffset ts)
    {
        var diff = DateTimeOffset.UtcNow - ts;
        if (diff.TotalSeconds < 60) return "agora";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}min atras";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h atras";
        return ts.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    }

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
