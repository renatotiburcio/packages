using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Enterprise;

namespace XForge.DesignSystem.Tests.Components.AI;

public class XFEnterpriseIntegrationComponentsTest : BunitContext
{
    #region XFAuthProvider

    [Fact]
    public void AuthProvider_RendersAnonymousStateByDefault()
    {
        var cut = Render<XFAuthProvider>();

        Assert.Contains("xf-auth-provider", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Nao autenticado", cut.Markup);
    }

    [Fact]
    public void AuthProvider_RendersAuthenticatedState_WithUserInfo()
    {
        var user = new XFAuthUser
        {
            Id = "u1",
            DisplayName = "Maria Silva",
            Email = "maria@example.com",
            Roles = new[] { "Admin", "Editor" }
        };
        var cut = Render<XFAuthProvider>(p =>
        {
            p.Add(x => x.State, XFAuthState.Authenticated);
            p.Add(x => x.CurrentUser, user);
        });

        Assert.Contains("Maria Silva", cut.Markup);
        Assert.Contains("maria@example.com", cut.Markup);
        Assert.Contains("Admin", cut.Markup);
        Assert.Contains("Editor", cut.Markup);
    }

    [Fact]
    public void AuthProvider_RendersAuthenticatingState_WithSpinner()
    {
        var cut = Render<XFAuthProvider>(p => p.Add(x => x.State, XFAuthState.Authenticating));

        Assert.Contains("Autenticando...", cut.Markup);
        Assert.Contains("animate-spin", cut.Markup);
    }

    [Fact]
    public void AuthProvider_RendersExpiredState_WithWarning()
    {
        var cut = Render<XFAuthProvider>(p => p.Add(x => x.State, XFAuthState.Expired));

        Assert.Contains("Sessao expirada", cut.Markup);
        Assert.Contains("bg-amber-50", cut.Markup);
    }

    [Fact]
    public void AuthProvider_RendersErrorState_WithMessage()
    {
        var cut = Render<XFAuthProvider>(p =>
        {
            p.Add(x => x.State, XFAuthState.Error);
            p.Add(x => x.ErrorMessage, "Token invalido");
        });

        Assert.Contains("Token invalido", cut.Markup);
        Assert.Contains("bg-red-50", cut.Markup);
    }

    [Fact]
    public void AuthProvider_ClickLogin_RaisesOnLogin()
    {
        var raised = false;
        var cut = Render<XFAuthProvider>(p => p.Add(x => x.OnLogin, () => { raised = true; }));

        cut.Find("button[aria-label='Entrar']").Click();

        Assert.True(raised);
    }

    [Fact]
    public void AuthProvider_ClickLogout_RaisesOnLogout()
    {
        var raised = false;
        var user = new XFAuthUser { Id = "u1", DisplayName = "Maria Silva", Roles = new[] { "Admin" } };
        var cut = Render<XFAuthProvider>(p =>
        {
            p.Add(x => x.State, XFAuthState.Authenticated);
            p.Add(x => x.CurrentUser, user);
            p.Add(x => x.OnLogout, () => { raised = true; });
        });

        cut.Find("button[aria-label='Sair da conta']").Click();

        Assert.True(raised);
    }

    [Fact]
    public void AuthProvider_ShowsProviderLabel_OIDC()
    {
        var user = new XFAuthUser { Id = "u1", DisplayName = "Admin User", Roles = new[] { "Admin" } };
        var cut = Render<XFAuthProvider>(p =>
        {
            p.Add(x => x.State, XFAuthState.Authenticated);
            p.Add(x => x.Provider, XFAuthProviderType.OIDC);
            p.Add(x => x.CurrentUser, user);
        });

        Assert.Contains("OIDC", cut.Markup);
    }

    #endregion

    #region XFRbacComponent

    [Fact]
    public void Rbac_RendersChildContent_WhenRolesMatch()
    {
        var cut = Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Admin" });
            p.Add(x => x.RequiredRoles, new[] { "Admin" });
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Protected Content")));
        });

