using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Maps
{
    public partial class XFMapsAdvanced : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<MapMarker> Markers { get; set; } = new List<MapMarker>();
        [Parameter] public IReadOnlyList<MapLayer> Layers { get; set; } = new List<MapLayer>();
        [Parameter] public string Provider { get; set; } = "openstreetmap";
        [Parameter] public bool ShowHeatmap { get; set; }
        [Parameter] public bool DrawingTools { get; set; } = false;
        [Parameter] public int Width { get; set; } = 800;
        [Parameter] public int Height { get; set; } = 600;
        [Parameter] public EventCallback<MapMarker> OnMarkerClick { get; set; }

        // assinatura de render: wrapper fino — o estado interativo vive no componente
        // interno; aqui o render e funcao dos parametros
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Markers.Count, Layers.Count, Provider, ShowHeatmap, DrawingTools, Width, Height);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
