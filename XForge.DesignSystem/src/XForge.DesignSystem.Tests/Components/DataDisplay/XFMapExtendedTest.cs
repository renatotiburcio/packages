using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Maps;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFMapExtendedTest : BunitContext
{
    public XFMapExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFMap> RenderMap(
        Action<ComponentParameterCollectionBuilder<XFMap>>? configure = null)
    {
        return Render<XFMap>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_Container()
    {
        var cut = RenderMap();
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_RoleApplication()
    {
        var cut = RenderMap();
        Assert.Contains("role=\"application\"", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_DataMapId()
    {
        var cut = RenderMap();
        Assert.Contains("data-map-id", cut.Markup);
    }

    [Fact]
    public void Extended_CustomCenter_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Center, new XFMapCoordinate(-23.5505, -46.6333)));
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_CustomZoom_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Zoom, 15));
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_InteractiveFalse_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Interactive, false));
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_ShowZoomControlFalse_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.ShowZoomControl, false));
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_ShowAttributionFalse_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.ShowAttribution, false));
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_MinZoom_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.MinZoom, 5));
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_MaxZoom_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.MaxZoom, 18));
        Assert.Contains("xf-map", cut.Markup);
    }

    [Fact]
    public void Extended_CustomAriaLabel_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.AriaLabel, "Mapa de unidades"));
        Assert.Contains("aria-label=\"Mapa de unidades\"", cut.Markup);
    }

    [Fact]
    public void Extended_DefaultAriaLabel_Renders()
    {
        var cut = RenderMap();
        Assert.Contains("aria-label=\"Mapa interativo\"", cut.Markup);
    }

    [Fact]
    public void Extended_CustomHeight_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Height, "600px"));
        Assert.Contains("height:600px", cut.Markup);
    }

    [Fact]
    public void Extended_CustomWidth_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Width, "50%"));
        Assert.Contains("width:50%", cut.Markup);
    }

    [Fact]
    public void Extended_CustomId_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Id, "branches-map"));
        Assert.Contains("id=\"branches-map\"", cut.Markup);
    }

    [Fact]
    public void Extended_CustomClass_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Class, "my-map"));
        Assert.Contains("my-map", cut.Markup);
    }

    [Fact]
    public void Extended_CustomStyle_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.Style, "border: 2px solid blue"));
        Assert.Contains("border: 2px solid blue", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.AdditionalAttributes,
            new Dictionary<string, object> { { "data-testid", "map" } }));
        Assert.Contains("data-testid=\"map\"", cut.Markup);
    }

    [Fact]
    public void Extended_ChildContent_Renders()
    {
        var cut = RenderMap(p => p.Add(x => x.ChildContent, (RenderFragment)(b =>
        {
            b.OpenElement(0, "div");
            b.AddContent(1, "Map overlay");
            b.CloseElement();
        })));
        Assert.Contains("Map overlay", cut.Markup);
    }

    [Fact]
    public void Extended_OnReady_CallbackRegistered()
    {
        var ready = false;
        var cut = RenderMap(p => p.Add(
            x => x.OnReady,
            EventCallback.Factory.Create(this, () => ready = true)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_OnMapClick_CallbackRegistered()
    {
        XFMapEventArgs? args = null;
        var cut = RenderMap(p => p.Add(
            x => x.OnMapClick,
            EventCallback.Factory.Create<XFMapEventArgs>(this, a => args = a)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_OnMapDoubleClick_CallbackRegistered()
    {
        XFMapEventArgs? args = null;
        var cut = RenderMap(p => p.Add(
            x => x.OnMapDoubleClick,
            EventCallback.Factory.Create<XFMapEventArgs>(this, a => args = a)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_OnMapContextMenu_CallbackRegistered()
    {
        XFMapEventArgs? args = null;
        var cut = RenderMap(p => p.Add(
            x => x.OnMapContextMenu,
            EventCallback.Factory.Create<XFMapEventArgs>(this, a => args = a)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_OnViewChanged_CallbackRegistered()
    {
        XFMapView? view = null;
        var cut = RenderMap(p => p.Add(
            x => x.OnViewChanged,
            EventCallback.Factory.Create<XFMapView>(this, v => view = v)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_CenterTwoWayBinding_Renders()
    {
        XFMapCoordinate? center = null;
        var cut = RenderMap(p => p.Add(
            x => x.CenterChanged,
            EventCallback.Factory.Create<XFMapCoordinate>(this, c => center = c)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_ZoomTwoWayBinding_Renders()
    {
        double? zoom = null;
        var cut = RenderMap(p => p.Add(
            x => x.ZoomChanged,
            EventCallback.Factory.Create<double>(this, z => zoom = z)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_XFMapCoordinate_Equality()
    {
        var a = new XFMapCoordinate(10, 20);
        var b = new XFMapCoordinate(10, 20);
        var c = new XFMapCoordinate(10, 30);
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }

    [Fact]
    public void Extended_XFMapCoordinate_ToString()
    {
        var coord = new XFMapCoordinate(-23.5505, -46.6333);
        var result = coord.ToString();
        Assert.Contains("-23", result);
        Assert.Contains("-46", result);
    }

    [Fact]
    public void Extended_XFMapBounds_CreatesCorrectly()
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
    public void Extended_XFMapView_WithBounds()
    {
        var view = new XFMapView(
            new XFMapCoordinate(10, 20), 15,
            new XFMapBounds(new XFMapCoordinate(-10, -20), new XFMapCoordinate(10, 20)));
        Assert.Equal(10, view.Center.Latitude);
        Assert.Equal(15, view.Zoom);
        Assert.NotNull(view.Bounds);
    }

    [Fact]
    public void Extended_XFMapEventArgs_CreatesCorrectly()
    {
        var args = new XFMapEventArgs(new XFMapCoordinate(10, 20), 100, 200);
        Assert.Equal(10, args.Coordinate.Latitude);
        Assert.Equal(20, args.Coordinate.Longitude);
        Assert.Equal(100, args.ClientX);
        Assert.Equal(200, args.ClientY);
    }

    [Fact]
    public void Extended_XFMapShapeStyle_CustomValues()
    {
        var style = new XFMapShapeStyle
        {
            StrokeColor = "#ff0000",
            StrokeOpacity = 0.8,
            StrokeWidth = 5,
            StrokeDashArray = "5,10",
            FillColor = "#00ff00",
            FillOpacity = 0.3
        };
        Assert.Equal("#ff0000", style.StrokeColor);
        Assert.Equal(0.8, style.StrokeOpacity);
        Assert.Equal(5, style.StrokeWidth);
        Assert.Equal("5,10", style.StrokeDashArray);
        Assert.Equal("#00ff00", style.FillColor);
        Assert.Equal(0.3, style.FillOpacity);
    }

    [Fact]
    public void Extended_XFMapMarkerIcon_FullConfig()
    {
        var icon = new XFMapMarkerIcon
        {
            Url = "https://example.com/marker.png",
            Size = (25, 41),
            Anchor = (12, 41),
            PopupAnchor = (0, -41)
        };
        Assert.Equal("https://example.com/marker.png", icon.Url);
        Assert.Equal((25, 41), icon.Size);
        Assert.Equal((12, 41), icon.Anchor);
        Assert.Equal((0, -41), icon.PopupAnchor);
    }

    [Fact]
    public void Extended_XFMapAnimationOptions_Custom()
    {
        var options = new XFMapAnimationOptions
        {
            Animated = false,
            Duration = 1.5,
            Easing = "ease-in-out"
        };
        Assert.False(options.Animated);
        Assert.Equal(1.5, options.Duration);
        Assert.Equal("ease-in-out", options.Easing);
    }

    [Fact]
    public void Extended_XFMapFitBoundsOptions_FullConfig()
    {
        var options = new XFMapFitBoundsOptions
        {
            Animated = false,
            Padding = (50, 50),
            MaxZoom = 16
        };
        Assert.False(options.Animated);
        Assert.Equal((50, 50), options.Padding);
        Assert.Equal(16, options.MaxZoom);
    }

    [Fact]
    public void Extended_XFMapTileSources_AllHaveUrls()
    {
        Assert.Contains("tile.openstreetmap.org", XFMapTileSources.OpenStreetMap);
        Assert.Contains("cartocdn.com", XFMapTileSources.CartoDbPositron);
        Assert.Contains("cartocdn.com", XFMapTileSources.CartoDbDarkMatter);
        Assert.Contains("OpenStreetMap", XFMapTileSources.OpenStreetMapAttribution);
        Assert.Contains("CARTO", XFMapTileSources.CartoDbAttribution);
    }

    [Fact]
    public void Extended_XFMapMarkerClickEventArgs_CreatesCorrectly()
    {
        var args = new XFMapMarkerClickEventArgs("marker-1", new XFMapCoordinate(10, 20));
        Assert.Equal("marker-1", args.MarkerId);
        Assert.Equal(10, args.Coordinate.Latitude);
    }

    [Fact]
    public void Extended_XFMapMarkerDragEventArgs_CreatesCorrectly()
    {
        var args = new XFMapMarkerDragEventArgs("marker-1", new XFMapCoordinate(15, 25));
        Assert.Equal("marker-1", args.MarkerId);
        Assert.Equal(15, args.Coordinate.Latitude);
    }

    [Fact]
    public void Extended_Parameter_Defaults_AreCorrect()
    {
        var cut = RenderMap();
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
    }
}
