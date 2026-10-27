using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Components.Layout;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Navigation.Menu
{
    public partial class XFMenu : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private readonly HashSet<string> expandedIds = new(StringComparer.Ordinal);
        private string? selectedId;
        private string? focusedId;
        private bool initialized;

        [Parameter] public IReadOnlyList<XFMenuItem> Items { get; set; } = Array.Empty<XFMenuItem>();
        [Parameter] public IReadOnlyList<string> ExpandedIds { get; set; } = Array.Empty<string>();
        [Parameter] public EventCallback<IReadOnlyList<string>> ExpandedIdsChanged { get; set; }
        [Parameter] public string? SelectedId { get; set; }
        [Parameter] public EventCallback<string?> SelectedIdChanged { get; set; }
        [Parameter] public EventCallback<XFMenuItemEventArgs> ItemClicked { get; set; }
        [Parameter] public RenderFragment<XFMenuItem>? ItemTemplate { get; set; }
        [Parameter] public XFOrientation Orientation { get; set; } = XFOrientation.Vertical;
        [Parameter] public XFMenuVariant Variant { get; set; } = XFMenuVariant.Surface;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFMenuDisplayMode DisplayMode { get; set; } = XFMenuDisplayMode.IconAndText;
        [Parameter] public XFSemanticColor ActiveTone { get; set; } = XFSemanticColor.Primary;
        [Parameter] public bool Multiple { get; set; } = true;
        [Parameter] public bool Collapsible { get; set; }
        [Parameter] public bool Expanded { get; set; } = true;
        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }
        [Parameter] public bool ShowArrows { get; set; } = true;
        [Parameter] public bool ExpandOnItemClick { get; set; } = true;
        [Parameter] public bool SelectOnItemClick { get; set; } = true;
        [Parameter] public string? AriaLabel { get; set; } = "Menu";
        [Parameter] public string EmptyText { get; set; } = "Nenhum item para exibir.";
        [Parameter] public string ToggleLabel { get; set; } = "Alternar menu";
        [Parameter] public string? ToggleExpandedLabel { get; set; }
        [Parameter] public string? ToggleCollapsedLabel { get; set; }

        protected override void OnParametersSet()
        {
            if (!initialized || ExpandedIds.Count > 0 || ExpandedIdsChanged.HasDelegate)
            {
                expandedIds.Clear();
                foreach (var id in ExpandedIds.Where(id => !string.IsNullOrWhiteSpace(id)))
                {
                    expandedIds.Add(id);
                }

                AddInitiallyExpanded(Items);
            }

            selectedId = SelectedId ?? selectedId ?? Flatten(Items).FirstOrDefault(item => item.Selected)?.Id;
            focusedId ??= FlattenVisible(Items).FirstOrDefault(item => !item.Disabled)?.Id;
            initialized = true;
        }

        protected override void OnInitialized()
        {
            EmptyText = L("menu.empty", EmptyText);
            ToggleLabel = L("menu.toggle", ToggleLabel);
            base.OnInitialized();
        }

        private string RootClass =>
            Join(
                "xf-menu",
                Orientation == XFOrientation.Horizontal ? "flex items-center gap-2" : "grid gap-1",
                VariantClass,
                !Expanded && Collapsible ? "w-11 min-w-11" : "",
                Class);

        private string VariantClass => Variant switch
        {
            XFMenuVariant.Plain => "",
            XFMenuVariant.Soft => "rounded-lg bg-surface-50 p-1 ring-1 ring-border-light",
            XFMenuVariant.TopNav => "rounded-none bg-transparent p-0",
            XFMenuVariant.Dropdown => "rounded-lg border border-border-light bg-bg-surface p-2 shadow-[0_18px_45px_rgb(15_23_42/.14)]",
            XFMenuVariant.Sidebar => "bg-transparent p-0",
            XFMenuVariant.DarkSidebar => "bg-surface-950 p-2 text-white",
            _ => "rounded-lg border border-border-light bg-bg-surface p-1 shadow-[0_1px_2px_rgb(15_23_42/.04)]"
        };

        private string EmptyClass => "px-4 py-6 text-center text-sm text-text-secondary";
        private string EffectiveToggleLabel => Expanded
            ? ToggleExpandedLabel ?? ToggleLabel
            : ToggleCollapsedLabel ?? ToggleLabel;

        private string ItemBaseClass =>
            Join(
                "group relative flex w-full items-center gap-3 text-left outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2",
                ItemRadiusClass,
                SizeClass);

        private string ItemRadiusClass => Variant switch
        {
            XFMenuVariant.TopNav => "rounded-md",
            XFMenuVariant.Dropdown => "rounded-md",
            XFMenuVariant.Sidebar or XFMenuVariant.DarkSidebar => "rounded-md",
            _ => "rounded-md"
        };

        private string SizeClass => Size switch
        {
            XFSize.Sm => "min-h-8 px-3 py-1.5 text-sm",
            XFSize.Lg => "min-h-12 px-4 py-3 text-base",
            _ => "min-h-10 px-4 py-2.5 text-sm"
        };

        private string ChildrenClass(int level) =>
            Orientation == XFOrientation.Horizontal && level == 1
                ? "absolute left-0 top-full z-30 mt-3 min-w-64 rounded-lg border border-border-light bg-bg-surface p-3 shadow-[0_22px_50px_rgb(15_23_42/.16)]"
                : Variant == XFMenuVariant.DarkSidebar
                    ? "grid gap-1 py-1 pl-7"
                    : Variant == XFMenuVariant.Sidebar
                        ? "grid gap-1 py-1 pl-7"
                        : "grid gap-1 py-1 pl-4";

        private string GetItemClass(XFMenuItem item)
        {
            var selected = IsSelected(item);
            var interactive = item.Disabled ? "cursor-not-allowed opacity-50" : "cursor-pointer";
            var focused = focusedId == item.Id ? "focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]" : "";
            var activeTone = item.ActiveTone ?? ActiveTone;

            var palette = Variant switch
            {
                XFMenuVariant.TopNav => selected
                    ? ActiveItemClass(activeTone, true)
                    : "text-text-secondary hover:bg-bg-surface hover:text-primary hover:shadow-[0_10px_24px_rgb(15_23_42/.08)]",
                XFMenuVariant.Dropdown => selected
                    ? ActiveItemClass(activeTone)
                    : "text-text-secondary hover:bg-surface-50 hover:text-primary",
                XFMenuVariant.Sidebar => selected
                    ? ActiveItemClass(activeTone)
                    : "text-text-secondary hover:bg-surface-50 hover:text-text-primary",
                XFMenuVariant.DarkSidebar => selected
                    ? DarkActiveItemClass(activeTone)
                    : "text-white/68 hover:bg-white/7 hover:text-white",
                _ => selected
                    ? ActiveItemClass(activeTone)
                    : "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)]"
            };

            return Join(ItemBaseClass, interactive, palette, focused);
        }

        private string SectionClass => Variant == XFMenuVariant.DarkSidebar
            ? "px-3 pb-2 pt-5 text-xs font-semibold uppercase tracking-wide text-white/55 first:pt-2"
            : "px-4 pb-2 pt-5 text-xs font-semibold uppercase tracking-wide text-text-muted first:pt-2";

        private string SeparatorClass => Variant == XFMenuVariant.DarkSidebar
            ? "my-2 h-px bg-white/10"
            : "my-2 h-px bg-border-light";

        private string IconClass => Variant switch
        {
            XFMenuVariant.DarkSidebar => "inline-flex h-5 w-5 shrink-0 items-center justify-center text-white/45 group-hover:text-white/80",
            XFMenuVariant.Dropdown => "inline-flex h-5 w-5 shrink-0 items-center justify-center text-text-muted group-hover:text-primary",
            XFMenuVariant.Sidebar => "inline-flex h-5 w-5 shrink-0 items-center justify-center text-text-muted group-hover:text-text-secondary",
            _ => "inline-flex h-5 w-5 shrink-0 items-center justify-center text-current"
        };

        private string DescriptionClass => Variant == XFMenuVariant.DarkSidebar
            ? "block truncate text-xs text-white/45"
            : "block truncate text-xs text-text-muted";

        private string ArrowClass(bool expanded) => Variant == XFMenuVariant.DarkSidebar
            ? Join("ml-auto inline-flex h-3 w-3 shrink-0 items-center justify-center text-white/45 transition-transform duration-150", expanded ? "rotate-90" : "")
            : Join("ml-auto inline-flex h-3 w-3 shrink-0 items-center justify-center text-text-muted transition-transform duration-150", expanded ? "rotate-90" : "");

        private string BadgeClass(XFMenuItem item)
        {
            return item.BadgeTone switch
            {
                XFSemanticColor.Primary => "ml-auto rounded-full bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] px-2 py-0.5 text-xs font-semibold text-primary",
                XFSemanticColor.Secondary => "ml-auto rounded-full bg-[color-mix(in_oklab,var(--color-secondary)_12%,transparent)] px-2 py-0.5 text-xs font-semibold text-secondary",
                XFSemanticColor.Success => "ml-auto rounded-full bg-[color-mix(in_oklab,var(--color-success)_12%,transparent)] px-2 py-0.5 text-xs font-semibold text-success",
                XFSemanticColor.Warning => "ml-auto rounded-full bg-[color-mix(in_oklab,var(--color-warning)_14%,transparent)] px-2 py-0.5 text-xs font-semibold text-warning",
                XFSemanticColor.Danger => "ml-auto rounded-full bg-[color-mix(in_oklab,var(--color-danger)_12%,transparent)] px-2 py-0.5 text-xs font-semibold text-danger",
                _ when Variant == XFMenuVariant.DarkSidebar => "ml-auto rounded-full bg-white/10 px-2 py-0.5 text-xs font-semibold text-white/65",
                _ => "ml-auto rounded-full bg-surface-100 px-2 py-0.5 text-xs font-semibold text-text-muted"
            };
        }

        private static string ActiveItemClass(XFSemanticColor tone, bool elevated = false)
        {
            var color = tone switch
            {
                XFSemanticColor.Secondary => "bg-[color-mix(in_oklab,var(--color-secondary)_12%,transparent)] text-secondary",
                XFSemanticColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_12%,transparent)] text-success",
                XFSemanticColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_14%,transparent)] text-warning",
                XFSemanticColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_12%,transparent)] text-danger",
                XFSemanticColor.Neutral => "bg-surface-100 text-text-primary",
                _ => "bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] text-primary"
            };

            return Join(color, elevated ? "shadow-[0_10px_24px_rgb(15_23_42/.08)]" : null);
        }

        private static string DarkActiveItemClass(XFSemanticColor tone)
        {
            return tone switch
            {
                XFSemanticColor.Secondary => "bg-secondary text-secondary-contrast shadow-[0_10px_22px_rgb(15_23_42/.18)]",
                XFSemanticColor.Success => "bg-success text-success-contrast shadow-[0_10px_22px_rgb(15_23_42/.18)]",
                XFSemanticColor.Warning => "bg-warning text-warning-contrast shadow-[0_10px_22px_rgb(15_23_42/.18)]",
                XFSemanticColor.Danger => "bg-danger text-danger-contrast shadow-[0_10px_22px_rgb(15_23_42/.18)]",
                XFSemanticColor.Neutral => "bg-white/12 text-white shadow-[0_10px_22px_rgb(15_23_42/.18)]",
                _ => "bg-primary text-primary-contrast shadow-[0_10px_22px_rgb(37_99_235/.24)]"
            };
        }

        private bool HasChildren(XFMenuItem item) => item.Children.Count > 0;

        private bool IsExpanded(XFMenuItem item) => expandedIds.Contains(item.Id);

        private bool IsSelected(XFMenuItem item) => string.Equals(selectedId, item.Id, StringComparison.Ordinal) || item.Selected;

        private async Task ToggleExpandedAsync()
        {
            Expanded = !Expanded;
            await ExpandedChanged.InvokeAsync(Expanded);
        }

        private async Task HandleItemClickAsync(XFMenuItem item)
        {
            if (item.Disabled)
            {
                return;
            }

            focusedId = item.Id;

            if (SelectOnItemClick)
            {
                selectedId = item.Id;
                await SelectedIdChanged.InvokeAsync(item.Id);
            }

            if (ExpandOnItemClick && HasChildren(item))
            {
                await ToggleItemAsync(item);
            }

            await ItemClicked.InvokeAsync(new XFMenuItemEventArgs(item, item.Id, item.Value, item.Href));
        }

        private async Task ToggleItemAsync(XFMenuItem item)
        {
            if (!HasChildren(item) || item.Disabled)
            {
                return;
            }

            if (expandedIds.Contains(item.Id))
            {
                expandedIds.Remove(item.Id);
            }
            else
            {
                if (!Multiple)
                {
                    CollapseSiblings(item);
                }

                expandedIds.Add(item.Id);
            }

            await ExpandedIdsChanged.InvokeAsync(expandedIds.ToArray());
        }

        private void CollapseSiblings(XFMenuItem item)
        {
            var parentItems = FindSiblings(Items, item.Id) ?? Items;
            foreach (var sibling in parentItems)
            {
                if (sibling.Id != item.Id)
                {
                    expandedIds.Remove(sibling.Id);
                }
            }
        }

        private static IReadOnlyList<XFMenuItem>? FindSiblings(IReadOnlyList<XFMenuItem> items, string id)
        {
            if (items.Any(item => item.Id == id))
            {
                return items;
            }

            foreach (var item in items)
            {
                var found = FindSiblings(item.Children, id);
                if (found is not null)
                {
                    return found;
                }
            }

            return null;
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            var visible = FlattenVisible(Items).Where(item => !item.Disabled).ToArray();
            if (visible.Length == 0)
            {
                return;
            }

            var current = visible.FirstOrDefault(item => item.Id == focusedId) ?? visible[0];
            var index = Array.IndexOf(visible, current);

            if (args.Key is "ArrowDown" || (Orientation == XFOrientation.Horizontal && args.Key == "ArrowRight"))
            {
                focusedId = visible[Math.Min(visible.Length - 1, index + 1)].Id;
            }
            else if (args.Key is "ArrowUp" || (Orientation == XFOrientation.Horizontal && args.Key == "ArrowLeft"))
            {
                focusedId = visible[Math.Max(0, index - 1)].Id;
            }
            else if (args.Key == "Home")
            {
                focusedId = visible[0].Id;
            }
            else if (args.Key == "End")
            {
                focusedId = visible[^1].Id;
            }
            else if (args.Key == "ArrowRight" && HasChildren(current) && !IsExpanded(current))
            {
                await ToggleItemAsync(current);
            }
            else if (args.Key == "ArrowLeft" && HasChildren(current) && IsExpanded(current))
            {
                await ToggleItemAsync(current);
            }
            else if (args.Key is "Enter" or " ")
            {
                await HandleItemClickAsync(current);
            }
        }

        private RenderFragment RenderItems(IReadOnlyList<XFMenuItem> items, int level) => builder =>
        {
            var sequence = 0;
            foreach (var item in items)
            {
                if (item.Kind != XFMenuItemKind.Item)
                {
                    builder.AddContent(sequence++, RenderUtilityItem(item));
                    continue;
                }

                builder.AddContent(sequence++, RenderItem(item, level));
            }
        };

        private RenderFragment RenderUtilityItem(XFMenuItem item) => builder =>
        {
            var sequence = 0;
            if (item.Kind == XFMenuItemKind.Separator)
            {
                builder.OpenElement(sequence++, "div");
                builder.AddAttribute(sequence++, "class", SeparatorClass);
                builder.AddAttribute(sequence++, "role", "separator");
                builder.CloseElement();
                return;
            }

            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", SectionClass);
            builder.AddContent(sequence++, item.Text);
            builder.CloseElement();
        };

        private RenderFragment RenderItem(XFMenuItem item, int level) => builder =>
        {
            var sequence = 0;
            var hasChildren = HasChildren(item);
            var expanded = IsExpanded(item);
            var role = Orientation == XFOrientation.Horizontal || Variant == XFMenuVariant.Dropdown ? "menuitem" : "treeitem";

            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", Orientation == XFOrientation.Horizontal && level == 1 ? "relative" : "");

            builder.OpenElement(sequence++, item.Href is not null && !hasChildren ? "a" : "button");
            builder.AddAttribute(sequence++, "class", GetItemClass(item));
            builder.AddAttribute(sequence++, "role", role);
            builder.AddAttribute(sequence++, "aria-disabled", item.Disabled ? "true" : null);
            if (Orientation == XFOrientation.Vertical && Variant != XFMenuVariant.Dropdown)
            {
                builder.AddAttribute(sequence++, "aria-selected", IsSelected(item) ? "true" : "false");
                builder.AddAttribute(sequence++, "aria-level", level.ToString());
            }
            builder.AddAttribute(sequence++, "tabindex", focusedId == item.Id ? "0" : "-1");

            if (hasChildren)
            {
                builder.AddAttribute(sequence++, "type", "button");
                builder.AddAttribute(sequence++, "aria-expanded", expanded ? "true" : "false");
            }
            else if (item.Href is not null)
            {
                builder.AddAttribute(sequence++, "href", item.Disabled ? null : item.Href);
                builder.AddAttribute(sequence++, "target", item.Target);
            }
            else
            {
                builder.AddAttribute(sequence++, "type", "button");
            }

            builder.AddAttribute(sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => HandleItemClickAsync(item)));

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
                RenderDefaultItemContent(builder, ref sequence, item, hasChildren, expanded);
            }

            builder.CloseElement();

            if (hasChildren && expanded)
            {
                builder.OpenElement(sequence++, "div");
                builder.AddAttribute(sequence++, "class", ChildrenClass(level));
                builder.AddAttribute(sequence++, "role", "group");
                builder.AddContent(sequence++, RenderItems(item.Children, level + 1));
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private void RenderDefaultItemContent(RenderTreeBuilder builder, ref int sequence, XFMenuItem item, bool hasChildren, bool expanded)
        {
            var shouldRenderIcon = DisplayMode == XFMenuDisplayMode.IconOnly || item.Icon is not null;

            if (DisplayMode != XFMenuDisplayMode.TextOnly && shouldRenderIcon)
            {
                builder.OpenElement(sequence++, "span");
                builder.AddAttribute(sequence++, "class", IconClass);
                if (item.Icon is not null)
                {
                    builder.AddContent(sequence++, item.Icon);
                }
                else
                {
                    builder.AddContent(sequence++, "");
                }
                builder.CloseElement();
            }

            if (Expanded || DisplayMode != XFMenuDisplayMode.IconOnly)
            {
                builder.OpenElement(sequence++, "span");
                builder.AddAttribute(sequence++, "class", "min-w-0 flex-1");
                builder.OpenElement(sequence++, "span");
                builder.AddAttribute(sequence++, "class", "block truncate font-medium");
                builder.AddContent(sequence++, item.Text);
                builder.CloseElement();
                if (!string.IsNullOrWhiteSpace(item.Description))
                {
                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", DescriptionClass);
                    builder.AddContent(sequence++, item.Description);
                    builder.CloseElement();
                }
                builder.CloseElement();
            }

            if (!string.IsNullOrWhiteSpace(item.Meta))
            {
                builder.OpenElement(sequence++, "span");
                builder.AddAttribute(sequence++, "class", DescriptionClass);
                builder.AddContent(sequence++, item.Meta);
                builder.CloseElement();
            }

            if (!string.IsNullOrWhiteSpace(item.Badge))
            {
                builder.OpenElement(sequence++, "span");
                builder.AddAttribute(sequence++, "class", BadgeClass(item));
                builder.AddContent(sequence++, item.Badge);
                builder.CloseElement();
            }

            if (ShowArrows && hasChildren && Expanded)
            {
                builder.OpenElement(sequence++, "span");
                builder.AddAttribute(sequence++, "class", ArrowClass(expanded));
                builder.AddContent(sequence++, XFSvgIcons.MarkupWithSize("chevron-right", 12));
                builder.CloseElement();
            }
        }

        private void AddInitiallyExpanded(IEnumerable<XFMenuItem> items)
        {
            foreach (var item in items)
            {
                if (item.Expanded)
                {
                    expandedIds.Add(item.Id);
                }

                AddInitiallyExpanded(item.Children);
            }
        }

        private static IEnumerable<XFMenuItem> Flatten(IEnumerable<XFMenuItem> items)
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

        private IEnumerable<XFMenuItem> FlattenVisible(IEnumerable<XFMenuItem> items)
        {
            foreach (var item in items)
            {
                if (item.Kind != XFMenuItemKind.Item)
                {
                    continue;
                }

                yield return item;
                if (HasChildren(item) && IsExpanded(item))
                {
                    foreach (var child in FlattenVisible(item.Children))
                    {
                        yield return child;
                    }
                }
            }
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
