using System;
using System.Collections.Generic;
using System.Linq;

namespace XForge.DesignSystem.Components.DataDisplay.Sankey
{
    /// <summary>
    /// Computes Sankey diagram layout using BFS layering and iterative relaxation.
    /// Inspired by d3-sankey algorithm.
    /// </summary>
    internal class XFSankeyLayout
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double NodeWidth { get; set; } = 24;
        public double NodePadding { get; set; } = 8;
        public XFSankeyAlignment Alignment { get; set; } = XFSankeyAlignment.Justify;

        private const int MaxIterations = 32;

        /// <summary>
        /// Computes the layout for given nodes and links.
        /// Returns computed nodes and links with positions and SVG paths.
        /// </summary>
        public (IList<XFSankeyComputedNode> Nodes, IList<XFSankeyComputedLink> Links) Compute(
            IEnumerable<XFSankeyNode> nodes, IEnumerable<XFSankeyLink> links)
        {
            var computedNodes = nodes.Select(n => new XFSankeyComputedNode
            {
                Id = n.Id,
                Label = n.Label,
                ExplicitValue = n.ExplicitValue,
                Width = NodeWidth,
                Y = 0,
                Height = 0
            }).ToList();

            var nodeById = computedNodes.ToDictionary(
                n => n.Id,
                StringComparer.Ordinal);

            var computedLinks = new List<XFSankeyComputedLink>();
            foreach (var link in links)
            {
                if (!nodeById.TryGetValue(link.Source, out var sourceNode) ||
                    !nodeById.TryGetValue(link.Target, out var targetNode))
                    continue;

                var computedLink = new XFSankeyComputedLink
                {
                    Source = link.Source,
                    Target = link.Target,
                    Value = link.Value,
                    SourceNode = sourceNode,
                    TargetNode = targetNode
                };
                computedLinks.Add(computedLink);
                sourceNode.SourceLinks.Add(computedLink);
                targetNode.TargetLinks.Add(computedLink);
            }

            ComputeNodeValues(computedNodes);
            ComputeNodeLayers(computedNodes);
            ComputeNodeBreadths(computedNodes);
            ComputeNodeDepths(computedNodes);
            ComputeLinkPaths(computedLinks);

            return (computedNodes, computedLinks);
        }

