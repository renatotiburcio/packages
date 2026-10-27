using System.Globalization;

namespace XForge.DesignSystem.Components.DataDisplay.Maps;

/// <summary>
/// Geographic coordinate (latitude/longitude).
/// </summary>
public readonly record struct XFMapCoordinate(double Latitude, double Longitude)
{
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{Latitude},{Longitude}");
}

/// <summary>
/// Geographic bounds (south-west and north-east corners).
/// </summary>
public readonly record struct XFMapBounds(XFMapCoordinate SouthWest, XFMapCoordinate NorthEast);

/// <summary>
/// Map view state (center + zoom).
/// </summary>
public record XFMapView(XFMapCoordinate Center, double Zoom = 13, XFMapBounds? Bounds = null);

/// <summary>
/// Map event args for click/double-click/context-menu on the map surface.
/// </summary>
public record XFMapEventArgs(XFMapCoordinate Coordinate, double ClientX, double ClientY);

/// <summary>
/// Marker click event args.
/// </summary>
public record XFMapMarkerClickEventArgs(string MarkerId, XFMapCoordinate Coordinate);

/// <summary>
/// Marker drag event args.
/// </summary>
public record XFMapMarkerDragEventArgs(string MarkerId, XFMapCoordinate Coordinate);

/// <summary>
/// Shape styling for circles, polylines, and polygons.
/// </summary>
public class XFMapShapeStyle
{
    /// <summary>Stroke color. Default: "#3388ff" (Leaflet blue).</summary>
    public string StrokeColor { get; set; } = "#3388ff";

    /// <summary>Stroke opacity (0–1). Default: 1.</summary>
    public double StrokeOpacity { get; set; } = 1;

    /// <summary>Stroke width in pixels. Default: 3.</summary>
    public double StrokeWidth { get; set; } = 3;

    /// <summary>Stroke dash array (e.g. "5 10"). Default: null (solid).</summary>
    public string? StrokeDashArray { get; set; }

    /// <summary>Fill color. Default: "#3388ff".</summary>
    public string FillColor { get; set; } = "#3388ff";

    /// <summary>Fill opacity (0–1). Default: 0.2.</summary>
    public double FillOpacity { get; set; } = 0.2;
}

/// <summary>
/// Custom marker icon.
/// </summary>
public class XFMapMarkerIcon
{
    /// <summary>Icon image URL.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Icon size (width, height) in pixels.</summary>
    public (int Width, int Height)? Size { get; set; }

    /// <summary>Icon anchor point (from top-left) in pixels.</summary>
    public (int X, int Y)? Anchor { get; set; }

    /// <summary>Popup anchor point (from icon anchor) in pixels.</summary>
    public (int X, int Y)? PopupAnchor { get; set; }
}

/// <summary>
/// Map animation options for programmatic view changes.
/// </summary>
public class XFMapAnimationOptions
{
    /// <summary>Whether to animate the transition. Default: true.</summary>
    public bool Animated { get; set; } = true;

    /// <summary>Animation duration in seconds. Default: null (Leaflet default: 0.25).</summary>
    public double? Duration { get; set; }

    /// <summary>Easing function name. Default: null (Leaflet default: "ease").</summary>
    public string? Easing { get; set; }
}

/// <summary>
/// Options for FitBounds.
/// </summary>
public class XFMapFitBoundsOptions : XFMapAnimationOptions
{
    /// <summary>Padding in pixels. Default: null (Leaflet default: [0, 0]).</summary>
    public (int X, int Y)? Padding { get; set; }

    /// <summary>Maximum zoom level. Default: null.</summary>
    public double? MaxZoom { get; set; }
}

/// <summary>
/// Tile layer configuration for the default OpenStreetMap tile source.
/// </summary>
public static class XFMapTileSources
{
    /// <summary>OpenStreetMap standard tiles. Requires attribution.</summary>
    public const string OpenStreetMap = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

    /// <summary>OpenStreetMap attribution (required by OSM license).</summary>
    public const string OpenStreetMapAttribution =
        "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors";

    /// <summary>CartoDB Positron (light) tiles — clean, minimal style.</summary>
    public const string CartoDbPositron = "https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png";

    /// <summary>CartoDB Dark Matter tiles — dark theme friendly.</summary>
    public const string CartoDbDarkMatter = "https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png";

    /// <summary>CartoDB attribution.</summary>
    public const string CartoDbAttribution =
        "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors &copy; <a href=\"https://carto.com/attributions\">CARTO</a>";
}
