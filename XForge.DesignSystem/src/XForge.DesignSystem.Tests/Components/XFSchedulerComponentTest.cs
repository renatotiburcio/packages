using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Scheduler;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFSchedulerComponentTest : XForgeTestContext
{
    public XFSchedulerComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_MonthView_RendersSlotsAndAppointments()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.StartProperty, nameof(Appointment.Start));
            parameters.Add(p => p.EndProperty, nameof(Appointment.End));
            parameters.Add(p => p.TextProperty, nameof(Appointment.Title));
        });

        Assert.Equal(42, cut.FindAll("[data-xf-scheduler-slot]").Count);
        Assert.Contains("Daily ops", cut.Markup);
    }

    [Fact]
    public void Click_Slot_RaisesSlotSelect()
    {
        XFSchedulerSlotSelectEventArgs? selected = null;
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.SlotSelect, args => selected = args);
        });

        cut.Find("[data-xf-scheduler-slot='2026-05-07']").Click();

        Assert.NotNull(selected);
        Assert.Equal(new DateTime(2026, 5, 7), selected!.Start.Date);
    }

    [Fact]
    public void Click_Appointment_RaisesAppointmentSelect()
    {
        XFSchedulerAppointmentSelectEventArgs<Appointment>? selected = null;
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.StartProperty, nameof(Appointment.Start));
            parameters.Add(p => p.EndProperty, nameof(Appointment.End));
            parameters.Add(p => p.TextProperty, nameof(Appointment.Title));
            parameters.Add(p => p.AppointmentSelect, args => selected = args);
        });

        cut.Find("[data-xf-scheduler-appointment='Daily ops']").Click();

        Assert.NotNull(selected);
        Assert.Equal("Daily ops", selected!.Text);
    }

    [Fact]
    public void WeekView_RendersTimelineSlots()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.View, XFSchedulerView.Week);
            parameters.Add(p => p.StartHour, 8);
            parameters.Add(p => p.EndHour, 10);
            parameters.Add(p => p.SlotMinutes, 60);
        });

        Assert.Contains("xf-sched", cut.Markup);
    }

    [Fact]
    public void Navigation_ChangesDate()
    {
        DateTime next = default;
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.DateChanged, value => next = value);
        });

        cut.FindAll("header button").Last(button => button.GetAttribute("aria-label") == "scheduler.next").Click();

        Assert.Equal(new DateTime(2026, 6, 7), next);
    }

    [Fact]
    public void Render_WithCustomLocalization_UsesConfiguredLabels()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.View, XFSchedulerView.Week);
            parameters.Add(p => p.TodayText, "Today");
            parameters.Add(p => p.PreviousPeriodLabel, "Previous period");
            parameters.Add(p => p.NextPeriodLabel, "Next period");
            parameters.Add(p => p.ViewSwitcherLabel, "Calendar views");
            parameters.Add(p => p.HourHeaderText, "Time");
            parameters.Add(p => p.WeekHeaderText, "Week");
            parameters.Add(p => p.MonthViewLabel, "Month");
            parameters.Add(p => p.WeekViewLabel, "Week");
            parameters.Add(p => p.DayViewLabel, "Day");
            parameters.Add(p => p.MultipleAppointmentsFormat, "{0} events");
        });

        Assert.Contains("scheduler.today", cut.Markup);
        Assert.Contains("scheduler.multiple", cut.Markup);
        Assert.Contains("scheduler.previous", cut.Markup);
        Assert.Contains("scheduler.view_switcher", cut.Markup);
        Assert.Contains("scheduler.month", cut.Markup);
        Assert.Contains("scheduler.day", cut.Markup);
    }

    [Fact]
    public void Render_DefaultLocalization_UsesPortugueseAccessibleLabels()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
        });

        Assert.Contains("aria-label=\"scheduler.aria_label\"", cut.Markup);
        Assert.NotNull(cut.Find("[aria-label='scheduler.view_switcher']"));
    }

    [Fact]
    public void Navigation_WhenUncontrolled_RebuildsVisibleSlots()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.View, XFSchedulerView.Day);
            parameters.Add(p => p.StartHour, 8);
            parameters.Add(p => p.EndHour, 9);
        });

        Assert.NotNull(cut.Find("[data-xf-scheduler-slot='2026-05-07T08:00']"));

        cut.FindAll("header button").Last(button => button.GetAttribute("aria-label") == "scheduler.next").Click();

        Assert.NotNull(cut.Find("[data-xf-scheduler-slot='2026-05-08T08:00']"));
    }

    [Fact]
    public void Template_RendersCustomAppointment()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.TextProperty, nameof(Appointment.Title));
            parameters.Add(p => p.Template, (XFSchedulerAppointmentContext<Appointment> item) => builder => builder.AddContent(0, $"{item.Text} custom"));
        });

        Assert.Contains("Daily ops custom", cut.Markup);
    }

    [Fact]
    public void SlotTemplate_RendersCustomSlotContext()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, Appointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.View, XFSchedulerView.Day);
            parameters.Add(p => p.StartHour, 9);
            parameters.Add(p => p.EndHour, 10);
            parameters.Add(p => p.SlotTemplate, (XFSchedulerSlotContext<Appointment> slot) => builder => builder.AddContent(0, $"{slot.Start:HH:mm}:{slot.Appointments.Count}"));
        });

        Assert.Contains("09:00:1", cut.Markup);
    }

    [Fact]
    public void ResourceGrouping_RendersResourceBuckets()
    {
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, ResourceAppointments);
            parameters.Add(p => p.Date, new DateTime(2026, 5, 7));
            parameters.Add(p => p.View, XFSchedulerView.Day);
            parameters.Add(p => p.StartHour, 9);
            parameters.Add(p => p.EndHour, 10);
            parameters.Add(p => p.ResourceProperty, nameof(Appointment.ResourceId));
            parameters.Add(p => p.Resources, Resources);
        });

        Assert.NotNull(cut.Find("[data-xf-scheduler-resource='sala-a']"));
        Assert.NotNull(cut.Find("[data-xf-scheduler-resource='sala-b']"));
        Assert.Contains("Sala A", cut.Markup);
        Assert.Contains("Sala B", cut.Markup);
    }

    [Fact]
    public async Task MoveAndResizeMethods_InvokeEnterpriseCallbacks()
    {
        XFSchedulerAppointmentMoveEventArgs<Appointment>? moved = null;
        XFSchedulerAppointmentResizeEventArgs<Appointment>? resized = null;
        var appointment = ResourceAppointments[0];
        var cut = Render<XFScheduler<Appointment>>(parameters =>
        {
            parameters.Add(p => p.Data, ResourceAppointments);
            parameters.Add(p => p.ResourceProperty, nameof(Appointment.ResourceId));
            parameters.Add(p => p.AppointmentMove, args => moved = args);
            parameters.Add(p => p.AppointmentResize, args => resized = args);
        });

        await cut.Instance.MoveAppointmentAsync(appointment, new DateTime(2026, 5, 7, 11, 0, 0), new DateTime(2026, 5, 7, 12, 0, 0));
        await cut.Instance.ResizeAppointmentAsync(appointment, new DateTime(2026, 5, 7, 9, 0, 0), new DateTime(2026, 5, 7, 11, 30, 0));

        Assert.NotNull(moved);
        Assert.Equal("sala-a", moved!.ResourceId);
        Assert.Equal(new DateTime(2026, 5, 7, 11, 0, 0), moved.NewStart);
        Assert.NotNull(resized);
        Assert.Equal(new DateTime(2026, 5, 7, 11, 30, 0), resized!.NewEnd);
    }

    private static readonly IReadOnlyList<Appointment> Appointments =
    [
        new("Daily ops", new DateTime(2026, 5, 7, 9, 0, 0), new DateTime(2026, 5, 7, 10, 0, 0)),
        new("Migration", new DateTime(2026, 5, 10, 8, 0, 0), new DateTime(2026, 5, 12, 18, 0, 0))
    ];

    private static readonly IReadOnlyList<Appointment> ResourceAppointments =
    [
        new("Daily ops", new DateTime(2026, 5, 7, 9, 0, 0), new DateTime(2026, 5, 7, 10, 0, 0), "sala-a"),
        new("Planning", new DateTime(2026, 5, 7, 9, 0, 0), new DateTime(2026, 5, 7, 10, 0, 0), "sala-b")
    ];

    private static readonly IReadOnlyList<XFSchedulerResource> Resources =
    [
        new("sala-a", "Sala A"),
        new("sala-b", "Sala B")
    ];

    private sealed record Appointment(string Title, DateTime Start, DateTime End, string? ResourceId = null);
}
