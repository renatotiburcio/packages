using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

using XForge.DesignSystem.Components.Layout;
namespace XForge.DesignSystem.Components.Navigation.Steps
{
    public partial class XFSteps : XForgeComponentBase
    {
        protected override string BaseClass => "xf-steps";

        [Parameter] public IReadOnlyList<XFStepItemModel> Items { get; set; } = Array.Empty<XFStepItemModel>();
        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Horizontal;
        [Parameter] public XFContainerVariant Variant { get; set; } = XFContainerVariant.Plain;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;

        [Parameter] public EventCallback<XFStepClickEventArgs> StepClick { get; set; }
        [Parameter] public EventCallback<XFStepClickEventArgs> StepClicking { get; set; }
        [Parameter] public string AriaLabel { get; set; } = "Etapas";
        [Parameter] public string StepAriaLabelFormat { get; set; } = "{0}, etapa {1} de {2}, {3}";
        [Parameter] public string CompleteStatusLabel { get; set; } = "concluido";
        [Parameter] public string CurrentStatusLabel { get; set; } = "atual";
        [Parameter] public string ErrorStatusLabel { get; set; } = "com erro";
        [Parameter] public string DisabledStatusLabel { get; set; } = "desabilitado";
        [Parameter] public string PendingStatusLabel { get; set; } = "pendente";
        [Parameter] public RenderFragment<XFStepContext>? ItemTemplate { get; set; }
        [Parameter] public RenderFragment<XFStepContext>? MarkerTemplate { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Items, Orientation, Variant, Size, StepClick, StepClicking, AriaLabel, StepAriaLabelFormat, CompleteStatusLabel, CurrentStatusLabel, ErrorStatusLabel, DisabledStatusLabel, PendingStatusLabel, ItemTemplate, MarkerTemplate);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override string GetComponentClass()
        {
            var variantClass = Variant switch
            {
                XFContainerVariant.Soft => "rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] p-3",
                XFContainerVariant.Surface => "rounded-lg border border-border-light bg-bg-surface p-3 shadow-[0_1px_2px_rgb(15_23_42/.04)]",
                _ => ""
            };

            var disabledClass = Disabled ? "opacity-60 pointer-events-none" : "";
            return GetClass($"{BaseClass} {variantClass} {disabledClass}");
        }

        private bool IsVertical => Orientation == XFOrientation.Vertical;

        private string ListClass => IsVertical ? "grid gap-3" : "grid gap-3 sm:grid-flow-col sm:auto-cols-fr";

        private string ItemClass(XFStepItemModel item, int index)
        {
            var opacity = IsItemDisabled(item) ? "opacity-60" : "";
            var clickable = !Disabled && !IsItemDisabled(item) && StepClick.HasDelegate ? "cursor-pointer" : "";
            return IsVertical
                ? $"relative grid grid-cols-[auto_1fr] gap-3 {opacity} {clickable}"
                : $"relative grid grid-cols-[auto_1fr] gap-3 sm:grid-cols-1 sm:justify-items-center sm:text-center {opacity} {clickable}";
        }

        private bool IsItemDisabled(XFStepItemModel item) => item.Disabled || item.Status == XFStepStatus.Disabled;

        private string GetStepAriaLabel(XFStepItemModel item, int index)
        {
            var status = item.Status switch
            {
                XFStepStatus.Complete => CompleteStatusLabel,
                XFStepStatus.Current => CurrentStatusLabel,
                XFStepStatus.Error => ErrorStatusLabel,
                XFStepStatus.Disabled => DisabledStatusLabel,
                _ => PendingStatusLabel
            };

            return string.Format(System.Globalization.CultureInfo.CurrentCulture, StepAriaLabelFormat, item.Label, index + 1, Items.Count, status);
        }

        private string MarkerShellClass => IsVertical ? "relative flex justify-center" : "relative flex w-full justify-center";

        private string ConnectorClass(XFStepItemModel item, int index)
        {
            if (index == Items.Count - 1)
            {
                return "hidden";
            }

            var tone = item.Status is XFStepStatus.Complete ? "bg-[var(--color-primary)]" : "bg-border-light";
            return IsVertical
                ? $"absolute left-1/2 top-8 h-[calc(100%+0.75rem)] w-0.5 -translate-x-1/2 {tone}"
                : $"absolute left-1/2 top-1/2 hidden h-0.5 w-full translate-x-5 -translate-y-1/2 sm:block {tone}";
        }

