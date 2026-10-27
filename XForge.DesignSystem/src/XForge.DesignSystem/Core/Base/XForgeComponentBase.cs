using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace XForge.DesignSystem.Core.Base
{
    /// <summary>
    /// Base class for all XForge Design System components.
    /// Provides common parameters (Id, Class, Style, Disabled), AdditionalAttributes passthrough,
    /// and CSS class composition via <see cref="GetClass"/> and <see cref="GetComponentClass"/>.
    /// </summary>
    public abstract class XForgeComponentBase : ComponentBase
    {
        /// <summary>
        /// Additional HTML attributes to render on the root element.
        /// Captures all unmatched parameters for passthrough to the underlying HTML.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? AdditionalAttributes { get; set; }

        /// <summary>
        /// Unique HTML element identifier. Auto-generated if not provided.
        /// </summary>
        [Parameter]
        public string Id { get; set; } = $"xf-{System.Guid.NewGuid().ToString("N")[..8]}";

        /// <summary>
        /// Additional CSS classes to append to the component's computed class list.
        /// </summary>
        [Parameter]
        public string? Class { get; set; }

        /// <summary>
        /// Inline CSS styles to apply to the root element.
        /// </summary>
        [Parameter]
        public string? Style { get; set; }

        /// <summary>
        /// Whether the component is disabled. Disabled components typically have reduced opacity
        /// and do not respond to user interaction.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// The base CSS class(es) for this component. Override in subclasses to set the component identity class.
        /// </summary>
        protected virtual string? BaseClass => null;

        /// <summary>
        /// Appends the consumer-provided <see cref="Class"/> to the given Tailwind classes.
        /// </summary>
        protected string GetClass(string tailwindClasses)
        {
            if (string.IsNullOrWhiteSpace(Class))
                return tailwindClasses;
            return $"{tailwindClasses} {Class}";
        }

        /// <summary>
        /// Computes the final CSS class list for the component's root element.
        /// Combines <see cref="BaseClass"/> with <see cref="Class"/> via <see cref="GetClass"/>.
        /// Override for custom class composition logic.
        /// </summary>
        protected virtual string GetComponentClass()
        {
            if (string.IsNullOrWhiteSpace(BaseClass))
                return GetClass(string.Empty);
            return GetClass(BaseClass);
        }
    }
}
