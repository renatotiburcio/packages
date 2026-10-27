using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Components.Overlays.Popover;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Overlays.DropdownMenu
{
    public partial class XFDropdownMenu : XForgeComponentBase
    {
        private readonly HashSet<string> expandedSubmenuIds = new(StringComparer.Ordinal);

        [Parameter] public bool Open { get; set; }
        [Parameter] public EventCallback<bool> OpenChanged { get; set; }
        [Parameter] public string ButtonText { get; set; } = "Menu";
        [Parameter] public string? MenuAriaLabel { get; set; }
        [Parameter] public string SubmenuAriaLabelFormat { get; set; } = "{0}";
        [Parameter] public IReadOnlyList<XFDropdownMenuItem> Items { get; set; } = Array.Empty<XFDropdownMenuItem>();
        [Parameter] public EventCallback<XFDropdownMenuItem> ItemSelected { get; set; }
        [Parameter] public RenderFragment<XFPopoverTriggerContext>? Trigger { get; set; }
        [Parameter] public XFPopoverPlacement Placement { get; set; } = XFPopoverPlacement.Bottom;
        [Parameter] public XFPopoverAlignment Alignment { get; set; } = XFPopoverAlignment.Start;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor ActiveTone { get; set; } = XFSemanticColor.Primary;
        [Parameter] public string? MaxHeightClass { get; set; } = "max-h-80";
        [Parameter] public bool CloseOnSelect { get; set; } = true;
        [Parameter] public bool CloseOnEscape { get; set; } = true;
        [Parameter] public bool CloseOnOutsideClick { get; set; } = true;

        private int ActiveIndex { get; set; } = -1;
        private XFSize PopoverSize => Size switch
        {
            XFSize.Sm => XFSize.Sm,
            XFSize.Lg => XFSize.Lg,
            _ => XFSize.Md
        };

        private string MenuId => $"{Id}-menu";
        private string EffectiveMenuAriaLabel => string.IsNullOrWhiteSpace(MenuAriaLabel) ? ButtonText : MenuAriaLabel;

        private string? ActiveDescendantId => Open && ActiveIndex >= 0 && ActiveIndex < Items.Count ? GetItemId(ActiveIndex) : null;

        private string ButtonClass => "inline-flex h-9 items-center justify-center gap-2 rounded-lg border border-surface-200 bg-bg-surface px-3 text-sm font-semibold text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-150 hover:border-[color-mix(in_oklab,var(--color-primary)_35%,var(--color-border))] hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";
        private string DropdownChevronClass => $"transition-transform duration-150 {(Open ? "rotate-180" : string.Empty)}";
        private string MenuClass => Join(
            "flex flex-col gap-0.5",
            expandedSubmenuIds.Count > 0 ? "overflow-visible" : "overflow-y-auto",
            expandedSubmenuIds.Count > 0 ? null : MaxHeightClass,
            Class);

        protected override void OnParametersSet()
        {
            if (Open && (ActiveIndex < 0 || ActiveIndex >= Items.Count || !IsSelectable(Items[ActiveIndex])))
            {
                ActiveIndex = Items
                    .Select((item, index) => new { item, index })
                    .FirstOrDefault(entry => IsSelectable(entry.item))?.index ?? -1;
            }
        }

        private async Task HandleOpenChangedAsync(bool open)
        {
            Open = open;

            if (open)
            {
                ActiveIndex = Items
                    .Select((item, index) => new { item, index })
                    .FirstOrDefault(entry => IsSelectable(entry.item))?.index ?? -1;
            }
            else
            {
                ActiveIndex = -1;
                expandedSubmenuIds.Clear();
            }

            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(open);
            }
        }

        private async Task ActivateItemAsync(XFDropdownMenuItem item)
        {
            if (item.Disabled || item.Kind is XFDropdownMenuItemKind.Header or XFDropdownMenuItemKind.Separator)
            {
                return;
            }

            if (HasChildren(item))
            {
                ToggleSubmenu(item);
                return;
            }

            if (ItemSelected.HasDelegate)
            {
                await ItemSelected.InvokeAsync(item);
            }

            if (CloseOnSelect && item.Kind == XFDropdownMenuItemKind.Action)
            {
                await HandleOpenChangedAsync(false);
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            var selectable = Items
                .Select((item, index) => new { item, index })
                .Where(entry => IsSelectable(entry.item))
                .ToArray();

            if (selectable.Length == 0)
            {
                return;
            }

            if (args.Key == "ArrowDown")
            {
                var current = Array.FindIndex(selectable, entry => entry.index == ActiveIndex);
                ActiveIndex = selectable[(current + 1 + selectable.Length) % selectable.Length].index;
            }
            else if (args.Key == "ArrowUp")
            {
                var current = Array.FindIndex(selectable, entry => entry.index == ActiveIndex);
                ActiveIndex = selectable[(current - 1 + selectable.Length) % selectable.Length].index;
            }
            else if (args.Key == "Home")
            {
                ActiveIndex = selectable[0].index;
            }
            else if (args.Key == "End")
            {
                ActiveIndex = selectable[^1].index;
            }
            else if (args.Key == "Enter" || args.Key == " ")
            {
                if (ActiveIndex >= 0 && ActiveIndex < Items.Count)
                {
                    await ActivateItemAsync(Items[ActiveIndex]);
                }
            }
            else if (args.Key == "Escape")
            {
                await HandleOpenChangedAsync(false);
            }
        }

        private string GetItemClass(int index)
        {
            var item = Items[index];
            return Join(
                "flex w-full items-center gap-2 rounded-md px-2 py-2 text-sm outline-none transition-colors duration-150",
                item.Disabled ? "cursor-not-allowed text-text-muted opacity-60" : null,
                !item.Disabled && index == ActiveIndex ? ActiveItemClass(item.Tone == XFSemanticColor.Neutral ? ActiveTone : item.Tone) : null,
                !item.Disabled && index != ActiveIndex ? HoverItemClass(item.Tone) : null);
        }

        private string GetSubmenuItemClass(XFDropdownMenuItem item)
        {
            return Join(
                "flex w-full items-center gap-2 rounded-md px-2 py-2 text-sm outline-none transition-colors duration-150",
                item.Disabled ? "cursor-not-allowed text-text-muted opacity-60" : null,
                !item.Disabled ? HoverItemClass(item.Tone) : null);
        }

        private static string ActiveItemClass(XFSemanticColor tone)
        {
            return tone switch
            {
                XFSemanticColor.Secondary => "bg-[color-mix(in_oklab,var(--color-secondary)_12%,transparent)] text-secondary",
                XFSemanticColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_12%,transparent)] text-success",
                XFSemanticColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_14%,transparent)] text-warning",
                XFSemanticColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_12%,transparent)] text-danger",
                XFSemanticColor.Neutral => "bg-surface-100 text-text-primary",
                _ => "bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] text-primary"
            };
        }

        private static string HoverItemClass(XFSemanticColor tone)
        {
            return tone switch
            {
                XFSemanticColor.Danger => "text-danger hover:bg-[color-mix(in_oklab,var(--color-danger)_8%,transparent)]",
                XFSemanticColor.Warning => "text-warning hover:bg-[color-mix(in_oklab,var(--color-warning)_10%,transparent)]",
                XFSemanticColor.Success => "text-success hover:bg-[color-mix(in_oklab,var(--color-success)_8%,transparent)]",
                XFSemanticColor.Secondary => "text-secondary hover:bg-[color-mix(in_oklab,var(--color-secondary)_8%,transparent)]",
                XFSemanticColor.Primary => "text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]",
                _ => "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary"
            };
        }

        private string GetBadgeClass(XFDropdownMenuItem item)
        {
            return Join("ml-3 rounded-full px-2 py-0.5 text-[0.6875rem] font-bold", ActiveItemClass(item.Tone == XFSemanticColor.Neutral ? ActiveTone : item.Tone));
        }

        private string IndicatorClass(XFDropdownMenuItem item)
        {
            return Join(
                "inline-flex h-5 w-5 shrink-0 items-center justify-center rounded-md text-primary",
                item.Kind == XFDropdownMenuItemKind.XFRadio ? "rounded-full" : null,
                item.Kind is XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio
                    ? "border border-surface-300 bg-bg-surface"
                    : null,
                item.Checked ? "border-[var(--color-primary)] bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]" : null);
        }

        private bool IsExpanded(XFDropdownMenuItem item) => expandedSubmenuIds.Contains(item.Id);

        private static bool HasChildren(XFDropdownMenuItem item) => item.Children.Count > 0;

        private void ToggleSubmenu(XFDropdownMenuItem item)
        {
            if (expandedSubmenuIds.Contains(item.Id))
            {
                expandedSubmenuIds.Remove(item.Id);
            }
            else
            {
                expandedSubmenuIds.Clear();
                expandedSubmenuIds.Add(item.Id);
            }
        }

        private static bool IsSelectable(XFDropdownMenuItem item) =>
            !item.Disabled && !HasChildren(item) && item.Kind is XFDropdownMenuItemKind.Action or XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio;

        private static string GetItemRole(XFDropdownMenuItem item) => item.Kind switch
        {
            XFDropdownMenuItemKind.XFCheckbox => "menuitemcheckbox",
            XFDropdownMenuItemKind.XFRadio => "menuitemradio",
            _ => "menuitem"
        };

        private string GetItemId(int index) => $"{MenuId}-item-{index}";

        private static string? GetAriaChecked(XFDropdownMenuItem item) => item.Kind is XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio
            ? item.Checked.ToString().ToLowerInvariant()
            : null;

        private string GetSubmenuAriaLabel(XFDropdownMenuItem item) => string.Format(
            CultureInfo.CurrentCulture,
            SubmenuAriaLabelFormat,
            item.Text);

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));

        private bool IsFirstInRadioGroup(XFDropdownMenuItem item, int index)
        {
            if (index == 0) return true;
            return Items[index - 1].Kind != XFDropdownMenuItemKind.XFRadio || Items[index - 1].Group != item.Group;
        }

        private async Task HandleCheckboxChanged(XFDropdownMenuItem item, bool? value)
        {
            item.Checked = value == true;
            if (ItemSelected.HasDelegate)
            {
                await ItemSelected.InvokeAsync(item);
            }
            StateHasChanged();
        }

        private async Task HandleRadioGroupChanged(string? group, string? selectedId)
        {
            foreach (var item in Items.Where(i => i.Kind == XFDropdownMenuItemKind.XFRadio && i.Group == group))
            {
                item.Checked = item.Id == selectedId;
            }

            var selected = Items.FirstOrDefault(i => i.Kind == XFDropdownMenuItemKind.XFRadio && i.Id == selectedId);
            if (selected is not null && ItemSelected.HasDelegate)
            {
                await ItemSelected.InvokeAsync(selected);
            }
            StateHasChanged();
        }
    }
}
