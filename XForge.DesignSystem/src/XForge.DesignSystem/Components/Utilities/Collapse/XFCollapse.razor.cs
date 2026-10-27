using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Utilities.Collapse
{
    public partial class XFCollapse : XForgeComponentBase
    {
        private ElementReference headerElement;
        private bool internalExpanded;
        private bool hasRenderedContent;
        private bool lastExpandedParam;

        protected override string BaseClass => "xf-collapse";

        // ── State ──────────────────────────────────────────────────────
        [Parameter] public bool Expanded { get; set; }
        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>Fires when the collapse toggles. D-001 compliant On{Action} convention.</summary>
        [Parameter] public EventCallback<bool> OnToggle { get; set; }

        // ── Content ────────────────────────────────────────────────────
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // ── Behavior ───────────────────────────────────────────────────
        [Parameter] public bool Lazy { get; set; }
        [Parameter] public bool KeepAlive { get; set; } = true;
        [Parameter] public XFCollapseAnimation Animation { get; set; } = XFCollapseAnimation.HeightFade;

        // ── Visual ─────────────────────────────────────────────────────
        [Parameter] public XFCollapseVariant Variant { get; set; } = XFCollapseVariant.Plain;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFCollapseIconMode IconMode { get; set; } = XFCollapseIconMode.Chevron;

        // ── Accessibility ──────────────────────────────────────────────
        [Parameter] public string? ExpandTitle { get; set; }
        [Parameter] public string? CollapseTitle { get; set; }
        [Parameter] public string ExpandTitleFormat { get; set; } = "Expand {0}";
        [Parameter] public string CollapseTitleFormat { get; set; } = "Collapse {0}";
        [Parameter] public string? AriaLabel { get; set; }

        // ── Computed ───────────────────────────────────────────────────
        private string HeaderId => $"{Id}-header";
        private string PanelId => $"{Id}-panel";
        private string StateName => internalExpanded ? "open" : "closed";
        private bool ShouldRenderContent => !Lazy || internalExpanded || (KeepAlive && hasRenderedContent);

        private bool? HiddenAttribute => Animation == XFCollapseAnimation.None && !internalExpanded ? true : null;

        // ── Lifecycle ──────────────────────────────────────────────────
        protected override void OnParametersSet()
        {
            if (Expanded != lastExpandedParam)
            {
                internalExpanded = Expanded;
                lastExpandedParam = Expanded;
            }

            if (internalExpanded)
            {
                hasRenderedContent = true;
            }
            else if (!KeepAlive)
            {
                hasRenderedContent = false;
            }
        }

        // ── Actions ────────────────────────────────────────────────────
        public async Task ToggleAsync()
        {
            if (Disabled)
            {
                return;
            }

            internalExpanded = !internalExpanded;

            if (internalExpanded)
            {
                hasRenderedContent = true;
            }
            else if (!KeepAlive)
            {
                hasRenderedContent = false;
            }

            if (ExpandedChanged.HasDelegate)
            {
                await ExpandedChanged.InvokeAsync(internalExpanded);
            }

            if (OnToggle.HasDelegate)
            {
                await OnToggle.InvokeAsync(internalExpanded);
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task HandleHeaderClickAsync()
        {
            await ToggleAsync();
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Enter" || args.Key == " ")
            {
                await ToggleAsync();
            }
        }

        // ── CSS Classes ────────────────────────────────────────────────
        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {VariantOuterClass}");
        }

        private string VariantOuterClass => Variant switch
        {
            XFCollapseVariant.Surface => "rounded-lg border border-border-light bg-surface-50/30 shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            XFCollapseVariant.Soft => "rounded-lg border border-transparent bg-primary/[0.04]",
            XFCollapseVariant.Outline => "rounded-lg border border-border-light bg-transparent",
            XFCollapseVariant.Ghost => "rounded-lg border border-transparent",
            _ => ""
        };

        private string HeaderClass => Join(
            "flex w-full items-center gap-3 rounded-lg text-left outline-none transition-all duration-200",
            "focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-primary/20",
            "disabled:cursor-not-allowed disabled:text-text-muted disabled:opacity-70",
            Variant == XFCollapseVariant.Ghost
                ? "hover:text-primary"
                : "hover:bg-primary/[0.04]",
            Size switch
            {
                XFSize.Sm => "px-3 py-2 min-h-[40px] text-sm",
                XFSize.Lg => "px-5 py-4 min-h-[48px] text-base",
                _ => "px-4 py-3 min-h-[44px] text-sm"
            });

        private string StartIconClass => Size switch
        {
            XFSize.Sm => "flex h-7 w-7 shrink-0 items-center justify-center rounded-md bg-primary/[0.08] text-primary",
            XFSize.Lg => "flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-primary/[0.08] text-primary",
            _ => "flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-primary/[0.08] text-primary"
        };

        private string IconContainerClass => Join(
            "inline-flex shrink-0 items-center justify-center rounded-md text-text-secondary transition-all duration-300",
            "motion-reduce:transition-none",
            internalExpanded
                ? "bg-primary/[0.09] text-primary"
                : "bg-surface-100",
            Size == XFSize.Lg ? "h-8 w-8" : "h-7 w-7");

        private string ChevronIconClass => Join(
            "inline-flex h-3.5 w-3.5 items-center justify-center transition-transform duration-300",
            "ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transition-none",
            internalExpanded ? "rotate-180" : "rotate-0");

        private string PlusIconClass => "h-3.5 w-3.5";

        private string PlusVerticalClass => Join(
            "origin-center transition-transform duration-300",
            "ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transition-none",
            internalExpanded ? "scale-y-0" : "scale-y-100");

        private string PanelOuterClass => Join(
            "grid overflow-hidden motion-reduce:transition-none",
            GetAnimationOuterClass(),
            internalExpanded ? "grid-rows-[1fr]" : "grid-rows-[0fr]");

        private string PanelInnerClass => Join(
            "min-h-0 overflow-hidden text-text-secondary",
            GetContentPaddingClass(),
            GetAnimationInnerClass());

        private string GetAnimationOuterClass()
        {
            return Animation switch
            {
                XFCollapseAnimation.None => "",
                XFCollapseAnimation.Height => "transition-[grid-template-rows] duration-300 ease-[cubic-bezier(0.4,0,0.2,1)]",
                XFCollapseAnimation.SlideFade => "transition-[grid-template-rows] duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]",
                _ => "transition-[grid-template-rows] duration-300 ease-[cubic-bezier(0.16,1,0.3,1)]"
            };
        }

        private string GetAnimationInnerClass()
        {
            return Animation switch
            {
                XFCollapseAnimation.None => "",
                XFCollapseAnimation.Height => "",
                XFCollapseAnimation.SlideFade => internalExpanded
                    ? "translate-y-0 opacity-100 transition-all duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transform-none motion-reduce:transition-none"
                    : "-translate-y-1 opacity-0 transition-all duration-200 ease-[cubic-bezier(0.4,0,0.2,1)] motion-reduce:transform-none motion-reduce:transition-none",
                _ => internalExpanded
                    ? "translate-y-0 opacity-100 transition-all duration-300 ease-[cubic-bezier(0.16,1,0.3,1)] motion-reduce:transform-none motion-reduce:transition-none"
                    : "-translate-y-1 opacity-0 transition-all duration-200 ease-[cubic-bezier(0.4,0,0.2,1)] motion-reduce:transform-none motion-reduce:transition-none"
            };
        }

        private string GetContentPaddingClass()
        {
            return Size switch
            {
                XFSize.Sm => "px-3 pb-3 pt-0",
                XFSize.Lg => "px-5 pb-5 pt-0",
                _ => "px-4 pb-4 pt-0"
            };
        }

        private string GetToggleTitle()
        {
            if (internalExpanded)
            {
                return CollapseTitle ?? string.Format(CultureInfo.CurrentCulture, CollapseTitleFormat, Title ?? "");
            }

            return ExpandTitle ?? string.Format(CultureInfo.CurrentCulture, ExpandTitleFormat, Title ?? "");
        }

        private static string Join(params string?[] values) =>
            string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }
}