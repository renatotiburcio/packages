using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Common;

/// <summary>
/// Enterprise AI provider configuration component.
/// Allows runtime selection and configuration of AI providers with model selection,
/// capability display, and connection status.
/// </summary>
public partial class XFAiProviderConfig : ComponentBase
{
    private string _selectedProviderId = "inmemory";
    private string? _selectedModel;
    private IAiProvider? _activeProvider;
    private IReadOnlyList<string> _availableModels = [];

    /// <summary>All available AI providers injected via DI.</summary>
    [Inject] public IEnumerable<IAiProvider> Providers { get; set; } = [];

    /// <summary>Title displayed in the header.</summary>
    [Parameter] public string Title { get; set; } = "Configura&ccedil;&atilde;o do Provedor IA";

    /// <summary>Label for the provider selector.</summary>
    [Parameter] public string ProviderLabel { get; set; } = "Provedor";

    /// <summary>Label for the model selector.</summary>
    [Parameter] public string ModelLabel { get; set; } = "Modelo";

    /// <summary>Whether to show the header.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the status badge.</summary>
    [Parameter] public bool ShowStatus { get; set; } = true;

    /// <summary>Whether to show provider capabilities.</summary>
    [Parameter] public bool ShowCapabilities { get; set; } = true;

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom content below the configuration.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Fired when the selected provider changes.</summary>
    [Parameter] public EventCallback<IAiProvider> OnProviderChange { get; set; }

    /// <summary>Fired when the selected model changes.</summary>
    [Parameter] public EventCallback<string> OnModelChange { get; set; }

    /// <summary>The currently selected provider ID (two-way bindable).</summary>
    [Parameter] public string SelectedProviderId { get; set; } = "inmemory";

    /// <summary>The currently selected model (two-way bindable).</summary>
    [Parameter] public string? SelectedModel { get; set; }

    /// <summary>Currently active provider (read-only).</summary>
    public IAiProvider? ActiveProvider => _activeProvider;

    private IEnumerable<IAiProvider> AvailableProviders => Providers?.Any() == true ? Providers : [];

    protected override async Task OnInitializedAsync()
    {
        if (AvailableProviders.Any())
        {
            _selectedProviderId = AvailableProviders.First().ProviderId;
            await OnProviderChangedAsync();
        }
    }

    private async Task OnProviderChangedAsync()
    {
        _activeProvider = AvailableProviders.FirstOrDefault(p => p.ProviderId == _selectedProviderId);

        if (_activeProvider is not null)
        {
            try
            {
                _availableModels = await _activeProvider.ListModelsAsync();
                _selectedModel = _activeProvider.DefaultModel ?? _availableModels.FirstOrDefault();
            }
            catch
            {
                _availableModels = [];
                _selectedModel = _activeProvider.DefaultModel;
            }

            await OnProviderChange.InvokeAsync(_activeProvider);
            if (_selectedModel is not null)
                await OnModelChange.InvokeAsync(_selectedModel);
        }

        StateHasChanged();
    }

    private async Task OnModelChangedAsync()
    {
        await OnModelChange.InvokeAsync(_selectedModel);
    }

    private string GetContainerClass()
    {
        return "rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 shadow-sm overflow-hidden";
    }

    private string GetStatusBadgeClass()
    {
        var isConnected = _activeProvider is not null;
        return isConnected
            ? "inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400"
            : "inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-500 dark:bg-gray-800 dark:text-gray-400";
    }

    private string GetStatusText()
    {
        return _activeProvider is not null ? $"Conectado ({_activeProvider.DisplayName})" : "Desconectado";
    }
}

