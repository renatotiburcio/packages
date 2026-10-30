using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.Select
{
    /// <summary>
    /// Groups related options within an XFSelect.
    /// Must be used as a direct child of XFSelect.
    /// </summary>
    public partial class XFSelectGroup<TValue>
    {
        /// <summary>The group label displayed as the heading.</summary>
        [Parameter] public string? Label { get; set; }

        /// <summary>Disables all options within this group.</summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>The options belonging to this group.</summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Label, Disabled, ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    }
}
