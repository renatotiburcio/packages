using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Skeleton
{
    public partial class XFSkeletonTable : XForgeComponentBase
    {
        protected override string BaseClass => "xf-skeleton-table";

        [Parameter] public int Rows { get; set; } = 4;
        [Parameter] public int Columns { get; set; } = 4;
        [Parameter] public bool ShowHeader { get; set; } = true;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Decorative { get; set; } = true;
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string DefaultAriaLabel { get; set; } = "Carregando tabela";
        [Parameter] public XFSkeletonAnimation Animation { get; set; } = XFSkeletonAnimation.Pulse;
        [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Sm;
        [Parameter] public bool Compact { get; set; }

        private int NormalizedRows => Math.Clamp(Rows, 1, 20);

        private int NormalizedColumns => Math.Clamp(Columns, 1, 8);

        private XFExtendedSize HeaderSize => Size is XFExtendedSize.Xs ? XFExtendedSize.Xs : XFExtendedSize.Sm;

        private string? Role => Decorative ? null : "status";

        private string? AriaHidden => Decorative ? "true" : null;

        private string? ResolvedAriaLabel => Decorative ? null : AriaLabel ?? DefaultAriaLabel;

        protected override string GetComponentClass()
        {
            var gap = Compact ? "gap-3" : "gap-4";
            return GetClass($"{BaseClass} grid {gap}");
        }

        private string RowClass => $"grid items-center {ColumnClass} {(Compact ? "gap-3" : "gap-4")}";

        private string ColumnClass => NormalizedColumns switch
        {
            1 => "grid-cols-1",
            2 => "grid-cols-2",
            3 => "grid-cols-3",
            4 => "grid-cols-4",
            5 => "grid-cols-5",
            6 => "grid-cols-6",
            7 => "grid-cols-7",
            _ => "grid-cols-8"
        };

        private string GetColumnWidthClass(int column)
        {
            return column switch
            {
                0 => "w-24",
                1 => "w-20",
                2 => "w-28",
                _ => "w-16"
            };
        }

        private string GetCellWidthClass(int row, int column)
        {
            var pattern = (row + column) % 4;
            return pattern switch
            {
                0 => "w-full",
                1 => "w-10/12",
                2 => "w-8/12",
                _ => "w-11/12"
            };
        }
    }
}
