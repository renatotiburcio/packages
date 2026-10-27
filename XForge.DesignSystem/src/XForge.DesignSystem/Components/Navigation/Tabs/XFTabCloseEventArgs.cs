namespace XForge.DesignSystem.Components.Navigation.Tabs
{
    public sealed class XFTabCloseEventArgs
    {
        public XFTabCloseEventArgs(string id, int index, XFTab? tab)
        {
            Id = id;
            Index = index;
            Tab = tab;
        }

        public string Id { get; }
        public int Index { get; }
        public XFTab? Tab { get; }
        public string? Label => Tab?.Label;
        public object? Value => Tab?.Value;
    }
}
