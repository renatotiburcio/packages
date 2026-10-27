using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.SelectBar
{
    /// <summary>
    /// Represents a single item within an <see cref="XFSelectBar"/>.
    /// Defines the text, value, icon, and disabled state for a segmented button.
    /// </summary>
    public partial class XFSelectBarItem : XForgeComponentBase
    {
        /// <summary>
        /// The base CSS class(es) for this component.
        /// </summary>
        protected override string BaseClass => "xf-select-bar-item";

        /// <summary>
        /// The display text for this item.
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// The value associated with this item. Used for two-way binding in <see cref="XFSelectBar"/>.
        /// </summary>
        [Parameter]
        public string? Value { get; set; }

        /// <summary>
        /// Icon rendered before the item text. Render a component (e.g., SVG) inside this fragment.
        /// </summary>
        [Parameter]
        public RenderFragment? Icon { get; set; }

        /// <summary>
        /// When <c>true</c>, the item is disabled and cannot be selected.
        /// Inherits from <see cref="XForgeComponentBase.Disabled"/>.
        /// </summary>
    }
}
