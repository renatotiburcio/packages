using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.Skeleton
{
    public partial class XFSkeletonGroup : XForgeComponentBase
    {
        protected override string BaseClass => "xf-skeleton-group";

        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Decorative { get; set; } = true;
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string DefaultAriaLabel { get; set; } = "Carregando conteudo";
        [Parameter] public bool Inline { get; set; }
        [Parameter] public string GapClass { get; set; } = "gap-3";
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Visible, Decorative, AriaLabel, DefaultAriaLabel, Inline, GapClass, ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string? Role => Decorative ? null : "status";

        private string? AriaHidden => Decorative ? "true" : null;

        private string? ResolvedAriaLabel => Decorative ? null : AriaLabel ?? DefaultAriaLabel;

        protected override string GetComponentClass()
        {
            var layout = Inline ? "flex items-center" : "grid";
            return GetClass($"{BaseClass} {layout} {GapClass}");
        }
    }
}
