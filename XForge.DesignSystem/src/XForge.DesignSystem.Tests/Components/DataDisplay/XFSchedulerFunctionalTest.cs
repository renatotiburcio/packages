using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Scheduler;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

public sealed class XFSchedulerFunctionalTest : XForgeTestContext
{
    public XFSchedulerFunctionalTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }
    [Fact] public void SchedulerView_Has3Values() => Assert.Equal(3, Enum.GetValues<XFSchedulerView>().Length);
    [Fact] public void SchedulerView_Month() => Assert.True(Enum.IsDefined(XFSchedulerView.Month));
    [Fact] public void SchedulerView_Week() => Assert.True(Enum.IsDefined(XFSchedulerView.Week));
    [Fact] public void SchedulerView_Day() => Assert.True(Enum.IsDefined(XFSchedulerView.Day));
    [Fact] public void Resource_Created() { var r = new XFSchedulerResource("r1", "Sala A"); Assert.Equal("r1", r.Id); Assert.Equal("Sala A", r.Text); }
    [Fact] public void Resource_Equality() { var a = new XFSchedulerResource("r1", "A"); var b = new XFSchedulerResource("r1", "A"); Assert.Equal(a, b); }
    [Fact] public void Resource_Inequality() { var a = new XFSchedulerResource("r1", "A"); var b = new XFSchedulerResource("r2", "B"); Assert.NotEqual(a, b); }
    private record SchedulerEvent(string Title, DateTime Start, DateTime End);
    [Fact] public void Renders_Default() => Render<XFScheduler<SchedulerEvent>>();
    [Fact] public void Renders_MonthView() => Render<XFScheduler<SchedulerEvent>>(p => { p.Add(x => x.View, XFSchedulerView.Month); });
    [Fact] public void Renders_WeekView() => Render<XFScheduler<SchedulerEvent>>(p => { p.Add(x => x.View, XFSchedulerView.Week); });
    [Fact] public void Renders_DayView() => Render<XFScheduler<SchedulerEvent>>(p => { p.Add(x => x.View, XFSchedulerView.Day); });

    [Fact] public void Renders_WithResources() => Render<XFScheduler<SchedulerEvent>>(p => { p.Add(x => x.Resources, new[] { new XFSchedulerResource("r1", "Room1") }); });
    [Fact] public void Renders_Disabled() => Render<XFScheduler<SchedulerEvent>>(p => { p.Add(x => x.Disabled, true); });
}
