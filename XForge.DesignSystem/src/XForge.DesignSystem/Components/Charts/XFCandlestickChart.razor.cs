using System.Text;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Charts
{
    public partial class XFCandlestickChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<Candle> Candles { get; set; } = new List<Candle>();
        [Parameter] public int Width { get; set; } = 800;
        [Parameter] public int Height { get; set; } = 400;
        [Parameter] public bool ShowVolume { get; set; } = false;
        [Parameter] public EventCallback<Candle> OnCandleClick { get; set; }

        // assinatura de render: gráficos são caros de renderizar — pula render redundante quando o
        // pai re-renderiza sem mudar nada aqui (e evita re-invocar interop JS no AfterRender)
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Candles is null ? -1 : Candles.Count, Width, Height, ShowVolume, OnCandleClick);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string RenderSvg()
        {
            if (!Candles.Any()) return "";
            var min = Candles.Min(c => c.Low);
            var max = Candles.Max(c => c.High);
            var range = max - min;
            var sb = new StringBuilder();
            double cw = (double)Width / Candles.Count;
            for (int i = 0; i < Candles.Count; i++)
            {
                var c = Candles[i];
                bool up = c.Close >= c.Open;
                var color = up ? "#10b981" : "#ef4444";
                var yOpen = Height - (c.Open - min) / range * Height;
                var yClose = Height - (c.Close - min) / range * Height;
                var yHigh = Height - (c.High - min) / range * Height;
                var yLow = Height - (c.Low - min) / range * Height;
                var x = i * cw + cw / 2;
                sb.Append($"<line x1=\"{x:F1}\" y1=\"{yHigh:F1}\" x2=\"{x:F1}\" y2=\"{yLow:F1}\" stroke=\"{color}\"/>");
                sb.Append($"<rect x=\"{x - cw * 0.3:F1}\" y=\"{Math.Min(yOpen, yClose):F1}\" width=\"{cw * 0.6:F1}\" height=\"{Math.Abs(yClose - yOpen):F1}\" fill=\"{color}\"/>");
            }
            return sb.ToString();
        }
    }
}
