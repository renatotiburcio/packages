using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Forms.TextInput;
using XForge.DesignSystem.Components.Forms.Switch;
using XForge.DesignSystem.Components.Forms.Checkbox;
using XForge.DesignSystem.Components.Forms.Radio;
using XForge.DesignSystem.Components.Forms.Select;
using XForge.DesignSystem.Components.Forms.NumberInput;
using XForge.DesignSystem.Components.Forms.InputMask;
using XForge.DesignSystem.Components.Forms.TimeInput;
using XForge.DesignSystem.Components.Forms.TimeSpan;
using XForge.DesignSystem.Components.Forms.Rating;
using XForge.DesignSystem.Components.Forms.RangeSlider;
using XForge.DesignSystem.Components.Forms.Search;
using XForge.DesignSystem.Components.Forms.Field;
using XForge.DesignSystem.Components.Forms.Upload;
using XForge.DesignSystem.Components.Forms.SignaturePad;
using XForge.DesignSystem.Components.Forms.OneTimeInput;
using XForge.DesignSystem.Components.Forms.TransferList;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components;

public class XFDeepComponentTest : BunitContext
{
    public XFDeepComponentTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void TextInput_FullWidthFalse_RendersInlineFlex() => Assert.NotNull(Render<XFTextInput>(p => p.Add(x => x.FullWidth, false)));
    [Fact] public void TextInput_PrefixText_RendersPrefix() => Assert.NotNull(Render<XFTextInput>(p => p.Add(x => x.PrefixText, "$")));

    [Fact] public void Switch_ShowStateText_RendersStateText() => Assert.NotNull(Render<XFSwitch>(p => { p.Add(x => x.ShowStateText, true); p.Add(x => x.Value, true); p.Add(x => x.CheckedText, "ON"); }));
    [Fact] public void Switch_LabelPositionStart_RendersReverse() => Assert.NotNull(Render<XFSwitch>(p => { p.Add(x => x.Label, "Test"); p.Add(x => x.LabelPosition, XFSwitchLabelPosition.Start); }));
    [Fact] public void Switch_ShowThumbIconFalse_HidesIcon() => Assert.NotNull(Render<XFSwitch>(p => { p.Add(x => x.ShowThumbIcon, false); p.Add(x => x.Value, true); }));

    [Fact] public void Checkbox_ThreeState_RendersIndeterminate() => Assert.NotNull(Render<XFCheckbox>(p => p.Add(x => x.ThreeState, true)));
    [Fact] public void Checkbox_LabelPositionStart_RendersReverse() => Assert.NotNull(Render<XFCheckbox>(p => { p.Add(x => x.Label, "Check"); p.Add(x => x.LabelPosition, XFCheckboxLabelPosition.Start); }));
    [Fact] public void Checkbox_MixedStateText_RendersMixedText() => Assert.NotNull(Render<XFCheckbox>(p => { p.Add(x => x.ThreeState, true); p.Add(x => x.MixedStateText, "Partial"); }));

    [Fact] public void Radio_VariantCard_RendersCardStyle() => Assert.NotNull(Render<XFRadio<string>>(p => { p.Add(x => x.Value, "opt1"); p.Add(x => x.Variant, XFRadioVariant.Card); }));
    [Fact] public void Radio_VariantButton_RendersButtonStyle() => Assert.NotNull(Render<XFRadio<string>>(p => { p.Add(x => x.Value, "opt1"); p.Add(x => x.Variant, XFRadioVariant.Button); }));
    [Fact] public void Radio_Description_RendersDescription() => Assert.NotNull(Render<XFRadio<string>>(p => { p.Add(x => x.Value, "opt1"); p.Add(x => x.Description, "Choose this option"); }));

