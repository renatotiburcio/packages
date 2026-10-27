using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Forms.SelectBar
{
    /// <summary>
    /// Segmented button group component. Renders a list of selectable items
    /// in a horizontal or vertical layout with single-selection behavior.
    /// </summary>
    public partial class XFSelectBar : XForgeComponentBase
    {
        /// <summary>
        /// The base CSS class(es) for this component.
        /// </summary>
        protected override string BaseClass => "xf-select-bar";

        /// <summary>
        /// The currently selected value.
        /// </summary>
        [Parameter]
        public string? Value { get; set; }

        /// <summary>
        /// Callback raised when the selected value changes.
        /// </summary>
        [Parameter]
        public EventCallback<string?> ValueChanged { get; set; }

        /// <summary>
        /// The list of items to display in the select bar.
        /// </summary>
        [Parameter]
        public IReadOnlyList<XFSelectBarItem>? Items { get; set; }

        /// <summary>
        /// The layout orientation of the select bar.
        /// </summary>
        [Parameter]
        public XFOrientation Orientation { get; set; } = XFOrientation.Horizontal;

        /// <summary>
        /// The size of the select bar buttons.
        /// </summary>
        [Parameter]
        public XFSize Size { get; set; } = XFSize.Md;

        /// <summary>
        /// ARIA label for accessibility. Applied as <c>aria-label</c> on the root element.
        /// </summary>
        [Parameter]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Computes the final CSS class list for the component's root element.
        /// </summary>
        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {GetLayoutClass()} {GetSizeClass()}");
        }

        /// <summary>
        /// Gets the Tailwind class for the current layout orientation.
        /// </summary>
        private string GetLayoutClass()
        {
            return Orientation == XFOrientation.Vertical
                ? "flex flex-col"
                : "inline-flex flex-row";
        }

        /// <summary>
        /// Gets the Tailwind class for the current size.
        /// </summary>
        private string GetSizeClass()
        {
            return Size switch
            {
                XFSize.Sm => "gap-0.5 rounded-lg p-0.5",
                XFSize.Lg => "gap-1 rounded-xl p-1",
                _ => "gap-0.5 rounded-lg p-0.5"
            };
        }

        /// <summary>
        /// Gets the Tailwind classes for an individual item button.
        /// </summary>
        protected string GetItemClass(string? itemValue, bool isDisabled)
        {
            var isSelected = string.Equals(Value, itemValue, StringComparison.Ordinal);
            var sizeClass = Size switch
            {
                XFSize.Sm => "h-8 px-2.5 text-xs",
                XFSize.Lg => "h-11 px-5 text-base",
                _ => "h-9 px-3.5 text-sm"
            };
            var selectedClass = isSelected
                ? "bg-primary text-primary-contrast shadow-sm"
                : "bg-transparent text-text-secondary hover:bg-surface-100";
            var disabledClass = isDisabled ? "opacity-50 cursor-not-allowed pointer-events-none" : "";
            return $"inline-flex items-center justify-center gap-1.5 rounded-md border border-transparent font-medium transition-all duration-150 ease-out outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_40%,transparent)] focus-visible:ring-offset-2 select-none {sizeClass} {selectedClass} {disabledClass}";
        }

        /// <summary>
        /// Handles item click events.
        /// </summary>
        protected async Task HandleItemClickAsync(string? itemValue, bool isDisabled)
        {
            if (isDisabled) return;

            Value = itemValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }
    }
}
