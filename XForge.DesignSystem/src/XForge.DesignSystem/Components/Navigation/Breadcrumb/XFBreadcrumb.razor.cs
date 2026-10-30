using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Navigation.Breadcrumb
{
    public partial class XFBreadcrumb : XForgeComponentBase
    {
        protected override string BaseClass => "xf-breadcrumb";

        [Parameter] public IReadOnlyList<XFBreadcrumbItemModel> Items { get; set; } = Array.Empty<XFBreadcrumbItemModel>();
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Separator { get; set; }
        [Parameter] public RenderFragment<XFBreadcrumbItemModel>? ItemTemplate { get; set; }
        [Parameter] public string AriaLabel { get; set; } = "Breadcrumb";
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFContainerVariant Variant { get; set; } = XFContainerVariant.Plain;
        [Parameter] public int MaxVisibleItems { get; set; } = 4;
        [Parameter] public bool CollapseMiddle { get; set; } = true;
        [Parameter] public string CollapsedSingleLabelFormat { get; set; } = "{0} hidden item";
        [Parameter] public string CollapsedMultipleLabelFormat { get; set; } = "{0} hidden items";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Items, ChildContent, Separator, ItemTemplate, AriaLabel, Size, Variant, MaxVisibleItems, CollapseMiddle, CollapsedSingleLabelFormat, CollapsedMultipleLabelFormat);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {VariantClass}");
        }

        private IEnumerable<XFBreadcrumbItemModel> GetVisibleItems()
        {
            if (!CollapseMiddle || MaxVisibleItems < 3 || Items.Count <= MaxVisibleItems)
            {
                return Items;
            }

            var tailCount = MaxVisibleItems - 1;
            return Items.Take(1).Concat(Items.Skip(Items.Count - tailCount));
        }

        private RenderFragment RenderItem(XFBreadcrumbItemModel item) => builder =>
        {
            var template = item.Template ?? ItemTemplate;
            if (template is not null)
            {
                builder.AddContent(0, template(item));
                return;
            }

            builder.OpenElement(1, string.IsNullOrWhiteSpace(item.Href) || item.Current ? "span" : "a");

            if (!string.IsNullOrWhiteSpace(item.Href) && !item.Current)
            {
                builder.AddAttribute(2, "href", item.Href);
            }

            if (item.Current)
            {
                builder.AddAttribute(3, "aria-current", "page");
            }

            builder.AddAttribute(4, "class", GetItemClass(item));

            if (item.Icon is not null)
            {
                builder.OpenElement(5, "span");
                builder.AddAttribute(6, "class", "inline-flex shrink-0 items-center justify-center");
                builder.AddContent(7, item.Icon);
                builder.CloseElement();
            }

            builder.OpenElement(8, "span");
            builder.AddAttribute(9, "class", "truncate");
            builder.AddContent(10, item.Text);
            builder.CloseElement();
            builder.CloseElement();
        };

        private RenderFragment RenderSeparator() => builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", SeparatorClass);
            builder.AddAttribute(2, "aria-hidden", "true");

            if (Separator is not null)
            {
                builder.AddContent(3, Separator);
            }
            else
            {
                builder.OpenElement(4, "svg");
                builder.AddAttribute(5, "class", "h-3.5 w-3.5");
                builder.AddAttribute(6, "viewBox", "0 0 16 16");
                builder.AddAttribute(7, "fill", "none");
                builder.AddAttribute(8, "stroke", "currentColor");
                builder.AddAttribute(9, "stroke-width", "2");
                builder.AddAttribute(10, "stroke-linecap", "round");
                builder.AddAttribute(11, "stroke-linejoin", "round");
                builder.OpenElement(12, "path");
                builder.AddAttribute(13, "d", "M6 4l4 4-4 4");
                builder.CloseElement();
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private string ListClass => "flex min-w-0 flex-wrap items-center gap-1";

        private string ItemShellClass => "inline-flex min-w-0 items-center gap-1";

        private string CollapsedClass => "inline-flex h-7 min-w-7 items-center justify-center rounded-md bg-surface-100 px-2 text-text-muted ring-1 ring-border-light";

        private string GetCollapsedLabel(int collapsedCount)
        {
            var format = collapsedCount == 1
                ? CollapsedSingleLabelFormat
                : CollapsedMultipleLabelFormat;

            return string.Format(CultureInfo.CurrentCulture, format, collapsedCount);
        }

        private string SeparatorClass => Size switch
        {
            XFSize.Sm => "inline-flex h-5 w-5 shrink-0 items-center justify-center text-text-muted",
            XFSize.Lg => "inline-flex h-7 w-7 shrink-0 items-center justify-center text-text-muted",
            _ => "inline-flex h-6 w-6 shrink-0 items-center justify-center text-text-muted"
        };

        private string VariantClass => Variant switch
        {
            XFContainerVariant.Soft => "rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] px-2 py-1",
            XFContainerVariant.Surface => "rounded-lg border border-border-light bg-bg-surface px-2 py-1 shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            _ => ""
        };

        private string GetItemClass(XFBreadcrumbItemModel item)
        {
            var sizeClass = Size switch
            {
                XFSize.Sm => "h-7 px-2 text-xs",
                XFSize.Lg => "h-9 px-3 text-base",
                _ => "h-8 px-2.5 text-sm"
            };

            var stateClass = item.Current
                ? "cursor-default text-text-primary"
                : "text-text-secondary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_40%,transparent)] focus-visible:ring-offset-2";

            return $"inline-flex min-w-0 max-w-48 items-center gap-1.5 rounded-md font-medium outline-none transition-all duration-150 {sizeClass} {stateClass}";
        }
    }
}
