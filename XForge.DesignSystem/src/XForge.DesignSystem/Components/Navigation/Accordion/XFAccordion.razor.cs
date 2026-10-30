using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Navigation.Accordion
{
    public partial class XFAccordion : XForgeComponentBase
    {
        private readonly List<XFAccordionItem> items = new();

        protected override string BaseClass => "xf-accordion";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Multiple { get; set; }
        [Parameter] public bool Collapsible { get; set; } = true;
        [Parameter] public XFAccordionVariant Variant { get; set; } = XFAccordionVariant.Surface;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFAccordionAnimation Animation { get; set; } = XFAccordionAnimation.HeightFade;
        [Parameter] public XFAccordionIconMode IconMode { get; set; } = XFAccordionIconMode.Chevron;
        [Parameter] public bool StaggerContent { get; set; }
        [Parameter] public EventCallback<XFAccordionItemChangeEventArgs> ItemChanged { get; set; }

        /// <summary>Fires when an accordion item is expanded. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback<XFAccordionItemChangeEventArgs> OnExpanded { get; set; }

        /// <summary>Fires when an accordion item is collapsed. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback<XFAccordionItemChangeEventArgs> OnCollapsed { get; set; }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {VariantClass} {SizeClass}");
        }

        internal void Register(XFAccordionItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
            }
        }

        internal void Unregister(XFAccordionItem item)
        {
            items.Remove(item);
        }

        internal int GetIndex(XFAccordionItem item) => items.IndexOf(item);

        internal bool IsFirst(XFAccordionItem item) => GetIndex(item) == 0;

        internal bool IsLast(XFAccordionItem item) => GetIndex(item) == items.Count - 1;

        internal async Task ToggleItemAsync(XFAccordionItem item)
        {
            if (item.Disabled)
            {
                return;
            }

            var nextExpanded = !item.CurrentExpanded;

            if (!nextExpanded && !Collapsible && !Multiple)
            {
                return;
            }

            if (nextExpanded && !Multiple)
            {
                foreach (var otherItem in items.Where(entry => !ReferenceEquals(entry, item) && entry.CurrentExpanded))
                {
                    await SetItemExpandedAsync(otherItem, false);
                }
            }

            await SetItemExpandedAsync(item, nextExpanded);
        }

        internal async Task FocusRelativeAsync(XFAccordionItem item, int direction)
        {
            var enabled = items.Where(entry => !entry.Disabled).ToArray();
            if (enabled.Length == 0)
            {
                return;
            }

            var current = Array.IndexOf(enabled, item);
            var next = enabled[(current + direction + enabled.Length) % enabled.Length];
            await next.FocusHeaderAsync();
        }

        internal async Task FocusFirstAsync()
        {
            var first = items.FirstOrDefault(entry => !entry.Disabled);
            if (first is not null)
            {
                await first.FocusHeaderAsync();
            }
        }

        internal async Task FocusLastAsync()
        {
            var last = items.LastOrDefault(entry => !entry.Disabled);
            if (last is not null)
            {
                await last.FocusHeaderAsync();
            }
        }

        private async Task SetItemExpandedAsync(XFAccordionItem item, bool expanded)
        {
            await item.SetExpandedFromParentAsync(expanded);

            var args = new XFAccordionItemChangeEventArgs(item, expanded, GetIndex(item));

            if (ItemChanged.HasDelegate)
            {
                await ItemChanged.InvokeAsync(args);
            }

            if (expanded && OnExpanded.HasDelegate)
            {
                await OnExpanded.InvokeAsync(args);
            }
            else if (!expanded && OnCollapsed.HasDelegate)
            {
                await OnCollapsed.InvokeAsync(args);
            }
        }

        private string VariantClass => Variant switch
        {
            XFAccordionVariant.Outline => "space-y-2",
            XFAccordionVariant.Ghost => "space-y-1",
            XFAccordionVariant.Soft => "space-y-2",
            _ => "space-y-2"
        };

        private string SizeClass => Size switch
        {
            XFSize.Sm => "text-sm",
            XFSize.Lg => "text-base",
            _ => "text-sm"
        };
    }
}
