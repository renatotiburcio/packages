using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Scheduler
{
    public partial class XFScheduler<TItem> : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-scheduler";

        private DateTime currentDate = DateTime.Today;
        private IReadOnlyList<AppointmentInfo> appointments = Array.Empty<AppointmentInfo>();
        private IReadOnlyList<DateOnly> monthDays = Array.Empty<DateOnly>();
        private IReadOnlyList<DateTime> weekDays = Array.Empty<DateTime>();
        private IReadOnlyList<TimelineSlot> timelineSlots = Array.Empty<TimelineSlot>();
        private IReadOnlyList<string> weekdayNames = Array.Empty<string>();
        private Dictionary<DateOnly, IReadOnlyList<AppointmentInfo>> appointmentsByDay = new();

        [Parameter] public IEnumerable<TItem>? Data { get; set; }
        [Parameter] public string? StartProperty { get; set; }
        [Parameter] public string? EndProperty { get; set; }
        [Parameter] public string? TextProperty { get; set; }
        [Parameter] public string? ResourceProperty { get; set; }
        [Parameter] public Func<TItem, string?>? ResourceSelector { get; set; }
        [Parameter] public IReadOnlyList<XFSchedulerResource> Resources { get; set; } = Array.Empty<XFSchedulerResource>();
        [Parameter] public RenderFragment<XFSchedulerAppointmentContext<TItem>>? Template { get; set; }
        [Parameter] public RenderFragment<XFSchedulerSlotContext<TItem>>? SlotTemplate { get; set; }
        [Parameter] public DateTime Date { get; set; } = DateTime.Today;
        [Parameter] public EventCallback<DateTime> DateChanged { get; set; }
        [Parameter] public XFSchedulerView View { get; set; } = XFSchedulerView.Month;
        [Parameter] public EventCallback<XFSchedulerView> ViewChanged { get; set; }
        [Parameter] public EventCallback<XFSchedulerSlotSelectEventArgs> SlotSelect { get; set; }
        [Parameter] public EventCallback<XFSchedulerAppointmentSelectEventArgs<TItem>> AppointmentSelect { get; set; }
        [Parameter] public EventCallback<XFSchedulerAppointmentMoveEventArgs<TItem>> AppointmentMove { get; set; }
        [Parameter] public EventCallback<XFSchedulerAppointmentResizeEventArgs<TItem>> AppointmentResize { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool ShowHeader { get; set; } = true;
        [Parameter] public bool AllowAppointmentMove { get; set; }
        [Parameter] public bool AllowAppointmentResize { get; set; }
        [Parameter] public string AriaLabel { get; set; } = "Agenda";
        [Parameter] public string TodayText { get; set; } = "Hoje";
        [Parameter] public string PreviousPeriodLabel { get; set; } = "Periodo anterior";
        [Parameter] public string NextPeriodLabel { get; set; } = "Proximo periodo";
        [Parameter] public string ViewSwitcherLabel { get; set; } = "Visualizacoes da agenda";
        [Parameter] public string HourHeaderText { get; set; } = "Hora";
        [Parameter] public string WeekHeaderText { get; set; } = "Semana";
        [Parameter] public string SingleAppointmentText { get; set; } = "1 agendamento";
        [Parameter] public string MultipleAppointmentsFormat { get; set; } = "{0} agendamentos";
        [Parameter] public string MonthViewLabel { get; set; } = "Mes";
        [Parameter] public string WeekViewLabel { get; set; } = "Semana";
        [Parameter] public string DayViewLabel { get; set; } = "Dia";
        [Parameter] public string ResourceHeaderText { get; set; } = "Recurso";
        [Parameter] public string MoveAppointmentLabel { get; set; } = "Mover agendamento";
        [Parameter] public string ResizeAppointmentLabel { get; set; } = "Redimensionar agendamento";
        [Parameter] public int StartHour { get; set; } = 8;
        [Parameter] public int EndHour { get; set; } = 18;
        [Parameter] public int SlotMinutes { get; set; } = 60;
        [Parameter] public int MaxAppointmentsPerSlot { get; set; } = 3;
        [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;
        [Parameter] public CultureInfo? Culture { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFContainerVariant Variant { get; set; } = XFContainerVariant.Surface;

        private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;

        private IReadOnlyList<AppointmentInfo> Appointments => appointments;

        protected override void OnInitialized()
        {
            AriaLabel = L("scheduler.aria_label", AriaLabel);
            TodayText = L("scheduler.today", TodayText);
            PreviousPeriodLabel = L("scheduler.previous", PreviousPeriodLabel);
            NextPeriodLabel = L("scheduler.next", NextPeriodLabel);
            ViewSwitcherLabel = L("scheduler.view_switcher", ViewSwitcherLabel);
            HourHeaderText = L("scheduler.hour", HourHeaderText);
            WeekHeaderText = L("scheduler.week", WeekHeaderText);
            SingleAppointmentText = L("scheduler.single", SingleAppointmentText);
            MultipleAppointmentsFormat = L("scheduler.multiple", MultipleAppointmentsFormat);
            MonthViewLabel = L("scheduler.month", MonthViewLabel);
            WeekViewLabel = L("scheduler.week_view", WeekViewLabel);
            DayViewLabel = L("scheduler.day", DayViewLabel);
            ResourceHeaderText = L("scheduler.resource", ResourceHeaderText);
            MoveAppointmentLabel = L("scheduler.move", MoveAppointmentLabel);
            ResizeAppointmentLabel = L("scheduler.resize", ResizeAppointmentLabel);
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            currentDate = Date;
            StartHour = Math.Clamp(StartHour, 0, 23);
            EndHour = Math.Clamp(EndHour, StartHour + 1, 24);
            SlotMinutes = Math.Clamp(SlotMinutes, 15, 240);
            MaxAppointmentsPerSlot = Math.Max(1, MaxAppointmentsPerSlot);

            RefreshScheduleCache();
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {VariantClass} {SizeClass}");
        }

        private async Task GoPreviousAsync()
        {
            var next = View switch
            {
                XFSchedulerView.Day => currentDate.AddDays(-1),
                XFSchedulerView.Week => currentDate.AddDays(-7),
                _ => currentDate.AddMonths(-1)
            };

            await SetDateAsync(next);
        }

        private async Task GoNextAsync()
        {
            var next = View switch
            {
                XFSchedulerView.Day => currentDate.AddDays(1),
                XFSchedulerView.Week => currentDate.AddDays(7),
                _ => currentDate.AddMonths(1)
            };

            await SetDateAsync(next);
        }

        private Task GoTodayAsync()
        {
            return SetDateAsync(DateTime.Today);
        }

        private async Task SetDateAsync(DateTime value)
        {
            currentDate = value.Date;
            RefreshScheduleCache();
            await DateChanged.InvokeAsync(currentDate);
        }

        private async Task SetViewAsync(XFSchedulerView value)
        {
            View = value;
            RefreshScheduleCache();
            await ViewChanged.InvokeAsync(value);
        }

        private Task SelectSlotAsync(DateTime start, DateTime end)
        {
            return SlotSelect.InvokeAsync(new XFSchedulerSlotSelectEventArgs(start, end, View));
        }

        private Task SelectAppointmentAsync(AppointmentInfo appointment)
        {
            return AppointmentSelect.InvokeAsync(new XFSchedulerAppointmentSelectEventArgs<TItem>(appointment.Item, appointment.Start, appointment.End, appointment.Text));
        }

        public Task MoveAppointmentAsync(TItem item, DateTime newStart, DateTime newEnd, string? resourceId = null)
        {
            var appointment = appointments.FirstOrDefault(value => EqualityComparer<TItem>.Default.Equals(value.Item, item));
            return appointment is null
                ? Task.CompletedTask
                : AppointmentMove.InvokeAsync(new XFSchedulerAppointmentMoveEventArgs<TItem>(appointment.Item, appointment.Start, appointment.End, newStart, newEnd, resourceId ?? appointment.ResourceId));
        }

        public Task ResizeAppointmentAsync(TItem item, DateTime newStart, DateTime newEnd, string? resourceId = null)
        {
            var appointment = appointments.FirstOrDefault(value => EqualityComparer<TItem>.Default.Equals(value.Item, item));
            return appointment is null
                ? Task.CompletedTask
                : AppointmentResize.InvokeAsync(new XFSchedulerAppointmentResizeEventArgs<TItem>(appointment.Item, appointment.Start, appointment.End, newStart, newEnd, resourceId ?? appointment.ResourceId));
        }

        private AppointmentInfo? ToAppointmentInfo(TItem item)
        {
            if (item is null)
            {
                return null;
            }

            var start = GetDatePropertyValue(item, StartProperty ?? "Start");
            if (start is null)
            {
                return null;
            }

            var end = GetDatePropertyValue(item, EndProperty ?? "End") ?? start.Value.AddHours(1);
            var text = GetPropertyValue<string>(item, TextProperty ?? "Text") ?? start.Value.ToString("g", EffectiveCulture);
            var resourceId = ResolveResourceId(item);
            var resourceText = ResolveResourceText(resourceId);

            if (end < start)
            {
                end = start.Value;
            }

            return new AppointmentInfo(item, start.Value, end, text, resourceId, resourceText);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Reflection on item.GetType() is required for property name binding; consumers must preserve TItem properties at AOT deployment.")]
        private static T? GetPropertyValue<T>(object item, string propertyName)
        {
            var property = item.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            var value = property?.GetValue(item);
            return value is T typed ? typed : default;
        }

        private IReadOnlyList<DateOnly> MonthDays => monthDays;

        private IReadOnlyList<TimelineSlot> TimelineSlots => timelineSlots;

        private IReadOnlyList<DateTime> WeekDays => weekDays;

        private IReadOnlyList<string> WeekdayNames => weekdayNames;

        private IReadOnlyList<AppointmentInfo> AppointmentsForDay(DateOnly day)
        {
            return appointmentsByDay.TryGetValue(day, out var dayAppointments)
                ? dayAppointments
                : Array.Empty<AppointmentInfo>();
        }

        private IReadOnlyList<AppointmentInfo> AppointmentsForSlot(DateTime start, DateTime end)
        {
            return Appointments.Where(appointment => appointment.Start < end && appointment.End > start).ToArray();
        }

        private IReadOnlyList<AppointmentInfo> AppointmentsForSlot(DateTime start, DateTime end, string? resourceId)
        {
            return AppointmentsForSlot(start, end)
                .Where(appointment => string.Equals(appointment.ResourceId, resourceId, StringComparison.Ordinal))
                .ToArray();
        }

        private RenderFragment RenderSlot(TimelineSlot slot, string? resourceId = null, string? resourceText = null) => builder =>
        {
            var slotAppointments = resourceId is null
                ? AppointmentsForSlot(slot.Start, slot.End)
                : AppointmentsForSlot(slot.Start, slot.End, resourceId);
            var context = new XFSchedulerSlotContext<TItem>(
                slot.Start,
                slot.End,
                View,
                slotAppointments.Select(appointment => appointment.Item).ToArray(),
                resourceId,
                resourceText);

            if (SlotTemplate is not null)
            {
                builder.AddContent(0, SlotTemplate(context));
                return;
            }

            builder.OpenElement(1, "span");
            builder.AddAttribute(2, "class", "relative min-h-16 min-w-0");
            if (!string.IsNullOrWhiteSpace(resourceId))
            {
                builder.AddAttribute(3, "data-xf-scheduler-resource", resourceId);
            }

            if (!string.IsNullOrWhiteSpace(resourceText))
            {
                builder.OpenElement(4, "span");
                builder.AddAttribute(5, "class", "text-[0.6875rem] font-bold uppercase tracking-wide text-text-muted");
                builder.AddContent(6, resourceText);
                builder.CloseElement();
            }

            var overlapPositions = GetOverlapPositions(slotAppointments);
            foreach (var pos in overlapPositions)
            {
                var style = $"left:{pos.Left:F1}%;width:{pos.Width:F1}%;";
                builder.AddContent(7, RenderAppointment(pos.Appointment, false, style));
            }

            builder.CloseElement();
        };

        private RenderFragment RenderMonthSlot(DateOnly day) => builder =>
        {
            var dayAppointments = AppointmentsForDay(day);
            if (SlotTemplate is not null)
            {
                var context = new XFSchedulerSlotContext<TItem>(
                    day.ToDateTime(TimeOnly.MinValue),
                    day.ToDateTime(TimeOnly.MaxValue),
                    View,
                    dayAppointments.Select(appointment => appointment.Item).ToArray(),
                    null,
                    null);
                builder.AddContent(0, SlotTemplate(context));
                return;
            }

            builder.OpenElement(1, "span");
            builder.AddAttribute(2, "class", "relative grid gap-1");

            var visibleAppointments = dayAppointments.Take(MaxAppointmentsPerSlot).ToList();
            var overlapPositions = GetOverlapPositions(visibleAppointments);
            foreach (var pos in overlapPositions)
            {
                var style = $"left:{pos.Left:F1}%;width:{pos.Width:F1}%;";
                builder.AddContent(3, RenderAppointment(pos.Appointment, true, style));
            }

            if (dayAppointments.Count > MaxAppointmentsPerSlot)
            {
                builder.OpenElement(4, "span");
                builder.AddAttribute(5, "class", "truncate rounded bg-surface-100 px-1.5 py-0.5 text-[0.6875rem] font-semibold text-text-muted");
                builder.AddContent(6, $"+{dayAppointments.Count - MaxAppointmentsPerSlot}");
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private RenderFragment RenderAppointment(AppointmentInfo appointment, bool compact, string? inlineStyle = null) => builder =>
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "type", "button");
            builder.AddAttribute(2, "class", AppointmentClass(compact));
            builder.AddAttribute(3, "data-xf-scheduler-appointment", appointment.Text);
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => SelectAppointmentAsync(appointment)));
            builder.AddAttribute(5, "onclick:stopPropagation", true);
            builder.AddAttribute(6, "aria-label", appointment.Text);

            if (!string.IsNullOrWhiteSpace(inlineStyle))
            {
                builder.AddAttribute(7, "style", inlineStyle);
            }

            if (!string.IsNullOrWhiteSpace(appointment.ResourceId))
            {
                builder.AddAttribute(8, "data-xf-scheduler-resource", appointment.ResourceId);
            }

            if (AllowAppointmentMove)
            {
                builder.AddAttribute(9, "draggable", "true");
                builder.AddAttribute(10, "ondragstart", EventCallback.Factory.Create(this, () => OnDragStart(appointment)));
                builder.AddAttribute(11, "title", MoveAppointmentLabel);
            }

            if (AllowAppointmentResize)
            {
                builder.AddAttribute(12, "data-xf-scheduler-resizable", "true");
                builder.AddAttribute(13, "aria-description", ResizeAppointmentLabel);
            }

            var context = new XFSchedulerAppointmentContext<TItem>(
                appointment.Item,
                appointment.Start,
                appointment.End,
                appointment.Text,
                appointment.Start.Date == appointment.End.Date && appointment.Start.TimeOfDay == TimeSpan.Zero && appointment.End.TimeOfDay >= new TimeSpan(23, 59, 0),
                appointment.Start.Date != appointment.End.Date,
                appointment.ResourceId,
                appointment.ResourceText);

            if (Template is not null)
            {
                builder.AddContent(14, Template(context));
            }
            else
            {
                builder.OpenElement(15, "span");
                builder.AddAttribute(16, "class", "truncate font-semibold");
                builder.AddContent(17, appointment.Text);
                builder.CloseElement();
                if (!compact && !string.IsNullOrWhiteSpace(appointment.ResourceText))
                {
                    builder.OpenElement(18, "span");
                    builder.AddAttribute(19, "class", "truncate text-[0.6875rem] opacity-80");
                    builder.AddContent(20, appointment.ResourceText);
                    builder.CloseElement();
                }
                if (!compact)
                {
                    builder.OpenElement(21, "span");
                    builder.AddAttribute(22, "class", "truncate text-[0.6875rem] opacity-80");
                    builder.AddContent(23, $"{appointment.Start:HH:mm} - {appointment.End:HH:mm}");
                    builder.CloseElement();
                }
            }

            if (AllowAppointmentResize && !compact)
            {
                builder.OpenElement(24, "div");
                builder.AddAttribute(25, "class", "absolute bottom-0 left-0 right-0 h-2 cursor-ns-resize");
                builder.AddAttribute(26, "onmousedown:stopPropagation", true);
                builder.AddAttribute(27, "onmousedown", EventCallback.Factory.Create(this, () => StartResize(appointment)));
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        private string? ResolveResourceId(TItem item)
        {
            if (item is null)
            {
                return null;
            }

            return ResourceSelector?.Invoke(item)
                ?? (ResourceProperty is null ? null : GetPropertyValue<string>(item, ResourceProperty));
        }

        private string? ResolveResourceText(string? resourceId)
        {
            return string.IsNullOrWhiteSpace(resourceId)
                ? null
                : Resources.FirstOrDefault(resource => string.Equals(resource.Id, resourceId, StringComparison.Ordinal))?.Text ?? resourceId;
        }

        private string Title => View switch
        {
            XFSchedulerView.Day => FormatCompactDate(currentDate),
            XFSchedulerView.Week => FormatCompactRange(WeekDays.First(), WeekDays.Last()),
            _ => $"{GetMonthName(currentDate)} {currentDate:yyyy}"
        };

        private string Subtitle => Appointments.Count == 1
            ? SingleAppointmentText
            : string.Format(CultureInfo.CurrentCulture, MultipleAppointmentsFormat, Appointments.Count);

        private string TimelineHeader => View == XFSchedulerView.Day ? FormatCompactDate(currentDate) : WeekHeaderText;

        private string GetViewLabel(XFSchedulerView view)
        {
            return view switch
            {
                XFSchedulerView.Month => MonthViewLabel,
                XFSchedulerView.Week => WeekViewLabel,
                XFSchedulerView.Day => DayViewLabel,
                _ => view.ToString()
            };
        }

        private string FormatCompactRange(DateTime start, DateTime end)
        {
            if (start.Month == end.Month && start.Year == end.Year)
            {
                return $"{start:dd}-{end:dd} {GetMonthName(end)}/{end:yyyy}";
            }

            return $"{start:dd} {GetMonthName(start)} - {end:dd} {GetMonthName(end)}/{end:yyyy}";
        }

        private string FormatCompactDate(DateTime value)
        {
            return $"{value:dd} {GetMonthName(value)}/{value:yyyy}";
        }

        private string GetMonthName(DateTime value)
        {
            var name = EffectiveCulture.DateTimeFormat.GetAbbreviatedMonthName(value.Month);
            return string.IsNullOrWhiteSpace(name) ? value.ToString("MMM", EffectiveCulture) : name.TrimEnd('.');
        }

        private string VariantClass => Variant switch
        {
            XFContainerVariant.Plain => "bg-transparent text-text-primary",
            XFContainerVariant.Soft => "rounded-xl bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]",
            _ => "rounded-xl border border-border-light bg-bg-surface text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)]"
        };

        private string SizeClass => Size switch
        {
            XFSize.Sm => "p-3",
            XFSize.Lg => "p-5",
            _ => "p-4"
        };

        private string HeaderClass => "mb-4 grid gap-3 lg:grid-cols-[1fr_auto_1fr] lg:items-center";

        private string TitleClass => "truncate text-base font-bold capitalize text-text-primary";

        private string IconButtonClass => "inline-flex h-9 w-9 items-center justify-center rounded-lg border border-border-light bg-bg-surface text-text-secondary outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]";

        private string TodayButtonClass => "inline-flex h-9 items-center justify-center rounded-lg border border-border-light bg-bg-surface px-3 text-sm font-semibold text-text-secondary outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]";

        private string ViewToggleClass => "flex justify-start gap-1 rounded-lg bg-surface-100 p-1 lg:justify-self-end";

        private string ViewButtonClass(XFSchedulerView view)
        {
            return View == view
                ? "rounded-md bg-bg-surface px-3 py-1.5 text-sm font-bold text-primary shadow-[0_1px_2px_rgb(15_23_42/.08)]"
                : "rounded-md px-3 py-1.5 text-sm font-semibold text-text-secondary transition-colors hover:bg-bg-surface/70 hover:text-text-primary";
        }

        private string WeekdayGridClass => "grid grid-cols-7 gap-1 pb-2 text-center text-xs font-bold text-text-muted";

        private string MonthGridClass => "grid grid-cols-7 gap-1";

        private string MonthCellClass(DateOnly day)
        {
            var inMonth = day.Month == currentDate.Month && day.Year == currentDate.Year;
            var today = day == DateOnly.FromDateTime(DateTime.Today);
            return Join(
                "flex min-h-28 flex-col gap-2 rounded-lg border border-border-light bg-bg-surface p-2 text-left outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]",
                inMonth ? "text-text-primary" : "text-text-subtle",
                today ? "ring-2 ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]" : null);
        }

        private string MonthDayLabelClass(DateOnly day)
        {
            return day == DateOnly.FromDateTime(DateTime.Today)
                ? "inline-flex h-6 w-6 items-center justify-center rounded-full bg-primary text-xs font-bold text-primary-contrast"
                : "text-xs font-bold";
        }

        private string TimelineClass => "max-h-[42rem] overflow-auto rounded-xl border border-border-light";

        private string TimelineSlotClass(TimelineSlot slot)
        {
            var working = slot.Start.Hour is >= 9 and < 18;
            return Join(
                "grid min-h-16 grid-cols-[4rem_1fr] gap-2 border-b border-border-light bg-bg-surface p-2 text-left outline-none transition-colors hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]",
                working ? "bg-[color-mix(in_oklab,var(--color-primary)_2%,var(--color-bg-surface))]" : null);
        }

        private string AppointmentClass(bool compact)
        {
            return Join(
                "relative flex min-w-0 flex-col rounded-md bg-[color-mix(in_oklab,var(--color-primary)_12%,transparent)] px-2 py-1 text-left text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] transition-colors hover:bg-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_28%,transparent)]",
                compact ? "text-[0.6875rem]" : "text-xs");
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));

        private void RefreshScheduleCache()
        {
            appointments = BuildAppointments();
            monthDays = BuildMonthDays();
            weekDays = BuildWeekDays();
            timelineSlots = BuildTimelineSlots();
            weekdayNames = BuildWeekdayNames();
            appointmentsByDay = BuildAppointmentsByDay(appointments);
        }

        private IReadOnlyList<AppointmentInfo> BuildAppointments()
        {
            return (Data ?? Array.Empty<TItem>())
                .Select(ToAppointmentInfo)
                .Where(appointment => appointment is not null)
                .Cast<AppointmentInfo>()
                .OrderBy(appointment => appointment.Start)
                .ToArray();
        }

        private IReadOnlyList<DateOnly> BuildMonthDays()
        {
            var first = new DateOnly(currentDate.Year, currentDate.Month, 1);
            var offset = ((int)first.DayOfWeek - (int)FirstDayOfWeek + 7) % 7;
            var start = first.AddDays(-offset);
            return Enumerable.Range(0, 42).Select(start.AddDays).ToArray();
        }

        private IReadOnlyList<DateTime> BuildWeekDays()
        {
            var offset = ((int)currentDate.DayOfWeek - (int)FirstDayOfWeek + 7) % 7;
            var start = currentDate.Date.AddDays(-offset);
            return Enumerable.Range(0, 7).Select(index => start.AddDays(index)).ToArray();
        }

        private IReadOnlyList<TimelineSlot> BuildTimelineSlots()
        {
            var days = View == XFSchedulerView.Week ? weekDays : new[] { currentDate.Date };
            var slots = new List<TimelineSlot>();
            foreach (var day in days)
            {
                var cursor = day.AddHours(StartHour);
                var end = day.AddHours(EndHour);
                while (cursor < end)
                {
                    slots.Add(new TimelineSlot(cursor, cursor.AddMinutes(SlotMinutes)));
                    cursor = cursor.AddMinutes(SlotMinutes);
                }
            }

            return slots;
        }

        private IReadOnlyList<string> BuildWeekdayNames()
        {
            var names = EffectiveCulture.DateTimeFormat.AbbreviatedDayNames;
            return Enumerable.Range(0, 7).Select(index => names[((int)FirstDayOfWeek + index) % 7].ToUpperInvariant()).ToArray();
        }

        private static Dictionary<DateOnly, IReadOnlyList<AppointmentInfo>> BuildAppointmentsByDay(IReadOnlyList<AppointmentInfo> source)
        {
            var map = new Dictionary<DateOnly, List<AppointmentInfo>>();
            foreach (var appointment in source)
            {
                var start = DateOnly.FromDateTime(appointment.Start);
                var end = DateOnly.FromDateTime(appointment.End);
                var days = Math.Min(366, end.DayNumber - start.DayNumber + 1);

                for (var index = 0; index < days; index++)
                {
                    var day = start.AddDays(index);
                    if (!map.TryGetValue(day, out var list))
                    {
                        list = new List<AppointmentInfo>();
                        map[day] = list;
                    }

                    list.Add(appointment);
                }
            }

            return map.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<AppointmentInfo>)pair.Value);
        }

        private static DateTime? GetDatePropertyValue(object item, string propertyName)
        {
            var value = GetPropertyValue<object>(item, propertyName);
            return value switch
            {
                DateTime dateTime => dateTime,
                DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                string text when DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsed) => parsed,
                _ => null
            };
        }

        private sealed record TimelineSlot(DateTime Start, DateTime End);

        private sealed record AppointmentPosition
        {
            public AppointmentInfo Appointment { get; init; }
            public int Column { get; init; }
            public int TotalColumns { get; init; }
            public double Left { get; init; }
            public double Width { get; init; }
        }

        private bool HasResources => Resources.Count > 0;

        private sealed record AppointmentInfo(TItem Item, DateTime Start, DateTime End, string Text, string? ResourceId, string? ResourceText);

        private const int DayViewSlotHeightPx = 64;

        private sealed record DayViewAppointment(AppointmentInfo Appointment, int Column, int TotalColumns);

        private IReadOnlyList<AppointmentInfo> AllDayAppointments => Appointments
            .Where(a => IsAllDayAppointment(a) || IsMultiDayAppointment(a))
            .ToArray();

        private bool IsCurrentTimeVisible => DateTime.Now.Date == currentDate.Date
            && DateTime.Now.Hour >= StartHour
            && DateTime.Now.Hour < EndHour;

        private int NowPositionPx
        {
            get
            {
                var now = DateTime.Now;
                var minutes = (now.Hour - StartHour) * 60 + now.Minute;
                return (int)(minutes / 60.0 * DayViewSlotHeightPx);
            }
        }

        private static bool IsAllDayAppointment(AppointmentInfo a) =>
            a.Start.Date == a.End.Date
            && a.Start.TimeOfDay == TimeSpan.Zero
            && a.End.TimeOfDay >= new TimeSpan(23, 59, 0);

        private static bool IsMultiDayAppointment(AppointmentInfo a) =>
            a.Start.Date != a.End.Date;

        private int GetAppointmentTopPx(AppointmentInfo appointment)
        {
            var hour = Math.Max(appointment.Start.Hour, StartHour);
            var minute = appointment.Start.Hour < StartHour ? 0 : appointment.Start.Minute;
            return (int)((hour - StartHour) * 60 + minute) / 60 * DayViewSlotHeightPx;
        }

        private int GetAppointmentHeightPx(AppointmentInfo appointment)
        {
            var startHour = Math.Max(appointment.Start.Hour, StartHour);
            var startMin = appointment.Start.Hour < StartHour ? 0 : appointment.Start.Minute;
            var endHour = Math.Min(appointment.End.Hour, EndHour);
            var endMin = appointment.End.Hour >= EndHour ? 0 : appointment.End.Minute;
            var durationMinutes = Math.Max((endHour - startHour) * 60 + (endMin - startMin), 15);
            return (int)(durationMinutes / 60.0 * DayViewSlotHeightPx);
        }

        private IReadOnlyList<DayViewAppointment> GetDayViewAppointments()
        {
            var dayAppointments = Appointments
                .Where(a => !IsAllDayAppointment(a) && !IsMultiDayAppointment(a) && a.Start.Date == currentDate.Date)
                .OrderBy(a => a.Start)
                .ThenByDescending(a => (a.End - a.Start).TotalMinutes)
                .ToList();

            if (dayAppointments.Count == 0)
                return Array.Empty<DayViewAppointment>();

            var clusters = new List<List<AppointmentInfo>>();
            var cluster = new List<AppointmentInfo> { dayAppointments[0] };
            var clusterEnd = dayAppointments[0].End;

            for (var i = 1; i < dayAppointments.Count; i++)
            {
                if (dayAppointments[i].Start < clusterEnd)
                {
                    cluster.Add(dayAppointments[i]);
                    if (dayAppointments[i].End > clusterEnd)
                        clusterEnd = dayAppointments[i].End;
                }
                else
                {
                    clusters.Add(cluster);
                    cluster = new List<AppointmentInfo> { dayAppointments[i] };
                    clusterEnd = dayAppointments[i].End;
                }
            }
            clusters.Add(cluster);

            var result = new List<DayViewAppointment>();
            foreach (var group in clusters)
            {
                var columns = new List<List<AppointmentInfo>>();
                foreach (var apt in group)
                {
                    var placed = false;
                    for (var col = 0; col < columns.Count; col++)
                    {
                        if (columns[col].Last().End <= apt.Start)
                        {
                            columns[col].Add(apt);
                            placed = true;
                            break;
                        }
                    }
                    if (!placed)
                        columns.Add(new List<AppointmentInfo> { apt });
                }

                var totalCols = columns.Count;
                for (var col = 0; col < columns.Count; col++)
                {
                    foreach (var apt in columns[col])
                        result.Add(new DayViewAppointment(apt, col, totalCols));
                }
            }

            return result;
        }

        // Week view helpers
        private const int WeekSlotHeightPx = 48;

        private string WeekViewWrapperClass => "rounded-xl border border-border-light overflow-hidden";

        private string WeekAppointmentTop(AppointmentInfo appointment, DateOnly day)
        {
            var dayStart = day.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(StartHour)));
            var minutes = Math.Max(0, (appointment.Start - dayStart).TotalMinutes);
            return $"top:{minutes * WeekSlotHeightPx / 60.0:F1}px";
        }

        private string WeekAppointmentHeight(AppointmentInfo appointment)
        {
            var minutes = Math.Max(15, (appointment.End - appointment.Start).TotalMinutes);
            return $"height:{minutes * WeekSlotHeightPx / 60.0:F1}px";
        }

        private (int Column, int Total) WeekOverlapInfo(AppointmentInfo appointment, IReadOnlyList<AppointmentInfo> dayAppts)
        {
            var sorted = dayAppts.OrderBy(a => a.Start).ToList();
            var overlapping = sorted.Where(a => a.Start < appointment.End && a.End > appointment.Start).ToList();
            var col = overlapping.IndexOf(appointment);
            return (col, overlapping.Count);
        }

        private string WeekTimeIndicatorStyle()
        {
            var now = DateTime.Now;
            if (now.Hour < StartHour || now.Hour >= EndHour)
                return "display:none";
            var minutes = (now - now.Date.AddHours(StartHour)).TotalMinutes;
            return $"top:{minutes * WeekSlotHeightPx / 60.0:F1}px";
        }

        // Overlap detection algorithm
        private List<AppointmentPosition> CalculateOverlapLayout(List<AppointmentInfo> appointments)
        {
            var positions = new List<AppointmentPosition>();
            var columns = new List<List<AppointmentInfo>>();

            foreach (var apt in appointments.OrderBy(a => a.Start))
            {
                int col = 0;
                while (col < columns.Count && columns[col].Any(a => a.End > apt.Start))
                    col++;

                if (col == columns.Count)
                    columns.Add(new List<AppointmentInfo>());

                columns[col].Add(apt);

                var totalCols = columns.Count;
                positions.Add(new AppointmentPosition
                {
                    Appointment = apt,
                    Column = col,
                    TotalColumns = totalCols,
                    Left = col * 90.0 / totalCols,
                    Width = 90.0 / totalCols
                });
            }

            return positions;
        }

        private List<AppointmentPosition> GetOverlapPositions(IReadOnlyList<AppointmentInfo> dayAppointments)
        {
            return CalculateOverlapLayout(dayAppointments.ToList());
        }

        // Drag-and-drop support
        private AppointmentInfo? draggedAppointment;
        private AppointmentInfo? resizedAppointment;
        private DateTime resizeOriginalEnd;

        private void OnDragStart(AppointmentInfo appointment)
        {
            draggedAppointment = appointment;
        }

        private async Task OnDrop(DateTime slotDate)
        {
            if (draggedAppointment == null) return;

            var duration = draggedAppointment.End - draggedAppointment.Start;
            var newStart = slotDate;
            var newEnd = slotDate + duration;

            await AppointmentMove.InvokeAsync(new XFSchedulerAppointmentMoveEventArgs<TItem>(
                draggedAppointment.Item,
                draggedAppointment.Start,
                draggedAppointment.End,
                newStart,
                newEnd,
                draggedAppointment.ResourceId));

            draggedAppointment = null;
        }

        // Resize support
        private void StartResize(AppointmentInfo appointment)
        {
            resizedAppointment = appointment;
            resizeOriginalEnd = appointment.End;
        }

        private async Task OnResizeEnd(DateTime newEnd)
        {
            if (resizedAppointment == null) return;

            if (newEnd > resizedAppointment.Start)
            {
                await AppointmentResize.InvokeAsync(new XFSchedulerAppointmentResizeEventArgs<TItem>(
                    resizedAppointment.Item,
                    resizedAppointment.Start,
                    resizedAppointment.End,
                    resizedAppointment.Start,
                    newEnd,
                    resizedAppointment.ResourceId));
            }

            resizedAppointment = null;
        }
    }
}
