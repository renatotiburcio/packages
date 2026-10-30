using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Toast
{
    public partial class XFToast : XForgeComponentBase
    {
        protected override string BaseClass => "xf-toast";

        [Parameter] public XFToastItem? Item { get; set; }
        [Parameter] public EventCallback<XFToastItem> OnDismiss { get; set; }
        [Parameter] public string DismissLabel { get; set; } = "Fechar notificação";
        [Parameter] public string ActionLabelFormat { get; set; } = "{0}: {1}";
        [Parameter] public int Index { get; set; }
        [Parameter] public bool Expanded { get; set; }
        [Parameter] public bool IsTopPlacement { get; set; }
        [Parameter] public int Gap { get; set; } = 14;
        [Parameter] public int ItemHeight { get; set; } = 88;
        [Parameter] public bool CloseButton { get; set; }
        [Parameter] public bool RichColors { get; set; }
        [Parameter] public bool SolidColors { get; set; }
        [Parameter] public XFToastColorMode ColorMode { get; set; } = XFToastColorMode.Default;

        // assinatura de render: item de toast — evita re-render da lista inteira a cada mudança
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Item, OnDismiss, DismissLabel, ActionLabelFormat,
                Index, Expanded, IsTopPlacement, Gap, ItemHeight, CloseButton);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string Role => Item?.Live == XFAriaLive.Assertive ? "alert" : "status";

        private string AriaLive => Item?.Live switch
        {
            XFAriaLive.Assertive => "assertive",
            XFAriaLive.Off => "off",
            _ => "polite"
        };

        private string TitleId => $"{Id}-title";
        private string MessageId => $"{Id}-message";
        private string? AriaLabelledBy => Item is not null && Item.Content is null && !string.IsNullOrWhiteSpace(Item.Title) ? TitleId : null;
        private string? AriaDescribedBy => Item is not null && Item.Content is null && !string.IsNullOrWhiteSpace(Item.Message) ? MessageId : null;

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {StackClass} pointer-events-auto box-border flex w-full max-w-sm items-start gap-3 rounded-lg border p-4 text-sm shadow-[0_18px_40px_rgb(15_23_42/.14),0_1px_2px_rgb(15_23_42/.08)] ring-1 transition-all duration-300 ease-out will-change-transform motion-safe:animate-in {ToneClass}");
        }

        private string StackClass => Expanded
            ? "relative"
            : "relative row-start-1 col-start-1 group-hover/toaster:translate-y-[var(--xf-toast-expanded-y)] group-hover/toaster:scale-100 group-hover/toaster:opacity-100";

        private string StackStyle
        {
            get
            {
                var safeIndex = Math.Max(0, Index);
                var collapsedOffset = Math.Min(safeIndex, 3) * 12;
                var expandedOffset = safeIndex * (Math.Max(64, ItemHeight) + Math.Max(0, Gap));
                var scale = Math.Max(0.92, 1 - safeIndex * 0.04).ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
                var opacity = Math.Max(0.72, 1 - safeIndex * 0.08).ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
                var collapsedDirection = IsTopPlacement ? 1 : -1;
                var expandedDirection = IsTopPlacement ? 1 : -1;
                var baseStyle = $"width:100%;min-height:var(--xf-toast-item-height);z-index:{100 - safeIndex};--xf-toast-expanded-y:{expandedDirection * expandedOffset}px;";

                if (Expanded)
                {
                    return $"{baseStyle}transform:translateY(var(--xf-toast-expanded-y));";
                }

                return $"{baseStyle}transform:translateY({collapsedDirection * collapsedOffset}px) scale({scale});opacity:{opacity};";
            }
        }

        private string ToneClass => Item?.Intent switch
        {
            XFIntent.Success when EffectiveColorMode is XFToastColorMode.Solid => "border-success bg-success text-white ring-[color-mix(in_oklab,var(--color-success)_22%,transparent)]",
            XFIntent.Warning when EffectiveColorMode is XFToastColorMode.Solid => "border-warning bg-warning text-surface-950 ring-[color-mix(in_oklab,var(--color-warning)_22%,transparent)]",
            XFIntent.Danger when EffectiveColorMode is XFToastColorMode.Solid => "border-danger bg-danger text-white ring-[color-mix(in_oklab,var(--color-danger)_22%,transparent)]",
            XFIntent.Primary when EffectiveColorMode is XFToastColorMode.Solid => "border-primary bg-primary text-primary-contrast ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]",
            XFIntent.Info when EffectiveColorMode is XFToastColorMode.Solid => "border-info bg-info text-white ring-[color-mix(in_oklab,var(--color-info)_22%,transparent)]",
            XFIntent.Neutral when EffectiveColorMode is XFToastColorMode.Solid => "border-surface-900 bg-surface-900 text-white ring-surface-300",
            XFIntent.Success when EffectiveColorMode is XFToastColorMode.Rich => "border-[color-mix(in_oklab,var(--color-success)_24%,transparent)] bg-[color-mix(in_oklab,var(--color-success)_8%,var(--color-bg-surface))] text-success ring-[color-mix(in_oklab,var(--color-success)_18%,transparent)]",
            XFIntent.Warning when EffectiveColorMode is XFToastColorMode.Rich => "border-[color-mix(in_oklab,var(--color-warning)_30%,transparent)] bg-[color-mix(in_oklab,var(--color-warning)_10%,var(--color-bg-surface))] text-warning ring-[color-mix(in_oklab,var(--color-warning)_18%,transparent)]",
            XFIntent.Danger when EffectiveColorMode is XFToastColorMode.Rich => "border-[color-mix(in_oklab,var(--color-danger)_28%,transparent)] bg-[color-mix(in_oklab,var(--color-danger)_8%,var(--color-bg-surface))] text-danger ring-[color-mix(in_oklab,var(--color-danger)_18%,transparent)]",
            XFIntent.Primary when EffectiveColorMode is XFToastColorMode.Rich => "border-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] bg-[color-mix(in_oklab,var(--color-primary)_8%,var(--color-bg-surface))] text-primary ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
            XFIntent.Info when EffectiveColorMode is XFToastColorMode.Rich => "border-[color-mix(in_oklab,var(--color-info)_24%,transparent)] bg-[color-mix(in_oklab,var(--color-info)_8%,var(--color-bg-surface))] text-info ring-[color-mix(in_oklab,var(--color-info)_18%,transparent)]",
            XFIntent.Neutral when EffectiveColorMode is XFToastColorMode.Rich => "border-border-light bg-surface-100 text-text-primary ring-surface-200",
            XFIntent.Success => "border-[color-mix(in_oklab,var(--color-success)_24%,transparent)] bg-bg-surface ring-[color-mix(in_oklab,var(--color-success)_18%,transparent)]",
            XFIntent.Warning => "border-[color-mix(in_oklab,var(--color-warning)_30%,transparent)] bg-bg-surface ring-[color-mix(in_oklab,var(--color-warning)_18%,transparent)]",
            XFIntent.Danger => "border-[color-mix(in_oklab,var(--color-danger)_28%,transparent)] bg-bg-surface ring-[color-mix(in_oklab,var(--color-danger)_18%,transparent)]",
            XFIntent.Primary => "border-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] bg-bg-surface ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
            XFIntent.Neutral => "border-border-light bg-bg-surface ring-surface-200",
            _ => "border-border-light bg-bg-surface ring-surface-200"
        };

        private XFToastColorMode EffectiveColorMode => SolidColors ? XFToastColorMode.Solid : RichColors ? XFToastColorMode.Rich : ColorMode;

        private string TitleClass => EffectiveColorMode is XFToastColorMode.Solid
            ? "font-semibold leading-5 text-inherit"
            : "font-semibold leading-5 text-text-primary";

        private string MessageClass => EffectiveColorMode is XFToastColorMode.Solid
            ? "mt-1 text-sm leading-5 text-inherit opacity-90"
            : "mt-1 text-sm leading-5 text-text-secondary";

        private string IconClass => Item?.Intent switch
        {
            XFIntent.Success => "mt-0.5 grid h-8 w-8 shrink-0 place-items-center rounded-lg bg-[color-mix(in_oklab,var(--color-success)_12%,transparent)] text-success",
            XFIntent.Warning => "mt-0.5 grid h-8 w-8 shrink-0 place-items-center rounded-lg bg-[color-mix(in_oklab,var(--color-warning)_16%,transparent)] text-warning",
            XFIntent.Danger => "mt-0.5 grid h-8 w-8 shrink-0 place-items-center rounded-lg bg-[color-mix(in_oklab,var(--color-danger)_12%,transparent)] text-danger",
            XFIntent.Primary => "mt-0.5 grid h-8 w-8 shrink-0 place-items-center rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] text-primary",
            XFIntent.Neutral => "mt-0.5 grid h-8 w-8 shrink-0 place-items-center rounded-lg bg-surface-100 text-text-primary",
            _ => "mt-0.5 grid h-8 w-8 shrink-0 place-items-center rounded-lg bg-[color-mix(in_oklab,var(--color-info)_12%,transparent)] text-info"
        };

        private string DefaultIcon => Item?.Intent switch
        {
            XFIntent.Success => "circle-check",
            XFIntent.Warning => "triangle-exclamation",
            XFIntent.Danger => "circle-xmark",
            XFIntent.Primary => "bell-ring",
            XFIntent.Neutral => "bell",
            _ => "circle-info"
        };

        private string ActionClass =>
            "mt-3 inline-flex h-8 items-center justify-center rounded-md px-2.5 text-xs font-semibold text-primary transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";

        private string DismissClass =>
            "ml-1 inline-flex h-7 w-7 shrink-0 items-center justify-center rounded-md text-text-muted transition-all duration-150 hover:bg-surface-100 hover:text-text-primary focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";

        private async Task InvokeActionAsync()
        {
            if (Item?.Action is not null)
            {
                await Item.Action.Invoke(Item);
            }
        }

        private string GetActionLabel()
        {
            if (Item is null || string.IsNullOrWhiteSpace(Item.Title))
            {
                return Item?.ActionText ?? string.Empty;
            }

            return string.Format(CultureInfo.CurrentCulture, ActionLabelFormat, Item.ActionText, Item.Title);
        }

        private async Task DismissAsync()
        {
            if (Item is not null)
            {
                await OnDismiss.InvokeAsync(Item);
            }
        }
    }
}