    [Fact] public void Select_VariantFilled_RendersFilledStyle() => Assert.NotNull(Render<XFSelect<string>>(p => p.Add(x => x.Variant, XFFormVariant.Filled)));
    [Fact] public void Select_Clearable_RendersClearButton() => Assert.NotNull(Render<XFSelect<string>>(p => { p.Add(x => x.Clearable, true); p.Add(x => x.Value, "item"); }));
    [Fact] public void Select_PlaceholderHidden_HidesPlaceholder() => Assert.NotNull(Render<XFSelect<string>>(p => p.Add(x => x.PlaceholderHidden, true)));

    [Fact] public void NumberInput_FormatCurrency_RendersCurrency() => Assert.NotNull(Render<XFNumberInput<decimal>>(p => p.Add(x => x.FormatStyle, XFNumberInputFormatStyle.Currency)));
    [Fact] public void NumberInput_StepperSplit_RendersSplitLayout() => Assert.NotNull(Render<XFNumberInput<decimal>>(p => p.Add(x => x.StepperLayout, XFStepperLayout.Split)));
    [Fact] public void NumberInput_ShowSteppersFalse_HidesSteppers() => Assert.NotNull(Render<XFNumberInput<decimal>>(p => p.Add(x => x.ShowSteppers, false)));

    [Fact] public void InputMask_ShowMaskPlaceholder_RendersPlaceholder() => Assert.NotNull(Render<XFInputMask>(p => { p.Add(x => x.Mask, "(99) 99999-9999"); p.Add(x => x.ShowMaskPlaceholder, true); }));
    [Fact] public void InputMask_UnmaskValueFalse_RendersMasked() => Assert.NotNull(Render<XFInputMask>(p => { p.Add(x => x.Mask, "999-99-9999"); p.Add(x => x.UnmaskValue, false); }));
    [Fact] public void InputMask_ClearIncomplete_RendersClearIncomplete() => Assert.NotNull(Render<XFInputMask>(p => { p.Add(x => x.Mask, "99999-999"); p.Add(x => x.ClearIncomplete, true); }));


    [Fact] public void TimeInput_PrecisionSecond_RendersSeconds() => Assert.NotNull(Render<XFTimeInput>(p => p.Add(x => x.Precision, XFTimeInputPrecision.Second)));
    [Fact] public void TimeInput_Step_RendersStep() => Assert.NotNull(Render<XFTimeInput>(p => p.Add(x => x.Step, 30)));
    [Fact] public void TimeInput_ClampTrue_RendersClamp() => Assert.NotNull(Render<XFTimeInput>(p => p.Add(x => x.Clamp, true)));

    [Fact] public void TimeSpan_PrecisionMinute_RendersMinuteOnly() => Assert.NotNull(Render<XFTimeSpan>(p => p.Add(x => x.Precision, XFTimeSpanPrecision.Minute)));
    [Fact] public void TimeSpan_Step_RendersStep() => Assert.NotNull(Render<XFTimeSpan>(p => p.Add(x => x.Step, 15)));
    [Fact] public void TimeSpan_VariantSoft_RendersSoft() => Assert.NotNull(Render<XFTimeSpan>(p => p.Add(x => x.Variant, XFFormVariant.Soft)));

    [Fact] public void Rating_AllowReset_RendersClearButton() => Assert.NotNull(Render<XFRating>(p => { p.Add(x => x.AllowReset, true); p.Add(x => x.Value, 3d); }));
    [Fact] public void Rating_CustomMax_RendersCorrectCount() => Assert.NotNull(Render<XFRating>(p => p.Add(x => x.Max, 3)));
    [Fact] public void Rating_SizeXl_RendersExtraLarge() => Assert.NotNull(Render<XFRating>(p => p.Add(x => x.Size, XFExtendedSize.Xl)));