        Assert.Contains("Protected Content", cut.Markup);
    }

    [Fact]
    public void Rbac_HidesChildContent_WhenRolesDontMatch()
    {
        var cut = Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Viewer" });
            p.Add(x => x.RequiredRoles, new[] { "Admin" });
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Protected Content")));
        });

        Assert.DoesNotContain("Protected Content", cut.Markup);
    }

    [Fact]
    public void Rbac_ShowsDeniedContent_WhenProvided()
    {
        var cut = Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Viewer" });
            p.Add(x => x.RequiredRoles, new[] { "Admin" });
            p.Add(x => x.DeniedContent, (RenderFragment)(b => b.AddContent(0, "Access Denied Message")));
        });

        Assert.Contains("Access Denied Message", cut.Markup);
    }

    [Fact]
    public void Rbac_ShowsDeniedIndicator_WhenShowDeniedIndicatorTrue()
    {
        var cut = Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Viewer" });
            p.Add(x => x.RequiredRoles, new[] { "Admin" });
            p.Add(x => x.ShowDeniedIndicator, true);
        });

        Assert.Contains("Acesso restrito", cut.Markup);
    }

    [Fact]
    public void Rbac_EvaluatesAndOperator_BothRoleAndPermissionNeeded()
    {
        // Has role but NOT permission -> denied under And
        var cut = Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Admin" });
            p.Add(x => x.UserPermissions, new[] { "read" });
            p.Add(x => x.RequiredRoles, new[] { "Admin" });
            p.Add(x => x.RequiredPermissions, new[] { "write" });
            p.Add(x => x.Operator, XFRbacOperator.And);
            p.Add(x => x.ShowDeniedIndicator, true);
        });

        Assert.Contains("Acesso restrito", cut.Markup);
    }

    [Fact]
    public void Rbac_EvaluatesOrOperator_EitherRoleOrPermissionSufficient()
    {
        // Has permission but NOT role -> allowed under Or
        var cut = Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Viewer" });
            p.Add(x => x.UserPermissions, new[] { "write" });
            p.Add(x => x.RequiredRoles, new[] { "Admin" });
            p.Add(x => x.RequiredPermissions, new[] { "write" });
            p.Add(x => x.Operator, XFRbacOperator.Or);
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Or Allowed")));
        });

        Assert.Contains("Or Allowed", cut.Markup);
    }

    [Fact]
    public void Rbac_UsesPreDefinedRule_WhenRuleProvided()
    {
        var rule = new XFRbacRule
        {
            RequiredRoles = new[] { "Manager" },
            RequiredPermissions = new[] { "approve" },
            Operator = XFRbacOperator.And
        };
        var cut = Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Manager" });
            p.Add(x => x.UserPermissions, new[] { "approve" });
            p.Add(x => x.Rule, rule);
            p.Add(x => x.ChildContent, (RenderFragment)(b => b.AddContent(0, "Rule Content")));
        });

        Assert.Contains("Rule Content", cut.Markup);
    }

    [Fact]
    public void Rbac_FiresOnDecision_Callback()
    {
        XFRbacDecision? capturedDecision = null;
        Render<XFRbacComponent>(p =>
        {
            p.Add(x => x.UserRoles, new[] { "Admin" });
            p.Add(x => x.RequiredRoles, new[] { "Admin" });
            p.Add(x => x.OnDecision, (XFRbacDecision d) => { capturedDecision = d; });
        });

        Assert.NotNull(capturedDecision);
        Assert.Equal(XFRbacDecision.Allow, capturedDecision);
    }

    #endregion

    #region XFAuditLogger

    [Fact]
    public void AuditLogger_RendersEmptyState_WhenNoEntries()
    {
        var cut = Render<XFAuditLogger>();

        Assert.Contains("xf-audit-logger", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Nenhum registro de auditoria", cut.Markup);
    }

    [Fact]
    public void AuditLogger_RendersEntries_WithSeverityIndicators()
    {
        var entries = new List<XFAuditEntry>
        {
            new() { Action = XFAuditAction.Login, UserName = "admin", Severity = XFAuditSeverity.Info, Details = "User logged in" },
            new() { Action = XFAuditAction.Delete, UserName = "admin", Severity = XFAuditSeverity.Warning, Details = "Record deleted" },
            new() { Action = XFAuditAction.Update, UserName = "admin", Severity = XFAuditSeverity.Error, Details = "Update failed", Success = false }
        };
        var cut = Render<XFAuditLogger>(p => p.Add(x => x.Entries, entries));

        Assert.Contains("Login", cut.Markup);
        Assert.Contains("admin", cut.Markup);
        Assert.Contains("User logged in", cut.Markup);
        Assert.Contains("border-amber-400", cut.Markup);
        Assert.Contains("border-red-400", cut.Markup);
    }

    [Fact]
    public void AuditLogger_FiltersBySeverity()
    {
        var entries = new List<XFAuditEntry>
        {
            new() { Action = XFAuditAction.Login, Severity = XFAuditSeverity.Info, Details = "Info entry" },
            new() { Action = XFAuditAction.Delete, Severity = XFAuditSeverity.Warning, Details = "Warning entry" }
        };
        var cut = Render<XFAuditLogger>(p => p.Add(x => x.Entries, entries));

        Assert.Contains("Info entry", cut.Markup);
        Assert.Contains("Warning entry", cut.Markup);

        cut.Find("select[aria-label='Filtrar por severidade']").Change("Warning");

        Assert.DoesNotContain("Info entry", cut.Markup);
        Assert.Contains("Warning entry", cut.Markup);
    }

    [Fact]
    public void AuditLogger_FiltersBySearchText()
    {
        var entries = new List<XFAuditEntry>
        {
            new() { Action = XFAuditAction.Login, Details = "User logged in successfully" },
            new() { Action = XFAuditAction.Delete, Details = "Record removed from database" }
        };
        var cut = Render<XFAuditLogger>(p => p.Add(x => x.Entries, entries));

        cut.Find("input[aria-label='Pesquisar nos logs']").Input("logged");

        Assert.Contains("User logged in", cut.Markup);
        Assert.DoesNotContain("Record removed", cut.Markup);
    }

    [Fact]
    public void AuditLogger_ShowsLoadMore_WhenEntriesExceedMaxVisible()
    {
        var entries = new List<XFAuditEntry>();
        for (int i = 0; i < 60; i++)
            entries.Add(new XFAuditEntry { Action = XFAuditAction.Read, Details = $"Entry {i}" });

        var cut = Render<XFAuditLogger>(p =>
        {
            p.Add(x => x.Entries, entries);
            p.Add(x => x.MaxVisible, 50);
        });

        Assert.Contains("Carregar mais", cut.Markup);
        Assert.Contains("restantes", cut.Markup);
    }

    #endregion

    #region XFCompliancePanel

    [Fact]
    public void CompliancePanel_RendersEmptyState_WhenNoReport()
    {
        var cut = Render<XFCompliancePanel>();

        Assert.Contains("xf-compliance-panel", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Nenhum relatorio de conformidade", cut.Markup);
    }

    [Fact]
    public void CompliancePanel_RendersSummaryCards()
    {
        var report = new XFComplianceReport
        {
            TotalUsers = 150,
            ActiveConsents = 120,
            RevokedConsents = 30,
            PendingDeletions = 5
        };
        var cut = Render<XFCompliancePanel>(p => p.Add(x => x.Report, report));

        Assert.Contains("Usuarios", cut.Markup);
        Assert.Contains("150", cut.Markup);
        Assert.Contains("Consentimentos ativos", cut.Markup);
        Assert.Contains("120", cut.Markup);
        Assert.Contains("Revogados", cut.Markup);
        Assert.Contains("30", cut.Markup);
        Assert.Contains("Exclusoes pendentes", cut.Markup);
        Assert.Contains("5", cut.Markup);
    }

    [Fact]
    public void CompliancePanel_RendersDeletionRequests_WithStatusBadges()
    {
        var requests = new List<XFDeletionRequest>
        {
            new() { UserId = "u1", Status = XFDeletionRequestStatus.Pending, Reason = "User request" },
            new() { UserId = "u2", Status = XFDeletionRequestStatus.Approved }
        };
        var report = new XFComplianceReport { TotalUsers = 10, ActiveConsents = 5 };
        var cut = Render<XFCompliancePanel>(p =>
        {
            p.Add(x => x.Report, report);
            p.Add(x => x.DeletionRequests, requests);
        });

        Assert.Contains("Pendente", cut.Markup);
        Assert.Contains("Aprovado", cut.Markup);
        Assert.Contains("u1", cut.Markup);
        Assert.Contains("u2", cut.Markup);
    }

    [Fact]
    public void CompliancePanel_RendersRetentionPolicies()
    {
        var report = new XFComplianceReport
        {
            TotalUsers = 10,
            ActiveConsents = 5,
            RetentionPolicies = new List<XFDataRetentionPolicy>
            {
                new() { Category = XFDataCategory.Personal, RetentionDays = 365, LegalBasis = XFLegalBasis.Consent },
                new() { Category = XFDataCategory.Financial, RetentionDays = 730, AutoDelete = true, LegalBasis = XFLegalBasis.LegalObligation }
            }
        };
        var cut = Render<XFCompliancePanel>(p => p.Add(x => x.Report, report));

        Assert.Contains("Dados pessoais", cut.Markup);
        Assert.Contains("365", cut.Markup);
        Assert.Contains("Dados financeiros", cut.Markup);
        Assert.Contains("730", cut.Markup);
        Assert.Contains("Auto", cut.Markup);
    }

    [Fact]
    public void CompliancePanel_ClickApprove_RaisesOnApproveDeletion()
    {
        XFDeletionRequest? approved = null;
        var requests = new List<XFDeletionRequest>
        {
            new() { Id = "del1", UserId = "u1", Status = XFDeletionRequestStatus.Pending }
        };
        var report = new XFComplianceReport { TotalUsers = 10, ActiveConsents = 5 };
        var cut = Render<XFCompliancePanel>(p =>
        {
            p.Add(x => x.Report, report);
            p.Add(x => x.DeletionRequests, requests);
            p.Add(x => x.OnApproveDeletion, (XFDeletionRequest r) => { approved = r; });
        });

        cut.Find("button[aria-label='Aprovar exclusao']").Click();

        Assert.NotNull(approved);
        Assert.Equal("u1", approved!.UserId);
    }

    [Fact]
    public void CompliancePanel_ClickReject_RaisesOnRejectDeletion()
    {
        XFDeletionRequest? rejected = null;
        var requests = new List<XFDeletionRequest>
        {
            new() { Id = "del1", UserId = "u1", Status = XFDeletionRequestStatus.Pending }
        };
        var report = new XFComplianceReport { TotalUsers = 10, ActiveConsents = 5 };
        var cut = Render<XFCompliancePanel>(p =>
        {
            p.Add(x => x.Report, report);
            p.Add(x => x.DeletionRequests, requests);
            p.Add(x => x.OnRejectDeletion, (XFDeletionRequest r) => { rejected = r; });
        });

        cut.Find("button[aria-label='Rejeitar exclusao']").Click();

        Assert.NotNull(rejected);
        Assert.Equal("u1", rejected!.UserId);
    }

    #endregion

    #region XFMicroFrontend

    [Fact]
    public void MicroFrontend_RendersEmptyState_WhenNoModule()
    {
        var cut = Render<XFMicroFrontend>();

        Assert.Contains("xf-micro-frontend", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
        Assert.Contains("Nenhum modulo configurado", cut.Markup);
    }

    [Fact]
    public void MicroFrontend_RendersLoadingState_WithSpinner()
    {
        var module = new XFMicroFrontendModule { Name = "Financeiro", Version = "1.0.0" };
        var cut = Render<XFMicroFrontend>(p =>
        {
            p.Add(x => x.Module, module);
            p.Add(x => x.CurrentState, XFMicroFrontendState.Loading);
        });

        Assert.Contains("Carregando", cut.Markup);
        Assert.Contains("Financeiro", cut.Markup);
        Assert.Contains("animate-spin", cut.Markup);
        Assert.Contains("bg-blue-50", cut.Markup);
    }

    [Fact]
    public void MicroFrontend_RendersErrorState_WithRetryButton()
    {
        var module = new XFMicroFrontendModule { Name = "Estoque" };
        var cut = Render<XFMicroFrontend>(p =>
        {
            p.Add(x => x.Module, module);
            p.Add(x => x.CurrentState, XFMicroFrontendState.Error);
            p.Add(x => x.ErrorMessage, "Network timeout");
        });

        Assert.Contains("Erro ao carregar modulo", cut.Markup);
        Assert.Contains("Network timeout", cut.Markup);
        Assert.Contains("Tentar novamente", cut.Markup);
    }

    [Fact]
    public void MicroFrontend_RendersLoadedState_WithModuleInfo()
    {
        var module = new XFMicroFrontendModule
        {
            Name = "Estoque",
            Version = "2.1.0",
            Scope = "inventory",
            Dependencies = new[] { "shared-utils", "api-client" }
        };
        var cut = Render<XFMicroFrontend>(p =>
        {
            p.Add(x => x.Module, module);
            p.Add(x => x.CurrentState, XFMicroFrontendState.Loaded);
        });

        Assert.Contains("Estoque", cut.Markup);
        Assert.Contains("green-500", cut.Markup);
        Assert.Contains("inventory", cut.Markup);
        Assert.Contains("shared-utils", cut.Markup);
        Assert.Contains("api-client", cut.Markup);
    }

    [Fact]
    public void MicroFrontend_ClickLoad_RaisesOnModuleLoad()
    {
        XFMicroFrontendModule? loadedModule = null;
        var module = new XFMicroFrontendModule { Name = "RH" };
        var cut = Render<XFMicroFrontend>(p =>
        {
            p.Add(x => x.Module, module);
            p.Add(x => x.CurrentState, XFMicroFrontendState.Idle);
            p.Add(x => x.OnModuleLoad, (XFMicroFrontendModule m) => { loadedModule = m; });
        });

        cut.Find("button[aria-label='Carregar modulo']").Click();

        Assert.NotNull(loadedModule);
        Assert.Equal("RH", loadedModule!.Name);
    }

    [Fact]
    public void MicroFrontend_ClickRetry_RaisesOnRetry()
    {
        var retried = false;
        var module = new XFMicroFrontendModule { Name = "RH" };
        var cut = Render<XFMicroFrontend>(p =>
        {
            p.Add(x => x.Module, module);
            p.Add(x => x.CurrentState, XFMicroFrontendState.Error);
            p.Add(x => x.OnRetry, () => { retried = true; });
        });

        cut.Find("button[aria-label='Tentar novamente']").Click();

        Assert.True(retried);
    }

    #endregion
}



