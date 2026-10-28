using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.TimeInput;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFTimeInputComponentTest : XForgeTestContext
{
    [Fact]
    public void Render_WithLabelHelperAndConstraints()
    {
        var cut = Render<XFTimeInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Start time");
            parameters.Add(p => p.HelperText, "Choose a time.");
            parameters.Add(p => p.Value, new TimeOnly(8, 30));
            parameters.Add(p => p.Min, new TimeOnly(8, 0));
            parameters.Add(p => p.Max, new TimeOnly(18, 0));
            parameters.Add(p => p.Step, 900);
        });

        Assert.Contains("xf-time-input", cut.Markup);
        Assert.Contains("Start time", cut.Markup);
        Assert.Contains("Choose a time.", cut.Markup);
        Assert.Contains("value=\"08:30\"", cut.Markup);
        Assert.Contains("min=\"08:00\"", cut.Markup);
        Assert.Contains("max=\"18:00\"", cut.Markup);
        Assert.Contains("step=\"900\"", cut.Markup);
    }

    [Fact]
    public void Change_WhenTimeSelected_UpdatesValue()
    {
        TimeOnly? value = null;
        var cut = Render<XFTimeInput>(parameters =>
        {
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, next => value = next));
        });

        cut.Find("input").Change("10:15");

        Assert.Equal(new TimeOnly(10, 15), value);
    }

    [Fact]
    public void Change_WhenAboveMax_ClampsValue()
    {
        TimeOnly? value = null;
        var cut = Render<XFTimeInput>(parameters =>
        {
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, next => value = next));
            parameters.Add(p => p.Max, new TimeOnly(18, 0));
        });

        cut.Find("input").Change("22:30");

        Assert.Equal(new TimeOnly(18, 0), value);
    }

    [Fact]
    public void Render_WithSecondPrecision_UsesSecondFormatAndDefaultStep()
    {
        var cut = Render<XFTimeInput>(parameters =>
        {
            parameters.Add(p => p.Precision, XFTimeInputPrecision.Second);
            parameters.Add(p => p.Value, new TimeOnly(10, 15, 30));
        });

        Assert.Contains("value=\"10:15:30\"", cut.Markup);
        Assert.Contains("step=\"1\"", cut.Markup);
    }

    [Fact]
    public void Input_WhenImmediate_UpdatesValue()
    {
        TimeOnly? value = null;
        var cut = Render<XFTimeInput>(parameters =>
        {
            parameters.Add(p => p.Immediate, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, next => value = next));
        });

        cut.Find("input").Input("09:45");

        Assert.Equal(new TimeOnly(9, 45), value);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFTimeInput>(parameters =>
        {
            parameters.Add(p => p.Variant, XFFormVariant.Soft);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.Add(p => p.PrefixText, "UTC");
            parameters.Add(p => p.SuffixText, "-03");
        });

        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("focus-within:border-[var(--color-warning)]", cut.Markup);
        Assert.Contains("UTC", cut.Markup);
        Assert.Contains("-03", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaValueTextFormat_UsesLocalizedAccessibleTime()
    {
        var cut = Render<XFTimeInput>(parameters =>
        {
            parameters.Add(p => p.Value, new TimeOnly(8, 30));
            parameters.Add(p => p.CultureName, "pt-BR");
            parameters.Add(p => p.AriaValueTextFormat, "Horario {0}");
        });

        var input = cut.Find("input");

        Assert.Equal("08:30", input.GetAttribute("value"));
        Assert.Equal("Horario 08:30", input.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new TimeModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.StartTime);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Time is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFTimeInput>(3);
                childBuilder.AddAttribute(4, "Value", model.StartTime);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<TimeOnly?>(this, value => model.StartTime = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<TimeOnly?>>)(() => model.StartTime));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Time is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    private sealed class TimeModel
    {
        [Required]
        public TimeOnly? StartTime { get; set; }
    }
}
