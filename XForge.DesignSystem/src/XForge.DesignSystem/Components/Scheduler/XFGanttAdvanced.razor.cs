using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Gantt
{
    public partial class XFGanttAdvanced : XForgeComponentBase
    {
        #region Parameters

        [Parameter] public IReadOnlyList<GanttTask> Tasks { get; set; } = Array.Empty<GanttTask>();
        [Parameter] public IReadOnlyList<GanttDependency> Dependencies { get; set; } = Array.Empty<GanttDependency>();
        [Parameter] public bool ShowTreePanel { get; set; } = true;
        [Parameter] public bool ShowDependencies { get; set; } = true;
        [Parameter] public bool ShowCriticalPath { get; set; } = true;
        [Parameter] public bool ShowBaseline { get; set; }
        [Parameter] public bool AllowDragToResize { get; set; } = true;
        [Parameter] public bool AllowInlineEdit { get; set; } = true;
        [Parameter] public bool AllowCollapse { get; set; } = true;
        [Parameter] public int RowHeight { get; set; } = 36;
        [Parameter] public int ColumnWidth { get; set; } = 80;
        [Parameter] public EventCallback<GanttTask> OnTaskClick { get; set; }
        [Parameter] public EventCallback<GanttTask> OnTaskUpdate { get; set; }
        [Parameter] public EventCallback<GanttTask> OnTaskCreate { get; set; }
        [Parameter] public EventCallback<string> OnTaskDelete { get; set; }
        [Parameter] public EventCallback<GanttDependency> OnDependencyCreate { get; set; }
        [Parameter] public DateTime? TimelineStart { get; set; }
        [Parameter] public DateTime? TimelineEnd { get; set; }
        [Parameter] public GanttViewMode ViewMode { get; set; } = GanttViewMode.Week;

        #endregion

        #region Internal State

        private HashSet<string> _collapsedIds = new();
        private GanttTask? _selectedTask;
        private GanttTask? _editingTask;
        private string _editValue = "";
        private bool _isDragging;
        private GanttTask? _draggedTask;
        private double _dragStartX;
        private DateTime _dragOriginalStart;
        private DateTime _dragOriginalEnd;
        private bool _showCreateModal;
        private GanttTaskModalModel _newTask = new();

        // Timeline
        private DateTime _timelineStart;
        private DateTime _timelineEnd;
        private int _totalDays;
        private int _timelineWidth;
        private int _calculatedHeight;

        #endregion

        #region Lifecycle

        protected override void OnInitialized()
        {
            CalculateTimeline();
        }

        protected override void OnParametersSet()
        {
            CalculateTimeline();
        }

        private void CalculateTimeline()
        {
            if (Tasks.Any())
            {
                _timelineStart = TimelineStart ?? Tasks.Min(t => t.Start).Date;
                _timelineEnd = TimelineEnd ?? Tasks.Max(t => t.End).Date.AddDays(1);
            }
            else
            {
                _timelineStart = TimelineStart ?? DateTime.Today.AddDays(-7);
                _timelineEnd = TimelineEnd ?? DateTime.Today.AddDays(30);
            }

            _totalDays = (int)(_timelineEnd - _timelineStart).TotalDays;
            _timelineWidth = _totalDays * ColumnWidth;
            _calculatedHeight = GetVisibleTasks().Count * RowHeight;
        }

        #endregion

        #region Tree Hierarchy

        private List<GanttTask> GetRootTasks()
        {
            return Tasks.Where(t => string.IsNullOrEmpty(t.ParentId) || !Tasks.Any(p => p.Id == t.ParentId))
                       .OrderBy(t => t.Start)
                       .ToList();
        }

        private List<GanttTask> GetChildTasks(string parentId)
        {
            return Tasks.Where(t => t.ParentId == parentId)
                       .OrderBy(t => t.Start)
                       .ToList();
        }

        private int GetTaskDepth(GanttTask task)
        {
            int depth = 0;
            var current = task;
            while (!string.IsNullOrEmpty(current.ParentId))
            {
                var parent = Tasks.FirstOrDefault(t => t.Id == current.ParentId);
                if (parent == null) break;
                current = parent;
                depth++;
            }
            return depth;
        }

        private bool HasChildren(GanttTask task)
        {
            return Tasks.Any(t => t.ParentId == task.Id);
        }

        private bool IsCollapsed(string taskId)
        {
            return _collapsedIds.Contains(taskId);
        }

        private void ToggleCollapse(string taskId)
        {
            if (_collapsedIds.Contains(taskId))
                _collapsedIds.Remove(taskId);
            else
                _collapsedIds.Add(taskId);
        }

        private List<GanttTask> GetVisibleTasks()
        {
            var result = new List<GanttTask>();
            foreach (var task in GetRootTasks())
            {
                AddVisibleTasks(task, result, 0);
            }
            return result;
        }

        private void AddVisibleTasks(GanttTask task, List<GanttTask> result, int depth)
        {
            result.Add(task);
            if (!IsCollapsed(task.Id))
            {
                foreach (var child in GetChildTasks(task.Id))
                {
                    AddVisibleTasks(child, result, depth + 1);
                }
            }
        }

        #endregion

        #region Timeline Helpers

        private double GetTaskX(GanttTask task)
        {
            var offset = (task.Start - _timelineStart).TotalDays;
            return offset * ColumnWidth;
        }

        private double GetTaskWidth(GanttTask task)
        {
            var duration = (task.End - task.Start).TotalDays;
            return Math.Max(duration * ColumnWidth, 4);
        }

        private int GetTaskY(GanttTask task)
        {
            var visibleTasks = GetVisibleTasks();
            var index = visibleTasks.FindIndex(t => t.Id == task.Id);
            return index >= 0 ? index * RowHeight + 4 : 0;
        }

        private string GetTaskColor(GanttTask task)
        {
            if (task.IsCritical) return "#ef4444";
            if (task.IsMilestone) return "#8b5cf6";
            return "#3b82f6";
        }

        #endregion

        #region Drag & Drop

        private void OnTaskPointerDown(GanttTask task, PointerEventArgs e)
        {
            if (!AllowDragToResize) return;

            _draggedTask = task;
            _dragStartX = e.ClientX;
            _dragOriginalStart = task.Start;
            _dragOriginalEnd = task.End;
            _isDragging = true;
        }

        private void OnTimelinePointerMove(PointerEventArgs e)
        {
            if (!_isDragging || _draggedTask == null) return;

            var deltaX = e.ClientX - _dragStartX;
            var daysDelta = deltaX / ColumnWidth;

            var newStart = _dragOriginalStart.AddDays(daysDelta);
            var newEnd = _dragOriginalEnd.AddDays(daysDelta);

            // Update task temporarily for visual feedback
            var index = Tasks.ToList().FindIndex(t => t.Id == _draggedTask.Id);
            if (index >= 0)
            {
                var updated = _draggedTask with { Start = newStart, End = newEnd };
                var list = Tasks.ToList();
                list[index] = updated;
                Tasks = list;
                _draggedTask = updated;
            }
        }

        private async Task OnTimelinePointerUp(PointerEventArgs e)
        {
            if (!_isDragging || _draggedTask == null) return;

            _isDragging = false;
            await OnTaskUpdate.InvokeAsync(_draggedTask);
            _draggedTask = null;
        }

        #endregion

        #region Inline Edit

        private void StartEdit(GanttTask task)
        {
            if (!AllowInlineEdit) return;
            _editingTask = task;
            _editValue = task.Name;
        }

        private async Task FinishEdit(GanttTask task)
        {
            if (_editingTask == null || _editingTask.Id != task.Id) return;

            var updated = task with { Name = _editValue };
            _editingTask = null;
            _editValue = "";
            await OnTaskUpdate.InvokeAsync(updated);
        }

        private void CancelEdit()
        {
            _editingTask = null;
            _editValue = "";
        }

        private void OnEditKeyDown(KeyboardEventArgs e, GanttTask task)
        {
            if (e.Key == "Enter")
                _ = FinishEdit(task);
            else if (e.Key == "Escape")
                CancelEdit();
        }

        #endregion

        #region Selection

        private void SelectTask(GanttTask task)
        {
            _selectedTask = task;
            _ = OnTaskClick.InvokeAsync(task);
        }

        private bool IsSelected(GanttTask task)
        {
            return _selectedTask?.Id == task.Id;
        }

        #endregion

        #region Create Task

        private void OpenCreateModal()
        {
            _newTask = new GanttTaskModalModel
            {
                Name = "",
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(1)
            };
            _showCreateModal = true;
        }

        private void CloseCreateModal()
        {
            _showCreateModal = false;
        }

        private async Task CreateTask()
        {
            if (string.IsNullOrWhiteSpace(_newTask.Name)) return;

            var task = new GanttTask(
                Guid.NewGuid().ToString("N"),
                _newTask.Name,
                _newTask.Start,
                _newTask.End,
                0,
                null,
                _newTask.IsMilestone
            );

            await OnTaskCreate.InvokeAsync(task);
            CloseCreateModal();
        }

        #endregion

        #region Delete Task

        private async Task DeleteTask(GanttTask task)
        {
            await OnTaskDelete.InvokeAsync(task.Id);
            if (_selectedTask?.Id == task.Id)
                _selectedTask = null;
        }

        #endregion

        #region Render Helpers

        private string RenderTimelineHeader()
        {
            var sb = new StringBuilder();
            var current = _timelineStart;

            while (current < _timelineEnd)
            {
                var x = (current - _timelineStart).TotalDays * ColumnWidth;
                var label = ViewMode switch
                {
                    GanttViewMode.Day => current.ToString("dd/MM"),
                    GanttViewMode.Week => current.ToString("dd MMM"),
                    GanttViewMode.Month => current.ToString("MMM yyyy"),
                    _ => current.ToString("dd/MM")
                };

                sb.Append($"<text x=\"{x + 4}\" y=\"14\" font-size=\"11\" fill=\"#6b7280\">{label}</text>");
                sb.Append($"<line x1=\"{x}\" y1=\"20\" x2=\"{x}\" y2=\"{_calculatedHeight}\" stroke=\"#e5e7eb\" />");

                current = ViewMode switch
                {
                    GanttViewMode.Day => current.AddDays(1),
                    GanttViewMode.Week => current.AddDays(7),
                    GanttViewMode.Month => current.AddMonths(1),
                    _ => current.AddDays(1)
                };
            }

            return sb.ToString();
        }

        private string RenderTaskBars()
        {
            var sb = new StringBuilder();
            var visibleTasks = GetVisibleTasks();

            for (int i = 0; i < visibleTasks.Count; i++)
            {
                var task = visibleTasks[i];
                var x = GetTaskX(task);
                var w = GetTaskWidth(task);
                var y = i * RowHeight + 8;
                var color = GetTaskColor(task);
                var isSelected = IsSelected(task);
                var depth = GetTaskDepth(task);

                // Row background
                if (isSelected)
                    sb.Append($"<rect x=\"0\" y=\"{i * RowHeight}\" width=\"{_timelineWidth}\" height=\"{RowHeight}\" fill=\"#eff6ff\" />");

                // Task bar
                sb.Append($"<rect x=\"{x}\" y=\"{y}\" width=\"{w}\" height=\"20\" rx=\"4\" fill=\"{color}\" " +
                         $"style=\"cursor:pointer\" @onclick=\"\" />");

                // Progress
                if (task.Progress > 0)
                    sb.Append($"<rect x=\"{x}\" y=\"{y}\" width=\"{w * task.Progress / 100}\" height=\"20\" rx=\"4\" fill=\"rgba(255,255,255,0.4)\" />");

                // Milestone diamond
                if (task.IsMilestone)
                    sb.Append($"<polygon points=\"{x},{y + 10} {x + 10},{y} {x + 20},{y + 10} {x + 10},{y + 20}\" fill=\"{color}\" />");

                // Task name (if space allows)
                if (w > 60)
                    sb.Append($"<text x=\"{x + 4}\" y=\"{y + 14}\" font-size=\"11\" fill=\"white\" font-weight=\"500\">{TruncateText(task.Name, 15)}</text>");
            }

            return sb.ToString();
        }

        private string RenderDependencies()
        {
            if (!ShowDependencies) return "";

            var sb = new StringBuilder();
            var visibleTasks = GetVisibleTasks();

            foreach (var dep in Dependencies)
            {
                var fromTask = visibleTasks.FirstOrDefault(t => t.Id == dep.FromId);
                var toTask = visibleTasks.FirstOrDefault(t => t.Id == dep.ToId);
                if (fromTask == null || toTask == null) continue;

                var fromX = GetTaskX(fromTask) + GetTaskWidth(fromTask);
                var fromY = GetTaskY(fromTask) + 10;
                var toX = GetTaskX(toTask);
                var toY = GetTaskY(toTask) + 10;

                var midX = (fromX + toX) / 2;

                sb.Append($"<path d=\"M {fromX} {fromY} C {midX} {fromY}, {midX} {toY}, {toX} {toY}\" " +
                         $"fill=\"none\" stroke=\"#9ca3af\" stroke-width=\"1.5\" marker-end=\"url(#arrowhead)\" />");
            }

            return sb.ToString();
        }

        private string TruncateText(string text, int maxLength)
        {
            return text.Length > maxLength ? text[..maxLength] + "..." : text;
        }

        #endregion
    }

    #region Supporting Types

    public record GanttTask(
        string Id,
        string Name,
        DateTime Start,
        DateTime End,
        double Progress = 0,
        string? ParentId = null,
        bool IsMilestone = false,
        bool IsCritical = false,
        string? ResourceId = null,
        string? Notes = null
    );

    public record GanttDependency(
        string FromId,
        string ToId,
        GanttDependencyType Type = GanttDependencyType.FinishToStart
    );

    public enum GanttDependencyType
    {
        FinishToStart,
        StartToStart,
        FinishToFinish,
        StartToFinish
    }

    public enum GanttViewMode
    {
        Day,
        Week,
        Month
    }

    /// <summary>
    /// Mutable model for the task creation modal form.
    /// </summary>
    public class GanttTaskModalModel
    {
        public string Name { get; set; } = "";
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsMilestone { get; set; }
    }

    #endregion
}
