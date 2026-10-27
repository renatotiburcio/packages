using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Utilities.LiveRegion
{
    /// <summary>
    /// Accessibility live region component. Renders a visually hidden <c>&lt;div&gt;</c>
    /// with <c>aria-live</c> and <c>role="status"</c> or <c>role="alert"</c>
    /// to announce dynamic content updates to screen readers.
    /// </summary>
    public partial class XFLiveRegion : XForgeComponentBase
    {
        /// <summary>
        /// The base CSS class(es) for this component.
        /// </summary>
        protected override string BaseClass => "xf-live-region";

        /// <summary>
        /// The text content announced to screen readers.
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// When <c>true</c>, uses <c>aria-live="assertive"</c> and <c>role="alert"</c>
        /// for urgent announcements. Defaults to <c>false</c> (polite).
        /// </summary>
        [Parameter]
        public bool Assertive { get; set; }

        /// <summary>
        /// Child content rendered inside the live region. Takes precedence over <see cref="Text"/>.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Gets the <c>aria-live</c> value based on the <see cref="Assertive"/> parameter.
        /// </summary>
        private string AriaLive => Assertive ? "assertive" : "polite";

        /// <summary>
        /// Gets the <c>role</c> attribute value based on the <see cref="Assertive"/> parameter.
        /// </summary>
        private string Role => Assertive ? "alert" : "status";

        /// <summary>
        /// Computes the final CSS class list for the component's root element.
        /// </summary>
        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} absolute h-px w-px overflow-hidden whitespace-nowrap border-0 p-0 clip-rect-0 clip inset-0 m-0");
        }
    }
}
