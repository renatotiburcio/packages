namespace XForge.DesignSystem.Components.DataDisplay.Sankey
{
    /// <summary>
    /// Node alignment strategy for Sankey layout.
    /// </summary>
    public enum XFSankeyAlignment
    {
        /// <summary>Distribute nodes evenly across the full width.</summary>
        Justify,
        /// <summary>Align nodes to the left with compact spacing.</summary>
        Left,
        /// <summary>Align nodes to the right with compact spacing.</summary>
        Right,
        /// <summary>Center nodes with proportional spacing.</summary>
        Center
    }

    /// <summary>
    /// Size variant for the Sankey diagram.
    /// </summary>
/// <summary>
    /// Internal node representation for layout computation.
    /// </summary>
    internal class XFSankeyNode
    {
        public string Id { get; set; } = string.Empty;
        public string? Label { get; set; }
        public double? ExplicitValue { get; set; }
    }

    /// <summary>
    /// Internal link representation for layout computation.
    /// </summary>
    internal class XFSankeyLink
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    /// <summary>
    /// Computed node with layout geometry.
    /// </summary>
    internal class XFSankeyComputedNode : XFSankeyNode
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double ComputedValue { get; set; }
        public int Layer { get; set; }
        public List<XFSankeyComputedLink> SourceLinks { get; set; } = new();
        public List<XFSankeyComputedLink> TargetLinks { get; set; } = new();
        public int ColorIndex { get; set; }
    }

    /// <summary>
    /// Computed link with SVG path and geometry.
    /// </summary>
    internal class XFSankeyComputedLink : XFSankeyLink
    {
        public string? Path { get; set; }
        public XFSankeyComputedNode? SourceNode { get; set; }
        public XFSankeyComputedNode? TargetNode { get; set; }
        public double Y0 { get; set; }
        public double Y1 { get; set; }
        public double Width { get; set; }
        public double WidthSource { get; set; }
        public double WidthTarget { get; set; }
    }

    /// <summary>
    /// Event arguments for node click events.
    /// </summary>
    public class XFSankeyNodeClickEventArgs
    {
        /// <summary>Node identifier.</summary>
        public string NodeId { get; set; } = string.Empty;
        /// <summary>Node label.</summary>
        public string? Label { get; set; }
        /// <summary>Computed value (max of incoming/outgoing).</summary>
        public double Value { get; set; }
        /// <summary>Sum of incoming link values.</summary>
        public double Incoming { get; set; }
        /// <summary>Sum of outgoing link values.</summary>
        public double Outgoing { get; set; }
    }

    /// <summary>
    /// Event arguments for link click events.
    /// </summary>
    public class XFSankeyLinkClickEventArgs
    {
        /// <summary>Source node identifier.</summary>
        public string Source { get; set; } = string.Empty;
        /// <summary>Target node identifier.</summary>
        public string Target { get; set; } = string.Empty;
        /// <summary>Flow value.</summary>
        public double Value { get; set; }
    }
}
