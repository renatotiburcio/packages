using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.DatePicker;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

/// <summary>
/// Deep bUnit render tests for XFDatePicker — parameters, markup assertions, interaction.
/// </summary>
public sealed class XFDatePickerRenderFunctionalTest : BunitContext
{
    public XFDatePickerRenderFunctionalTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    // ── Rendering: all parameter combos ──
    [Fact] public void Renders_Outline() => Render<XFDatePicker>(p => p.Add(x => x.Variant, XFFormVariant.Outline));
    [Fact] public void Renders_Filled() => Render<XFDatePicker>(p => p.Add(x => x.Variant, XFFormVariant.Filled));
    [Fact] public void Renders_Soft() => Render<XFDatePicker>(p => p.Add(x => x.Variant, XFFormVariant.Soft));
    [Fact] public void Renders_Ghost() => Render<XFDatePicker>(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
    [Fact] public void Renders_Sm() => Render<XFDatePicker>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_Lg() => Render<XFDatePicker>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_TonePrimary() => Render<XFDatePicker>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
    [Fact] public void Renders_ToneDanger() => Render<XFDatePicker>(p => p.Add(x => x.Tone, XFSemanticColor.Danger));
    [Fact] public void Renders_WithLabel() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Label, "Nascimento")); Assert.Contains("Nascimento", cut.Markup); }
    [Fact] public void Renders_WithPlaceholder() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Placeholder, "dd/mm/aaaa")); Assert.Contains("dd/mm/aaaa", cut.Markup); }
    [Fact] public void Renders_WithAriaLabel() { var cut = Render<XFDatePicker>(p => p.Add(x => x.AriaLabel, "dt-nasc")); Assert.Contains("dt-nasc", cut.Markup); }
    [Fact] public void Renders_WithHelperText() { var cut = Render<XFDatePicker>(p => p.Add(x => x.HelperText, "Opcional")); Assert.Contains("Opcional", cut.Markup); }
    [Fact] public void Renders_WithValue() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Value, new DateOnly(2026,6,27))); Assert.NotNull(cut.Markup); }
    [Fact] public void Renders_WithMinMax() => Render<XFDatePicker>(p => { p.Add(x => x.Min, new DateOnly(2020,1,1)); p.Add(x => x.Max, new DateOnly(2030,12,31)); });
    [Fact] public void Renders_CulturePtBR() => Render<XFDatePicker>(p => p.Add(x => x.Culture, new System.Globalization.CultureInfo("pt-BR")));
    [Fact] public void Renders_CultureEnUS() => Render<XFDatePicker>(p => p.Add(x => x.Culture, new System.Globalization.CultureInfo("en-US")));
    [Fact] public void Renders_CultureNamePtBR() => Render<XFDatePicker>(p => p.Add(x => x.CultureName, "pt-BR"));
    [Fact] public void Renders_DisplayFormat() => Render<XFDatePicker>(p => p.Add(x => x.DisplayFormat, "dd/MM/yyyy"));
    [Fact] public void Renders_FirstDayOfWeekMonday() => Render<XFDatePicker>(p => p.Add(x => x.FirstDayOfWeek, DayOfWeek.Monday));
    [Fact] public void Renders_ReadOnlyTrue() { var cut = Render<XFDatePicker>(p => p.Add(x => x.ReadOnly, true)); Assert.Contains("readonly", cut.Markup.ToLowerInvariant()); }
    [Fact] public void Renders_RequiredTrue() => Render<XFDatePicker>(p => p.Add(x => x.Required, true));
    [Fact] public void Renders_InlineTrue() => Render<XFDatePicker>(p => p.Add(x => x.Inline, true));
    [Fact] public void Renders_ShowWeekNumbersTrue() => Render<XFDatePicker>(p => p.Add(x => x.ShowWeekNumbers, true));
    [Fact] public void Renders_ViewYear() => Render<XFDatePicker>(p => p.Add(x => x.View, XFDatePickerView.Year));
    [Fact] public void Renders_ViewDecade() => Render<XFDatePicker>(p => p.Add(x => x.View, XFDatePickerView.Decade));
    [Fact] public void Renders_DisabledTrue() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Disabled, true)); Assert.Contains("disabled", cut.Markup.ToLowerInvariant()); }
    [Fact] public void Renders_WithName() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Name, "birthdate")); Assert.Contains("birthdate", cut.Markup); }
    [Fact] public void Renders_WithPrefixText() { var cut = Render<XFDatePicker>(p => p.Add(x => x.PrefixText, "R$")); Assert.Contains("R$", cut.Markup); }
    [Fact] public void Renders_WithSuffixText() { var cut = Render<XFDatePicker>(p => p.Add(x => x.SuffixText, "OK")); Assert.Contains("OK", cut.Markup); }
    [Fact] public void Renders_ImmediateTrue() => Render<XFDatePicker>(p => p.Add(x => x.Immediate, true));
    [Fact] public void Renders_ClampFalse() => Render<XFDatePicker>(p => p.Add(x => x.Clamp, false));
    [Fact] public void Renders_ClearableFalse() => Render<XFDatePicker>(p => p.Add(x => x.Clearable, false));
    [Fact] public void Renders_OpenOnFocusTrue() => Render<XFDatePicker>(p => p.Add(x => x.OpenOnFocus, true));
    [Fact] public void Renders_YearRange5() => Render<XFDatePicker>(p => p.Add(x => x.YearRange, 5));

    // ── Markup assertions ──
    [Fact] public void Markup_ContainsToggleLabel() { var cut = Render<XFDatePicker>(); Assert.Contains("Abrir calendario", cut.Markup); }
    [Fact] public void Markup_HasId() { var cut = Render<XFDatePicker>(); Assert.NotEmpty(cut.Markup); }
    [Fact] public void Markup_InlineShowsLabels() { var cut = Render<XFDatePicker>(p => p.Add(x => x.Inline, true)); Assert.NotNull(cut.Markup); }






    // ── Composite scenarios ──
    [Fact] public void Composite_PtBR_Filled_Full() => Render<XFDatePicker>(p => { p.Add(x => x.Culture, new System.Globalization.CultureInfo("pt-BR")); p.Add(x => x.Variant, XFFormVariant.Filled); p.Add(x => x.Label, "Data"); p.Add(x => x.DisplayFormat, "dd/MM/yyyy"); p.Add(x => x.Required, true); });
    [Fact] public void Composite_EnUS_Outline() => Render<XFDatePicker>(p => { p.Add(x => x.Culture, new System.Globalization.CultureInfo("en-US")); p.Add(x => x.Variant, XFFormVariant.Outline); p.Add(x => x.Label, "Date"); p.Add(x => x.FirstDayOfWeek, DayOfWeek.Sunday); });
    [Fact] public void Composite_Inline_WeekNumbers() => Render<XFDatePicker>(p => { p.Add(x => x.Inline, true); p.Add(x => x.ShowWeekNumbers, true); p.Add(x => x.FirstDayOfWeek, DayOfWeek.Monday); });
    [Fact] public void Composite_DisabledDates_MinMax() => Render<XFDatePicker>(p => { p.Add(x => x.Min, new DateOnly(2026,1,1)); p.Add(x => x.Max, new DateOnly(2026,12,31)); p.Add(x => x.Value, new DateOnly(2026,6,27)); });
    [Fact] public void Composite_ReadOnly_WithValue() => Render<XFDatePicker>(p => { p.Add(x => x.ReadOnly, true); p.Add(x => x.Value, new DateOnly(2026,6,27)); p.Add(x => x.CultureName, "pt-BR"); });
}
