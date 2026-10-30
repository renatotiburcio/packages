using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.DatePicker
{
    public partial class XFDatePicker : XForgeComponentBase
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public DateOnly? Value { get; set; }
        [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<DateOnly?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<DateOnly?> Change { get; set; }
        [Parameter] public DateOnly? Min { get; set; }
        [Parameter] public DateOnly? Max { get; set; }
        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
        [Parameter] public string? CultureName { get; set; }
        [Parameter] public string? DisplayFormat { get; set; }
        [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public bool Clamp { get; set; } = true;
        [Parameter] public bool Clearable { get; set; } = true;
        [Parameter] public bool OpenOnFocus { get; set; }

        [Inject] private IXFLocalizer? Localizer { get; set; }

        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public string ToggleLabel { get; set; } = default!;
        [Parameter] public string CalendarLabel { get; set; } = default!;
        [Parameter] public string PreviousMonthLabel { get; set; } = default!;
        [Parameter] public string NextMonthLabel { get; set; } = default!;
        [Parameter] public string TodayLabel { get; set; } = default!;
        [Parameter] public string ClearLabel { get; set; } = default!;
        [Parameter] public string SelectedLabel { get; set; } = default!;
        [Parameter] public string DisabledDayLabel { get; set; } = default!;

        // --- Enterprise parameters (S-G012) ---

        /// <summary>Renders the calendar always visible without the input field.</summary>
        [Parameter] public bool Inline { get; set; }

        /// <summary>Callback to customize each day cell: set CssClass or override IsDisabled.</summary>
        [Parameter] public Action<XFDatePickerDateRenderArgs>? DateRender { get; set; }

        /// <summary>Specific dates that should be disabled (e.g., holidays).</summary>
        [Parameter] public IEnumerable<DateOnly>? DisabledDates { get; set; }

        /// <summary>Days of week that should always be disabled (e.g., weekends).</summary>
        [Parameter] public IEnumerable<DayOfWeek>? DisabledDaysOfWeek { get; set; }

        /// <summary>Custom footer content rendered below the calendar grid.</summary>
        [Parameter] public RenderFragment<XFDatePickerFooterContext>? FooterTemplate { get; set; }

        /// <summary>Shows ISO week numbers in a column before the first day.</summary>
        [Parameter] public bool ShowWeekNumbers { get; set; }

        /// <summary>Controls which calendar view is displayed: Month, Year, or Decade.</summary>
        [Parameter] public XFDatePickerView View { get; set; } = XFDatePickerView.Month;

        /// <summary>Fires when the user navigates to a different month or year.</summary>
        [Parameter] public EventCallback<DateOnly> ViewChanged { get; set; }

        /// <summary>Year range displayed in the decade view (e.g., 20 means current year +/- 10).</summary>
        [Parameter] public int YearRange { get; set; } = 20;

        /// <summary>Label for the year navigation button.</summary>
        [Parameter] public string PreviousYearLabel { get; set; } = default!;

        /// <summary>Label for the year navigation button.</summary>
        [Parameter] public string NextYearLabel { get; set; } = default!;

        /// <summary>Label for the decade navigation button.</summary>
        [Parameter] public string PreviousDecadeLabel { get; set; } = default!;

        /// <summary>Label for the decade navigation button.</summary>
        [Parameter] public string NextDecadeLabel { get; set; } = default!;

        /// <summary>Format for week number aria label.</summary>
        [Parameter] public string WeekNumberLabelFormat { get; set; } = default!;

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private DateOnly? CurrentValue { get; set; }
        private DateOnly? LastParameterValue { get; set; }
        private DateOnly VisibleMonth { get; set; }
        private bool IsOpen { get; set; }
        private string? InputText { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string AriaInvalid => HasValidationError ? "true" : "false";
        private string DisplayValue => InputText ?? FormatDisplayDate(CurrentValue);
        private string MonthTitle => VisibleMonth.ToString("MMMM yyyy", Culture);
        private DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
        private bool ToggleDisabled => Disabled || ReadOnly;
        private bool IsTodayDisabled => Disabled || ReadOnly || IsDateDisabled(Today);

        private bool IsCalendarVisible => Inline || IsOpen;

        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(FieldIdentifierValue!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && ValueExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string? DescribedBy => HasValidationError
            ? ErrorId
            : !string.IsNullOrWhiteSpace(HelperText) ? HelperId : null;

        private IReadOnlyList<string> WeekdayNames
        {
            get
            {
                var names = Culture.DateTimeFormat.AbbreviatedDayNames;
                return Enumerable.Range(0, 7)
                    .Select(index => names[((int)FirstDayOfWeek + index) % 7])
                    .ToArray();
            }
        }

        private IReadOnlyList<XFDatePickerDay> CalendarDays
        {
            get
            {
                var firstOfMonth = new DateOnly(VisibleMonth.Year, VisibleMonth.Month, 1);
                var offset = ((int)firstOfMonth.DayOfWeek - (int)FirstDayOfWeek + 7) % 7;
                var start = firstOfMonth.AddDays(-offset);

                return Enumerable.Range(0, 42)
                    .Select(index =>
                    {
                        var date = start.AddDays(index);
                        var isDisabled = IsDateDisabled(date);
                        var args = new XFDatePickerDateRenderArgs
                        {
                            Date = date,
                            IsCurrentMonth = date.Month == VisibleMonth.Month,
                            IsSelected = CurrentValue == date,
                            IsToday = Today == date,
                            IsDisabled = isDisabled
                        };

                        DateRender?.Invoke(args);

                        return new XFDatePickerDay(
                            date,
                            args.IsCurrentMonth,
                            args.IsSelected,
                            args.IsToday,
                            args.IsDisabled,
                            args.CssClass);
                    })
                    .ToArray();
            }
        }

        private string RootClass =>
            Join("xf-date-picker flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string PickerShellClass => "relative";

        private string FieldShellClass =>
            Join(
                "group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out",
                SizeClass,
                VariantClass,
                ToneClass,
                StateClass);

        private string InputClass =>
            Join(
                "min-w-0 flex-1 border-0 bg-transparent p-0 text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted",
                SizeTextClass);

        private string AdornmentClass =>
            "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string IconButtonClass =>
            "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95 disabled:pointer-events-none disabled:text-text-muted";

        private string CalendarContainerClass => Inline
            ? "w-80 rounded-lg border border-surface-200 bg-bg-surface p-3"
            : "absolute left-0 top-full z-50 mt-2 w-80 rounded-lg border border-surface-200 bg-bg-surface p-3 shadow-[0_18px_40px_rgb(15_23_42/.14)] ring-1 ring-surface-200/80";

        private string NavButtonClass =>
            "inline-flex h-8 w-8 items-center justify-center rounded-md text-text-secondary outline-none transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";

        private string FooterButtonClass =>
            "inline-flex h-8 items-center rounded-md px-2.5 text-sm font-medium text-primary outline-none transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] disabled:pointer-events-none disabled:text-text-muted";

        private string HelperClass => "text-xs leading-5 text-text-secondary";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "h-9 py-1.5 pl-3 pr-1.5",
            XFSize.Lg => "h-12 py-2.5 pl-4 pr-2",
            _ => "h-10 py-2 pl-3.5 pr-1.5"
        };

        private string SizeTextClass => Size switch
        {
            XFSize.Sm => "text-sm",
            XFSize.Lg => "text-base",
            _ => "text-sm"
        };

        private string VariantClass => Variant switch
        {
            XFFormVariant.Filled => "border-transparent bg-surface-100 ring-1 ring-surface-200/70 shadow-[inset_0_1px_0_rgb(255_255_255/.55)]",
            XFFormVariant.Soft => "border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)] bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] ring-1 ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]",
            XFFormVariant.Ghost => "border-transparent bg-transparent ring-1 ring-transparent hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]",
            _ => "border-surface-200 bg-bg-surface ring-1 ring-surface-200/80 shadow-[0_1px_2px_rgb(15_23_42/.035)]"
        };

        private string ToneClass
        {
            get
            {
                if (HasValidationError)
                {
                    return "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] ring-2 ring-[color-mix(in_oklab,var(--color-danger)_15%,transparent)] focus-within:border-[var(--color-danger)] focus-within:ring-[color-mix(in_oklab,var(--color-danger)_25%,transparent)]";
                }

                return Tone switch
                {
                    XFSemanticColor.Primary => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
                    XFSemanticColor.Secondary => "focus-within:border-[var(--color-secondary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]",
                    XFSemanticColor.Success => "focus-within:border-[var(--color-success)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]",
                    XFSemanticColor.Warning => "focus-within:border-[var(--color-warning)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]",
                    XFSemanticColor.Danger => "focus-within:border-[var(--color-danger)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]",
                    _ => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
                };
            }
        }

        private string StateClass => Disabled
            ? "cursor-not-allowed opacity-60"
            : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]";

        // --- Year/Decade view helpers ---

        private int DecadeStart => VisibleMonth.Year - (VisibleMonth.Year % 10);

        private IReadOnlyList<int> DecadeYears => Enumerable.Range(0, 10)
            .Select(i => DecadeStart + i)
            .ToArray();

        private IReadOnlyList<int> MonthRange => Enumerable.Range(1, 12).ToArray();

        private string GetMonthCellClass(int month)
        {
            var isCurrent = month == VisibleMonth.Month;
            return Join(
                "inline-flex h-10 items-center justify-center rounded-md text-sm outline-none transition-all duration-150",
                isCurrent
                    ? "bg-primary text-white"
                    : "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] hover:text-primary",
                "focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_26%,transparent)]");
        }

        private string GetYearCellClass(int year)
        {
            var isCurrent = year == VisibleMonth.Year;
            return Join(
                "inline-flex h-10 items-center justify-center rounded-md text-sm outline-none transition-all duration-150",
                isCurrent
                    ? "bg-primary text-white"
                    : "text-text-primary hover:bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] hover:text-primary",
                "focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_26%,transparent)]");
        }

        private string GetWeekNumberClass() =>
            "inline-flex h-9 w-8 items-center justify-center text-[11px] font-medium text-text-muted";

        // --- Footer context ---

        private XFDatePickerFooterContext FooterContext => new()
        {
            Value = CurrentValue,
            Today = Today,
            SelectTodayAsync = SelectTodayAsync,
            ClearAsync = ClearAsync,
            IsTodayDisabled = IsTodayDisabled,
            IsClearDisabled = Disabled || ReadOnly || CurrentValue is null
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrWhiteSpace(ToggleLabel)) ToggleLabel = L("datepicker.open", ToggleLabel);
            if (string.IsNullOrWhiteSpace(CalendarLabel)) CalendarLabel = L("datepicker.label", CalendarLabel);
            if (string.IsNullOrWhiteSpace(PreviousMonthLabel)) PreviousMonthLabel = L("datepicker.prev_month", PreviousMonthLabel);
            if (string.IsNullOrWhiteSpace(NextMonthLabel)) NextMonthLabel = L("datepicker.next_month", NextMonthLabel);
            if (string.IsNullOrWhiteSpace(TodayLabel)) TodayLabel = L("datepicker.today", TodayLabel);
            if (string.IsNullOrWhiteSpace(ClearLabel)) ClearLabel = L("datepicker.clear", ClearLabel);
            if (string.IsNullOrWhiteSpace(SelectedLabel)) SelectedLabel = L("datepicker.selected", SelectedLabel);
            if (string.IsNullOrWhiteSpace(DisabledDayLabel)) DisabledDayLabel = L("datepicker.unavailable", DisabledDayLabel);
            if (string.IsNullOrWhiteSpace(PreviousYearLabel)) PreviousYearLabel = L("datepicker.prev_year", PreviousYearLabel);
            if (string.IsNullOrWhiteSpace(NextYearLabel)) NextYearLabel = L("datepicker.next_year", NextYearLabel);
            if (string.IsNullOrWhiteSpace(PreviousDecadeLabel)) PreviousDecadeLabel = L("datepicker.prev_decade", PreviousDecadeLabel);
            if (string.IsNullOrWhiteSpace(NextDecadeLabel)) NextDecadeLabel = L("datepicker.next_decade", NextDecadeLabel);
            if (string.IsNullOrWhiteSpace(WeekNumberLabelFormat)) WeekNumberLabelFormat = L("datepicker.week_format", WeekNumberLabelFormat);
        }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrWhiteSpace(CultureName))
            {
                Culture = CultureInfo.GetCultureInfo(CultureName);
            }

            if (Value != LastParameterValue)
            {
                CurrentValue = Value;
                LastParameterValue = Value;
                InputText = null;
                VisibleMonth = new DateOnly((Value ?? Today).Year, (Value ?? Today).Month, 1);
            }
            else if (VisibleMonth == default)
            {
                VisibleMonth = new DateOnly((CurrentValue ?? Today).Year, (CurrentValue ?? Today).Month, 1);
            }

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private Task HandleFocusAsync()
        {
            if (OpenOnFocus && !ToggleDisabled)
            {
                IsOpen = true;
            }

            return Task.CompletedTask;
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            InputText = args.Value?.ToString();
            if (Immediate)
            {
                await SetValueFromStringAsync(InputText, notifyChange: true, close: false);
            }
        }

        private async Task HandleChangeAsync(ChangeEventArgs args)
        {
            InputText = args.Value?.ToString();
            await SetValueFromStringAsync(InputText, notifyChange: true, close: false);
        }

        private Task ToggleAsync()
        {
            if (!ToggleDisabled)
            {
                IsOpen = !IsOpen;
            }

            return Task.CompletedTask;
        }

        private Task PreviousMonthAsync()
        {
            VisibleMonth = VisibleMonth.AddMonths(-1);
            return NotifyViewChangedAsync();
        }

        private Task NextMonthAsync()
        {
            VisibleMonth = VisibleMonth.AddMonths(1);
            return NotifyViewChangedAsync();
        }

        private Task PreviousYearAsync()
        {
            VisibleMonth = VisibleMonth.AddYears(-1);
            return NotifyViewChangedAsync();
        }

        private Task NextYearAsync()
        {
            VisibleMonth = VisibleMonth.AddYears(1);
            return NotifyViewChangedAsync();
        }

        private Task PreviousDecadeAsync()
        {
            VisibleMonth = VisibleMonth.AddYears(-10);
            return NotifyViewChangedAsync();
        }

        private Task NextDecadeAsync()
        {
            VisibleMonth = VisibleMonth.AddYears(10);
            return NotifyViewChangedAsync();
        }

        private Task SwitchToMonthView()
        {
            View = XFDatePickerView.Month;
            return Task.CompletedTask;
        }

        private Task SwitchToYearView()
        {
            View = XFDatePickerView.Year;
            return Task.CompletedTask;
        }

        private Task SwitchToDecadeView()
        {
            View = XFDatePickerView.Decade;
            return Task.CompletedTask;
        }

        private Task SelectMonthAsync(int month)
        {
            VisibleMonth = new DateOnly(VisibleMonth.Year, month, 1);
            View = XFDatePickerView.Month;
            return NotifyViewChangedAsync();
        }

        private Task SelectYearAsync(int year)
        {
            VisibleMonth = new DateOnly(year, VisibleMonth.Month, 1);
            View = XFDatePickerView.Year;
            return NotifyViewChangedAsync();
        }

        private Task SelectTodayAsync()
        {
            return SelectDateAsync(Today);
        }

        private async Task SelectDateAsync(DateOnly date)
        {
            if (IsDateDisabled(date))
            {
                return;
            }

            await SetValueAsync(NormalizeDate(date), notifyChange: true, close: !Inline);
        }

        private Task ClearAsync()
        {
            return SetValueAsync(null, notifyChange: true, close: !Inline);
        }

        private async Task HandleRootKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Escape" && !Inline)
            {
                IsOpen = false;
            }
            else if ((args.Key == "ArrowDown" || args.Key == "Enter") && !IsOpen && !ToggleDisabled && !Inline)
            {
                IsOpen = true;
            }

            await Task.CompletedTask;
        }

        private async Task HandleDayKeyDownAsync(KeyboardEventArgs args, DateOnly date)
        {
            var next = args.Key switch
            {
                "ArrowLeft" => date.AddDays(-1),
                "ArrowRight" => date.AddDays(1),
                "ArrowUp" => date.AddDays(-7),
                "ArrowDown" => date.AddDays(7),
                "Home" => date.AddDays(-((int)date.DayOfWeek - (int)FirstDayOfWeek + 7) % 7),
                "End" => date.AddDays(6 - (((int)date.DayOfWeek - (int)FirstDayOfWeek + 7) % 7)),
                "PageUp" => date.AddMonths(-1),
                "PageDown" => date.AddMonths(1),
                _ => (DateOnly?)null
            };

            if (args.Key == "Enter" || args.Key == " ")
            {
                await SelectDateAsync(date);
                return;
            }

            if (next is not null)
            {
                VisibleMonth = new DateOnly(next.Value.Year, next.Value.Month, 1);
            }
        }

        private async Task SetValueFromStringAsync(string? value, bool notifyChange, bool close)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                await SetValueAsync(null, notifyChange, close);
                return;
            }

            if (DateOnly.TryParse(value, Culture, DateTimeStyles.None, out var date)
                || DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                await SetValueAsync(NormalizeDate(date), notifyChange, close);
            }
        }

        private async Task SetValueAsync(DateOnly? value, bool notifyChange, bool close)
        {
            CurrentValue = value;
            Value = value;
            InputText = null;

            if (value is not null)
            {
                VisibleMonth = new DateOnly(value.Value.Year, value.Value.Month, 1);
            }

            if (close)
            {
                IsOpen = false;
            }

            if (notifyChange && ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (notifyChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(Value);
            }

            if (notifyChange && CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private DateOnly NormalizeDate(DateOnly value)
        {
            if (!Clamp)
            {
                return value;
            }

            if (Min is not null && value < Min.Value)
            {
                return Min.Value;
            }

            if (Max is not null && value > Max.Value)
            {
                return Max.Value;
            }

            return value;
        }

        private bool IsDateDisabled(DateOnly date)
        {
            if (Min is not null && date < Min.Value)
            {
                return true;
            }

            if (Max is not null && date > Max.Value)
            {
                return true;
            }

            if (DisabledDaysOfWeek is not null && DisabledDaysOfWeek.Contains(date.DayOfWeek))
            {
                return true;
            }

            if (DisabledDates is not null)
            {
                foreach (var disabled in DisabledDates)
                {
                    if (disabled == date)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string FormatDisplayDate(DateOnly? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return value.Value.ToString(DisplayFormat ?? Culture.DateTimeFormat.ShortDatePattern, Culture);
        }

        private string GetDayClass(XFDatePickerDay day)
        {
            var baseClass = "inline-flex h-9 w-9 items-center justify-center rounded-md text-sm outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_26%,transparent)]";
            var monthClass = day.IsSelected ? null : day.IsCurrentMonth ? "text-text-primary" : "text-text-muted";
            var stateClass = day.IsSelected
                ? "bg-primary text-white shadow-[0_1px_2px_rgb(15_23_42/.12)] hover:bg-[color-mix(in_oklab,var(--color-primary)_88%,black)]"
                : day.IsToday
                    ? "text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)]"
                    : "hover:bg-[color-mix(in_oklab,var(--color-primary)_7%,transparent)] hover:text-primary";
            var disabledClass = day.IsDisabled ? "pointer-events-none opacity-35" : "";

            return Join(baseClass, monthClass, stateClass, disabledClass, day.CustomCssClass);
        }

        private string GetDayAriaLabel(XFDatePickerDay day)
        {
            var parts = new List<string> { day.Date.ToString("D", Culture) };
            if (day.IsToday)
            {
                parts.Add(TodayLabel);
            }

            if (day.IsSelected)
            {
                parts.Add(SelectedLabel);
            }

            if (day.IsDisabled)
            {
                parts.Add(DisabledDayLabel);
            }

            return string.Join(", ", parts);
        }

        private int GetIsoWeekNumber(DateOnly date)
        {
            var dt = date.ToDateTime(TimeOnly.MinValue);
            var cal = Culture.Calendar;
            return cal.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, FirstDayOfWeek);
        }

        private async Task NotifyViewChangedAsync()
        {
            if (ViewChanged.HasDelegate)
            {
                await ViewChanged.InvokeAsync(VisibleMonth);
            }
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }

    /// <summary>Context passed to the DatePicker FooterTemplate.</summary>
    public sealed class XFDatePickerFooterContext
    {
        public DateOnly? Value { get; init; }
        public DateOnly Today { get; init; }
        public bool IsTodayDisabled { get; init; }
        public bool IsClearDisabled { get; init; }
        public Func<Task> SelectTodayAsync { get; init; } = default!;
        public Func<Task> ClearAsync { get; init; } = default!;
    }
}
