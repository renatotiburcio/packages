using System.Text;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Diagram
{
    public partial class XFDiagramAdvanced : XForgeComponentBase
    {
        [Parameter] public IReadOnlyList<DiagramNode> Nodes { get; set; } = new List<DiagramNode>();
        [Parameter] public IReadOnlyList<DiagramEdge> Edges { get; set; } = new List<DiagramEdge>();
        [Parameter] public string Layout { get; set; } = "manual";
        [Parameter] public string StyleLibrary { get; set; } = "default";
        [Parameter] public int Width { get; set; } = 1000;
        [Parameter] public int Height { get; set; } = 700;
        [Parameter] public EventCallback<DiagramNode> OnNodeClick { get; set; }

        private string Render()
        {
            var sb = new StringBuilder();
            foreach (var e in Edges)
            {
                var f = Nodes.FirstOrDefault(n => n.Id == e.FromId);
                var t = Nodes.FirstOrDefault(n => n.Id == e.ToId);
                if (f != null && t != null)
                    sb.Append($"<line x1=\"{f.X}\" y1=\"{f.Y}\" x2=\"{t.X}\" y2=\"{t.Y}\" stroke=\"#666\"/>");
            }
            foreach (var n in Nodes)
            {
                sb.Append($"<rect x=\"{n.X - 40}\" y=\"{n.Y - 15}\" width=\"80\" height=\"30\" fill=\"{n.Color ?? "#3b82f6"}\" rx=\"4\"/><text x=\"{n.X}\" y=\"{n.Y + 5}\" text-anchor=\"middle\" fill=\"#fff\">{n.Label}</text>");
            }
            return sb.ToString();
        }
    }
}
