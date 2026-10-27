using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Table
{
    public partial class XFTable : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-table";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? Caption { get; set; }
        [Parameter] public string EmptyText { get; set; } = "Nenhum dado para exibir.";
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Striped { get; set; }
        [Parameter] public bool Hoverable { get; set; } = true;
        [Parameter] public bool StickyHeader { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public int? ViewportRows { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFListVariant Variant { get; set; } = XFListVariant.Bordered;

        protected override void OnInitialized()
        {
            EmptyText = L("table.empty", EmptyText);
            base.OnInitialized();
        }

        private string WrapperClass => Join("w-full overflow-auto rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.035)]", ViewportClass);

        protected override string GetComponentClass()
        {
            var width = FullWidth ? "w-full" : "w-auto";
            return GetClass($"{BaseClass} border-separate border-spacing-0 text-left text-sm {width} {VariantClass}");
        }

        private string VariantClass => Variant switch
        {
            XFListVariant.Plain => "bg-transparent",
            XFListVariant.Soft => "bg-surface-50",
            _ => "bg-bg-surface"
        };

        internal string HeaderClass => StickyHeader
            ? "sticky top-0 z-10 bg-surface-50/95 backdrop-blur supports-[backdrop-filter]:bg-surface-50/80"
            : "bg-surface-50";

        internal string RowClass => Join(
            "group transition-colors duration-150",
            Striped ? "odd:bg-bg-surface even:bg-surface-50/65" : null,
            Hoverable ? "hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]" : null);

        internal string CellBaseClass => Size switch
        {
            XFSize.Sm => "px-3 py-2",
            XFSize.Lg => "px-5 py-4",
            _ => "px-4 py-3"
        };

        internal string CellBorderClass => "border-b border-border-light last:border-r-0";

        private string CaptionClass => "caption-bottom px-4 py-3 text-left text-sm text-text-secondary";

        private string EmptyClass => $"{CellBaseClass} text-center text-sm text-text-muted";

        private string? ViewportClass => (NormalizeViewportRows(), Size) switch
        {
            (5, XFSize.Sm) => "min-h-52 max-h-52",
            (5, XFSize.Lg) => "min-h-80 max-h-80",
            (5, _) => "min-h-64 max-h-64",
            (10, XFSize.Sm) => "min-h-96 max-h-96",
            (10, XFSize.Lg) => "min-h-[42rem] max-h-[42rem]",
            (10, _) => "min-h-[34rem] max-h-[34rem]",
            (15, XFSize.Sm) => "min-h-[34rem] max-h-[34rem]",
            (15, XFSize.Lg) => "min-h-[56rem] max-h-[56rem]",
            (15, _) => "min-h-[46rem] max-h-[46rem]",
            (20, XFSize.Sm) => "min-h-[44rem] max-h-[44rem]",
            (20, XFSize.Lg) => "min-h-[72rem] max-h-[72rem]",
            (20, _) => "min-h-[60rem] max-h-[60rem]",
            _ => null
        };

        private int? NormalizeViewportRows()
        {
            if (ViewportRows is null or <= 0)
            {
                return null;
            }

            return ViewportRows <= 5 ? 5 :
                ViewportRows <= 10 ? 10 :
                ViewportRows <= 15 ? 15 :
                20;
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
