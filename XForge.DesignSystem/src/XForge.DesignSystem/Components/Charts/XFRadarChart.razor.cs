using System.Text;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Charts
{
    public partial class XFRadarChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<string> Axes { get; set; } = new List<string>();
        [Parameter] public IReadOnlyList<RadarSeries> Series { get; set; } = new List<RadarSeries>();
        [Parameter] public double Max { get; set; } = 1.0;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Axes.Count, Series.Count, Max);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string RenderSvg()
        {
            var sb = new StringBuilder();
            int n = Axes.Count;
            for (int level = 1; level <= 4; level++)
            {
                var pts = new List<string>();
                for (int i = 0; i < n; i++)
                {
                    var angle = i * 2 * Math.PI / n - Math.PI / 2;
                    var r = (double)level / 4;
                    pts.Add($"{Math.Cos(angle) * r:F3},{Math.Sin(angle) * r:F3}");
                }
                sb.Append($"<polygon points=\"{string.Join(" ", pts)}\" fill=\"none\" stroke=\"#ccc\"/>");
            }
            foreach (var s in Series)
            {
                var pts = new List<string>();
                for (int i = 0; i < n; i++)
                {
                    var angle = i * 2 * Math.PI / n - Math.PI / 2;
                    var r = s.Values[i] / Max;
                    pts.Add($"{Math.Cos(angle) * r:F3},{Math.Sin(angle) * r:F3}");
                }
                sb.Append($"<polygon points=\"{string.Join(" ", pts)}\" fill=\"{s.Color}\" fill-opacity=\"0.3\" stroke=\"{s.Color}\"/>");
            }
            return sb.ToString();
        }
    }
}
