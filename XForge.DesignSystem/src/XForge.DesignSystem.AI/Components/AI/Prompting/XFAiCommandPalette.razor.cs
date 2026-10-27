using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Prompting;

/// <summary>
/// VSCode/Copilot-style command palette for AI commands.
/// Provides fuzzy search, keyboard navigation, slash command detection, and recent actions tracking.
/// </summary>
public partial class XFAiCommandPalette : XForgeComponentBase
{
    private const int MaxRecentActions = 5;

    private string _searchText = string.Empty;
    private int _selectedIndex;
    private ElementReference _searchInputRef;
    private readonly List<XFAiCommandItem> _recentActions = new();

    /// <summary>Whether the palette is visible (two-way bindable).</summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>Two-way binding callback for Visible.</summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>Available commands for the palette.</summary>
    [Parameter] public IReadOnlyList<XFAiCommandItem> Commands { get; set; } = Array.Empty<XFAiCommandItem>();

    /// <summary>Search input placeholder text.</summary>
    [Parameter] public string Placeholder { get; set; } = "Pesquisar comandos ou digite /...";

    /// <summary>Maximum number of visible results.</summary>
    [Parameter] public int MaxResults { get; set; } = 10;

    /// <summary>Whether to show recent actions section.</summary>
    [Parameter] public bool ShowRecentActions { get; set; } = true;

    /// <summary>Label for the recent actions section.</summary>
    [Parameter] public string RecentActionsLabel { get; set; } = "Recentes";

    /// <summary>Label for the commands section.</summary>
    [Parameter] public string CommandsLabel { get; set; } = "Comandos";

    /// <summary>Text displayed when no results are found.</summary>
    [Parameter] public string NoResultsText { get; set; } = "Nenhum resultado encontrado.";

    /// <summary>Custom template for rendering each command item.</summary>
    [Parameter] public RenderFragment<XFAiCommandItem>? CommandTemplate { get; set; }

    /// <summary>Content shown when search is empty.</summary>
    [Parameter] public RenderFragment? EmptySearchContent { get; set; }

    /// <summary>Fires when a command is executed.</summary>
    [Parameter] public EventCallback<XFAiCommandItem> OnCommandExecute { get; set; }

    /// <summary>Fires when search input changes.</summary>
    [Parameter] public EventCallback<string> OnSearch { get; set; }

    private bool IsSlashMode => _searchText.StartsWith("/", StringComparison.OrdinalIgnoreCase);

    private List<XFAiCommandItem> FilteredCommands
    {
        get
        {
            IEnumerable<XFAiCommandItem> source;

            if (IsSlashMode)
            {
                source = Commands.Where(c => c.IsSlashCommand);
            }
            else if (ShowRecentActions && string.IsNullOrWhiteSpace(_searchText) && _recentActions.Count > 0)
            {
                source = Commands;
            }
            else
            {
                source = Commands;
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var query = _searchText.Trim();
                source = source.Where(c => FuzzyMatch(c, query));
            }

            return source.Take(MaxResults).ToList();
        }
    }

    private List<XFAiCommandItem> VisibleRecentActions
    {
        get
        {
            if (!ShowRecentActions || !string.IsNullOrWhiteSpace(_searchText) || _recentActions.Count == 0)
                return new List<XFAiCommandItem>();

            return _recentActions.Take(MaxRecentActions).ToList();
        }
    }

    private int TotalVisibleItems => VisibleRecentActions.Count + FilteredCommands.Count;

    private string ActiveDescendantId => TotalVisibleItems > 0 && _selectedIndex >= 0
        ? $"{Id}-option-{_selectedIndex}"
        : string.Empty;

    private bool HasResults => TotalVisibleItems > 0;

    private static bool FuzzyMatch(XFAiCommandItem item, string query)
    {
        return ContainsIgnoreCase(item.Label, query)
            || ContainsIgnoreCase(item.Description, query)
            || ContainsIgnoreCase(item.Category, query)
            || ContainsIgnoreCase(item.SlashPrefix, query);
    }

    private static bool ContainsIgnoreCase(string? source, string value)
    {
        if (string.IsNullOrEmpty(source)) return false;
        return source.Contains(value, StringComparison.OrdinalIgnoreCase);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Visible)
        {
            await _searchInputRef.FocusAsync();
        }
    }

    private async Task HandleSearchInputAsync(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;
        _selectedIndex = 0;
        await OnSearch.InvokeAsync(_searchText);
        StateHasChanged();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowDown":
                _selectedIndex = Math.Min(_selectedIndex + 1, TotalVisibleItems - 1);
                break;

            case "ArrowUp":
                _selectedIndex = Math.Max(_selectedIndex - 1, 0);
                break;

            case "Enter":
                await ExecuteSelectedCommandAsync();
                break;

            case "Escape":
                await CloseAsync();
                break;
        }
    }

    private async Task ExecuteSelectedCommandAsync()
    {
        var allItems = VisibleRecentActions.Concat(FilteredCommands).ToList();
        if (_selectedIndex < 0 || _selectedIndex >= allItems.Count) return;

        var command = allItems[_selectedIndex];
        if (command.Disabled) return;

        TrackRecentAction(command);
        await OnCommandExecute.InvokeAsync(command);
        await CloseAsync();
    }

    private async Task ExecuteCommandAsync(XFAiCommandItem command)
    {
        if (command.Disabled) return;

        TrackRecentAction(command);
        await OnCommandExecute.InvokeAsync(command);
        await CloseAsync();
    }

    private void TrackRecentAction(XFAiCommandItem command)
    {
        _recentActions.RemoveAll(r => r.Id == command.Id);
        _recentActions.Insert(0, command);

        while (_recentActions.Count > MaxRecentActions)
        {
            _recentActions.RemoveAt(_recentActions.Count - 1);
        }
    }

    private async Task CloseAsync()
    {
        _searchText = string.Empty;
        _selectedIndex = 0;
        Visible = false;

        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(false);
        }
    }

    private async Task HandleOverlayClickAsync()
    {
        await CloseAsync();
    }

    private string GetOptionId(int index) => $"{Id}-option-{index}";

    private bool IsSelected(int index) => _selectedIndex == index;

    private int GetGlobalIndex(int localIndex, bool isRecent)
    {
        return isRecent ? localIndex : VisibleRecentActions.Count + localIndex;
    }

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
}