using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.TransferList;

public partial class XFTransferList : XForgeComponentBase
{
    [Inject] private IXFLocalizer? Localizer { get; set; }
    private string L(string key, string fallback) => Localizer?[key] ?? fallback;

    private readonly HashSet<string> sourceSelected = new(StringComparer.Ordinal);
    private readonly HashSet<string> targetSelected = new(StringComparer.Ordinal);
    private IReadOnlyList<string> currentTargetValues = Array.Empty<string>();
    private IReadOnlyList<string>? lastTargetValues;
    private string? sourceSearch;
    private string? targetSearch;
    private string? liveMessage;

    [Parameter] public IReadOnlyList<XFTransferListItem> Items { get; set; } = Array.Empty<XFTransferListItem>();
    [Parameter] public IReadOnlyList<string> TargetValues { get; set; } = Array.Empty<string>();
    [Parameter] public EventCallback<IReadOnlyList<string>> TargetValuesChanged { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<string>> Change { get; set; }
    [Parameter] public string SourceLabel { get; set; } = default!;
    [Parameter] public string TargetLabel { get; set; } = default!;
    [Parameter] public string SearchPlaceholder { get; set; } = default!;
    [Parameter] public string SourceSearchLabel { get; set; } = default!;
    [Parameter] public string TargetSearchLabel { get; set; } = default!;
    [Parameter] public string MoveSelectedRightLabel { get; set; } = default!;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Items.Count, TargetValues.Count, SourceLabel, TargetLabel, SearchPlaceholder, SourceSearchLabel, TargetSearchLabel, MoveSelectedRightLabel, MoveSelectedLeftLabel, MoveAllRightLabel, MoveAllLeftLabel);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }
    [Parameter] public string MoveSelectedLeftLabel { get; set; } = default!;
    [Parameter] public string MoveAllRightLabel { get; set; } = default!;
    [Parameter] public string MoveAllLeftLabel { get; set; } = default!;

    [Parameter] public string SelectAllText { get; set; } = default!;
    [Parameter] public string SingleCountFormat { get; set; } = default!;
    [Parameter] public string MultipleCountFormat { get; set; } = default!;
    [Parameter] public string MovedLiveMessageFormat { get; set; } = default!;
    [Parameter] public bool Searchable { get; set; } = true;

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(SourceLabel)) SourceLabel = L("transferlist.source", SourceLabel);
        if (string.IsNullOrWhiteSpace(TargetLabel)) TargetLabel = L("transferlist.target", TargetLabel);
        if (string.IsNullOrWhiteSpace(SearchPlaceholder)) SearchPlaceholder = L("transferlist.search", SearchPlaceholder);
        if (string.IsNullOrWhiteSpace(SourceSearchLabel)) SourceSearchLabel = L("transferlist.source_search", SourceSearchLabel);
        if (string.IsNullOrWhiteSpace(TargetSearchLabel)) TargetSearchLabel = L("transferlist.target_search", TargetSearchLabel);
        if (string.IsNullOrWhiteSpace(MoveSelectedRightLabel)) MoveSelectedRightLabel = L("transferlist.move_selected_right", MoveSelectedRightLabel);
        if (string.IsNullOrWhiteSpace(MoveSelectedLeftLabel)) MoveSelectedLeftLabel = L("transferlist.move_selected_left", MoveSelectedLeftLabel);
        if (string.IsNullOrWhiteSpace(MoveAllRightLabel)) MoveAllRightLabel = L("transferlist.move_all_right", MoveAllRightLabel);
        if (string.IsNullOrWhiteSpace(MoveAllLeftLabel)) MoveAllLeftLabel = L("transferlist.move_all_left", MoveAllLeftLabel);
        if (string.IsNullOrWhiteSpace(SelectAllText)) SelectAllText = L("transferlist.select_all", SelectAllText);
        if (string.IsNullOrWhiteSpace(SingleCountFormat)) SingleCountFormat = L("transferlist.single_count", SingleCountFormat);
        if (string.IsNullOrWhiteSpace(MultipleCountFormat)) MultipleCountFormat = L("transferlist.multiple_count", MultipleCountFormat);
        if (string.IsNullOrWhiteSpace(MovedLiveMessageFormat)) MovedLiveMessageFormat = L("transferlist.moved_message", MovedLiveMessageFormat);
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(TargetValues, lastTargetValues))
        {
            currentTargetValues = TargetValues ?? Array.Empty<string>();
            lastTargetValues = TargetValues;
        }
    }

    private HashSet<string> TargetSet => new(currentTargetValues, StringComparer.Ordinal);
    private IEnumerable<XFTransferListItem> SourceItems => Items.Where(item => !TargetSet.Contains(item.Value));
    private IEnumerable<XFTransferListItem> TargetItems => Items.Where(item => TargetSet.Contains(item.Value));
    private bool CanMoveRight => sourceSelected.Count > 0 && !Disabled;
    private bool CanMoveLeft => targetSelected.Count > 0 && !Disabled;
    private bool CanMoveAllRight => SourceItems.Any(item => !item.Disabled) && !Disabled;
    private bool CanMoveAllLeft => TargetItems.Any(item => !item.Disabled) && !Disabled;

    private string RootClass => Join(
        "xf-transfer-list grid gap-3 lg:grid-cols-[minmax(0,1fr)_auto_minmax(0,1fr)]",
        Disabled ? "opacity-60" : "",
        Class);

    private string PaneClass =>
        "min-w-0 overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.035)]";

    private string MoveButtonClass =>
        "inline-flex h-9 min-w-9 items-center justify-center gap-1 rounded-lg border border-border-light bg-bg-surface px-2 text-text-secondary shadow-sm outline-none transition hover:border-primary hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] disabled:cursor-not-allowed disabled:opacity-45";

    private string SearchClass =>
        "h-9 w-full rounded-lg border border-border-light bg-bg-surface px-3 text-sm text-text-primary outline-none transition placeholder:text-text-muted focus:border-primary focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]";

    private IEnumerable<XFTransferListItem> GetVisibleItems(bool target)
    {
        var search = GetSearch(target);
        var items = target ? TargetItems : SourceItems;

        if (string.IsNullOrWhiteSpace(search))
        {
            return items;
        }

        return items.Where(item =>
            item.Text.Contains(search, StringComparison.OrdinalIgnoreCase)
            || item.Value.Contains(search, StringComparison.OrdinalIgnoreCase)
            || (item.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
    }

    private string GetSearch(bool target) => target ? targetSearch ?? string.Empty : sourceSearch ?? string.Empty;

    private void SetSearch(bool target, string? value)
    {
        if (target)
        {
            targetSearch = value;
        }
        else
        {
            sourceSearch = value;
        }
    }

    private bool IsSelected(bool target, string value) => target ? targetSelected.Contains(value) : sourceSelected.Contains(value);

    private void ToggleSelection(bool target, XFTransferListItem item)
    {
        if (Disabled || item.Disabled)
        {
            return;
        }

        var set = target ? targetSelected : sourceSelected;
        if (!set.Add(item.Value))
        {
            set.Remove(item.Value);
        }
    }

    private async Task MoveSelectedRightAsync() => await MoveAsync(sourceSelected.ToArray(), toTarget: true);
    private async Task MoveSelectedLeftAsync() => await MoveAsync(targetSelected.ToArray(), toTarget: false);
    private async Task MoveAllRightAsync() => await MoveAsync(SourceItems.Where(item => !item.Disabled).Select(item => item.Value).ToArray(), toTarget: true);
    private async Task MoveAllLeftAsync() => await MoveAsync(TargetItems.Where(item => !item.Disabled).Select(item => item.Value).ToArray(), toTarget: false);
    private async Task MoveSingleAsync(bool target, XFTransferListItem item) => await MoveAsync(new[] { item.Value }, toTarget: !target);

    private async Task MoveAsync(IReadOnlyCollection<string> values, bool toTarget)
    {
        if (Disabled || values.Count == 0)
        {
            return;
        }

        var movable = Items
            .Where(item => values.Contains(item.Value) && !item.Disabled)
            .Select(item => item.Value)
            .ToHashSet(StringComparer.Ordinal);

        if (movable.Count == 0)
        {
            return;
        }

        var next = currentTargetValues.ToList();
        if (toTarget)
        {
            foreach (var value in Items.Select(item => item.Value).Where(movable.Contains))
            {
                if (!next.Contains(value, StringComparer.Ordinal))
                {
                    next.Add(value);
                }
            }
        }
        else
        {
            next.RemoveAll(value => movable.Contains(value));
        }

        sourceSelected.Clear();
        targetSelected.Clear();
        currentTargetValues = next;
        lastTargetValues = next;
        liveMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, MovedLiveMessageFormat, movable.Count);

        if (TargetValuesChanged.HasDelegate)
        {
            await TargetValuesChanged.InvokeAsync(next);
        }

        if (Change.HasDelegate)
        {
            await Change.InvokeAsync(next);
        }
    }

    private string GetPaneCountText(bool target)
    {
        var count = (target ? TargetItems : SourceItems).Count();
        return string.Format(System.Globalization.CultureInfo.CurrentCulture, count == 1 ? SingleCountFormat : MultipleCountFormat, count);
    }

    private static string ItemClass(bool selected, bool disabled) => Join(
        "mb-1 flex w-full items-center gap-3 rounded-lg border px-3 py-2 text-left outline-none transition",
        selected
            ? "border-primary bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary"
            : "border-transparent text-text-primary hover:border-border-light hover:bg-surface-50",
        disabled ? "cursor-not-allowed opacity-50" : "cursor-pointer focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)]");

    private static string CheckClass(bool selected) => Join(
        "flex h-5 w-5 shrink-0 items-center justify-center rounded-md border",
        selected ? "border-primary bg-primary text-primary-contrast" : "border-border-light bg-bg-surface text-transparent");

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