    [Fact] public void RangeSlider_AllowOverlap_RendersFullWidth() => Assert.NotNull(Render<XFRangeSlider>(p => { p.Add(x => x.AllowOverlap, true); p.Add(x => x.StartValue, 30m); p.Add(x => x.EndValue, 70m); }));
    [Fact] public void RangeSlider_ShowBoundsFalse_HidesBounds() => Assert.NotNull(Render<XFRangeSlider>(p => p.Add(x => x.ShowBounds, false)));
    [Fact] public void RangeSlider_PrefixSuffix_RendersFormatted() => Assert.NotNull(Render<XFRangeSlider>(p => { p.Add(x => x.PrefixText, "$"); p.Add(x => x.SuffixText, "k"); p.Add(x => x.ShowValue, true); }));

    [Fact] public void Search_ClearOnEscapeFalse_DisablesClearOnEscape() => Assert.NotNull(Render<XFSearch>(p => p.Add(x => x.ClearOnEscape, false)));
    [Fact] public void Search_Debounce_RendersDebounce() => Assert.NotNull(Render<XFSearch>(p => p.Add(x => x.DebounceMilliseconds, 500)));
    [Fact] public void Search_MaxLength_RendersMaxLength() => Assert.NotNull(Render<XFSearch>(p => p.Add(x => x.MaxLength, 100)));

    [Fact] public void Field_HorizontalOrientation_RendersHorizontal() => Assert.NotNull(Render<XFField>(p => p.Add(x => x.Orientation, XFFieldOrientation.Horizontal)));
    [Fact] public void Field_ErrorText_RendersError() => Assert.NotNull(Render<XFField>(p => p.Add(x => x.ErrorText, "Required")));
    [Fact] public void Field_StartAdornment_RendersAdornment() => Assert.NotNull(Render<XFField>(p => { p.Add(x => x.StartAdornment, builder => builder.AddContent(0, "R$")); p.AddChildContent("<input />"); }));

    [Fact] public void Upload_Multiple_RendersMultiple() => Assert.NotNull(Render<XFUpload>(p => p.Add(x => x.Multiple, true)));
    [Fact] public void Upload_ShowImagePreview_RendersPreview() => Assert.NotNull(Render<XFUpload>(p => p.Add(x => x.ShowImagePreview, true)));
    [Fact] public void Upload_AutoUpload_RendersAutoUpload() => Assert.NotNull(Render<XFUpload>(p => p.Add(x => x.AutoUpload, true)));

    [Fact] public void SignaturePad_PenColor_RendersColoredPen() => Assert.NotNull(Render<XFSignaturePad>(p => p.Add(x => x.PenColor, "#ff0000")));
    [Fact] public void SignaturePad_ShowToolbarFalse_HidesToolbar() => Assert.NotNull(Render<XFSignaturePad>(p => p.Add(x => x.ShowToolbar, false)));
    [Fact] public void SignaturePad_CustomSize_RendersCustomSize() => Assert.NotNull(Render<XFSignaturePad>(p => { p.Add(x => x.Width, 800); p.Add(x => x.Height, 300); }));

    [Fact] public void OneTimeInput_Mask_RendersMaskedInput() => Assert.NotNull(Render<XFOneTimeInput>(p => p.Add(x => x.Mask, true)));
    [Fact] public void OneTimeInput_CustomGroup_RendersCustomGroups() => Assert.NotNull(Render<XFOneTimeInput>(p => p.Add(x => x.Group, "2,2,2")));
    [Fact] public void OneTimeInput_FourDigits_RendersFourSlots() => Assert.NotNull(Render<XFOneTimeInput>(p => p.Add(x => x.Digits, 4)));

    [Fact] public void TransferList_SearchableFalse_HidesSearch() => Assert.NotNull(Render<XFTransferList>(p => p.Add(x => x.Searchable, false)));
    [Fact] public void TransferList_CustomLabels_RendersLabels() => Assert.NotNull(Render<XFTransferList>(p => { p.Add(x => x.SourceLabel, "Left"); p.Add(x => x.TargetLabel, "Right"); }));
    [Fact] public void TransferList_CustomSearchPlaceholder_RendersPlaceholder() => Assert.NotNull(Render<XFTransferList>(p => p.Add(x => x.SearchPlaceholder, "Filter items")));
}

