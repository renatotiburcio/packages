using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.TimeInput;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFTimeInputFunctionalTest : BunitContext
{
    public XFTimeInputFunctionalTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void Renders_Default() => Render<XFTimeInput>();
    [Fact] public void Renders_WithLabel() => Render<XFTimeInput>(p => p.Add(x => x.Label, "Hora"));
    [Fact] public void Renders_WithAriaLabel() => Render<XFTimeInput>(p => p.Add(x => x.AriaLabel, "Time input"));
    [Fact] public void Renders_WithHelperText() => Render<XFTimeInput>(p => p.Add(x => x.HelperText, "Pick a time"));
    [Fact] public void Renders_WithValue() => Render<XFTimeInput>(p => p.Add(x => x.Value, new TimeOnly(14, 30)));
    [Fact] public void Renders_WithMinMax() => Render<XFTimeInput>(p => { p.Add(x => x.Min, new TimeOnly(8, 0)); p.Add(x => x.Max, new TimeOnly(18, 0)); });
    [Fact] public void Culture_PtBR() => Render<XFTimeInput>(p => p.Add(x => x.Culture, new System.Globalization.CultureInfo("pt-BR")));
    [Fact] public void CultureName_PtBR() => Render<XFTimeInput>(p => p.Add(x => x.CultureName, "pt-BR"));
    [Fact] public void Precision_Minute() => Render<XFTimeInput>(p => p.Add(x => x.Precision, XFTimeInputPrecision.Minute));
    [Fact] public void Precision_Second() => Render<XFTimeInput>(p => p.Add(x => x.Precision, XFTimeInputPrecision.Second));
    [Fact] public void Step_15() => Render<XFTimeInput>(p => p.Add(x => x.Step, 15));
    [Fact] public void Variant_Filled() => Render<XFTimeInput>(p => p.Add(x => x.Variant, XFFormVariant.Filled));
    [Fact] public void Variant_Soft() => Render<XFTimeInput>(p => p.Add(x => x.Variant, XFFormVariant.Soft));
    [Fact] public void Variant_Ghost() => Render<XFTimeInput>(p => p.Add(x => x.Variant, XFFormVariant.Ghost));
    [Fact] public void Size_Sm() => Render<XFTimeInput>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Size_Lg() => Render<XFTimeInput>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Tone_Primary() => Render<XFTimeInput>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
    [Fact] public void ReadOnly_True() => Render<XFTimeInput>(p => p.Add(x => x.ReadOnly, true));
    [Fact] public void Required_True() => Render<XFTimeInput>(p => p.Add(x => x.Required, true));
    [Fact] public void FullWidth_False() => Render<XFTimeInput>(p => p.Add(x => x.FullWidth, false));
    [Fact] public void Immediate_True() => Render<XFTimeInput>(p => p.Add(x => x.Immediate, true));
    [Fact] public void PrefixText() => Render<XFTimeInput>(p => p.Add(x => x.PrefixText, "Horario:"));
    [Fact] public void SuffixText() => Render<XFTimeInput>(p => p.Add(x => x.SuffixText, "(BRT)"));
}
