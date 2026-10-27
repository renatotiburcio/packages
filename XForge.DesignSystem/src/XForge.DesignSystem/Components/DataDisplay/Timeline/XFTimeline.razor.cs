using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Components.Layout;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Timeline
{
    public partial class XFTimeline : XForgeComponentBase
    {
        private readonly List<XFTimelineItem> items = new();

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string DefaultAriaLabel { get; set; } = "Linha do tempo";
        [Parameter] public string ItemAriaLabelFormat { get; set; } = "{0}, item {1} de {2}";
        [Parameter] public string DefaultItemText { get; set; } = "Evento";
        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Vertical;
        [Parameter] public XFTimelineAlign Align { get; set; } = XFTimelineAlign.Start;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Primary;
        [Parameter] public bool Reverse { get; set; }
        [Parameter] public bool Alternating { get; set; }
        [Parameter] public bool ShowConnector { get; set; } = true;

        internal XFOrientation CurrentOrientation => Orientation;
        internal XFTimelineAlign CurrentAlign => Alternating ? XFTimelineAlign.Alternate : Align;
        internal XFSize CurrentSize => Size;
        internal XFSemanticColor CurrentTone => Tone;
        internal bool HasConnector => ShowConnector;
        internal int ItemCount => items.Count;
        internal string CurrentItemAriaLabelFormat => ItemAriaLabelFormat;
        internal string CurrentDefaultItemText => DefaultItemText;
        private string ResolvedAriaLabel => AriaLabel ?? DefaultAriaLabel;

        private string RootClass =>
            Join(
                "xf-timeline",
                Orientation == XFOrientation.Horizontal
                    ? "flex w-full gap-0 overflow-x-auto pb-2"
                    : "grid gap-0",
                Reverse ? (Orientation == XFOrientation.Horizontal ? "flex-row-reverse" : "[&>*]:[direction:ltr]") : "",
                Class);

        internal void Register(XFTimelineItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                _ = InvokeAsync(StateHasChanged);
            }
        }

        internal void Unregister(XFTimelineItem item)
        {
            if (items.Remove(item))
            {
                _ = InvokeAsync(StateHasChanged);
            }
        }

        internal int GetIndex(XFTimelineItem item)
        {
            return items.IndexOf(item);
        }

        internal bool IsLast(XFTimelineItem item)
        {
            return GetIndex(item) == items.Count - 1;
        }

        internal bool IsOpposite(int index)
        {
            return CurrentAlign == XFTimelineAlign.End
                || (CurrentAlign == XFTimelineAlign.Alternate && index % 2 == 1);
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
