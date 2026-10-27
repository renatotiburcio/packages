namespace XForge.DesignSystem.Components.DataDisplay.TreeView
{
    public sealed record XFTreeNodeEventArgs(XFTreeNode Node);

    public sealed class XFTreeLoadChildrenEventArgs
    {
        public XFTreeLoadChildrenEventArgs(XFTreeNode node)
        {
            Node = node;
        }

        public XFTreeNode Node { get; }

        public IReadOnlyList<XFTreeNode>? Children { get; set; }
    }

    public sealed class XFTreeDragEventArgs
    {
        public XFTreeDragEventArgs(XFTreeNode source)
        {
            Source = source;
        }

        public XFTreeNode Source { get; }

        public bool Cancel { get; set; }
    }

    public sealed class XFTreeDropEventArgs
    {
        public XFTreeDropEventArgs(XFTreeNode source, XFTreeNode target, XFTreeDropPosition position)
        {
            Source = source;
            Target = target;
            Position = position;
        }

        public XFTreeNode Source { get; }

        public XFTreeNode Target { get; }

        public XFTreeDropPosition Position { get; }

        public bool Cancel { get; set; }
    }
}
