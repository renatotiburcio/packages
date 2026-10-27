using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Workflow;

/// <summary>
/// Rule-based automation component with condition/action builder,
/// AI enhancement support, and visual rule display. Pure Blazor.
/// </summary>
public partial class XFAiAutomationRule : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Automation rules to display.</summary>
    [Parameter]
    public IReadOnlyList<XFAiAutomationRuleItem> Rules { get; set; } = Array.Empty<XFAiAutomationRuleItem>();

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Regras de Automacao";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Regras de automacao";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Empty state text.</summary>
    [Parameter]
    public string EmptyText { get; set; } = "Nenhuma regra configurada.";

    /// <summary>Custom empty content.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    // ── Events ──────────────────────────────────────────────────────────

    /// <summary>Fired when the user requests adding a new rule.</summary>
    [Parameter]
    public EventCallback OnAddRule { get; set; }

    /// <summary>Fired when a rule is toggled (enabled/disabled).</summary>
    [Parameter]
    public EventCallback<XFAiAutomationRuleItem> OnRuleToggle { get; set; }

    /// <summary>Fired when a rule is selected for editing.</summary>
    [Parameter]
    public EventCallback<XFAiAutomationRuleItem> OnRuleEdit { get; set; }

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-automation-rule overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string GetOperatorLabel(XFAiConditionOperator op) => op switch
    {
        XFAiConditionOperator.Equals => "=",
        XFAiConditionOperator.NotEquals => "!=",
        XFAiConditionOperator.Contains => "contém",
        XFAiConditionOperator.GreaterThan => ">",
        XFAiConditionOperator.LessThan => "<",
        XFAiConditionOperator.GreaterOrEqual => ">=",
        XFAiConditionOperator.LessOrEqual => "<=",
        XFAiConditionOperator.IsEmpty => "é vazio",
        XFAiConditionOperator.IsNotEmpty => "não vazio",
        XFAiConditionOperator.StartsWith => "inicia com",
        XFAiConditionOperator.EndsWith => "termina com",
        _ => "?"
    };

    private static string GetLogicalLabel(XFAiLogicalOperator op) => op switch
    {
        XFAiLogicalOperator.And => "E",
        XFAiLogicalOperator.Or => "OU",
        _ => "E"
    };

    private static string GetActionIcon(XFAiAutomationActionType type) => type switch
    {
        XFAiAutomationActionType.SendNotification => "bell",
        XFAiAutomationActionType.ExecuteWorkflow => "play",
        XFAiAutomationActionType.CallAiProvider => "sparkles",
        XFAiAutomationActionType.UpdateField => "pen",
        XFAiAutomationActionType.SendEmail => "envelope",
        XFAiAutomationActionType.TriggerWebhook => "globe",
        XFAiAutomationActionType.LogEvent => "file-lines",
        XFAiAutomationActionType.WaitForApproval => "clock",
        _ => "circle"
    };

    // ── Interactions ────────────────────────────────────────────────────

    private Task HandleAddRule() => OnAddRule.InvokeAsync();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
