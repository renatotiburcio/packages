using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.TextInput;
using XForge.DesignSystem.Components.Forms.Textarea;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.DesignSystem.Components.Forms.Checkbox;
using XForge.DesignSystem.Components.Forms.Radio;
using XForge.DesignSystem.Components.Forms.Select;
using XForge.DesignSystem.Components.Forms.NumberInput;
using XForge.DesignSystem.Components.Forms.InputMask;
using XForge.DesignSystem.Components.Forms.TimeInput;
using XForge.DesignSystem.Components.Forms.TimeSpan;
using XForge.DesignSystem.Components.Forms.Upload;
using XForge.DesignSystem.Components.Forms.OneTimeInput;
using XForge.DesignSystem.Components.Forms.TransferList;
using XForge.DesignSystem.Components.Forms.SignaturePad;
using XForge.DesignSystem.Components.Forms.Field;
using XForge.DesignSystem.Components.Forms.Search;
using XForge.DesignSystem.Components.Forms.RangeSlider;
using XForge.DesignSystem.Components.Forms.Rating;
using XForge.DesignSystem.Components.Forms.MultiSelect;
using XForge.DesignSystem.Components.Forms.Combobox;
using XForge.DesignSystem.Components.Forms.FilterForm;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components;

public sealed class XFFormComponentsTest : XForgeTestContext
{
    public XFFormComponentsTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void TextInput_Renders() => Render<XFTextInput>();
    [Fact] public void TextInput_Label() => Assert.Contains("Name", Render<XFTextInput>(p => p.Add(x => x.Label, "Name")).Markup);
    [Fact] public void TextInput_Value() => Render<XFTextInput>(p => p.Add(x => x.Value, "test"));
    [Fact] public void TextInput_ReadOnly() => Render<XFTextInput>(p => p.Add(x => x.ReadOnly, true));
    [Fact] public void Textarea_Renders() => Render<XFTextarea>();
    [Fact] public void Textarea_ReadOnly() => Render<XFTextarea>(p => p.Add(x => x.ReadOnly, true));
    [Fact] public void Switch_Renders() => Render<XFSwitch>();
    [Fact] public void Switch_Value() => Render<XFSwitch>(p => p.Add(x => x.Value, true));
    [Fact] public void CheckBox_Renders() => Render<XFCheckbox>();
    [Fact] public void CheckBox_Value() => Render<XFCheckbox>(p => p.Add(x => x.Value, true));
    [Fact] public void Radio_Renders() => Render<XFRadio<string>>();
    [Fact] public void RadioGroup_Renders() => Render<XFRadioGroup<string>>();
    [Fact] public void Select_Renders() => Render<XFSelect<string>>();
    [Fact] public void Select_Value() => Render<XFSelect<string>>(p => p.Add(x => x.Value, "item1"));
    [Fact] public void MultiSelect_Renders() => Render<XFMultiSelect<string, string>>();
    [Fact] public void Combobox_Renders() => Render<XFCombobox<string, string>>();
    [Fact] public void NumberInput_Renders() => Render<XFNumberInput<decimal>>();
    [Fact] public void NumberInput_Value() => Render<XFNumberInput<decimal>>(p => p.Add(x => x.Value, 42));
    [Fact] public void InputMask_Renders() => Render<XFInputMask>();
    [Fact] public void InputMask_Mask() => Render<XFInputMask>(p => p.Add(x => x.Mask, "000.000.000-00"));
    [Fact] public void TimeInput_Renders() => Render<XFTimeInput>();
    [Fact] public void TimeSpan_Renders() => Render<XFTimeSpan>();
    [Fact] public void Upload_Renders() => Render<XFUpload>();
    [Fact] public void Upload_Multiple() => Render<XFUpload>(p => p.Add(x => x.Multiple, true));
    [Fact] public void OneTimeInput_Renders() => Render<XFOneTimeInput>();
    [Fact] public void TransferList_Renders() => Render<XFTransferList>();
    [Fact] public void SignaturePad_Renders() => Render<XFSignaturePad>();
    [Fact] public void Field_Renders() => Render<XFField>();
    [Fact] public void Search_Renders() => Render<XFSearch>();
    [Fact] public void RangeSlider_Renders() => Render<XFRangeSlider>();
    [Fact] public void Rating_Renders() => Render<XFRating>();
    [Fact] public void Rating_Value() => Render<XFRating>(p => p.Add(x => x.Value, 4));
    [Fact] public void FilterForm_Renders() => Render<HxFilterForm<object>>();
}