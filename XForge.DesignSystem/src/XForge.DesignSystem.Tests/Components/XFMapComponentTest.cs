using Bunit;
using Bunit.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using XForge.DesignSystem.Components.DataDisplay.Maps;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFMapComponentTest : XForgeTestContext
{
    public XFMapComponentTest()
    {
        // XFMap loads Leaflet.js via ES module import — configure bUnit to handle it loosely
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_Default_RendersContainer()
    {
        var cut = Render<XFMap>();
        Assert.Contains("xf-map", cut.Markup);
        Assert.Contains("role=\"application\"", cut.Markup);
    }

    [Fact]
    public void Render_CustomAriaLabel_RendersLabel()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.AriaLabel, "Mapa de filiais"));
        Assert.Contains("aria-label=\"Mapa de filiais\"", cut.Markup);
    }

    [Fact]
    public void Render_DefaultAriaLabel_RendersDefault()
    {
        var cut = Render<XFMap>();
        Assert.Contains("aria-label=\"Mapa interativo\"", cut.Markup);
    }

    [Fact]
    public void Render_CustomHeight_RendersStyle()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Height, "500px"));
        Assert.Contains("height:500px", cut.Markup);
    }

    [Fact]
    public void Render_CustomWidth_RendersStyle()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Width, "80%"));
        Assert.Contains("width:80%", cut.Markup);
    }

    [Fact]
    public void Render_CustomId_RendersId()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Id, "my-map"));
        Assert.Contains("id=\"my-map\"", cut.Markup);
        Assert.Contains("data-map-id=\"my-map\"", cut.Markup);
    }

    [Fact]
    public void Render_CustomClass_RendersClass()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Class, "custom-map"));
        Assert.Contains("custom-map", cut.Markup);
    }

    [Fact]
    public void Render_CustomStyle_RendersStyle()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.Style, "border: 1px solid red"));
        Assert.Contains("border: 1px solid red", cut.Markup);
    }

    [Fact]
    public void Render_AdditionalAttributes_RendersAttribute()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-test", "map" } }));
        Assert.Contains("data-test=\"map\"", cut.Markup);
    }

    [Fact]
    public void Parameter_Defaults_AreCorrect()
    {
        var cut = Render<XFMap>();
        var instance = cut.Instance;
        Assert.Equal(-15.7801, instance.Center.Latitude, 3);
        Assert.Equal(-47.9292, instance.Center.Longitude, 3);
        Assert.Equal(13, instance.Zoom);
        Assert.Equal("400px", instance.Height);
        Assert.Equal("100%", instance.Width);
        Assert.True(instance.Interactive);
        Assert.True(instance.ShowZoomControl);
        Assert.True(instance.ShowAttribution);
        Assert.Null(instance.MinZoom);
        Assert.Null(instance.MaxZoom);
        Assert.Null(instance.AriaLabel);
    }

    [Fact]
    public void Render_WithChildContent_RendersChild()
    {
        var cut = Render<XFMap>(p => p.Add(x => x.ChildContent, builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, "Map content");
            builder.CloseElement();
        }));
        Assert.Contains("Map content", cut.Markup);
    }

    [Fact]
    public void XFMapCoordinate_ToString_FormatsCorrectly()
    {
        var coord = new XFMapCoordinate(-23.5505, -46.6333);
        var result = coord.ToString();
        Assert.Contains("-23", result);
        Assert.Contains("-46", result);
        Assert.Contains(",", result); // invariant culture uses decimal point
        // Verify invariant culture: uses '.' not ','
        Assert.Contains(".", result);
    }

    [Fact]
    public void XFMapCoordinate_Equality_Works()
    {
        var a = new XFMapCoordinate(10, 20);
        var b = new XFMapCoordinate(10, 20);
        var c = new XFMapCoordinate(10, 30);
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }

    [Fact]
    public void XFMapBounds_CreatesCorrectly()
    {
        var bounds = new XFMapBounds(
            new XFMapCoordinate(-10, -20),
            new XFMapCoordinate(10, 20));
        Assert.Equal(-10, bounds.SouthWest.Latitude);
        Assert.Equal(-20, bounds.SouthWest.Longitude);
        Assert.Equal(10, bounds.NorthEast.Latitude);
        Assert.Equal(20, bounds.NorthEast.Longitude);
    }

    [Fact]
    public void XFMapView_CreatesCorrectly()
    {
        var view = new XFMapView(new XFMapCoordinate(10, 20), 15);
        Assert.Equal(10, view.Center.Latitude);
        Assert.Equal(20, view.Center.Longitude);
        Assert.Equal(15, view.Zoom);
        Assert.Null(view.Bounds);
    }

    [Fact]
    public void XFMapEventArgs_CreatesCorrectly()
    {
        var args = new XFMapEventArgs(new XFMapCoordinate(10, 20), 100, 200);
        Assert.Equal(10, args.Coordinate.Latitude);
        Assert.Equal(20, args.Coordinate.Longitude);
        Assert.Equal(100, args.ClientX);
        Assert.Equal(200, args.ClientY);
    }

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
    public void XFMapMarkerIcon_CreatesCorrectly()
    {
        var icon = new XFMapMarkerIcon
        {
            Url = "https://example.com/icon.png",
            Size = (32, 32),
            Anchor = (16, 32),
            PopupAnchor = (0, -32)
        };
        Assert.Equal("https://example.com/icon.png", icon.Url);
        Assert.Equal((32, 32), icon.Size);
        Assert.Equal((16, 32), icon.Anchor);
        Assert.Equal((0, -32), icon.PopupAnchor);
    }

    [Fact]
    public void XFMapAnimationOptions_Defaults_AreCorrect()
    {
        var options = new XFMapAnimationOptions();
        Assert.True(options.Animated);
        Assert.Null(options.Duration);
        Assert.Null(options.Easing);
    }

    [Fact]
    public void XFMapFitBoundsOptions_Defaults_AreCorrect()
    {
        var options = new XFMapFitBoundsOptions();
        Assert.True(options.Animated);
        Assert.Null(options.Padding);
        Assert.Null(options.MaxZoom);
    }

    [Fact]
    public void XFMapTileSources_HasCorrectUrls()
    {
        Assert.Contains("tile.openstreetmap.org", XFMapTileSources.OpenStreetMap);
        Assert.Contains("cartocdn.com", XFMapTileSources.CartoDbPositron);
        Assert.Contains("cartocdn.com", XFMapTileSources.CartoDbDarkMatter);
        Assert.Contains("OpenStreetMap", XFMapTileSources.OpenStreetMapAttribution);
        Assert.Contains("CARTO", XFMapTileSources.CartoDbAttribution);
    }

    [Fact]
    public void XFMapMarkerClickEventArgs_CreatesCorrectly()
    {
        var args = new XFMapMarkerClickEventArgs("marker-1", new XFMapCoordinate(10, 20));
        Assert.Equal("marker-1", args.MarkerId);
        Assert.Equal(10, args.Coordinate.Latitude);
        Assert.Equal(20, args.Coordinate.Longitude);
    }

    [Fact]
    public void XFMapMarkerDragEventArgs_CreatesCorrectly()
    {
        var args = new XFMapMarkerDragEventArgs("marker-1", new XFMapCoordinate(15, 25));
        Assert.Equal("marker-1", args.MarkerId);
        Assert.Equal(15, args.Coordinate.Latitude);
        Assert.Equal(25, args.Coordinate.Longitude);
    }
}
