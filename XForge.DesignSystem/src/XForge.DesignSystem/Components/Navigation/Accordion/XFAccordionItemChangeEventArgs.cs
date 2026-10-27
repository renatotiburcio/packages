namespace XForge.DesignSystem.Components.Navigation.Accordion
{
    public sealed class XFAccordionItemChangeEventArgs
    {
        public XFAccordionItemChangeEventArgs(XFAccordionItem item, bool expanded, int index)
        {
            Item = item;
            Expanded = expanded;
            Index = index;
        }

        public XFAccordionItem Item { get; }
        public bool Expanded { get; }
        public int Index { get; }
        public string? Value => Item.Value;
        public string? Title => Item.Title;
    }
}
