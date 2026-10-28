using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Calendar;

namespace XForge.DesignSystem.Tests.Components;

public class XFCalendarComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_MonthView_RendersSixWeekGrid()
    {
        var cut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
        });

        Assert.Equal(42, cut.FindAll("div[role=grid] button").Count);
        Assert.Contains("2026", cut.Markup);
    }

    [Fact]
    public void Click_Day_ChangesValue()
    {
        DateOnly? selected = null;
        var cut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
            parameters.Add(p => p.ValueChanged, value => selected = value);
        });

        cut.Find("[data-xf-calendar-date='2026-05-07']").Click();

        Assert.Equal(new DateOnly(2026, 5, 7), selected);
    }

    [Fact]
    public void DisabledDate_DoesNotChangeValue()
    {
        DateOnly? selected = null;
        var cut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
            parameters.Add(p => p.DisabledDate, (DateOnly date) => date.Day == 7);
            parameters.Add(p => p.ValueChanged, value => selected = value);
        });

        cut.Find("[data-xf-calendar-date='2026-05-07']").Click();

        Assert.Null(selected);
    }

    [Fact]
    public void Navigation_ChangesDisplayMonth()
    {
        DateOnly display = default;
        var cut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
            parameters.Add(p => p.DisplayDateChanged, value => display = value);
        });

        cut.FindAll("header button").Last().Click();

        Assert.Equal(new DateOnly(2026, 6, 7), display);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredLabels()
    {
        var cut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
            parameters.Add(p => p.MonthViewLabel, "Vista mensal");
            parameters.Add(p => p.PreviousMonthLabel, "Mes previo");
            parameters.Add(p => p.NextMonthLabel, "Mes seguinte");
        });

        Assert.Contains("Vista mensal", cut.Markup);
        Assert.Contains("aria-label=\"Mes previo\"", cut.Markup);
        Assert.Contains("aria-label=\"Mes seguinte\"", cut.Markup);
    }

    [Fact]
    public void Render_DefaultPortugueseLabelsAndCustomDayAriaFormat()
    {
        var defaultCut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
        });
        var customCut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
            parameters.Add(p => p.DayAriaLabelFormat, "Selecionar {0:yyyy-MM-dd}");
        });

        Assert.Contains("aria-label=\"Calendar\"", defaultCut.Markup);
        Assert.Contains("Monthly view", defaultCut.Markup);
        Assert.Contains("aria-label=\"Selecionar 2026-05-07\"", customCut.Markup);
    }

    [Fact]
    public void WeekView_RendersSevenDays()
    {
        var cut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.View, XFCalendarView.Week);
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
        });

        Assert.Equal(7, cut.FindAll("[role=list] button").Count);
    }

    [Fact]
    public void DayTemplate_RendersCustomContent()
    {
        var cut = Render<XFCalendar>(parameters =>
        {
            parameters.Add(p => p.DisplayDate, new DateOnly(2026, 5, 7));
            parameters.Add(p => p.Highlights, new[] { new XFCalendarHighlight(new DateOnly(2026, 5, 7), "Test event") });
            parameters.Add(p => p.DayTemplate, (XFCalendarDayContext day) => builder =>
            {
                if (day.IsHighlighted)
                {
                    builder.AddContent(0, "event");
                }
                else
                {
                    builder.AddContent(1, day.Date.Day);
                }
            });
        });

        Assert.Contains("event", cut.Markup);
    }
}
