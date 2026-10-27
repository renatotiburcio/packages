using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XForge.DesignSystem.Services;

/// <summary>
/// Represents the type of a rail navigation item, determining its behavior and visual presentation.
/// </summary>
public enum RailNavItemType
{
    /// <summary>
    /// A navigational link that supports active state detection based on the current URL.
    /// </summary>
    Route,

    /// <summary>
    /// A functional trigger (e.g., Logout, Settings) that never receives active state.
    /// </summary>
    Action,

    /// <summary>
    /// A visual divider with constrained layout. Never receives active state.
    /// </summary>
    Separator
}

/// <summary>
/// Represents a rail navigation item with brand metadata and active state tracking.
/// </summary>
public class RailNavItem
{
    private bool _isActive;

    /// <summary>
    /// The type of this navigation item, determining its behavior.
    /// Defaults to Route for backward compatibility.
    /// </summary>
    public RailNavItemType Type { get; init; } = RailNavItemType.Route;

    /// <summary>
    /// The brand identifier. E.g., "components", "tokens", "forms"
    /// Only used when Type is Route.
    /// </summary>
    public required string Brand { get; init; }

    /// <summary>
    /// Display label for the item.
    /// </summary>
    public required string Label { get; init; }

    /// <summary>
    /// Optional icon (SVG string or icon identifier).
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Route prefixes that activate this item when Type is Route.
    /// </summary>
    public string[]? RoutePrefixes { get; init; }

    /// <summary>
    /// The overview page route for this brand.
    /// </summary>
    public string? OverviewRoute { get; init; }

    /// <summary>
    /// Whether this is the default/home item.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Gets or sets whether this item is currently active.
    /// Only Route items can be active.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (Type == RailNavItemType.Route)
                _isActive = value;
            else
                _isActive = false;
        }
    }

    /// <summary>
    /// The height of this item in pixels for indicator positioning.
    /// </summary>
    public int Height => Type switch
    {
        RailNavItemType.Separator => 24,
        RailNavItemType.Action => 40,
        RailNavItemType.Route => 40,
        _ => 40
    };

    /// <summary>
    /// Gets whether this item should be considered for keyboard focus and active indicators.
    /// </summary>
    public bool IsInteractive => Type is RailNavItemType.Route or RailNavItemType.Action;

    /// <summary>
    /// Gets the navigation target for this item.
    /// </summary>
    public string? GetNavigationRoute()
    {
        if (Type != RailNavItemType.Route)
            return null;
        if (!string.IsNullOrEmpty(OverviewRoute))
            return OverviewRoute;
        return $"/{Brand.ToLowerInvariant()}/overview";
    }

    public bool MatchesPath(string path)
    {
        if (Type != RailNavItemType.Route)
            return false;

        var normalizedPath = NormalizePath(path);
        if (string.IsNullOrEmpty(normalizedPath) || normalizedPath == "/")
            return IsDefault;

        if (RoutePrefixes is { Length: > 0 })
        {
            return RoutePrefixes
                .Select(NormalizePath)
                .Where(prefix => !string.IsNullOrEmpty(prefix))
                .Any(prefix => prefix == "/"
                    ? IsDefault && normalizedPath == "/"
                    : normalizedPath.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                      normalizedPath.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase));
        }

        return normalizedPath.StartsWith($"/{Brand}", StringComparison.OrdinalIgnoreCase);
    }

    internal static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var normalized = path.Trim();

        var queryIndex = normalized.IndexOf('?');
        if (queryIndex >= 0)
            normalized = normalized[..queryIndex];
        var hashIndex = normalized.IndexOf('#');
        if (hashIndex >= 0)
            normalized = normalized[..hashIndex];

        if (!normalized.StartsWith('/'))
            normalized = "/" + normalized;

        return normalized == "/" ? "/" : normalized.TrimEnd('/');
    }
}

