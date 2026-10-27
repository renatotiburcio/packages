using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Components.Layout;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Timeline
{
    public partial class XFTimelineItem : XForgeComponentBase, IDisposable
    {
        [CascadingParameter] private XFTimeline? ParentTimeline { get; set; }

        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Timestamp { get; set; }
        [Parameter] public string? Text { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Icon { get; set; }
        [Parameter] public RenderFragment? OppositeContent { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public XFSemanticColor? Tone { get; set; }
        [Parameter] public XFSize? Size { get; set; }

        private bool registered;
        private int Index => ParentTimeline?.GetIndex(this) ?? 0;
        private bool IsLast => ParentTimeline?.IsLast(this) ?? true;
        private bool IsHorizontal => ParentTimeline?.CurrentOrientation == XFOrientation.Horizontal;
        private bool IsOpposite => ParentTimeline?.IsOpposite(Index) ?? false;
        private XFSize ResolvedSize => Size ?? ParentTimeline?.CurrentSize ?? XFSize.Md;
        private XFSemanticColor ResolvedTone => Tone ?? ParentTimeline?.CurrentTone ?? XFSemanticColor.Primary;
        private bool HasConnector => ParentTimeline?.HasConnector == true && !IsLast;
        private string ResolvedAriaLabel => AriaLabel ?? string.Format(
            CultureInfo.CurrentCulture,
            ParentTimeline?.CurrentItemAriaLabelFormat ?? "{0}, item {1} de {2}",
            !string.IsNullOrWhiteSpace(Title) ? Title : Timestamp ?? Text ?? (ParentTimeline?.CurrentDefaultItemText ?? "Evento"),
            Index + 1,
            Math.Max(ParentTimeline?.ItemCount ?? 0, 1));

        private string ItemClass =>
            Join(
                "xf-timeline-item relative",
                IsHorizontal
                    ? "grid min-w-64 grid-rows-[auto_auto_1fr] justify-items-center gap-3 px-3"
                    : IsOpposite ? "grid grid-cols-[minmax(0,1fr)_auto_minmax(0,1fr)] gap-x-4" : "grid grid-cols-[auto_minmax(0,1fr)] gap-x-4",
                Class);

        private string AxisClass =>
            Join(
                "relative flex justify-center",
                IsHorizontal ? "h-8 w-full items-center" : "w-8");

        private string PointClass =>
            Join(
                "relative z-10 inline-flex items-center justify-center rounded-full border bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.12)]",
                SizeClass,
                ToneClass);

        private string ConnectorClass =>
            IsHorizontal
                ? "absolute left-1/2 top-1/2 h-px w-full translate-y-[-50%] bg-border-light"
                : "absolute left-1/2 top-8 h-full w-px -translate-x-1/2 bg-border-light";

        private string ContentClass =>
            Join(
                "min-w-0 pb-6",
                IsHorizontal ? "w-full text-center" : "",
                IsOpposite ? "col-start-1 row-start-1 text-right" : "");

        private string OppositeClass =>
            Join(
                "min-w-0 pb-6 text-sm text-text-secondary",
                IsHorizontal ? "row-start-1 text-center" : "col-start-3 row-start-1");

        private string AxisPlacementClass =>
            IsOpposite ? "col-start-2 row-start-1" : "";

        private string TitleClass => "text-sm font-semibold leading-6 text-text-primary";
        private string TimestampClass => "text-xs font-medium leading-5 text-text-secondary";
        private string TextClass => "text-sm leading-6 text-text-secondary";

        private string SizeClass => ResolvedSize switch
        {
            XFSize.Sm => "h-6 w-6 text-xs",
            XFSize.Lg => "h-10 w-10 text-base",
            _ => "h-8 w-8 text-sm"
        };

        private string ToneClass => ResolvedTone switch
        {
            XFSemanticColor.Secondary => "border-[color-mix(in_oklab,var(--color-secondary)_35%,transparent)] text-text-secondary ring-4 ring-surface-100",
            XFSemanticColor.Success => "border-[color-mix(in_oklab,var(--color-success)_45%,transparent)] text-success ring-4 ring-[color-mix(in_oklab,var(--color-success)_12%,transparent)]",
            XFSemanticColor.Warning => "border-[color-mix(in_oklab,var(--color-warning)_45%,transparent)] text-warning ring-4 ring-[color-mix(in_oklab,var(--color-warning)_14%,transparent)]",
            XFSemanticColor.Danger => "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] text-danger ring-4 ring-[color-mix(in_oklab,var(--color-danger)_12%,transparent)]",
            XFSemanticColor.Neutral => "border-surface-300 text-text-primary ring-4 ring-surface-100",
            XFSemanticColor.Default => "border-border-light text-text-muted ring-4 ring-surface-100",
            _ => "border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] text-primary ring-4 ring-[color-mix(in_oklab,var(--color-primary)_12%,transparent)]"
        };

        protected override void OnInitialized()
        {
            ParentTimeline?.Register(this);
            registered = ParentTimeline is not null;
        }

        public void Dispose()
        {
            if (registered)
            {
                ParentTimeline?.Unregister(this);
            }
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
