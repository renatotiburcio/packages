using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.DataDisplay.Calendar;
using Xunit;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFCalendarExtendedTest : XForgeTestContext
{
    public XFCalendarExtendedTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    private IRenderedComponent<XFCalendar> RenderDefault(
        Action<ComponentParameterCollectionBuilder<XFCalendar>>? configure = null)
    {
        return Render<XFCalendar>(p =>
        {
            p.Add(x => x.DisplayDate, new DateOnly(2026, 5, 15));
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-calendar", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_WithSectionTag()
    {
        var cut = RenderDefault();
        Assert.Contains("<section", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_AriaLabel()
    {
        var cut = RenderDefault();
        Assert.Contains("aria-label=\"Calendar\"", cut.Markup);
    }

    [Fact]
    public void Extended_CustomAriaLabel_Renders()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.AriaLabel, "My Calendar"));
        Assert.Contains("aria-label=\"My Calendar\"", cut.Markup);
    }

    [Fact]
    public void Extended_VisibleFalse_DoesNotRender()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Visible, false));
        Assert.DoesNotContain("xf-calendar", cut.Markup);
    }

    [Fact]
    public void Extended_MonthView_RendersWeekdayNames()
    {
        var cut = RenderDefault();
        Assert.Contains("grid-cols-7", cut.Markup);
    }

    [Fact]
    public void Extended_MonthView_Renders42DayButtons()
    {
        var cut = RenderDefault();
        var dayButtons = cut.FindAll("div[role=grid] button");
        Assert.Equal(42, dayButtons.Count);
    }

    [Fact]
    public void Extended_WeekView_Renders7Days()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.View, XFCalendarView.Week));
        var dayButtons = cut.FindAll("[role=list] button");
        Assert.Equal(7, dayButtons.Count);
    }

    [Fact]
    public void Extended_DayView_RendersSingleDay()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.View, XFCalendarView.Day));
        Assert.Contains("text-5xl font-black", cut.Markup);
    }

    [Fact]
    public void Extended_SelectDate_FiresValueChanged()
    {
        DateOnly? selected = null;
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => selected = v)));
        cut.Find("[data-xf-calendar-date='2026-05-15']").Click();
        Assert.Equal(new DateOnly(2026, 5, 15), selected);
    }

    [Fact]
    public void Extended_SelectDate_FiresChange()
    {
        DateOnly? changed = null;
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Change, EventCallback.Factory.Create<DateOnly?>(this, v => changed = v)));
        cut.Find("[data-xf-calendar-date='2026-05-15']").Click();
        Assert.Equal(new DateOnly(2026, 5, 15), changed);
    }

    [Fact]
    public void Extended_NextButton_ChangesDisplayDate()
    {
        DateOnly display = default;
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.DisplayDateChanged, EventCallback.Factory.Create<DateOnly>(this, v => display = v)));
        var buttons = cut.FindAll("header button");
        buttons.Last().Click();
        Assert.Equal(new DateOnly(2026, 6, 15), display);
    }

    [Fact]
    public void Extended_PreviousButton_ChangesDisplayDate()
    {
        DateOnly display = default;
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.DisplayDateChanged, EventCallback.Factory.Create<DateOnly>(this, v => display = v)));
        var buttons = cut.FindAll("header button");
        buttons.First().Click();
        Assert.Equal(new DateOnly(2026, 4, 15), display);
    }

    [Fact]
    public void Extended_DisabledDate_DoesNotFireValueChanged()
    {
        DateOnly? selected = null;
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.DisabledDate, (DateOnly date) => date.Day == 15)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => selected = v)));
        cut.Find("[data-xf-calendar-date='2026-05-15']").Click();
        Assert.Null(selected);
    }

    [Fact]
    public void Extended_MinDate_DisablesEarlierDates()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Min, new DateOnly(2026, 5, 10)));
        var day5 = cut.FindAll("div[role=grid] button").First(b => b.GetAttribute("data-xf-calendar-date") == "2026-05-05");
        Assert.True(day5.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_MaxDate_DisablesLaterDates()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Max, new DateOnly(2026, 5, 20)));
        var day25 = cut.FindAll("div[role=grid] button").FirstOrDefault(b => b.GetAttribute("data-xf-calendar-date") == "2026-05-25");
        if (day25 != null)
        {
            Assert.True(day25.HasAttribute("disabled"));
        }
    }

    [Fact]
    public void Extended_Highlights_RendersDots()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Highlights, new[] { new XFCalendarHighlight(new DateOnly(2026, 5, 15), "Event", XFSemanticColor.Success) }));
        Assert.Contains("bg-success", cut.Markup);
    }

    [Fact]
    public void Extended_Highlights_MultipleColors()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Highlights, new[]
            {
                new XFCalendarHighlight(new DateOnly(2026, 5, 15), "Success", XFSemanticColor.Success),
                new XFCalendarHighlight(new DateOnly(2026, 5, 15), "Warning", XFSemanticColor.Warning),
                new XFCalendarHighlight(new DateOnly(2026, 5, 15), "Danger", XFSemanticColor.Danger),
            }));
        Assert.Contains("bg-success", cut.Markup);
        Assert.Contains("bg-warning", cut.Markup);
        Assert.Contains("bg-danger", cut.Markup);
    }

    [Fact]
    public void Extended_DayTemplate_RendersCustomContent()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.DayTemplate, (XFCalendarDayContext ctx) => builder => builder.AddContent(0, $"DAY:{ctx.Date.Day}")));
        Assert.Contains("DAY:15", cut.Markup);
    }

    [Fact]
    public void Extended_SelectedDate_HasAriaPressed()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Value, new DateOnly(2026, 5, 15)));
        var selectedButton = cut.Find("[data-xf-calendar-date='2026-05-15']");
        Assert.Equal("true", selectedButton.GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Extended_Today_HasAriaCurrent()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, today));
        var todayButton = cut.Find($"[data-xf-calendar-date='{today:yyyy-MM-dd}']");
        Assert.Equal("date", todayButton.GetAttribute("aria-current"));
    }

    [Fact]
    public void Extended_SizeSm_RendersSmClass()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Size, XFSize.Sm));
        Assert.Contains("p-2", cut.Markup);
    }

    [Fact]
    public void Extended_SizeLg_RendersLgClass()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Size, XFSize.Lg));
        Assert.Contains("p-5", cut.Markup);
    }

    [Fact]
    public void Extended_VariantSoft_RendersSoftStyles()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Variant, XFContainerVariant.Soft));
        Assert.Contains("xf-calendar", cut.Markup);
    }

    [Fact]
    public void Extended_VariantPlain_RendersPlainStyles()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Variant, XFContainerVariant.Plain));
        Assert.Contains("bg-transparent", cut.Markup);
    }

    [Fact]
    public void Extended_FirstDayOfWeekMonday_RendersMondayFirst()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.FirstDayOfWeek, DayOfWeek.Monday));
        Assert.Contains("xf-calendar", cut.Markup);
    }

    [Fact]
    public void Extended_CustomCulture_RendersLocalized()
    {
        var culture = new System.Globalization.CultureInfo("pt-BR");
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Culture, culture));
        Assert.Contains("xf-calendar", cut.Markup);
    }

    [Fact]
    public void Extended_CustomLabels_RendersLabels()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.PreviousMonthLabel, "Anterior")
            .Add(x => x.NextMonthLabel, "Próximo"));
        Assert.Contains("Anterior", cut.Markup);
        Assert.Contains("Próximo", cut.Markup);
    }

    [Fact]
    public void Extended_DayAriaLabelFormat_CustomFormat()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.DayAriaLabelFormat, "Selecionar {0:yyyy-MM-dd}"));
        Assert.Contains("Selecionar 2026-05-15", cut.Markup);
    }

    [Fact]
    public void Extended_Class_CustomClass_Appended()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.Class, "my-calendar"));
        Assert.Contains("my-calendar", cut.Markup);
    }

    [Fact]
    public void Extended_AdditionalAttributes_Rendered()
    {
        var cut = Render<XFCalendar>(p => p
            .Add(x => x.DisplayDate, new DateOnly(2026, 5, 15))
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-testid", "calendar" } }));
        Assert.Contains("data-testid=\"calendar\"", cut.Markup);
    }

    [Fact]
    public void Extended_Enum_XFCalendarView_HasExpectedValues()
    {
        Assert.Equal(3, Enum.GetValues<XFCalendarView>().Length);
        Assert.Contains(XFCalendarView.Month, Enum.GetValues<XFCalendarView>());
        Assert.Contains(XFCalendarView.Week, Enum.GetValues<XFCalendarView>());
        Assert.Contains(XFCalendarView.Day, Enum.GetValues<XFCalendarView>());
    }

    [Fact]
    public void Extended_XFCalendarDayContext_IsHighlighted_True()
    {
        var ctx = new XFCalendarDayContext(
            new DateOnly(2026, 5, 15), true, false, false, false,
            new[] { new XFCalendarHighlight(new DateOnly(2026, 5, 15), "Test") });
        Assert.True(ctx.IsHighlighted);
    }

    [Fact]
    public void Extended_XFCalendarDayContext_IsHighlighted_False()
    {
        var ctx = new XFCalendarDayContext(
            new DateOnly(2026, 5, 15), true, false, false, false,
            Array.Empty<XFCalendarHighlight>());
        Assert.False(ctx.IsHighlighted);
    }

    // WeekViewClass Sm (line 340)
    [Fact]
    public void Extended_WeekViewSizeSm_RendersSmallWeekView()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.View, XFCalendarView.Week);
            p.Add(x => x.Size, XFSize.Sm);
        });
        Assert.Contains("grid gap-1 sm:grid-cols-7", cut.Markup);
    }

    // WeekViewClass Lg (line 341)
    [Fact]
    public void Extended_WeekViewSizeLg_RendersLargeWeekView()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.View, XFCalendarView.Week);
            p.Add(x => x.Size, XFSize.Lg);
        });
        Assert.Contains("grid gap-3 sm:grid-cols-7", cut.Markup);
    }

    // WeekDayClass Sm (line 366) — Week view day button Sm = "min-h-20 p-2"
    [Fact]
    public void Extended_WeekDaySizeSm_RendersSmallDay()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.View, XFCalendarView.Week);
            p.Add(x => x.Size, XFSize.Sm);
        });
        Assert.Contains("min-h-20 p-2", cut.Markup);
    }

    // WeekDayClass Lg (line 367) — Week view day button Lg = "min-h-36 p-4"
    [Fact]
    public void Extended_WeekDaySizeLg_RendersLargeDay()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.View, XFCalendarView.Week);
            p.Add(x => x.Size, XFSize.Lg);
        });
        Assert.Contains("min-h-36 p-4", cut.Markup);
    }

    // GetHighlightDotClass Success (line 247)
    [Fact]
    public void Extended_HighlightDotSuccess_RendersSuccessDot()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.Highlights, new List<XFCalendarHighlight>
            {
                new(new DateOnly(2026, 5, 15), "Event", XFSemanticColor.Success)
            });
        });
        Assert.Contains("bg-success", cut.Markup);
    }

    // GetHighlightDotClass Warning (line 248)
    [Fact]
    public void Extended_HighlightDotWarning_RendersWarningDot()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.Highlights, new List<XFCalendarHighlight>
            {
                new(new DateOnly(2026, 5, 15), "Event", XFSemanticColor.Warning)
            });
        });
        Assert.Contains("bg-warning", cut.Markup);
    }

    // GetHighlightDotClass Danger (line 249)
    [Fact]
    public void Extended_HighlightDotDanger_RendersDangerDot()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.Highlights, new List<XFCalendarHighlight>
            {
                new(new DateOnly(2026, 5, 15), "Event", XFSemanticColor.Danger)
            });
        });
        Assert.Contains("bg-danger", cut.Markup);
    }

    // GetHighlightDotClass Info (line 250)
    [Fact]
    public void Extended_HighlightDotInfo_RendersInfoDot()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.Highlights, new List<XFCalendarHighlight>
            {
                new(new DateOnly(2026, 5, 15), "Event", XFSemanticColor.Info)
            });
        });
        Assert.Contains("bg-info", cut.Markup);
    }

    // GetHighlightDotClass Secondary (line 251)
    [Fact]
    public void Extended_HighlightDotSecondary_RendersSecondaryDot()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.Highlights, new List<XFCalendarHighlight>
            {
                new(new DateOnly(2026, 5, 15), "Event", XFSemanticColor.Secondary)
            });
        });
        Assert.Contains("bg-text-secondary", cut.Markup);
    }

    // GetHighlightDotClassLg Success (line 257) — used in Day view
    [Fact]
    public void Extended_HighlightDotLgSuccess_RendersLargeSuccessDot()
    {
        var cut = RenderDefault(p =>
        {
            p.Add(x => x.View, XFCalendarView.Day);
            p.Add(x => x.Highlights, new List<XFCalendarHighlight>
            {
                new(new DateOnly(2026, 5, 15), "Event", XFSemanticColor.Success)
            });
            p.Add(x => x.Size, XFSize.Lg);
        });
        Assert.Contains("h-2 w-2 rounded-full bg-success", cut.Markup);
    }
}
