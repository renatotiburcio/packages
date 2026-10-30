using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

using XForge.DesignSystem.Components.Layout;
namespace XForge.DesignSystem.Components.Layout
{
    public partial class XFStack : XForgeComponentBase
    {
        protected override string BaseClass => "xf-stack";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Horizontal;
        [Parameter] public XFAlignItems AlignItems { get; set; } = XFAlignItems.Normal;
        [Parameter] public XFJustifyContent JustifyContent { get; set; } = XFJustifyContent.Normal;
        [Parameter] public XFSizeToken? Gap { get; set; }
        [Parameter] public XFSizeToken? RowGap { get; set; }
        [Parameter] public XFSizeToken? ColumnGap { get; set; }
        [Parameter] public XFWrap Wrap { get; set; } = XFWrap.NoWrap;
        [Parameter] public bool Reverse { get; set; }
        [Parameter] public bool Inline { get; set; }
        [Parameter] public bool FullWidth { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ChildContent, Orientation, AlignItems, JustifyContent, Gap, RowGap, ColumnGap, Wrap, Reverse, Inline, FullWidth);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {GetDisplayClass()} {GetDirectionClass()} {GetWrapClass()} {GetAlignClass()} {GetJustifyClass()} {GetWidthClass()} {GetGapClass()}");
        }

        protected string GetComponentStyle()
        {
            var styles = Style ?? string.Empty;
            if (!string.IsNullOrEmpty(styles) && !styles.EndsWith(";"))
                styles += ";";

            if (Gap.HasValue && !string.IsNullOrEmpty(Gap.Value.Value) && string.IsNullOrEmpty(GetGapClass()))
                styles += $"gap: {Gap.Value.Value}px;";

            if (RowGap.HasValue && !string.IsNullOrEmpty(RowGap.Value.Value))
                styles += $"row-gap: {RowGap.Value.Value}px;";

            if (ColumnGap.HasValue && !string.IsNullOrEmpty(ColumnGap.Value.Value))
                styles += $"column-gap: {ColumnGap.Value.Value}px;";

            return styles;
        }

        private string GetDisplayClass()
        {
            return Inline ? "inline-flex" : "flex";
        }

        private string GetDirectionClass()
        {
            return (Orientation, Reverse) switch
            {
                (XFOrientation.Vertical, true) => "flex-col-reverse",
                (XFOrientation.Vertical, false) => "flex-col",
                (XFOrientation.Horizontal, true) => "flex-row-reverse",
                _ => "flex-row"
            };
        }

        private string GetWrapClass()
        {
            return Wrap switch
            {
                XFWrap.Wrap => "flex-wrap",
                XFWrap.WrapReverse => "flex-wrap-reverse",
                _ => "flex-nowrap"
            };
        }

        private string GetAlignClass()
        {
            return AlignItems switch
            {
                XFAlignItems.Start => "items-start",
                XFAlignItems.Center => "items-center",
                XFAlignItems.End => "items-end",
                XFAlignItems.Stretch => "items-stretch",
                XFAlignItems.Baseline => "items-baseline",
                _ => ""
            };
        }

        private string GetJustifyClass()
        {
            return JustifyContent switch
            {
                XFJustifyContent.Start => "justify-start",
                XFJustifyContent.End => "justify-end",
                XFJustifyContent.Center => "justify-center",
                XFJustifyContent.SpaceBetween => "justify-between",
                XFJustifyContent.SpaceAround => "justify-around",
                XFJustifyContent.SpaceEvenly => "justify-evenly",
                _ => ""
            };
        }

        private string GetWidthClass()
        {
            return FullWidth ? "w-full" : "";
        }

        private string GetGapClass()
        {
            if (!Gap.HasValue)
                return "";

            return Gap.Value.Value switch
            {
                "0" => "gap-0",
                "1" => "gap-1",
                "2" => "gap-2",
                "3" => "gap-3",
                "4" => "gap-4",
                "5" => "gap-5",
                "6" => "gap-6",
                "8" => "gap-8",
                "10" => "gap-10",
                "12" => "gap-12",
                _ => ""
            };
        }
    }
}
