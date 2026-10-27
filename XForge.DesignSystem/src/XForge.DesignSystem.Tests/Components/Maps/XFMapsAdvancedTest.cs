using Bunit;
using Xunit;
using XForge.Components.Maps;

namespace XForge.DesignSystem.Tests.Components.Maps;

public class XFMapsAdvancedTest : Bunit.BunitContext
{
    public XFMapsAdvancedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFMapsAdvanced>();
        Assert.Contains("xf-maps-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithMarkers_ShowsMarkers()
    {
        var markers = new List<XFMapsAdvanced.MapMarker>
        {
            new() { Title = "Office", Latitude = 50.0, Longitude = 10.0 }
        };

        var cut = Render<XFMapsAdvanced>(p => p.Add(x => x.Markers, markers));
        Assert.Contains("data-markers=\"1\"", cut.Markup);
        Assert.Contains("Office", cut.Markup);
    }

    [Fact]
    public void Render_WithLayers()
    {
        var layers = new List<XFMapsAdvanced.MapLayer>
        {
            new() { Name = "Heatmap", Type = "heatmap" }
        };

        var cut = Render<XFMapsAdvanced>(p => p.Add(x => x.Layers, layers));
        Assert.Contains("data-layers=\"1\"", cut.Markup);
        Assert.Contains("Heatmap", cut.Markup);
    }
}
