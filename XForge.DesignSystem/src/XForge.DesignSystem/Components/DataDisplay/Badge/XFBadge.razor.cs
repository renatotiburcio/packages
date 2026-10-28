using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Badge
{
    public partial class XFBadge : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-badge";

        [Parameter] public string? Text { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndIcon { get; set; }
        [Parameter] public RenderFragment? DismissIcon { get; set; }
        [Parameter] public XFBadgeVariant Variant { get; set; } = XFBadgeVariant.Soft;
        [Parameter] public XFBadgeColor Color { get; set; } = XFBadgeColor.Info;
        [Parameter] public XFBadgeSize Size { get; set; } = XFBadgeSize.Md;
        [Parameter] public XFBadgeShape Shape { get; set; } = XFBadgeShape.Rounded;
        [Parameter] public bool Rounded { get; set; }
        [Parameter] public bool Dot { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? DismissTitle { get; set; }
        [Parameter] public string DefaultDismissTitle { get; set; } = default!;
        [Parameter] public string DismissTitleFormat { get; set; } = default!;
        [Parameter] public string? DismissAriaLabel { get; set; }
        [Parameter] public string DefaultDismissAriaLabel { get; set; } = default!;
        [Parameter] public string DismissAriaLabelFormat { get; set; } = default!;
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnDismiss { get; set; }

        private string? Role => OnClick.HasDelegate ? "button" : null;
        private string? DismissContext => !string.IsNullOrWhiteSpace(Text) ? Text : AriaLabel;
        private string ResolvedDismissTitle => DismissTitle
            ?? (!string.IsNullOrWhiteSpace(DismissContext)
                ? string.Format(CultureInfo.CurrentCulture, DismissTitleFormat, DismissContext)
                : DefaultDismissTitle);
        private string ResolvedDismissAriaLabel => DismissAriaLabel
            ?? (!string.IsNullOrWhiteSpace(DismissContext)
                ? string.Format(CultureInfo.CurrentCulture, DismissAriaLabelFormat, DismissContext)
                : DefaultDismissAriaLabel);

        protected override void OnInitialized()
        {
            // honra o valor do usuário quando informado; default localizado caso contrário
            if (string.IsNullOrWhiteSpace(DefaultDismissTitle)) DefaultDismissTitle = L("badge.dismiss", "Remover marcador");
            if (string.IsNullOrWhiteSpace(DismissTitleFormat)) DismissTitleFormat = L("badge.dismiss_format", "Remover {0}");
            if (string.IsNullOrWhiteSpace(DefaultDismissAriaLabel)) DefaultDismissAriaLabel = L("badge.dismiss_aria", "Remover marcador");
            if (string.IsNullOrWhiteSpace(DismissAriaLabelFormat)) DismissAriaLabelFormat = L("badge.dismiss_aria_format", "Remover marcador {0}");
            base.OnInitialized();
        }

        protected override string GetComponentClass()
        {
            var sizeClass = Size switch
            {
                XFBadgeSize.Xs => IsCircle ? "h-5 min-w-5 px-1 text-[0.6875rem]" : "min-h-5 px-1.5 py-0.5 text-[0.6875rem]",
                XFBadgeSize.Sm => IsCircle ? "h-6 min-w-6 px-1.5 text-xs" : "min-h-6 px-2 py-0.5 text-xs",
                XFBadgeSize.Lg => IsCircle ? "h-8 min-w-8 px-2.5 text-sm" : "min-h-8 px-3 py-1 text-sm",
                _ => IsCircle ? "h-7 min-w-7 px-2 text-xs" : "min-h-7 px-2.5 py-1 text-xs"
            };

            var shapeClass = Shape switch
            {
                XFBadgeShape.Circle => "rounded-full justify-center aspect-square",
                XFBadgeShape.Pill => "rounded-full",
                _ when Rounded => "rounded-full",
                _ => "rounded-md"
            };

            var interactiveClass = OnClick.HasDelegate
                ? "cursor-pointer hover:ring-2 hover:ring-current/10 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2"
                : "";

            return GetClass($"{BaseClass} inline-flex max-w-full items-center gap-1.5 border font-semibold leading-none outline-none transition-all duration-200 ease-out {shapeClass} {sizeClass} {ToneClass} {interactiveClass}");
        }

        private bool IsCircle => Shape == XFBadgeShape.Circle;

        private string IconClass => Size is XFBadgeSize.Xs or XFBadgeSize.Sm
            ? "flex h-3.5 w-3.5 shrink-0 items-center justify-center"
            : "flex h-4 w-4 shrink-0 items-center justify-center";

        private string DotClass => Size is XFBadgeSize.Xs or XFBadgeSize.Sm
            ? $"h-1.5 w-1.5 rounded-full {ToneDotClass}"
            : $"h-2 w-2 rounded-full {ToneDotClass}";

        private string DismissClass =>
            "ml-0.5 inline-flex h-4 w-4 shrink-0 items-center justify-center rounded-full text-current opacity-70 transition hover:bg-current/10 hover:opacity-100 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:outline-none";

        private string ToneClass => Variant switch
        {
            XFBadgeVariant.Solid => $"{ToneSolidClass} {ToneBorderClass} shadow-[0_1px_1px_rgb(15_23_42/.08)]",
            XFBadgeVariant.Outline => $"{ToneTextClass} {ToneBorderClass} bg-bg-surface shadow-[0_1px_1px_rgb(15_23_42/.03)]",
            XFBadgeVariant.Subtle => $"{ToneTextClass} border-border-light bg-surface-50",
            XFBadgeVariant.Ghost => $"{ToneTextClass} border-transparent bg-transparent",
            _ => $"{ToneSoftClass} border-transparent ring-1 ring-current/10"
        };

        private string ToneTextClass => Color switch
        {
            XFBadgeColor.Primary => "text-primary",
            XFBadgeColor.Secondary => "text-secondary",
            XFBadgeColor.Success => "text-success",
            XFBadgeColor.Warning => "text-warning",
            XFBadgeColor.Danger => "text-danger",
            XFBadgeColor.Info => "text-info",
            _ => "text-text-secondary"
        };

        private string ToneBorderClass => Color switch
        {
            XFBadgeColor.Primary => "border-primary",
            XFBadgeColor.Secondary => "border-secondary",
            XFBadgeColor.Success => "border-success",
            XFBadgeColor.Warning => "border-warning",
            XFBadgeColor.Danger => "border-danger",
            XFBadgeColor.Info => "border-info",
            _ => "border-border"
        };

        private string ToneSolidClass => Color switch
        {
            XFBadgeColor.Primary => "bg-primary text-primary-contrast",
            XFBadgeColor.Secondary => "bg-secondary text-secondary-contrast",
            XFBadgeColor.Success => "bg-success text-white",
            XFBadgeColor.Warning => "bg-warning text-surface-950",
            XFBadgeColor.Danger => "bg-danger text-white",
            XFBadgeColor.Info => "bg-info text-white",
            _ => "bg-surface-950 text-white"
        };

        private string ToneSoftClass => Color switch
        {
            XFBadgeColor.Primary => "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary",
            XFBadgeColor.Secondary => "bg-[color-mix(in_oklab,var(--color-secondary)_10%,transparent)] text-secondary",
            XFBadgeColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_10%,transparent)] text-success",
            XFBadgeColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_15%,transparent)] text-warning",
            XFBadgeColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger",
            XFBadgeColor.Info => "bg-[color-mix(in_oklab,var(--color-info)_10%,transparent)] text-info",
            _ => "bg-surface-100 text-text-secondary"
        };

        private string ToneDotClass => Color switch
        {
            XFBadgeColor.Primary => "bg-primary",
            XFBadgeColor.Secondary => "bg-secondary",
            XFBadgeColor.Success => "bg-success",
            XFBadgeColor.Warning => "bg-warning",
            XFBadgeColor.Danger => "bg-danger",
            XFBadgeColor.Info => "bg-info",
            _ => "bg-surface-500"
        };

        private async Task HandleClickAsync(MouseEventArgs e)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
        }

        private async Task HandleDismissAsync(MouseEventArgs e)
        {
            if (OnDismiss.HasDelegate)
            {
                await OnDismiss.InvokeAsync(e);
            }
        }

        private async Task HandleDismissKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key is "Enter" or " ")
            {
                await OnDismiss.InvokeAsync(new MouseEventArgs());
            }
        }
    }
}
