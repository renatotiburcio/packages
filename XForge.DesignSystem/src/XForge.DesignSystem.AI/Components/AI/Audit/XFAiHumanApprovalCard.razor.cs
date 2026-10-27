using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Audit;

/// <summary>
/// Enterprise AI human approval card component. Displays an approval request
/// with risk score, impact analysis, approve/reject/edit workflow.
/// </summary>
public partial class XFAiHumanApprovalCard : XForgeComponentBase
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>The approval request to display.</summary>
    [Parameter]
    public XFAiApprovalRequest Request { get; set; } = new();

    /// <summary>Approve button label.</summary>
    [Parameter]
    public string ApproveLabel { get; set; } = "Aprovar";

    /// <summary>Reject button label.</summary>
    [Parameter]
    public string RejectLabel { get; set; } = "Rejeitar";

    /// <summary>Whether to show the "Edit before apply" option.</summary>
    [Parameter]
    public bool ShowEditOption { get; set; } = true;

    /// <summary>External loading state.</summary>
    [Parameter]
    public bool Loading { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when the request is approved.</summary>
    [Parameter]
    public EventCallback<XFAiApprovalRequest> OnApproved { get; set; }

    /// <summary>Fired when the request is rejected.</summary>
    [Parameter]
    public EventCallback<XFAiApprovalRequest> OnRejected { get; set; }

    /// <summary>Fired when the request is approved with edits.</summary>
    [Parameter]
    public EventCallback<XFAiApprovalEditResult> OnEdited { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFAiApprovalDecision _decision = XFAiApprovalDecision.Pending;
    private string? _editedPayload;

    private bool IsBusy => Loading;
    private string AriaBusyValue => IsBusy ? "true" : "false";

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-human-approval-card relative overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_3px_rgb(15_23_42/.06)]",
        Class).Trim();

    private string RiskBarClass => Request.RiskScore switch
    {
        >= 0.8 => "h-1 w-full bg-red-500",
        >= 0.5 => "h-1 w-full bg-amber-500",
        >= 0.2 => "h-1 w-full bg-yellow-400",
        _ => "h-1 w-full bg-emerald-500"
    };

    private string HeaderClass =>
        "flex items-start justify-between gap-3 border-b border-border-light px-4 py-3";

    private string CategoryBadgeClass =>
        "inline-flex shrink-0 items-center rounded-full bg-surface-100 px-2 py-0.5 text-[10px] font-semibold text-text-secondary";

    private string BodyClass => "px-4 py-3";

    private string RiskTrackClass =>
        "h-1.5 flex-1 overflow-hidden rounded-full bg-surface-200";

    private string RiskFillClass => Request.RiskScore switch
    {
        >= 0.8 => "h-full rounded-full transition-all duration-300 bg-red-500",
        >= 0.5 => "h-full rounded-full transition-all duration-300 bg-amber-500",
        _ => "h-full rounded-full transition-all duration-300 bg-emerald-500"
    };

    private string RiskLabelClass => Request.RiskScore switch
    {
        >= 0.8 => "text-xs font-bold tabular-nums text-red-600",
        >= 0.5 => "text-xs font-bold tabular-nums text-amber-600",
        _ => "text-xs font-bold tabular-nums text-emerald-600"
    };

    private string DecisionBannerClass => _decision switch
    {
        XFAiApprovalDecision.Approved => "mt-3 flex items-center gap-2 rounded-md bg-emerald-50 px-3 py-2 text-xs text-emerald-700 ring-1 ring-emerald-200",
        XFAiApprovalDecision.Rejected => "mt-3 flex items-center gap-2 rounded-md bg-red-50 px-3 py-2 text-xs text-red-700 ring-1 ring-red-200",
        XFAiApprovalDecision.Edited => "mt-3 flex items-center gap-2 rounded-md bg-blue-50 px-3 py-2 text-xs text-blue-700 ring-1 ring-blue-200",
        _ => ""
    };

    private string ActionsClass =>
        "flex items-center justify-end gap-2 border-t border-border-light px-4 py-3";

    private string ApproveButtonClass =>
        "inline-flex h-8 items-center gap-1.5 rounded-md bg-emerald-500 px-3 text-sm font-semibold text-white outline-none transition-all hover:bg-emerald-600 focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50";

    private string RejectButtonClass =>
        "inline-flex h-8 items-center gap-1.5 rounded-md bg-red-50 px-3 text-sm font-semibold text-red-700 outline-none transition-all ring-1 ring-red-200 hover:bg-red-100 focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50";

    private string EditButtonClass =>
        "inline-flex h-8 items-center gap-1.5 rounded-md bg-surface-50 px-3 text-sm font-medium text-text-secondary outline-none transition-all ring-1 ring-border-light hover:bg-surface-100 focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50";

    // ── Helpers ─────────────────────────────────────────────────────────

    private string GetDecisionIcon() => _decision switch
    {
        XFAiApprovalDecision.Approved => "circle-check",
        XFAiApprovalDecision.Rejected => "circle-xmark",
        XFAiApprovalDecision.Edited => "pencil",
        _ => "circle-question"
    };

    private string GetDecisionLabel() => _decision switch
    {
        XFAiApprovalDecision.Approved => "Aprovado",
        XFAiApprovalDecision.Rejected => "Rejeitado",
        XFAiApprovalDecision.Edited => "Aprovado com edicoes",
        _ => "Pendente"
    };

    // ── Events ──────────────────────────────────────────────────────────

    private async Task ApproveAsync()
    {
        if (Disabled) return;
        _decision = XFAiApprovalDecision.Approved;
        if (OnApproved.HasDelegate)
            await OnApproved.InvokeAsync(Request);
    }

    private async Task RejectAsync()
    {
        if (Disabled) return;
        _decision = XFAiApprovalDecision.Rejected;
        if (OnRejected.HasDelegate)
            await OnRejected.InvokeAsync(Request);
    }

    private async Task ApplyEditAsync()
    {
        if (Disabled) return;
        _decision = XFAiApprovalDecision.Edited;
        if (OnEdited.HasDelegate)
            await OnEdited.InvokeAsync(new XFAiApprovalEditResult
            {
                Request = Request,
                EditedPayload = _editedPayload ?? Request.ProposedPayload ?? string.Empty
            });
    }
}

/// <summary>
/// Result of an edited approval decision.
/// </summary>
public sealed record XFAiApprovalEditResult
{
    /// <summary>The original approval request.</summary>
    public XFAiApprovalRequest Request { get; init; } = new();

    /// <summary>The edited payload.</summary>
    public string EditedPayload { get; init; } = string.Empty;

    /// <summary>Optional reviewer notes.</summary>
    public string? ReviewerNotes { get; init; }
}
