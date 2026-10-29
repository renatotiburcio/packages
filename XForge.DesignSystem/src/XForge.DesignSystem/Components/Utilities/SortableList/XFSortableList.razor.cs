using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Utilities.SortableList
{
    public partial class XFSortableList<TItem> : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
        [Parameter] public EventCallback<IReadOnlyList<TItem>> ItemsChanged { get; set; }
        [Parameter] public EventCallback<XFSortableListReorderEventArgs<TItem>> Reordered { get; set; }
        [Parameter] public RenderFragment<XFSortableListItemContext<TItem>>? ItemTemplate { get; set; }
        [Parameter] public Func<TItem, string>? TextSelector { get; set; }
        [Parameter] public Func<TItem, bool>? ItemDisabled { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string MoveUpLabel { get; set; } = default!;
        [Parameter] public string MoveDownLabel { get; set; } = default!;
        [Parameter] public string MoveUpLabelFormat { get; set; } = default!;
        [Parameter] public string MoveDownLabelFormat { get; set; } = default!;
        [Parameter] public bool ShowOrder { get; set; } = true;
        [Parameter] public bool AllowDragReorder { get; set; } = true;
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFListVariant Variant { get; set; } = XFListVariant.Bordered;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Items.Count, ItemsChanged, ItemTemplate, AriaLabel, MoveUpLabel, MoveDownLabel, MoveUpLabelFormat, MoveDownLabelFormat, ShowOrder);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(MoveUpLabel)) MoveUpLabel = L("sortable.move_up", MoveUpLabel);
            if (string.IsNullOrWhiteSpace(MoveDownLabel)) MoveDownLabel = L("sortable.move_down", MoveDownLabel);
            if (string.IsNullOrWhiteSpace(MoveUpLabelFormat)) MoveUpLabelFormat = L("sortable.move_up_format", MoveUpLabelFormat);
            if (string.IsNullOrWhiteSpace(MoveDownLabelFormat)) MoveDownLabelFormat = L("sortable.move_down_format", MoveDownLabelFormat);
            base.OnInitialized();
        }

        private int? DragIndex { get; set; }

        private string RootClass => Join("xf-sortable-list grid gap-2", FullWidth ? "w-full" : "inline-grid", Class);

        private string ItemClass(TItem item) => Join(
            "group flex items-center gap-3 rounded-lg border bg-bg-surface text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-150",
            SizeClass,
            VariantClass,
            DragIndex is not null && !IsDisabled(item) ? "cursor-grabbing" : AllowDragReorder && !IsDisabled(item) ? "cursor-grab" : null,
            IsDisabled(item) ? "opacity-60" : "hover:border-[color-mix(in_oklab,var(--color-primary)_30%,var(--color-border))]");

        private string SizeClass => Size switch
        {
            XFSize.Sm => "min-h-10 px-3 py-2 text-sm",
            XFSize.Lg => "min-h-16 px-5 py-4 text-base",
            _ => "min-h-12 px-4 py-3 text-sm"
        };

        private string VariantClass => Variant switch
        {
            XFListVariant.Plain => "border-transparent shadow-none",
            XFListVariant.Soft => "border-border-light bg-surface-50",
            _ => "border-border-light"
        };

        private string OrderClass => "inline-flex h-7 min-w-7 shrink-0 items-center justify-center rounded-md bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] px-2 text-xs font-semibold text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]";
        private string ContentClass => "min-w-0 flex-1";
        private string TextClass => "truncate font-medium";
        private string ControlsClass => "flex shrink-0 items-center gap-1";
        private string ButtonClass => "inline-flex h-8 w-8 items-center justify-center rounded-md text-text-muted outline-none ring-1 ring-transparent transition-colors duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] disabled:cursor-not-allowed disabled:opacity-40";

        private bool IsDisabled(TItem item) => Disabled || ItemDisabled?.Invoke(item) == true;

        private bool CanDrag(TItem item) => AllowDragReorder && !IsDisabled(item);

        private string GetText(TItem item) => TextSelector?.Invoke(item) ?? item?.ToString() ?? string.Empty;

        private string FormatMoveLabel(string format, string actionLabel, TItem item)
        {
            return string.Format(CultureInfo.CurrentCulture, format, actionLabel, GetText(item));
        }

        private async Task MoveAsync(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= Items.Count || newIndex < 0 || newIndex >= Items.Count)
            {
                return;
            }

            var item = Items[oldIndex];
            if (IsDisabled(item))
            {
                return;
            }

            var next = Items.ToList();
            next.RemoveAt(oldIndex);
            next.Insert(newIndex, item);

            Items = next;
            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(next);
            }

            if (Reordered.HasDelegate)
            {
                await Reordered.InvokeAsync(new XFSortableListReorderEventArgs<TItem>(item, oldIndex, newIndex, next));
            }
        }

        private void HandleDragStart(int index)
        {
            if (index < 0 || index >= Items.Count || !CanDrag(Items[index]))
            {
                DragIndex = null;
                return;
            }

            DragIndex = index;
        }

        private static void HandleDragOver(DragEventArgs _)
        {
        }

        private async Task DropAsync(int targetIndex)
        {
            if (DragIndex is not { } sourceIndex)
            {
                return;
            }

            DragIndex = null;
            await MoveAsync(sourceIndex, targetIndex);
        }

        private void HandleDragEnd()
        {
            DragIndex = null;
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
