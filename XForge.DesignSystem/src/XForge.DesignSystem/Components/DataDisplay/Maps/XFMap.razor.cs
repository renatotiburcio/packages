using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Maps;

/// <summary>
/// Enterprise interactive map component powered by Leaflet.js.
/// Supports tile layers, markers, circles, polylines, polygons, and rich event handling.
/// Provider-neutral architecture (Leaflet first, extensible to other providers).
/// </summary>
public partial class XFMap : XForgeComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private ElementReference _containerRef;
    private IJSObjectReference? _module;
    private DotNetObjectReference<XFMap>? _dotNetRef;
    private bool _initialized;
    private readonly List<int> _pendingLayers = new();

    /// <summary>
    /// Map center coordinate. Two-way bindable.
    /// </summary>
    [Parameter] public XFMapCoordinate Center { get; set; } = new(-15.7801, -47.9292); // Brasília

    /// <summary>Fires when Center changes.</summary>
    [Parameter] public EventCallback<XFMapCoordinate> CenterChanged { get; set; }

    /// <summary>
    /// Map zoom level (0–19). Two-way bindable.
    /// </summary>
    [Parameter] public double Zoom { get; set; } = 13;

    /// <summary>Fires when Zoom changes.</summary>
    [Parameter] public EventCallback<double> ZoomChanged { get; set; }

    /// <summary>
    /// Map container height. Default: "400px".
    /// </summary>
    [Parameter] public string Height { get; set; } = "400px";

    /// <summary>
    /// Map container width. Default: "100%".
    /// </summary>
    [Parameter] public string Width { get; set; } = "100%";

    /// <summary>
    /// Whether the map is interactive (pan, zoom, click). Default: true.
    /// </summary>
    [Parameter] public bool Interactive { get; set; } = true;

    /// <summary>
    /// Show zoom control. Default: true.
    /// </summary>
    [Parameter] public bool ShowZoomControl { get; set; } = true;

    /// <summary>
    /// Show attribution control. Default: true.
    /// </summary>
    [Parameter] public bool ShowAttribution { get; set; } = true;

    /// <summary>
    /// Minimum zoom level.
    /// </summary>
    [Parameter] public double? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom level.
    /// </summary>
    [Parameter] public double? MaxZoom { get; set; }

    /// <summary>
    /// Accessible label for the map.
    /// </summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>
    /// Child content for map layers (XFMapTileLayer, XFMapMarker, XFMapCircle, etc.).
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // --- Events ---

    /// <summary>Fires when the map is ready (Leaflet loaded and initialized).</summary>
    [Parameter] public EventCallback OnReady { get; set; }

    /// <summary>Fires when the map is clicked.</summary>
    [Parameter] public EventCallback<XFMapEventArgs> OnMapClick { get; set; }

    /// <summary>Fires when the map is double-clicked.</summary>
    [Parameter] public EventCallback<XFMapEventArgs> OnMapDoubleClick { get; set; }

    /// <summary>Fires when the map is right-clicked.</summary>
    [Parameter] public EventCallback<XFMapEventArgs> OnMapContextMenu { get; set; }

    /// <summary>Fires when the map view changes (pan, zoom).</summary>
    [Parameter] public EventCallback<XFMapView> OnViewChanged { get; set; }

    // --- Computed ---

    protected override string? BaseClass => "xf-map relative";

    private string _containerStyle => $"width:{Width};height:{Height};{(Style ?? "")}";

    // --- Lifecycle ---

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);

            try
            {
                _module = await JS.InvokeAsync<IJSObjectReference>("import",
                    "./_content/XForge.DesignSystem/js/xf-map.js?v=1.0.0");
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
                return;
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
                return;
            }

            try
            {
                await _module.InvokeVoidAsync("xfMap.initialize",
                    _containerRef, _dotNetRef,
                    Center.Latitude, Center.Longitude, Zoom,
                    Interactive, ShowZoomControl, ShowAttribution,
                    MinZoom, MaxZoom);
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
                return;
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
                return;
            }

            _initialized = true;
            _pendingLayers.Clear();
            StateHasChanged();
        }
    }

    // --- Public Methods ---

    /// <summary>Sets the map view to the specified center and zoom.</summary>
    public async Task SetViewAsync(XFMapCoordinate center, double zoom, bool animated = true)
    {
        Center = center;
        Zoom = zoom;
        if (_initialized && _module != null)
        {
            await _module.InvokeVoidAsync("xfMap.setView", _containerRef,
                center.Latitude, center.Longitude, zoom, animated);
        }
    }

    /// <summary>Pans the map to the specified center.</summary>
    public async Task PanToAsync(XFMapCoordinate center, bool animated = true)
    {
        Center = center;
        if (_initialized && _module != null)
        {
            await _module.InvokeVoidAsync("xfMap.panTo", _containerRef,
                center.Latitude, center.Longitude, animated);
        }
    }

    /// <summary>Zooms in one level.</summary>
    public async Task ZoomInAsync()
    {
        if (_initialized && _module != null)
        {
            await _module.InvokeVoidAsync("xfMap.zoomIn", _containerRef);
        }
    }

    /// <summary>Zooms out one level.</summary>
    public async Task ZoomOutAsync()
    {
        if (_initialized && _module != null)
        {
            await _module.InvokeVoidAsync("xfMap.zoomOut", _containerRef);
        }
    }

    /// <summary>Fits the map to the specified bounds.</summary>
    public async Task FitBoundsAsync(XFMapBounds bounds, XFMapFitBoundsOptions? options = null)
    {
        if (_initialized && _module != null)
        {
            var padding = options?.Padding != null
                ? new[] { options.Padding.Value.X, options.Padding.Value.Y }
                : null;
            await _module.InvokeVoidAsync("xfMap.fitBounds", _containerRef,
                bounds.SouthWest.Latitude, bounds.SouthWest.Longitude,
                bounds.NorthEast.Latitude, bounds.NorthEast.Longitude,
                options?.Animated ?? true, padding);
        }
    }

    /// <summary>Gets the current map view state.</summary>
    public async Task<XFMapView?> GetViewAsync()
    {
        if (_initialized && _module != null)
        {
            var result = await _module.InvokeAsync<XFMapViewJS>("xfMap.getView", _containerRef);
            if (result != null)
            {
                return new XFMapView(
                    new XFMapCoordinate(result.lat, result.lng),
                    result.zoom,
                    new XFMapBounds(
                        new XFMapCoordinate(result.south, result.west),
                        new XFMapCoordinate(result.north, result.east)));
            }
        }
        return null;
    }

    /// <summary>Invalidates the map size (call after container resize).</summary>
    public async Task InvalidateSizeAsync()
    {
        if (_initialized && _module != null)
        {
            await _module.InvokeVoidAsync("xfMap.invalidateSize", _containerRef);
        }
    }

    // --- Internal Layer Methods (called by child components) ---

    internal async Task<int> AddTileLayerAsync(string source, string? attribution, int tileSize,
        double? minZoom, double? maxZoom, string[]? subdomains)
    {
        if (_initialized && _module != null)
        {
            return await _module.InvokeAsync<int>("xfMap.addTileLayer", _containerRef,
                source, attribution ?? "", tileSize, minZoom, maxZoom, subdomains);
        }
        return -1;
    }

    internal async Task<int> AddMarkerAsync(string markerId, XFMapCoordinate position,
        string? title, string? popupText, bool draggable, XFMapMarkerIcon? icon)
    {
        if (_initialized && _module != null)
        {
            object? iconJs = icon != null ? new
            {
                url = icon.Url,
                size = icon.Size != null ? new[] { icon.Size.Value.Width, icon.Size.Value.Height } : null,
                anchor = icon.Anchor != null ? new[] { icon.Anchor.Value.X, icon.Anchor.Value.Y } : null,
                popupAnchor = icon.PopupAnchor != null ? new[] { icon.PopupAnchor.Value.X, icon.PopupAnchor.Value.Y } : null,
            } : null;

            return await _module.InvokeAsync<int>("xfMap.addMarker", _containerRef,
                markerId, position.Latitude, position.Longitude,
                title, popupText, draggable, iconJs);
        }
        return -1;
    }

    internal async Task<int> AddCircleAsync(XFMapCoordinate center, double radius, XFMapShapeStyle style)
    {
        if (_initialized && _module != null)
        {
            return await _module.InvokeAsync<int>("xfMap.addCircle", _containerRef,
                center.Latitude, center.Longitude, radius, StyleToJS(style));
        }
        return -1;
    }

    internal async Task<int> AddPolylineAsync(IReadOnlyList<XFMapCoordinate> path, XFMapShapeStyle style)
    {
        if (_initialized && _module != null)
        {
            var paths = path.Select(c => new[] { c.Latitude, c.Longitude }).ToArray();
            return await _module.InvokeAsync<int>("xfMap.addPolyline", _containerRef, paths, StyleToJS(style));
        }
        return -1;
    }

    internal async Task<int> AddPolygonAsync(IReadOnlyList<IReadOnlyList<XFMapCoordinate>> rings, XFMapShapeStyle style)
    {
        if (_initialized && _module != null)
        {
            var ringArrays = rings.Select(r => r.Select(c => new[] { c.Latitude, c.Longitude }).ToArray()).ToArray();
            return await _module.InvokeAsync<int>("xfMap.addPolygon", _containerRef, ringArrays, StyleToJS(style));
        }
        return -1;
    }

    internal async Task RemoveLayerAsync(int layerIndex)
    {
        if (_initialized && _module != null)
        {
            await _module.InvokeVoidAsync("xfMap.removeLayer", _containerRef, layerIndex);
        }
    }

    internal async Task ClearLayersAsync()
    {
        if (_initialized && _module != null)
        {
            await _module.InvokeVoidAsync("xfMap.clearLayers", _containerRef);
        }
    }

    internal bool IsReady => _initialized;

    // --- JS Callbacks ---

    [JSInvokable]
    public async Task OnMapReadyJS()
    {
        await OnReady.InvokeAsync();
    }

    [JSInvokable]
    public async Task OnMapClickJS(double lat, double lng, double clientX, double clientY)
    {
        await OnMapClick.InvokeAsync(new XFMapEventArgs(new XFMapCoordinate(lat, lng), clientX, clientY));
    }

    [JSInvokable]
    public async Task OnMapDoubleClickJS(double lat, double lng, double clientX, double clientY)
    {
        await OnMapDoubleClick.InvokeAsync(new XFMapEventArgs(new XFMapCoordinate(lat, lng), clientX, clientY));
    }

    [JSInvokable]
    public async Task OnMapContextMenuJS(double lat, double lng, double clientX, double clientY)
    {
        await OnMapContextMenu.InvokeAsync(new XFMapEventArgs(new XFMapCoordinate(lat, lng), clientX, clientY));
    }

    [JSInvokable]
    public async Task OnViewChangedJS(double lat, double lng, double zoom,
        double south, double west, double north, double east, string source)
    {
        Center = new XFMapCoordinate(lat, lng);
        Zoom = zoom;
        await CenterChanged.InvokeAsync(Center);
        await ZoomChanged.InvokeAsync(Zoom);
        await OnViewChanged.InvokeAsync(new XFMapView(Center, zoom,
            new XFMapBounds(new XFMapCoordinate(south, west), new XFMapCoordinate(north, east))));
    }

    [JSInvokable]
    public async Task OnMarkerClickJS(string markerId, double lat, double lng)
    {
        // Marker click is handled by child components — forward via callback
    }

    [JSInvokable]
    public async Task OnMarkerDragEndJS(string markerId, double lat, double lng)
    {
        // Marker drag is handled by child components — forward via callback
    }

    // --- Helpers ---

    private static object StyleToJS(XFMapShapeStyle style) => new
    {
        strokeColor = style.StrokeColor,
        strokeOpacity = style.StrokeOpacity,
        strokeWidth = style.StrokeWidth,
        strokeDashArray = style.StrokeDashArray,
        fillColor = style.FillColor,
        fillOpacity = style.FillOpacity,
    };

    // --- Disposal ---

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module != null)
            {
                await _module.InvokeVoidAsync("xfMap.destroy", _containerRef);
                await _module.DisposeAsync();
            }
            _dotNetRef?.Dispose();
        }
        catch
        {
            // Ignore disposal errors
        }
    }

    /// <summary>JS view result DTO.</summary>
    private record XFMapViewJS(double lat, double lng, double zoom,
        double south, double west, double north, double east);
}
