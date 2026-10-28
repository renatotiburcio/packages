using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Components.Layout;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Navigation.Toolbar
{
    public partial class XFToolbar : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private string? focusedId;

        [Parameter] public IReadOnlyList<XFToolbarItem> Items { get; set; } = Array.Empty<XFToolbarItem>();
        [Parameter] public IReadOnlyList<XFToolbarItem> EndItems { get; set; } = Array.Empty<XFToolbarItem>();
        [Parameter] public RenderFragment? StartContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? EndContent { get; set; }
        [Parameter] public RenderFragment<XFToolbarItem>? ItemTemplate { get; set; }
        [Parameter] public EventCallback<XFToolbarItemEventArgs> ItemClicked { get; set; }
        [Parameter] public XFOrientation Orientation { get; set; }
        [Parameter] public XFToolbarVariant Variant { get; set; } = XFToolbarVariant.Surface;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public bool Wrap { get; set; } = true;
        [Parameter] public bool Dense { get; set; }
        [Parameter] public string? AriaLabel { get; set; } = "Barra de ferramentas";

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("toolbar.label", AriaLabel);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            focusedId ??= FlattenActions().FirstOrDefault(item => !item.Disabled)?.Id;
        }

        private string OrientationValue => Orientation == XFOrientation.Vertical ? "vertical" : "horizontal";

        private string RootClass => Join(
            "xf-toolbar",
            Orientation == XFOrientation.Vertical ? "inline-flex flex-col items-stretch" : "flex items-center",
            Wrap && Orientation == XFOrientation.Horizontal ? "flex-wrap" : "",
            Dense ? "gap-1" : "gap-2",
            VariantClass,
            Class);

        private string VariantClass => Variant switch
        {
            XFToolbarVariant.Plain => "bg-transparent",
            XFToolbarVariant.Soft => "rounded-lg bg-surface-50 p-1 ring-1 ring-border-light",
            XFToolbarVariant.Bordered => "rounded-lg border border-border bg-bg-surface p-1 shadow-[0_1px_2px_rgb(15_23_42/.04)]",
            _ => "rounded-lg border border-border-light bg-bg-surface p-1 shadow-[0_1px_2px_rgb(15_23_42/.04)]"
        };

        private string SectionClass => Join(
            Orientation == XFOrientation.Vertical ? "grid gap-1" : "flex items-center",
            Dense ? "gap-1" : "gap-1.5");

        private string EndSectionClass => Join(
            SectionClass,
            Orientation == XFOrientation.Vertical ? "mt-auto pt-1" : "ml-auto");

        private string ItemClass(XFToolbarItem item)
        {
            var sizeClass = Size switch
            {
                XFSize.Sm => HasText(item) ? "min-h-8 px-2.5 text-xs" : "h-8 w-8",
                XFSize.Lg => HasText(item) ? "min-h-11 px-4 text-base" : "h-11 w-11",
                _ => HasText(item) ? "min-h-9 px-3 text-sm" : "h-9 w-9"
            };

            return Join(
                "inline-flex shrink-0 items-center justify-center gap-2 rounded-md border border-transparent font-semibold outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2",
                sizeClass,
                item.Active
                    ? "bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] text-primary"
                    : "text-text-secondary hover:bg-surface-100 hover:text-text-primary",
                item.Disabled || Disabled ? "pointer-events-none cursor-not-allowed opacity-50" : "cursor-pointer");
        }

        private string IconClass => Size == XFSize.Sm
            ? "inline-flex h-4 w-4 shrink-0 items-center justify-center"
            : "inline-flex h-5 w-5 shrink-0 items-center justify-center";

        private string SeparatorClass => Orientation == XFOrientation.Vertical
            ? "my-1 h-px bg-border-light"
            : "mx-1 h-6 w-px bg-border-light";

        private bool HasText(XFToolbarItem item) => !string.IsNullOrWhiteSpace(item.Text);

        private static string? GetItemAriaLabel(XFToolbarItem item) =>
            !string.IsNullOrWhiteSpace(item.AriaLabel)
                ? item.AriaLabel
                : string.IsNullOrWhiteSpace(item.Text) ? item.Title : null;

        private async Task HandleItemClickAsync(XFToolbarItem item)
        {
            if (Disabled || item.Disabled || item.Kind != XFToolbarItemKind.Action)
            {
                return;
            }

            focusedId = item.Id;
            await ItemClicked.InvokeAsync(new XFToolbarItemEventArgs(item, item.Id, item.Value, item.Href));
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            var actions = FlattenActions().Where(item => !item.Disabled).ToArray();
            if (actions.Length == 0)
            {
                return;
            }

            var current = actions.FirstOrDefault(item => item.Id == focusedId) ?? actions[0];
            var index = Array.IndexOf(actions, current);
            var nextKey = Orientation == XFOrientation.Vertical ? "ArrowDown" : "ArrowRight";
            var previousKey = Orientation == XFOrientation.Vertical ? "ArrowUp" : "ArrowLeft";

            if (args.Key == nextKey)
            {
                focusedId = actions[(index + 1) % actions.Length].Id;
            }
            else if (args.Key == previousKey)
            {
                focusedId = actions[(index - 1 + actions.Length) % actions.Length].Id;
            }
            else if (args.Key == "Home")
            {
                focusedId = actions[0].Id;
            }
            else if (args.Key == "End")
            {
                focusedId = actions[^1].Id;
            }
            else if (args.Key is "Enter" or " ")
            {
                await HandleItemClickAsync(current);
            }
        }

        private IEnumerable<XFToolbarItem> FlattenActions() =>
            Items.Concat(EndItems)
                .SelectMany(item => Flatten([item]))
                .Where(item => item.Kind == XFToolbarItemKind.Action);

        private static IEnumerable<XFToolbarItem> Flatten(IEnumerable<XFToolbarItem> items)
        {
            foreach (var item in items)
            {
                yield return item;
                foreach (var child in Flatten(item.Children))
                {
                    yield return child;
                }
            }
        }

        private RenderFragment RenderItems(IReadOnlyList<XFToolbarItem> items) => builder =>
        {
            var sequence = 0;
            foreach (var item in items)
            {
                builder.AddContent(sequence++, RenderItem(item));
            }
        };

        private RenderFragment RenderItem(XFToolbarItem item) => builder =>
        {
            var sequence = 0;

            if (item.Kind == XFToolbarItemKind.Separator)
            {
                builder.OpenElement(sequence++, "span");
                builder.AddAttribute(sequence++, "class", SeparatorClass);
                builder.AddAttribute(sequence++, "role", "separator");
                builder.AddAttribute(sequence++, "aria-orientation", OrientationValue);
                builder.CloseElement();
                return;
            }

            if (item.Kind == XFToolbarItemKind.Group)
            {
                builder.OpenElement(sequence++, "div");
                builder.AddAttribute(sequence++, "class", SectionClass);
                builder.AddAttribute(sequence++, "role", "group");
                if (!string.IsNullOrWhiteSpace(item.AriaLabel ?? item.Text))
                {
                    builder.AddAttribute(sequence++, "aria-label", item.AriaLabel ?? item.Text);
                }
                builder.AddContent(sequence++, RenderItems(item.Children));
                builder.CloseElement();
                return;
            }

            builder.OpenElement(sequence++, item.Href is null ? "button" : "a");
            builder.AddAttribute(sequence++, "class", ItemClass(item));
            builder.AddAttribute(sequence++, "role", "button");
            builder.AddAttribute(sequence++, "aria-disabled", (Disabled || item.Disabled) ? "true" : null);
            builder.AddAttribute(sequence++, "aria-pressed", item.Active ? "true" : null);
            builder.AddAttribute(sequence++, "aria-label", GetItemAriaLabel(item));
            builder.AddAttribute(sequence++, "title", item.Title ?? item.AriaLabel ?? item.Text);
            builder.AddAttribute(sequence++, "tabindex", focusedId == item.Id ? "0" : "-1");
            builder.AddAttribute(sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => HandleItemClickAsync(item)));

            if (item.Href is null)
            {
                builder.AddAttribute(sequence++, "type", "button");
            }
            else
            {
                builder.AddAttribute(sequence++, "href", item.Disabled || Disabled ? null : item.Href);
                builder.AddAttribute(sequence++, "target", item.Target);
            }

            if (item.Template is not null)
            {
                builder.AddContent(sequence++, item.Template(item));
            }
            else if (ItemTemplate is not null)
            {
                builder.AddContent(sequence++, ItemTemplate(item));
            }
            else
            {
                if (item.Icon is not null)
                {
                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", IconClass);
                    builder.AddAttribute(sequence++, "aria-hidden", "true");
                    builder.AddContent(sequence++, item.Icon);
                    builder.CloseElement();
                }

                if (!string.IsNullOrWhiteSpace(item.Text))
                {
                    builder.OpenElement(sequence++, "span");
                    builder.AddContent(sequence++, item.Text);
                    builder.CloseElement();
                }
            }

            builder.CloseElement();
        };

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
