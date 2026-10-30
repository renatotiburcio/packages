using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Image
{
    public partial class XFFigure : XForgeComponentBase
    {
        protected override string BaseClass => "xf-figure";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? CaptionContent { get; set; }
        [Parameter] public string? Caption { get; set; }
        [Parameter] public string? Credit { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Framed { get; set; }
        [Parameter] public XFImageRadius Radius { get; set; } = XFImageRadius.Lg;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent, CaptionContent, Caption, Credit, Visible, Framed, Radius);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private bool HasCaption => CaptionContent is not null || !string.IsNullOrWhiteSpace(Caption) || !string.IsNullOrWhiteSpace(Credit);

        protected override string GetComponentClass()
        {
            var frame = Framed ? "bg-bg-surface p-2 shadow-sm ring-1 ring-border-light" : "";
            return GetClass($"{BaseClass} space-y-3 {RadiusClass} {frame}");
        }

        private string CaptionClass => "space-y-1 text-sm leading-6 text-text-secondary";

        private string RadiusClass => Radius switch
        {
            XFImageRadius.None => "rounded-none",
            XFImageRadius.Sm => "rounded-md",
            XFImageRadius.Md => "rounded-lg",
            XFImageRadius.Xl => "rounded-2xl",
            XFImageRadius.Full => "rounded-full",
            _ => "rounded-xl"
        };
    }
}
