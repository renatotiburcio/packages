using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Navigation.Accordion
{
    public partial class XFAccordionItem : XForgeComponentBase, IDisposable
    {
        private ElementReference headerElement;
        private bool hasRenderedContent;
        private bool expanded;
        private bool previousExpandedParameter;

        protected override string BaseClass => "xf-accordion-item";

        [CascadingParameter] internal XFAccordion? Parent { get; set; }

        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public bool Expanded { get; set; }
        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }
        [Parameter] public EventCallback<XFAccordionItemChangeEventArgs> Change { get; set; }
        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Lazy { get; set; }
        [Parameter] public bool KeepAlive { get; set; } = true;
        [Parameter] public string? ExpandTitle { get; set; }
        [Parameter] public string? CollapseTitle { get; set; }
        [Parameter] public string ExpandTitleFormat { get; set; } = "Expandir {0}";
        [Parameter] public string CollapseTitleFormat { get; set; } = "Recolher {0}";

        internal bool CurrentExpanded => expanded;

        private string HeaderId => $"{Id}-header";
        private string PanelId => $"{Id}-panel";
        private string State => CurrentExpanded ? "open" : "closed";

        protected override void OnInitialized()
        {
            expanded = Expanded;
            previousExpandedParameter = Expanded;
            Parent?.Register(this);
        }

        protected override void OnParametersSet()
        {
            if (previousExpandedParameter != Expanded)
            {
                expanded = Expanded;
                previousExpandedParameter = Expanded;
            }

            if (expanded)
            {
                hasRenderedContent = true;
            }
            else if (!KeepAlive)
            {
                hasRenderedContent = false;
            }
        }

        public void Dispose()
        {
            Parent?.Unregister(this);
        }

        internal async Task SetExpandedFromParentAsync(bool value)
        {
            if (expanded == value)
            {
                return;
            }

            expanded = value;

            if (expanded)
            {
                hasRenderedContent = true;
            }
            else if (!KeepAlive)
            {
                hasRenderedContent = false;
            }

            if (ExpandedChanged.HasDelegate)
            {
                await ExpandedChanged.InvokeAsync(value);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(new XFAccordionItemChangeEventArgs(this, value, Parent?.GetIndex(this) ?? -1));
            }

            await InvokeAsync(StateHasChanged);
        }

        internal async Task FocusHeaderAsync()
        {
            await headerElement.FocusAsync();
        }

        private async Task ToggleAsync()
        {
            if (Parent is not null)
            {
                await Parent.ToggleItemAsync(this);
                return;
            }

            await SetExpandedFromParentAsync(!expanded);
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (Parent is null)
            {
                return;
            }

            if (args.Key == "ArrowDown")
            {
                await Parent.FocusRelativeAsync(this, 1);
            }
            else if (args.Key == "ArrowUp")
            {
                await Parent.FocusRelativeAsync(this, -1);
            }
            else if (args.Key == "Home")
            {
                await Parent.FocusFirstAsync();
            }
            else if (args.Key == "End")
            {
                await Parent.FocusLastAsync();
            }
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {ItemClass}");
        }

        private string ItemClass => Parent?.Variant switch
        {
            XFAccordionVariant.Ghost => "rounded-lg border border-transparent transition-colors duration-200 hover:bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]",
            XFAccordionVariant.Outline => "rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-200 data-[state=open]:border-[color-mix(in_oklab,var(--color-primary)_32%,var(--color-border))] data-[state=open]:ring-1 data-[state=open]:ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]",
            XFAccordionVariant.Soft => "rounded-lg border border-transparent bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] transition-all duration-200 data-[state=open]:bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] data-[state=open]:ring-1 data-[state=open]:ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]",
            _ => "rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-200 data-[state=open]:shadow-[0_10px_30px_rgb(15_23_42/.07)]"
        };

        private string HeaderClass => Join(
            "flex w-full items-center gap-3 rounded-lg text-left outline-none transition-all duration-200 ease-[cubic-bezier(0.4,0,0.2,1)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:text-text-muted disabled:opacity-70",
            Parent?.Variant == XFAccordionVariant.Ghost ? "hover:text-primary" : "hover:bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)]",
            Parent?.Size switch
            {
                XFSize.Sm => "px-3 py-2 text-sm",
                XFSize.Lg => "px-5 py-4 text-base",
                _ => "px-4 py-3 text-sm"
            });

        private string StartIconClass => Parent?.Size switch
        {
            XFSize.Sm => "flex h-7 w-7 shrink-0 items-center justify-center rounded-md bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary",
            XFSize.Lg => "flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary",
            _ => "flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary"
        };

        private string IconClass => Join(
            "inline-flex shrink-0 items-center justify-center rounded-md text-text-secondary transition-all duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transition-none",
            CurrentExpanded ? "bg-[color-mix(in_oklab,var(--color-primary)_9%,transparent)] text-primary" : "bg-surface-100",
            Parent?.Size == XFSize.Lg ? "h-8 w-8" : "h-7 w-7");

        private string ChevronIconClass => Join(
            "inline-flex h-3.5 w-3.5 items-center justify-center transition-transform duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transition-none",
            CurrentExpanded ? "rotate-180" : "rotate-0");

        private string PlusIconClass => "h-3.5 w-3.5";

        private string PlusVerticalClass => Join(
            "origin-center transition-transform duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transition-none",
            CurrentExpanded ? "scale-y-0" : "scale-y-100");

        private string PanelOuterClass => Join(
            "grid overflow-hidden motion-reduce:transition-none",
            GetAnimationOuterClass(),
            CurrentExpanded ? "grid-rows-[1fr]" : "grid-rows-[0fr]");

        private string PanelInnerClass => Join(
            "min-h-0 overflow-hidden text-text-secondary",
            GetContentPaddingClass(),
            GetAnimationInnerClass(),
            Parent?.StaggerContent == true ? "[&>*]:transition-all [&>*]:duration-300 [&>*:nth-child(1)]:delay-75 [&>*:nth-child(2)]:delay-100 [&>*:nth-child(3)]:delay-150" : null);

        private string GetAnimationOuterClass()
        {
            return Parent?.Animation switch
            {
                XFAccordionAnimation.None => "",
                XFAccordionAnimation.ScaleFade => "transition-[grid-template-rows] duration-[250ms] ease-[cubic-bezier(0.16,1,0.3,1)]",
                XFAccordionAnimation.Height => "transition-[grid-template-rows] duration-300 ease-[cubic-bezier(0.4,0,0.2,1)]",
                _ => "transition-[grid-template-rows] duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]"
            };
        }

        private string GetAnimationInnerClass()
        {
            return Parent?.Animation switch
            {
                XFAccordionAnimation.None => "",
                XFAccordionAnimation.Height => "",
                XFAccordionAnimation.ScaleFade => CurrentExpanded
                    ? "origin-top scale-y-100 opacity-100 transition-all duration-[250ms] ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transform-none motion-reduce:transition-none"
                    : "origin-top scale-y-[.98] opacity-0 transition-all duration-200 ease-[cubic-bezier(0.4,0,0.2,1)] motion-reduce:transform-none motion-reduce:transition-none",
                XFAccordionAnimation.SlideFade => CurrentExpanded
                    ? "translate-y-0 opacity-100 transition-all duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transform-none motion-reduce:transition-none"
                    : "-translate-y-1 opacity-0 transition-all duration-200 ease-[cubic-bezier(0.4,0,0.2,1)] motion-reduce:transform-none motion-reduce:transition-none",
                _ => CurrentExpanded
                    ? "translate-y-0 opacity-100 transition-all duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transform-none motion-reduce:transition-none"
                    : "-translate-y-1 opacity-0 transition-all duration-200 ease-[cubic-bezier(0.4,0,0.2,1)] motion-reduce:transform-none motion-reduce:transition-none"
            };
        }

        private string GetContentPaddingClass()
        {
            if (!CurrentExpanded && !KeepAlive)
            {
                return "";
            }

            return Parent?.Size switch
            {
                XFSize.Sm => "px-3 pb-3 pt-0",
                XFSize.Lg => "px-5 pb-5 pt-0",
                _ => "px-4 pb-4 pt-0"
            };
        }

        private string GetToggleTitle()
        {
            if (CurrentExpanded)
            {
                return CollapseTitle ?? string.Format(CultureInfo.CurrentCulture, CollapseTitleFormat, Title);
            }

            return ExpandTitle ?? string.Format(CultureInfo.CurrentCulture, ExpandTitleFormat, Title);
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
