using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Navigation.Link
{
    /// <summary>
    /// Styled navigation link component. Renders an <c>&lt;a&gt;</c> element
    /// with Tailwind-compatible classes and optional icon support.
    /// </summary>
    public partial class XFLink : XForgeComponentBase
    {
        /// <summary>
        /// The base CSS class(es) for this component.
        /// </summary>
        protected override string BaseClass => "xf-link";

        /// <summary>
        /// The URL the link navigates to. Maps to the <c>href</c> attribute.
        /// </summary>
        [Parameter]
        public string? Href { get; set; }

        /// <summary>
        /// The link text displayed when <see cref="ChildContent"/> is not provided.
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// The target attribute for the link (e.g., <c>_blank</c>).
        /// </summary>
        [Parameter]
        public string? Target { get; set; }

        /// <summary>
        /// Icon rendered before the link text. Render a component (e.g., SVG) inside this fragment.
        /// </summary>
        [Parameter]
        public RenderFragment? Icon { get; set; }

        /// <summary>
        /// Child content rendered inside the link. Takes precedence over <see cref="Text"/>.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// ARIA label for accessibility. Applied as <c>aria-label</c> on the <c>&lt;a&gt;</c> element.
        /// </summary>
        [Parameter]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Computes the final CSS class list for the component's root element.
        /// </summary>
        protected override string GetComponentClass()
        {
            var baseClasses = Disabled
                ? $"{BaseClass} inline-flex items-center gap-1.5 text-sm font-medium no-underline cursor-not-allowed opacity-60 pointer-events-none text-text-muted"
                : $"{BaseClass} inline-flex items-center gap-1.5 text-sm font-medium no-underline text-primary outline-none transition-colors duration-150 hover:text-primary-hover focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_40%,transparent)] focus-visible:ring-offset-2";

            return GetClass(baseClasses);
        }
    }
}
