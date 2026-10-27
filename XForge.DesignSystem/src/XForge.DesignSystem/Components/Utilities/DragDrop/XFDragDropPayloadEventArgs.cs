namespace XForge.DesignSystem.Components.Utilities.DragDrop
{
    public sealed class XFDragDropPayloadEventArgs<TPayload>
    {
        public XFDragDropPayloadEventArgs(XFDragDropEventArgs source, TPayload? payload)
        {
            Source = source;
            Payload = payload;
        }

        public XFDragDropEventArgs Source { get; }
        public TPayload? Payload { get; }
        public bool Accepted => Source.Accepted;
        public string ZoneId => Source.ZoneId;
        public IReadOnlyList<string> Types => Source.Types;
        public IReadOnlyList<string> Files => Source.Files;
    }
}