        private void ComputeNodeValues(List<XFSankeyComputedNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.ExplicitValue.HasValue)
                {
                    node.ComputedValue = node.ExplicitValue.Value;
                }
                else
                {
                    var incoming = node.TargetLinks.Sum(l => l.Value);
                    var outgoing = node.SourceLinks.Sum(l => l.Value);
                    node.ComputedValue = Math.Max(incoming, outgoing);
                }
            }
        }

        private void ComputeNodeLayers(List<XFSankeyComputedNode> nodes)
        {
            var sources = nodes.Where(n => n.TargetLinks.Count == 0).ToList();
            var visited = new HashSet<XFSankeyComputedNode>();

            var queue = new Queue<XFSankeyComputedNode>();
            foreach (var source in sources)
            {
                source.Layer = 0;
                visited.Add(source);
                queue.Enqueue(source);
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                foreach (var link in node.SourceLinks)
                {
                    var target = link.TargetNode;
                    if (target == null) continue;

                    if (!visited.Contains(target))
                    {
                        target.Layer = node.Layer + 1;
                        visited.Add(target);
                        queue.Enqueue(target);
                    }
                    else
                    {
                        target.Layer = Math.Max(target.Layer, node.Layer + 1);
                    }
                }
            }

            // Handle disconnected nodes
            foreach (var node in nodes)
            {
                if (!visited.Contains(node))
                    node.Layer = 0;
            }
        }

        private void ComputeNodeBreadths(List<XFSankeyComputedNode> nodes)
        {
            var maxLayer = nodes.Count > 0 ? nodes.Max(n => n.Layer) : 0;
            var nodesByLayer = nodes.GroupBy(n => n.Layer).OrderBy(g => g.Key).ToList();

            foreach (var layer in nodesByLayer)
            {
                var layerNodes = layer.ToList();

                foreach (var node in layerNodes)
                {
                    if (maxLayer == 0)
                    {
                        node.X = (Width - NodeWidth) / 2;
                    }
                    else
                    {
                        var layerSpacing = (Width - NodeWidth) / maxLayer;

                        switch (Alignment)
                        {
                            case XFSankeyAlignment.Left:
                                var leftSpacing = Math.Min(layerSpacing, (Width * 0.7) / maxLayer);
                                node.X = node.Layer * leftSpacing;
                                break;
                            case XFSankeyAlignment.Right:
                                var rightSpacing = Math.Min(layerSpacing, (Width * 0.7) / maxLayer);
                                var totalRightWidth = maxLayer * rightSpacing;
                                node.X = Width - NodeWidth - totalRightWidth + (node.Layer * rightSpacing);
                                break;
                            case XFSankeyAlignment.Center:
                                var centerSpacing = layerSpacing * 0.8;
                                var totalWidth = maxLayer * centerSpacing;
                                var startX = (Width - totalWidth - NodeWidth) / 2;
                                node.X = startX + node.Layer * centerSpacing;
                                break;
                            case XFSankeyAlignment.Justify:
                            default:
                                node.X = node.Layer * layerSpacing;
                                break;
                        }
                    }
                }
            }
        }

        private void ComputeNodeDepths(List<XFSankeyComputedNode> nodes)
        {
            var nodesByLayer = nodes.GroupBy(n => n.Layer).OrderBy(g => g.Key).ToList();

            foreach (var layer in nodesByLayer)
            {
                InitializeNodeDepths(layer.ToList());
            }

            ResolveCollisions(nodesByLayer);
            AssignLinkDepths(nodes);
        }

        private void InitializeNodeDepths(List<XFSankeyComputedNode> nodes)
        {
            if (nodes.Count == 0) return;

            var totalValue = nodes.Sum(n => n.ComputedValue);
            var totalPadding = Math.Max(0, (nodes.Count - 1) * NodePadding);
            var availableHeight = Math.Max(1, Height - totalPadding);

            if (totalValue > 0)
            {
                var valueScale = availableHeight / totalValue;
                var y = 0.0;

                foreach (var node in nodes)
                {
                    node.Height = Math.Max(3, node.ComputedValue * valueScale);
                    node.Y = y;
                    y += node.Height + NodePadding;
                }
            }
            else
            {
                var nodeHeight = Math.Max(1, availableHeight / nodes.Count);
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Height = nodeHeight;
                    nodes[i].Y = i * (nodeHeight + NodePadding);
                }
            }
        }

        private void ResolveCollisions(List<IGrouping<int, XFSankeyComputedNode>> nodesByLayer)
        {
            for (int iteration = 0; iteration < MaxIterations; iteration++)
            {
                var alpha = 1 - Math.Pow(0.001, 1.0 / MaxIterations);

                // Forward pass
                for (int i = 1; i < nodesByLayer.Count; i++)
                {
                    RelaxLeftToRight(nodesByLayer[i].ToList(), alpha);
                }

                // Backward pass
                for (int i = nodesByLayer.Count - 2; i >= 0; i--)
                {
                    RelaxRightToLeft(nodesByLayer[i].ToList(), alpha);
                }
            }
        }

        private void RelaxLeftToRight(List<XFSankeyComputedNode> nodes, double alpha)
        {
            foreach (var node in nodes)
            {
                if (node.TargetLinks.Count > 0 && node.ComputedValue > 0)
                {
                    var y = node.TargetLinks.Sum(l =>
                        ((l.SourceNode?.Y ?? 0) + l.Y0) * l.Value) / node.ComputedValue;
                    node.Y += (y - node.Y) * alpha;
                }
            }

            ResolveOverlaps(nodes);
        }

        private void RelaxRightToLeft(List<XFSankeyComputedNode> nodes, double alpha)
        {
            foreach (var node in nodes)
            {
                if (node.SourceLinks.Count > 0 && node.ComputedValue > 0)
                {
                    var y = node.SourceLinks.Sum(l =>
                        ((l.TargetNode?.Y ?? 0) + l.Y1) * l.Value) / node.ComputedValue;
                    node.Y += (y - node.Y) * alpha;
                }
            }

            ResolveOverlaps(nodes);
        }

        private void ResolveOverlaps(List<XFSankeyComputedNode> nodes)
        {
            nodes = nodes.OrderBy(n => n.Y).ToList();
            var y0 = 0.0;
            foreach (var node in nodes)
            {
                var dy = y0 - node.Y;
                if (dy > 0) node.Y += dy;
                y0 = node.Y + node.Height + NodePadding;
            }

            if (nodes.Count == 0) return;

            var minY = nodes.Min(n => n.Y);
            var maxY = nodes.Max(n => n.Y + n.Height);
            var totalHeight = maxY - minY;

            if (minY != 0)
            {
                foreach (var node in nodes) node.Y -= minY;
                maxY -= minY;
                minY = 0;
            }

            if (totalHeight > Height)
            {
                var scale = Height / totalHeight;
                foreach (var node in nodes)
                {
                    node.Y *= scale;
                    node.Height *= scale;
                }
            }
            else
            {
                var offset = Math.Min((Height - totalHeight) / 2, Height * 0.1);
                foreach (var node in nodes) node.Y += offset;
            }
        }

        private void AssignLinkDepths(List<XFSankeyComputedNode> nodes)
        {
            foreach (var node in nodes)
            {
                var outgoingSum = node.SourceLinks.Sum(l => l.Value);
                if (outgoingSum > 0)
                {
                    var y0 = 0.0;
                    var sorted = node.SourceLinks.OrderBy(l => l.TargetNode?.Y ?? 0).ToList();
                    foreach (var link in sorted)
                    {
                        link.Y0 = y0;
                        link.WidthSource = (link.Value / outgoingSum) * node.Height;
                        link.Width = link.WidthSource;
                        y0 += link.WidthSource;
                    }
                }
            }

            foreach (var node in nodes)
            {
                var incomingSum = node.TargetLinks.Sum(l => l.Value);
                if (incomingSum > 0)
                {
                    var y1 = 0.0;
                    var sorted = node.TargetLinks.OrderBy(l => l.SourceNode?.Y ?? 0).ToList();
                    foreach (var link in sorted)
                    {
                        link.Y1 = y1;
                        link.WidthTarget = (link.Value / incomingSum) * node.Height;
                        y1 += link.WidthTarget;
                    }
                }
            }
        }

        private void ComputeLinkPaths(List<XFSankeyComputedLink> links)
        {
            foreach (var link in links)
            {
                if (link.SourceNode == null || link.TargetNode == null) continue;

                var x0 = link.SourceNode.X + link.SourceNode.Width;
                var x1 = link.TargetNode.X;
                var y0 = link.SourceNode.Y + link.Y0;
                var y1 = link.TargetNode.Y + link.Y1;
                var ws = link.WidthSource > 0 ? link.WidthSource : link.Width;
                var wt = link.WidthTarget > 0 ? link.WidthTarget : link.Width;
                var xi = (x0 + x1) / 2;

                link.Path = $"M{F(x0)},{F(y0)} C{F(xi)},{F(y0)} {F(xi)},{F(y1)} {F(x1)},{F(y1)} " +
                           $"L{F(x1)},{F(y1 + wt)} C{F(xi)},{F(y1 + wt)} {F(xi)},{F(y0 + ws)} {F(x0)},{F(y0 + ws)} Z";
            }
        }

        private static string F(double value) =>
            value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
    }
}
