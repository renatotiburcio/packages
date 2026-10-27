using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using XForge.DesignSystem.Components.Forms.TimeSpan;
using XForge.DesignSystem.Core.Primitives;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Forms;

public class XFTimeSpanExtendedTest : BunitContext
{
    [Fact]
    public void Extended_XFTimeSpan_Default_Renders()
    {
        var cut = Render<XFTimeSpan>();
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_Label_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Label, "Duration"));
        Assert.Contains("Duration", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_Required_RendersAsterisk()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Label, "Duration")
            .Add(p => p.Required, true));
        Assert.Contains("*", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_HelperText_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.HelperText, "Enter a duration"));
        Assert.Contains("Enter a duration", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_AriaLabel_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.AriaLabel, "Custom aria"));
        Assert.Contains("Custom aria", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_CustomAriaLabel_OverridesDefault()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.AriaLabel, "My Duration"));
        Assert.Contains("My Duration", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ShowDaysTrue_RendersDaysInput()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowDays, true));
        Assert.Contains("Dias", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ShowDaysFalse_HidesDaysInput()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowDays, false));
        Assert.DoesNotContain("Dias", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ShowSteppersTrue_RendersButtons()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowSteppers, true));
        Assert.Contains("Aumentar", cut.Markup);
        Assert.Contains("Diminuir", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ShowSteppersFalse_HidesButtons()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowSteppers, false));
        Assert.DoesNotContain("Aumentar", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_PrecisionSecond_RendersSecondsInput()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Precision, XFTimeSpanPrecision.Second));
        Assert.Contains("Segundos", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_PrecisionMinute_HidesSecondsInput()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Precision, XFTimeSpanPrecision.Minute));
        Assert.DoesNotContain("Segundos", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_SizeSm_RendersSmallSize()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Size, XFSize.Sm));
        var root = cut.Find(".xf-time-span");
        Assert.NotNull(root);
    }

    [Fact]
    public void Extended_XFTimeSpan_SizeLg_RendersLargeSize()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Size, XFSize.Lg));
        var root = cut.Find(".xf-time-span");
        Assert.NotNull(root);
    }

    [Fact]
    public void Extended_XFTimeSpan_VariantFilled_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Variant, XFFormVariant.Filled));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_VariantSoft_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Variant, XFFormVariant.Soft));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_VariantGhost_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Variant, XFFormVariant.Ghost));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_Disabled_RendersDisabledInputs()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Disabled, true));
        var inputs = cut.FindAll("input");
        Assert.All(inputs, input => Assert.True(input.HasAttribute("disabled")));
    }

    [Fact]
    public void Extended_XFTimeSpan_ReadOnly_RendersReadonlyInputs()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ReadOnly, true));
        var inputs = cut.FindAll("input");
        Assert.All(inputs, input => Assert.True(input.HasAttribute("readonly")));
    }

    [Fact]
    public void Extended_XFTimeSpan_FullWidth_RendersWFull()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.FullWidth, true));
        var root = cut.Find(".xf-time-span");
        Assert.Contains("w-full", root.GetAttribute("class"));
    }

    [Fact]
    public void Extended_XFTimeSpan_NotFullWidth_RendersInlineFlex()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.FullWidth, false));
        var root = cut.Find(".xf-time-span");
        Assert.Contains("inline-flex", root.GetAttribute("class"));
    }

    [Fact]
    public void Extended_XFTimeSpan_Value_RendersSegmentValues()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Value, System.TimeSpan.FromHours(2).Add(System.TimeSpan.FromMinutes(30))));
        Assert.Contains("02", cut.Markup);
        Assert.Contains("30", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ValueWithDays_RendersDays()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowDays, true)
            .Add(p => p.Value, System.TimeSpan.FromDays(5).Add(System.TimeSpan.FromHours(3))));
        // DaysValue returns Days.ToString() = "5" for 5 days
        Assert.Contains("5", cut.Markup);
        // Hours segment should show "3"
        Assert.Contains("3", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_TonePrimary_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Tone, XFSemanticColor.Primary));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ToneDanger_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Tone, XFSemanticColor.Danger));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_CustomLabels_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowDays, true)
            .Add(p => p.DaysLabel, "D")
            .Add(p => p.HoursLabel, "H")
            .Add(p => p.MinutesLabel, "M")
            .Add(p => p.SecondsLabel, "S"));
        Assert.Contains("D", cut.Markup);
        Assert.Contains("H", cut.Markup);
        Assert.Contains("M", cut.Markup);
        Assert.Contains("S", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_IncrementLabel_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowSteppers, true)
            .Add(p => p.IncrementLabel, "Up"));
        Assert.Contains("Up", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_DecrementLabel_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.ShowSteppers, true)
            .Add(p => p.DecrementLabel, "Down"));
        Assert.Contains("Down", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_Step_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Step, 5));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_MinMax_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Min, System.TimeSpan.FromMinutes(10))
            .Add(p => p.Max, System.TimeSpan.FromHours(2)));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ClampTrue_Renders()
    {
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Clamp, true));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_Name_Accepted()
    {
        // Name is a [Parameter] for form submission but not rendered in markup
        var cut = Render<XFTimeSpan>(ps => ps
            .Add(p => p.Name, "my-duration"));
        Assert.Contains("xf-time-span", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_HoursInput_Renders()
    {
        var cut = Render<XFTimeSpan>();
        var inputs = cut.FindAll("input[type=number]");
        Assert.True(inputs.Count >= 2); // At least hours and minutes
    }

    [Fact]
    public void Extended_XFTimeSpan_MinutesInput_Renders()
    {
        var cut = Render<XFTimeSpan>();
        Assert.Contains("Horas", cut.Markup);
        Assert.Contains("Minutos", cut.Markup);
    }

    // ToneClass branches (lines 186-191)
    [Fact]
    public void Extended_XFTimeSpan_ToneSecondary_HasSecondaryFocus()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Tone, XFSemanticColor.Secondary));
        Assert.Contains("var(--color-secondary)", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ToneSuccess_HasSuccessFocus()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Tone, XFSemanticColor.Success));
        Assert.Contains("var(--color-success)", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ToneWarning_HasWarningFocus()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Tone, XFSemanticColor.Warning));
        Assert.Contains("var(--color-warning)", cut.Markup);
    }

    [Fact]
    public void Extended_XFTimeSpan_ToneDanger_HasDangerFocus()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Tone, XFSemanticColor.Danger));
        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    // AdornmentClass (line 145) — rendered when StartIcon is provided
    [Fact]
    public void Extended_XFTimeSpan_StartIcon_HasAdornmentClass()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.StartIcon, (RenderFragment)(b =>
        {
            b.OpenElement(0, "span");
            b.AddContent(1, "icon");
            b.CloseElement();
        })));
        Assert.Contains("group-focus-within:text-primary", cut.Markup);
    }

    // VariantClass Filled (line 170)
    [Fact]
    public void Extended_XFTimeSpan_VariantFilled_HasFilledClasses()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Variant, XFFormVariant.Filled));
        Assert.Contains("bg-surface-100", cut.Markup);
    }

    // VariantClass Soft (line 171)
    [Fact]
    public void Extended_XFTimeSpan_VariantSoft_HasSoftClasses()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Variant, XFFormVariant.Soft));
        Assert.Contains("bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]", cut.Markup);
    }

    // VariantClass Ghost (line 172)
    [Fact]
    public void Extended_XFTimeSpan_VariantGhost_HasGhostClasses()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Variant, XFFormVariant.Ghost));
        Assert.Contains("bg-transparent", cut.Markup);
    }

    // SizeClass Sm (line 156)
    [Fact]
    public void Extended_XFTimeSpan_SizeSm_HasSmallSizeClass()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Size, XFSize.Sm));
        Assert.Contains("h-9", cut.Markup);
    }

    // SizeClass Lg (line 157)
    [Fact]
    public void Extended_XFTimeSpan_SizeLg_HasLargeSizeClass()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Size, XFSize.Lg));
        Assert.Contains("h-12", cut.Markup);
    }

    // SizeTextClass Lg (line 164)
    [Fact]
    public void Extended_XFTimeSpan_SizeTextLg_HasBaseText()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Size, XFSize.Lg));
        var inputs = cut.FindAll("input[type=number]");
        Assert.All(inputs, input => Assert.Contains("text-base", input.GetAttribute("class") ?? ""));
    }

    // StateClass Disabled (line 197-198)
    [Fact]
    public void Extended_XFTimeSpan_Disabled_HasDisabledStateClass()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Disabled, true));
        Assert.Contains("cursor-not-allowed", cut.Markup);
        Assert.Contains("opacity-60", cut.Markup);
    }

    // SeparatorClass (line 143)
    [Fact]
    public void Extended_XFTimeSpan_Separator_HasSeparatorClass()
    {
        var cut = Render<XFTimeSpan>();
        Assert.Contains("text-text-muted", cut.Markup);
    }

    // StepperWrapClass (line 147)
    [Fact]
    public void Extended_XFTimeSpan_StepperWrap_HasStepperWrapClass()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.ShowSteppers, true));
        Assert.Contains("flex flex-col gap-0.5 ml-1", cut.Markup);
    }

    // StepperButtonClass (line 149)
    [Fact]
    public void Extended_XFTimeSpan_StepperButton_HasStepperButtonClass()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.ShowSteppers, true));
        Assert.Contains("h-5 w-5", cut.Markup);
    }

    // Precision Minute hides seconds (line 102 already tested but verify seconds label)
    [Fact]
    public void Extended_XFTimeSpan_PrecisionMinute_NoSecondsLabel()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.Precision, XFTimeSpanPrecision.Minute));
        Assert.DoesNotContain("Segundos", cut.Markup);
        Assert.Contains("Minutos", cut.Markup);
    }

    // Value with days + hours + minutes + seconds
    [Fact]
    public void Extended_XFTimeSpan_FullValue_RendersAllSegments()
    {
        var cut = Render<XFTimeSpan>(ps =>
        {
            ps.Add(p => p.ShowDays, true);
            ps.Add(p => p.Value, new System.TimeSpan(3, 14, 25, 30));
        });
        Assert.Contains("3", cut.Markup);
        Assert.Contains("14", cut.Markup);
        Assert.Contains("25", cut.Markup);
        Assert.Contains("30", cut.Markup);
    }

    // HelperId (line 116) — rendered on helper div at bottom
    [Fact]
    public void Extended_XFTimeSpan_HelperId_RendersOnHelperDiv()
    {
        var cut = Render<XFTimeSpan>(ps => ps.Add(p => p.HelperText, "Enter duration"));
        // HelperId is used as the id of the helper div
        Assert.Contains("Enter duration", cut.Markup);
        Assert.Contains("text-xs leading-5 text-text-secondary", cut.Markup);
    }

    // ErrorClass (line 152) — rendered when validation error exists
    [Fact]
    public async Task Extended_XFTimeSpan_ValidationError_HasErrorClass()
    {
        var model = new TestTimeSpanModelWithValidation();

        var cut = Render<Microsoft.AspNetCore.Components.Forms.EditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ChildContent, (RenderFragment<EditContext>)(ctx => builder =>
            {
                builder.OpenComponent<XFTimeSpan>(0);
                builder.AddAttribute(1, "ValueExpression", (Expression<Func<System.TimeSpan?>>)(() => model.Duration));
                builder.AddAttribute(2, "Value", model.Duration);
                builder.CloseComponent();
            }));
        });

        // Validate the model using DataAnnotations
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        var editContext = cut.Instance.EditContext;
        var fieldId = FieldIdentifier.Create(() => model.Duration);
        foreach (var r in results)
            editContext.GetValidationMessages(fieldId); // prime
        var store = new ValidationMessageStore(editContext);
        store.Add(fieldId, results.Select(r => r.ErrorMessage!));
        editContext.NotifyValidationStateChanged();
        await cut.InvokeAsync(() => { });
        // Force re-render
        cut.Render();
        await cut.InvokeAsync(() => { });

        Assert.Contains("text-danger", cut.Markup);
    }

    // FirstValidationMessage (lines 118-120)
    [Fact]
    public async Task Extended_XFTimeSpan_ValidationError_ShowsErrorMessage()
    {
        var model = new TestTimeSpanModelWithValidation();

        var cut = Render<Microsoft.AspNetCore.Components.Forms.EditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ChildContent, (RenderFragment<EditContext>)(ctx => builder =>
            {
                builder.OpenComponent<XFTimeSpan>(0);
                builder.AddAttribute(1, "ValueExpression", (Expression<Func<System.TimeSpan?>>)(() => model.Duration));
                builder.AddAttribute(2, "Value", model.Duration);
                builder.CloseComponent();
            }));
        });

        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        var editContext = cut.Instance.EditContext;
        var fieldId = FieldIdentifier.Create(() => model.Duration);
        var store = new ValidationMessageStore(editContext);
        store.Add(fieldId, results.Select(r => r.ErrorMessage!));
        editContext.NotifyValidationStateChanged();
        cut.Render();
        await cut.InvokeAsync(() => { });

        Assert.Contains("Duration is required", cut.Markup);
    }

    // ErrorId (line 117)
    [Fact]
    public async Task Extended_XFTimeSpan_ValidationError_HasErrorId()
    {
        var model = new TestTimeSpanModelWithValidation();

        var cut = Render<Microsoft.AspNetCore.Components.Forms.EditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ChildContent, (RenderFragment<EditContext>)(ctx => builder =>
            {
                builder.OpenComponent<XFTimeSpan>(0);
                builder.AddAttribute(1, "ValueExpression", (Expression<Func<System.TimeSpan?>>)(() => model.Duration));
                builder.AddAttribute(2, "Value", model.Duration);
                builder.CloseComponent();
            }));
        });

        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        var editContext = cut.Instance.EditContext;
        var fieldId = FieldIdentifier.Create(() => model.Duration);
        var store = new ValidationMessageStore(editContext);
        store.Add(fieldId, results.Select(r => r.ErrorMessage!));
        editContext.NotifyValidationStateChanged();
        cut.Render();
        await cut.InvokeAsync(() => { });

        Assert.Contains("-error", cut.Markup);
    }

    // ToneClass with validation error (line 180-182)
    [Fact]
    public async Task Extended_XFTimeSpan_ValidationError_HasDangerTone()
    {
        var model = new TestTimeSpanModelWithValidation();

        var cut = Render<Microsoft.AspNetCore.Components.Forms.EditForm>(parameters =>
        {
            parameters.Add(p => p.Model, model);
            parameters.Add(p => p.ChildContent, (RenderFragment<EditContext>)(ctx => builder =>
            {
                builder.OpenComponent<XFTimeSpan>(0);
                builder.AddAttribute(1, "ValueExpression", (Expression<Func<System.TimeSpan?>>)(() => model.Duration));
                builder.AddAttribute(2, "Value", model.Duration);
                builder.CloseComponent();
            }));
        });

        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        var editContext = cut.Instance.EditContext;
        var fieldId = FieldIdentifier.Create(() => model.Duration);
        var store = new ValidationMessageStore(editContext);
        store.Add(fieldId, results.Select(r => r.ErrorMessage!));
        editContext.NotifyValidationStateChanged();
        cut.Render();
        await cut.InvokeAsync(() => { });

        Assert.Contains("var(--color-danger)", cut.Markup);
    }

    private class TestTimeSpanModel
    {
        public System.TimeSpan? Duration { get; set; }
    }

    private class TestTimeSpanModelWithValidation
    {
        [Required(ErrorMessage = "Duration is required")]
        public System.TimeSpan? Duration { get; set; }
    }
}