/// <summary>
/// Centralized navigation state service with localStorage persistence for brand state.
/// </summary>
public class NavigationStateService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime? _jsRuntime;
    private readonly List<RailNavItem> _railItems = new();
    private string _currentBrand = string.Empty;
    private string _currentPath = string.Empty;
    private bool _disposed;
    private bool _initialized;
    private bool _isNavigating; // Prevents cascading navigation during InitializeAsync

    private const string LocalStorageKey = "xf-nav-brand";

    public event EventHandler<string>? BrandChanged;
    public event EventHandler<RailNavItem?>? ActiveItemChanged;
    public event EventHandler<int>? ActiveIndexChanged;

    public string CurrentBrand => _currentBrand;
    public string CurrentPath => _currentPath;
    public IReadOnlyList<RailNavItem> RailItems => _railItems.AsReadOnly();
    public bool IsInitialized => _initialized;

    public NavigationStateService(NavigationManager navigationManager, IJSRuntime? jsRuntime = null)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
        _currentPath = _navigationManager.Uri;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    /// <summary>
    /// Initialize rail items and synchronize state. Restores brand from localStorage on first load.
    /// </summary>
    public async Task InitializeAsync(IEnumerable<RailNavItem> items)
    {
        var previousBrand = _currentBrand;
        var previousItem = GetActiveItem();

        // Add items to the list first (don't sync - _currentBrand is not set yet)
        _railItems.Clear();
        _railItems.AddRange(items);

        var path = new Uri(_navigationManager.Uri).AbsolutePath;
        var urlBrand = ResolveBrandFromPath(path);

        var persistedBrand = await GetPersistedBrandAsync();
        _currentBrand = !string.IsNullOrEmpty(urlBrand) ? urlBrand : persistedBrand ?? string.Empty;

        // Now sync active state after _currentBrand is correctly set
        SyncItemsActiveState();
        _initialized = true;
        if (!string.Equals(previousBrand, _currentBrand, StringComparison.OrdinalIgnoreCase))
        {
            BrandChanged?.Invoke(this, _currentBrand);
        }

        var currentItem = GetActiveItem();
        if (!ReferenceEquals(previousItem, currentItem))
        {
            ActiveItemChanged?.Invoke(this, currentItem);
            ActiveIndexChanged?.Invoke(this, GetActiveIndex());
        }

        _ = PersistBrandAsync(_currentBrand);
        // Note: We don't auto-navigate here because:
        // 1. The visual state (IsActive) is already correctly synced
        // 2. Auto-navigation can cause infinite loops when combined with LocationChanged
    }

    /// <summary>
    /// Register rail navigation items. Does NOT sync active state (call SyncItemsActiveState separately).
    /// </summary>
    public void RegisterRailItems(IEnumerable<RailNavItem> items)
    {
        _railItems.Clear();
        _railItems.AddRange(items);
    }

    /// <summary>
    /// Sync IsActive on all items based on current brand.
    /// </summary>
    public void SyncItemsActiveState()
    {
        var path = new Uri(_navigationManager.Uri).AbsolutePath;
        foreach (var item in _railItems)
        {
            item.IsActive = item.Type == RailNavItemType.Route &&
                           item.Brand.Equals(_currentBrand, StringComparison.OrdinalIgnoreCase) &&
                           item.MatchesPath(path);
        }

        if (!_railItems.Any(x => x.IsActive))
        {
            var fallback = _railItems.FirstOrDefault(item =>
                item.Type == RailNavItemType.Route &&
                item.Brand.Equals(_currentBrand, StringComparison.OrdinalIgnoreCase));
            if (fallback != null)
            {
                fallback.IsActive = true;
            }
        }
    }

    /// <summary>
    /// Navigate to a specific brand's overview page.
    /// </summary>
    public void NavigateToBrand(string brand)
    {
        var item = _railItems.FirstOrDefault(x =>
            x.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));
        var target = item?.GetNavigationRoute() ?? $"/{brand.ToLowerInvariant()}/overview";
        _navigationManager.NavigateTo(target);
    }

    /// <summary>
    /// Get the active rail item index for indicator positioning.
    /// </summary>
    public int GetActiveIndex()
    {
        return _railItems.FindIndex(x => x.IsActive);
    }

    /// <summary>
    /// Get the currently active rail item.
    /// </summary>
    public RailNavItem? GetActiveItem()
    {
        return _railItems.FirstOrDefault(x => x.IsActive);
    }

    /// <summary>
    /// Calculate the vertical offset for the active indicator.
    /// </summary>
    public int GetIndicatorOffset()
    {
        var activeIndex = GetActiveIndex();
        if (activeIndex < 0)
            return -1;

        int offset = 0;
        for (int i = 0; i < activeIndex; i++)
        {
            offset += _railItems[i].Height + ItemGap;
        }

        var activeItem = _railItems[activeIndex];
        int centeringOffset = (activeItem.Height - IndicatorHeight) / 2;
        offset += centeringOffset;

        return offset;
    }

    public const int IndicatorHeight = 28;
    public const int ItemGap = 16;

    /// <summary>
    /// Check if a specific brand is currently active.
    /// </summary>
    public bool IsBrandActive(string brand)
    {
        return _currentBrand.Equals(brand, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Check if a specific rail item is currently active.
    /// </summary>
    public bool IsItemActive(RailNavItem item)
    {
        return item.IsActive;
    }

    public static string DeriveBrandFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        path = path.Trim('/').ToLowerInvariant();

        var queryIndex = path.IndexOf('?');
        if (queryIndex >= 0)
            path = path[..queryIndex];
        var hashIndex = path.IndexOf('#');
        if (hashIndex >= 0)
            path = path[..hashIndex];

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return string.Empty;

        return segments[0];
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _currentPath = e.Location;
        if (_isNavigating)
            return; // Ignore location changes during navigation initiated by InitializeAsync
        ParseCurrentRoute();
    }

    /// <summary>
    /// Navigate to a specific rail item's overview page.
    /// </summary>
    public void NavigateToItem(RailNavItem item)
    {
        if (item.Type != RailNavItemType.Route)
            return;
        var route = item.GetNavigationRoute();
        if (!string.IsNullOrEmpty(route))
        {
            _isNavigating = true;
            _navigationManager.NavigateTo(route);
            _isNavigating = false;
        }
    }

    private void ParseCurrentRoute()
    {
        var path = new Uri(_currentPath).AbsolutePath;
        var derivedBrand = ResolveBrandFromPath(path);

        if (string.Equals(_currentBrand, derivedBrand, StringComparison.OrdinalIgnoreCase))
            return; // No change, don't trigger events

        var previousBrand = _currentBrand;
        var previousItem = GetActiveItem();

        _currentBrand = derivedBrand;
        SyncItemsActiveState();

        BrandChanged?.Invoke(this, _currentBrand);

        var currentItem = GetActiveItem();
        if (previousItem != currentItem)
        {
            ActiveItemChanged?.Invoke(this, currentItem);
            var newIndex = GetActiveIndex();
            ActiveIndexChanged?.Invoke(this, newIndex);
        }

        _ = PersistBrandAsync(_currentBrand);
    }

    private string ResolveBrandFromPath(string path)
    {
        var normalizedPath = RailNavItem.NormalizePath(path);
        if (string.IsNullOrEmpty(normalizedPath) || normalizedPath == "/")
        {
            return _railItems.FirstOrDefault(x => x.Type == RailNavItemType.Route && x.IsDefault)?.Brand ?? string.Empty;
        }

        var routeMatch = _railItems
            .Where(x => x.Type == RailNavItemType.Route)
            .Select(item => new
            {
                Item = item,
                Prefix = item.RoutePrefixes?
                    .Select(RailNavItem.NormalizePath)
                    .Where(prefix => !string.IsNullOrEmpty(prefix) && prefix != "/")
                    .Where(prefix => normalizedPath.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                                     normalizedPath.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(prefix => prefix.Length)
                    .FirstOrDefault()
            })
            .Where(x => x.Prefix != null)
            .OrderByDescending(x => x.Prefix!.Length)
            .FirstOrDefault();

        if (routeMatch != null)
        {
            return routeMatch.Item.Brand;
        }

        var derivedBrand = DeriveBrandFromPath(normalizedPath);
        return _railItems.Any(x => x.Type == RailNavItemType.Route &&
                                   x.Brand.Equals(derivedBrand, StringComparison.OrdinalIgnoreCase))
            ? derivedBrand
            : string.Empty;
    }

    private async Task<string?> GetPersistedBrandAsync()
    {
        try
        {
            if (_jsRuntime != null)
            {
                return await _jsRuntime.InvokeAsync<string?>("xfNavState.getBrand");
            }
        }
        catch
        {
        }
        return null;
    }

    private async Task PersistBrandAsync(string brand)
    {
        try
        {
            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("xfNavState.setBrand", brand);
            }
        }
        catch
        {
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _navigationManager.LocationChanged -= OnLocationChanged;
        GC.SuppressFinalize(this);
    }
}