        private string MarkerClass(XFStepItemModel item)
        {
            var sizeClass = Size switch
            {
                XFSize.Sm => "h-7 w-7 text-xs",
                XFSize.Lg => "h-10 w-10 text-base",
                _ => "h-8 w-8 text-sm"
            };

            var stateClass = item.Status switch
            {
                XFStepStatus.Complete => "border-[var(--color-primary)] bg-primary text-primary-contrast shadow-[0_4px_10px_rgb(15_23_42/.12)]",
                XFStepStatus.Current => "border-[var(--color-primary)] bg-bg-surface text-primary ring-4 ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]",
                XFStepStatus.Error => "border-[var(--color-danger)] bg-danger text-white ring-4 ring-[color-mix(in_oklab,var(--color-danger)_14%,transparent)]",
                XFStepStatus.Disabled => "border-border-light bg-surface-100 text-text-muted",
                _ => "border-border-light bg-bg-surface text-text-secondary"
            };

            return $"relative z-10 inline-flex shrink-0 items-center justify-center rounded-full border font-semibold transition-all duration-200 {sizeClass} {stateClass}";
        }

        private string TextClass => IsVertical ? "min-w-0 pb-2" : "min-w-0 sm:max-w-40";

        private string DescriptionClass => Size == XFSize.Sm
            ? "mt-0.5 block text-xs text-text-muted"
            : "mt-1 block text-xs text-text-muted";

        private string LabelClass(XFStepItemModel item)
        {
            var tone = item.Status switch
            {
                XFStepStatus.Current => "text-primary",
                XFStepStatus.Complete => "text-text-primary",
                XFStepStatus.Error => "text-danger",
                _ => "text-text-secondary"
            };

            var size = Size switch
            {
                XFSize.Sm => "text-xs",
                XFSize.Lg => "text-base",
                _ => "text-sm"
            };

            return $"block truncate font-semibold {tone} {size}";
        }

        private XFStepStatus GetEffectiveStatus(XFStepItemModel item) => Disabled ? XFStepStatus.Disabled : item.Status;

        private async Task HandleStepClickAsync(XFStepItemModel item, int index)
        {
            if (Disabled || IsItemDisabled(item))
            {
                return;
            }

            var args = new XFStepClickEventArgs(item, index);
            await StepClicking.InvokeAsync(args);

            if (args.Cancel)
            {
                return;
            }

            await StepClick.InvokeAsync(args);
        }

        private RenderFragment DefaultMarker(XFStepContext ctx) => builder =>
        {
            if (ctx.Item.Icon is not null)
            {
                builder.AddContent(0, ctx.Item.Icon);
            }
            else if (ctx.Status == XFStepStatus.Complete)
            {
                builder.AddContent(0, RenderCheck);
            }
            else if (ctx.Status == XFStepStatus.Error)
            {
                builder.AddContent(0, RenderAlert);
            }
            else
            {
                builder.AddContent(0, ctx.Index + 1);
            }
        };

        private RenderFragment RenderCheck => builder => RenderSvg(builder, "M20 6L9 17l-5-5");

        private RenderFragment RenderAlert => builder => RenderSvg(builder, "M12 8v4m0 4h.01");

        private static void RenderSvg(RenderTreeBuilder builder, string path)
        {
            builder.OpenElement(0, "svg");
            builder.AddAttribute(1, "class", "h-4 w-4");
            builder.AddAttribute(2, "viewBox", "0 0 24 24");
            builder.AddAttribute(3, "fill", "none");
            builder.AddAttribute(4, "stroke", "currentColor");
            builder.AddAttribute(5, "stroke-width", "2.4");
            builder.AddAttribute(6, "stroke-linecap", "round");
            builder.AddAttribute(7, "stroke-linejoin", "round");
            builder.OpenElement(8, "path");
            builder.AddAttribute(9, "d", path);
            builder.CloseElement();
            builder.CloseElement();
        }
    }
}
