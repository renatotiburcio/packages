using Xunit;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFDatePickerDomainFunctionalTest
{
    [Fact] public void DateRenderArgs_Defaults() { var a = new XFDatePickerDateRenderArgs(); Assert.False(a.IsDisabled); Assert.Null(a.CssClass); }
    [Fact] public void DateRenderArgs_CanSetIsDisabled() { var a = new XFDatePickerDateRenderArgs(); a.IsDisabled = true; Assert.True(a.IsDisabled); }
    [Fact] public void DateRenderArgs_CanSetCssClass() { var a = new XFDatePickerDateRenderArgs(); a.CssClass = "holiday"; Assert.Equal("holiday",a.CssClass); }
    [Fact] public void DateRenderArgs_CanClearCssClass() { var a = new XFDatePickerDateRenderArgs(); a.CssClass = "x"; a.CssClass = null; Assert.Null(a.CssClass); }
    [Fact] public void DateRenderArgs_DateInit() { var a = new XFDatePickerDateRenderArgs { Date = new DateOnly(2026,6,26) }; Assert.Equal(new DateOnly(2026,6,26),a.Date); }
    [Fact] public void DateRenderArgs_IsCurrentMonthInit() => Assert.True(new XFDatePickerDateRenderArgs { IsCurrentMonth = true }.IsCurrentMonth);
    [Fact] public void DateRenderArgs_IsSelectedInit() => Assert.True(new XFDatePickerDateRenderArgs { IsSelected = true }.IsSelected);
    [Fact] public void DateRenderArgs_IsTodayInit() => Assert.True(new XFDatePickerDateRenderArgs { IsToday = true }.IsToday);
    [Fact] public void DateRenderArgs_DisableViaCallback() { var a = new XFDatePickerDateRenderArgs(); a.IsDisabled = true; a.CssClass = "disabled-day"; Assert.True(a.IsDisabled); Assert.Equal("disabled-day",a.CssClass); }
    [Fact] public void DateRenderArgs_ToggleDisabled() { var a = new XFDatePickerDateRenderArgs(); a.IsDisabled = true; a.IsDisabled = false; Assert.False(a.IsDisabled); }

    [Fact] public void DatePickerView_Has3Values() => Assert.Equal(3, Enum.GetValues<XFDatePickerView>().Length);
    [Fact] public void DatePickerView_Month() => Assert.True(Enum.IsDefined(XFDatePickerView.Month));
    [Fact] public void DatePickerView_Year() => Assert.True(Enum.IsDefined(XFDatePickerView.Year));
    [Fact] public void DatePickerView_Decade() => Assert.True(Enum.IsDefined(XFDatePickerView.Decade));

    [Fact] public void FooterContext_HasToday() { var c = CreateFooterContext(); Assert.Equal(DateOnly.FromDateTime(DateTime.Today),c.Today); }
    [Fact] public void FooterContext_IsTodayDisabled_DefaultFalse() => Assert.False(CreateFooterContext().IsTodayDisabled);
    [Fact] public void FooterContext_IsClearDisabled_DefaultFalse() => Assert.False(CreateFooterContext().IsClearDisabled);
    [Fact] public void FooterContext_SelectTodayAsync_NotNull() => Assert.NotNull(CreateFooterContext().SelectTodayAsync);
    [Fact] public void FooterContext_ClearAsync_NotNull() => Assert.NotNull(CreateFooterContext().ClearAsync);
    [Fact] public void FooterContext_ValueDefaultNull() => Assert.Null(CreateFooterContext().Value);

    [Fact] public void FormVariant_Has4Values() => Assert.Equal(4, Enum.GetValues<XFFormVariant>().Length);
    [Fact] public void SemanticColor_Has8Values() => Assert.Equal(8, Enum.GetValues<XFSemanticColor>().Length);
    [Fact] public void Size_Has3Values() => Assert.Equal(3, Enum.GetValues<XFSize>().Length);
    [Fact] public void DayOfWeek_Has7Values() => Assert.Equal(7, Enum.GetValues<DayOfWeek>().Length);

    [Fact] public void DateOnly_BRParsing() => Assert.True(DateOnly.TryParse("26/06/2026",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"),System.Globalization.DateTimeStyles.None,out _));
    [Fact] public void DateOnly_USParsing() => Assert.True(DateOnly.TryParse("06/26/2026",System.Globalization.CultureInfo.GetCultureInfo("en-US"),System.Globalization.DateTimeStyles.None,out _));
    [Fact] public void DateOnly_BRCreation() { var d = new DateOnly(2026,6,26); Assert.Equal(26,d.Day); Assert.Equal(6,d.Month); Assert.Equal(2026,d.Year); }
    [Fact] public void DateOnly_FirstDayOfMonth() => Assert.Equal(DayOfWeek.Monday, new DateOnly(2026,6,1).DayOfWeek);
    [Fact] public void DateOnly_AddMonths() { var d = new DateOnly(2026,1,31).AddMonths(1); Assert.Equal(2,d.Month); Assert.Equal(28,d.Day); }

    private static XFDatePickerFooterContext CreateFooterContext() => new() { Today = DateOnly.FromDateTime(DateTime.Today), Value = null, IsTodayDisabled = false, IsClearDisabled = false, SelectTodayAsync = () => Task.CompletedTask, ClearAsync = () => Task.CompletedTask };
}
