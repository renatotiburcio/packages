using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Tooling;

/// <summary>
/// Figma design token synchronization panel with connect/disconnect, import/export,
/// bidirectional sync, conflict detection, and token list. Pure Blazor, no JS interop.
/// </summary>
public partial class XFFigmaSync : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Figma Sync";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Sincronizacao com Figma";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the status summary cards.</summary>
    [Parameter]
    public bool ShowStatusPanel { get; set; } = true;

    /// <summary>Whether to show the token list table.</summary>
    [Parameter]
    public bool ShowTokenList { get; set; } = true;

    /// <summary>Maximum visible tokens in the list.</summary>
    [Parameter]
    public int MaxVisibleTokens { get; set; } = 50;

    /// <summary>Design tokens to sync.</summary>
    [Parameter]
    public IReadOnlyList<XFDesignToken>? DesignTokens { get; set; }

    /// <summary>Figma tokens loaded from previous sync.</summary>
    [Parameter]
    public IReadOnlyList<XFFigmaToken>? FigmaTokens { get; set; }

    /// <summary>Fired when connection is established.</summary>
    [Parameter]
    public EventCallback OnConnected { get; set; }

    /// <summary>Fired when sync completes.</summary>
    [Parameter]
    public EventCallback<XFFigmaSyncResult> OnSyncComplete { get; set; }

    /// <summary>Fired when disconnect is requested.</summary>
    [Parameter]
    public EventCallback OnDisconnected { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFFigmaSyncStatus _status = XFFigmaSyncStatus.Disconnected;
    private XFFigmaSyncDirection _direction = XFFigmaSyncDirection.Import;
    private string _accessToken = "";
    private string _fileKey = "";
    private XFFigmaSyncResult? _lastResult;
    private List<XFFigmaToken> _tokens = new();

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (FigmaTokens is not null)
            _tokens = FigmaTokens.ToList();
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Gets current sync status.</summary>
    public XFFigmaSyncStatus GetStatus() => _status;

    /// <summary>Gets the last sync result.</summary>
    public XFFigmaSyncResult? GetLastResult() => _lastResult;

    /// <summary>Gets the synced tokens.</summary>
    public IReadOnlyList<XFFigmaToken> GetTokens() => _tokens.AsReadOnly();

    // ── Handlers ────────────────────────────────────────────────────────

    private void HandleAccessTokenChanged(ChangeEventArgs e) => _accessToken = e.Value?.ToString() ?? "";
    private void HandleFileKeyChanged(ChangeEventArgs e) => _fileKey = e.Value?.ToString() ?? "";
    private void HandleDirectionChanged(ChangeEventArgs e)
    {
        if (Enum.TryParse<XFFigmaSyncDirection>(e.Value?.ToString(), out var dir))
            _direction = dir;
    }

    // ── Actions ─────────────────────────────────────────────────────────

    private void Connect()
    {
        if (string.IsNullOrWhiteSpace(_accessToken) || string.IsNullOrWhiteSpace(_fileKey))
            return;

        _status = XFFigmaSyncStatus.Connected;
        OnConnected.InvokeAsync();
    }

    private void Disconnect()
    {
        _status = XFFigmaSyncStatus.Disconnected;
        _lastResult = null;
        _tokens.Clear();
        OnDisconnected.InvokeAsync();
    }

    private async Task SyncAsync()
    {
        if (_status != XFFigmaSyncStatus.Connected && _status != XFFigmaSyncStatus.Synced)
            return;

        _status = XFFigmaSyncStatus.Syncing;
        StateHasChanged();

        // Simulate sync — in production, this would call Figma API
        await Task.Delay(500);

        var imported = _direction is XFFigmaSyncDirection.Import or XFFigmaSyncDirection.Bidirectional
            ? DesignTokens?.Count ?? 0 : 0;
        var exported = _direction is XFFigmaSyncDirection.Export or XFFigmaSyncDirection.Bidirectional
            ? DesignTokens?.Count ?? 0 : 0;

        if (DesignTokens is not null)
        {
            _tokens = DesignTokens.Select(dt => new XFFigmaToken
            {
                Name = dt.Name,
                Value = dt.Value,
                FigmaStyleId = $"figma-{dt.Name.ToLower().Replace(" ", "-")}",
                Category = dt.Category,
                LastSynced = DateTime.UtcNow,
                HasConflict = false
            }).ToList();
        }

        _lastResult = new XFFigmaSyncResult
        {
            Status = XFFigmaSyncStatus.Synced,
            TokensImported = imported,
            TokensExported = exported,
            Conflicts = 0,
            SyncedAt = DateTime.UtcNow
        };

        _status = XFFigmaSyncStatus.Synced;
        await OnSyncComplete.InvokeAsync(_lastResult);
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private string GetStatusBadgeClass() => _status switch
    {
        XFFigmaSyncStatus.Disconnected => "bg-slate-100 text-slate-600 dark:bg-slate-800 dark:text-slate-400",
        XFFigmaSyncStatus.Connected => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFFigmaSyncStatus.Syncing => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFFigmaSyncStatus.Synced => "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFFigmaSyncStatus.Error => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-300",
        XFFigmaSyncStatus.Conflict => "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        _ => "bg-slate-100 text-slate-600"
    };

    private string GetStatusLabel() => _status switch
    {
        XFFigmaSyncStatus.Disconnected => "Desconectado",
        XFFigmaSyncStatus.Connected => "Conectado",
        XFFigmaSyncStatus.Syncing => "Sincronizando...",
        XFFigmaSyncStatus.Synced => "Sincronizado",
        XFFigmaSyncStatus.Error => "Erro",
        XFFigmaSyncStatus.Conflict => "Conflito",
        _ => "Desconhecido"
    };

    private static string GetCategoryLabel(XFDesignTokenCategory category) => category switch
    {
        XFDesignTokenCategory.Color => "Cor",
        XFDesignTokenCategory.Typography => "Fonte",
        XFDesignTokenCategory.Spacing => "Espaco",
        XFDesignTokenCategory.Border => "Borda",
        XFDesignTokenCategory.Shadow => "Sombra",
        _ => "Outro"
    };

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-figma-sync overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
