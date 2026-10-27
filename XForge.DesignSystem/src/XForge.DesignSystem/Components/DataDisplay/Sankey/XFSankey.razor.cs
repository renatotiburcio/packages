using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.DataDisplay.Sankey
{
    /// <summary>
    /// Enterprise Sankey diagram component for visualizing flows between nodes.
    /// Pure SVG rendering with C# layout algorithm (no JS dependency).
    /// </summary>
    /// <typeparam name="TItem">The type of data item representing a flow/link.</typeparam>
    public partial class XFSankey<TItem> : XForgeComponentBase
    {
        private ElementReference ContainerRef;
        private List<XFSankeyComputedNode>? _computedNodes;
        private List<XFSankeyComputedLink>? _computedLinks;
        private double _width;
        private double _height;
        private bool _showTooltip;
        private double _tooltipX;
        private double _tooltipY;
        private RenderFragment? _tooltipContent;

        // Property selectors
        private Func<TItem, string>? _sourceGetter;
        private Func<TItem, string>? _targetGetter;
        private Func<TItem, double>? _valueGetter;
        private Func<TItem, string>? _sourceLabelGetter;
        private Func<TItem, string>? _targetLabelGetter;

        #region Parameters

        /// <summary>Whether the component is visible.</summary>
        [Parameter] public bool Visible { get; set; } = true;

        /// <summary>Data items. Each item represents one flow/link.</summary>
        [Parameter, EditorRequired] public IEnumerable<TItem>? Data { get; set; }

        /// <summary>Selector for the source node identifier.</summary>
        [Parameter, EditorRequired] public Func<TItem, string>? SourceSelector { get; set; }

        /// <summary>Selector for the target node identifier.</summary>
        [Parameter, EditorRequired] public Func<TItem, string>? TargetSelector { get; set; }

        /// <summary>Selector for the flow value.</summary>
        [Parameter, EditorRequired] public Func<TItem, double>? ValueSelector { get; set; }

        /// <summary>Optional selector for source node label (defaults to source ID).</summary>
        [Parameter] public Func<TItem, string>? SourceLabelSelector { get; set; }

        /// <summary>Optional selector for target node label (defaults to target ID).</summary>
        [Parameter] public Func<TItem, string>? TargetLabelSelector { get; set; }

        /// <summary>Width of node rectangles in SVG units.</summary>
        [Parameter] public double NodeWidth { get; set; } = 24;

        /// <summary>Vertical padding between nodes in SVG units.</summary>
        [Parameter] public double NodePadding { get; set; } = 8;

        /// <summary>Node alignment strategy.</summary>
        [Parameter] public XFSankeyAlignment Alignment { get; set; } = XFSankeyAlignment.Justify;

        /// <summary>Size variant.</summary>
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;

        /// <summary>Custom fill colors for nodes (indexed by node order).</summary>
        [Parameter] public IReadOnlyList<string>? NodeFills { get; set; }

        /// <summary>Custom fill colors for links (indexed by link order).</summary>
        [Parameter] public IReadOnlyList<string>? LinkFills { get; set; }

        /// <summary>Whether to animate flow in links.</summary>
        [Parameter] public bool Animated { get; set; }

        /// <summary>Custom value formatter for tooltips.</summary>
        [Parameter] public Func<double, string>? ValueFormatter { get; set; }

        /// <summary>Tooltip label for value.</summary>
        [Parameter] public string ValueLabel { get; set; } = "Valor";

        /// <summary>Tooltip label for incoming flow.</summary>
        [Parameter] public string IncomingLabel { get; set; } = "Entrada";

        /// <summary>Tooltip label for outgoing flow.</summary>
        [Parameter] public string OutgoingLabel { get; set; } = "Saída";

        /// <summary>Tooltip label for flow.</summary>
        [Parameter] public string FlowLabel { get; set; } = "Fluxo";

        /// <summary>Text to show when data is empty.</summary>
        [Parameter] public string? EmptyText { get; set; }

        /// <summary>Theme override (light/dark). Inherits from container if not set.</summary>
        [Parameter] public string? Theme { get; set; }

        /// <summary>Accessible label for the diagram.</summary>
        [Parameter] public string? AriaLabel { get; set; }

        /// <summary>Event raised when a node is clicked.</summary>
        [Parameter] public EventCallback<XFSankeyNodeClickEventArgs> OnNodeClick { get; set; }

        /// <summary>Event raised when a link is clicked.</summary>
        [Parameter] public EventCallback<XFSankeyLinkClickEventArgs> OnLinkClick { get; set; }

        #endregion

        #region Computed properties

        private double _marginLeft => Size switch
        {
            XFSize.Sm => 60,
            XFSize.Lg => 100,
            _ => 80
        };
        private double _marginRight => _marginLeft;
        private double _marginTop => 10;
        private double _marginBottom => 10;
        private double LabelPadding => Size switch
        {
            XFSize.Sm => 6,
            XFSize.Lg => 14,
            _ => 10
        };
        private int LabelFontSize => Size switch
        {
            XFSize.Sm => 10,
            XFSize.Lg => 14,
            _ => 12
        };
        private double EffectiveNodeWidth => Size switch
        {
            XFSize.Sm => Math.Min(NodeWidth, 18),
            XFSize.Lg => Math.Max(NodeWidth, 30),
            _ => NodeWidth
        };
        private double EffectiveNodePadding => Size switch
        {
            XFSize.Sm => Math.Min(NodePadding, 6),
            XFSize.Lg => Math.Max(NodePadding, 12),
            _ => NodePadding
        };

        private string ContainerClass
        {
            get
            {
                var baseClass = "xf-sankey relative w-full";
                var sizeClass = Size switch
                {
                    XFSize.Sm => "h-48",
                    XFSize.Lg => "h-96",
                    _ => "h-64"
                };
                return $"{baseClass} {sizeClass} {Class}".Trim();
            }
        }

        #endregion

        #region Lifecycle

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            var changed = false;

            if (_sourceGetter == null && SourceSelector != null)
            {
                _sourceGetter = SourceSelector;
                changed = true;
            }
            else if (_sourceGetter != SourceSelector && SourceSelector != null)
            {
                _sourceGetter = SourceSelector;
                changed = true;
            }

            if (_targetGetter == null && TargetSelector != null)
            {
                _targetGetter = TargetSelector;
                changed = true;
            }
            else if (_targetGetter != TargetSelector && TargetSelector != null)
            {
                _targetGetter = TargetSelector;
                changed = true;
            }

            if (_valueGetter == null && ValueSelector != null)
            {
                _valueGetter = ValueSelector;
                changed = true;
            }
            else if (_valueGetter != ValueSelector && ValueSelector != null)
            {
                _valueGetter = ValueSelector;
                changed = true;
            }

            _sourceLabelGetter = SourceLabelSelector;
            _targetLabelGetter = TargetLabelSelector;

            if (changed || _computedNodes == null)
            {
                ComputeLayout();
            }
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                // Use fallback dimensions (no JS interop in v1)
                if (_width <= 0 || _height <= 0)
                {
                    _width = 800;
                    _height = 400;
                    ComputeLayout();
                    StateHasChanged();
                }
            }
        }

        #endregion

        #region Layout

        private void ComputeLayout()
        {
            if (Data == null || _sourceGetter == null || _targetGetter == null || _valueGetter == null)
            {
                _computedNodes = null;
                _computedLinks = null;
                return;
            }

            var usableWidth = Math.Max(100, _width - _marginLeft - _marginRight);
            var usableHeight = Math.Max(100, _height - _marginTop - _marginBottom);

            var nodeMap = new Dictionary<string, XFSankeyNode>(StringComparer.Ordinal);
            var sankeyLinks = new List<XFSankeyLink>();

            foreach (var item in Data)
            {
                var source = _sourceGetter(item);
                var target = _targetGetter(item);
                var value = _valueGetter(item);

                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                    continue;

                if (!nodeMap.ContainsKey(source))
                {
                    nodeMap[source] = new XFSankeyNode
                    {
                        Id = source,
                        Label = _sourceLabelGetter?.Invoke(item) ?? source
                    };
                }

                if (!nodeMap.ContainsKey(target))
                {
                    nodeMap[target] = new XFSankeyNode
                    {
                        Id = target,
                        Label = _targetLabelGetter?.Invoke(item) ?? target
                    };
                }

                sankeyLinks.Add(new XFSankeyLink
                {
                    Source = source,
                    Target = target,
                    Value = value
                });
            }

            var layout = new XFSankeyLayout
            {
                Width = usableWidth,
                Height = usableHeight,
                NodeWidth = EffectiveNodeWidth,
                NodePadding = EffectiveNodePadding,
                Alignment = Alignment
            };

            var (nodes, links) = layout.Compute(nodeMap.Values, sankeyLinks);
            _computedNodes = nodes.ToList();
            _computedLinks = links.ToList();

            // Assign color indices
            for (int i = 0; i < _computedNodes.Count; i++)
            {
                _computedNodes[i].ColorIndex = i;
            }
        }

        #endregion

        #region Color helpers

        private string? GetNodeFill(XFSankeyComputedNode node)
        {
            if (NodeFills != null && _computedNodes != null)
            {
                var index = _computedNodes.IndexOf(node);
                if (index >= 0 && index < NodeFills.Count)
                    return NodeFills[index];
            }
            return null;
        }

        private string? GetLinkFill(XFSankeyComputedLink link, int index)
        {
            if (LinkFills != null && index < LinkFills.Count)
            {
                var fill = LinkFills[index];
                if (fill.Length == 7 && fill.StartsWith('#'))
                    return fill + "80"; // Add 50% alpha
                return fill;
            }

            // Inherit from source node with alpha
            var nodeFill = link.SourceNode != null ? GetNodeFill(link.SourceNode) : null;
            if (nodeFill != null && nodeFill.Length == 7 && nodeFill.StartsWith('#'))
                return nodeFill + "80";

            return null; // Use CSS class
        }

        private string GetLinkBaseColor(XFSankeyComputedLink link)
        {
            var fill = link.SourceNode != null ? GetNodeFill(link.SourceNode) : null;
            if (fill != null && fill.StartsWith('#'))
                return fill.Length > 7 ? fill[..7] : fill;
            return "var(--color-primary, #6366f1)";
        }

        #endregion

        #region Event handlers

        private void HandleNodeClick(XFSankeyComputedNode node)
        {
            if (!OnNodeClick.HasDelegate) return;

            var args = new XFSankeyNodeClickEventArgs
            {
                NodeId = node.Id,
                Label = node.Label,
                Value = node.ComputedValue,
                Incoming = node.TargetLinks.Sum(l => l.Value),
                Outgoing = node.SourceLinks.Sum(l => l.Value)
            };
            _ = OnNodeClick.InvokeAsync(args);
        }

        private void HandleLinkClick(XFSankeyComputedLink link)
        {
            if (!OnLinkClick.HasDelegate) return;

            var args = new XFSankeyLinkClickEventArgs
            {
                Source = link.Source,
                Target = link.Target,
                Value = link.Value
            };
            _ = OnLinkClick.InvokeAsync(args);
        }

        private void HandleNodeMouseOver(MouseEventArgs args, XFSankeyComputedNode node)
        {
            var valueStr = FormatValue(node.ComputedValue);
            var incoming = node.TargetLinks.Sum(l => l.Value);
            var outgoing = node.SourceLinks.Sum(l => l.Value);

            _tooltipContent = builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "xf-sankey-tooltip-title font-semibold mb-1");
                builder.AddContent(2, node.Label ?? node.Id);
                builder.CloseElement();

                builder.OpenElement(3, "div");
                builder.AddContent(4, $"{ValueLabel}: ");
                builder.OpenElement(5, "strong");
                builder.AddContent(6, valueStr);
                builder.CloseElement();
                builder.CloseElement();

                if (outgoing > 0)
                {
                    builder.OpenElement(7, "div");
                    builder.AddContent(8, $"{OutgoingLabel}: ");
                    builder.OpenElement(9, "strong");
                    builder.AddContent(10, FormatValue(outgoing));
                    builder.CloseElement();
                    builder.CloseElement();
                }

                if (incoming > 0)
                {
                    builder.OpenElement(11, "div");
                    builder.AddContent(12, $"{IncomingLabel}: ");
                    builder.OpenElement(13, "strong");
                    builder.AddContent(14, FormatValue(incoming));
                    builder.CloseElement();
                    builder.CloseElement();
                }
            };

            ShowTooltip(args);
        }

        private void HandleLinkMouseOver(MouseEventArgs args, XFSankeyComputedLink link)
        {
            var sourceLabel = link.SourceNode?.Label ?? link.Source;
            var targetLabel = link.TargetNode?.Label ?? link.Target;

            _tooltipContent = builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "xf-sankey-tooltip-title font-semibold mb-1");
                builder.AddContent(2, $"{sourceLabel} → {targetLabel}");
                builder.CloseElement();

                builder.OpenElement(3, "div");
                builder.AddContent(4, $"{FlowLabel}: ");
                builder.OpenElement(5, "strong");
                builder.AddContent(6, FormatValue(link.Value));
                builder.CloseElement();
                builder.CloseElement();
            };

            ShowTooltip(args);
        }

        private void HandleMouseOut()
        {
            _showTooltip = false;
            _tooltipContent = null;
        }

        private void ShowTooltip(MouseEventArgs args)
        {
            _tooltipX = args.ClientX + 12;
            _tooltipY = args.ClientY - 10;
            _showTooltip = true;
        }

        private string FormatValue(double value)
        {
            if (ValueFormatter != null) return ValueFormatter(value);
            return value.ToString("N0", CultureInfo.InvariantCulture);
        }

        #endregion

        #region Helpers

        private static string F(double value) =>
            value.ToString("F2", CultureInfo.InvariantCulture);

        #endregion
    }
}
