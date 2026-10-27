using System.Globalization;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Maps;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

/// <summary>
/// Tests for Map types at 0% coverage: XFMapCoordinate, XFMapBounds, XFMapView,
/// XFMapEventArgs, XFMapMarkerClickEventArgs, XFMapMarkerDragEventArgs, XFMapShapeStyle,
/// XFMapMarkerIcon, XFMapAnimationOptions, XFMapFitBoundsOptions, XFMapTileSources.
/// </summary>
public sealed class XFMapTypesTest
{
    #region XFMapCoordinate

    [Fact]
    public void XFMapCoordinate_Constructs_WithValues()
    {
        var coord = new XFMapCoordinate(-23.5505, -46.6333);
        Assert.Equal(-23.5505, coord.Latitude, 4);
        Assert.Equal(-46.6333, coord.Longitude, 4);
    }

    [Fact]
    public void XFMapCoordinate_ToString_UsesInvariantCulture()
    {
        var coord = new XFMapCoordinate(48.8566, 2.3522);
        var s = coord.ToString();
        Assert.Equal("48.8566,2.3522", s);
    }

    [Fact]
    public void XFMapCoordinate_Equality_Works()
    {
        var a = new XFMapCoordinate(10, 20);
        var b = new XFMapCoordinate(10, 20);
        var c = new XFMapCoordinate(10, 21);
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void XFMapCoordinate_Default_IsZeroZero()
    {
        var coord = default(XFMapCoordinate);
        Assert.Equal(0, coord.Latitude);
        Assert.Equal(0, coord.Longitude);
    }

    #endregion

    #region XFMapBounds

    [Fact]
    public void XFMapBounds_Constructs_WithCorners()
    {
        var sw = new XFMapCoordinate(-23.6, -46.7);
        var ne = new XFMapCoordinate(-23.5, -46.6);
        var bounds = new XFMapBounds(sw, ne);
        Assert.Equal(sw, bounds.SouthWest);
        Assert.Equal(ne, bounds.NorthEast);
    }

    [Fact]
    public void XFMapBounds_Equality_Works()
    {
        var a = new XFMapBounds(new XFMapCoordinate(0, 0), new XFMapCoordinate(1, 1));
        var b = new XFMapBounds(new XFMapCoordinate(0, 0), new XFMapCoordinate(1, 1));
        Assert.Equal(a, b);
    }

    #endregion

    #region XFMapView

    [Fact]
    public void XFMapView_Defaults_AreCorrect()
    {
        var view = new XFMapView(new XFMapCoordinate(0, 0));
        Assert.Equal(13, view.Zoom);
        Assert.Null(view.Bounds);
    }

    [Fact]
    public void XFMapView_WithValues_AssignsCorrectly()
    {
        var center = new XFMapCoordinate(51.5074, -0.1278);
        var bounds = new XFMapBounds(new XFMapCoordinate(51.4, -0.2), new XFMapCoordinate(51.6, 0.0));
        var view = new XFMapView(center, 10, bounds);
        Assert.Equal(center, view.Center);
        Assert.Equal(10, view.Zoom);
        Assert.NotNull(view.Bounds);
        Assert.Equal(bounds, view.Bounds);
    }

    #endregion

    #region XFMapEventArgs

    [Fact]
    public void XFMapEventArgs_Constructs_WithValues()
    {
        var coord = new XFMapCoordinate(40.7128, -74.0060);
        var args = new XFMapEventArgs(coord, 150, 200);
        Assert.Equal(coord, args.Coordinate);
        Assert.Equal(150, args.ClientX);
        Assert.Equal(200, args.ClientY);
    }

    #endregion

    #region XFMapMarkerClickEventArgs

    [Fact]
    public void XFMapMarkerClickEventArgs_Constructs_WithValues()
    {
        var coord = new XFMapCoordinate(35.6762, 139.6503);
        var args = new XFMapMarkerClickEventArgs("marker-1", coord);
        Assert.Equal("marker-1", args.MarkerId);
        Assert.Equal(coord, args.Coordinate);
    }

    #endregion

    #region XFMapMarkerDragEventArgs

    [Fact]
    public void XFMapMarkerDragEventArgs_Constructs_WithValues()
    {
        var coord = new XFMapCoordinate(35.6762, 139.6503);
        var args = new XFMapMarkerDragEventArgs("marker-2", coord);
        Assert.Equal("marker-2", args.MarkerId);
        Assert.Equal(coord, args.Coordinate);
    }

    #endregion

    #region XFMapShapeStyle

    [Fact]
    public void XFMapShapeStyle_Defaults_AreCorrect()
    {
        var style = new XFMapShapeStyle();
        Assert.Equal("#3388ff", style.StrokeColor);
        Assert.Equal(1, style.StrokeOpacity);
        Assert.Equal(3, style.StrokeWidth);
        Assert.Null(style.StrokeDashArray);
        Assert.Equal("#3388ff", style.FillColor);
        Assert.Equal(0.2, style.FillOpacity);
    }

    [Fact]
    public void XFMapShapeStyle_CustomValues_AssignCorrectly()
    {
        var style = new XFMapShapeStyle
        {
            StrokeColor = "red",
            StrokeOpacity = 0.5,
            StrokeWidth = 5,
            StrokeDashArray = "5 10",
            FillColor = "blue",
            FillOpacity = 0.4
        };
        Assert.Equal("red", style.StrokeColor);
        Assert.Equal(0.5, style.StrokeOpacity);
        Assert.Equal(5, style.StrokeWidth);
        Assert.Equal("5 10", style.StrokeDashArray);
        Assert.Equal("blue", style.FillColor);
        Assert.Equal(0.4, style.FillOpacity);
    }

    #endregion

    #region XFMapMarkerIcon

    [Fact]
    public void XFMapMarkerIcon_Defaults_AreCorrect()
    {
        var icon = new XFMapMarkerIcon();
        Assert.Equal(string.Empty, icon.Url);
        Assert.Null(icon.Size);
        Assert.Null(icon.Anchor);
        Assert.Null(icon.PopupAnchor);
    }

    [Fact]
    public void XFMapMarkerIcon_WithValues_AssignsCorrectly()
    {
        var icon = new XFMapMarkerIcon
        {
            Url = "https://example.com/marker.png",
            Size = (32, 42),
            Anchor = (16, 42),
            PopupAnchor = (0, -42)
        };
        Assert.Equal("https://example.com/marker.png", icon.Url);
        Assert.Equal((32, 42), icon.Size);
        Assert.Equal((16, 42), icon.Anchor);
        Assert.Equal((0, -42), icon.PopupAnchor);
    }

    #endregion

    #region XFMapAnimationOptions

    [Fact]
    public void XFMapAnimationOptions_Defaults_AreCorrect()
    {
        var opts = new XFMapAnimationOptions();
        Assert.True(opts.Animated);
        Assert.Null(opts.Duration);
        Assert.Null(opts.Easing);
    }

    [Fact]
    public void XFMapAnimationOptions_CustomValues_AssignCorrectly()
    {
        var opts = new XFMapAnimationOptions
        {
            Animated = false,
            Duration = 0.5,
            Easing = "easeInOut"
        };
        Assert.False(opts.Animated);
        Assert.Equal(0.5, opts.Duration);
        Assert.Equal("easeInOut", opts.Easing);
    }

    #endregion

    #region XFMapFitBoundsOptions

    [Fact]
    public void XFMapFitBoundsOptions_InheritsAnimationOptions()
    {
        var opts = new XFMapFitBoundsOptions();
        Assert.True(opts.Animated);
        Assert.Null(opts.Padding);
        Assert.Null(opts.MaxZoom);
    }

    [Fact]
    public void XFMapFitBoundsOptions_WithValues_AssignsCorrectly()
    {
        var opts = new XFMapFitBoundsOptions
        {
            Animated = true,
            Duration = 1.0,
            Padding = (20, 20),
            MaxZoom = 18
        };
        Assert.True(opts.Animated);
        Assert.Equal(1.0, opts.Duration);
        Assert.Equal((20, 20), opts.Padding);
        Assert.Equal(18, opts.MaxZoom);
    }

    #endregion

    #region XFMapTileSources

    [Fact]
    public void XFMapTileSources_OpenStreetMap_ContainsPlaceholders()
    {
        Assert.Contains("{z}", XFMapTileSources.OpenStreetMap);
        Assert.Contains("{x}", XFMapTileSources.OpenStreetMap);
        Assert.Contains("{y}", XFMapTileSources.OpenStreetMap);
    }

    [Fact]
    public void XFMapTileSources_CartoDbPositron_ContainsPlaceholders()
    {
        Assert.Contains("{z}", XFMapTileSources.CartoDbPositron);
        Assert.Contains("{s}", XFMapTileSources.CartoDbPositron);
    }

    [Fact]
    public void XFMapTileSources_CartoDbDarkMatter_ContainsPlaceholders()
    {
        Assert.Contains("{z}", XFMapTileSources.CartoDbDarkMatter);
        Assert.Contains("{r}", XFMapTileSources.CartoDbDarkMatter);
    }

    [Fact]
    public void XFMapTileSources_Attributions_AreNotEmpty()
    {
        Assert.NotEmpty(XFMapTileSources.OpenStreetMapAttribution);
        Assert.NotEmpty(XFMapTileSources.CartoDbAttribution);
    }

    [Fact]
    public void XFMapTileSources_Urls_StartWithHttps()
    {
        Assert.StartsWith("https://", XFMapTileSources.OpenStreetMap);
        Assert.StartsWith("https://", XFMapTileSources.CartoDbPositron);
        Assert.StartsWith("https://", XFMapTileSources.CartoDbDarkMatter);
    }

    #endregion
}
