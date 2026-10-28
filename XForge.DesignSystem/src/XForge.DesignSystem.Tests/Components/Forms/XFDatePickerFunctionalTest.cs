using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFDatePickerFunctionalTest : XForgeTestContext
{
    public XFDatePickerFunctionalTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void Renders_Default() => Render<XFDatePicker>();
    [Fact] public void Renders_WithLabel() => Render<XFDatePicker>(p => p.Add(x => x.Label, "Data"));
    [Fact] public void Renders_WithPlaceholder() => Render<XFDatePicker>(p => p.Add(x => x.Placeholder, "Selecione..."));
    [Fact] public void Renders_WithAriaLabel() => Render<XFDatePicker>(p => p.Add(x => x.AriaLabel, "Date picker"));
    [Fact] public void Renders_WithHelperText() => Render<XFDatePicker>(p => p.Add(x => x.HelperText, "Pick a date"));
    [Fact] public void Renders_WithValue() => Render<XFDatePicker>(p => p.Add(x => x.Value, new DateOnly(2026, 6, 26)));
    [Fact] public void Renders_WithMinMax() => Render<XFDatePicker>(p => { p.Add(x => x.Min, new DateOnly(2020, 1, 1)); p.Add(x => x.Max, new DateOnly(2030, 12, 31)); });
    [Fact] public void Culture_PtBR() => Render<XFDatePicker>(p => p.Add(x => x.Culture, new System.Globalization.CultureInfo("pt-BR")));
    [Fact] public void CultureName_PtBR() => Render<XFDatePicker>(p => p.Add(x => x.CultureName, "pt-BR"));
    [Fact] public void Culture_EnUS() => Render<XFDatePicker>(p => p.Add(x => x.Culture, new System.Globalization.CultureInfo("en-US")));
    [Fact] public void DisplayFormat_DdMmYyyy() => Render<XFDatePicker>(p => p.Add(x => x.DisplayFormat, "dd/MM/yyyy"));
    [Fact] public void FirstDayOfWeek_Monday() => Render<XFDatePicker>(p => p.Add(x => x.FirstDayOfWeek, DayOfWeek.Monday));
    [Fact] public void Variant_Filled() => Render<XFDatePicker>(p => p.Add(x => x.Variant, XFFormVariant.Filled));
    [Fact] public void Variant_Soft() => Render<XFDatePicker>(p => p.Add(x => x.Variant, XFFormVariant.Soft));
    [Fact] public void Variant_Ghost() => Render<XFDatePicker>(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
    [Fact] public void Size_Sm() => Render<XFDatePicker>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Size_Lg() => Render<XFDatePicker>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Tone_Primary() => Render<XFDatePicker>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
    [Fact] public void Tone_Danger() => Render<XFDatePicker>(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
    [Fact] public void ReadOnly_True() => Render<XFDatePicker>(p => p.Add(x => x.ReadOnly, true));
    [Fact] public void Required_True() => Render<XFDatePicker>(p => p.Add(x => x.Required, true));
    [Fact] public void FullWidth_False() => Render<XFDatePicker>(p => p.Add(x => x.FullWidth, false));
    [Fact] public void Immediate_True() => Render<XFDatePicker>(p => p.Add(x => x.Immediate, true));
    [Fact] public void Clearable_False() => Render<XFDatePicker>(p => p.Add(x => x.Clearable, false));
    [Fact] public void OpenOnFocus_True() => Render<XFDatePicker>(p => p.Add(x => x.OpenOnFocus, true));
    [Fact] public void Inline_True() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true));
    [Fact] public void ShowWeekNumbers_True() => Render<XFDatePicker>(p => p.Add(x => x.ShowWeekNumbers, true));
    [Fact] public void View_Year() => Render<XFDatePicker>(p => p.Add(x => x.View, XFDatePickerView.Year));
    [Fact] public void View_Decade() => Render<XFDatePicker>(p => p.Add(x => x.View, XFDatePickerView.Decade));
    [Fact] public void YearRange_10() => Render<XFDatePicker>(p => p.Add(x => x.YearRange, 10));
    [Fact] public void PtBR_WithDisplayFormat() => Render<XFDatePicker>(p => { p.Add(x => x.Culture, new System.Globalization.CultureInfo("pt-BR")); p.Add(x => x.DisplayFormat, "dd/MM/yyyy"); });
    [Fact] public void ContainsToggleLabel() { var cut = Render<XFDatePicker>(); Assert.Contains("Abrir calendário", cut.Markup); }
}
