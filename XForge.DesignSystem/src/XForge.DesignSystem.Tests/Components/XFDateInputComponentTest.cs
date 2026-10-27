using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.DateInput;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFDateInputComponentTest : BunitContext
{
    [Fact]
    public void Render_WithLabelHelperAndConstraints()
    {
        var value = new DateOnly(2026, 5, 6);
        var cut = Render<XFDateInput>(parameters =>
        {
            parameters.Add(p => p.Label, "Due date");
            parameters.Add(p => p.HelperText, "Choose a working day.");
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.Min, new DateOnly(2026, 5, 1));
            parameters.Add(p => p.Max, new DateOnly(2026, 5, 31));
        });

        Assert.Contains("xf-date-input", cut.Markup);
        Assert.Contains("Due date", cut.Markup);
        Assert.Contains("Choose a working day.", cut.Markup);
        Assert.Contains("value=\"2026-05-06\"", cut.Markup);
        Assert.Contains("min=\"2026-05-01\"", cut.Markup);
        Assert.Contains("max=\"2026-05-31\"", cut.Markup);
    }

    [Fact]
    public void Change_WhenDateSelected_UpdatesValue()
    {
        DateOnly? value = null;
        var cut = Render<XFDateInput>(parameters =>
        {
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, next => value = next));
        });

        cut.Find("input").Change("2026-05-20");

        Assert.Equal(new DateOnly(2026, 5, 20), value);
    }

    [Fact]
    public void Change_WhenAboveMax_ClampsValue()
    {
        DateOnly? value = null;
        var cut = Render<XFDateInput>(parameters =>
        {
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, next => value = next));
            parameters.Add(p => p.Max, new DateOnly(2026, 5, 10));
        });

        cut.Find("input").Change("2026-05-20");

        Assert.Equal(new DateOnly(2026, 5, 10), value);
    }

    [Fact]
    public void Input_WhenImmediate_UpdatesValue()
    {
        DateOnly? value = null;
        var cut = Render<XFDateInput>(parameters =>
        {
            parameters.Add(p => p.Immediate, true);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, next => value = next));
        });

        cut.Find("input").Input("2026-05-21");

        Assert.Equal(new DateOnly(2026, 5, 21), value);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFDateInput>(parameters =>
        {
            parameters.Add(p => p.Variant, XFFormVariant.Soft);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
            parameters.Add(p => p.PrefixText, "Ref.");
            parameters.Add(p => p.SuffixText, "BR");
        });

        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("focus-within:border-[var(--color-warning)]", cut.Markup);
        Assert.Contains("Ref.", cut.Markup);
        Assert.Contains("BR", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaValueTextFormat_UsesLocalizedAccessibleDate()
    {
        var cut = Render<XFDateInput>(parameters =>
        {
            parameters.Add(p => p.Value, new DateOnly(2026, 5, 6));
            parameters.Add(p => p.CultureName, "pt-BR");
            parameters.Add(p => p.AriaValueTextFormat, "Data {0}");
        });

        var input = cut.Find("input");

        Assert.Equal("2026-05-06", input.GetAttribute("value"));
        Assert.Equal("Data 06/05/2026", input.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new DateModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.DueDate);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Date is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFDateInput>(3);
                childBuilder.AddAttribute(4, "Value", model.DueDate);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<DateOnly?>(this, value => model.DueDate = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<DateOnly?>>)(() => model.DueDate));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Date is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)]", cut.Markup);
    }

    private sealed class DateModel
    {
        [Required]
        public DateOnly? DueDate { get; set; }
    }
}
