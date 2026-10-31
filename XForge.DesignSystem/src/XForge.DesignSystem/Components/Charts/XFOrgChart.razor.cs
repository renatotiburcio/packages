using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Charts
{
    public partial class XFOrgChart : XForgeComponentBase
    {
        [Parameter] public OrgNode? Root { get; set; }
        [Parameter] public EventCallback<OrgNode> OnNodeClick { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Root, OnNodeClick);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private RenderFragment RenderNode(OrgNode? n) => __builder =>
        {
            if (n == null) return;
            __builder.OpenElement(0, "li");
            __builder.OpenElement(1, "div");
            __builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => OnNodeClick.InvokeAsync(n)));
            __builder.AddContent(3, $"{n.Name} - {n.Title}");
            __builder.CloseElement();
            if (n.Children.Any())
            {
                __builder.OpenElement(4, "ul");
                foreach (var c in n.Children)
                {
                    __builder.OpenElement(5, "li");
                    __builder.AddContent(6, RenderNode(c));
                    __builder.CloseElement();
                }
                __builder.CloseElement();
            }
            __builder.CloseElement();
        };
    }
}
