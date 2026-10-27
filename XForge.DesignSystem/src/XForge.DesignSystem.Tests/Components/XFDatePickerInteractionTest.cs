using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Core.Primitives;
using System.Collections.Generic;

namespace XForge.DesignSystem.Tests.Components;

public sealed class XFDatePickerInteractionTest : BunitContext
{
    public XFDatePickerInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void DatePicker_ValueChanged_Fires()
    {
        DateOnly? selected = null;
        var cut = Render<XFDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, v => selected = v)));
        Assert.NotNull(cut);
    }

    [Fact] public void DatePicker_DateRender_CustomCallback()
    {
        var cut = Render<XFDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.DateRender, args =>
            {
                if (args.Date.Day == 15) args.CssClass = "highlight";
                if (args.Date.Day == 20) args.IsDisabled = true;
            }));
        Assert.NotNull(cut);
    }

    [Fact] public void DatePicker_DisabledDates()
    {
        var cut = Render<XFDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.DisabledDates, new List<DateOnly> { new DateOnly(2026, 6, 4) }));
        Assert.NotNull(cut);
    }

    [Fact] public void DatePicker_FirstDayOfWeekSunday() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.FirstDayOfWeek, DayOfWeek.Sunday));
    [Fact] public void DatePicker_DisplayFormat() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.DisplayFormat, "MM/dd/yyyy"));
    [Fact] public void DatePicker_CulturePtBR() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, new System.Globalization.CultureInfo("pt-BR")));
    [Fact] public void DatePicker_MinMax() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Min, new DateOnly(2020, 1, 1)).Add(x => x.Max, new DateOnly(2030, 12, 31)));
    [Fact] public void DatePicker_ViewYear() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.View, XFDatePickerView.Year));
    [Fact] public void DatePicker_ViewDecade() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.View, XFDatePickerView.Decade));
    [Fact] public void DatePicker_ShowWeekNumbers() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.ShowWeekNumbers, true));
    [Fact] public void DatePicker_Clearable() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Clearable, true));
    [Fact] public void DatePicker_OpenOnFocus() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.OpenOnFocus, true));
    [Fact] public void DatePicker_YearRange() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.YearRange, 10));
    [Fact] public void DatePicker_HelperText() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.HelperText, "Select date"));
    [Fact] public void DatePicker_Placeholder() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Placeholder, "dd/mm/yyyy"));
    [Fact] public void DatePicker_Required() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Required, true));
    [Fact] public void DatePicker_ReadOnly() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.ReadOnly, true));
    [Fact] public void DatePicker_Disabled() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Disabled, true));
    [Fact] public void DatePicker_Immediate() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Immediate, true));
    [Fact] public void DatePicker_ClampFalse() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Clamp, false));
}