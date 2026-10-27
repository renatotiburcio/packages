using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Chip
{
    public partial class XFChip : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-chip";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndIcon { get; set; }
        [Parameter] public RenderFragment? Avatar { get; set; }
        [Parameter] public XFChipVariant Variant { get; set; } = XFChipVariant.Solid;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool Selected { get; set; }
        [Parameter] public bool Dismissible { get; set; }
        [Parameter] public bool Clickable { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string DismissLabel { get; set; } = "Remover";

        /// <summary>Fires when the chip is clicked (if Clickable). Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>Fires when the chip is dismissed. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback OnDismiss { get; set; }


        private string RootClass => Join(
            BaseClass,
            "inline-flex items-center gap-1 rounded-full font-medium transition-all duration-150",
            SizeClass,
            VariantClass,
            Disabled ? "cursor-not-allowed opacity-60" : (Clickable ? "cursor-pointer" : "cursor-default"),
            Selected ? SelectedClass : null,
            Class);

        private string IconClass => Size switch
        {
            XFSize.Sm => "h-3 w-3 shrink-0",
            XFSize.Lg => "h-5 w-5 shrink-0",
            _ => "h-4 w-4 shrink-0"
        };

        private string AvatarClass => Size switch
        {
            XFSize.Sm => "-ml-0.5 h-4 w-4 shrink-0 overflow-hidden rounded-full",
            XFSize.Lg => "-ml-1 h-6 w-6 shrink-0 overflow-hidden rounded-full",
            _ => "-ml-0.5 h-5 w-5 shrink-0 overflow-hidden rounded-full"
        };

        private string DismissButtonClass => Join(
            "inline-flex shrink-0 items-center justify-center rounded-full outline-none transition-all duration-150",
            "hover:bg-[color-mix(in_oklab,var(--color-primary)_15%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)]",
            "disabled:pointer-events-none disabled:opacity-60",
            Size switch
            {
                XFSize.Sm => "h-3.5 w-3.5",
                XFSize.Lg => "h-5 w-5",
                _ => "h-4 w-4"
            });

        private string SizeClass => Size switch
        {
            XFSize.Sm => "h-6 px-2 py-0.5 text-xs",
            XFSize.Lg => "h-9 px-4 py-1.5 text-sm",
            _ => "h-7 px-3 py-1 text-xs"
        };

        private string VariantClass => Tone switch
        {
            XFSemanticColor.Primary => Variant switch
            {
                XFChipVariant.Outline => "border border-primary text-primary",
                XFChipVariant.Soft => "bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] text-primary",
                XFChipVariant.Ghost => "text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]",
                _ => "bg-primary text-primary-contrast"
            },
            XFSemanticColor.Secondary => Variant switch
            {
                XFChipVariant.Outline => "border border-secondary text-secondary",
                XFChipVariant.Soft => "bg-[color-mix(in_oklab,var(--color-secondary)_12%,transparent)] text-secondary",
                XFChipVariant.Ghost => "text-secondary hover:bg-[color-mix(in_oklab,var(--color-secondary)_8%,transparent)]",
                _ => "bg-secondary text-secondary-contrast"
            },
            XFSemanticColor.Success => Variant switch
            {
                XFChipVariant.Outline => "border border-success text-success",
                XFChipVariant.Soft => "bg-[color-mix(in_oklab,var(--color-success)_12%,transparent)] text-success",
                XFChipVariant.Ghost => "text-success hover:bg-[color-mix(in_oklab,var(--color-success)_8%,transparent)]",
                _ => "bg-success text-white"
            },
            XFSemanticColor.Warning => Variant switch
            {
                XFChipVariant.Outline => "border border-warning text-warning",
                XFChipVariant.Soft => "bg-[color-mix(in_oklab,var(--color-warning)_12%,transparent)] text-warning",
                XFChipVariant.Ghost => "text-warning hover:bg-[color-mix(in_oklab,var(--color-warning)_8%,transparent)]",
                _ => "bg-warning text-warning-contrast"
            },
            XFSemanticColor.Danger => Variant switch
            {
                XFChipVariant.Outline => "border border-danger text-danger",
                XFChipVariant.Soft => "bg-[color-mix(in_oklab,var(--color-danger)_12%,transparent)] text-danger",
                XFChipVariant.Ghost => "text-danger hover:bg-[color-mix(in_oklab,var(--color-danger)_8%,transparent)]",
                _ => "bg-danger text-white"
            },
            XFSemanticColor.Info => Variant switch
            {
                XFChipVariant.Outline => "border border-info text-info",
                XFChipVariant.Soft => "bg-[color-mix(in_oklab,var(--color-info)_12%,transparent)] text-info",
                XFChipVariant.Ghost => "text-info hover:bg-[color-mix(in_oklab,var(--color-info)_8%,transparent)]",
                _ => "bg-info text-white"
            },
            _ => Variant switch
            {
                XFChipVariant.Outline => "border border-border-light text-text-primary",
                XFChipVariant.Soft => "bg-surface-100 text-text-primary",
                XFChipVariant.Ghost => "text-text-primary hover:bg-surface-100",
                _ => "bg-surface-200 text-text-primary"
            }
        };

        private string SelectedClass => "ring-2 ring-primary ring-offset-1 ring-offset-bg-surface";

        protected override void OnInitialized()
        {
            DismissLabel = L("chip.dismiss", DismissLabel);
            base.OnInitialized();
        }

        protected override string GetComponentClass()
        {
            return RootClass;
        }

        private async Task HandleClickAsync(MouseEventArgs args)
        {
            if (Disabled || !Clickable)
            {
                return;
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(args);
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (Disabled || !Clickable)
            {
                return;
            }

            if (args.Key == "Enter" || args.Key == " ")
            {
                if (OnClick.HasDelegate)
                {
                    await OnClick.InvokeAsync(new MouseEventArgs());
                }
            }
        }

        private async Task HandleDismissAsync()
        {
            if (Disabled)
            {
                return;
            }

            if (OnDismiss.HasDelegate)
            {
                await OnDismiss.InvokeAsync();
            }
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
