using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.DataDisplay.Table
{
    public partial class XFTableCell
    {
        [CascadingParameter] public XFTable? ParentTable { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Header { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string Scope { get; set; } = "col";
        [Parameter] public int? ColSpan { get; set; }
        [Parameter] public int? RowSpan { get; set; }
        [Parameter] public XFTableAlign Align { get; set; } = XFTableAlign.Start;
        [Parameter] public bool Sticky { get; set; }
        [Parameter] public XFTableStickySide StickySide { get; set; } = XFTableStickySide.Start;
        [Parameter] public XFTableStickyOffset StickyOffset { get; set; } = XFTableStickyOffset.None;
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

        private string? ColSpanValue => ColSpan is > 1 ? ColSpan.Value.ToString() : null;

        private string? RowSpanValue => RowSpan is > 1 ? RowSpan.Value.ToString() : null;

        private string CellClass => Join(
            ParentTable?.CellBaseClass ?? "px-4 py-3",
            ParentTable?.CellBorderClass ?? "border-b border-border-light",
            AlignClass,
            StickyClass,
            Header ? "text-xs font-semibold uppercase text-text-secondary" : "text-sm text-text-primary");

        private string AlignClass => Align switch
        {
            XFTableAlign.Center => "text-center",
            XFTableAlign.End => "text-right",
            _ => "text-left"
        };

        private string? StickyClass => Sticky
            ? Join(
                "sticky z-[2] bg-bg-surface shadow-[1px_0_0_var(--color-border-light)] group-hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,var(--color-bg-surface))]",
                Header ? "z-[12] bg-surface-50 group-hover:bg-surface-50" : null,
                StickyPositionClass)
            : null;

        private string StickyPositionClass => StickySide == XFTableStickySide.End
            ? StickyEndClass
            : StickyStartClass;

        private string StickyStartClass => StickyOffset switch
        {
            XFTableStickyOffset.Sm => "left-24",
            XFTableStickyOffset.Md => "left-32",
            XFTableStickyOffset.Lg => "left-40",
            XFTableStickyOffset.Xl => "left-56",
            _ => "left-0"
        };

        private string StickyEndClass => StickyOffset switch
        {
            XFTableStickyOffset.Sm => "right-24",
            XFTableStickyOffset.Md => "right-32",
            XFTableStickyOffset.Lg => "right-40",
            XFTableStickyOffset.Xl => "right-56",
            _ => "right-0"
        };

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
