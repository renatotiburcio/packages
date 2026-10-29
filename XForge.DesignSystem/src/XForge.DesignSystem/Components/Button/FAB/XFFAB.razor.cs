using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Button.FAB;

public partial class XFFAB : XForgeComponentBase
{
    [Parameter, EditorRequired] public string AriaLabel { get; set; } = string.Empty;
    [Parameter] public string? Text { get; set; }
    [Parameter] public RenderFragment? Icon { get; set; }
    [Parameter] public IReadOnlyList<XFFABMenuItem> Items { get; set; } = Array.Empty<XFFABMenuItem>();
    [Parameter] public XFFABPlacement Placement { get; set; }
    [Parameter] public XFButtonColor Color { get; set; } = XFButtonColor.Primary;
    [Parameter] public XFExtendedSize Size { get; set; } = XFExtendedSize.Lg;
    [Parameter] public bool Mini { get; set; }
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public EventCallback<XFFABMenuItem> ItemSelected { get; set; }
    [Parameter] public string MenuLabel { get; set; } = "Acoes";
    [Parameter] public string? OpenMenuAriaLabel { get; set; }
    [Parameter] public string? CloseMenuAriaLabel { get; set; }

        // assinatura de render: botões são o elemento mais frequente em UI — evita re-render
        // redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Text, Icon, Items.Count, Placement, Color, Size, Mini, Open, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

    private string MenuId => $"{Id}-menu";
    private bool IsExtended => !string.IsNullOrWhiteSpace(Text);
    private string EffectiveButtonAriaLabel => Items.Count > 0
        ? Open
            ? CloseMenuAriaLabel ?? AriaLabel
            : OpenMenuAriaLabel ?? AriaLabel
        : AriaLabel;

    private string RootClass => Join(
        "xf-fab fixed z-40 flex flex-col items-end gap-3",
        PlacementClass,
        Class);

    private string PlacementClass => Placement switch
    {
        XFFABPlacement.BottomStart => "bottom-6 left-6",
        XFFABPlacement.TopEnd => "right-6 top-6 flex-col-reverse",
        XFFABPlacement.TopStart => "left-6 top-6 flex-col-reverse items-start",
        _ => "bottom-6 right-6"
    };

    private string ButtonClass => Join(
        "inline-flex items-center justify-center border font-semibold shadow-[0_14px_30px_rgb(15_23_42/.22)] outline-none transition-all duration-200 ease-out focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2",
        IsExtended ? "gap-2 rounded-full px-4" : "rounded-full",
        Mini ? SizeClassMini : SizeClass,
        ToneClass,
        Disabled ? "cursor-not-allowed opacity-60" : "cursor-pointer hover:-translate-y-0.5 hover:shadow-[0_18px_38px_rgb(15_23_42/.26)] active:translate-y-0");

    private string SizeClass => Size switch
    {
        XFExtendedSize.Xs => "h-8 w-8",
        XFExtendedSize.Sm => "h-11 w-11",
        XFExtendedSize.Md => "h-12 w-12",
        XFExtendedSize.Xl => "h-16 w-16",
        _ => "h-14 w-14"
    };

    private string SizeClassMini => Size switch
    {
        XFExtendedSize.Xs => "h-6 w-6",
        XFExtendedSize.Sm => "h-8 w-8",
        XFExtendedSize.Md => "h-10 w-10",
        XFExtendedSize.Xl => "h-14 w-14",
        _ => "h-10 w-10"
    };

    private string IconClass => (Mini ? Size : Size) switch
    {
        XFExtendedSize.Xs => "inline-flex h-4 w-4 items-center justify-center [&_svg]:h-full [&_svg]:w-full",
        XFExtendedSize.Xl => "inline-flex h-7 w-7 items-center justify-center [&_svg]:h-full [&_svg]:w-full",
        _ => "inline-flex h-6 w-6 items-center justify-center [&_svg]:h-full [&_svg]:w-full"
    };

    private string ToneClass => Color switch
    {
        XFButtonColor.Secondary => "border-secondary bg-secondary text-secondary-contrast",
        XFButtonColor.Success => "border-success bg-success text-white",
        XFButtonColor.Warning => "border-warning bg-warning text-surface-950",
        XFButtonColor.Danger => "border-danger bg-danger text-white",
        XFButtonColor.Neutral => "border-surface-950 bg-surface-950 text-white",
        XFButtonColor.Info => "border-info bg-info text-white",
        _ => "border-primary bg-primary text-primary-contrast"
    };

    private string MenuClass => "grid gap-2";

    private string MenuItemClass =>
        "inline-flex min-h-10 cursor-pointer items-center justify-end gap-3 rounded-full border border-border-light bg-bg-surface py-2 pl-4 pr-2 text-sm font-semibold text-text-primary shadow-[0_10px_24px_rgb(15_23_42/.14)] outline-none transition hover:text-primary focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-50";

    private string MenuItemIconClass =>
        "inline-flex h-7 w-7 shrink-0 items-center justify-center overflow-hidden rounded-full bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary [&_svg]:h-3 [&_svg]:w-3";

    private async Task HandleClickAsync(MouseEventArgs args)
    {
        if (Disabled)
        {
            return;
        }

        if (Items.Count > 0)
        {
            Open = !Open;
            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(Open);
            }
        }

        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }
    }

    private async Task SelectItemAsync(XFFABMenuItem item)
    {
        if (Disabled || item.Disabled)
        {
            return;
        }

        if (ItemSelected.HasDelegate)
        {
            await ItemSelected.InvokeAsync(item);
        }

        Open = false;
        if (OpenChanged.HasDelegate)
        {
            await OpenChanged.InvokeAsync(false);
        }
    }

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
