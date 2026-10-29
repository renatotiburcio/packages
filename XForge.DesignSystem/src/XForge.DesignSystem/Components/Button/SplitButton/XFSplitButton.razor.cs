using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Components.Overlays.DropdownMenu;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Button.SplitButton;

public partial class XFSplitButton : XForgeComponentBase, IAsyncDisposable
{
    private DotNetObjectReference<XFSplitButton>? dotNetReference;
    private bool registered;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? StartIcon { get; set; }
    [Parameter] public string? Text { get; set; }
    [Parameter] public XFButtonVariant Variant { get; set; } = XFButtonVariant.Solid;
    [Parameter] public XFButtonColor Color { get; set; } = XFButtonColor.Primary;
    [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Md;
    [Parameter] public XFButtonType Type { get; set; } = XFButtonType.Button;
    [Parameter] public bool FullWidth { get; set; }
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public IReadOnlyList<XFDropdownMenuItem> Items { get; set; } = Array.Empty<XFDropdownMenuItem>();
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public EventCallback<XFDropdownMenuItem> ItemSelected { get; set; }
    [Parameter] public string MenuAriaLabel { get; set; } = "Abrir opcoes";
    [Parameter] public string? MenuPanelAriaLabel { get; set; }
    [Parameter] public bool CloseOnSelect { get; set; } = true;

        // assinatura de render: botões são o elemento mais frequente em UI — evita re-render
        // redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ChildContent, StartIcon, Text, Variant, Color, Size, Type, FullWidth, Disabled, Open);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

    private int activeIndex = -1;
    private bool IsDisabled => Disabled;
    private string ButtonTypeValue => Type.ToString().ToLowerInvariant();
    private string TriggerId => $"{Id}-trigger";
    private string PanelId => $"{Id}-panel";
    private string EffectiveMenuPanelAriaLabel => string.IsNullOrWhiteSpace(MenuPanelAriaLabel) ? MenuAriaLabel : MenuPanelAriaLabel;
    private string IconClass => Size is XFExtendedSize.Xs or XFExtendedSize.Sm
        ? "flex h-4 w-4 shrink-0 items-center justify-center"
        : "flex h-5 w-5 shrink-0 items-center justify-center";

    private string RootClass => Join(
        "xf-split-button relative inline-flex align-middle",
        FullWidth ? "w-full" : "",
        Class);

    private string PrimaryButtonClass => Join(ButtonBaseClass, SizePrimaryClass, ToneClass, "rounded-l-md border-r-0", FullWidth ? "flex-1" : "");

    private string TriggerButtonClass => Join(ButtonBaseClass, SizeTriggerClass, ToneClass, "rounded-r-md px-2");

    private string ButtonBaseClass => Join(
        "inline-flex items-center justify-center border font-semibold outline-none transition-all duration-200 ease-out focus-visible:z-10 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2",
        IsDisabled ? "pointer-events-none cursor-not-allowed opacity-60" : "cursor-pointer hover:ring-2 hover:ring-current/15 active:translate-y-px",
        Size is XFExtendedSize.Xs or XFExtendedSize.Sm ? "gap-1.5" : "gap-2");

    private string SizePrimaryClass => Size switch
    {
        XFExtendedSize.Xs => "h-7 px-2.5 text-xs",
        XFExtendedSize.Sm => "h-8 px-3 text-sm",
        XFExtendedSize.Lg => "h-11 px-5 text-base",
        XFExtendedSize.Xl => "h-12 px-6 text-lg",
        _ => "h-10 px-4 text-sm"
    };

    private string SizeTriggerClass => Size switch
    {
        XFExtendedSize.Xs => "h-7 text-xs",
        XFExtendedSize.Sm => "h-8 text-sm",
        XFExtendedSize.Lg => "h-11 text-base",
        XFExtendedSize.Xl => "h-12 text-lg",
        _ => "h-10 text-sm"
    };

    private string ToneClass => (Color, Variant) switch
    {
        (_, XFButtonVariant.Outline) => $"{ToneTextClass} bg-bg-surface {ToneBorderClass} shadow-[0_1px_1px_rgb(15_23_42/.04)]",
        (_, XFButtonVariant.Ghost) => $"{ToneTextClass} border-transparent bg-transparent",
        (_, XFButtonVariant.Soft) => $"{ToneSoftClass} border-transparent",
        (_, XFButtonVariant.Subtle) => $"{ToneTextClass} border-border-light bg-surface-50",
        _ => $"{ToneSolidClass} {ToneBorderClass} shadow-[0_1px_2px_rgb(15_23_42/.12),inset_0_1px_0_rgb(255_255_255/.18)]"
    };

    private string ToneTextClass => Color switch
    {
        XFButtonColor.Secondary => "text-text-secondary",
        XFButtonColor.Success => "text-success",
        XFButtonColor.Warning => "text-warning",
        XFButtonColor.Danger => "text-danger",
        XFButtonColor.Neutral => "text-text-primary",
        XFButtonColor.Info => "text-info",
        _ => "text-primary"
    };

    private string ToneBorderClass => Color switch
    {
        XFButtonColor.Secondary or XFButtonColor.Neutral => "border-border",
        XFButtonColor.Success => "border-success",
        XFButtonColor.Warning => "border-warning",
        XFButtonColor.Danger => "border-danger",
        XFButtonColor.Info => "border-info",
        _ => "border-primary"
    };

    private string ToneSolidClass => Color switch
    {
        XFButtonColor.Secondary => "bg-secondary text-secondary-contrast hover:bg-secondary-hover active:bg-secondary-active",
        XFButtonColor.Success => "bg-success text-white",
        XFButtonColor.Warning => "bg-warning text-surface-950",
        XFButtonColor.Danger => "bg-danger text-white",
        XFButtonColor.Neutral => "bg-surface-950 text-white",
        XFButtonColor.Info => "bg-info text-white",
        _ => "bg-primary text-primary-contrast hover:bg-primary-hover active:bg-primary-active"
    };

    private string ToneSoftClass => Color switch
    {
        XFButtonColor.Secondary => "bg-surface-100 text-text-secondary",
        XFButtonColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_10%,transparent)] text-success",
        XFButtonColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_10%,transparent)] text-warning",
        XFButtonColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_10%,transparent)] text-danger",
        XFButtonColor.Neutral => "bg-surface-100 text-text-primary",
        XFButtonColor.Info => "bg-[color-mix(in_oklab,var(--color-info)_10%,transparent)] text-info",
        _ => "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary"
    };

    private string ChevronClass => Join("inline-flex h-3.5 w-3.5 items-center justify-center transition-transform duration-150", Open ? "rotate-180" : "");

    private string MenuPanelClass =>
        "fixed z-50 min-w-56 rounded-lg border border-border-light bg-bg-surface p-2 shadow-[0_18px_45px_rgb(15_23_42/.14)]";

    protected override void OnParametersSet()
    {
        if (Open && (activeIndex < 0 || activeIndex >= Items.Count || !IsSelectable(Items[activeIndex])))
        {
            activeIndex = FirstSelectableIndex();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Open)
        {
            dotNetReference ??= DotNetObjectReference.Create(this);
            try
            {
                await JSRuntime.InvokeVoidAsync("xfSplitButton.register", TriggerId, PanelId, dotNetReference);
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
            registered = true;
        }
        else if (registered)
        {
            await ReleaseAsync();
        }
    }

    private async Task HandlePrimaryClickAsync(MouseEventArgs args)
    {
        if (!IsDisabled && OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }
    }

    private async Task ToggleMenuAsync()
    {
        if (IsDisabled)
        {
            return;
        }

        await SetOpenAsync(!Open);
    }

    private async Task SetOpenAsync(bool open)
    {
        Open = open;
        activeIndex = open ? FirstSelectableIndex() : -1;

        if (!open)
        {
            await ReleaseAsync();
        }

        if (OpenChanged.HasDelegate)
        {
            await OpenChanged.InvokeAsync(open);
        }
    }

    [JSInvokable]
    public async Task CloseFromJsAsync()
    {
        await SetOpenAsync(false);
    }

    private async Task ReleaseAsync()
    {
        if (!registered)
        {
            return;
        }

        try
        {
            await JSRuntime.InvokeVoidAsync("xfSplitButton.release", PanelId);
        }
        catch (InvalidOperationException)
        {
            // JS not available during prerender
        }
        catch (JSDisconnectedException)
        {
            // JS disconnected
        }
        registered = false;
    }

    public async ValueTask DisposeAsync()
    {
        if (registered)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("xfSplitButton.release", PanelId);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        dotNetReference?.Dispose();
    }

    private async Task SelectItemAsync(XFDropdownMenuItem item)
    {
        if (!IsSelectable(item))
        {
            return;
        }

        if (ItemSelected.HasDelegate)
        {
            await ItemSelected.InvokeAsync(item);
        }

        if (CloseOnSelect)
        {
            await SetOpenAsync(false);
        }
    }

    private async Task HandleTriggerKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key is "ArrowDown" or "Enter" or " ")
        {
            await SetOpenAsync(true);
        }
    }

    private async Task HandleMenuKeyDownAsync(KeyboardEventArgs args)
    {
        var selectable = Items.Select((item, index) => new { item, index }).Where(entry => IsSelectable(entry.item)).ToArray();
        if (selectable.Length == 0)
        {
            return;
        }

        if (args.Key == "ArrowDown")
        {
            var current = Array.FindIndex(selectable, entry => entry.index == activeIndex);
            activeIndex = selectable[(current + 1 + selectable.Length) % selectable.Length].index;
        }
        else if (args.Key == "ArrowUp")
        {
            var current = Array.FindIndex(selectable, entry => entry.index == activeIndex);
            activeIndex = selectable[(current - 1 + selectable.Length) % selectable.Length].index;
        }
        else if (args.Key == "Home")
        {
            activeIndex = selectable[0].index;
        }
        else if (args.Key == "End")
        {
            activeIndex = selectable[^1].index;
        }
        else if (args.Key is "Enter" or " ")
        {
            if (activeIndex >= 0 && activeIndex < Items.Count)
            {
                await SelectItemAsync(Items[activeIndex]);
            }
        }
        else if (args.Key == "Escape")
        {
            await SetOpenAsync(false);
        }
    }

    private string GetItemClass(int index)
    {
        var item = Items[index];
        return Join(
            "flex w-full items-center gap-2 rounded-md px-2 py-2 text-sm outline-none transition-colors duration-150",
            item.Disabled ? "cursor-not-allowed text-text-muted opacity-60" : "",
            !item.Disabled && index == activeIndex ? "bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] text-primary" : "",
            !item.Disabled && index != activeIndex ? "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary" : "");
    }

    private string IndicatorClass(XFDropdownMenuItem item) => Join(
        "inline-flex h-5 w-5 shrink-0 items-center justify-center rounded-md text-primary",
        item.Kind == XFDropdownMenuItemKind.XFRadio ? "rounded-full" : "",
        item.Kind is XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio ? "border border-surface-300 bg-bg-surface" : "",
        item.Checked ? "border-primary bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]" : "");

    private int FirstSelectableIndex() =>
        Items.Select((item, index) => new { item, index }).FirstOrDefault(entry => IsSelectable(entry.item))?.index ?? -1;

    private static bool IsSelectable(XFDropdownMenuItem item) =>
        !item.Disabled && item.Kind is XFDropdownMenuItemKind.Action or XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio;

    private static string GetItemRole(XFDropdownMenuItem item) => item.Kind switch
    {
            XFDropdownMenuItemKind.XFCheckbox => "menuitemcheckbox",
            XFDropdownMenuItemKind.XFRadio => "menuitemradio",
        _ => "menuitem"
    };

    private static string? GetAriaChecked(XFDropdownMenuItem item) => item.Kind is XFDropdownMenuItemKind.XFCheckbox or XFDropdownMenuItemKind.XFRadio
        ? item.Checked.ToString().ToLowerInvariant()
        : null;

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
