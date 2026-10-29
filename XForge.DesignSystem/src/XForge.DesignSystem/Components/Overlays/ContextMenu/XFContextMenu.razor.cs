using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Overlays.ContextMenu
{
    public partial class XFContextMenu : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public bool Open { get; set; }
        [Parameter] public EventCallback<bool> OpenChanged { get; set; }
        [Parameter] public IReadOnlyList<XFDropdownMenuItem> Items { get; set; } = Array.Empty<XFDropdownMenuItem>();
        [Parameter] public EventCallback<XFDropdownMenuItem> ItemSelected { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public bool CloseOnSelect { get; set; } = true;
        [Parameter] public bool CloseOnEscape { get; set; } = true;
        [Parameter] public bool CloseOnOutsideClick { get; set; } = true;
        [Parameter] public double CollisionPadding { get; set; } = 8;
        [Parameter] public string AriaLabel { get; set; } = default!;
        [Parameter] public string SubmenuAriaLabelFormat { get; set; } = default!;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Open, Items.Count, ItemSelected, ChildContent, Size, CloseOnSelect, CloseOnEscape, CloseOnOutsideClick, CollisionPadding);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("contextmenu.label", AriaLabel);
            base.OnInitialized();
        }

        private double X { get; set; }
        private double Y { get; set; }
        private int ActiveIndex { get; set; } = -1;

        private string RootClass => Join("xf-context-menu contents", Class);
        private string OverlayClass => "fixed inset-0 z-50 bg-transparent";
        private string MenuClass => Join(
            "fixed z-[51] flex flex-col gap-0.5 rounded-lg border border-surface-200 bg-bg-surface p-3 text-sm text-text-secondary shadow-[0_18px_40px_rgb(15_23_42/.14)] outline-none ring-1 ring-surface-200/80",
            HasAnySubmenu ? "overflow-visible" : "overflow-y-auto",
            SizeClass);

        private string SizeClass => Size switch
        {
            XFSize.Sm => "w-56",
            XFSize.Lg => "w-96",
            _ => "w-72"
        };

        private int EstimatedWidth => Size switch
        {
            XFSize.Sm => 224,
            XFSize.Lg => 384,
            _ => 288
        };

        private int EstimatedHeight => Math.Min(480, Math.Max(72, Items.Count(item => item.Kind != XFDropdownMenuItemKind.Separator) * 48 + Items.Count(item => item.Kind == XFDropdownMenuItemKind.Separator) * 12 + 24));
        private bool HasAnySubmenu => Items.Any(HasSubmenu);

        private string MenuStyle => $"left: min(max({X}px, {CollisionPadding}px), calc(100vw - {EstimatedWidth}px - {CollisionPadding}px)); top: min(max({Y}px, {CollisionPadding}px), calc(100vh - {EstimatedHeight}px - {CollisionPadding}px)); max-height: calc(100vh - {CollisionPadding * 2}px);";

        protected override void OnParametersSet()
        {
            if (Open && (ActiveIndex < 0 || ActiveIndex >= Items.Count || !IsSelectable(Items[ActiveIndex])))
            {
                ActiveIndex = FirstSelectableIndex();
            }
        }

        private async Task HandleContextMenuAsync(MouseEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            X = Math.Max(CollisionPadding, args.ClientX);
            Y = Math.Max(CollisionPadding, args.ClientY);
            ActiveIndex = FirstSelectableIndex();
            await SetOpenAsync(true);
        }

        private async Task CloseAsync()
        {
            ActiveIndex = -1;
            await SetOpenAsync(false);
        }

        private async Task HandleOverlayClickAsync()
        {
            if (CloseOnOutsideClick)
            {
                await CloseAsync();
            }
        }

        private async Task ActivateItemAsync(XFDropdownMenuItem item)
        {
            if (!IsSelectable(item) || HasSubmenu(item))
            {
                return;
            }

            if (ItemSelected.HasDelegate)
            {
                await ItemSelected.InvokeAsync(item);
            }

            if (CloseOnSelect && item.Kind == XFDropdownMenuItemKind.Action)
            {
                await CloseAsync();
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            var selectable = Items
                .Select((item, index) => new { item, index })
                .Where(entry => IsSelectable(entry.item))
                .ToArray();

            if (args.Key == "Escape")
            {
                if (CloseOnEscape)
                {
                    await CloseAsync();
                }
                return;
            }

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
            else if (args.Key is "Enter" or " ")
            {
                if (ActiveIndex >= 0 && ActiveIndex < Items.Count)
                {
                    await ActivateItemAsync(Items[ActiveIndex]);
                }
            }
        }

        private void SetActiveIndex(int index)
        {
            if (index >= 0 && index < Items.Count && IsSelectable(Items[index]))
            {
                ActiveIndex = index;
            }
        }

        private string GetItemClass(int index)
        {
            var item = Items[index];
            return Join(
                "flex w-full items-center gap-2 rounded-md px-2 py-2 text-sm outline-none transition-colors duration-150",
                item.Disabled ? "cursor-not-allowed text-text-muted opacity-60" : null,
                !item.Disabled && index == ActiveIndex ? "bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary" : null,
                !item.Disabled && index != ActiveIndex ? "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary" : null);
        }

        private string GetSubmenuClass(int index)
        {
            return Join(
                "absolute left-[calc(100%+0.35rem)] top-0 z-[52] hidden w-56 flex-col gap-0.5 rounded-lg border border-surface-200 bg-bg-surface p-2 text-sm text-text-secondary shadow-[0_18px_40px_rgb(15_23_42/.14)] ring-1 ring-surface-200/80 group-hover:flex",
                index == ActiveIndex ? "flex" : null);
        }

        private string GetSubmenuItemClass(XFDropdownMenuItem item)
        {
            return Join(
                "flex w-full items-center gap-2 rounded-md px-2 py-2 text-sm outline-none transition-colors duration-150",
                item.Disabled ? "cursor-not-allowed text-text-muted opacity-60" : "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary");
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

        private int FirstSelectableIndex() => Items
            .Select((item, index) => new { item, index })
            .FirstOrDefault(entry => IsSelectable(entry.item))?.index ?? -1;

        private async Task SetOpenAsync(bool open)
        {
            Open = open;
            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(open);
            }
        }

        private static bool HasSubmenu(XFDropdownMenuItem item) => item.Children.Count > 0;

        private static bool IsSelectable(XFDropdownMenuItem item) =>
            !item.Disabled && (HasSubmenu(item) || item.Kind is XFDropdownMenuItemKind.Action or XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio);

        private static string GetItemRole(XFDropdownMenuItem item) => item.Kind switch
        {
            XFDropdownMenuItemKind.XFCheckbox => "menuitemcheckbox",
            XFDropdownMenuItemKind.XFRadio => "menuitemradio",
            _ => "menuitem"
        };

        private static string? GetAriaChecked(XFDropdownMenuItem item) => item.Kind is XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio
            ? item.Checked.ToString().ToLowerInvariant()
            : null;

        private string GetSubmenuAriaLabel(XFDropdownMenuItem item) => string.Format(
            CultureInfo.CurrentCulture,
            SubmenuAriaLabelFormat,
            item.Text);

        public async ValueTask DisposeAsync()
        {
            if (TrapActive)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfFocusTrap.release", Id);
                }
                catch (JSDisconnectedException) { }
                catch (InvalidOperationException) { }
            }
        }

        private bool TrapActive { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Open && !TrapActive)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfFocusTrap.activate", Id);
                }
                catch (InvalidOperationException)
                {
                    // JS not available during prerender
                    return;
                }
                catch (JSDisconnectedException)
                {
                    // JS disconnected
                    return;
                }
                TrapActive = true;
            }
            else if (!Open && TrapActive)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfFocusTrap.release", Id);
                }
                catch (InvalidOperationException)
                {
                    // JS not available during prerender
                }
                catch (JSDisconnectedException)
                {
                    // JS disconnected
                }
                TrapActive = false;
            }
        }

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
