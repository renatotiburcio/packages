namespace XForge.DesignSystem.Components.Navigation.Steps
{
    public sealed class XFStepClickEventArgs
    {
        public XFStepClickEventArgs(XFStepItemModel item, int index)
        {
            Item = item;
            Index = index;
        }

        public XFStepItemModel Item { get; }

        public int Index { get; }

        public bool Cancel { get; set; }
    }
}
