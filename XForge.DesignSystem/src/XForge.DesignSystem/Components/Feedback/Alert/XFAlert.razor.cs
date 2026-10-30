using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Alert
{
    public partial class XFAlert : XForgeComponentBase
    {
        protected override string BaseClass => "xf-alert";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? Actions { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Text { get; set; }
        [Parameter] public XFIntent Intent { get; set; } = XFIntent.Info;
        [Parameter] public XFAlertVariant Variant { get; set; } = XFAlertVariant.Soft;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public bool ShowIcon { get; set; } = true;
        [Parameter] public bool Dismissible { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
        [Parameter] public EventCallback OnDismiss { get; set; }
        [Parameter] public XFAriaLive Live { get; set; } = XFAriaLive.Polite;
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? DismissLabel { get; set; }
        [Parameter] public string DismissLabelFormat { get; set; } = "Fechar alerta {0}";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ChildContent, StartIcon, Actions, Title, Text, Intent, Variant, Size, ShowIcon, Dismissible, Visible, VisibleChanged, OnDismiss, Live, AriaLabel, DismissLabel, DismissLabelFormat);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private bool ShowLeadingIcon => ShowIcon || StartIcon is not null;
        private string DismissContext => !string.IsNullOrWhiteSpace(Title) ? Title : AriaLabel ?? "alerta";
        private string ResolvedDismissLabel => DismissLabel ?? string.Format(CultureInfo.CurrentCulture, DismissLabelFormat, DismissContext);

        private string Role => Live == XFAriaLive.Assertive ? "alert" : "status";

        private string AriaLiveValue => Live switch
        {
            XFAriaLive.Assertive => "assertive",
            XFAriaLive.Off => "off",
            _ => "polite"
        };

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {BaseAlertClass} {SizeClass} {VariantClass}");
        }

        private string BaseAlertClass =>
            "relative flex w-full items-start gap-3 rounded-lg border outline-none transition-all duration-200 ease-out";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "px-3 py-2 text-sm",
            XFSize.Lg => "px-5 py-4 text-base",
            _ => "px-4 py-3 text-sm"
        };

        private string VariantClass => Variant switch
        {
            XFAlertVariant.Solid => SolidClass,
            XFAlertVariant.Outline => OutlineClass,
            XFAlertVariant.Subtle => SubtleClass,
            _ => SoftClass
        };

        private string SoftClass => Intent switch
        {
            XFIntent.Success => "border-transparent bg-[color-mix(in_oklab,var(--color-success)_10%,transparent)] text-success ring-1 ring-[color-mix(in_oklab,var(--color-success)_20%,transparent)] shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            XFIntent.Warning => "border-transparent bg-[color-mix(in_oklab,var(--color-warning)_15%,transparent)] text-warning ring-1 ring-[color-mix(in_oklab,var(--color-warning)_25%,transparent)] shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            XFIntent.Danger => "border-transparent bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger ring-1 ring-[color-mix(in_oklab,var(--color-danger)_20%,transparent)] shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            XFIntent.Primary => "border-transparent bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            XFIntent.Secondary => "border-transparent bg-[color-mix(in_oklab,var(--color-secondary)_10%,transparent)] text-secondary ring-1 ring-[color-mix(in_oklab,var(--color-secondary)_20%,transparent)] shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            XFIntent.Neutral => "border-transparent bg-surface-100 text-text-primary ring-1 ring-surface-300/70 shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            _ => "border-transparent bg-[color-mix(in_oklab,var(--color-info)_10%,transparent)] text-info ring-1 ring-[color-mix(in_oklab,var(--color-info)_20%,transparent)] shadow-[0_1px_2px_rgb(15_23_42/.04)]"
        };

        private string SolidClass => Intent switch
        {
            XFIntent.Success => "border-success bg-success text-white shadow-[0_8px_20px_rgb(var(--color-success)/.18)]",
            XFIntent.Warning => "border-warning bg-warning text-surface-950 shadow-[0_8px_20px_rgb(var(--color-warning)/.18)]",
            XFIntent.Danger => "border-danger bg-danger text-white shadow-[0_8px_20px_rgb(var(--color-danger)/.18)]",
            XFIntent.Primary => "border-primary bg-primary text-primary-contrast shadow-[0_8px_20px_rgb(var(--color-primary)/.18)]",
            XFIntent.Secondary => "border-secondary bg-secondary text-secondary-contrast shadow-[0_8px_20px_rgb(var(--color-secondary)/.18)]",
            XFIntent.Neutral => "border-surface-900 bg-surface-900 text-white shadow-[0_8px_20px_rgb(15_23_42/.16)]",
            _ => "border-info bg-info text-white shadow-[0_8px_20px_rgb(var(--color-info)/.18)]"
        };

        private string OutlineClass => Intent switch
        {
            XFIntent.Success => "border-[color-mix(in_oklab,var(--color-success)_45%,transparent)] bg-bg-surface text-success ring-1 ring-[color-mix(in_oklab,var(--color-success)_10%,transparent)]",
            XFIntent.Warning => "border-[color-mix(in_oklab,var(--color-warning)_50%,transparent)] bg-bg-surface text-warning ring-1 ring-[color-mix(in_oklab,var(--color-warning)_10%,transparent)]",
            XFIntent.Danger => "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] bg-bg-surface text-danger ring-1 ring-[color-mix(in_oklab,var(--color-danger)_10%,transparent)]",
            XFIntent.Primary => "border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] bg-bg-surface text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]",
            XFIntent.Secondary => "border-[color-mix(in_oklab,var(--color-secondary)_45%,transparent)] bg-bg-surface text-secondary ring-1 ring-[color-mix(in_oklab,var(--color-secondary)_10%,transparent)]",
            XFIntent.Neutral => "border-surface-300 bg-bg-surface text-text-primary ring-1 ring-surface-200",
            _ => "border-[color-mix(in_oklab,var(--color-info)_45%,transparent)] bg-bg-surface text-info ring-1 ring-[color-mix(in_oklab,var(--color-info)_10%,transparent)]"
        };

        private string SubtleClass => Intent switch
        {
            XFIntent.Success => "border-[color-mix(in_oklab,var(--color-success)_15%,transparent)] bg-bg-surface text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-success)_15%,transparent)]",
            XFIntent.Warning => "border-[color-mix(in_oklab,var(--color-warning)_20%,transparent)] bg-bg-surface text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-warning)_15%,transparent)]",
            XFIntent.Danger => "border-[color-mix(in_oklab,var(--color-danger)_15%,transparent)] bg-bg-surface text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-danger)_15%,transparent)]",
            XFIntent.Primary => "border-[color-mix(in_oklab,var(--color-primary)_15%,transparent)] bg-bg-surface text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_15%,transparent)]",
            XFIntent.Secondary => "border-[color-mix(in_oklab,var(--color-secondary)_15%,transparent)] bg-bg-surface text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-secondary)_15%,transparent)]",
            XFIntent.Neutral => "border-border-light bg-bg-surface text-text-primary ring-1 ring-surface-200",
            _ => "border-[color-mix(in_oklab,var(--color-info)_15%,transparent)] bg-bg-surface text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-info)_15%,transparent)]"
        };

        private string IconClass => Size switch
        {
            XFSize.Sm => "mt-0.5 flex h-5 w-5 shrink-0 items-center justify-center rounded-md bg-current/10 text-xs font-semibold",
            XFSize.Lg => "mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-current/10 text-sm font-semibold",
            _ => "mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-current/10 text-xs font-semibold"
        };

        private string TitleClass => "font-semibold leading-5 tracking-normal";

        private string TextClass => ChildContent is null
            ? "leading-6 text-current/85"
            : "mt-1 leading-6 text-current/85";

        private string BodyClass => string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Text)
            ? "leading-6 text-current/85"
            : "mt-1 leading-6 text-current/85";

        private string ActionsClass => Size == XFSize.Sm
            ? "ml-auto flex shrink-0 flex-wrap items-center gap-1.5"
            : "ml-auto flex shrink-0 flex-wrap items-center gap-2";

        private string DismissClass =>
            "ml-auto inline-flex h-7 w-7 shrink-0 items-center justify-center rounded-md text-current/65 outline-none transition-all duration-200 hover:bg-current/10 hover:text-current focus-visible:ring-2 focus-visible:ring-current/25 focus-visible:ring-offset-2 active:scale-95";

        private string DefaultIcon => Intent switch
        {
            XFIntent.Success => "circle-check",
            XFIntent.Warning => "triangle-exclamation",
            XFIntent.Danger => "circle-xmark",
            _ => "circle-info"
        };

        public async Task ShowAsync()
        {
            await SetVisibleAsync(true);
        }

        public async Task HideAsync()
        {
            await SetVisibleAsync(false);
        }

        public async Task ToggleAsync()
        {
            await SetVisibleAsync(!Visible);
        }

        private async Task DismissAsync()
        {
            await SetVisibleAsync(false);

            if (OnDismiss.HasDelegate)
            {
                await OnDismiss.InvokeAsync();
            }
        }

        private async Task SetVisibleAsync(bool visible)
        {
            Visible = visible;

            if (VisibleChanged.HasDelegate)
            {
                await VisibleChanged.InvokeAsync(visible);
            }
        }
    }
}
