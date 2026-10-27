using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.Label
{
    /// <summary>
    /// Semantic HTML label that binds to an input via the <c>for</c> attribute.
    /// Supports text, child content, and a required indicator.
    /// </summary>
    public partial class XFInputLabel : XForgeComponentBase
    {
        /// <summary>
        /// The base CSS class(es) for this component.
        /// </summary>
        protected override string BaseClass => "xf-input-label";

        /// <summary>
        /// The label text displayed when <see cref="ChildContent"/> is not provided.
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// The <c>name</c> or <c>id</c> of the input element this label is associated with.
        /// Maps to the <c>for</c> attribute of the rendered <c>&lt;label&gt;</c>.
        /// </summary>
        [Parameter]
        public string? For { get; set; }

        /// <summary>
        /// Child content rendered inside the label. Takes precedence over <see cref="Text"/>.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// When <c>true</c>, appends a visible required indicator (<c>*</c>) after the label text.
        /// </summary>
        [Parameter]
        public bool Required { get; set; }

        /// <summary>
        /// ARIA label for accessibility. Applied as <c>aria-label</c> on the label element.
        /// </summary>
        [Parameter]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Computes the final CSS class list for the component's root element.
        /// </summary>
        protected override string GetComponentClass()
        {
            return GetClass(
                $"{BaseClass} inline-flex items-center gap-1 text-sm font-medium leading-5 text-text-primary cursor-pointer select-none");
        }
    }
}
