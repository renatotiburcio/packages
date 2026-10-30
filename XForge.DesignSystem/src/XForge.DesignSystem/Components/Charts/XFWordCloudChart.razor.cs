using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Charts
{
    public partial class XFWordCloudChart : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<WordCloudItem> Words { get; set; } = new List<WordCloudItem>();
        [Parameter] public int MaxFontSize { get; set; } = 48;
        [Parameter] public int Width { get; set; } = 600;
        [Parameter] public int Height { get; set; } = 400;
        [Parameter] public EventCallback<WordCloudItem> OnWordClick { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Words.Count, MaxFontSize, Width, Height, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
