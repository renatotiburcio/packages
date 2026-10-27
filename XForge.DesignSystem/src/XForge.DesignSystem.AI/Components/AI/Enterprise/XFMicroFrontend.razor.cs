using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Enterprise;

/// <summary>
/// Micro-frontend container — loads and renders remote modules with state management,
/// error boundaries, timeout handling, and retry logic for module federation patterns.
/// </summary>
public partial class XFMicroFrontend : XForgeComponentBase, IDisposable
{
    // ─── Parameters ────────────────────────────────────────────────────────────

    /// <summary>Module definition to load.</summary>
    [Parameter] public XFMicroFrontendModule? Module { get; set; }

    /// <summary>Current load state (controlled externally or set by events).</summary>
    [Parameter] public XFMicroFrontendState CurrentState { get; set; } = XFMicroFrontendState.Idle;

    /// <summary>Error message when state is Error.</summary>
    [Parameter] public string? ErrorMessage { get; set; }

    /// <summary>Show load button in Idle state.</summary>
    [Parameter] public bool ShowLoadButton { get; set; } = true;

    /// <summary>Show retry button in Error state.</summary>
    [Parameter] public bool ShowRetryButton { get; set; } = true;

    /// <summary>Load timeout in seconds.</summary>
    [Parameter] public int TimeoutSeconds { get; set; } = 30;

    // ─── Labels (i18n) ─────────────────────────────────────────────────────────

    [Parameter] public string EmptyLabel { get; set; } = "Nenhum modulo configurado";
    [Parameter] public string LoadingLabel { get; set; } = "Carregando";
    [Parameter] public string ErrorLabel { get; set; } = "Erro ao carregar modulo";
    [Parameter] public string TimeoutLabel { get; set; } = "Tempo esgotado";
    [Parameter] public string RetryLabel { get; set; } = "Tentar novamente";
    [Parameter] public string LoadLabel { get; set; } = "Carregar";

    // ─── Content ───────────────────────────────────────────────────────────────

    /// <summary>Content to render when module is loaded.</summary>
    [Parameter] public RenderFragment<XFMicroFrontendModule>? ModuleContent { get; set; }

    /// <summary>Content to render when no module is configured.</summary>
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    /// <summary>Content to render on error (fallback).</summary>
    [Parameter] public RenderFragment? FallbackContent { get; set; }

    // ─── Events ────────────────────────────────────────────────────────────────

    /// <summary>Raised when module load is requested.</summary>
    [Parameter] public EventCallback<XFMicroFrontendModule> OnModuleLoad { get; set; }

    /// <summary>Raised when module loads successfully.</summary>
    [Parameter] public EventCallback<XFMicroFrontendModule> OnModuleLoaded { get; set; }

    /// <summary>Raised when module load fails.</summary>
    [Parameter] public EventCallback<string> OnModuleError { get; set; }

    /// <summary>Raised when retry is requested.</summary>
    [Parameter] public EventCallback OnRetry { get; set; }

    // ─── Computed ───────────────────────────────────────────────────────────────

    private string RootClass => $"xf-micro-frontend {Class}".Trim();

    // ─── Handlers ──────────────────────────────────────────────────────────────

    private async Task HandleLoadAsync()
    {
        if (Module is null) return;
        if (OnModuleLoad.HasDelegate)
            await OnModuleLoad.InvokeAsync(Module);
    }

    private async Task HandleRetryAsync()
    {
        if (OnRetry.HasDelegate)
            await OnRetry.InvokeAsync();
        else if (Module is not null && OnModuleLoad.HasDelegate)
            await OnModuleLoad.InvokeAsync(Module);
    }

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
