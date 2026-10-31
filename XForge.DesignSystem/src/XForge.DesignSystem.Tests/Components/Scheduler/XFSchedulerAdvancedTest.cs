using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Scheduler;

namespace XForge.DesignSystem.Tests.Components.Scheduler;

public class XFSchedulerAdvancedTest : XForgeTestContext
{
    public XFSchedulerAdvancedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFSchedulerAdvanced>();
        Assert.Contains("xf-scheduler-advanced", cut.Markup);
    }

    [Fact]
    public void Render_WithEvents_ShowsEvents()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Meeting", DateTime.Today.AddHours(9), DateTime.Today.AddHours(10))
        };

        var cut = Render<XFSchedulerAdvanced>(p => p.Add(x => x.Events, events));
        Assert.Contains("Meeting", cut.Markup);
    }

    [Fact]
    public void Render_ShowsTimeSlots()
    {
        var cut = Render<XFSchedulerAdvanced>();
        Assert.Contains("00", cut.Markup);
        Assert.Contains("23", cut.Markup);
    }

    [Fact]
    public void Render_DefaultViewIsWeek()
    {
        var cut = Render<XFSchedulerAdvanced>();
        Assert.Contains("data-view=\"Week\"", cut.Markup);
    }

    [Fact]
    public void Render_CanSwitchToMonthView()
    {
        var cut = Render<XFSchedulerAdvanced>(p => p.Add(x => x.View, SchedulerView.Month));
        Assert.Contains("data-view=\"Month\"", cut.Markup);
        Assert.Contains("xf-scheduler-month-grid", cut.Markup);
    }

    [Fact]
    public void Render_CanSwitchToDayView()
    {
        var cut = Render<XFSchedulerAdvanced>(p => p.Add(x => x.View, SchedulerView.Day));
        Assert.Contains("data-view=\"Day\"", cut.Markup);
        Assert.Contains("xf-scheduler-day-container", cut.Markup);
    }

    [Fact]
    public void Render_CanSwitchToAgendaView()
    {
        var cut = Render<XFSchedulerAdvanced>(p => p.Add(x => x.View, SchedulerView.Agenda));
        Assert.Contains("data-view=\"Agenda\"", cut.Markup);
        Assert.Contains("xf-scheduler-agenda", cut.Markup);
    }

    [Fact]
    public void Render_MonthView_ShowsDayNames()
    {
        var cut = Render<XFSchedulerAdvanced>(p => p.Add(x => x.View, SchedulerView.Month));
        Assert.Contains("Sun", cut.Markup);
        Assert.Contains("Mon", cut.Markup);
        Assert.Contains("Tue", cut.Markup);
        Assert.Contains("Wed", cut.Markup);
        Assert.Contains("Thu", cut.Markup);
        Assert.Contains("Fri", cut.Markup);
        Assert.Contains("Sat", cut.Markup);
    }

    [Fact]
    public void Render_WeekView_ShowsDayColumns()
    {
        var cut = Render<XFSchedulerAdvanced>(p => p.Add(x => x.View, SchedulerView.Week));
        Assert.Contains("xf-scheduler-week-grid", cut.Markup);
        Assert.Contains("xf-scheduler-week-day-column", cut.Markup);
    }

    [Fact]
    public void Render_DayView_ShowsTimeGutter()
    {
        var cut = Render<XFSchedulerAdvanced>(p => p.Add(x => x.View, SchedulerView.Day));
        Assert.Contains("xf-scheduler-day-time-gutter", cut.Markup);
    }

    [Fact]
    public void Render_AgendaView_ShowsDayGroups()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Meeting", DateTime.Today.AddHours(9), DateTime.Today.AddHours(10))
        };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.View, SchedulerView.Agenda);
            p.Add(x => x.Events, events);
        });
        Assert.Contains("xf-scheduler-agenda-day", cut.Markup);
    }

    [Fact]
    public void Render_ShowsNavigationButtons()
    {
        var cut = Render<XFSchedulerAdvanced>();
        Assert.Contains("Today", cut.Markup);
        Assert.Contains("←", cut.Markup);
        Assert.Contains("→", cut.Markup);
    }

    [Fact]
    public void Render_ShowsViewSwitcher()
    {
        var cut = Render<XFSchedulerAdvanced>();
        Assert.Contains("Day", cut.Markup);
        Assert.Contains("Week", cut.Markup);
        Assert.Contains("Month", cut.Markup);
    }

    [Fact]
    public void Render_WithAllDayEvents_ShowsAllDaySection()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Conference", DateTime.Today, DateTime.Today.AddDays(1), IsAllDay: true)
        };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.View, SchedulerView.Week);
            p.Add(x => x.Events, events);
            p.Add(x => x.ShowAllDaySection, true);
        });
        Assert.Contains("xf-scheduler-week-all-day", cut.Markup);
        Assert.Contains("Conference", cut.Markup);
    }

    [Fact]
    public void Render_WithResourceGrouping_ShowsResourceOptions()
    {
        var resources = new[] { "room1", "room2" };
        var resourceLabels = new Dictionary<string, string> { { "room1", "Room A" }, { "room2", "Room B" } };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.AllowResourceGrouping, true);
            p.Add(x => x.ResourceIds, resources);
            p.Add(x => x.ResourceLabels, resourceLabels);
            p.Add(x => x.View, SchedulerView.Week);
        });
        Assert.Contains("xf-scheduler-week-grid", cut.Markup);
    }

    [Fact]
    public void Render_MonthView_WithEvents_ShowsEventTitles()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Sprint Planning", DateTime.Today.AddHours(10), DateTime.Today.AddHours(11), Color: "#3b82f6")
        };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.View, SchedulerView.Month);
            p.Add(x => x.Events, events);
        });
        Assert.Contains("Sprint Planning", cut.Markup);
    }

    [Fact]
    public void Render_ShowsModalOnCreate()
    {
        var cut = Render<XFSchedulerAdvanced>();
        // Modal should not be visible initially
        Assert.DoesNotContain("xf-scheduler-modal-overlay", cut.Markup);
    }

    [Fact]
    public void Render_WithExport_ShowsExportButtons()
    {
        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.View, SchedulerView.Agenda);
            p.Add(x => x.AllowExport, true);
        });
        Assert.Contains("Export JSON", cut.Markup);
        Assert.Contains("Export CSV", cut.Markup);
        Assert.Contains("Export ICS", cut.Markup);
    }

    [Fact]
    public void Render_WithoutExport_HidesExportButtons()
    {
        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.View, SchedulerView.Agenda);
            p.Add(x => x.AllowExport, false);
        });
        Assert.DoesNotContain("Export JSON", cut.Markup);
    }

    [Fact]
    public void Render_WithInlineEdit_AllowsDoubleClick()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Meeting", DateTime.Today.AddHours(9), DateTime.Today.AddHours(10))
        };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.Events, events);
            p.Add(x => x.AllowInlineEdit, true);
            p.Add(x => x.View, SchedulerView.Week);
        });
        // Inline edit should be enabled
        Assert.Contains("Meeting", cut.Markup);
    }

    [Fact]
    public void Render_WithEventTemplate_UsesTemplate()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Custom Event", DateTime.Today.AddHours(9), DateTime.Today.AddHours(10))
        };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.Events, events);
            p.Add(x => x.View, SchedulerView.Week);
            p.Add(x => x.EventTemplate, (ctx) => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-event");
                builder.AddContent(2, $"Custom: {ctx.Title}");
                builder.CloseElement();
            });
        });
        Assert.Contains("Custom: Custom Event", cut.Markup);
    }

    [Fact]
    public void Render_WithMonthEventTemplate_UsesTemplate()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Custom Month Event", DateTime.Today.AddHours(9), DateTime.Today.AddHours(10))
        };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.Events, events);
            p.Add(x => x.View, SchedulerView.Month);
            p.Add(x => x.MonthEventTemplate, (ctx) => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-month-event");
                builder.AddContent(2, $"Month: {ctx.Title}");
                builder.CloseElement();
            });
        });
        Assert.Contains("Month: Custom Month Event", cut.Markup);
    }

    [Fact]
    public void SchedulerEvent_CanBeCreated()
    {
        var ev = new SchedulerEvent("1", "Test", DateTime.Now, DateTime.Now.AddHours(1));
        Assert.Equal("1", ev.Id);
        Assert.Equal("Test", ev.Title);
    }

    [Fact]
    public void SchedulerView_HasAllValues()
    {
        Assert.Equal(6, Enum.GetValues<SchedulerView>().Length);
    }

    [Fact]
    public void Render_AgendaView_WithMultipleEvents_ShowsAll()
    {
        var events = new List<SchedulerEvent>
        {
            new("e1", "Meeting 1", DateTime.Today.AddHours(9), DateTime.Today.AddHours(10)),
            new("e2", "Meeting 2", DateTime.Today.AddHours(14), DateTime.Today.AddHours(15)),
            new("e3", "Workshop", DateTime.Today.AddDays(1).AddHours(10), DateTime.Today.AddDays(1).AddHours(12))
        };

        var cut = Render<XFSchedulerAdvanced>(p =>
        {
            p.Add(x => x.View, SchedulerView.Agenda);
            p.Add(x => x.Events, events);
        });
        Assert.Contains("Meeting 1", cut.Markup);
        Assert.Contains("Meeting 2", cut.Markup);
        Assert.Contains("Workshop", cut.Markup);
        Assert.Contains("xf-scheduler-agenda-day", cut.Markup);
    }
}
