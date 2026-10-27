using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Performance;

/// <summary>
/// Lazy-load wrapper for heavy components. Defers rendering until triggered,
/// with timeout, retry, progress tracking, and error states. Pure Blazor.
/// </summary>
public partial class XFAiLazyComponent : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Componente Lazy";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Componente com carregamento lazy";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the footer bar.</summary>
    [Parameter]
    public bool ShowFooter { get; set; } = true;

    /// <summary>Placeholder text before loading.</summary>
    [Parameter]
    public string PlaceholderText { get; set; } = "Carregando componente...";

    /// <summary>Placeholder icon (emoji or text).</summary>
    [Parameter]
    public string? PlaceholderIcon { get; set; }

    /// <summary>Loading text shown during load.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Carregando...";

    /// <summary>Whether to load automatically on first render.</summary>
    [Parameter]
    public bool AutoLoad { get; set; }

    /// <summary>Whether to show a progress bar during loading.</summary>
    [Parameter]
    public bool ShowProgress { get; set; } = true;

    /// <summary>Timeout in milliseconds.</summary>
    [Parameter]
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>Maximum retry attempts.</summary>
    [Parameter]
    public int MaxRetries { get; set; } = 3;

    /// <summary>Delay before auto-loading (ms).</summary>
    [Parameter]
    public int DelayMs { get; set; }

    /// <summary>The lazy-loaded content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Alternative loaded content (named slot).</summary>
    [Parameter]
    public RenderFragment? LoadedContent { get; set; }

    /// <summary>Fired when loading starts.</summary>
    [Parameter]
    public EventCallback OnLoadStart { get; set; }

    /// <summary>Fired when loading completes.</summary>
    [Parameter]
    public EventCallback<double> OnLoadComplete { get; set; }

    /// <summary>Fired when loading fails.</summary>
    [Parameter]
    public EventCallback<string> OnLoadError { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFAiLazyLoadState _state = XFAiLazyLoadState.NotLoaded;
    private double _loadTimeMs;
    private double _progress;
    private string? _errorMessage;
    private int _retryCount;
    private CancellationTokenSource? _cts;

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && AutoLoad)
        {
            if (DelayMs > 0)
                await Task.Delay(DelayMs);
            await LoadAsync();
        }
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Triggers the lazy load.</summary>
    public async Task LoadAsync()
    {
        if (_state is XFAiLazyLoadState.Loading or XFAiLazyLoadState.Loaded)
            return;

        _state = XFAiLazyLoadState.Loading;
        _progress = 0;
        _errorMessage = null;
        _cts = new CancellationTokenSource();
        var startTime = DateTime.UtcNow;

        if (OnLoadStart.HasDelegate)
            await OnLoadStart.InvokeAsync();

        StateHasChanged();

        try
        {
            // Simulate progress
            var progressTask = SimulateProgressAsync(_cts.Token);

            // Wait for timeout or completion
            var completedTask = await Task.WhenAny(
                Task.Delay(TimeoutMs, _cts.Token),
                Task.Run(async () =>
                {
                    // Simulate actual load work
                    await Task.Delay(Math.Min(500, TimeoutMs), _cts.Token);
                    _progress = 100;
                }, _cts.Token));

            _cts.Cancel();

            if (_progress >= 100)
            {
                _loadTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                _state = XFAiLazyLoadState.Loaded;

                if (OnLoadComplete.HasDelegate)
                    await OnLoadComplete.InvokeAsync(_loadTimeMs);
            }
            else
            {
                _state = XFAiLazyLoadState.Timeout;
            }
        }
        catch (OperationCanceledException)
        {
            _state = XFAiLazyLoadState.Timeout;
        }
        catch (Exception ex)
        {
            _state = XFAiLazyLoadState.Error;
            _errorMessage = ex.Message;

            if (OnLoadError.HasDelegate)
                await OnLoadError.InvokeAsync(ex.Message);
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            StateHasChanged();
        }
    }

    /// <summary>Retries loading after error/timeout.</summary>
    public async Task RetryAsync()
    {
        if (_retryCount >= MaxRetries)
            return;

        _retryCount++;
        _state = XFAiLazyLoadState.NotLoaded;
        await LoadAsync();
    }

    /// <summary>Resets to initial (not loaded) state.</summary>
    public void Reset()
    {
        _state = XFAiLazyLoadState.NotLoaded;
        _progress = 0;
        _errorMessage = null;
        _retryCount = 0;
        _loadTimeMs = 0;
        StateHasChanged();
    }

    /// <summary>Current load state.</summary>
    public XFAiLazyLoadState CurrentState => _state;

    // ── Internal ────────────────────────────────────────────────────────

    private async Task SimulateProgressAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _progress < 95)
        {
            _progress += Random.Shared.Next(5, 15);
            if (_progress > 95) _progress = 95;
            StateHasChanged();
            await Task.Delay(100, ct);
        }
    }

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-lazy-component overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    private string GetStateBadgeClass() => _state switch
    {
        XFAiLazyLoadState.NotLoaded => "rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600 dark:bg-slate-800 dark:text-slate-400",
        XFAiLazyLoadState.Loading => "rounded-full bg-blue-100 px-2 py-0.5 text-xs text-blue-700 dark:bg-blue-900/30 dark:text-blue-300",
        XFAiLazyLoadState.Loaded => "rounded-full bg-emerald-100 px-2 py-0.5 text-xs text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300",
        XFAiLazyLoadState.Error => "rounded-full bg-red-100 px-2 py-0.5 text-xs text-red-700 dark:bg-red-900/30 dark:text-red-300",
        XFAiLazyLoadState.Timeout => "rounded-full bg-amber-100 px-2 py-0.5 text-xs text-amber-700 dark:bg-amber-900/30 dark:text-amber-300",
        _ => "rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-500"
    };

    private string GetStateLabel() => _state switch
    {
        XFAiLazyLoadState.NotLoaded => "Aguardando",
        XFAiLazyLoadState.Loading => "Carregando",
        XFAiLazyLoadState.Loaded => "Carregado",
        XFAiLazyLoadState.Error => "Erro",
        XFAiLazyLoadState.Timeout => "Timeout",
        _ => "—"
    };

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
