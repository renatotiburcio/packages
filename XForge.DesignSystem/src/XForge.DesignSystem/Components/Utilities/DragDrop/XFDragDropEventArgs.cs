using Microsoft.AspNetCore.Components.Web;

namespace XForge.DesignSystem.Components.Utilities.DragDrop
{
    public sealed class XFDragDropEventArgs
    {
        public XFDragDropEventArgs(DragEventArgs source, string zoneId, bool accepted)
        {
            Source = source;
            ZoneId = zoneId;
            Accepted = accepted;
        }

        public DragEventArgs Source { get; }
        public string ZoneId { get; }
        public bool Accepted { get; }
        public IReadOnlyList<string> Types => Source.DataTransfer?.Types ?? Array.Empty<string>();
        public IReadOnlyList<string> Files => Source.DataTransfer?.Files ?? Array.Empty<string>();
    }
}
