using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.Repeater
{
    public partial class XFRepeater<TItem> : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-repeater";

        private readonly HashSet<int> _expandedItems = new();
        private int _draggedIndex = -1;

        #region Parameters

        /// <summary>List of items.</summary>
        [Parameter]
        public IReadOnlyList<TItem>? Items { get; set; }

        /// <summary>Callback when items change.</summary>
        [Parameter]
        public EventCallback<IReadOnlyList<TItem>> ItemsChanged { get; set; }

        /// <summary>Item template (required).</summary>
        [Parameter]
        public RenderFragment<XFRepeaterItemContext<TItem>>? ItemTemplate { get; set; }

        /// <summary>Header template for each item.</summary>
        [Parameter]
        public RenderFragment<XFRepeaterItemContext<TItem>>? HeaderTemplate { get; set; }

        /// <summary>Empty state template.</summary>
        [Parameter]
        public RenderFragment? EmptyTemplate { get; set; }

        /// <summary>Label above the repeater.</summary>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>Helper text below the repeater.</summary>
        [Parameter]
        public string? HelperText { get; set; }

        /// <summary>Factory function to create new items.</summary>
        [Parameter]
        public Func<TItem>? ItemFactory { get; set; }

        /// <summary>Function to get item title for default header.</summary>
        [Parameter]
        public Func<TItem, int, string>? TitleSelector { get; set; }

        /// <summary>Whether to allow adding items.</summary>
        [Parameter]
        public bool AllowAdd { get; set; } = true;

        /// <summary>Whether to allow removing items.</summary>
        [Parameter]
        public bool AllowRemove { get; set; } = true;

        /// <summary>Whether to allow reordering items.</summary>
        [Parameter]
        public bool AllowReorder { get; set; }

        /// <summary>Whether to allow expanding/collapsing items.</summary>
        [Parameter]
        public bool AllowExpand { get; set; }

        /// <summary>Whether the repeater is read-only.</summary>
        [Parameter]
        public bool ReadOnly { get; set; }

        /// <summary>Whether to show the item count.</summary>
        [Parameter]
        public bool ShowItemCount { get; set; } = true;

        /// <summary>Empty state text.</summary>
        [Parameter]
        public string EmptyText { get; set; } = "Nenhum item.";

        /// <summary>Callback when an item is added.</summary>
        [Parameter]
        public EventCallback<TItem> OnItemAdded { get; set; }

        /// <summary>Callback when an item is removed.</summary>
        [Parameter]
        public EventCallback<(TItem Item, int Index)> OnItemRemoved { get; set; }

        /// <summary>Callback when items are reordered.</summary>
        [Parameter]
        public EventCallback<(int FromIndex, int ToIndex)> OnReorder { get; set; }

        #endregion

        #region Labels

        private string AriaLabel => L("repeater.aria_label", "Repeatable list");
        private string AddItemText => L("repeater.add_item", "Add item");
        private string RemoveItemText => L("repeater.remove_item", "Remove item {0}");
        private string ReorderText => L("repeater.reorder", "Reorder");
        private string ItemCountText => L("repeater.item_count", "items");

        #endregion

        #region CSS

        private string ListContainerClass => "space-y-2";

        private string ItemClass(bool isExpanded) => string.Join(" ",
            "rounded-lg border border-border-light bg-bg-surface transition-all duration-200",
            isExpanded ? "shadow-sm" : "");

        private string DragHandleClass => "cursor-grab active:cursor-grabbing p-1 rounded hover:bg-surface-100";

        private string ExpandButtonClass => "p-1 rounded hover:bg-surface-100 transition-colors";

        private string RemoveButtonClass => "p-1 rounded text-text-muted hover:text-danger hover:bg-surface-100 transition-colors";

        private string AddButtonClass => string.Join(" ",
            "flex items-center gap-2 w-full mt-3 px-4 py-2 rounded-lg",
            "border-2 border-dashed border-border-light",
            "text-sm font-medium text-text-secondary",
            "hover:border-[var(--color-primary)] hover:text-primary transition-colors");

        private string EmptyStateClass => "text-center py-8 text-sm text-text-muted";

        #endregion

        #region Item Management

        private string GetItemTitle(TItem item, int index)
        {
            return TitleSelector?.Invoke(item, index) ?? $"Item {index + 1}";
        }

        private async Task AddItemAsync()
        {
            if (ItemFactory == null) return;

            var newItem = ItemFactory();
            var items = new List<TItem>(Items ?? Array.Empty<TItem>());
            items.Add(newItem);

            await UpdateItemsAsync(items);
            await OnItemAdded.InvokeAsync(newItem);
        }

        private async Task RemoveItemAsync(int index)
        {
            if (Items == null || index < 0 || index >= Items.Count) return;

            var removedItem = Items[index];
            var items = new List<TItem>(Items);
            items.RemoveAt(index);

            await UpdateItemsAsync(items);
            await OnItemRemoved.InvokeAsync((removedItem, index));

            _expandedItems.Remove(index);
            // Adjust expanded indices
            var adjusted = new HashSet<int>();
            foreach (var i in _expandedItems)
            {
                if (i > index) adjusted.Add(i - 1);
                else adjusted.Add(i);
            }
            _expandedItems.Clear();
            foreach (var i in adjusted) _expandedItems.Add(i);
        }

        private void ToggleExpand(int index)
        {
            if (_expandedItems.Contains(index))
                _expandedItems.Remove(index);
            else
                _expandedItems.Add(index);

            StateHasChanged();
        }

        private async Task UpdateItemsAsync(IReadOnlyList<TItem> items)
        {
            Items = items;
            await ItemsChanged.InvokeAsync(items);
            StateHasChanged();
        }

        #endregion

        #region Drag & Drop

        private void OnDragStart(int index)
        {
            _draggedIndex = index;
        }

        private async Task OnDrop(int targetIndex)
        {
            if (_draggedIndex < 0 || _draggedIndex == targetIndex || Items == null)
            {
                _draggedIndex = -1;
                return;
            }

            var items = new List<TItem>(Items);
            var draggedItem = items[_draggedIndex];
            items.RemoveAt(_draggedIndex);
            items.Insert(targetIndex, draggedItem);

            _draggedIndex = -1;
            await UpdateItemsAsync(items);
            await OnReorder.InvokeAsync((_draggedIndex, targetIndex));
        }

        #endregion
    }

    #region Supporting types

    public class XFRepeaterItemContext<TItem>
    {
        public TItem Item { get; set; } = default!;
        public int Index { get; set; }

        public XFRepeaterItemContext(TItem item, int index)
        {
            Item = item;
            Index = index;
        }
    }

    #endregion
}
