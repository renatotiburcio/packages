using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.DataGrid
{
    public partial class XFTreeGrid : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<TreeNode> Nodes { get; set; } = new List<TreeNode>();
        [Parameter] public bool ShowCheckboxes { get; set; }
        [Parameter] public EventCallback<TreeNode> OnNodeClick { get; set; }

        // assinatura de render: wrapper fino — o estado interativo vive no componente
        // interno; aqui o render e funcao dos parametros
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Nodes.Count, ShowCheckboxes);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private RenderFragment RenderTree(IReadOnlyList<TreeNode> nodes) => __builder =>
        {
            foreach (var n in nodes)
            {
                __builder.OpenElement(0, "li");
                __builder.OpenElement(1, "span");
                __builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, () => OnNodeClick.InvokeAsync(n)));
                __builder.AddContent(3, n.Label);
                __builder.CloseElement();
                if (n.Children.Any())
                {
                    __builder.OpenElement(4, "ul");
                    __builder.AddContent(5, RenderTree(n.Children));
                    __builder.CloseElement();
                }
                __builder.CloseElement();
            }
        };
    }
}
