using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Layout
{
    public partial class XFGrid : XForgeComponentBase
    {
        protected override string BaseClass => "xf-grid";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public int Cols { get; set; } = 1;
        [Parameter] public int? Sm { get; set; }
        [Parameter] public int? Md { get; set; }
        [Parameter] public int? Lg { get; set; }
        [Parameter] public int? Xl { get; set; }
        [Parameter] public XFSizeToken? Gap { get; set; }
        [Parameter] public XFSizeToken? RowGap { get; set; }
        [Parameter] public XFSizeToken? ColumnGap { get; set; }
        [Parameter] public XFGridFlow Flow { get; set; } = XFGridFlow.Row;
        [Parameter] public XFGridItems AlignItems { get; set; } = XFGridItems.Normal;
        [Parameter] public XFGridItems JustifyItems { get; set; } = XFGridItems.Normal;
        [Parameter] public bool FullWidth { get; set; }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} grid {GetColsClass()} {GetResponsiveColsClass()} {GetGapClass()} {GetRowGapClass()} {GetColumnGapClass()} {GetFlowClass()} {GetAlignItemsClass()} {GetJustifyItemsClass()} {GetWidthClass()}");
        }

        protected string GetComponentStyle()
        {
            var styles = string.Empty;

            if (!string.IsNullOrEmpty(Style))
            {
                styles += Style.EndsWith(";") ? Style : Style + ";";
            }

            if (Gap.HasValue && !string.IsNullOrEmpty(Gap.Value.Value) && string.IsNullOrEmpty(GetGapClass()))
            {
                styles += $"gap: {Gap.Value.Value}px;";
            }

            if (RowGap.HasValue && !string.IsNullOrEmpty(RowGap.Value.Value) && string.IsNullOrEmpty(GetRowGapClass()))
            {
                styles += $"row-gap: {RowGap.Value.Value}px;";
            }

            if (ColumnGap.HasValue && !string.IsNullOrEmpty(ColumnGap.Value.Value) && string.IsNullOrEmpty(GetColumnGapClass()))
            {
                styles += $"column-gap: {ColumnGap.Value.Value}px;";
            }

            return styles;
        }

        private string GetColsClass() => Cols switch
        {
            1 => "grid-cols-1",
            2 => "grid-cols-2",
            3 => "grid-cols-3",
            4 => "grid-cols-4",
            5 => "grid-cols-5",
            6 => "grid-cols-6",
            12 => "grid-cols-12",
            _ => "grid-cols-1"
        };

        private string GetResponsiveColsClass()
        {
            var classes = new List<string>();

            AddResponsiveClass(classes, "sm", Sm);
            AddResponsiveClass(classes, "md", Md);
            AddResponsiveClass(classes, "lg", Lg);
            AddResponsiveClass(classes, "xl", Xl);

            return string.Join(" ", classes);
        }

        private static void AddResponsiveClass(List<string> classes, string prefix, int? cols)
        {
            if (!cols.HasValue)
                return;

            var suffix = cols.Value switch
            {
                1 => "grid-cols-1",
                2 => "grid-cols-2",
                3 => "grid-cols-3",
                4 => "grid-cols-4",
                5 => "grid-cols-5",
                6 => "grid-cols-6",
                12 => "grid-cols-12",
                _ => null
            };

            if (suffix is not null)
                classes.Add($"{prefix}:{suffix}");
        }

        private string GetGapClass() => TokenToClass(Gap, "gap");
        private string GetRowGapClass() => TokenToClass(RowGap, "gap-y");
        private string GetColumnGapClass() => TokenToClass(ColumnGap, "gap-x");

        private static string TokenToClass(XFSizeToken? token, string prefix)
        {
            if (!token.HasValue)
                return "";

            return token.Value.Value switch
            {
                "0" => $"{prefix}-0",
                "1" => $"{prefix}-1",
                "2" => $"{prefix}-2",
                "3" => $"{prefix}-3",
                "4" => $"{prefix}-4",
                "5" => $"{prefix}-5",
                "6" => $"{prefix}-6",
                "8" => $"{prefix}-8",
                "10" => $"{prefix}-10",
                "12" => $"{prefix}-12",
                _ => ""
            };
        }

        private string GetFlowClass() => Flow switch
        {
            XFGridFlow.Column => "grid-flow-col",
            XFGridFlow.Dense => "grid-flow-dense",
            XFGridFlow.RowDense => "grid-flow-row-dense",
            XFGridFlow.ColumnDense => "grid-flow-col-dense",
            _ => "grid-flow-row"
        };

        private string GetAlignItemsClass() => AlignItems switch
        {
            XFGridItems.Start => "items-start",
            XFGridItems.Center => "items-center",
            XFGridItems.End => "items-end",
            XFGridItems.Stretch => "items-stretch",
            _ => ""
        };

        private string GetJustifyItemsClass() => JustifyItems switch
        {
            XFGridItems.Start => "justify-items-start",
            XFGridItems.Center => "justify-items-center",
            XFGridItems.End => "justify-items-end",
            XFGridItems.Stretch => "justify-items-stretch",
            _ => ""
        };

        private string GetWidthClass() => FullWidth ? "w-full" : "";
    }
}
