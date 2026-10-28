using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Gantt
{
    public partial class XFGantt<TItem> : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private List<XFGanttNode<TItem>> _roots = new();
        private readonly HashSet<string> _collapsedIds = new();
        private DateTime _visibleStart;
        private DateTime _visibleEnd;
        private int _columnCount;
        private double _columnWidthPx = 80;
        private readonly List<XFGanttNode<TItem>> _flatVisible = new();
        private string? _editingTaskId;
        private string _editValue = "";

        // Drag & Resize state
        private bool _isDragging;
        private double _dragStartX;
        private DateTime _dragOriginalStart;
        private DateTime _dragOriginalEnd;
        private int _draggedTaskIndex = -1;
        private string _dragMode = "";

        #region Parameters

        /// <summary>Flat task list.</summary>
        [Parameter, EditorRequired]
        public IEnumerable<TItem>? Data { get; set; }

        /// <summary>Unique ID selector.</summary>
        [Parameter, EditorRequired]
        public Func<TItem, string>? IdFunc { get; set; }

        /// <summary>Parent ID selector (null = root task).</summary>
        [Parameter]
        public Func<TItem, string?>? ParentIdFunc { get; set; }

        /// <summary>Task title selector.</summary>
        [Parameter, EditorRequired]
        public Func<TItem, string>? TitleFunc { get; set; }

        /// <summary>Start date selector.</summary>
        [Parameter, EditorRequired]
        public Func<TItem, DateTime>? StartFunc { get; set; }

        /// <summary>End date selector.</summary>
        [Parameter, EditorRequired]
        public Func<TItem, DateTime>? EndFunc { get; set; }

        /// <summary>Resource ID selector for task assignment.</summary>
        [Parameter]
        public Func<TItem, string?>? ResourceIdFunc { get; set; }

        /// <summary>Resource data for the resource panel.</summary>
        [Parameter]
        public IEnumerable<XFGanttResource>? Resources { get; set; }

        /// <summary>Whether to show the resource panel on the left.</summary>
        [Parameter]
        public bool ShowResourcePanel { get; set; }

        /// <summary>Resource panel width.</summary>
        [Parameter]
        public string ResourcePanelWidth { get; set; } = "150px";

        /// <summary>Progress 0-100 selector.</summary>
        [Parameter]
        public Func<TItem, double>? ProgressFunc { get; set; }

        /// <summary>Task dependencies.</summary>
        [Parameter]
        public IEnumerable<XFGanttDependency>? Dependencies { get; set; }

        /// <summary>Current view mode.</summary>
        [Parameter]
        public XFGanttView View { get; set; } = XFGanttView.Week;

        /// <summary>View mode changed callback.</summary>
        [Parameter]
        public EventCallback<XFGanttView> ViewChanged { get; set; }

        /// <summary>Current visible date center.</summary>
        [Parameter]
        public DateTime Date { get; set; } = DateTime.Today;

        /// <summary>Date changed callback.</summary>
        [Parameter]
        public EventCallback<DateTime> DateChanged { get; set; }

        /// <summary>Whether to show progress bars.</summary>
        [Parameter]
        public bool ShowProgress { get; set; } = true;

        /// <summary>Whether to show dependency arrows.</summary>
        [Parameter]
        public bool ShowDependencies { get; set; } = true;

        /// <summary>Whether to show today line.</summary>
        [Parameter]
        public bool ShowTodayLine { get; set; } = true;

        /// <summary>Whether to show grid lines.</summary>
        [Parameter]
        public bool ShowGridLines { get; set; } = true;

        /// <summary>Row height in pixels.</summary>
        [Parameter]
        public int RowHeight { get; set; } = 40;

        /// <summary>Task list panel width.</summary>
        [Parameter]
        public string TaskListWidth { get; set; } = "300px";

        /// <summary>Custom date format string.</summary>
        [Parameter]
        public string? DateFormat { get; set; }

        /// <summary>Currently selected task.</summary>
        [Parameter]
        public TItem? SelectedTask { get; set; }

        /// <summary>Selected task changed callback.</summary>
        [Parameter]
        public EventCallback<TItem?> SelectedTaskChanged { get; set; }

        /// <summary>Task click callback.</summary>
        [Parameter]
        public EventCallback<XFGanttTaskClickEventArgs<TItem>> OnTaskClick { get; set; }

        /// <summary>Task move callback (drag).</summary>
        [Parameter]
        public EventCallback<XFGanttTaskMoveEventArgs<TItem>> OnTaskMove { get; set; }

        /// <summary>Task resize callback (drag right edge).</summary>
        [Parameter]
        public EventCallback<XFGanttTaskResizeEventArgs<TItem>> OnTaskResize { get; set; }

        /// <summary>Task edit callback (inline title editing).</summary>
        [Parameter]
        public EventCallback<XFGanttTaskEditEventArgs<TItem>> OnTaskEdit { get; set; }

        /// <summary>Whether to allow task move by dragging.</summary>
        [Parameter]
        public bool AllowTaskMove { get; set; }

        /// <summary>Whether to allow task resize by dragging right edge.</summary>
        [Parameter]
        public bool AllowTaskResize { get; set; }

        /// <summary>Zoom level (1=day, 2=week, 3=month).</summary>
        [Parameter]
        public XFGanttZoomLevel ZoomLevel { get; set; } = XFGanttZoomLevel.Week;

        /// <summary>Zoom level changed callback.</summary>
        [Parameter]
        public EventCallback<XFGanttZoomLevel> ZoomLevelChanged { get; set; }

        /// <summary>Label for "Today".</summary>
        [Parameter]
        public string TodayLabel { get; set; } = "Hoje";

        /// <summary>Text for empty state.</summary>
        [Parameter]
        public string NoDataText { get; set; } = "Nenhuma tarefa encontrada";

        /// <summary>Label for "Progress".</summary>
        [Parameter]
        public string ProgressLabel { get; set; } = "Progresso";

        /// <summary>Label for "Start".</summary>
        [Parameter]
        public string StartLabel { get; set; } = "Início";

        /// <summary>Label for "End".</summary>
        [Parameter]
        public string EndLabel { get; set; } = "Fim";

        /// <summary>Label for "Resource" panel header.</summary>
        [Parameter]
        public string ResourceLabel { get; set; } = "Recurso";

        /// <summary>Size variant.</summary>
        [Parameter]
        public XFSize Size { get; set; } = XFSize.Md;

        /// <summary>Density variant.</summary>
        [Parameter]
        public XFGanttVariant Variant { get; set; } = XFGanttVariant.Default;

        #endregion

        #region Computed properties

        private bool HasData => _roots.Count > 0;

        private string ContainerClass => Join(
            "xf-gantt flex border rounded-lg overflow-hidden",
            "border-border-light",
            "bg-bg-surface",
            Disabled ? "opacity-50 pointer-events-none" : null,
            SizeClass,
            Class
        );

        private string ResourcePanelClass => Join(
            "flex flex-col border-r border-border-light",
            "bg-surface-50",
            "flex-shrink-0"
        );

        private string SizeClass => Size switch
        {
            XFSize.Sm => "text-xs",
            XFSize.Lg => "text-base",
            _ => "text-sm"
        };

        private int EffectiveRowHeight => Variant switch
        {
            XFGanttVariant.Compact => Math.Max(28, RowHeight - 8),
            XFGanttVariant.Comfortable => RowHeight + 8,
            _ => RowHeight
        };

        private string TaskListPanelClass => Join(
            "xf-gantt-tasklist flex-shrink-0 overflow-y-auto overflow-x-hidden border-r",
            "border-border-light",
            "bg-surface-50"
        );

        private string TimelinePanelClass => "xf-gantt-timeline flex-1 overflow-auto";

        private string HeaderClass => Join(
            "xf-gantt-header flex border-b",
            "border-border-light",
            "bg-surface-100",
            "font-medium text-text-secondary"
        );

        private string TodayLineClass => "xf-gantt-today-line absolute top-0 bottom-0 border-l-2 border-dashed border-primary z-10 pointer-events-none";

        private string EmptyClass => "flex items-center justify-center p-8 text-text-muted";

        private bool IsSelected(TItem? item) =>
            item is not null && SelectedTask is not null &&
            EqualityComparer<TItem>.Default.Equals(item, SelectedTask);

        #endregion

        #region Lifecycle

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(TodayLabel)) TodayLabel = L("gantt.today", TodayLabel);
            if (string.IsNullOrWhiteSpace(NoDataText)) NoDataText = L("gantt.empty", NoDataText);
            if (string.IsNullOrWhiteSpace(ProgressLabel)) ProgressLabel = L("gantt.progress", ProgressLabel);
            if (string.IsNullOrWhiteSpace(StartLabel)) StartLabel = L("gantt.start", StartLabel);
            if (string.IsNullOrWhiteSpace(EndLabel)) EndLabel = L("gantt.end", EndLabel);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            BuildTree();
            ComputeTimelineRange();
        }

        #endregion

        #region Tree building

        private void BuildTree()
        {
            _roots.Clear();
            _flatVisible.Clear();

            if (Data is null || IdFunc is null || TitleFunc is null || StartFunc is null || EndFunc is null)
                return;

            var items = Data.Where(d => d is not null).ToList();
            if (items.Count == 0) return;

            // Build node lookup
            var nodes = new Dictionary<string, XFGanttNode<TItem>>();
            foreach (var item in items)
            {
                var id = IdFunc(item);
                if (string.IsNullOrWhiteSpace(id)) continue;

                var node = new XFGanttNode<TItem>
                {
                    Id = id,
                    ParentId = ParentIdFunc?.Invoke(item),
                    Title = TitleFunc(item) ?? string.Empty,
                    Start = StartFunc(item),
                    End = EndFunc(item),
                    Progress = ProgressFunc?.Invoke(item) ?? 0,
                    Item = item,
                    IsExpanded = !_collapsedIds.Contains(id)
                };

                // Clamp progress
                node.Progress = Math.Clamp(node.Progress, 0, 100);
                nodes[id] = node;
            }

            // Build parent-child relationships
            foreach (var node in nodes.Values)
            {
                if (!string.IsNullOrWhiteSpace(node.ParentId) && nodes.TryGetValue(node.ParentId, out var parent))
                {
                    parent.Children.Add(node);
                    parent.HasChildren = true;
                    node.Depth = parent.Depth + 1;
                }
                else
                {
                    _roots.Add(node);
                }
            }

            // Flatten visible nodes
            foreach (var root in _roots)
            {
                FlattenVisible(root);
            }
        }

        private void FlattenVisible(XFGanttNode<TItem> node)
        {
            _flatVisible.Add(node);
            if (node.IsExpanded)
            {
                foreach (var child in node.Children)
                {
                    FlattenVisible(child);
                }
            }
        }

        #endregion

        #region Timeline computation

        private void ComputeTimelineRange()
        {
            var date = Date == default ? DateTime.Today : Date;

            switch (View)
            {
                case XFGanttView.Day:
                    _visibleStart = date.Date;
                    _visibleEnd = date.Date.AddDays(1);
                    _columnCount = 24; // hours
                    _columnWidthPx = 60;
                    break;

                case XFGanttView.Week:
                    var dow = date.DayOfWeek;
                    var diff = (dow == DayOfWeek.Sunday ? -6 : DayOfWeek.Monday - dow);
                    _visibleStart = date.Date.AddDays(diff);
                    _visibleEnd = _visibleStart.AddDays(7);
                    _columnCount = 7;
                    _columnWidthPx = 100;
                    break;

                case XFGanttView.Month:
                    _visibleStart = new DateTime(date.Year, date.Month, 1);
                    _visibleEnd = _visibleStart.AddMonths(1);
                    _columnCount = DateTime.DaysInMonth(date.Year, date.Month);
                    _columnWidthPx = 40;
                    break;

                case XFGanttView.Year:
                    _visibleStart = new DateTime(date.Year, 1, 1);
                    _visibleEnd = new DateTime(date.Year + 1, 1, 1);
                    _columnCount = 12;
                    _columnWidthPx = 80;
                    break;
            }
        }

        private double TimelineWidth => _columnCount * _columnWidthPx;

        private List<string> ColumnHeaders
        {
            get
            {
                var headers = new List<string>();
                var culture = CultureInfo.GetCultureInfo("pt-BR");

                switch (View)
                {
                    case XFGanttView.Day:
                        for (int h = 0; h < 24; h++)
                            headers.Add($"{h:00}:00");
                        break;

                    case XFGanttView.Week:
                        for (int d = 0; d < 7; d++)
                        {
                            var day = _visibleStart.AddDays(d);
                            headers.Add(day.ToString("ddd dd/MM", culture));
                        }
                        break;

                    case XFGanttView.Month:
                        for (int d = 1; d <= _columnCount; d++)
                            headers.Add(d.ToString());
                        break;

                    case XFGanttView.Year:
                        for (int m = 1; m <= 12; m++)
                        {
                            var month = new DateTime(Date.Year, m, 1);
                            headers.Add(month.ToString("MMM", culture));
                        }
                        break;
                }

                return headers;
            }
        }

        private double GetBarLeft(DateTime start)
        {
            if (start <= _visibleStart) return 0;
            if (start >= _visibleEnd) return TimelineWidth;

            var totalDuration = (_visibleEnd - _visibleStart).TotalDays;
            if (totalDuration <= 0) return 0;

            var offset = (start - _visibleStart).TotalDays;
            return Math.Clamp(offset / totalDuration * TimelineWidth, 0, TimelineWidth);
        }

        private double GetBarWidth(DateTime start, DateTime end)
        {
            var left = GetBarLeft(start);
            var right = GetBarLeft(end);
            return Math.Max(4, right - left); // minimum 4px width
        }

        private double GetTodayLineLeft()
        {
            var today = DateTime.Today;
            if (today < _visibleStart || today >= _visibleEnd) return -1;
            return GetBarLeft(today);
        }

        #endregion

        #region Expand/Collapse

        private void ToggleExpand(string nodeId)
        {
            if (_collapsedIds.Contains(nodeId))
                _collapsedIds.Remove(nodeId);
            else
                _collapsedIds.Add(nodeId);

            _flatVisible.Clear();
            foreach (var root in _roots)
                FlattenVisible(root);
        }

        /// <summary>Expands all tree nodes.</summary>
        public Task ExpandAll()
        {
            _collapsedIds.Clear();
            _flatVisible.Clear();
            foreach (var root in _roots)
                FlattenVisible(root);
            return InvokeAsync(StateHasChanged);
        }

        /// <summary>Collapses all tree nodes.</summary>
        public Task CollapseAll()
        {
            _collapsedIds.Clear();
            foreach (var node in _roots)
                CollapseRecursive(node);
            _flatVisible.Clear();
            foreach (var root in _roots)
                FlattenVisible(root);
            return InvokeAsync(StateHasChanged);
        }

        private void CollapseRecursive(XFGanttNode<TItem> node)
        {
            if (node.HasChildren)
            {
                _collapsedIds.Add(node.Id);
                foreach (var child in node.Children)
                    CollapseRecursive(child);
            }
        }

        #endregion

        #region Navigation

        /// <summary>Navigates to today's date.</summary>
        public Task NavigateToToday()
        {
            Date = DateTime.Today;
            DateChanged.InvokeAsync(Date);
            ComputeTimelineRange();
            return InvokeAsync(StateHasChanged);
        }

        /// <summary>Navigates to the previous period.</summary>
        public Task NavigatePrevious()
        {
            Date = View switch
            {
                XFGanttView.Day => Date.AddDays(-1),
                XFGanttView.Week => Date.AddDays(-7),
                XFGanttView.Month => Date.AddMonths(-1),
                XFGanttView.Year => Date.AddYears(-1),
                _ => Date.AddDays(-7)
            };
            DateChanged.InvokeAsync(Date);
            ComputeTimelineRange();
            return InvokeAsync(StateHasChanged);
        }

        /// <summary>Navigates to the next period.</summary>
        public Task NavigateNext()
        {
            Date = View switch
            {
                XFGanttView.Day => Date.AddDays(1),
                XFGanttView.Week => Date.AddDays(7),
                XFGanttView.Month => Date.AddMonths(1),
                XFGanttView.Year => Date.AddYears(1),
                _ => Date.AddDays(7)
            };
            DateChanged.InvokeAsync(Date);
            ComputeTimelineRange();
            return InvokeAsync(StateHasChanged);
        }

        /// <summary>Sets the view mode.</summary>
        public Task SetView(XFGanttView view)
        {
            View = view;
            ViewChanged.InvokeAsync(view);
            ComputeTimelineRange();
            return InvokeAsync(StateHasChanged);
        }

        #endregion

        #region Event handlers

        private async Task HandleTaskClick(XFGanttNode<TItem> node)
        {
            SelectedTask = node.Item;
            await SelectedTaskChanged.InvokeAsync(node.Item);

            if (OnTaskClick.HasDelegate)
            {
                await OnTaskClick.InvokeAsync(new XFGanttTaskClickEventArgs<TItem>
                {
                    Item = node.Item,
                    Start = node.Start,
                    End = node.End
                });
            }
        }

        private async Task OnBarPointerDown(PointerEventArgs e, int taskIndex, bool allowMove)
        {
            if (!allowMove)
            {
                // No drag — just fire task click
                if (taskIndex >= 0 && taskIndex < _flatVisible.Count)
                    await HandleTaskClick(_flatVisible[taskIndex]);
                return;
            }

            _isDragging = true;
            _dragStartX = e.ClientX;
            _draggedTaskIndex = taskIndex;
            _dragMode = "move";

            var node = _flatVisible[taskIndex];
            _dragOriginalStart = node.Start;
            _dragOriginalEnd = node.End;
        }

        private void OnBarResizePointerDown(PointerEventArgs e, int taskIndex)
        {
            if (!AllowTaskResize) return;

            _isDragging = true;
            _dragStartX = e.ClientX;
            _draggedTaskIndex = taskIndex;
            _dragMode = "resize";

            var node = _flatVisible[taskIndex];
            _dragOriginalStart = node.Start;
            _dragOriginalEnd = node.End;
        }

        private void OnBarPointerMove(PointerEventArgs e)
        {
            if (!_isDragging || _draggedTaskIndex < 0) return;

            var deltaX = e.ClientX - _dragStartX;
            var totalDuration = (_visibleEnd - _visibleStart).TotalDays;
            if (totalDuration <= 0) return;

            var daysDelta = deltaX / _columnWidthPx * (totalDuration / _columnCount);

            if (_dragMode == "move")
            {
                var newStart = _dragOriginalStart.AddDays(daysDelta);
                var duration = _dragOriginalEnd - _dragOriginalStart;
                var node = _flatVisible[_draggedTaskIndex];
                node.Start = newStart;
                node.End = newStart + duration;
            }
            else if (_dragMode == "resize")
            {
                var newEnd = _dragOriginalEnd.AddDays(daysDelta);
                if (newEnd > _dragOriginalStart)
                {
                    var node = _flatVisible[_draggedTaskIndex];
                    node.End = newEnd;
                }
            }

            InvokeAsync(StateHasChanged);
        }

        private async Task OnBarPointerUp(PointerEventArgs e)
        {
            if (!_isDragging || _draggedTaskIndex < 0) return;

            var deltaX = e.ClientX - _dragStartX;
            var totalDuration = (_visibleEnd - _visibleStart).TotalDays;
            if (totalDuration <= 0) { ResetDragState(); return; }

            var daysDelta = deltaX / _columnWidthPx * (totalDuration / _columnCount);
            var node = _flatVisible[_draggedTaskIndex];

            if (_dragMode == "move" && OnTaskMove.HasDelegate)
            {
                var newStart = _dragOriginalStart.AddDays(daysDelta);
                var duration = _dragOriginalEnd - _dragOriginalStart;
                await OnTaskMove.InvokeAsync(new XFGanttTaskMoveEventArgs<TItem>
                {
                    Item = node.Item,
                    OriginalStart = _dragOriginalStart,
                    OriginalEnd = _dragOriginalEnd,
                    NewStart = newStart,
                    NewEnd = newStart + duration
                });
            }
            else if (_dragMode == "resize" && OnTaskResize.HasDelegate)
            {
                var newEnd = _dragOriginalEnd.AddDays(daysDelta);
                if (newEnd > _dragOriginalStart)
                {
                    await OnTaskResize.InvokeAsync(new XFGanttTaskResizeEventArgs<TItem>
                    {
                        Item = node.Item,
                        OriginalStart = _dragOriginalStart,
                        OriginalEnd = _dragOriginalEnd,
                        NewEnd = newEnd
                    });
                }
            }

            ResetDragState();
            await InvokeAsync(StateHasChanged);
        }

        private void ResetDragState()
        {
            _isDragging = false;
            _draggedTaskIndex = -1;
            _dragMode = "";
        }

        private int CountTasksForResource(string resourceId)
        {
            if (ResourceIdFunc == null || Data == null) return 0;
            return Data.Count(item => ResourceIdFunc(item) == resourceId);
        }

        #endregion

        #region Helpers

        private int FindTaskIndex(string taskId)
        {
            for (int i = 0; i < _flatVisible.Count; i++)
            {
                if (_flatVisible[i].Id == taskId) return i;
            }
            return -1;
        }

        private int CountDescendants(XFGanttNode<TItem> node)
        {
            int count = 0;
            foreach (var child in node.Children)
            {
                count++;
                count += CountDescendants(child);
            }
            return count;
        }

        private void HandleEditKeydown(KeyboardEventArgs e, XFGanttNode<TItem> node)
        {
            if (e.Key == "Enter")
                _ = FinishEdit(node);
            else if (e.Key == "Escape")
                CancelEdit();
        }

        private void StartEdit(XFGanttNode<TItem> node)
        {
            _editingTaskId = node.Id;
            _editValue = node.Title;
        }

        private async Task FinishEdit(XFGanttNode<TItem> node)
        {
            if (_editingTaskId == null) return;
            var newTitle = _editValue?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(newTitle))
                newTitle = node.Title; // revert if empty

            _editingTaskId = null;
            _editValue = "";

            if (OnTaskEdit.HasDelegate && newTitle != node.Title)
            {
                await OnTaskEdit.InvokeAsync(new XFGanttTaskEditEventArgs<TItem>
                {
                    Item = node.Item,
                    OriginalTitle = node.Title,
                    NewTitle = newTitle
                });
            }

            await InvokeAsync(StateHasChanged);
        }

        private void CancelEdit()
        {
            _editingTaskId = null;
            _editValue = "";
        }

        private string FormatDateHeader(DateTime date)
        {
            if (!string.IsNullOrWhiteSpace(DateFormat))
                return date.ToString(DateFormat);

            return View switch
            {
                XFGanttView.Day => date.ToString("HH:mm"),
                XFGanttView.Week => date.ToString("ddd dd/MM"),
                XFGanttView.Month => date.ToString("dd"),
                XFGanttView.Year => date.ToString("MMM"),
                _ => date.ToString("dd/MM")
            };
        }

        private string FormatDate(DateTime date) =>
            date.ToString("dd/MM/yyyy");

        private string GetBarColor(XFGanttNode<TItem> node)
        {
            // Use progress to determine opacity
            var opacity = node.Progress >= 100 ? "1" : "0.7";
            return node.Progress >= 100
                ? $"bg-green-500 dark:bg-green-400"
                : $"bg-blue-500 dark:bg-blue-400";
        }

        private string GetProgressColor(XFGanttNode<TItem> node)
        {
            return node.Progress >= 100
                ? "bg-green-600 dark:bg-green-300"
                : "bg-blue-600 dark:bg-blue-300";
        }

        private string GetBarAriaLabel(XFGanttNode<TItem> node)
        {
            var progress = ShowProgress ? $", {ProgressLabel}: {node.Progress:F0}%" : string.Empty;
            return $"{node.Title}, {StartLabel}: {FormatDate(node.Start)}, {EndLabel}: {FormatDate(node.End)}{progress}";
        }

        private string GetExpandIcon(XFGanttNode<TItem> node)
        {
            if (!node.HasChildren) return "";
            return node.IsExpanded ? "▼" : "▶";
        }

        private static string Join(params string?[] values) =>
            string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));

        #endregion
    }
}
