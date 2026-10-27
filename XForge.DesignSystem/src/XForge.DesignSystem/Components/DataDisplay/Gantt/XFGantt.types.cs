using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.DataDisplay.Gantt
{
    /// <summary>
    /// View mode for the Gantt timeline.
    /// </summary>
    public enum XFGanttView
    {
        /// <summary>Day-level scale — columns represent hours.</summary>
        Day,
        /// <summary>Week-level scale — columns represent days.</summary>
        Week,
        /// <summary>Month-level scale — columns represent days.</summary>
        Month,
        /// <summary>Year-level scale — columns represent months.</summary>
        Year
    }

    /// <summary>
    /// Size variant for the Gantt chart.
    /// </summary>
/// <summary>
    /// Visual density variant.
    /// </summary>
    public enum XFGanttVariant
    {
        /// <summary>Default density.</summary>
        Default,
        /// <summary>Compact — tighter spacing.</summary>
        Compact,
        /// <summary>Comfortable — more whitespace.</summary>
        Comfortable
    }

    /// <summary>
    /// Dependency link type between two tasks.
    /// </summary>
    public enum XFGanttDependencyType
    {
        /// <summary>Finish-to-Start: successor cannot start until predecessor finishes.</summary>
        FinishToStart,
        /// <summary>Start-to-Start: successor cannot start until predecessor starts.</summary>
        StartToStart,
        /// <summary>Finish-to-Finish: successor cannot finish until predecessor finishes.</summary>
        FinishToFinish,
        /// <summary>Start-to-Finish: successor cannot finish until predecessor starts.</summary>
        StartToFinish
    }

    /// <summary>
    /// Represents a dependency link between two tasks.
    /// </summary>
    public class XFGanttDependency
    {
        /// <summary>ID of the predecessor task.</summary>
        public string FromId { get; set; } = string.Empty;

        /// <summary>ID of the successor task.</summary>
        public string ToId { get; set; } = string.Empty;

        /// <summary>Type of dependency.</summary>
        public XFGanttDependencyType Type { get; set; } = XFGanttDependencyType.FinishToStart;
    }

    /// <summary>
    /// Event args for task click events.
    /// </summary>
    public class XFGanttTaskClickEventArgs<TItem>
    {
        /// <summary>The clicked task item.</summary>
        public TItem Item { get; init; } = default!;

        /// <summary>Task start date.</summary>
        public DateTime Start { get; init; }

        /// <summary>Task end date.</summary>
        public DateTime End { get; init; }
    }

    /// <summary>
    /// Event args for task move events.
    /// </summary>
    public class XFGanttTaskMoveEventArgs<TItem>
    {
        /// <summary>The moved task item.</summary>
        public TItem Item { get; init; } = default!;

        /// <summary>Original start date before move.</summary>
        public DateTime OriginalStart { get; init; }

        /// <summary>Original end date before move.</summary>
        public DateTime OriginalEnd { get; init; }

        /// <summary>New start date after move.</summary>
        public DateTime NewStart { get; init; }

        /// <summary>New end date after move.</summary>
        public DateTime NewEnd { get; init; }
    }

    /// <summary>
    /// Event args for task edit events.
    /// </summary>
    public class XFGanttTaskEditEventArgs<TItem>
    {
        /// <summary>The edited task item.</summary>
        public TItem Item { get; init; } = default!;

        /// <summary>Original title before edit.</summary>
        public string OriginalTitle { get; init; } = string.Empty;

        /// <summary>New title after edit.</summary>
        public string NewTitle { get; init; } = string.Empty;
    }

    /// <summary>
    /// Event args for task resize events.
    /// </summary>
    public class XFGanttTaskResizeEventArgs<TItem>
    {
        /// <summary>The resized task item.</summary>
        public TItem Item { get; init; } = default!;

        /// <summary>Original start date before resize.</summary>
        public DateTime OriginalStart { get; init; }

        /// <summary>Original end date before resize.</summary>
        public DateTime OriginalEnd { get; init; }

        /// <summary>New end date after resize.</summary>
        public DateTime NewEnd { get; init; }
    }

    /// <summary>
    /// Zoom level for the Gantt chart.
    /// </summary>
    public enum XFGanttZoomLevel
    {
        Day,
        Week,
        Month
    }

    /// <summary>
    /// Represents a resource that can be assigned to Gantt tasks.
    /// </summary>
    public record XFGanttResource
    {
        /// <summary>Unique identifier for the resource.</summary>
        public string Id { get; init; } = string.Empty;

        /// <summary>Display name of the resource.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Optional color for the resource indicator.</summary>
        public string? Color { get; set; }
    }

    /// <summary>
    /// Internal representation of a Gantt task node in the tree.
    /// </summary>
    internal class XFGanttNode<TItem>
    {
        public string Id { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double Progress { get; set; }
        public TItem Item { get; set; } = default!;
        public int Depth { get; set; }
        public bool IsExpanded { get; set; } = true;
        public bool HasChildren { get; set; }
        public List<XFGanttNode<TItem>> Children { get; set; } = new();
    }
}
