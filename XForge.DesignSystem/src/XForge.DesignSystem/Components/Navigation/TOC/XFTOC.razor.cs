using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Navigation.TOC;

public partial class XFTOC : XForgeComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IXFLocalizer? Localizer { get; set; }
    private string L(string key, string fallback) => Localizer?[key] ?? fallback;

    [Parameter] public IReadOnlyList<XFTOCItem> Items { get; set; } = Array.Empty<XFTOCItem>();
    [Parameter] public string? ActiveId { get; set; }
    [Parameter] public EventCallback<string?> ActiveIdChanged { get; set; }
    [Parameter] public EventCallback<XFTOCItem> ItemClicked { get; set; }
    [Parameter] public XFTOCOrientation Orientation { get; set; }
    [Parameter] public XFTOCMarker Marker { get; set; } = XFTOCMarker.Dot;
    [Parameter] public string AriaLabel { get; set; } = default!;
    [Parameter] public string? Title { get; set; } = "Nesta página";
    [Parameter] public string EmptyText { get; set; } = default!;
    [Parameter] public string ActiveItemAriaLabelFormat { get; set; } = default!;
    [Parameter] public string DisabledItemAriaLabelFormat { get; set; } = default!;
    [Parameter] public bool HighlightParent { get; set; } = true;
    [Parameter] public int Offset { get; set; }
    [Parameter] public int StartNumber { get; set; } = 1;
    [Parameter] public RenderFragment<XFTOCItem>? ItemTemplate { get; set; }
    [Parameter] public RenderFragment<XFTOCItem>? MarkerTemplate { get; set; }

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("toc.aria_label", AriaLabel);
        Title = L("toc.title", Title!);
        if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("toc.empty", EmptyText);
        if (string.IsNullOrWhiteSpace(ActiveItemAriaLabelFormat)) ActiveItemAriaLabelFormat = L("toc.active_format", ActiveItemAriaLabelFormat);
        if (string.IsNullOrWhiteSpace(DisabledItemAriaLabelFormat)) DisabledItemAriaLabelFormat = L("toc.disabled_format", DisabledItemAriaLabelFormat);
        base.OnInitialized();
    }

    private string RootClass => Join(
        "xf-toc",
        Orientation == XFTOCOrientation.Horizontal
            ? "overflow-x-auto rounded-lg border border-border-light bg-bg-surface p-2"
            : "rounded-lg border border-border-light bg-bg-surface p-3 shadow-[0_1px_2px_rgb(15_23_42/.035)]",
        Class);

    private string TitleClass => Orientation == XFTOCOrientation.Horizontal
        ? "sr-only"
        : "mb-2 px-2 text-xs font-semibold uppercase tracking-wide text-text-muted";

    private string ListClass => Orientation == XFTOCOrientation.Horizontal
        ? "flex min-w-max items-center gap-1"
        : "grid gap-1";

    private string ItemShellClass(int level) => Orientation == XFTOCOrientation.Horizontal || level == 1
        ? "min-w-0"
        : "min-w-0 pl-4";

    private string GetItemClass(XFTOCItem item, int level)
    {
        var active = IsActive(item);
        var depth = Orientation == XFTOCOrientation.Horizontal ? "" : level > 1 ? "text-xs" : "text-sm";
        var layout = Orientation == XFTOCOrientation.Horizontal
            ? "inline-flex h-8 items-center gap-2 whitespace-nowrap rounded-md px-3 text-sm"
            : "flex min-h-8 items-center gap-2 rounded-md px-2";
        var state = item.Disabled
            ? "pointer-events-none text-text-muted opacity-55"
            : active
                ? "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] font-semibold text-primary"
                : "text-text-secondary hover:bg-surface-50 hover:text-text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]";

        return Join(layout, depth, "outline-none transition-colors", state);
    }

    private string MarkerClass(XFTOCItem item) => Join(
        "shrink-0 rounded-full transition-colors",
        Orientation == XFTOCOrientation.Horizontal ? "h-2 w-2" : "h-2 w-2",
        IsActive(item) ? "bg-primary" : "bg-surface-300");

    private string NumberMarkerClass(XFTOCItem item) => Join(
        "inline-flex h-5 w-5 shrink-0 items-center justify-center rounded-full text-[10px] font-semibold transition-colors",
        IsActive(item)
            ? "bg-primary text-white"
            : "bg-surface-100 text-text-muted");

    private bool IsActive(XFTOCItem item)
    {
        if (string.Equals(ActiveId, item.Id, StringComparison.Ordinal))
        {
            return true;
        }

        return HighlightParent
            && item.Children.Any(child => string.Equals(ActiveId, child.Id, StringComparison.Ordinal));
    }

    private string? GetItemAriaLabel(XFTOCItem item)
    {
        if (item.Disabled)
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, DisabledItemAriaLabelFormat, item.Text);
        }

        if (IsActive(item))
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, ActiveItemAriaLabelFormat, item.Text);
        }

        return null;
    }

    private string GetHref(XFTOCItem item)
    {
        if (item.Disabled)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(item.Href))
        {
            return item.Href;
        }

        var current = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        var fragmentIndex = current.IndexOf('#', StringComparison.Ordinal);
        if (fragmentIndex >= 0)
        {
            current = current[..fragmentIndex];
        }

        var queryIndex = current.IndexOf('?', StringComparison.Ordinal);
        if (queryIndex >= 0)
        {
            current = current[..queryIndex];
        }

        current = string.IsNullOrWhiteSpace(current) ? "/" : $"/{current.TrimStart('/')}";
        return $"{current}#{item.Id}";
    }

    private async Task HandleItemClickAsync(XFTOCItem item)
    {
        if (Disabled || item.Disabled)
        {
            return;
        }

        ActiveId = item.Id;

        if (ActiveIdChanged.HasDelegate)
        {
            await ActiveIdChanged.InvokeAsync(item.Id);
        }

        if (ItemClicked.HasDelegate)
        {
            await ItemClicked.InvokeAsync(item);
        }
    }

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
