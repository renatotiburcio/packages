using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Forms.InputMask;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFInputMaskBRTest : BunitContext
{
    public XFInputMaskBRTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    // CPF: 000.000.000-00
    [Fact] public void CPF_Mask_Renders() => Render<XFInputMask>(p => p.Add(x => x.Mask, "000.000.000-00"));
    [Fact] public void CPF_WithLabel() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.Label, "CPF"); });
    [Fact] public void CPF_WithPlaceholder() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.Placeholder, "000.000.000-00"); });
    [Fact] public void CPF_Required() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.Required, true); });

    // CNPJ: 00.000.000/0000-00
    [Fact] public void CNPJ_Mask_Renders() => Render<XFInputMask>(p => p.Add(x => x.Mask, "00.000.000/0000-00"));
    [Fact] public void CNPJ_WithLabel() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "00.000.000/0000-00"); p.Add(x => x.Label, "CNPJ"); });
    [Fact] public void CNPJ_Required() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "00.000.000/0000-00"); p.Add(x => x.Required, true); });

    // CEP: 00000-000
    [Fact] public void CEP_Mask_Renders() => Render<XFInputMask>(p => p.Add(x => x.Mask, "00000-000"));
    [Fact] public void CEP_WithLabel() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "00000-000"); p.Add(x => x.Label, "CEP"); });
    [Fact] public void CEP_WithPlaceholder() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "00000-000"); p.Add(x => x.Placeholder, "00000-000"); });

    // Phone: (00) 00000-0000
    [Fact] public void Phone_Mask_Renders() => Render<XFInputMask>(p => p.Add(x => x.Mask, "(00) 00000-0000"));
    [Fact] public void Phone_WithLabel() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "(00) 00000-0000"); p.Add(x => x.Label, "Telefone"); });
    [Fact] public void Phone_Landline() => Render<XFInputMask>(p => p.Add(x => x.Mask, "(00) 0000-0000"));

    // General mask features
    [Fact] public void UnmaskValue_True() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.UnmaskValue, true); });
    [Fact] public void UnmaskValue_False() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.UnmaskValue, false); });
    [Fact] public void ClearIncomplete_True() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.ClearIncomplete, true); });
    [Fact] public void Clearable_True() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.Clearable, true); });
    [Fact] public void ShowMaskPlaceholder_True() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.ShowMaskPlaceholder, true); });
    [Fact] public void Variant_Filled() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.Variant, XFFormVariant.Filled); });
    [Fact] public void Size_Sm() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.Size, XFSize.Sm); });
    [Fact] public void ReadOnly_True() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.ReadOnly, true); });
    [Fact] public void Immediate_True() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.Immediate, true); });
    [Fact] public void HelperText() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.HelperText, "Somente numeros"); });
    [Fact] public void MaskDescriptionFormat() => Render<XFInputMask>(p => { p.Add(x => x.Mask, "000.000.000-00"); p.Add(x => x.MaskDescriptionFormat, "Formato: {0}"); });

    // PIS/PASEP: 000.00000.00-0
    [Fact] public void PIS_Mask_Renders() => Render<XFInputMask>(p => p.Add(x => x.Mask, "000.00000.00-0"));

    // RG (SP): 00.000.000-0
    [Fact] public void RG_Mask_Renders() => Render<XFInputMask>(p => p.Add(x => x.Mask, "00.000.000-0"));

    // Renavam: 0000000000-0
    [Fact] public void Renavam_Mask_Renders() => Render<XFInputMask>(p => p.Add(x => x.Mask, "0000000000-0"));

    // Placa veicular: AAA-0000 (old) / AAA0A00 (Mercosul)
    [Fact] public void Placa_Old() => Render<XFInputMask>(p => p.Add(x => x.Mask, "AAA-0000"));
}
