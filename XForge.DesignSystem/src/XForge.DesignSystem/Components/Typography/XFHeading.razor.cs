using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Typography
{
    /// <summary>
    /// Heading wrapper component that renders <c>&lt;h1&gt;</c> through <c>&lt;h6&gt;</c>
    /// based on the <see cref="Level"/> parameter, with Tailwind-compatible classes.
    /// </summary>
    public partial class XFHeading : XForgeComponentBase
    {
        /// <summary>
        /// The base CSS class(es) for this component.
        /// </summary>
        protected override string BaseClass => "xf-heading";

        /// <summary>
        /// The heading level (1-6). Determines which HTML heading element is rendered.
        /// </summary>
        [Parameter]
        public int Level { get; set; } = 1;

        /// <summary>
        /// The heading text displayed when <see cref="ChildContent"/> is not provided.
        /// </summary>
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// Child content rendered inside the heading. Takes precedence over <see cref="Text"/>.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// ARIA label for accessibility. Applied as <c>aria-label</c> on the heading element.
        /// </summary>
        [Parameter]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Gets the HTML tag name based on the heading level.
        /// </summary>
        protected string TagName => $"h{Math.Clamp(Level, 1, 6)}";

        /// <summary>
        /// Computes the final CSS class list for the component's root element.
        /// </summary>
        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {GetLevelClass()}");
        }

        /// <summary>
        /// Gets the appropriate Tailwind class for the heading level.
        /// </summary>
        private string GetLevelClass()
        {
            return Level switch
            {
                1 => "text-4xl font-bold leading-tight text-text-primary",
                2 => "text-3xl font-bold leading-tight text-text-primary",
                3 => "text-2xl font-semibold leading-tight text-text-primary",
                4 => "text-xl font-semibold leading-tight text-text-primary",
                5 => "text-lg font-semibold leading-snug text-text-primary",
                6 => "text-base font-semibold leading-snug text-text-primary",
                _ => "text-4xl font-bold leading-tight text-text-primary"
            };
        }
    }
}
