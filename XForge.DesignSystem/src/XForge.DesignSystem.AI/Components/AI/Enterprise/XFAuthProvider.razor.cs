using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Enterprise;

/// <summary>
/// Enterprise authentication provider component — displays SSO/OIDC/OAuth2 integration status,
/// user information, session management, and login/logout controls.
/// </summary>
public partial class XFAuthProvider : XForgeComponentBase, IDisposable
{
    // ─── Parameters ────────────────────────────────────────────────────────────

    /// <summary>Current authentication state.</summary>
    [Parameter] public XFAuthState State { get; set; } = XFAuthState.Anonymous;

    /// <summary>Authentication provider type.</summary>
    [Parameter] public XFAuthProviderType Provider { get; set; } = XFAuthProviderType.OIDC;

    /// <summary>Current authenticated user.</summary>
    [Parameter] public XFAuthUser? CurrentUser { get; set; }

    /// <summary>Current active session.</summary>
    [Parameter] public XFAuthSession? CurrentSession { get; set; }

    /// <summary>Show login button when not authenticated.</summary>
    [Parameter] public bool ShowLoginButton { get; set; } = true;

    /// <summary>Show logout button when authenticated.</summary>
    [Parameter] public bool ShowLogoutButton { get; set; } = true;

    /// <summary>Error message to display in Error state.</summary>
    [Parameter] public string? ErrorMessage { get; set; }

    // ─── Labels (i18n) ─────────────────────────────────────────────────────────

    /// <summary>Login button label.</summary>
    [Parameter] public string LoginLabel { get; set; } = "Entrar";

    /// <summary>Logout button label.</summary>
    [Parameter] public string LogoutLabel { get; set; } = "Sair";

    /// <summary>Anonymous state label.</summary>
    [Parameter] public string AnonymousLabel { get; set; } = "Nao autenticado";

    /// <summary>Authenticating state label.</summary>
    [Parameter] public string AuthenticatingLabel { get; set; } = "Autenticando...";

    /// <summary>Session expired label.</summary>
    [Parameter] public string SessionExpiredLabel { get; set; } = "Sessao expirada";

    /// <summary>Account locked label.</summary>
    [Parameter] public string LockedLabel { get; set; } = "Conta bloqueada";

    /// <summary>Error state label.</summary>
    [Parameter] public string ErrorLabel { get; set; } = "Erro na autenticacao";

    /// <summary>Retry button label.</summary>
    [Parameter] public string RetryLabel { get; set; } = "Tentar novamente";

    // ─── Events ────────────────────────────────────────────────────────────────

    /// <summary>Raised when user clicks login.</summary>
    [Parameter] public EventCallback OnLogin { get; set; }

    /// <summary>Raised when user clicks logout.</summary>
    [Parameter] public EventCallback OnLogout { get; set; }

    /// <summary>Raised when session expires.</summary>
    [Parameter] public EventCallback OnSessionExpired { get; set; }

    // ─── Computed ───────────────────────────────────────────────────────────────

    private string RootClass => $"xf-auth-provider {Class}".Trim();

    // ─── Handlers ──────────────────────────────────────────────────────────────

    private async Task HandleLoginAsync()
    {
        if (OnLogin.HasDelegate)
            await OnLogin.InvokeAsync();
    }

    private async Task HandleLogoutAsync()
    {
        if (OnLogout.HasDelegate)
            await OnLogout.InvokeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
        return parts[0][0].ToString().ToUpperInvariant();
    }

    private static string GetProviderLabel(XFAuthProviderType provider) => provider switch
    {
        XFAuthProviderType.OIDC => "OIDC",
        XFAuthProviderType.OAuth2 => "OAuth2",
        XFAuthProviderType.SAML => "SAML",
        XFAuthProviderType.LDAP => "LDAP",
        XFAuthProviderType.APIKey => "API Key",
        XFAuthProviderType.Custom => "Custom",
        _ => provider.ToString()
    };

    private static string FormatExpiry(DateTimeOffset expiry)
    {
        var remaining = expiry - DateTimeOffset.UtcNow;
        if (remaining.TotalMinutes < 1) return "em breve";
        if (remaining.TotalHours < 1) return $"em {remaining.Minutes}min";
        if (remaining.TotalDays < 1) return $"em {remaining.Hours}h{remaining.Minutes:D2}m";
        return $"em {(int)remaining.TotalDays}d";
    }

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
