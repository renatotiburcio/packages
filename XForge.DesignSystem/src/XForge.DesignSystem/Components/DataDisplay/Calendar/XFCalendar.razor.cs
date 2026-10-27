using System.Globalization;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.DesignSystem.Components.Shared;

namespace XForge.DesignSystem.Components.DataDisplay.Calendar
{
    public partial class XFCalendar : XForgeComponentBase
    {
        protected override string BaseClass => "xf-calendar";

        private DateOnly currentDate = DateOnly.FromDateTime(DateTime.Today);
        private DateOnly? _prevDisplayDate;
        private DateOnly? _prevValue;
        private DateOnly? _hoverDate;
        private DateOnly? _rangeStart;
        private DateOnly? _rangeEnd;
        private HashSet<DateOnly> _selectedDatesSet = new();

        [Parameter] public DateOnly? Value { get; set; }
        [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
        [Parameter] public EventCallback<DateOnly?> Change { get; set; }
        [Parameter] public XFCalendarSelectMode SelectMode { get; set; } = XFCalendarSelectMode.Single;
        [Parameter] public IReadOnlyList<DateOnly> SelectedDates { get; set; } = Array.Empty<DateOnly>();
        [Parameter] public EventCallback<IReadOnlyList<DateOnly>> SelectedDatesChanged { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<DateOnly>> SelectionChange { get; set; }
        [Parameter] public DateOnly? DisplayDate { get; set; }
        [Parameter] public EventCallback<DateOnly> DisplayDateChanged { get; set; }
        [Parameter] public DateOnly? Min { get; set; }
        [Parameter] public DateOnly? Max { get; set; }
        [Parameter] public IReadOnlyList<XFCalendarHighlight> Highlights { get; set; } = Array.Empty<XFCalendarHighlight>();
        [Parameter] public Func<DateOnly, bool>? DisabledDate { get; set; }
        [Parameter] public RenderFragment<XFCalendarDayContext>? DayTemplate { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string AriaLabel { get; set; } = "Calendar";
        [Parameter] public XFCalendarView View { get; set; } = XFCalendarView.Month;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFContainerVariant Variant { get; set; } = XFContainerVariant.Surface;
        [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;
        [Parameter] public CultureInfo? Culture { get; set; }
        [Parameter] public string DayViewLabel { get; set; } = "Daily view";
        [Parameter] public string WeekViewLabel { get; set; } = "Weekly view";
        [Parameter] public string MonthViewLabel { get; set; } = "Monthly view";
        [Parameter] public string DayAriaLabelFormat { get; set; } = "{0:D}";
        [Parameter] public string PreviousDayLabel { get; set; } = "Previous day";
        [Parameter] public string PreviousWeekLabel { get; set; } = "Previous week";
        [Parameter] public string PreviousMonthLabel { get; set; } = "Previous month";
        [Parameter] public string NextDayLabel { get; set; } = "Next day";
        [Parameter] public string NextWeekLabel { get; set; } = "Next week";
        [Parameter] public string NextMonthLabel { get; set; } = "Next month";

        private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;

        private DateOnly CurrentDate => DisplayDate ?? currentDate;

        private Dictionary<DateOnly, List<XFCalendarHighlight>> _highlightMap = new();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // External DisplayDate changed
            if (DisplayDate != _prevDisplayDate)
            {
                _prevDisplayDate = DisplayDate;
                if (DisplayDate.HasValue)
                {
                    currentDate = DisplayDate.Value;
                }
            }

            // External Value changed (not from our own navigation)
            if (Value != _prevValue)
            {
                _prevValue = Value;
                if (!DisplayDate.HasValue && Value.HasValue)
                {
                    currentDate = Value.Value;
                }
            }

            // Build highlight map for O(1) lookup
            _highlightMap = new Dictionary<DateOnly, List<XFCalendarHighlight>>();
            foreach (var h in Highlights)
            {
                if (!_highlightMap.TryGetValue(h.Date, out var list))
                {
                    list = new List<XFCalendarHighlight>();
                    _highlightMap[h.Date] = list;
                }
                list.Add(h);
            }
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {VariantClass} {SizeClass}");
        }

        private async Task GoPreviousAsync()
        {
            var prev = View switch
            {
                XFCalendarView.Day => CurrentDate.AddDays(-1),
                XFCalendarView.Week => CurrentDate.AddDays(-7),
                _ => CurrentDate.AddMonths(-1)
            };

            await SetDisplayDateAsync(prev);
        }

        private async Task GoNextAsync()
        {
            var next = View switch
            {
                XFCalendarView.Day => CurrentDate.AddDays(1),
                XFCalendarView.Week => CurrentDate.AddDays(7),
                _ => CurrentDate.AddMonths(1)
            };

            await SetDisplayDateAsync(next);
        }

        private async Task SetDisplayDateAsync(DateOnly value)
        {
            currentDate = value;
            _prevDisplayDate = null;
            _prevValue = null;
            await DisplayDateChanged.InvokeAsync(value);
        }

        private async Task SelectDateAsync(DateOnly value)
        {
            if (IsDisabled(value))
            {
                return;
            }

            if (SelectMode == XFCalendarSelectMode.Range)
            {
                if (_rangeStart is null || (_rangeStart is not null && _rangeEnd is not null))
                {
                    _rangeStart = value;
                    _rangeEnd = null;
                }
                else
                {
                    _rangeEnd = value;
                    if (_rangeStart > _rangeEnd)
                    {
                        (_rangeStart, _rangeEnd) = (_rangeEnd, _rangeStart);
                    }

                    if (_rangeStart is not null && _rangeEnd is not null)
                    {
                        var range = Enumerable.Range(_rangeStart.Value.DayNumber, _rangeEnd.Value.DayNumber - _rangeStart.Value.DayNumber + 1)
                            .Select(DateOnly.FromDayNumber).ToList();
                        await SelectedDatesChanged.InvokeAsync(range);
                        await SelectionChange.InvokeAsync(range);
                    }
                }
            }
            else if (SelectMode == XFCalendarSelectMode.Multiple)
            {
                if (_selectedDatesSet.Contains(value))
                {
                    _selectedDatesSet.Remove(value);
                }
                else
                {
                    _selectedDatesSet.Add(value);
                }
                var dates = _selectedDatesSet.OrderBy(d => d).ToList();
                await SelectedDatesChanged.InvokeAsync(dates);
                await SelectionChange.InvokeAsync(dates);
            }
            else
            {
                currentDate = value;
                _prevValue = value;
                await ValueChanged.InvokeAsync(value);
                await Change.InvokeAsync(value);
            }
        }

        private void HandleDayHover(DateOnly date)
        {
            if (SelectMode == XFCalendarSelectMode.Range && _rangeStart is not null && _rangeEnd is null)
            {
                _hoverDate = date;
            }
        }

        private void HandleDayHoverLeave()
        {
            _hoverDate = null;
        }

        private IReadOnlyList<IReadOnlyList<DateOnly>> MonthWeeks
        {
            get
            {
                var first = new DateOnly(CurrentDate.Year, CurrentDate.Month, 1);
                var startOffset = ((int)first.DayOfWeek - (int)FirstDayOfWeek + 7) % 7;
                var start = first.AddDays(-Math.Min(startOffset, first.DayNumber - 1));
                var maxDayNumber = DateOnly.MaxValue.DayNumber;
                return Enumerable.Range(0, 6)
                    .Select(week => Enumerable.Range(0, 7)
                        .Select(day =>
                        {
                            var dayNum = start.DayNumber + (week * 7) + day;
                            return dayNum <= maxDayNumber ? DateOnly.FromDayNumber(dayNum) : DateOnly.MaxValue;
                        }).ToArray())
                    .ToArray();
            }
        }

        private IReadOnlyList<DateOnly> WeekDays
        {
            get
            {
                var offset = ((int)CurrentDate.DayOfWeek - (int)FirstDayOfWeek + 7) % 7;
                var safeOffset = Math.Min(offset, CurrentDate.DayNumber - 1);
                var start = CurrentDate.AddDays(-safeOffset);
                return Enumerable.Range(0, 7).Select(i =>
                {
                    var dayNum = start.DayNumber + i;
                    return dayNum <= DateOnly.MaxValue.DayNumber ? DateOnly.FromDayNumber(dayNum) : DateOnly.MaxValue;
                }).ToArray();
            }
        }

        private IReadOnlyList<string> WeekdayNames
        {
            get
            {
                var names = EffectiveCulture.DateTimeFormat.AbbreviatedDayNames;
                return Enumerable.Range(0, 7).Select(index => names[((int)FirstDayOfWeek + index) % 7].ToUpperInvariant()).ToArray();
            }
        }

        private XFCalendarDayContext CreateContext(DateOnly date)
        {
            _highlightMap.TryGetValue(date, out var dayHighlights);
            var isSelected = SelectMode == XFCalendarSelectMode.Single ? Value == date : false;
            var isInRange = false;
            var isRangeStart = false;
            var isRangeEnd = false;
            var isMultipleSelected = false;

            if (SelectMode == XFCalendarSelectMode.Range)
            {
                var start = _rangeStart;
                var end = _rangeEnd ?? _hoverDate;
                if (start is not null && end is not null)
                {
                    var minD = start < end ? start.Value : end.Value;
                    var maxD = start < end ? end.Value : start.Value;
                    isInRange = date >= minD && date <= maxD;
                    isRangeStart = date == minD;
                    isRangeEnd = date == maxD;
                }
                else if (start is not null)
                {
                    isRangeStart = date == start;
                }
            }
            else if (SelectMode == XFCalendarSelectMode.Multiple)
            {
                isMultipleSelected = _selectedDatesSet.Contains(date);
            }

            return new XFCalendarDayContext(
                date,
                date.Month == CurrentDate.Month && date.Year == CurrentDate.Year,
                isSelected,
                date == DateOnly.FromDateTime(DateTime.Today),
                IsDisabled(date),
                (IReadOnlyList<XFCalendarHighlight>?)dayHighlights ?? Array.Empty<XFCalendarHighlight>(),
                isInRange,
                isRangeStart,
                isRangeEnd,
                isMultipleSelected);
        }

        private bool IsDisabled(DateOnly date)
        {
            if (Min is not null && date < Min)
            {
                return true;
            }

            if (Max is not null && date > Max)
            {
                return true;
            }

            return DisabledDate?.Invoke(date) == true;
        }

        private string Title => View switch
        {
            XFCalendarView.Day => CurrentDate.ToString("D", EffectiveCulture),
            XFCalendarView.Week => $"{WeekDays.First():dd MMM} - {WeekDays.Last():dd MMM yyyy}",
            _ => CurrentDate.ToString("MMMM yyyy", EffectiveCulture)
        };

        private string Subtitle => View switch
        {
            XFCalendarView.Day => DayViewLabel,
            XFCalendarView.Week => WeekViewLabel,
            _ => MonthViewLabel
        };

        private string PreviousLabel => View switch
        {
            XFCalendarView.Day => PreviousDayLabel,
            XFCalendarView.Week => PreviousWeekLabel,
            _ => PreviousMonthLabel
        };

        private string NextLabel => View switch
        {
            XFCalendarView.Day => NextDayLabel,
            XFCalendarView.Week => NextWeekLabel,
            _ => NextMonthLabel
        };

        private string GetWeekdayName(DateOnly date) => EffectiveCulture.DateTimeFormat.AbbreviatedDayNames[(int)date.DayOfWeek];

        private string GetMonthYear(DateOnly date) => date.ToString("MMMM yyyy", EffectiveCulture);

        private string GetDayAriaLabel(DateOnly date) => string.Format(
            EffectiveCulture,
            DayAriaLabelFormat,
            date.ToDateTime(TimeOnly.MinValue));

        private string GetHighlightDotClass(XFSemanticColor color) => color switch
        {
            XFSemanticColor.Success => "h-1.5 w-1.5 rounded-full bg-success",
            XFSemanticColor.Warning => "h-1.5 w-1.5 rounded-full bg-warning",
            XFSemanticColor.Danger => "h-1.5 w-1.5 rounded-full bg-danger",
            XFSemanticColor.Info => "h-1.5 w-1.5 rounded-full bg-info",
            XFSemanticColor.Secondary => "h-1.5 w-1.5 rounded-full bg-text-secondary",
            _ => "h-1.5 w-1.5 rounded-full bg-primary"
        };

        private string GetHighlightDotClassLg(XFSemanticColor color) => color switch
        {
            XFSemanticColor.Success => "h-2 w-2 rounded-full bg-success",
            XFSemanticColor.Warning => "h-2 w-2 rounded-full bg-warning",
            XFSemanticColor.Danger => "h-2 w-2 rounded-full bg-danger",
            XFSemanticColor.Info => "h-2 w-2 rounded-full bg-info",
            XFSemanticColor.Secondary => "h-2 w-2 rounded-full bg-text-secondary",
            _ => "h-2 w-2 rounded-full bg-primary"
        };

        private static XFTooltipTheme GetHighlightTheme(XFSemanticColor color) => color switch
        {
            XFSemanticColor.Success => XFTooltipTheme.Success,
            XFSemanticColor.Warning => XFTooltipTheme.Warning,
            XFSemanticColor.Danger => XFTooltipTheme.Danger,
            XFSemanticColor.Info => XFTooltipTheme.Info,
            XFSemanticColor.Secondary => XFTooltipTheme.Dark,
            _ => XFTooltipTheme.Primary
        };

        private string VariantClass => Variant switch
        {
            XFContainerVariant.Plain => "bg-transparent text-text-primary",
            XFContainerVariant.Soft => "rounded-xl bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] text-text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]",
            _ => "rounded-xl border border-border-light bg-bg-surface text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)]"
        };

        private string SizeClass => Size switch
        {
            XFSize.Sm => "w-full p-2",
            XFSize.Lg => "w-full p-5",
            _ => "w-full p-4"
        };

        private string HeaderClass => Size switch
        {
            XFSize.Sm => "mb-2 flex items-center justify-between gap-2",
            XFSize.Lg => "mb-5 flex items-center justify-between gap-4",
            _ => "mb-4 flex items-center justify-between gap-3"
        };

        private string TitleClass => Size switch
        {
            XFSize.Sm => "truncate text-sm font-bold capitalize text-text-primary",
            XFSize.Lg => "truncate text-lg font-bold capitalize text-text-primary",
            _ => "truncate text-base font-bold capitalize text-text-primary"
        };

        private string SubtitleClass => Size switch
        {
            XFSize.Sm => "text-[10px] font-medium text-text-muted",
            XFSize.Lg => "text-xs font-medium text-text-muted",
            _ => "text-xs font-medium text-text-muted"
        };

        private string NavButtonClass => Size switch
        {
            XFSize.Sm => "inline-flex h-7 w-7 items-center justify-center rounded-md border border-border-light bg-bg-surface text-text-secondary outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]",
            XFSize.Lg => "inline-flex h-10 w-10 items-center justify-center rounded-lg border border-border-light bg-bg-surface text-text-secondary outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]",
            _ => "inline-flex h-9 w-9 items-center justify-center rounded-lg border border-border-light bg-bg-surface text-text-secondary outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]"
        };

        private string NavIconClass => Size switch
        {
            XFSize.Sm => "inline-block h-3 w-3",
            XFSize.Lg => "inline-block h-5 w-5",
            _ => "inline-block h-4 w-4"
        };

        private string WeekdayGridClass => Size switch
        {
            XFSize.Sm => "grid grid-cols-7 gap-0.5 pb-1 text-center text-[10px] font-bold text-text-muted",
            XFSize.Lg => "grid grid-cols-7 gap-1.5 pb-3 text-center text-sm font-bold text-text-muted",
            _ => "grid grid-cols-7 gap-1 pb-2 text-center text-xs font-bold text-text-muted"
        };

        private string DayGridClass => Size switch
        {
            XFSize.Sm => "grid grid-cols-7 gap-0.5",
            XFSize.Lg => "grid grid-cols-7 gap-1.5",
            _ => "grid grid-cols-7 gap-1"
        };

        private string WeekViewClass => Size switch
        {
            XFSize.Sm => "grid gap-1 sm:grid-cols-7",
            XFSize.Lg => "grid gap-3 sm:grid-cols-7",
            _ => "grid gap-2 sm:grid-cols-7"
        };

        private string DayButtonClass(XFCalendarDayContext context)
        {
            var baseSize = Size switch
            {
                XFSize.Sm => "min-h-8 text-xs",
                XFSize.Lg => "min-h-12 text-base",
                _ => "min-h-10 text-sm"
            };

            return Join(
                $"inline-flex aspect-square {baseSize} flex-col items-center justify-center gap-1 rounded-lg outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)] disabled:pointer-events-none disabled:opacity-45",
                context.IsSelected ? "bg-primary text-white shadow-[0_8px_18px_rgb(15_23_42/.12)]" : null,
                context.IsRangeStart || context.IsRangeEnd ? "bg-primary text-white shadow-[0_8px_18px_rgb(15_23_42/.12)]" : null,
                context.IsInRange && !context.IsRangeStart && !context.IsRangeEnd ? "bg-[color-mix(in_oklab,var(--color-primary)_15%,transparent)] text-primary" : null,
                context.IsMultipleSelected && !context.IsSelected ? "bg-[color-mix(in_oklab,var(--color-primary)_15%,transparent)] text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]" : null,
                !context.IsSelected && !context.IsRangeStart && !context.IsRangeEnd && context.IsToday ? "text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]" : null,
                !context.IsSelected && !context.IsRangeStart && !context.IsRangeEnd && !context.IsCurrentMonth ? "text-text-subtle hover:text-text-primary" : null,
                !context.IsSelected && !context.IsRangeStart && !context.IsRangeEnd && context.IsCurrentMonth ? "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)]" : null);
        }

        private string WeekDayClass(XFCalendarDayContext context)
        {
            var baseSize = Size switch
            {
                XFSize.Sm => "min-h-20 p-2",
                XFSize.Lg => "min-h-36 p-4",
                _ => "min-h-28 p-3"
            };

            return Join(
                $"flex {baseSize} flex-col items-start justify-between rounded-lg border border-border-light bg-bg-surface text-left outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)] disabled:pointer-events-none disabled:opacity-45",
                context.IsSelected ? "border-[var(--color-primary)] bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]" : null);
        }

        private string WeekDayNameClass(XFCalendarDayContext context)
        {
            return Join(
                "text-xs font-semibold uppercase text-text-muted",
                context.IsSelected ? "text-primary" : null);
        }

        private string WeekDayNumberClass(XFCalendarDayContext context)
        {
            return Join(
                "text-lg font-bold",
                context.IsSelected ? "text-primary" : null,
                context.IsToday && !context.IsSelected ? "text-primary" : null);
        }

        private string SingleDayClass(XFCalendarDayContext context)
        {
            return Join(
                "flex min-h-72 w-full flex-col items-center justify-center gap-3 rounded-xl border border-border-light bg-bg-surface p-6 text-center outline-none transition-all duration-150 hover:border-[var(--color-primary)] hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)] disabled:pointer-events-none disabled:opacity-45",
                context.IsSelected ? "border-[var(--color-primary)] text-primary ring-2 ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]" : null);
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
