using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Enterprise;

/// <summary>
/// Role-Based Access Control (RBAC) component — conditionally renders child content
/// based on user roles and permissions with And/Or logical operators.
/// </summary>
public partial class XFRbacComponent : XForgeComponentBase, IDisposable
{
    // ─── Parameters ────────────────────────────────────────────────────────────

    /// <summary>Roles assigned to the current user.</summary>
    [Parameter] public IReadOnlyList<string> UserRoles { get; set; } = Array.Empty<string>();

    /// <summary>Permissions assigned to the current user.</summary>
    [Parameter] public IReadOnlyList<string> UserPermissions { get; set; } = Array.Empty<string>();

    /// <summary>Required roles to access content (any match when Operator=Or).</summary>
    [Parameter] public IReadOnlyList<string> RequiredRoles { get; set; } = Array.Empty<string>();

    /// <summary>Required permissions to access content.</summary>
    [Parameter] public IReadOnlyList<string> RequiredPermissions { get; set; } = Array.Empty<string>();

    /// <summary>Logical operator for combining role/permission checks.</summary>
    [Parameter] public XFRbacOperator Operator { get; set; } = XFRbacOperator.And;

    /// <summary>Pre-defined RBAC rule to evaluate (overrides RequiredRoles/RequiredPermissions).</summary>
    [Parameter] public XFRbacRule? Rule { get; set; }

    /// <summary>Show a denied indicator when access is denied and no DeniedContent is provided.</summary>
    [Parameter] public bool ShowDeniedIndicator { get; set; }

    /// <summary>Denied access label.</summary>
    [Parameter] public string DeniedLabel { get; set; } = "Acesso restrito";

    // ─── Content ───────────────────────────────────────────────────────────────

    /// <summary>Content to render when access is allowed.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Content to render when access is denied.</summary>
    [Parameter] public RenderFragment? DeniedContent { get; set; }

    // ─── Events ────────────────────────────────────────────────────────────────

    /// <summary>Raised when RBAC evaluation completes with the decision.</summary>
    [Parameter] public EventCallback<XFRbacDecision> OnDecision { get; set; }

    // ─── Computed ───────────────────────────────────────────────────────────────

    private string RootClass => $"xf-rbac-component {Class}".Trim();

    /// <summary>Whether the current user has access.</summary>
    public bool IsAllowed => Evaluate();

    // ─── Evaluation ────────────────────────────────────────────────────────────

    private bool Evaluate()
    {
        var roles = Rule?.RequiredRoles ?? RequiredRoles;
        var perms = Rule?.RequiredPermissions ?? RequiredPermissions;
        var op = Rule?.Operator ?? Operator;

        var hasRoles = roles.Count == 0 || roles.Any(r => UserRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
        var hasPerms = perms.Count == 0 || perms.Any(p => UserPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));

        return op switch
        {
            XFRbacOperator.And => hasRoles && hasPerms,
            XFRbacOperator.Or => hasRoles || hasPerms,
            _ => hasRoles && hasPerms
        };
    }

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    protected override async Task OnParametersSetAsync()
    {
        if (OnDecision.HasDelegate)
        {
            var decision = IsAllowed ? XFRbacDecision.Allow : XFRbacDecision.Deny;
            await OnDecision.InvokeAsync(decision);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
