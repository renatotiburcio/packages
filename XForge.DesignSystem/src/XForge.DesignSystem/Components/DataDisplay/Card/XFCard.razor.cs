using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.DataDisplay.Card
{
    public partial class XFCard : XForgeComponentBase
    {
        protected override string BaseClass => "xf-card";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Media { get; set; }
        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public RenderFragment? Actions { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
        [Parameter] public XFPadding Padding { get; set; } = XFPadding.Md;
        [Parameter] public XFCardVariant Variant { get; set; } = XFCardVariant.Elevated;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFCardRadius Radius { get; set; } = XFCardRadius.Lg;
        [Parameter] public bool Interactive { get; set; }
        [Parameter] public bool AreaRestricted { get; set; } = true;
        [Parameter] public bool Dividers { get; set; } = true;
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        private bool IsInteractive => Interactive || OnClick.HasDelegate;
        private string? Role => IsInteractive ? "button" : null;

        protected override string GetComponentClass()
        {
            var overflowClass = AreaRestricted ? "overflow-hidden" : "overflow-visible";
            var interactiveClass = IsInteractive
                ? "cursor-pointer outline-none transition-all duration-200 ease-out hover:-translate-y-0.5 hover:ring-2 hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 active:translate-y-0"
                : "transition-colors duration-200";

            return GetClass($"{BaseClass} {overflowClass} {RadiusClass} {VariantClass} {ToneClass} {interactiveClass}");
        }

        private string GetContentClass()
        {
            return PaddingClass;
        }

        private string GetMediaClass()
        {
            return "bg-surface-100";
        }

        private string GetHeaderClass()
        {
            var divider = Dividers ? "border-b border-border-light" : "";
            return $"{divider} {PaddingClass}";
        }

        private string GetFooterClass()
        {
            var divider = Dividers ? "border-t border-border-light" : "";
            return $"{divider} {PaddingClass}";
        }

        private string GetActionsClass()
        {
            var divider = Dividers ? "border-t border-border-light" : "";
            return $"{divider} {PaddingClass} flex flex-wrap items-center justify-end gap-2";
        }

        private string PaddingClass => Padding switch
        {
            XFPadding.None => "p-0",
            XFPadding.Sm => "p-3",
            XFPadding.Lg => "p-6",
            XFPadding.Xl => "p-8",
            _ => "p-5"
        };

        private string RadiusClass => Radius switch
        {
            XFCardRadius.Sm => "rounded-md",
            XFCardRadius.Md => "rounded-lg",
            XFCardRadius.Xl => "rounded-xl",
            _ => "rounded-lg"
        };

        private string VariantClass => Variant switch
        {
            XFCardVariant.Outlined => $"{BorderBaseClass} bg-bg-surface shadow-[0_1px_1px_rgb(15_23_42/.03)]",
            XFCardVariant.Filled => $"{BorderBaseClass} bg-surface-50 shadow-[inset_0_1px_0_rgb(255_255_255/.45)]",
            XFCardVariant.Flat => "border border-transparent bg-bg-surface shadow-none",
            XFCardVariant.Ghost => "border border-transparent bg-transparent shadow-none",
            _ => $"{BorderBaseClass} bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.06),0_8px_24px_rgb(15_23_42/.04)]"
        };

        private bool HasTone => Tone != XFSemanticColor.Default;

        private string BorderBaseClass => HasTone
            ? "border border-transparent"
            : "border border-border-light";

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Primary => "border-l-4 border-l-[var(--color-primary)] bg-[color-mix(in_oklab,var(--color-primary)_4%,white)] ring-1 ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]",
            XFSemanticColor.Secondary => "border-l-4 border-l-[var(--color-secondary)] bg-[color-mix(in_oklab,var(--color-secondary)_4%,white)] ring-1 ring-[color-mix(in_oklab,var(--color-secondary)_10%,transparent)]",
            XFSemanticColor.Success => "border-l-4 border-l-[var(--color-success)] bg-[color-mix(in_oklab,var(--color-success)_5%,white)] ring-1 ring-[color-mix(in_oklab,var(--color-success)_10%,transparent)]",
            XFSemanticColor.Warning => "border-l-4 border-l-[var(--color-warning)] bg-[color-mix(in_oklab,var(--color-warning)_8%,white)] ring-1 ring-[color-mix(in_oklab,var(--color-warning)_15%,transparent)]",
            XFSemanticColor.Danger => "border-l-4 border-l-[var(--color-danger)] bg-[color-mix(in_oklab,var(--color-danger)_4%,white)] ring-1 ring-[color-mix(in_oklab,var(--color-danger)_10%,transparent)]",
            XFSemanticColor.Neutral => "border-l-4 border-l-[var(--color-surface-400)] bg-surface-50 ring-1 ring-surface-300/60",
            _ => ""
        };

        private async Task HandleClickAsync(MouseEventArgs e)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
        }
    }
}
