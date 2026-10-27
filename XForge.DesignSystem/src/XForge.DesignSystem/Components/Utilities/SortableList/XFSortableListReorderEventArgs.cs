namespace XForge.DesignSystem.Components.Utilities.SortableList
{
    public sealed class XFSortableListReorderEventArgs<TItem>
    {
        public XFSortableListReorderEventArgs(TItem item, int oldIndex, int newIndex, IReadOnlyList<TItem> items)
        {
            Item = item;
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Items = items;
        }

        public TItem Item { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }
        public IReadOnlyList<TItem> Items { get; }
    }
}
