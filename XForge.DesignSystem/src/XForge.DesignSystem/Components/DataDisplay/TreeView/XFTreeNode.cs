namespace XForge.DesignSystem.Components.DataDisplay.TreeView
{
    public sealed class XFTreeNode
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Text { get; set; } = "";

        public string? Description { get; set; }

        public bool Disabled { get; set; }

        public bool HasChildren { get; set; }

        public bool Loading { get; set; }

        public bool Checkable { get; set; } = true;

        public bool Draggable { get; set; } = true;

        public bool Droppable { get; set; } = true;

        public IReadOnlyList<XFTreeNode> Children { get; set; } = Array.Empty<XFTreeNode>();
    }
}
