using Bunit;
using Bunit.Extensions;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFDatePickerInteractionTest : BunitContext
{
    public XFDatePickerInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void ClickToggleButton_OpensCalendar() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Label, "Data")); var toggleBtn = cut.Find("[aria-label=\"Abrir calendario\"]"); toggleBtn.Click(); Assert.NotNull(cut.Find("[role=\"grid\"]")); }
    [Fact] public void InlineMode_ShowsCalendarByDefault() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Inline, true)); Assert.NotNull(cut.Find("[role=\"grid\"]")); }
    [Fact] public void InlineMode_WithValue_ShowsCalendar() { var cut = Render<XFDatePicker>(p => { p.Add(x => x.Inline, true); p.Add(x => x.Value, new DateOnly(2026, 6, 27)); }); Assert.NotNull(cut.Find("[role=\"grid\"]")); }
    [Fact] public void InlineMode_CanNavigateMonth() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Inline, true)); cut.Find("[aria-label=\"Mes anterior\"]").Click(); cut.Find("[aria-label=\"Proximo mes\"]").Click(); Assert.NotNull(cut.Find("[role=\"grid\"]")); }
    [Fact] public void Clearable_WithValue_ShowsClearOption() { var cut = Render<XFDatePicker>(p => { p.Add(x => x.Clearable, true); p.Add(x => x.Inline, true); p.Add(x => x.Value, new DateOnly(2026, 6, 27)); }); Assert.Contains("Limpar", cut.Markup); }
    [Fact] public void Disabled_PreventsToggle() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Disabled, true)); var toggleBtn = cut.Find("[aria-label=\"Abrir calendario\"]"); Assert.True(toggleBtn.GetAttribute("disabled") != null); }
    [Fact] public void ReadOnly_PreventsToggle() { var cut = Render<XFDatePicker>(p => p.Add(x => x.ReadOnly, true)); var toggleBtn = cut.Find("[aria-label=\"Abrir calendario\"]"); Assert.True(toggleBtn.GetAttribute("disabled") != null); }
    [Fact] public void InlineMode_SelectDay_FiresValueChange() { DateOnly? selected = null; var cut = Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => selected = v))); var dayButtons = cut.FindAll("[role=\"gridcell\"] button"); if (dayButtons.Count > 0) { dayButtons[0].Click(); Assert.NotNull(selected); } }
    [Fact] public void InlineMode_WeekNumbers_Visible() { var cut = Render<XFDatePicker>(p => { p.Add(x => x.Inline, true); p.Add(x => x.ShowWeekNumbers, true); }); Assert.Contains("Semana", cut.Markup); }
    [Fact] public void InlineMode_FirstDayOfWeekMonday() { var cut = Render<XFDatePicker>(p => { p.Add(x => x.Inline, true); p.Add(x => x.FirstDayOfWeek, DayOfWeek.Monday); }); Assert.NotNull(cut.Find("[role=\"grid\"]")); }
    [Fact] public void InlineMode_WithMinMax_Renders() { var cut = Render<XFDatePicker>(p => { p.Add(x => x.Inline, true); p.Add(x => x.Min, new DateOnly(2026, 6, 1)); p.Add(x => x.Max, new DateOnly(2026, 6, 30)); }); Assert.NotNull(cut.Find("[role=\"grid\"]")); }
    [Fact] public void InlineMode_TodayLabel_Visible() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Inline, true)); Assert.Contains("Hoje", cut.Markup); }
    [Fact] public void InlineMode_SelectedLabel_Visible() { var cut = Render<XFDatePicker>(p => { p.Add(x => x.Inline, true); p.Add(x => x.Value, new DateOnly(2026, 6, 15)); }); Assert.Contains("selecionado", cut.Markup); }
}
