namespace XForge.DesignSystem.Components.Utilities.SortableList
{
    public sealed class XFSortableListItemContext<TItem>
    {
        public XFSortableListItemContext(TItem item, int index, bool disabled)
        {
            Item = item;
            Index = index;
            Disabled = disabled;
        }

        public TItem Item { get; }
        public int Index { get; }
        public bool Disabled { get; }
    }
}
