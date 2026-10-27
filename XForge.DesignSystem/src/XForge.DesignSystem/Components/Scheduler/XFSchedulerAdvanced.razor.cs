using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.Components.Scheduler
{
    public partial class XFSchedulerAdvanced : XForgeComponentBase, IAsyncDisposable
    {
        #region Parameters

        [Parameter] public SchedulerView View { get; set; } = SchedulerView.Week;
        [Parameter] public EventCallback<SchedulerView> ViewChanged { get; set; }
        [Parameter] public DateTime CurrentDate { get; set; } = DateTime.Today;
        [Parameter] public EventCallback<DateTime> CurrentDateChanged { get; set; }
        [Parameter] public IReadOnlyList<SchedulerEvent> Events { get; set; } = Array.Empty<SchedulerEvent>();
        [Parameter] public EventCallback<SchedulerEvent> OnEventClick { get; set; }
        [Parameter] public EventCallback<SchedulerEventCreateArgs> OnEventCreate { get; set; }
        [Parameter] public EventCallback<SchedulerEvent> OnEventUpdate { get; set; }
        [Parameter] public EventCallback<string> OnEventDelete { get; set; }
        [Parameter] public bool AllowDragToCreate { get; set; } = true;
        [Parameter] public bool AllowDragToMove { get; set; } = true;
        [Parameter] public bool AllowDragToResize { get; set; } = true;
        [Parameter] public bool AllowRecurring { get; set; }
        [Parameter] public bool AllowResourceGrouping { get; set; }
        [Parameter] public bool ShowAllDaySection { get; set; } = true;
        [Parameter] public int StartHour { get; set; } = 0;
        [Parameter] public int EndHour { get; set; } = 24;
        [Parameter] public int HourHeight { get; set; } = 60;
        [Parameter] public string? TitleFormat { get; set; }
        [Parameter] public string[]? ResourceIds { get; set; }
        [Parameter] public Dictionary<string, string>? ResourceLabels { get; set; }
        [Parameter] public EventCallback<SchedulerEvent> OnEventDoubleClick { get; set; }

        // Templates
        [Parameter] public RenderFragment<SchedulerEvent>? EventTemplate { get; set; }
        [Parameter] public RenderFragment<SchedulerEvent>? MonthEventTemplate { get; set; }
        [Parameter] public RenderFragment<SchedulerEvent>? AgendaEventTemplate { get; set; }
        [Parameter] public RenderFragment<SchedulerEvent>? ResourceTemplate { get; set; }

        // Export
        [Parameter] public bool AllowExport { get; set; }
        [Parameter] public EventCallback<SchedulerExportArgs> OnExport { get; set; }

        // Inline editing
        [Parameter] public bool AllowInlineEdit { get; set; } = true;

        // Touch gestures
        [Parameter] public bool AllowTouchGestures { get; set; } = true;

        #endregion

        #region Internal State

        private SchedulerEvent? _draggedEvent;
        private SchedulerEvent? _resizingEvent;
        private SchedulerView _currentView;
        private DateTime _currentDate;
        private bool _isDragging;
        private double _dragStartX;
        private double _dragStartY;
        private DateTime _dragOriginalStart;
        private DateTime _dragOriginalEnd;
        private bool _showEventModal;
        private SchedulerEvent? _editingEvent;
        private SchedulerEventModalModel _modalEvent = new();

        // Inline editing
        private SchedulerEvent? _inlineEditingEvent;
        private string _inlineEditTitle = "";

        // Drag-create
        private bool _isCreating;
        private DateTime _createStart;
        private DateTime _createEnd;

        // Month view
        private DateTime[][] _monthGrid = Array.Empty<DateTime[]>();

        // Week view
        private DateTime[] _weekDays = Array.Empty<DateTime>();

        // Day view
        private int _dayStartHour;
        private int _dayEndHour;

        // JS Interop
        [Inject] private IJSRuntime? JSRuntime { get; set; }
        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<XFSchedulerAdvanced>? _dotNetRef;

        // Touch gestures
        private double _touchStartX;
        private double _touchStartY;
        private DateTime _touchStartTime;
        private bool _isTouchSwipe;
        private double _pinchStartDistance;

        // Expanded recurring events cache
        private List<SchedulerEvent> _expandedEvents = new();

        #endregion

        #region Lifecycle

        protected override async Task OnInitializedAsync()
        {
            _currentView = View;
            _currentDate = CurrentDate;
            RefreshView();

            // Initialize JS interop
            _dotNetRef = DotNetObjectReference.Create(this);
            try
            {
                if (JSRuntime != null)
                {
                    _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/XForge.DesignSystem/js/xfscheduler-advanced.js");
                }
            }
            catch
            {
                // JS interop not available, fall back to basic behavior
            }
        }

        protected override void OnParametersSet()
        {
            if (View != _currentView)
            {
                _currentView = View;
                RefreshView();
            }
            if (CurrentDate != _currentDate)
            {
                _currentDate = CurrentDate;
                RefreshView();
            }
        }

        private void RefreshView()
        {
            _dayStartHour = StartHour;
            _dayEndHour = EndHour;

            switch (_currentView)
            {
                case SchedulerView.Month:
                    BuildMonthGrid();
                    break;
                case SchedulerView.Week:
                    BuildWeekDays();
                    break;
            }
        }

        private void BuildMonthGrid()
        {
            var firstDay = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var startOffset = (int)firstDay.DayOfWeek;
            var startDate = firstDay.AddDays(-startOffset);
            var weeks = new List<DateTime[]>();
            var current = startDate;

            for (int w = 0; w < 6; w++)
            {
                var week = new DateTime[7];
                for (int d = 0; d < 7; d++)
                {
                    week[d] = current;
                    current = current.AddDays(1);
                }
                weeks.Add(week);
                if (current.Month != _currentDate.Month && current.DayOfWeek == DayOfWeek.Sunday)
                    break;
            }
            _monthGrid = weeks.ToArray();
        }

        private void BuildWeekDays()
        {
            var startOfWeek = _currentDate.AddDays(-(int)_currentDate.DayOfWeek);
            _weekDays = Enumerable.Range(0, 7).Select(i => startOfWeek.AddDays(i)).ToArray();
        }

        public async ValueTask DisposeAsync()
        {
            if (_jsModule != null)
            {
                await _jsModule.DisposeAsync();
            }
            _dotNetRef?.Dispose();
        }

        #endregion

        #region Navigation

        private async Task NavigatePrevious()
        {
            _currentDate = _currentView switch
            {
                SchedulerView.Day => _currentDate.AddDays(-1),
                SchedulerView.Week => _currentDate.AddDays(-7),
                SchedulerView.Month => _currentDate.AddMonths(-1),
                SchedulerView.Year => _currentDate.AddYears(-1),
                _ => _currentDate.AddDays(-7)
            };
            RefreshView();
            await CurrentDateChanged.InvokeAsync(_currentDate);
        }

        private async Task NavigateNext()
        {
            _currentDate = _currentView switch
            {
                SchedulerView.Day => _currentDate.AddDays(1),
                SchedulerView.Week => _currentDate.AddDays(7),
                SchedulerView.Month => _currentDate.AddMonths(1),
                SchedulerView.Year => _currentDate.AddYears(1),
                _ => _currentDate.AddDays(7)
            };
            RefreshView();
            await CurrentDateChanged.InvokeAsync(_currentDate);
        }

        private async Task NavigateToday()
        {
            _currentDate = DateTime.Today;
            RefreshView();
            await CurrentDateChanged.InvokeAsync(_currentDate);
        }

        private async Task SwitchView(SchedulerView newView)
        {
            _currentView = newView;
            RefreshView();
            await ViewChanged.InvokeAsync(newView);
        }

        #endregion

        #region Event Positioning

        private double GetEventLeft(SchedulerEvent ev, int dayIndex)
        {
            if (_currentView == SchedulerView.Week)
            {
                var dayStart = _weekDays[dayIndex].Date;
                var dayEnd = dayStart.AddDays(1);
                var start = ev.Start < dayStart ? dayStart : ev.Start;
                var end = ev.End > dayEnd ? dayEnd : ev.End;
                var totalMinutes = (dayEnd - dayStart).TotalMinutes;
                var startMinutes = (start - dayStart).TotalMinutes;
                return (startMinutes / totalMinutes) * 100;
            }
            else if (_currentView == SchedulerView.Day)
            {
                var dayStart = _currentDate.Date.AddHours(_dayStartHour);
                var dayEnd = _currentDate.Date.AddHours(_dayEndHour);
                var totalMinutes = (dayEnd - dayStart).TotalMinutes;
                var startMinutes = (ev.Start - dayStart).TotalMinutes;
                return (startMinutes / totalMinutes) * 100;
            }
            return 0;
        }

        private double GetEventWidth(SchedulerEvent ev, int dayIndex)
        {
            if (_currentView == SchedulerView.Week)
            {
                var dayStart = _weekDays[dayIndex].Date;
                var dayEnd = dayStart.AddDays(1);
                var start = ev.Start < dayStart ? dayStart : ev.Start;
                var end = ev.End > dayEnd ? dayEnd : ev.End;
                var totalMinutes = (dayEnd - dayStart).TotalMinutes;
                var durationMinutes = (end - start).TotalMinutes;
                return Math.Max((durationMinutes / totalMinutes) * 100, 2);
            }
            else if (_currentView == SchedulerView.Day)
            {
                var dayStart = _currentDate.Date.AddHours(_dayStartHour);
                var dayEnd = _currentDate.Date.AddHours(_dayEndHour);
                var totalMinutes = (dayEnd - dayStart).TotalMinutes;
                var durationMinutes = (ev.End - ev.Start).TotalMinutes;
                return Math.Max((durationMinutes / totalMinutes) * 100, 2);
            }
            return 10;
        }

        private double GetEventTop(SchedulerEvent ev)
        {
            var dayStart = _currentDate.Date.AddHours(_dayStartHour);
            var startMinutes = (ev.Start - dayStart).TotalMinutes;
            return (startMinutes / 60.0) * HourHeight;
        }

        private double GetEventHeight(SchedulerEvent ev)
        {
            var durationMinutes = (ev.End - ev.Start).TotalMinutes;
            return Math.Max((durationMinutes / 60.0) * HourHeight, 20);
        }

        private bool IsEventOnDay(SchedulerEvent ev, DateTime day)
        {
            return ev.Start.Date <= day.Date && ev.End.Date >= day.Date;
        }

        private IEnumerable<SchedulerEvent> GetEventsForDay(DateTime day)
        {
            return GetExpandedEvents().Where(ev => IsEventOnDay(ev, day));
        }

        private IEnumerable<SchedulerEvent> GetEventsForDayAndResource(DateTime day, string? resourceId)
        {
            return GetExpandedEvents().Where(ev => IsEventOnDay(ev, day) && ev.ResourceId == resourceId);
        }

        #endregion

        #region Drag and Drop

        private void OnEventPointerDown(SchedulerEvent ev, PointerEventArgs e)
        {
            if (!AllowDragToMove && !AllowDragToResize) return;

            _draggedEvent = ev;
            _dragStartX = e.ClientX;
            _dragStartY = e.ClientY;
            _dragOriginalStart = ev.Start;
            _dragOriginalEnd = ev.End;
            _isDragging = true;
        }

        private async Task OnGridPointerMove(PointerEventArgs e)
        {
            if (!_isDragging || _draggedEvent == null) return;

            var deltaX = e.ClientX - _dragStartX;
            var deltaY = e.ClientY - _dragStartY;

            // Calculate time delta based on pixels
            var minutesPerPixel = 60.0 / HourHeight;
            var minuteDelta = (int)(deltaY * minutesPerPixel);

            if (minuteDelta != 0)
            {
                var newStart = _dragOriginalStart.AddMinutes(minuteDelta);
                var newEnd = _dragOriginalEnd.AddMinutes(minuteDelta);

                // Update the event temporarily for visual feedback
                var updated = _draggedEvent with { Start = newStart, End = newEnd };
                var list = Events.ToList();
                var index = list.FindIndex(ev => ev.Id == _draggedEvent.Id);
                if (index >= 0)
                {
                    list[index] = updated;
                    Events = list;
                    _draggedEvent = updated;
                }

                // Use JS interop for smoother drag experience
                if (_jsModule != null)
                {
                    await _jsModule.InvokeVoidAsync("scheduler.updateDragVisual", _dotNetRef, e.ClientX, e.ClientY);
                }
            }
        }

        private async Task OnGridPointerUp(PointerEventArgs e)
        {
            if (!_isDragging || _draggedEvent == null) return;

            _isDragging = false;

            // Notify parent of the change
            await OnEventUpdate.InvokeAsync(_draggedEvent);

            _draggedEvent = null;
        }

        #endregion

        #region Drag Create

        private async Task OnSlotPointerDown(DateTime slotStart, PointerEventArgs e)
        {
            if (!AllowDragToCreate) return;

            _isCreating = true;
            _createStart = slotStart;
            _createEnd = slotStart.AddHours(1);
        }

        private async Task OnSlotPointerMove(PointerEventArgs e)
        {
            if (!_isCreating) return;

            // Calculate end time based on drag distance
            var minutesPerPixel = 60.0 / HourHeight;
            var deltaY = e.ClientY - _dragStartY;
            var minuteDelta = (int)(deltaY * minutesPerPixel);

            _createEnd = _createStart.AddMinutes(Math.Max(15, minuteDelta));
        }

        private async Task OnSlotPointerUp(PointerEventArgs e)
        {
            if (!_isCreating) return;

            _isCreating = false;

            // Create the new event
            var newEvent = new SchedulerEvent(
                Guid.NewGuid().ToString("N"),
                "New Event",
                _createStart,
                _createEnd
            );

            await OnEventCreate.InvokeAsync(new SchedulerEventCreateArgs(newEvent));
        }

        #endregion

        #region Inline Editing

        private void StartInlineEdit(SchedulerEvent ev)
        {
            if (!AllowInlineEdit) return;
            _inlineEditingEvent = ev;
            _inlineEditTitle = ev.Title;
        }

        private async Task SaveInlineEdit(SchedulerEvent ev)
        {
            if (_inlineEditingEvent == null || _inlineEditingEvent.Id != ev.Id) return;

            var updated = ev with { Title = _inlineEditTitle };
            _inlineEditingEvent = null;
            _inlineEditTitle = "";
            await OnEventUpdate.InvokeAsync(updated);
        }

        private void CancelInlineEdit()
        {
            _inlineEditingEvent = null;
            _inlineEditTitle = "";
        }

        private void OnInlineEditKeyDown(KeyboardEventArgs e, SchedulerEvent ev)
        {
            if (e.Key == "Enter")
                _ = SaveInlineEdit(ev);
            else if (e.Key == "Escape")
                CancelInlineEdit();
        }

        private bool IsInlineEditing(SchedulerEvent ev)
        {
            return _inlineEditingEvent?.Id == ev.Id;
        }

        #endregion

        #region Event Modal

        private void OpenCreateModal(DateTime start)
        {
            _editingEvent = null;
            _modalEvent = new SchedulerEventModalModel
            {
                Title = "",
                Start = start,
                End = start.AddHours(1)
            };
            _showEventModal = true;
        }

        private void OpenEditModal(SchedulerEvent ev)
        {
            _editingEvent = ev;
            _modalEvent = new SchedulerEventModalModel
            {
                Title = ev.Title,
                Start = ev.Start,
                End = ev.End,
                ResourceId = ev.ResourceId,
                Color = ev.Color,
                IsAllDay = ev.IsAllDay
            };
            _showEventModal = true;
        }

        private void CloseModal()
        {
            _showEventModal = false;
            _editingEvent = null;
        }

        private async Task SaveEvent()
        {
            var newEvent = new SchedulerEvent(
                _editingEvent?.Id ?? Guid.NewGuid().ToString("N"),
                _modalEvent.Title,
                _modalEvent.Start,
                _modalEvent.End,
                _modalEvent.ResourceId,
                _modalEvent.Color,
                _modalEvent.IsAllDay
            );

            if (_editingEvent != null)
            {
                await OnEventUpdate.InvokeAsync(newEvent);
            }
            else
            {
                await OnEventCreate.InvokeAsync(new SchedulerEventCreateArgs(newEvent));
            }
            CloseModal();
        }

        private async Task DeleteEvent()
        {
            if (_editingEvent != null)
            {
                await OnEventDelete.InvokeAsync(_editingEvent.Id);
                CloseModal();
            }
        }

        #endregion

        #region Export

        private async Task ExportEventsAsync(SchedulerExportFormat format)
        {
            var exportData = new SchedulerExportArgs
            {
                Format = format,
                Events = Events,
                StartDate = _currentDate,
                EndDate = _currentView switch
                {
                    SchedulerView.Day => _currentDate.AddDays(1),
                    SchedulerView.Week => _currentDate.AddDays(7),
                    SchedulerView.Month => _currentDate.AddMonths(1),
                    _ => _currentDate.AddDays(30)
                }
            };

            await OnExport.InvokeAsync(exportData);

            // If no custom handler, use default JS export
            if (_jsModule != null && !OnExport.HasDelegate)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(exportData.Events);
                await _jsModule.InvokeVoidAsync("scheduler.downloadFile", $"events.{format.ToString().ToLower()}", json);
            }
        }

        #endregion

        #region Recurring Events

        private List<SchedulerEvent> GetExpandedEvents()
        {
            var expanded = new List<SchedulerEvent>();
            var rangeStart = _currentDate.AddDays(-7);
            var rangeEnd = _currentDate.AddMonths(2);

            foreach (var ev in Events)
            {
                if (ev.Recurrence == null || ev.Recurrence.Type == SchedulerRecurrenceType.None)
                {
                    expanded.Add(ev);
                }
                else
                {
                    expanded.AddRange(ExpandRecurringEvent(ev, rangeStart, rangeEnd));
                }
            }

            return expanded;
        }

        private List<SchedulerEvent> ExpandRecurringEvent(SchedulerEvent ev, DateTime rangeStart, DateTime rangeEnd)
        {
            var occurrences = new List<SchedulerEvent>();
            var current = ev.Start;
            var duration = ev.End - ev.Start;
            var maxOccurrences = ev.Recurrence?.Count ?? 365;
            var until = ev.Recurrence?.Until ?? rangeEnd;
            var interval = ev.Recurrence?.Interval ?? 1;

            for (int i = 0; i < maxOccurrences && current <= until; i++)
            {
                if (current >= rangeStart && current <= rangeEnd)
                {
                    occurrences.Add(ev with
                    {
                        Id = $"{ev.Id}-r{i}",
                        Start = current,
                        End = current + duration
                    });
                }

                current = ev.Recurrence?.Type switch
                {
                    SchedulerRecurrenceType.Daily => current.AddDays(interval),
                    SchedulerRecurrenceType.Weekly => current.AddDays(7 * interval),
                    SchedulerRecurrenceType.Monthly => current.AddMonths(interval),
                    SchedulerRecurrenceType.Yearly => current.AddYears(interval),
                    _ => current.AddDays(7 * interval)
                };

                // Handle specific days of week for weekly recurrence
                if (ev.Recurrence?.Type == SchedulerRecurrenceType.Weekly && ev.Recurrence.DaysOfWeek?.Length > 0)
                {
                    var nextDay = current;
                    while (!ev.Recurrence.DaysOfWeek.Contains((int)nextDay.DayOfWeek) && nextDay < until)
                    {
                        nextDay = nextDay.AddDays(1);
                    }
                    current = nextDay;
                }
            }

            return occurrences;
        }

        #endregion

        #region Resource Grouping

        private bool HasResources => AllowResourceGrouping && ResourceIds?.Any() == true;

        private IEnumerable<IGrouping<string?, SchedulerEvent>> GetEventsByResource()
        {
            return GetExpandedEvents().GroupBy(ev => ev.ResourceId);
        }

        private string GetResourceLabel(string? resourceId)
        {
            if (string.IsNullOrEmpty(resourceId)) return "Unassigned";
            return ResourceLabels?.GetValueOrDefault(resourceId) ?? resourceId;
        }

        #endregion

        #region Touch Gestures

        private void OnTouchStart(TouchEventArgs e)
        {
            if (!AllowTouchGestures || e.Touches.Length == 0) return;

            _touchStartX = e.Touches[0].ClientX;
            _touchStartY = e.Touches[0].ClientY;
            _touchStartTime = DateTime.UtcNow;

            if (e.Touches.Length == 2)
            {
                _pinchStartDistance = GetPinchDistance(e.Touches);
            }
        }

        private async Task OnTouchMove(TouchEventArgs e)
        {
            if (!AllowTouchGestures || e.Touches.Length == 0) return;

            // Handle pinch zoom
            if (e.Touches.Length == 2)
            {
                var currentDistance = GetPinchDistance(e.Touches);
                var scale = currentDistance / _pinchStartDistance;

                if (Math.Abs(scale - 1.0) > 0.1)
                {
                    // Adjust hour height based on pinch
                    var newHourHeight = Math.Clamp((int)(HourHeight * scale), 30, 120);
                    // This would require a parameter callback in production
                }
            }
        }

        private async Task OnTouchEnd(TouchEventArgs e)
        {
            if (!AllowTouchGestures) return;

            var duration = (DateTime.UtcNow - _touchStartTime).TotalMilliseconds;

            // Detect swipe gesture
            if (e.ChangedTouches.Length > 0 && duration < 300)
            {
                var deltaX = e.ChangedTouches[0].ClientX - _touchStartX;
                var deltaY = e.ChangedTouches[0].ClientY - _touchStartY;

                if (Math.Abs(deltaX) > 50 && Math.Abs(deltaX) > Math.Abs(deltaY))
                {
                    // Horizontal swipe - navigate
                    if (deltaX > 0)
                        await NavigatePrevious();
                    else
                        await NavigateNext();
                }
                else if (Math.Abs(deltaY) > 50 && Math.Abs(deltaY) > Math.Abs(deltaX))
                {
                    // Vertical swipe - switch views
                    if (deltaY > 0 && _currentView != SchedulerView.Month)
                        await SwitchView(_currentView - 1);
                    else if (deltaY < 0 && _currentView != SchedulerView.Agenda)
                        await SwitchView(_currentView + 1);
                }
            }

            // Detect tap (short touch without movement)
            if (duration < 200 && Math.Abs(e.ChangedTouches[0].ClientX - _touchStartX) < 10)
            {
                // Single tap - could open create modal
            }
        }

        private double GetPinchDistance(TouchPoint[] touches)
        {
            if (touches.Length < 2) return 0;
            var dx = touches[0].ClientX - touches[1].ClientX;
            var dy = touches[0].ClientY - touches[1].ClientY;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion

        #region Helpers

        private string GetTitle()
        {
            return _currentView switch
            {
                SchedulerView.Day => _currentDate.ToString("dddd, MMMM d, yyyy"),
                SchedulerView.Week => $"{_weekDays.First():MMM d} - {_weekDays.Last():MMM d, yyyy}",
                SchedulerView.Month => _currentDate.ToString("MMMM yyyy"),
                SchedulerView.Year => _currentDate.ToString("yyyy"),
                SchedulerView.Agenda => $"Agenda - {_currentDate.ToString("MMMM yyyy")}",
                SchedulerView.Timeline => _currentDate.ToString("MMMM yyyy"),
                _ => _currentDate.ToString("MMMM yyyy")
            };
        }

        private string GetViewLabel(SchedulerView view)
        {
            return view switch
            {
                SchedulerView.Day => "Day",
                SchedulerView.Week => "Week",
                SchedulerView.Month => "Month",
                SchedulerView.Year => "Year",
                SchedulerView.Timeline => "Timeline",
                SchedulerView.Agenda => "Agenda",
                _ => view.ToString()
            };
        }

        private string GetTimeLabel(int hour)
        {
            return $"{hour:D2}:00";
        }

        private int[] GetHours()
        {
            return Enumerable.Range(_dayStartHour, _dayEndHour - _dayStartHour).ToArray();
        }

        private bool IsToday(DateTime date)
        {
            return date.Date == DateTime.Today;
        }

        private bool IsCurrentMonth(DateTime date)
        {
            return date.Month == _currentDate.Month && date.Year == _currentDate.Year;
        }

        private string GetEventStyle(SchedulerEvent ev, int dayIndex)
        {
            var left = GetEventLeft(ev, dayIndex);
            var width = GetEventWidth(ev, dayIndex);
            var top = GetEventTop(ev);
            var height = GetEventHeight(ev);

            var bgColor = ev.Color ?? "var(--color-primary)";
            return $"left:{left}%;width:{width}%;top:{top}px;height:{height}px;background-color:{bgColor};";
        }

        #endregion
    }

    #region Supporting Types

    public record SchedulerEvent(
        string Id,
        string Title,
        DateTime Start,
        DateTime End,
        string? ResourceId = null,
        string? Color = null,
        bool IsAllDay = false,
        SchedulerRecurrence? Recurrence = null
    );

    public record SchedulerRecurrence(
        SchedulerRecurrenceType Type,
        int Interval = 1,
        int? Count = null,
        DateTime? Until = null,
        int[]? DaysOfWeek = null
    );

    public enum SchedulerRecurrenceType
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public enum SchedulerView
    {
        Day,
        Week,
        Month,
        Year,
        Timeline,
        Agenda
    }

    public record SchedulerEventCreateArgs(SchedulerEvent Event);

    /// <summary>
    /// Mutable model for the event modal form.
    /// </summary>
    public class SchedulerEventModalModel
    {
        public string Title { get; set; } = "";
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? ResourceId { get; set; }
        public string? Color { get; set; }
        public bool IsAllDay { get; set; }
    }

    public enum SchedulerExportFormat
    {
        Json,
        Csv,
        Ics
    }

    public class SchedulerExportArgs
    {
        public SchedulerExportFormat Format { get; set; }
        public IReadOnlyList<SchedulerEvent> Events { get; set; } = Array.Empty<SchedulerEvent>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    #endregion
}
